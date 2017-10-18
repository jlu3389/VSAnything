using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace Company.VSAnything
{
	internal class SettingsCmd
	{
		public void Initialise(OleMenuCommandService mcs)
		{
            CommandID menuCommandID = new CommandID(GuidList.guidVSAnythingCmdSet, 259);
			MenuCommand menuItem = new MenuCommand(new EventHandler(this.MenuItemCallback), menuCommandID);
			mcs.AddCommand(menuItem);
		}

		private void MenuItemCallback(object sender, EventArgs e)
		{
			new SettingsDialog().ShowDialog();
		}
	}
}
