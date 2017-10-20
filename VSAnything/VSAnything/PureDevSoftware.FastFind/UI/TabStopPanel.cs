using System;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace Company.VSAnything
{
	internal class TabStopPanel : Panel
	{
		internal HwndHost WpfHost
		{
			get;
			set;
		}

		public TabStopPanel()
		{
			this.Dock = DockStyle.Fill;
			base.TabStop = false;
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			return base.ProcessDialogKey(keyData) || ((keyData & (Keys.Control | Keys.Alt)) == Keys.None && (keyData & Keys.KeyCode) == Keys.Tab && ((IKeyboardInputSink)this.WpfHost).KeyboardInputSite.OnNoMoreTabStops(new TraversalRequest(((keyData & Keys.Shift) == Keys.None) ? FocusNavigationDirection.Next : FocusNavigationDirection.Previous)));
		}
	}
}
