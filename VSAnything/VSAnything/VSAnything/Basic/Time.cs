using System;
using System.Runtime.InteropServices;

namespace SCLCoreCLR
{
	public class Time
	{
		private static long m_Freq;

		[DllImport("Kernel32.dll")]
		private static extern int QueryPerformanceCounter(ref long count);

		[DllImport("Kernel32.dll")]
		private static extern int QueryPerformanceFrequency(ref long frequency);

		public static long Now_HiRes()
		{
			long result = 0L;
			Time.QueryPerformanceCounter(ref result);
			return result;
		}

		public static long GetTicksPerSec()
		{
			if (Time.m_Freq == 0L)
			{
				Time.QueryPerformanceFrequency(ref Time.m_Freq);
			}
			return Time.m_Freq;
		}

		public static int Now()
		{
			return (int)(Time.Now_HiRes() * 1000L / Time.GetTicksPerSec());
		}
	}
}
