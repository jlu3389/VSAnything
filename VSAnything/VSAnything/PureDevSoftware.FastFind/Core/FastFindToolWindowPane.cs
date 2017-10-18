using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace Company.VSAnything
{
	[Guid("6AF1F5E3-CF53-4AC4-8AB1-C5EEECCCD081")]
	internal class FastFindToolWindowPane : ToolWindowPane, IOleCommandTarget
	{
		private static Guid m_VisualStudioCommandGroup = Guid.Parse("5efc7975-14bc-11cf-9b2b-00aa00573819");

		private const int m_GotoNextLocationCmdId = 279;

		private const int m_GotoPrevLocationCmdId = 280;

		private static DTE m_DTE;

		private static SolutionFiles m_SolutionFiles;

		private static FileFinder m_FileFinder;

		private static TextFinder m_TextFinder;

		private static GetOpenFilesThread m_GetOpenFilesThread;

		private static Settings m_Settings;

		private FastFindControlWPFWrapper m_Control;

		public static void Initialise(DTE dte, SolutionFiles solution_files, FileFinder file_finder, TextFinder text_finder, GetOpenFilesThread get_open_files_thread, Settings settings)
		{
			FastFindToolWindowPane.m_DTE = dte;
			FastFindToolWindowPane.m_SolutionFiles = solution_files;
			FastFindToolWindowPane.m_FileFinder = file_finder;
			FastFindToolWindowPane.m_TextFinder = text_finder;
			FastFindToolWindowPane.m_GetOpenFilesThread = get_open_files_thread;
			FastFindToolWindowPane.m_Settings = settings;
		}

		public static void Destroy()
		{
			FastFindToolWindowPane.m_DTE = null;
			FastFindToolWindowPane.m_SolutionFiles = null;
			FastFindToolWindowPane.m_FileFinder = null;
			FastFindToolWindowPane.m_TextFinder = null;
			FastFindToolWindowPane.m_GetOpenFilesThread = null;
			FastFindToolWindowPane.m_Settings = null;
		}

		public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
		{
			if (pguidCmdGroup == FastFindToolWindowPane.m_VisualStudioCommandGroup)
			{
				if (nCmdID == 279u)
				{
					this.m_Control.FastFindControl.GotoNextLocation();
					return 0;
				}
				if (nCmdID == 280u)
				{
					this.m_Control.FastFindControl.GotoPrevLocation();
					return 0;
				}
			}
			return -2147221248;
		}

		public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
		{
			if (pguidCmdGroup == FastFindToolWindowPane.m_VisualStudioCommandGroup)
			{
				uint cmdID = prgCmds[0].cmdID;
				if (cmdID - 279u <= 1u)
				{
					prgCmds[0].cmdf = 3u;
					return 0;
				}
			}
			return -2147221248;
		}

		public FastFindToolWindowPane() : base(null)
		{
            //mariotodo
            //base.set_Caption("Fast Find");
            //base.set_BitmapResourceID(301);
            //base.set_BitmapIndex(0);
            //this.m_Control = new FastFindControlWPFWrapper(FastFindToolWindowPane.m_DTE, FastFindToolWindowPane.m_SolutionFiles, FastFindToolWindowPane.m_FileFinder, FastFindToolWindowPane.m_TextFinder, FastFindToolWindowPane.m_GetOpenFilesThread, FastFindToolWindowPane.m_Settings, this);
            //base.set_Content(this.m_Control);
		}

		public override void OnToolWindowCreated()
		{
			base.OnToolWindowCreated();
			(this.GetService(typeof(IVsTrackSelectionEx)) as IVsTrackSelectionEx).OnElementValueChange(6u, 0, this);
		}
	}
}
