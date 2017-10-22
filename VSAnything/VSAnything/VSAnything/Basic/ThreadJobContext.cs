using System;
using System.Runtime.CompilerServices;

namespace SCLCoreCLR
{
	public class ThreadJobContext
	{
		public delegate void JobCancelledHandler();

		private bool m_Cancel;

		private Progress m_Progress = new Progress();

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event ThreadJobContext.JobCancelledHandler JobCancelled;

		public bool Cancel
		{
			get
			{
				return this.m_Cancel;
			}
		}

		public Progress Progress
		{
			get
			{
				return this.m_Progress;
			}
		}

		public void CancelJob()
		{
			this.m_Cancel = true;
			if (this.JobCancelled != null)
			{
				this.JobCancelled();
			}
		}
	}
}
