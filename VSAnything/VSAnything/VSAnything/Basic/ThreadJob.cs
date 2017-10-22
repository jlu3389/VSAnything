using System;
using System.Threading;

namespace SCLCoreCLR
{
	public class ThreadJob
	{
		private Thread m_Thread;

		private ThreadJobMain m_ThreadJobMain;

		private object m_Arg;

		private object m_Result;

		private volatile bool m_Finished;

		private ThreadJobContext m_Context = new ThreadJobContext();

		public bool Finished
		{
			get
			{
				return this.m_Finished;
			}
		}

		public object Result
		{
			get
			{
				return this.m_Result;
			}
		}

		public int PercentComplete
		{
			get
			{
				return this.m_Context.Progress.PercentComplete;
			}
		}

		public ThreadJob(ThreadJobMain thread_job_main) : this(thread_job_main, null)
		{
		}

		public ThreadJob(ThreadJobMain thread_job_main, object arg)
		{
			this.m_ThreadJobMain = thread_job_main;
			this.m_Arg = arg;
		}

		public void Run()
		{
			this.m_Thread = new Thread(new ThreadStart(this.ThreadMain));
			this.m_Thread.Start();
		}

		public void Abort()
		{
			if (this.m_Thread.IsAlive)
			{
				this.m_Thread.Abort();
			}
		}

		private void ThreadMain()
		{
			this.m_Context.Progress.Start();
			this.m_Result = this.m_ThreadJobMain(this.m_Arg, this.m_Context);
			this.m_Context.Progress.Finish();
			this.m_Finished = true;
		}

		public void Cancel()
		{
			this.m_Context.CancelJob();
		}
	}
}
