using System;
using System.Drawing;

namespace Company.VSAnything
{
	internal class InfoListBoxItem
	{
		private string m_Message;

		private Brush m_Brush;

		public string Message
		{
			get
			{
				return this.m_Message;
			}
			set
			{
				this.m_Message = value;
			}
		}

		public Brush Brush
		{
			get
			{
				return this.m_Brush;
			}
			set
			{
				this.m_Brush = value;
			}
		}

		public InfoListBoxItem(string message, Brush brush)
		{
			this.m_Message = message;
			this.m_Brush = brush;
		}
	}
}
