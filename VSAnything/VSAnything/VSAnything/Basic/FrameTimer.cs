using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SCLCoreCLR
{
	public class FrameTimer
	{
		private string m_Name;

		private Stopwatch m_Stopwatch = new Stopwatch();

		private bool m_Started;

		private int m_Count;

		private static Set<FrameTimer> m_Timers = new Set<FrameTimer>();

		private static int m_LastPrintTime;

		public FrameTimer(string name)
		{
			FrameTimer.m_Timers.Add(this);
			this.m_Name = name;
		}

		public void Start()
		{
			this.m_Started = true;
			this.m_Stopwatch.Start();
		}

		public void Stop()
		{
			this.m_Stopwatch.Stop();
			this.m_Started = false;
		}

		public void Clear()
		{
			this.m_Stopwatch.Reset();
			this.m_Count = 0;
		}

		public void Print()
		{
			float num = (this.m_Count != 0) ? ((float)((double)this.m_Stopwatch.ElapsedTicks * 1000.0 / (double)Stopwatch.Frequency / (double)this.m_Count)) : 0f;
			Log.WriteLine(string.Concat(new object[]
			{
				"Timer ",
				this.m_Name,
				": ",
				num
			}));
		}

		public static void Update()
		{
			using (IEnumerator<FrameTimer> enumerator = FrameTimer.m_Timers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					enumerator.Current.m_Count++;
				}
			}
			int tickCount = Environment.TickCount;
			if (tickCount - FrameTimer.m_LastPrintTime > 5000)
			{
				foreach (FrameTimer expr_5B in FrameTimer.m_Timers)
				{
					expr_5B.Print();
					expr_5B.Clear();
				}
				FrameTimer.m_LastPrintTime = tickCount;
			}
		}
	}
}
