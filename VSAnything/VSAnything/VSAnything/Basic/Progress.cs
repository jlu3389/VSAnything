using System;
using System.Collections.Generic;

namespace SCLCoreCLR
{
	public class Progress
	{
		private List<ProgressRange> m_Ranges = new List<ProgressRange>();

		private bool m_Started;

		private bool m_Finished;

		public int PercentComplete
		{
			get
			{
				return Misc.Clamp(this.m_Ranges[this.m_Ranges.Count - 1].Progress, 0, 100);
			}
			set
			{
				this.Set((long)value, 100L);
			}
		}

		public bool Finished
		{
			get
			{
				return this.m_Finished;
			}
		}

		public Progress()
		{
			this.Reset();
		}

		public void Start()
		{
			this.Reset();
			this.m_Started = true;
		}

		private void Check()
		{
			int arg_1D_0 = this.m_Ranges[this.m_Ranges.Count - 1].Progress;
		}

		public void Set(long value, long max = 100L)
		{
			value = (long)((int)(value * 100L / max));
			ProgressRange progressRange = this.m_Ranges[this.m_Ranges.Count - 1];
			int num = progressRange.end - progressRange.start;
			progressRange.Progress = progressRange.start + (int)(value * (long)num / 100L);
			this.Check();
		}

		private void Reset()
		{
			this.m_Finished = false;
			this.m_Ranges.Clear();
			this.m_Started = true;
			this.Push(100);
			this.m_Started = false;
		}

		public void Push()
		{
			ProgressRange progressRange = this.m_Ranges[this.m_Ranges.Count - 1];
			this.Push(progressRange.end - progressRange.Progress);
		}

		public void Push(int length)
		{
			int expr_16 = (this.m_Ranges.Count != 0) ? this.PercentComplete : 0;
			ProgressRange item = new ProgressRange(expr_16, expr_16 + length);
			this.m_Ranges.Add(item);
			this.Check();
		}

		public void Pop()
		{
			int index = this.m_Ranges.Count - 1;
			ProgressRange progressRange = this.m_Ranges[index];
			this.m_Ranges.RemoveAt(index);
			this.m_Ranges[this.m_Ranges.Count - 1].Progress = progressRange.end;
			this.Check();
		}

		public void Finish()
		{
			this.m_Finished = true;
		}
	}
}
