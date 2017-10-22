using SCLCoreCLR;
using System;

namespace Company.VSAnything
{
	internal class ProfileTimer
	{
		private string m_Name;

		private int m_StartTime;

		public ProfileTimer(string name)
		{
			this.m_Name = name;
			this.m_StartTime = Environment.TickCount;
		}

		public void Stop()
		{
			int el = Environment.TickCount - this.m_StartTime;
			Log.WriteLine(string.Concat(new object[]
			{
				"Timer ",
				this.m_Name,
				": ",
				el
			}));
		}
	}
}
