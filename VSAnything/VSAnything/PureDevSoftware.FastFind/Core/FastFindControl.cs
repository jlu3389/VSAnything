using SCLCoreCLR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Company.VSAnything
{
	internal class FastFindControl : UserControl
	{
		public delegate void ControlWantsToCloseHandler();

		private class GetOpenFilesResults
		{
			public List<string> m_OpenFiles;

			public List<UnsavedDocument> m_UnsavedDocuments;
		}

		private struct FileAndLine
		{
			public string m_Filename;

			public int m_Line;

			public FileAndLine(string filename)
			{
				this.m_Filename = filename;
				this.m_Line = -1;
			}

			public FileAndLine(string filename, int line)
			{
				this.m_Filename = filename;
				this.m_Line = line;
			}
		}

		private class FilterComparer : IComparer<FindFileResult>
		{
			private string m_Filter;

			public FilterComparer(string filter)
			{
				this.m_Filter = filter.ToLower();
			}

			public int Compare(FindFileResult a, FindFileResult b)
			{
				int result;
				try
				{
					bool a_starts_with_filter = a.m_FileName.ToLower().StartsWith(this.m_Filter);
					bool b_starts_with_filter = b.m_FileName.ToLower().StartsWith(this.m_Filter);
					if (a_starts_with_filter && !b_starts_with_filter)
					{
						result = -1;
					}
					else if (b_starts_with_filter && !a_starts_with_filter)
					{
						result = 1;
					}
					else
					{
						result = string.Compare(a.m_FileName, b.m_FileName);
					}
				}
				catch (Exception arg_56_0)
				{
					Utils.LogExceptionQuiet(arg_56_0);
					result = 0;
				}
				return result;
			}
		}

		private class FindFilesFinishedResults
		{
			public List<FindFileResult> m_Files;

			public string m_TextBoxValue;
		}

		private struct LogicalOperator
		{
			public Pattern.Operator m_Operator;

			public string m_Name;

			public LogicalOperator(Pattern.Operator op, string name)
			{
				this.m_Operator = op;
				this.m_Name = name;
			}
		}

		private const int m_MinBusyTimeForProgressBar = 500;

		private const int m_MouseWheelJump = 3;

		private const int m_MaxPreviousSearches = 10;

		private DTE m_DTE;

		private SolutionFiles m_SolutionFiles;

		private FileFinder m_FileFinder;

		private TextFinder m_TextFinder;

		private GetOpenFilesThread m_GetOpenFilesThread;

		private Settings m_Settings;

		private bool m_Disposed;

		private bool m_IsModal;

		private Set<string> m_ExtToScan = new Set<string>();

		private ControlTaskDispatcher m_ControlTaskDispatcher = new ControlTaskDispatcher();

		private OldItemsDropDownForm m_OldItemsDropDownForm;

		private FastFindProgressBar m_ProgressBar = new FastFindProgressBar();

		private string m_LastFindString;

		private string m_SolutionFilename;

		private List<string> m_AllSolutionFiles = new List<string>();

		private List<string> m_FilesToSearch = new List<string>();

		private Set<string> m_FilesToSearchSet = new Set<string>();

		private List<FindFileResult> m_FindFileResults = new List<FindFileResult>();

		private List<TextFinderResult> m_TextFinderResults = new List<TextFinderResult>();

		private System.Windows.Forms.Timer m_UpdateTimer = new System.Windows.Forms.Timer();

		private const int m_UpdateTimerTickInterval = 10;

		private int m_BusyStartTime = -1;

		private int m_LastTextBoxChangedTime;

		private bool m_IgnoreTextBoxTextChanges;

		private System.Windows.Forms.Timer m_SelectTextTimer = new System.Windows.Forms.Timer();

		private int m_OldSearchesListBoxCloseTime;

		private bool m_IgnoreOldsearchesButtonClick;

		private static List<string> m_OpenFiles = new List<string>();

		private static List<UnsavedDocument> m_UnsavedDocuments = new List<UnsavedDocument>();

		private List<UnsavedDocument> m_UnsavedDocumentsToSearch = new List<UnsavedDocument>();

		private object m_InitialSelectedItem;

		private static FastFindControl.LogicalOperator[] m_LogicalOperators = new FastFindControl.LogicalOperator[]
		{
			new FastFindControl.LogicalOperator(Pattern.Operator.AND_NOT, " AND NOT "),
			new FastFindControl.LogicalOperator(Pattern.Operator.AND, " AND "),
			new FastFindControl.LogicalOperator(Pattern.Operator.OR, " OR ")
		};

		private const string m_MatchWholeWordOperator = " WORD";

		private const string m_MatchCaseOperator = " CASE";

		private static string m_TipLinkValue;

		private bool m_SelectTextOnNextFocus = true;

		private IContainer components;

		private ComboBox m_FileExtComboBox;

		private CheckBox m_FindTextCheckBox;

		private MyListBox m_ListBox;

		private Panel m_OptionsPanel;

		private Label label1;

		private CheckBox m_WildcardsCheckBox;

		private CheckBox m_FilesCheckBox;

		private Panel m_BottomPanel;

		private FastFindTextBox m_TextBox;

		private Button m_CancelButton;

		private Button m_OpenButton;

		private Button m_OptionsButton;

		private Panel m_TextBoxPanel;

		private CheckBox m_SolutionFilesMatchCaseCheckBox;

		private CheckBox m_FindTextMatchCaseCheckBox;

		private Label m_FullPathTextBox;

		private Panel m_TextBoxBorderPanel;

		private Button button1;

		private Button button2;

		private CheckBox m_LogicalOperatorsCheckBox;

		private Panel m_TipsPanel;

		private Button button4;

		private Button button3;

		private Panel panel1;

		private Label label2;

		private Label m_TipLabel;

		private Label m_TipLinkLabel;

		private CheckBox m_WholeWordCheckbox;

		private Button button5;

		private Button button6;

		private Button button7;

		private CheckBox m_RegExpCheckBox;

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event FastFindControl.ControlWantsToCloseHandler ControlWantsToClose;

		private bool IsEnteringPath
		{
			get
			{
				string path = this.m_TextBox.Text.ToLower();
				char[] invalidPathChars = Path.GetInvalidPathChars();
				for (int i = 0; i < invalidPathChars.Length; i++)
				{
					char c = invalidPathChars[i];
					if (path.Contains(c))
					{
						return false;
					}
				}
				if (path.Length < "e:\\".Length)
				{
					return false;
				}
				if (path[0] < 'a' || path[0] > 'z')
				{
					return false;
				}
				if (path[1] != ':')
				{
					return false;
				}
				if (path[2] != '\\')
				{
					return false;
				}
				if (!Path.IsPathRooted(path))
				{
					return false;
				}
				if (path.Length == 3)
				{
					return true;
				}
				bool result;
				try
				{
					result = Directory.Exists(Path.GetDirectoryName(path));
				}
				catch (Exception)
				{
					result = false;
				}
				return result;
			}
		}

		public string TextBoxText
		{
			get
			{
				return this.m_TextBox.Text;
			}
			set
			{
				this.m_TextBox.Text = value;
			}
		}

		public object SelectedItem
		{
			get
			{
				return this.m_ListBox.SelectedItem;
			}
		}

		public FastFindControl(DTE dte, SolutionFiles solution_files, FileFinder file_finder, TextFinder text_finder, GetOpenFilesThread get_open_files_thread, Settings settings, string initial_text, object initial_selected_item, bool is_modal)
		{
			ProfileTimer arg_429_0 = new ProfileTimer("FastFindControl Constructor");
			this.InitializeComponent();
			this.m_DTE = dte;
			this.m_SolutionFiles = solution_files;
			this.m_FileFinder = file_finder;
			this.m_TextFinder = text_finder;
			this.m_GetOpenFilesThread = get_open_files_thread;
			this.m_Settings = settings;
			this.m_IsModal = is_modal;
			AppearanceDialogPage appearance_settings = VSAnythingPackage.Inst.GetAppearanceDialogPage();
			this.m_OptionsPanel.Visible = this.m_Settings.GetOptionsPanelVisible(is_modal);
			this.m_FileExtComboBox.Text = this.m_Settings.GetFastFindFileExt(is_modal);
			Font font = new Font(appearance_settings.FontName, appearance_settings.FontSize);
			this.BackColor = appearance_settings.Colours.m_ControlColour;
			this.ForeColor = Utils.ModifyColour(this.BackColor, 100);
			this.m_ListBox.Font = font;
			this.m_ListBox.BackColor = appearance_settings.Colours.m_BackColour;
			this.m_ListBox.ForeColor = appearance_settings.Colours.m_ForeColour;
			this.m_ListBox.SetupBrushes(settings);
			this.m_ListBox.Settings = settings;
			this.m_TextBox.Font = font;
			this.m_TextBox.BackColor = appearance_settings.Colours.m_BackColour;
			this.m_TextBox.ForeColor = appearance_settings.Colours.m_ForeColour;
			this.m_TextBoxBorderPanel.BackColor = appearance_settings.Colours.m_BackColour;
			this.m_TextBoxPanel.Size = new Size(this.m_TextBoxPanel.Width, font.Height + 8);
			this.m_FileExtComboBox.BackColor = appearance_settings.Colours.m_BackColour;
			this.m_FileExtComboBox.ForeColor = appearance_settings.Colours.m_ForeColour;
			this.m_FilesCheckBox.Checked = this.m_Settings.GetFastFindShowFiles(is_modal);
			this.m_FindTextCheckBox.Checked = this.m_Settings.GetFastFindFindText(is_modal);
			this.m_WholeWordCheckbox.Checked = this.m_Settings.GetMatchWholeWord(is_modal);
			this.m_RegExpCheckBox.Checked = this.m_Settings.GetRegExpression(is_modal);
			this.m_WildcardsCheckBox.Checked = this.m_Settings.GetFastFindWildcards(is_modal);
			this.m_FindTextMatchCaseCheckBox.Checked = this.m_Settings.GetFindTextMatchCase(is_modal);
			this.m_LogicalOperatorsCheckBox.Checked = this.m_Settings.GetUseLogicalOperators(is_modal);
			this.m_SolutionFilesMatchCaseCheckBox.Checked = this.m_Settings.GetSolutionFilesMatchCase(is_modal);
			this.m_FullPathTextBox.Text = "";
			this.InitialiseTipsPanel();
			this.m_UpdateTimer.Tick += new EventHandler(this.TimerTick);
			this.m_UpdateTimer.Interval = 10;
			this.m_SelectTextTimer.Interval = 5;
			this.m_SelectTextTimer.Tick += new EventHandler(this.SelectTextTimerTick);
			this.m_ProgressBar.Colour = appearance_settings.Colours.m_SelectColour;
			this.m_ProgressBar.Size = new Size(base.ClientSize.Width, 2);
			this.m_ProgressBar.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.m_BottomPanel.Controls.Add(this.m_ProgressBar);
			this.m_ProgressBar.Visible = false;
			if (!is_modal)
			{
				this.m_BottomPanel.Visible = false;
			}
			if (!string.IsNullOrEmpty(initial_text))
			{
				this.m_IgnoreTextBoxTextChanges = true;
				this.m_TextBox.Text = initial_text;
				this.m_InitialSelectedItem = initial_selected_item;
				this.m_IgnoreTextBoxTextChanges = false;
				this.m_TextBox.SelectAll();
			}
			this.m_TextBox.Focus();
			this.HookEvents();
			this.SetSolutionFilename(this.m_SolutionFiles.SolutionFilename);
			this.UpdateExtToScan();
			this.UpdateFilesToSearchAndRefresh();
			arg_429_0.Stop();
		}

		private void SolutionFilenameChanged(string solution_filename)
		{
			this.m_ControlTaskDispatcher.QueueTask(new ControlTaskDispatcher.TaskFunction1Arg(this.SolutionFilenameChanged_MainThread), solution_filename);
		}

		private void SolutionFilenameChanged_MainThread(object arg)
		{
			string solution_filename = (string)arg;
			this.SetSolutionFilename(solution_filename);
			this.UpdateFilesToSearchAndRefresh();
		}

		private void SetSolutionFilename(string solution_filename)
		{
			if (this.m_SolutionFilename != solution_filename)
			{
				this.m_SolutionFilename = solution_filename;
				this.m_AllSolutionFiles = new List<string>(this.m_SolutionFiles.Files);
			}
		}

		private void HookEvents()
		{
			base.Disposed += new EventHandler(this.OnDispose);
			this.m_GetOpenFilesThread.Finished += new GetOpenFilesThread.FinishedHandler(this.GetOpenFilesFinished);
			this.m_TextFinder.ScanFinished += new ScanFinishedHandler(this.ScanFinished);
			this.m_SolutionFiles.SolutionFilenameChanged += new SolutionFiles.SolutionFilenameChangedHandler(this.SolutionFilenameChanged);
			this.m_SolutionFiles.SolutionFileListChanged += new SolutionFiles.SolutionFileListChangedHandler(this.SolutionFileListChanged);
			this.m_TextBox.MouseDown += new MouseEventHandler(this.TextBoxMouseDown);
			this.m_TextBox.GotFocus += new EventHandler(this.TextBoxGotFocus);
			if (!this.m_IsModal)
			{
				this.RecursiveHookFocusEvents(this);
			}
		}

		private void UnhookEvents()
		{
			base.Disposed -= new EventHandler(this.OnDispose);
			this.m_GetOpenFilesThread.Finished -= new GetOpenFilesThread.FinishedHandler(this.GetOpenFilesFinished);
			this.m_TextFinder.ScanFinished -= new ScanFinishedHandler(this.ScanFinished);
			this.m_SolutionFiles.SolutionFilenameChanged -= new SolutionFiles.SolutionFilenameChangedHandler(this.SolutionFilenameChanged);
			this.m_SolutionFiles.SolutionFileListChanged -= new SolutionFiles.SolutionFileListChangedHandler(this.SolutionFileListChanged);
			this.m_TextBox.MouseDown -= new MouseEventHandler(this.TextBoxMouseDown);
			this.m_TextBox.GotFocus -= new EventHandler(this.TextBoxGotFocus);
			if (!this.m_IsModal)
			{
				this.RecursiveUnhookFocusEvents(this);
			}
		}

		private void RecursiveHookFocusEvents(Control control)
		{
			control.GotFocus += new EventHandler(this.ControlGotFocus);
			control.LostFocus += new EventHandler(this.ControlLostFocus);
			foreach (Control child in control.Controls)
			{
				this.RecursiveHookFocusEvents(child);
			}
		}

		private void RecursiveUnhookFocusEvents(Control control)
		{
			control.GotFocus += new EventHandler(this.ControlGotFocus);
			control.LostFocus += new EventHandler(this.ControlLostFocus);
			foreach (Control child in control.Controls)
			{
				this.RecursiveUnhookFocusEvents(child);
			}
		}

		public void FocusTextBox()
		{
			this.StartGettingOpenFiles();
			this.m_TextBox.SelectAll();
			this.m_TextBox.Focus();
		}

		private void FocusDockedToolWindow()
		{
			if (!this.m_IsModal)
			{
				VSAnythingPackage.Inst.ActivateFastFindDockedWindow();
			}
		}

		public void OnActivated()
		{
			this.StartGettingOpenFiles();
		}

		private void TextBoxGotFocus(object sender, EventArgs e)
		{
			this.FocusDockedToolWindow();
		}

		private void StartGettingOpenFiles()
		{
			Log.WriteLine("START GetOpenFiles");
			this.m_GetOpenFilesThread.Start(this.m_ExtToScan);
		}

		private void GetOpenFilesFinished(List<string> open_files, List<UnsavedDocument> unsaved_documents)
		{
			Log.WriteLine("FINISHED GetOpenFiles");
			FastFindControl.GetOpenFilesResults results = new FastFindControl.GetOpenFilesResults();
			results.m_OpenFiles = open_files;
			results.m_UnsavedDocuments = unsaved_documents;
			this.m_ControlTaskDispatcher.QueueTask(new ControlTaskDispatcher.TaskFunction1Arg(this.GetOpenFilesFinished_MainThread), results);
		}

		private void GetOpenFilesFinished_MainThread(object arg)
		{
			FastFindControl.GetOpenFilesResults expr_06 = (FastFindControl.GetOpenFilesResults)arg;
			FastFindControl.m_OpenFiles = expr_06.m_OpenFiles;
			FastFindControl.m_UnsavedDocuments = expr_06.m_UnsavedDocuments;
			this.UpdateFilesToSearchAndRefresh();
		}

		private void TextBoxMouseDown(object sender, MouseEventArgs e)
		{
			if (!this.m_TextBox.Focused)
			{
				this.FocusDockedToolWindow();
			}
		}

		public void SelectText()
		{
			this.m_TextBox.Focus();
			this.m_TextBox.SelectAll();
			if (!this.m_TextBox.Focused)
			{
				this.m_SelectTextTimer.Start();
			}
		}

		private void SelectTextTimerTick(object sender, EventArgs e)
		{
			this.m_TextBox.Focus();
			this.m_TextBox.SelectAll();
			if (this.m_TextBox.Focused)
			{
				this.m_SelectTextTimer.Stop();
			}
		}

		private void OnDispose(object sender, EventArgs e)
		{
			if (this.m_Disposed)
			{
				return;
			}
			this.m_Disposed = true;
			this.UnhookEvents();
			this.m_UpdateTimer.Stop();
			this.m_UpdateTimer.Tick -= new EventHandler(this.TimerTick);
		}

		private void SolutionFileListChanged(ICollection<string> solution_files, ICollection<string> projects)
		{
			this.m_ControlTaskDispatcher.QueueTask(new ControlTaskDispatcher.TaskFunction1Arg(this.SolutionFileListChanged_MainThread), new List<string>(solution_files));
		}

		private void SolutionFileListChanged_MainThread(object arg)
		{
			this.m_AllSolutionFiles = (List<string>)arg;
			this.UpdateFilesToSearchAndRefresh();
		}

		private void UpdateFilesToSearchAndRefresh()
		{
			if (this.UpdateFilesToSearch())
			{
				this.RefreshFindResults(true);
			}
		}

		private bool UpdateFilesToSearch()
		{
			Set<string> old_files_to_search_set = new Set<string>(this.m_FilesToSearchSet);
			this.m_FilesToSearch = new List<string>();
			this.m_FilesToSearchSet = new Set<string>();
			using (List<string>.Enumerator enumerator = this.m_AllSolutionFiles.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string file_norm = Utils.NormalisePath(enumerator.Current);
					string ext = Path.GetExtension(file_norm);
					if (this.m_ExtToScan.Contains(ext))
					{
						this.m_FilesToSearch.Add(file_norm);
						this.m_FilesToSearchSet.Add(file_norm);
					}
				}
			}
			foreach (string expr_9D in FastFindControl.m_OpenFiles)
			{
				string open_file_norm = Utils.NormalisePath(expr_9D);
				string ext2 = Path.GetExtension(expr_9D).ToLower();
				if (this.m_ExtToScan.Contains(ext2) && !this.m_FilesToSearchSet.Contains(open_file_norm))
				{
					this.m_FilesToSearch.Add(open_file_norm);
					this.m_FilesToSearchSet.Add(open_file_norm);
				}
			}
			Set<string> old_unsaved_documents = new Set<string>();
			foreach (UnsavedDocument unsaved_document in this.m_UnsavedDocumentsToSearch)
			{
				old_unsaved_documents.Add(unsaved_document.Filename);
			}
			this.m_UnsavedDocumentsToSearch.Clear();
			Set<string> new_unsaved_documents = new Set<string>();
			foreach (UnsavedDocument unsaved_document2 in FastFindControl.m_UnsavedDocuments)
			{
				string ext3 = Path.GetExtension(unsaved_document2.Filename).ToLower();
				if (this.m_ExtToScan.Contains(ext3))
				{
					this.m_UnsavedDocumentsToSearch.Add(unsaved_document2);
					new_unsaved_documents.Add(unsaved_document2.Filename);
				}
			}
			return !Set<string>.Match(old_files_to_search_set, this.m_FilesToSearchSet) || !Set<string>.Match(old_unsaved_documents, new_unsaved_documents);
		}

		private void TimerTick(object sender, EventArgs e)
		{
			int now = Environment.TickCount;
			bool busy = this.m_TextFinder.ScanInProgress || this.m_TextFinder.FindInProgress;
			if (this.m_SolutionFiles.GettingSolutionFiles)
			{
				this.m_ProgressBar.Visible = true;
				this.m_ProgressBar.Progress = Environment.TickCount % 1000 / 10;
			}
			else if (busy)
			{
				if (this.m_BusyStartTime == -1)
				{
					this.m_BusyStartTime = now;
				}
				this.m_ProgressBar.Visible = (now - this.m_BusyStartTime > 500);
				this.m_ProgressBar.Progress = (this.m_TextFinder.ScanInProgress ? this.m_TextFinder.ScanPercentComplete : this.m_TextFinder.PercentComplete);
				this.m_ListBox.FindingTextPercent = this.m_TextFinder.PercentComplete;
				this.m_ListBox.ScanningFilesPercent = this.m_TextFinder.ScanPercentComplete;
			}
			else
			{
				this.m_ProgressBar.Visible = false;
				this.m_BusyStartTime = -1;
			}
			if (now - this.m_LastTextBoxChangedTime > 1000 && !busy)
			{
				this.m_UpdateTimer.Stop();
				this.m_ProgressBar.Visible = false;
			}
		}

		private void ScanFinished()
		{
			this.m_ControlTaskDispatcher.QueueTask(new ControlTaskDispatcher.TaskFunction(this.ScanFinished_MainThread));
		}

		private void ScanFinished_MainThread()
		{
			this.RefreshFindResults(true);
		}

		private void StartFindFiles()
		{
			if (!this.m_FilesCheckBox.Checked)
			{
				return;
			}
			this.m_FileFinder.Find(this.m_TextBox.Text, this.m_SolutionFiles.SolutionRootDir, this.m_SolutionFilesMatchCaseCheckBox.Checked, this.m_WildcardsCheckBox.Checked, this.IsEnteringPath, this.m_IsModal, new FindFilesFinishedHandler(this.FileFinderFinished));
		}

		private string GetFirstSelectedFile()
		{
			List<FastFindControl.FileAndLine> files = this.GetSelectedFiles();
			if (files.Count == 0)
			{
				return "";
			}
			return files[0].m_Filename;
		}

		private List<FastFindControl.FileAndLine> GetSelectedFiles()
		{
			List<FastFindControl.FileAndLine> selected_files = new List<FastFindControl.FileAndLine>();
			if (this.IsEnteringPath)
			{
				selected_files.Add(new FastFindControl.FileAndLine(this.m_TextBox.Text));
			}
			else if (this.m_ListBox.SelectedIndexStart != -1)
			{
				int start_index = this.m_ListBox.SelectedIndexStart;
				int end_index = this.m_ListBox.SelectedIndexEnd;
				if (end_index < start_index)
				{
					Utils.Swap<int>(ref start_index, ref end_index);
				}
				for (int sel_index = start_index; sel_index <= end_index; sel_index++)
				{
					object item = this.m_ListBox.Items[sel_index];
					FilteredFileItem filtered_file_item = item as FilteredFileItem;
					if (filtered_file_item != null)
					{
						selected_files.Add(new FastFindControl.FileAndLine(filtered_file_item.m_FullPath));
					}
					else
					{
						FindTextResult find_text_result = item as FindTextResult;
						if (find_text_result != null)
						{
							string filename = find_text_result.m_Path;
							if (filename != null && this.m_SolutionFiles.SolutionRootDir != null)
							{
								if (VSAnythingPackage.Inst.GetSettingsDialogPage().FindTextPathMode == PathMode.Relative)
								{
									filename = Utils.GetFullPath(Path.Combine(this.m_SolutionFiles.SolutionRootDir, filename));
								}
								if (!Path.IsPathRooted(filename))
								{
									filename = Path.Combine(this.m_SolutionFiles.SolutionRootDir, filename);
								}
								selected_files.Add(new FastFindControl.FileAndLine(filename, find_text_result.m_LineIndex));
							}
						}
					}
				}
			}
			return selected_files;
		}

		private int GetSelectedFileLine()
		{
			FindTextResult text_result = this.m_ListBox.SelectedItem as FindTextResult;
			if (text_result != null)
			{
				return text_result.m_LineIndex;
			}
			return -1;
		}

		private void ScrollSelectionTo(int index, bool multi_select)
		{
			if (multi_select)
			{
				this.m_ListBox.SelectedIndexEnd = Misc.Clamp(index, 0, this.m_ListBox.SelectableItemCount - 1);
				return;
			}
			this.m_ListBox.SelectedIndex = Misc.Clamp(index, 0, this.m_ListBox.SelectableItemCount - 1);
		}

		private void ScrollTo(int index)
		{
			this.m_ListBox.ScrollIndex = Misc.Clamp(index, 0, this.m_ListBox.ItemCount - 1);
		}

		private void OldSearchesDropDownButtonClicked(object sender, EventArgs e)
		{
			if (this.m_IgnoreOldsearchesButtonClick)
			{
				this.m_IgnoreOldsearchesButtonClick = false;
				return;
			}
			if (this.m_Settings.OldSearches.Count != 0)
			{
				this.ShowOldSearchComboBox();
			}
		}

		private void OldSearchesDropDownButtonMouseDown(object sender, MouseEventArgs e)
		{
			this.m_IgnoreOldsearchesButtonClick = (Environment.TickCount - this.m_OldSearchesListBoxCloseTime < 100);
		}

		private void ShowOldSearchComboBox()
		{
			if (this.m_OldItemsDropDownForm != null)
			{
				return;
			}
			this.m_OldItemsDropDownForm = new OldItemsDropDownForm();
			AppearanceDialogPage appearance_settings = VSAnythingPackage.Inst.GetAppearanceDialogPage();
			this.m_OldItemsDropDownForm.Font = this.m_TextBox.Font;
			this.m_OldItemsDropDownForm.BackColor = this.m_TextBox.BackColor;
			this.m_OldItemsDropDownForm.ForeColor = this.m_TextBox.ForeColor;
			this.m_OldItemsDropDownForm.SetSelectBarColour(appearance_settings.Colours.m_SelectColour);
			this.m_OldItemsDropDownForm.SetItems(this.m_Settings.OldSearches);
			Point location = this.m_TextBoxPanel.PointToScreen(new Point(0, this.m_TextBoxPanel.Height));
			if (location.Y + this.m_OldItemsDropDownForm.Height > Screen.PrimaryScreen.Bounds.Height)
			{
				location = new Point(location.X, location.Y - this.m_TextBoxPanel.Height - this.m_OldItemsDropDownForm.Height);
			}
			this.m_OldItemsDropDownForm.Location = location;
			this.m_OldItemsDropDownForm.Size = new Size(this.m_TextBox.Width, this.m_OldItemsDropDownForm.Height);
			string first_old_search = "";
			using (IEnumerator<string> enumerator = this.m_Settings.OldSearches.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					first_old_search = enumerator.Current;
				}
			}
			this.m_TextBox.Text = first_old_search;
			this.m_OldItemsDropDownForm.Selectionchanged += new OldItemsDropDownForm.SelectionchangedHandler(this.OldSearchesListBoxSelectionChanged);
			this.m_OldItemsDropDownForm.FormClosed += new FormClosedEventHandler(this.OldSearchesListBoxClosed);
			this.m_OldItemsDropDownForm.Show();
			this.m_OldItemsDropDownForm.Activate();
		}

		private void OldSearchesListBoxClosed(object sender, FormClosedEventArgs e)
		{
			this.m_OldItemsDropDownForm.Selectionchanged -= new OldItemsDropDownForm.SelectionchangedHandler(this.OldSearchesListBoxSelectionChanged);
			this.m_OldItemsDropDownForm.FormClosed -= new FormClosedEventHandler(this.OldSearchesListBoxClosed);
			this.m_OldItemsDropDownForm = null;
			this.m_OldSearchesListBoxCloseTime = Environment.TickCount;
			if (!base.ContainsFocus && !this.m_IsModal)
			{
				this.m_SelectTextOnNextFocus = true;
			}
		}

		private void OldSearchesListBoxSelectionChanged()
		{
			this.m_TextBox.Text = this.m_OldItemsDropDownForm.SelectedItem;
		}

		private void UpdateInitialSelectedItem()
		{
			this.m_InitialSelectedItem = ((this.m_ListBox.SelectedIndex > 0) ? this.m_ListBox.SelectedItem : null);
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData <= Keys.Next)
			{
				if (keyData <= Keys.Return)
				{
					if (keyData != Keys.Tab)
					{
						if (keyData == Keys.Return)
						{
							this.Submit();
							return true;
						}
					}
					else if (this.AutoCompletePath())
					{
						return true;
					}
				}
				else
				{
					if (keyData == Keys.Escape)
					{
						if (this.m_IsModal)
						{
							this.CloseForm();
						}
						return true;
					}
					if (keyData == Keys.Prior)
					{
						this.ScrollSelectionTo(this.m_ListBox.SelectedIndex - this.m_ListBox.Height / this.m_ListBox.ItemHeight, false);
						this.UpdateInitialSelectedItem();
						return true;
					}
					if (keyData == Keys.Next)
					{
						this.ScrollSelectionTo(this.m_ListBox.SelectedIndex + this.m_ListBox.Height / this.m_ListBox.ItemHeight, false);
						this.UpdateInitialSelectedItem();
						return true;
					}
				}
			}
			else if (keyData <= (Keys.LButton | Keys.Space | Keys.Shift))
			{
				if (keyData == Keys.Up)
				{
					this.ScrollSelectionTo(this.m_ListBox.SelectedIndex - 1, false);
					this.UpdateInitialSelectedItem();
					return true;
				}
				if (keyData == Keys.Down)
				{
					if (this.m_TextBox.Text.Length == 0 && this.m_Settings.OldSearches.Count != 0)
					{
						this.ShowOldSearchComboBox();
					}
					else
					{
						this.ScrollSelectionTo(this.m_ListBox.SelectedIndex + 1, false);
					}
					this.UpdateInitialSelectedItem();
					return true;
				}
				if (keyData == (Keys.LButton | Keys.Space | Keys.Shift))
				{
					this.ScrollSelectionTo(this.m_ListBox.SelectedIndexEnd - this.m_ListBox.Height / this.m_ListBox.ItemHeight, true);
					this.UpdateInitialSelectedItem();
					return true;
				}
			}
			else
			{
				if (keyData == (Keys.RButton | Keys.Space | Keys.Shift))
				{
					this.ScrollSelectionTo(this.m_ListBox.SelectedIndexEnd + this.m_ListBox.Height / this.m_ListBox.ItemHeight, true);
					this.UpdateInitialSelectedItem();
					return true;
				}
				if (keyData == (Keys.RButton | Keys.MButton | Keys.Space | Keys.Shift))
				{
					this.ScrollSelectionTo(this.m_ListBox.SelectedIndexEnd - 1, true);
					this.UpdateInitialSelectedItem();
					return true;
				}
				if (keyData == (Keys.Back | Keys.Space | Keys.Shift))
				{
					if (this.m_TextBox.Text.Length == 0 && this.m_Settings.OldSearches.Count != 0)
					{
						this.ShowOldSearchComboBox();
					}
					else
					{
						this.ScrollSelectionTo(this.m_ListBox.SelectedIndexEnd + 1, true);
					}
					this.UpdateInitialSelectedItem();
					return true;
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void Submit()
		{
			List<FastFindControl.FileAndLine> selected_items = this.GetSelectedFiles();
			if (selected_items.Count == 1 && !string.IsNullOrEmpty(selected_items[0].m_Filename) && !File.Exists(selected_items[0].m_Filename))
			{
				this.AutoCompletePath();
				selected_items = this.GetSelectedFiles();
			}
			bool first = true;
			foreach (FastFindControl.FileAndLine file_and_line in selected_items)
			{
				string filename = file_and_line.m_Filename;
				if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
				{
					if (Path.GetExtension(filename).ToLower() == ".sln")
					{
						this.m_DTE.OpenSolution(filename);
					}
					else if (this.m_DTE.GetActiveDocumentFilename() != null && Utils.NormalisePathAndLowerCase(this.m_DTE.GetActiveDocumentFilename()) == Utils.NormalisePathAndLowerCase(filename))
					{
						int line = this.GetSelectedFileLine();
						if (line != -1)
						{
							this.m_DTE.SetActiveDocumentLine(line);
						}
					}
					else
					{
						VSAnythingPackage.Inst.ActivateDoc(filename, file_and_line.m_Line, first);
						this.m_DTE.OpenFile(filename);
					}
				}
				first = false;
			}
			this.UpdateOldSearchesList();
			if (this.m_IsModal)
			{
				this.CloseForm();
			}
		}

		private void UpdateOldSearchesList()
		{
			List<string> old_searches = new List<string>(this.m_Settings.OldSearches);
			if (old_searches.Contains(this.m_TextBox.Text))
			{
				old_searches.Remove(this.m_TextBox.Text);
			}
			old_searches.Insert(0, this.m_TextBox.Text);
			if (old_searches.Count > 10)
			{
				old_searches.RemoveAt(old_searches.Count - 1);
			}
			if (!this.m_Settings.OldSearches.SequenceEqual(old_searches))
			{
				this.m_Settings.OldSearches = old_searches;
				this.m_Settings.Write();
			}
		}

		private bool AutoCompletePath()
		{
			return this.AutoCompletePath(true);
		}

		private bool AutoCompletePath(bool add_slash)
		{
			if (this.m_ListBox.SelectedIndex != -1 && this.IsEnteringPath)
			{
				string cur_dir = Path.GetDirectoryName(this.m_TextBox.Text);
				this.m_TextBox.Text = Path.Combine(cur_dir, this.m_ListBox.SelectedItem.ToString());
				if (add_slash && Directory.Exists(this.m_TextBox.Text) && !this.m_TextBox.Text.EndsWith("\\"))
				{
					FastFindTextBox expr_80 = this.m_TextBox;
					expr_80.Text += "\\";
				}
				this.m_TextBox.SelectionStart = this.m_TextBox.Text.Length;
				return true;
			}
			return false;
		}

		private bool ListBoxCointains(string value)
		{
			foreach (object item in this.m_ListBox.Items)
			{
				FilteredFileItem filtered_file_item = item as FilteredFileItem;
				if (filtered_file_item != null && filtered_file_item.ToString() == value)
				{
					bool result = true;
					return result;
				}
				FindTextResult text_result_result = item as FindTextResult;
				if (text_result_result != null && text_result_result.ToString() == value)
				{
					bool result = true;
					return result;
				}
			}
			return false;
		}

		private void SelectListBoxItem(string value)
		{
			foreach (object item in this.m_ListBox.Items)
			{
				FilteredFileItem filtered_file_item = item as FilteredFileItem;
				if (filtered_file_item != null && filtered_file_item.ToString() == value)
				{
					this.m_ListBox.SelectedItem = item;
					break;
				}
				FindTextResult find_text_result = item as FindTextResult;
				if (find_text_result != null && find_text_result.ToString() == value)
				{
					this.m_ListBox.SelectedItem = item;
					break;
				}
			}
		}

		protected override void WndProc(ref Message m)
		{
			try
			{
				this.m_ControlTaskDispatcher.ProcessMessage(base.Handle, ref m);
			}
			catch (Exception arg_14_0)
			{
				Utils.LogException(arg_14_0);
			}
			base.WndProc(ref m);
		}

		private void RefreshFindResults(bool force_update)
		{
			if (this.m_TextBox.Text != this.m_LastFindString | force_update)
			{
				this.m_LastFindString = this.m_TextBox.Text;
				this.StartFindFiles();
				this.StartFindText(this.m_ListBox.MaxVisibleItemCount);
				this.m_UpdateTimer.Start();
			}
		}

		private static bool ContainsLogicalOperator(string pattern)
		{
			FastFindControl.LogicalOperator[] logicalOperators = FastFindControl.m_LogicalOperators;
			for (int i = 0; i < logicalOperators.Length; i++)
			{
				FastFindControl.LogicalOperator op = logicalOperators[i];
				if (pattern.Contains(op.m_Name))
				{
					return true;
				}
			}
			return false;
		}

		private Pattern[] SplitPattern(string pattern_string)
		{
			if (this.m_Settings.GetUseLogicalOperators(this.m_IsModal) && FastFindControl.ContainsLogicalOperator(pattern_string))
			{
				List<Pattern> patterns_list = new List<Pattern>();
				Pattern.Operator op = Pattern.Operator.OR;
				while (FastFindControl.ContainsLogicalOperator(pattern_string))
				{
					int start = 0;
					int length = 0;
					int min_index = 2147483647;
					Pattern.Operator next_op = Pattern.Operator.OR;
					FastFindControl.LogicalOperator[] logicalOperators = FastFindControl.m_LogicalOperators;
					for (int i = 0; i < logicalOperators.Length; i++)
					{
						FastFindControl.LogicalOperator logic_op = logicalOperators[i];
						int index = pattern_string.IndexOf(logic_op.m_Name);
						if (index != -1 && index < min_index)
						{
							start = index;
							length = logic_op.m_Name.Length;
							next_op = logic_op.m_Operator;
							min_index = index;
						}
					}
					string sub_pattern = pattern_string.Substring(0, start);
					pattern_string = pattern_string.Substring(start + length);
					patterns_list.Add(new Pattern
					{
						m_Pattern = sub_pattern,
						m_Operator = op,
						m_UseWildcard = (this.m_WildcardsCheckBox.Checked && sub_pattern.Contains('*'))
					});
					op = next_op;
				}
				if (pattern_string.Length != 0)
				{
					patterns_list.Add(new Pattern
					{
						m_Pattern = pattern_string,
						m_Operator = op,
						m_UseWildcard = (this.m_WildcardsCheckBox.Checked && pattern_string.Contains('*'))
					});
				}
				return patterns_list.ToArray();
			}
			if (pattern_string.Length != 0)
			{
				Pattern[] expr_158 = new Pattern[1];
				expr_158[0].m_Pattern = pattern_string;
				expr_158[0].m_Operator = Pattern.Operator.OR;
				expr_158[0].m_UseWildcard = (this.m_WildcardsCheckBox.Checked && pattern_string.Contains('*'));
				return expr_158;
			}
			return new Pattern[0];
		}

		private void StripFileExtOverride(ref string text, ref List<string> ext_override)
		{
			int index = text.IndexOf(" IN .");
			if (index != -1)
			{
                //int ext_index = index + " IN ".Length;   
				int ext_index = index + " IN .".Length;  // 这里似乎应该加上.
				string arg_4C_0 = text.Substring(ext_index, text.Length - ext_index);
				text = text.Substring(0, index);
				ext_override = new List<string>();
				string[] array = arg_4C_0.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					string ext = array[i];
					ext_override.Add(ext.Trim());
				}
			}
		}

		private void StripOperator(ref string text, string op, ref bool match_whole_word)
		{
			if (text.EndsWith(op))
			{
				match_whole_word = true;
				text = text.Substring(0, text.Length - op.Length);
			}
		}

		private void StartFindText(int max_result_count)
		{
			if (this.m_FindTextCheckBox.Checked && !string.IsNullOrEmpty(this.m_SolutionFiles.SolutionRootDir))
			{
				string text = this.m_TextBox.Text;
				List<string> ext_override = null;
				this.StripFileExtOverride(ref text, ref ext_override);
				bool match_whole_word_override = false;
				bool case_override = false;
				this.StripOperator(ref text, " WORD", ref match_whole_word_override);
				this.StripOperator(ref text, " CASE", ref case_override);
				Pattern[] patterns = this.SplitPattern(text);
				if (!this.m_RegExpCheckBox.Checked)
				{
					for (int i = 0; i < patterns.Length; i++)
					{
						string sub_pattern = patterns[i].m_Pattern;
						if (sub_pattern.Contains('*'))
						{
							if (!sub_pattern.StartsWith("*"))
							{
								sub_pattern = "*" + sub_pattern;
							}
							if (!sub_pattern.EndsWith("*"))
							{
								sub_pattern += "*";
							}
						}
						patterns[i].m_Pattern = sub_pattern;
					}
				}
				FindTextRequest request = new FindTextRequest();
				request.m_Patterns = patterns;
				request.m_MatchCase = (this.m_Settings.GetFindTextMatchCase(this.m_IsModal) | case_override);
				request.m_MaxResultCount = max_result_count;
				request.m_TextBoxText = this.m_TextBox.Text;
				request.FindFinished = new FindFinishedHandler(this.FindTextFinished);
				SettingsDialogPage settings = VSAnythingPackage.Inst.GetSettingsDialogPage();
				this.m_TextFinder.Find(request, this.m_FilesToSearch, FastFindControl.m_UnsavedDocuments, settings.FindTextPathMode, this.m_SolutionFiles.SolutionRootDir, ext_override, this.m_WholeWordCheckbox.Checked | match_whole_word_override, this.m_RegExpCheckBox.Checked);
			}
		}

		private void FindTextFinished(FindTextRequest request)
		{
			this.m_ControlTaskDispatcher.QueueTask(new ControlTaskDispatcher.TaskFunction1Arg(this.FindTextFinished_MainThread), request);
		}

		private void FindTextFinished_MainThread(object arg)
		{
			FindTextRequest request = (FindTextRequest)arg;
			int old_results_count = this.m_TextFinderResults.Count;
			this.m_TextFinderResults = request.m_MatchingWords;
			int max_results = VSAnythingPackage.Inst.GetSettingsDialogPage().MaxResults;
			bool limited_match = request.m_MaxResultCount < max_results && request.m_MatchingWords.Count >= request.m_MaxResultCount;
			if (limited_match)
			{
				TextFinderResult filler = default(TextFinderResult);
				filler.m_Filename = "";
				filler.m_Line = "";
				while (this.m_TextFinderResults.Count < old_results_count)
				{
					this.m_TextFinderResults.Add(filler);
				}
			}
			this.UpdateListBox();
			if (limited_match && this.m_TextBox.Text == request.m_TextBoxText)
			{
				this.StartFindText(max_results + 1);
			}
		}

		private void FileFinderFinished(List<FindFileResult> files, string text_box_value)
		{
			files.Sort(new FastFindControl.FilterComparer(text_box_value));
			FastFindControl.FindFilesFinishedResults results = new FastFindControl.FindFilesFinishedResults();
			results.m_Files = files;
			results.m_TextBoxValue = text_box_value;
			this.m_ControlTaskDispatcher.QueueTask(new ControlTaskDispatcher.TaskFunction1Arg(this.FindFilesFinished_MainThread), results);
		}

		private void FindFilesFinished_MainThread(object arg)
		{
			FastFindControl.FindFilesFinishedResults results = (FastFindControl.FindFilesFinishedResults)arg;
			this.m_FindFileResults = results.m_Files;
			this.UpdateListBox();
		}

		private void UpdateListBox()
		{
			try
			{
				this.m_ListBox.BeginUpdate();
				if (this.m_ListBox.SelectedIndex > 0)
				{
					int arg_24_0 = this.m_ListBox.SelectedIndex;
				}
				this.m_ListBox.Clear();
				int selected_index = -1;
				if (this.IsEnteringPath)
				{
					foreach (FindFileResult filtered_file in this.m_FindFileResults)
					{
						this.m_ListBox.AddItem(filtered_file.m_FileName);
					}
					string text_box_file = Utils.GetFileNameSafe(this.m_TextBox.Text).ToLower();
					int index = 0;
					using (List<FindFileResult>.Enumerator enumerator = this.m_FindFileResults.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.m_FileName.ToLower() == text_box_file)
							{
								selected_index = index;
								break;
							}
							index++;
						}
						goto IL_259;
					}
				}
				if (this.m_FilesCheckBox.Checked)
				{
					foreach (FindFileResult filtered_file2 in this.m_FindFileResults)
					{
						if (!filtered_file2.m_PathMatch)
						{
							string full_path = filtered_file2.m_FilePath;
							string display_filename = full_path;
							if (Path.IsPathRooted(display_filename))
							{
								switch (VSAnythingPackage.Inst.GetSettingsDialogPage().FindFilesPathMode)
								{
								case PathMode.Relative:
									display_filename = filtered_file2.m_RelativeFilePath;
									break;
								case PathMode.Filename:
									display_filename = Path.GetFileName(display_filename);
									break;
								}
							}
							if (selected_index == -1 && this.m_InitialSelectedItem != null && filtered_file2.MyEquals(this.m_InitialSelectedItem))
							{
								selected_index = this.m_ListBox.ItemCount;
							}
							this.m_ListBox.AddItem(new FilteredFileItem(display_filename, full_path));
						}
					}
				}
				if (this.m_FindTextCheckBox.Checked)
				{
					foreach (TextFinderResult result in this.m_TextFinderResults)
					{
						FindTextResult item = new FindTextResult(result.m_Filename, result.m_LineIndex, result.m_Line, result.m_StartIndex, result.m_EndIndex);
						if (selected_index == -1 && item.MyEquals(this.m_InitialSelectedItem))
						{
							selected_index = this.m_ListBox.ItemCount;
						}
						this.m_ListBox.AddItem(item);
					}
				}
				IL_259:
				int max_results = VSAnythingPackage.Inst.GetSettingsDialogPage().MaxResults;
				this.m_ListBox.OnlyShowingMaxMatches = (this.m_TextFinderResults.Count >= max_results && this.m_FindTextCheckBox.Checked && !this.IsEnteringPath);
				this.m_ListBox.ShowGettingSolutionFiles = this.m_SolutionFiles.GettingSolutionFiles;
				this.m_ListBox.ShowScanningFiles = this.m_TextFinder.ScanInProgress;
				this.m_ListBox.ShowFindingTextMessage = this.m_TextFinder.FindInProgress;
				this.m_ListBox.ShowNoItemsMessage = (this.m_TextBox.Text.Length != 0);
				this.m_ListBox.UpdateInfoItem();
				this.m_ListBox.MaxMatchCount = max_results;
				if (selected_index != -1)
				{
					this.m_ListBox.SelectedIndex = selected_index;
				}
				else if (this.m_ListBox.SelectableItemCount != 0)
				{
					this.m_ListBox.SelectedIndex = 0;
					this.m_InitialSelectedItem = null;
				}
				else
				{
					this.m_ListBox.SelectedIndex = -1;
					this.m_InitialSelectedItem = null;
				}
				this.m_ListBox.EndUpdate();
				this.UpdateFullPathText();
				this.UpdateTitle();
			}
			catch (Exception arg_372_0)
			{
				Utils.LogException(arg_372_0);
			}
		}

		private void UpdateTitle()
		{
			try
			{
				Form parent_form = base.Parent as Form;
				if (parent_form != null)
				{
					string new_title = "FastFind";
					int max_results = VSAnythingPackage.Inst.GetSettingsDialogPage().MaxResults;
					int expr_36 = Math.Min(this.m_ListBox.SelectableItemCount, max_results);
					string plus = (expr_36 == max_results) ? "+" : "";
					if (expr_36 != 0)
					{
						new_title = string.Concat(new object[]
						{
							new_title,
							"   ",
							this.m_ListBox.ItemCount,
							plus,
							" matches"
						});
					}
					parent_form.Text = new_title;
				}
			}
			catch (Exception arg_89_0)
			{
				Utils.LogExceptionQuiet(arg_89_0);
			}
		}

		private void TextBoxTextChanged(object sender, EventArgs e)
		{
			if (VSAnythingPackage.Inst.Expired)
			{
				return;
			}
			if (this.m_IgnoreTextBoxTextChanges)
			{
				return;
			}
			this.m_UpdateTimer.Start();
			this.m_InitialSelectedItem = null;
			this.RefreshFindResults(false);
			this.m_LastTextBoxChangedTime = Environment.TickCount;
		}

		private void ListBoxSelectedIndexChanged()
		{
			this.UpdateFullPathText();
			this.UpdateInitialSelectedItem();
		}

		private void UpdateFullPathText()
		{
			if (this.IsEnteringPath)
			{
				this.m_FullPathTextBox.Text = "";
				return;
			}
			this.m_FullPathTextBox.Text = this.GetFirstSelectedFile();
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			this.OnMouseWheel(e.Delta);
			base.OnMouseWheel(e);
		}

		public void OnMouseWheel(int delta)
		{
			if (delta < 0)
			{
				this.ScrollTo(this.m_ListBox.ScrollIndex + 3);
				return;
			}
			if (delta > 0)
			{
				this.ScrollTo(this.m_ListBox.ScrollIndex - 3);
			}
		}

		private void FindTextCheckBoxChanged(object sender, EventArgs e)
		{
			if (this.m_Settings.SetFastFindFindText(this.m_FindTextCheckBox.Checked, this.m_IsModal))
			{
				this.UpdateListBox();
				this.m_Settings.Write();
				this.RefreshFindResults(true);
			}
		}

		private void FilesCheckBoxChanged(object sender, EventArgs e)
		{
			if (this.m_Settings.SetFastFindShowFiles(this.m_FilesCheckBox.Checked, this.m_IsModal))
			{
				this.UpdateListBox();
				this.m_Settings.Write();
				this.RefreshFindResults(true);
			}
		}

		private void WildcardsCheckboxChanged(object sender, EventArgs e)
		{
			if (this.m_Settings.SetFastFindWildcards(this.m_WildcardsCheckBox.Checked, this.m_IsModal))
			{
				this.m_Settings.Write();
				this.RefreshFindResults(true);
			}
		}

		private void MatchWholeWordCheckedChanged(object sender, EventArgs e)
		{
			if (this.m_Settings.SetMatchWholeWord(this.m_WholeWordCheckbox.Checked, this.m_IsModal))
			{
				this.m_Settings.Write();
				this.RefreshFindResults(true);
			}
		}

		private void FindTextMatchCaseCheckBoxChanged(object sender, EventArgs e)
		{
			if (this.m_Settings.SetFindTextMatchCase(this.m_FindTextMatchCaseCheckBox.Checked, this.m_IsModal))
			{
				this.m_Settings.Write();
				this.RefreshFindResults(true);
			}
		}

		private void LogicalOpsCheckboxChanged(object sender, EventArgs e)
		{
			if (this.m_Settings.SetUseLogicalOperators(this.m_LogicalOperatorsCheckBox.Checked, this.m_IsModal))
			{
				this.m_Settings.Write();
				this.RefreshFindResults(true);
			}
		}

		private void SolutionFilesMatchCaseCheckBoxChanged(object sender, EventArgs e)
		{
			if (this.m_Settings.SetSolutionFilesMatchCase(this.m_SolutionFilesMatchCaseCheckBox.Checked, this.m_IsModal))
			{
				this.m_Settings.Write();
				this.RefreshFindResults(true);
			}
		}

		private void RegExpressionCheckBoxchanged(object sender, EventArgs e)
		{
			if (this.m_Settings.SetRegExpression(this.m_RegExpCheckBox.Checked, this.m_IsModal))
			{
				this.m_Settings.Write();
				this.RefreshFindResults(true);
			}
		}

		private void OptionsButtonClicked(object sender, EventArgs e)
		{
			bool visible = !this.m_Settings.GetOptionsPanelVisible(this.m_IsModal);
			this.m_Settings.SetOptionsPanelVisible(visible, this.m_IsModal);
			this.m_OptionsPanel.Visible = visible;
		}

		private void ExtTypesControlLeaveEvent(object sender, EventArgs e)
		{
			if (this.m_Settings.SetFastFindFileExt(this.m_FileExtComboBox.Text, this.m_IsModal))
			{
				this.UpdateExtToScan();
				this.m_Settings.Write();
			}
		}

		private void OpenButtonClicked(object sender, EventArgs e)
		{
			this.Submit();
		}

		private void CancelButtonClicked(object sender, EventArgs e)
		{
			this.CloseForm();
		}

		private void CloseForm()
		{
			if (this.ControlWantsToClose != null)
			{
				this.ControlWantsToClose();
			}
		}

		private void ListBoxItemDoubleClicked()
		{
			this.Submit();
		}

		private void RescanButtonClicked(object sender, EventArgs e)
		{
			this.m_SolutionFiles.ForceUpdateFileList();
		}

		public void GotoNextLocation()
		{
			this.m_ControlTaskDispatcher.QueueTask(new ControlTaskDispatcher.TaskFunction(this.GotoNextLocation_MainThread));
		}

		private void GotoNextLocation_MainThread()
		{
			if (this.m_ListBox.SelectedIndex != -1 && this.m_ListBox.SelectedIndex < this.m_ListBox.ItemCount - 1)
			{
				this.ScrollSelectionTo(this.m_ListBox.SelectedIndex + 1, false);
				this.Submit();
			}
		}

		public void GotoPrevLocation()
		{
			this.m_ControlTaskDispatcher.QueueTask(new ControlTaskDispatcher.TaskFunction(this.GotoPrevLocation_MainThread));
		}

		private void GotoPrevLocation_MainThread()
		{
			if (this.m_ListBox.SelectedIndex > 0)
			{
				this.ScrollSelectionTo(this.m_ListBox.SelectedIndex - 1, false);
				this.Submit();
			}
		}

		private void TextBoxEscapeKeyPressed()
		{
			this.m_DTE.ActivateActiveDocument();
		}

		private void UpdateExtToScan()
		{
			ICollection<string> old_ext_to_scan = VSAnythingPackage.Inst.GetSettingsDialogPage().ExtList;
			Set<string> ext_to_scan;
			if (this.m_FileExtComboBox.Text == "Default" || string.IsNullOrEmpty(this.m_FileExtComboBox.Text))
			{
				ext_to_scan = new Set<string>(old_ext_to_scan);
			}
			else
			{
				ext_to_scan = new Set<string>(this.m_FileExtComboBox.Text.Split(new char[]
				{
					';'
				}));
			}
			Set<string> ext_to_scan_lower = Utils.ToLower(ext_to_scan);
			if (!ext_to_scan_lower.SequenceEqual(this.m_ExtToScan))
			{
				this.m_ExtToScan = ext_to_scan_lower;
				bool found_new_ext = false;
				Set<string> settings_ext_list = Utils.ToLower(new Set<string>(old_ext_to_scan));
				foreach (string ext in ext_to_scan_lower)
				{
					if (!settings_ext_list.Contains(ext))
					{
						settings_ext_list.Add(ext);
						found_new_ext = true;
					}
				}
				if (found_new_ext)
				{
					VSAnythingPackage.Inst.GetSettingsDialogPage().ExtList = settings_ext_list.ToArray();
					VSAnythingPackage.Inst.OnSettingsExtListChanged();
				}
				this.UpdateFilesToSearchAndRefresh();
				this.StartGettingOpenFiles();
			}
		}

		private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			char keyChar = e.KeyChar;
			if (keyChar == '\\' && this.IsEnteringPath)
			{
				this.AutoCompletePath(false);
			}
		}

		private void DisableTipsButton(object sender, EventArgs e)
		{
			VSAnythingPackage.Inst.GetSettingsDialogPage().EnableTips = false;
			this.m_TipsPanel.Visible = false;
		}

		private void HideTipsButton(object sender, EventArgs e)
		{
			this.m_TipsPanel.Visible = false;
		}

		private void InitialiseTipsPanel()
		{
			bool arg_41_0 = VSAnythingPackage.Inst.GetSettingsDialogPage().EnableTips;
			int days_since_last_tip = DateTime.Now.DayOfYear - this.m_Settings.LastShowTipDay;
			this.m_Settings.LastShowTipDay = DateTime.Now.DayOfYear;
			bool tips_visible = arg_41_0 && days_since_last_tip != 0;
			this.m_TipsPanel.Visible = tips_visible;
			if (tips_visible)
			{
				this.ShowNextTip();
			}
		}

		private void ShowNextTip()
		{
			this.m_Settings.TipIndex = this.m_Settings.TipIndex % Tips.TipStrings.Length;
			string[] arg_39_0 = Tips.TipStrings;
			Settings expr_29 = this.m_Settings;
			int tipIndex = expr_29.TipIndex;
			expr_29.TipIndex = tipIndex + 1;
			string tip_text = arg_39_0[tipIndex];
			bool show_link_label = tip_text.Contains("link(");
			this.m_TipLinkLabel.Visible = show_link_label;
			if (show_link_label)
			{
				int index = tip_text.IndexOf("link(");
				string expr_77 = tip_text.Substring(index + "link(".Length);
				int end_index = expr_77.IndexOf(')');
				string[] expr_99 = expr_77.Substring(0, end_index).Split(new char[]
				{
					','
				});
				string link_text = expr_99[0];
				string arg_AD_0 = expr_99[1];
				this.m_TipLinkLabel.Text = link_text;
				FastFindControl.m_TipLinkValue = arg_AD_0;
				tip_text = tip_text.Substring(0, index);
				Graphics graphics = base.CreateGraphics();
				int width = Utils.GetStringWidth(tip_text, tip_text.Length, graphics, this.m_TipLabel.Font, StringFormat.GenericDefault);
				this.m_TipLinkLabel.Location = new Point(this.m_TipLabel.Left + width, this.m_TipLabel.Top + this.m_TipLabel.Height / 2 - this.m_TipLinkLabel.Height / 2);
				graphics.Dispose();
			}
			this.m_TipLabel.Text = tip_text;
		}

		private void TipLinkLabelClicked(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(FastFindControl.m_TipLinkValue))
			{
				Process.Start(FastFindControl.m_TipLinkValue);
			}
		}

		private void PrevTipButtonClicked(object sender, EventArgs e)
		{
			this.ShowNextTip();
		}

		private void NextTipButtonClicked(object sender, EventArgs e)
		{
			this.ShowNextTip();
		}

		private void HelpButtonClicked(object sender, EventArgs e)
		{
			Process.Start("www.puredevsoftware.com/fastfind/features.htm");
		}

		private void ControlGotFocus(object sender, EventArgs e)
		{
			if (!this.m_IsModal && this.m_SelectTextOnNextFocus)
			{
				this.m_SelectTextOnNextFocus = false;
				this.m_TextBox.SelectAll();
				this.m_TextBox.Focus();
			}
		}

		private void ControlLostFocus(object sender, EventArgs e)
		{
			if (!base.ContainsFocus && this.m_OldItemsDropDownForm == null)
			{
				this.m_SelectTextOnNextFocus = true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            this.SuspendLayout();
            // 
            // FastFindControl
            // 
            this.Name = "FastFindControl";
            //this.Load += new System.EventHandler(this.FastFindControl_Load); //todo
            this.ResumeLayout(false);

		}
	}
}
