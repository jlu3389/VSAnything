using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Company.VSAnything
{
	internal class FastFindTextBox : TextBox
	{
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EscapeKeyPressedHandler EscapeKeyPressed;

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape && this.EscapeKeyPressed != null)
			{
				this.EscapeKeyPressed();
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
