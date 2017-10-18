using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;

namespace Company.VSAnything
{
	internal class FastFindWindowCmd
	{
		private Package m_Package;

		private DTE m_DTE;

		private Settings m_Settings;

		private bool m_InShow;

		public void Initialise(Package package, DTE dte, OleMenuCommandService mcs, Settings settings)
		{
			this.m_Package = package;
			this.m_DTE = dte;
			this.m_Settings = settings;
			if (mcs != null)
			{
                CommandID menuCommandID = new CommandID(GuidList.guidVSAnythingCmdSet, 257);
				MenuCommand menuItem = new MenuCommand(new EventHandler(this.MenuItemCallback), menuCommandID);
				mcs.AddCommand(menuItem);
			}
		}

		private void MenuItemCallback(object sender, EventArgs e)
		{
			this.Show();
			this.SelectText(true);
		}

		public void Show()
		{
			if (this.m_InShow)
			{
				return;
			}
			this.m_InShow = true;
			if (!VSAnythingPackage.Inst.CheckRegistration())
			{
				FastFindControl fastfind_control = this.GetFastFindControl();
				if (fastfind_control != null)
				{
					fastfind_control.Enabled = false;
				}
				this.m_InShow = false;
				return;
			}
			ToolWindowPane window = this.m_Package.FindToolWindow(typeof(FastFindToolWindowPane), 0, true);
            //mariotodo
            //if (window != null && window.get_Frame() != null)
            //{
            //    ErrorHandler.ThrowOnFailure(((IVsWindowFrame)window.get_Frame()).Show());
            //    FastFindControl fastfind_control2 = this.GetFastFindControl();
            //    if (fastfind_control2 != null)
            //    {
            //        fastfind_control2.OnActivated();
            //    }
            //}
			this.m_InShow = false;
		}

		private FastFindControl GetFastFindControl()
		{
            //mariotodo
            //ToolWindowPane window = this.m_Package.FindToolWindow(typeof(FastFindToolWindowPane), 0, true);
            //if (window != null && window.get_Frame() != null)
            //{
            //    return ((FastFindControlWPFWrapper)window.get_Content()).FastFindControl;
            //}
			return null;
		}

		public void SelectText(bool get_selected_text)
		{
			FastFindControl fastfind_control = this.GetFastFindControl();
			if (fastfind_control != null)
			{
				if (get_selected_text)
				{
					string initial_text = this.m_DTE.GetSelectedText();
					if (!string.IsNullOrEmpty(initial_text))
					{
						fastfind_control.TextBoxText = initial_text;
					}
				}
				fastfind_control.SelectText();
			}
		}
	}
}
