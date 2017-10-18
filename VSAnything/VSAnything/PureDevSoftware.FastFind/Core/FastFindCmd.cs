using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace Company.VSAnything
{
	internal class FastFindCmd
	{
		private DTE m_DTE;

		private SolutionFiles m_SolutionFiles;

		private FileFinder m_FileFinder;

		private TextFinder m_TextFinder;

		private GetOpenFilesThread m_GetOpenFilesThread;

		private Settings m_Settings;

		public void Initialise(DTE dte, SolutionFiles solution_files, FileFinder file_finder, TextFinder text_finder, GetOpenFilesThread get_open_files_thread, OleMenuCommandService mcs, Settings settings)
		{
			this.m_DTE = dte;
			this.m_SolutionFiles = solution_files;
			this.m_FileFinder = file_finder;
			this.m_TextFinder = text_finder;
			this.m_GetOpenFilesThread = get_open_files_thread;
			this.m_Settings = settings;
			if (mcs != null)
			{
                CommandID menuCommandID = new CommandID(GuidList.guidVSAnythingCmdSet, 256);
				MenuCommand menuItem = new MenuCommand(new EventHandler(this.MenuItemCallback), menuCommandID);
				mcs.AddCommand(menuItem);
			}
		}

		private void MenuItemCallback(object sender, EventArgs e)
		{
			try
			{
				if (VSAnythingPackage.Inst.CheckRegistration())
				{
					string initial_text = this.m_DTE.GetSelectedText();
					new FastFindForm(this.m_DTE, this.m_SolutionFiles, this.m_FileFinder, this.m_TextFinder, this.m_GetOpenFilesThread, this.m_Settings, initial_text).ShowDialog();
				}
			}
			catch (Exception arg_4C_0)
			{
				Utils.LogException(arg_4C_0);
			}
		}
	}
}
