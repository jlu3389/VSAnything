using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;


using Registration;
using SCLCoreCLR;
using EnvDTE;

namespace Company.VSAnything
{
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]  
    [Guid(GuidList.guidVSAnythingPkgString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // ProvideOptionPage(typeof(SettingsDialogPage), "FastFind", "Settings", 106, 107, true), ProvideOptionPage(typeof(AppearanceDialogPage), "FastFind", "Appearance", 106, 108, true), ProvideProfile(typeof(SettingsDialogPage), "FastFind", "Settings", 106, 107, true, DescriptionResourceID = 109), 
    //ProvideProfile(typeof(AppearanceDialogPage), "FastFind", "Appearance", 106, 108, true, DescriptionResourceID = 110), 
    [ProvideToolWindow(typeof(FastFindToolWindowPane))]

    public partial class VSAnythingPackage : Package, IVsSolutionEvents, IVsSolutionLoadEvents, IVsHierarchyEvents, IDisposable
	{
		private class ActivateDocEvent
		{
			public bool m_SetLine;

			public int m_RequestTime;

			public string m_Filename;

			public int m_Line = -1;
		}

		private struct HierarchyEventInfo
		{
			public uint m_Cookie;
		}

		private static VSAnythingPackage m_Inst;

		private DTE m_DTE;


		public static string m_ProductName = "VSAnything";




		private const int m_ScanDelay = 1000;


		private FastFindCmd m_FastFindCmd = new FastFindCmd();

		private FastFindWindowCmd m_FastFindWindowCmd = new FastFindWindowCmd();

		private SourceHeaderToggleCmd m_SourceHeaderToggleCmd = new SourceHeaderToggleCmd();

		private SettingsCmd m_SettingsCmd = new SettingsCmd();

		private Settings m_Settings = new Settings();

		private IVsSolution2 m_Solution;

		private uint m_SolutionEventsCookie;

		private DocumentEvents m_DocumentEvents;

		private SolutionEvents m_SolutionEvents;

		private WindowEvents m_WindowEvents;

        private Dictionary<IVsHierarchy, VSAnythingPackage.HierarchyEventInfo> m_HierarchyEventMap = new Dictionary<IVsHierarchy, VSAnythingPackage.HierarchyEventInfo>();

		private SolutionFiles m_SolutionFiles;

		private FileFinder m_FileFinder;

		private TextFinder m_TextFinder;

		private GetOpenFilesThread m_GetOpenFilesThread;

		private bool m_SolutionLoaded;

		private const int m_ActivateDocHackTimeout = 200;

		private const int m_ForceActivateDocHackCount = 2;

        private Dictionary<string, VSAnythingPackage.ActivateDocEvent> m_ActivateDocEvents = new Dictionary<string, VSAnythingPackage.ActivateDocEvent>();

        private VSAnythingPackage.ActivateDocEvent m_ForceActivateDocEvent;

		private int m_ForceActivateDocCount;



        public static VSAnythingPackage Inst
		{
			get
			{
				return VSAnythingPackage.m_Inst;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			try
			{
				Log.OpenFile(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VSAnything\\VSAnything.log");
				VSAnythingPackage.m_Inst = this;
				Log.WriteLine("----------------------------------------------------------");
				Log.WriteLine("FastFind Initialise");
				Log.WriteLine("FastFind Version: 4.8");
				this.m_Settings.Read();
				EnvDTE.DTE env_dte = (EnvDTE.DTE)base.GetService(typeof(SDTE));
				string vs_version;
				try
				{
					vs_version = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
				}
				catch (Exception e)
				{
					vs_version = "Error getting VS version: " + e.Message;
				}
				Log.WriteLine("Visual Studio Version: " + vs_version);
				this.m_DTE = new DTE(env_dte);
				this.m_TextFinder = new TextFinder();
				this.m_FileFinder = new FileFinder(this.m_Settings);
				this.m_FileFinder.SetSolutionFiles(new List<string>(this.m_Settings.SolutionFiles));
				this.m_SolutionFiles = new SolutionFiles(this.m_DTE);
				this.m_GetOpenFilesThread = new GetOpenFilesThread(env_dte);
				this.m_SolutionFiles.SolutionFileListChanged += new SolutionFiles.SolutionFileListChangedHandler(this.SolutionFilesChanged);
				this.m_DocumentEvents = env_dte.Events.get_DocumentEvents(null);
				this.m_SolutionEvents = env_dte.Events.SolutionEvents;
				this.m_WindowEvents = env_dte.Events.get_WindowEvents(null);

                //mariotodo 改成下面那样，不知道对不对
                //this.m_DocumentEvents.DocumentSaved += new _dispDocumentEvents_DocumentSavedEventHandler(this, (UIntPtr)System.Reflection.Emit.OpCodes.Ldftn(DocumentSaved));
                //this.m_SolutionEvents.ProjectAdded += new _dispSolutionEvents_ProjectAddedEventHandler(this, (UIntPtr)ldftn(ProjectAddedOrRemoved));
                //this.m_SolutionEvents.ProjectRemoved += new _dispSolutionEvents_ProjectRemovedEventHandler(this, (UIntPtr)ldftn(ProjectAddedOrRemoved));
                //this.m_WindowEvents.WindowActivated += new _dispWindowEvents_WindowActivatedEventHandler(this, (UIntPtr)ldftn(WindowActivated));

                this.m_DocumentEvents.DocumentSaved += new _dispDocumentEvents_DocumentSavedEventHandler(this.DocumentSaved);
                this.m_SolutionEvents.ProjectAdded += new _dispSolutionEvents_ProjectAddedEventHandler(this.ProjectAddedOrRemoved);
                this.m_SolutionEvents.ProjectRemoved += new _dispSolutionEvents_ProjectRemovedEventHandler(this.ProjectAddedOrRemoved);
                this.m_WindowEvents.WindowActivated += new _dispWindowEvents_WindowActivatedEventHandler(this.WindowActivated);

				FastFindToolWindowPane.Initialise(this.m_DTE, this.m_SolutionFiles, this.m_FileFinder, this.m_TextFinder, this.m_GetOpenFilesThread, this.m_Settings);
				OleMenuCommandService mcs = base.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
				this.m_FastFindCmd.Initialise(this.m_DTE, this.m_SolutionFiles, this.m_FileFinder, this.m_TextFinder, this.m_GetOpenFilesThread, mcs, this.m_Settings);
				this.m_FastFindWindowCmd.Initialise(this, this.m_DTE, mcs, this.m_Settings);
				this.m_SourceHeaderToggleCmd.Initialise(this.m_DTE, this.m_SolutionFiles, mcs);
				this.m_SettingsCmd.Initialise(mcs);
				this.m_Solution = (base.GetService(typeof(SVsSolution)) as IVsSolution2);
				if (this.m_Solution != null)
				{
					int ret = this.m_Solution.AdviseSolutionEvents(this, out this.m_SolutionEventsCookie);
					Log.WriteLine("AdviseSolutionEvents returned " + ret);
				}
				this.AddSolutionFileToSettings();
				if (this.m_DTE.EnvDTE.Solution != null)
				{
					foreach (Project project in this.m_DTE.EnvDTE.Solution.Projects)
					{
						IVsHierarchy pHierarchy = null;
						if (this.m_Solution.GetProjectOfUniqueName(project.UniqueName, out pHierarchy) == 0 && pHierarchy != null)
						{
							this.AdviseHierarchyEvents(pHierarchy);
						}
					}
				}
				if (!this.m_Settings.ShownWelcomeForm)
				{
					new System.Threading.Thread(new ThreadStart(this.WelcomeThread)).Start();
				} 
			}
			catch (Exception arg_368_0)
			{
				Utils.LogException(arg_368_0);
			}
		}

        private VSAnythingPackage.ActivateDocEvent GetActivateDocEvent(string doc_name)
		{
			if (doc_name == null)
			{
				return null;
			}
			VSAnythingPackage.ActivateDocEvent active_doc_event;
			if (this.m_ActivateDocEvents.TryGetValue(doc_name, out active_doc_event))
			{
				if (active_doc_event.m_SetLine || Environment.TickCount - active_doc_event.m_RequestTime < 200)
				{
					return active_doc_event;
				}
				this.m_ActivateDocEvents.Remove(doc_name);
			}
			return null;
		}

		private void WindowActivated(Window GotFocus, Window LostFocus)
		{
			if (GotFocus.Caption == VSAnythingPackage.m_ProductName)
			{
                //this.m_FastFindWindowCmd.SelectText(false);
			}
			string doc_name = null;
			if (GotFocus != null && GotFocus.Document != null)
			{
				doc_name = Utils.NormalisePathAndLowerCase(GotFocus.Document.FullName);
			}
			VSAnythingPackage.ActivateDocEvent active_doc_event = this.GetActivateDocEvent(doc_name);
			if (active_doc_event != null && Utils.NormalisePathAndLowerCase(this.m_DTE.GetActiveDocumentFilename()) == active_doc_event.m_Filename && active_doc_event.m_SetLine)
			{
				if (active_doc_event.m_Line != -1)
				{
					this.m_DTE.SetActiveDocumentLine(active_doc_event.m_Line);
				}
				active_doc_event.m_SetLine = false;
			}
			if (this.m_ForceActivateDocEvent != null && doc_name != this.m_ForceActivateDocEvent.m_Filename)
			{
				if (Environment.TickCount - this.m_ForceActivateDocEvent.m_RequestTime < 200 && this.m_ForceActivateDocCount > 0)
				{
					this.m_ForceActivateDocCount--;
					this.m_ForceActivateDocEvent.m_SetLine = true;
					this.m_DTE.OpenFile(this.m_ForceActivateDocEvent.m_Filename);
					return;
				}
				this.m_ForceActivateDocEvent = null;
			}
		}

		public void ActivateDoc(string filename, int line, bool force)
		{
			VSAnythingPackage.ActivateDocEvent activation_event = new VSAnythingPackage.ActivateDocEvent();
			filename = Utils.NormalisePathAndLowerCase(filename);
			activation_event.m_SetLine = true;
			activation_event.m_RequestTime = Environment.TickCount;
			activation_event.m_Filename = filename;
			activation_event.m_Line = line;
			this.m_ActivateDocEvents[activation_event.m_Filename] = activation_event;
			if (force)
			{
				this.m_ForceActivateDocCount = 2;
				this.m_ForceActivateDocEvent = activation_event;
			}
		}

		private void WelcomeThread()
		{
			System.Threading.Thread.Sleep(5000);
			new WelcomeForm().ShowDialog();
			this.m_Settings.ShownWelcomeForm = true;
			this.m_Settings.Write();
		}

		private void AdviseHierarchyEvents(IVsHierarchy pHierarchy)
		{
			try
			{
				object item;
				if (pHierarchy.GetProperty(4294967294u, -2012, out item) == 0 && item != null && item.ToString() == "Performance")
				{
					return;
				}
			}
			catch (Exception arg_2A_0)
			{
				Utils.LogExceptionQuiet(arg_2A_0);
			}
			if (!this.m_HierarchyEventMap.ContainsKey(pHierarchy))
			{
				uint cookie = 0u;
				pHierarchy.AdviseHierarchyEvents(this, out cookie);
				VSAnythingPackage.HierarchyEventInfo info = default(VSAnythingPackage.HierarchyEventInfo);
				info.m_Cookie = cookie;
				this.m_HierarchyEventMap[pHierarchy] = info;
			}
		}

		private void UnadviseHierarchyEvents(IVsHierarchy pHierarchy)
		{
			if (this.m_HierarchyEventMap.ContainsKey(pHierarchy))
			{
				try
				{
					pHierarchy.UnadviseHierarchyEvents(this.m_HierarchyEventMap[pHierarchy].m_Cookie);
				}
				catch (Exception arg_28_0)
				{
					Log.WriteLine(arg_28_0.Message);
				}
				this.m_HierarchyEventMap.Remove(pHierarchy);
			}
		}

		private void SolutionFilesChanged(ICollection<string> solution_files, ICollection<string> projects)
		{
			this.UpdateFileFinderSolutionFiles(solution_files, projects);
		}

		private void UpdateFileFinderSolutionFiles(ICollection<string> solution_files, ICollection<string> projects)
		{
			List<string> all_solution_files = new List<string>(solution_files);
			all_solution_files.AddRange(this.m_Settings.SolutionFiles);
			this.m_FileFinder.SetSolutionFiles(all_solution_files);
			this.m_TextFinder.SetSolutionFiles(projects, solution_files);
			this.m_TextFinder.StartFileScan();
		}

		private void ShowError(string message)
		{
			string text = "There is a problem with the FastFind Registration. Please uninstall and then reinstall FastFind.";
			if (message != null)
			{
				text = text + "\n\nDetails:\n" + message;
			}
		}

		private void ProjectAddedOrRemoved(Project project)
		{
			Log.WriteLine("Project added or removed: " + project.FullName);
			if (this.m_SolutionLoaded)
			{
				this.m_SolutionFiles.UpdateSolutionFileList(1000);
			}
		}

		private void DocumentSaved(Document Document)
		{
			Log.WriteLine("Document saved: " + Document.FullName);
			try
			{
				this.m_TextFinder.ScanFile(Document.FullName);
			}
			catch (Exception arg_28_0)
			{
				Utils.LogExceptionQuiet(arg_28_0);
			}
		}

		public void Dispose()
		{
			if (this.m_FileFinder != null)
			{
				this.m_FileFinder.Dispose();
				this.m_FileFinder = null;
			}
			if (this.m_GetOpenFilesThread != null)
			{
				this.m_GetOpenFilesThread.Dispose();
				this.m_GetOpenFilesThread = null;
			}
			if (this.m_SolutionFiles != null)
			{
				this.m_SolutionFiles.Dispose();
				this.m_SolutionFiles = null;
			}
			if (this.m_Settings != null)
			{
				this.m_Settings.Dispose();
				this.m_Settings = null;
			}
			if (this.m_TextFinder != null)
			{
				this.m_TextFinder.Dispose();
				this.m_TextFinder = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				try
				{
				}
				catch (Exception)
				{
				}
				this.m_GetOpenFilesThread.Exit();
				this.m_TextFinder.Exit();
				this.m_SolutionFiles.Exit();
				this.m_Settings.Exit();
				FastFindToolWindowPane.Destroy();
				if (this.m_Solution != null && this.m_SolutionEventsCookie != 0u)
				{
					this.m_Solution.UnadviseSolutionEvents(this.m_SolutionEventsCookie);
				}
				this.Dispose();
			}
			catch (Exception e)
			{
				Log.Write(e.Message + "\n" + e.StackTrace);
			}
			base.Dispose(disposing);
		}

		private void SolutionLoadComplete()
		{
			this.StartSolutionScan();
		}

		private void StartSolutionScan()
		{
			Log.WriteLine("--SolutionLoadComplete--");
			this.m_SolutionLoaded = true;
			this.m_SolutionFiles.UpdateSolutionFileList(1000);
			this.m_TextFinder.SetSolutionPath(this.m_SolutionFiles.SolutionFilename);
			this.UpdateFileFinderSolutionFiles(this.m_SolutionFiles.Files, this.m_SolutionFiles.Projects);
			this.AddSolutionFileToSettings();
		}

		private void SolutionClosed()
		{
			this.m_SolutionFiles.OnSolutionClose();
			this.m_SolutionLoaded = false;
			this.m_TextFinder.SetSolutionPath(null);
		}

		public int OnAfterCloseSolution(object pUnkReserved)
		{
			Log.WriteLine("Solution closed");
			this.SolutionClosed();
			return 0;
		}

		public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			Log.WriteLine("OnAfterLoadProject");
			if (this.m_SolutionLoaded)
			{
				this.m_SolutionFiles.UpdateSolutionFileList(1000);
			}
			return 0;
		}

		public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
		{
			Log.WriteLine("OnAfterOpenProject");
			this.AdviseHierarchyEvents(pHierarchy);
			if (this.m_SolutionLoaded)
			{
				this.m_SolutionFiles.UpdateSolutionFileList(1000);
			}
			return 0;
		}

		public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
		{
			Log.WriteLine("OnAfterOpenSolution");
			object pvar;
			this.m_Solution.GetProperty(-8031, out pvar);
			bool fully_loaded = (bool)pvar;
			Log.WriteLine("fully_loaded: " + fully_loaded.ToString());
			if (fully_loaded)
			{
				this.SolutionLoadComplete();
			}
			return 0;
		}

		public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
		{
			Log.WriteLine("OnBeforeCloseProject");
			this.UnadviseHierarchyEvents(pHierarchy);
			return 0;
		}

		public int OnBeforeCloseSolution(object pUnkReserved)
		{
			this.SolutionClosed();
			return 0;
		}

		public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
		{
			return 0;
		}

		public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
		{
			return 0;
		}

		public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
		{
			return 0;
		}

		public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
		{
			return 0;
		}

		public int OnAfterBackgroundSolutionLoadComplete()
		{
			Log.WriteLine("OnAfterBackgroundSolutionLoadComplete");
			this.SolutionLoadComplete();
			return 0;
		}

		public int OnAfterLoadProjectBatch(bool fIsBackgroundIdleBatch)
		{
			Log.WriteLine("OnAfterLoadProjectBatch");
			if (this.m_SolutionLoaded)
			{
				this.m_SolutionFiles.UpdateSolutionFileList(1000);
			}
			return 0;
		}

		public int OnBeforeBackgroundSolutionLoadBegins()
		{
			Log.WriteLine("OnBeforeBackgroundSolutionLoadBegins");
			this.m_SolutionLoaded = false;
			return 0;
		}

		public int OnBeforeLoadProjectBatch(bool fIsBackgroundIdleBatch)
		{
			return 0;
		}

		public int OnBeforeOpenSolution(string pszSolutionFilename)
		{
			Log.WriteLine("OnBeforeOpenSolution");
			return 0;
		}

		public int OnQueryBackgroundLoadProjectBatch(out bool pfShouldDelayLoadToNextIdle)
		{
			pfShouldDelayLoadToNextIdle = false;
			return 0;
		}

		private void AddSolutionFileToSettings()
		{
			string solution_filename = this.m_SolutionFiles.SolutionFilename;
			if (!string.IsNullOrEmpty(solution_filename))
			{
				string solution_filename_lwr = solution_filename.ToLower();
				using (IEnumerator<string> enumerator = this.m_Settings.SolutionFiles.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.ToLower() == solution_filename_lwr)
						{
							return;
						}
					}
				}
				this.m_Settings.AddSolutionFile(solution_filename);
				this.m_Settings.Write();
			}
		}

		public int OnInvalidateIcon(IntPtr hicon)
		{
			return 0;
		}

		public int OnInvalidateItems(uint itemidParent)
		{
			return 0;
		}

		private ProjectItem GetProjectItemFromHeirachy(uint item_id)
		{
			foreach (IVsHierarchy hierarchy in this.m_HierarchyEventMap.Keys)
			{
				try
				{
					object item;
					if (hierarchy.GetProperty(item_id, -2027, out item) == 0)
					{
						return item as ProjectItem;
					}
				}
				catch (Exception arg_36_0)
				{
					Log.WriteLine(arg_36_0.Message);
				}
			}
			return null;
		}

		private string GetFilenamePromProjectItem(uint item_id)
		{
			ProjectItem item = this.GetProjectItemFromHeirachy(item_id);
			if (item == null || item.FileCount == 0)
			{
				return null;
			}
			return item.get_FileNames(0);
		}

		public int OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded)
		{
			if (this.m_SolutionLoaded)
			{
				this.m_SolutionFiles.UpdateSolutionFileList(100);
				string path = this.GetFilenamePromProjectItem(itemidAdded);
				if (path != null)
				{
					this.m_TextFinder.ScanFile(path);
				}
			}
			return 0;
		}

		public int OnItemDeleted(uint itemid)
		{
			Log.WriteLine("OnItemDeleted");
			if (this.m_SolutionLoaded)
			{
				this.m_SolutionFiles.UpdateSolutionFileList(100);
			}
			return 0;
		}

		public int OnItemsAppended(uint itemidParent)
		{
			return 0;
		}

		public int OnPropertyChanged(uint itemid, int propid, uint flags)
		{
			string path = this.GetFilenamePromProjectItem(itemid);
			if (path != null && File.Exists(path))
			{
				this.m_TextFinder.ScanFile(path);
			}
			return 0;
		}

		public void ActivateFastFindDockedWindow()
		{
			this.m_FastFindWindowCmd.Show();
		}

		public void OnSettingsExtListChanged()
		{
			this.StartSolutionScan();
		}

		internal SettingsDialogPage GetSettingsDialogPage()
		{
			return (SettingsDialogPage)base.GetDialogPage(typeof(SettingsDialogPage));
		}

		internal AppearanceDialogPage GetAppearanceDialogPage()
		{
			return (AppearanceDialogPage)base.GetDialogPage(typeof(AppearanceDialogPage));
		}
	}
}
