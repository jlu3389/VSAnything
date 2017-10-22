using SCLCoreCLR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Company.VSAnything
{
	internal class Settings : IDisposable
	{
		private object m_Lock = new object();

		private const int m_WriteDelay = 1000;

		private AsyncTask m_AsyncWriteTask;

		private Size m_FastFindFormSize;

		private FastFindControlSettings m_FastFindControlSettings_Modal = new FastFindControlSettings();

		private FastFindControlSettings m_FastFindControlSettings = new FastFindControlSettings();

		private string m_EMail;

		private string m_RegKey;

		private int m_LastShowExpireForm;

		private List<string> m_SolutionFiles = new List<string>();

		private List<string> m_OldSearches = new List<string>();

		private int m_TipIndex;

		private int m_LastShowTipDay = DateTime.Now.DayOfYear - 1;

		private bool m_ShownWelcomeForm;

		private static string Filename
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FastFind\\FastFind_settings.xml";
			}
		}

		public string EMail
		{
			get
			{
				object @lock = this.m_Lock;
				string eMail;
				lock (@lock)
				{
					eMail = this.m_EMail;
				}
				return eMail;
			}
			set
			{
				object @lock = this.m_Lock;
				lock (@lock)
				{
					this.m_EMail = value;
				}
			}
		}

		public string RegKey
		{
			get
			{
				object @lock = this.m_Lock;
				string regKey;
				lock (@lock)
				{
					regKey = this.m_RegKey;
				}
				return regKey;
			}
			set
			{
				object @lock = this.m_Lock;
				lock (@lock)
				{
					this.m_RegKey = value;
				}
			}
		}

		public int LastShowExpireForm
		{
			get
			{
				object @lock = this.m_Lock;
				int lastShowExpireForm;
				lock (@lock)
				{
					lastShowExpireForm = this.m_LastShowExpireForm;
				}
				return lastShowExpireForm;
			}
			set
			{
				object @lock = this.m_Lock;
				lock (@lock)
				{
					this.m_LastShowExpireForm = value;
				}
			}
		}

		public Size FastFindFormSize
		{
			get
			{
				object @lock = this.m_Lock;
				Size fastFindFormSize;
				lock (@lock)
				{
					fastFindFormSize = this.m_FastFindFormSize;
				}
				return fastFindFormSize;
			}
			set
			{
				object @lock = this.m_Lock;
				lock (@lock)
				{
					this.m_FastFindFormSize = value;
				}
			}
		}

		public ICollection<string> SolutionFiles
		{
			get
			{
				object @lock = this.m_Lock;
				ICollection<string> result;
				lock (@lock)
				{
					result = new List<string>(this.m_SolutionFiles);
				}
				return result;
			}
		}

		public ICollection<string> OldSearches
		{
			get
			{
				object @lock = this.m_Lock;
				ICollection<string> result;
				lock (@lock)
				{
					result = new List<string>(this.m_OldSearches);
				}
				return result;
			}
			set
			{
				object @lock = this.m_Lock;
				lock (@lock)
				{
					this.m_OldSearches = new List<string>(value);
				}
			}
		}

		public int TipIndex
		{
			get
			{
				object @lock = this.m_Lock;
				int tipIndex;
				lock (@lock)
				{
					tipIndex = this.m_TipIndex;
				}
				return tipIndex;
			}
			set
			{
				object @lock = this.m_Lock;
				lock (@lock)
				{
					this.m_TipIndex = value;
				}
			}
		}

		public int LastShowTipDay
		{
			get
			{
				object @lock = this.m_Lock;
				int lastShowTipDay;
				lock (@lock)
				{
					lastShowTipDay = this.m_LastShowTipDay;
				}
				return lastShowTipDay;
			}
			set
			{
				object @lock = this.m_Lock;
				lock (@lock)
				{
					this.m_LastShowTipDay = value;
				}
			}
		}

		public bool ShownWelcomeForm
		{
			get
			{
				object @lock = this.m_Lock;
				bool shownWelcomeForm;
				lock (@lock)
				{
					shownWelcomeForm = this.m_ShownWelcomeForm;
				}
				return shownWelcomeForm;
			}
			set
			{
				object @lock = this.m_Lock;
				lock (@lock)
				{
					this.m_ShownWelcomeForm = value;
				}
			}
		}

		public Settings()
		{
			this.SetupDefaultValues();
			this.m_AsyncWriteTask = new AsyncTask(new AsyncTask.TaskFunction(this.WriteTask), "Settings Write Thread");
		}

		public void Dispose()
		{
			if (this.m_AsyncWriteTask != null)
			{
				this.m_AsyncWriteTask.Dispose();
			}
		}

		public void Exit()
		{
			this.m_AsyncWriteTask.Exit();
			this.WriteInternal();
		}

		public void SetupDefaultValues()
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				this.m_FastFindControlSettings_Modal.SetDefaults();
				this.m_FastFindControlSettings.SetDefaults();
				this.m_FastFindFormSize = new Size(1323, 373);
				this.m_EMail = "";
				this.m_RegKey = "";
			}
		}

		public void Read()
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				try
				{
					string path = Settings.Filename;
					if (File.Exists(path))
					{
						XmlReadStream stream = new XmlReadStream();
						if (stream.Load(path) && stream.StartElement(VSAnythingPackage.m_ProductName))
						{
							this.Read(stream);
							stream.EndElement();
						}
					}
				}
				catch (Exception arg_4C_0)
				{
					Utils.LogExceptionQuiet(arg_4C_0);
				}
			}
		}

		public void Write()
		{
			this.m_AsyncWriteTask.Start(1000);
		}

		private void WriteTask(AsyncTask.Context context)
		{
			this.WriteInternal();
		}

		private void WriteInternal()
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				try
				{
					string path = Settings.Filename;
					string dir = Path.GetDirectoryName(path);
					if (!Directory.Exists(dir))
					{
						Directory.CreateDirectory(dir);
					}
					XmlWriteStream stream = new XmlWriteStream();
					stream.StartElement(VSAnythingPackage.m_ProductName);
					this.Write(stream);
					stream.EndElement();
					stream.Save(path);
				}
				catch (Exception arg_5A_0)
				{
					Utils.LogException(arg_5A_0);
				}
			}
		}

		private void Read(XmlReadStream stream)
		{
			stream.Read("FastFindFormSize", ref this.m_FastFindFormSize);
			stream.Read("EMail", ref this.m_EMail);
			stream.Read("RegKey", ref this.m_RegKey);
			stream.Read("SolutionFiles", ref this.m_SolutionFiles);
			stream.Read("OldSearches", ref this.m_OldSearches);
			stream.Read("TipIndex", ref this.m_TipIndex);
			stream.Read("LastShowTipDay", ref this.m_LastShowTipDay);
			stream.Read("ShownWelcomeForm", ref this.m_ShownWelcomeForm);
			this.m_FastFindControlSettings_Modal.Read("FastFindControlSettings", stream);
			this.m_FastFindControlSettings.Read("FastFindControlSettings_Docked", stream);
			bool disable_tips = false;
			if (stream.Read("DisableTips", ref disable_tips))
			{
				VSAnythingPackage.Inst.GetSettingsDialogPage().EnableTips = !disable_tips;
			}
			bool remember_last_find = false;
			if (stream.Read("FastFindRememberLastFind", ref remember_last_find))
			{
				VSAnythingPackage.Inst.GetSettingsDialogPage().RememberLastFind = remember_last_find;
			}
			bool spaces_as_wildcards = false;
			if (stream.Read("SpacesAsWildcardsForFindFile", ref spaces_as_wildcards))
			{
				VSAnythingPackage.Inst.GetSettingsDialogPage().SpacesAsWildcardsForFindFile = spaces_as_wildcards;
			}
			PathMode find_files_path_mode = PathMode.Relative;
			if (stream.ReadEnum<PathMode>("FindFilesPathMode", ref find_files_path_mode))
			{
				VSAnythingPackage.Inst.GetSettingsDialogPage().FindFilesPathMode = find_files_path_mode;
			}
			PathMode find_text_path_mode = PathMode.Relative;
			if (stream.ReadEnum<PathMode>("FindTextPathMode", ref find_text_path_mode))
			{
				VSAnythingPackage.Inst.GetSettingsDialogPage().FindTextPathMode = find_text_path_mode;
			}
			int maximum_file_size = 0;
			if (stream.Read("MaximumFileSize", ref maximum_file_size))
			{
				VSAnythingPackage.Inst.GetSettingsDialogPage().MaximumFileSize = maximum_file_size;
			}
			string font_name = null;
			if (stream.Read("FontName", ref font_name))
			{
				VSAnythingPackage.Inst.GetAppearanceDialogPage().FontName = font_name;
			}
			float font_size = 0f;
			if (stream.Read("FontSize", ref font_size))
			{
				VSAnythingPackage.Inst.GetAppearanceDialogPage().FontSize = font_size;
			}
			try
			{
				string colour_theme = null;
				if (stream.Read("ColourTheme", ref colour_theme))
				{
					VSAnythingPackage.Inst.GetAppearanceDialogPage().ColourTheme = (ColourTheme)Enum.Parse(typeof(ColourTheme), colour_theme);
				}
			}
			catch (Exception)
			{
			}
			List<string> ext_to_scan = null;
			if (stream.Read("ExtToScan", ref ext_to_scan))
			{
				VSAnythingPackage.Inst.GetSettingsDialogPage().ExtList = ext_to_scan.ToArray();
			}
			if (stream.StartElement("ColourThemes"))
			{
				for (int i = 0; i < stream.Count; i++)
				{
					stream.StartElement(i);
					string name = null;
					stream.Read("Name", ref name);
					if (name == "Custom")
					{
						new ColourSettings();
						Color back_colour = Color.Black;
						if (stream.Read("BackColour", ref back_colour))
						{
							VSAnythingPackage.Inst.GetAppearanceDialogPage().BackColour = back_colour;
						}
						Color fore_colour = Color.Black;
						if (stream.Read("ForeColour", ref fore_colour))
						{
							VSAnythingPackage.Inst.GetAppearanceDialogPage().ForeColour = fore_colour;
						}
						Color control_colour = Color.Black;
						if (stream.Read("ControlColour", ref control_colour))
						{
							VSAnythingPackage.Inst.GetAppearanceDialogPage().ControlColour = control_colour;
						}
						Color select_colour = Color.Black;
						if (stream.Read("SelectColour", ref select_colour))
						{
							VSAnythingPackage.Inst.GetAppearanceDialogPage().SelectColour = select_colour;
						}
						Color highlight_colour = Color.Black;
						if (stream.Read("HighlightColour", ref highlight_colour))
						{
							VSAnythingPackage.Inst.GetAppearanceDialogPage().HighlightColour = highlight_colour;
						}
						Color highlight_text_colour = Color.Black;
						if (stream.Read("HighlightTextColour", ref highlight_text_colour))
						{
							VSAnythingPackage.Inst.GetAppearanceDialogPage().HighlightTextColour = highlight_text_colour;
						}
						Color selected_highlight_text_colour = Color.Black;
						if (stream.Read("SelectedHighlightTextColour", ref selected_highlight_text_colour))
						{
							VSAnythingPackage.Inst.GetAppearanceDialogPage().SelectedHighlightTextColour = selected_highlight_text_colour;
						}
						Color code_colour = Color.Black;
						if (stream.Read("CodeColour", ref code_colour))
						{
							VSAnythingPackage.Inst.GetAppearanceDialogPage().CodeColour = code_colour;
						}
						Color code_filename_colour = Color.Black;
						if (stream.Read("CodeFilenameColour", ref code_filename_colour))
						{
							VSAnythingPackage.Inst.GetAppearanceDialogPage().CodeFilenameColour = code_filename_colour;
						}
					}
					stream.EndElement();
				}
				stream.EndElement();
			}
			VSAnythingPackage.Inst.GetAppearanceDialogPage().SaveSettingsToStorage();
		}

		private void Write(XmlWriteStream stream)
		{
			stream.Write("FastFindFormSize", this.m_FastFindFormSize);
			stream.Write<string>("EMail", this.m_EMail);
			stream.Write<string>("RegKey", this.m_RegKey);
			stream.Write("SolutionFiles", this.m_SolutionFiles);
			stream.Write("OldSearches", this.m_OldSearches);
			stream.Write<int>("TipIndex", this.m_TipIndex);
			stream.Write<int>("LastShowTipDay", this.m_LastShowTipDay);
			stream.Write<bool>("ShownWelcomeForm", this.m_ShownWelcomeForm);
			this.m_FastFindControlSettings_Modal.Write("FastFindControlSettings", stream);
			this.m_FastFindControlSettings.Write("FastFindControlSettings_Docked", stream);
		}

		private FastFindControlSettings GetControlSettings(bool modal)
		{
			if (!modal)
			{
				return this.m_FastFindControlSettings;
			}
			return this.m_FastFindControlSettings_Modal;
		}

		public string GetFastFindFileExt(bool modal)
		{
			object @lock = this.m_Lock;
			string fastFindFileExt;
			lock (@lock)
			{
				fastFindFileExt = this.GetControlSettings(modal).m_FastFindFileExt;
			}
			return fastFindFileExt;
		}

		public bool SetFastFindFileExt(string value, bool modal)
		{
			object @lock = this.m_Lock;
			bool result;
			lock (@lock)
			{
				if (this.GetControlSettings(modal).m_FastFindFileExt != value)
				{
					this.GetControlSettings(modal).m_FastFindFileExt = value;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool GetFastFindShowFiles(bool modal)
		{
			object @lock = this.m_Lock;
			bool fastFindShowFiles;
			lock (@lock)
			{
				fastFindShowFiles = this.GetControlSettings(modal).m_FastFindShowFiles;
			}
			return fastFindShowFiles;
		}

		public bool SetFastFindShowFiles(bool value, bool modal)
		{
			object @lock = this.m_Lock;
			bool result;
			lock (@lock)
			{
				if (this.GetControlSettings(modal).m_FastFindShowFiles != value)
				{
					this.GetControlSettings(modal).m_FastFindShowFiles = value;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool GetFastFindFindText(bool modal)
		{
			object @lock = this.m_Lock;
			bool fastFindFindText;
			lock (@lock)
			{
				fastFindFindText = this.GetControlSettings(modal).m_FastFindFindText;
			}
			return fastFindFindText;
		}

		public bool SetFastFindFindText(bool value, bool modal)
		{
			object @lock = this.m_Lock;
			bool result;
			lock (@lock)
			{
				if (this.GetControlSettings(modal).m_FastFindFindText != value)
				{
					this.GetControlSettings(modal).m_FastFindFindText = value;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool GetFastFindWildcards(bool modal)
		{
			object @lock = this.m_Lock;
			bool fastFindWildcards;
			lock (@lock)
			{
				fastFindWildcards = this.GetControlSettings(modal).m_FastFindWildcards;
			}
			return fastFindWildcards;
		}

		public bool SetFastFindWildcards(bool value, bool modal)
		{
			object @lock = this.m_Lock;
			bool result;
			lock (@lock)
			{
				if (this.GetControlSettings(modal).m_FastFindWildcards != value)
				{
					this.GetControlSettings(modal).m_FastFindWildcards = value;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool GetMatchWholeWord(bool modal)
		{
			object @lock = this.m_Lock;
			bool matchWholeWord;
			lock (@lock)
			{
				matchWholeWord = this.GetControlSettings(modal).m_MatchWholeWord;
			}
			return matchWholeWord;
		}

		public bool SetMatchWholeWord(bool value, bool modal)
		{
			object @lock = this.m_Lock;
			bool result;
			lock (@lock)
			{
				if (this.GetControlSettings(modal).m_MatchWholeWord != value)
				{
					this.GetControlSettings(modal).m_MatchWholeWord = value;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool GetRegExpression(bool modal)
		{
			object @lock = this.m_Lock;
			bool regExpression;
			lock (@lock)
			{
				regExpression = this.GetControlSettings(modal).m_RegExpression;
			}
			return regExpression;
		}

		public bool SetRegExpression(bool value, bool modal)
		{
			object @lock = this.m_Lock;
			bool result;
			lock (@lock)
			{
				if (this.GetControlSettings(modal).m_RegExpression != value)
				{
					this.GetControlSettings(modal).m_RegExpression = value;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool GetFindTextMatchCase(bool modal)
		{
			object @lock = this.m_Lock;
			bool findTextMatchCase;
			lock (@lock)
			{
				findTextMatchCase = this.GetControlSettings(modal).m_FindTextMatchCase;
			}
			return findTextMatchCase;
		}

		public bool SetFindTextMatchCase(bool value, bool modal)
		{
			object @lock = this.m_Lock;
			bool result;
			lock (@lock)
			{
				if (this.GetControlSettings(modal).m_FindTextMatchCase != value)
				{
					this.GetControlSettings(modal).m_FindTextMatchCase = value;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool GetUseLogicalOperators(bool modal)
		{
			object @lock = this.m_Lock;
			bool useLogicalOperators;
			lock (@lock)
			{
				useLogicalOperators = this.GetControlSettings(modal).m_UseLogicalOperators;
			}
			return useLogicalOperators;
		}

		public bool SetUseLogicalOperators(bool value, bool modal)
		{
			object @lock = this.m_Lock;
			bool result;
			lock (@lock)
			{
				if (this.GetControlSettings(modal).m_UseLogicalOperators != value)
				{
					this.GetControlSettings(modal).m_UseLogicalOperators = value;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool GetSolutionFilesMatchCase(bool modal)
		{
			object @lock = this.m_Lock;
			bool solutionFilesMatchCase;
			lock (@lock)
			{
				solutionFilesMatchCase = this.GetControlSettings(modal).m_SolutionFilesMatchCase;
			}
			return solutionFilesMatchCase;
		}

		public bool SetSolutionFilesMatchCase(bool value, bool modal)
		{
			object @lock = this.m_Lock;
			bool result;
			lock (@lock)
			{
				if (this.GetControlSettings(modal).m_SolutionFilesMatchCase != value)
				{
					this.GetControlSettings(modal).m_SolutionFilesMatchCase = value;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool GetOptionsPanelVisible(bool modal)
		{
			object @lock = this.m_Lock;
			bool optionsPanelVisible;
			lock (@lock)
			{
				optionsPanelVisible = this.GetControlSettings(modal).m_OptionsPanelVisible;
			}
			return optionsPanelVisible;
		}

		public void SetOptionsPanelVisible(bool value, bool modal)
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				this.GetControlSettings(modal).m_OptionsPanelVisible = value;
			}
		}

		public void AddSolutionFile(string solution_file)
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				this.m_SolutionFiles.Add(solution_file);
			}
		}
	}
}
