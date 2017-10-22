using System;
using System.Runtime.InteropServices;

namespace Company.VSAnything
{
	internal class NativeMethods
	{
		[DllImport("user32.dll")]
		public static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
	}
}
