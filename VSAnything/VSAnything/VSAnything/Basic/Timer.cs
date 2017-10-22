using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SCLCoreCLR
{
	public class Timer
	{
		private static Dictionary<string, Stopwatch> m_Timers = new Dictionary<string, Stopwatch>();

		public static void StartTimer(string name)
		{
			Stopwatch stopwatch = new Stopwatch();
			Timer.m_Timers[name] = stopwatch;
			stopwatch.Start();
		}

		public static void StopTimer(string name)
		{
			Stopwatch expr_0B = Timer.m_Timers[name];
			expr_0B.Stop();
			float num = (float)((double)expr_0B.ElapsedTicks * 1000.0 / (double)Stopwatch.Frequency);
			Timer.m_Timers[name] = null;
			Log.WriteLine(string.Concat(new object[]
			{
				"Timer ",
				name,
				" ",
				num
			}));
		}
	}
}
