using Microsoft.VisualStudio.Shell;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Markup;

namespace Company.VSAnything
{
	public partial class FastFindControlWPFWrapper : System.Windows.Controls.UserControl, IDisposable, IComponentConnector
	{
		private FastFindControl m_FastFindControl;

		private DTE m_DTE;

		internal FastFindControl FastFindControl
		{
			get
			{
				return this.m_FastFindControl;
			}
		}

		internal FastFindControlWPFWrapper(DTE dte, SolutionFiles solution_files, FileFinder file_finder, TextFinder text_finder, GetOpenFilesThread get_open_files_thread, Settings settings, WindowPane window_pane_owner)
		{
			this.InitializeComponent();
			this.m_DTE = dte;
			this.m_TabStopPanel.WpfHost = this.m_WindowsFormsHost;
			bool is_modal = false;
			this.m_FastFindControl = new FastFindControl(dte, solution_files, file_finder, text_finder, get_open_files_thread, settings, "", -1, is_modal);
			this.m_FastFindControl.Dock = DockStyle.Fill;
			this.m_TabStopPanel.Controls.Add(this.m_FastFindControl);
		}

		public void Dispose()
		{
			this.m_FastFindControl.Dispose();
		}

		private void FastFindDockedWindowGotFocus(object sender, RoutedEventArgs e)
		{
			this.m_FastFindControl.FocusTextBox();
		}
	}
}
