using System;

namespace SCLCoreCLR
{
	internal class ProgressRange
	{
		public int Progress;

		public int start;

		public int end;

		public ProgressRange(int range_start, int range_end)
		{
			this.Progress = range_start;
			this.start = range_start;
			this.end = range_end;
		}
	}
}
