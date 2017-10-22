using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Company.VSAnything
{
	internal class FastFindForm : Form
	{
		private FastFindControl m_FastFindControl;

		private Settings m_Settings;

		private static string m_LastFindText;

		private static object m_LastSelectedItem;

		private IContainer components;

		public FastFindForm(DTE dte, SolutionFiles solution_files, FileFinder file_finder, TextFinder text_finder, GetOpenFilesThread get_open_files_thread, Settings settings, string initial_text)
		{
			this.InitializeComponent();
			this.m_Settings = settings;
			base.Size = settings.FastFindFormSize;
			SettingsDialogPage settings_page = VSAnythingPackage.Inst.GetSettingsDialogPage();
			if (string.IsNullOrEmpty(initial_text) && settings_page.RememberLastFind && !string.IsNullOrEmpty(FastFindForm.m_LastFindText))
			{
				initial_text = FastFindForm.m_LastFindText;
			}
			bool is_modal = true;
			this.m_FastFindControl = new FastFindControl(dte, solution_files, file_finder, text_finder, get_open_files_thread, settings, initial_text, FastFindForm.m_LastSelectedItem, is_modal);
			this.m_FastFindControl.Dock = DockStyle.Fill;
			this.m_FastFindControl.ControlWantsToClose += new FastFindControl.ControlWantsToCloseHandler(this.FastFindControlWantsToClose);
			base.Controls.Add(this.m_FastFindControl);
			this.m_FastFindControl.OnActivated();
		}

		private void FastFindControlWantsToClose()
		{
			base.Close();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			FastFindForm.m_LastFindText = this.m_FastFindControl.TextBoxText;
			FastFindForm.m_LastSelectedItem = this.m_FastFindControl.SelectedItem;
			base.OnClosing(e);
		}

		protected override void OnResize(EventArgs e)
		{
			if (this.m_Settings != null)
			{
				this.m_Settings.FastFindFormSize = base.Size;
				this.m_Settings.Write();
			}
			base.OnResize(e);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			this.m_FastFindControl.OnMouseWheel(e.Delta);
			base.OnMouseWheel(e);
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(FastFindForm));
			base.SuspendLayout();
			//base.AutoScaleDimensions = new SizeF(6f, 13f);
			//base.AutoScaleMode = AutoScaleMode.Font;
			//base.ClientSize = new Size(720, 442);
			//base.FormBorderStyle = FormBorderStyle.SizableToolWindow;
			base.Icon = (Icon)resources.GetObject("$this.Icon");
			base.Name = "FastFindForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = VSAnythingPackage.m_ProductName;
			base.ResumeLayout(false);
		}
	}
}
