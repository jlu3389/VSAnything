using System;
using System.Threading;

namespace Company.VSAnything
{
	internal class AsyncTask : IDisposable
	{
		public delegate void TaskFunction(AsyncTask.Context context);

		public class Context
		{
			private object m_Arg;

			private volatile bool m_Cancelled;

			public object Arg
			{
				get
				{
					return this.m_Arg;
				}
			}

			public bool Cancelled
			{
				get
				{
					return this.m_Cancelled;
				}
			}

			public Context()
			{
			}

			public Context(object arg)
			{
				this.m_Arg = arg;
			}

			public void Cancel()
			{
				this.m_Cancelled = true;
			}
		}

		private Thread m_Thread;

		private AsyncTask.TaskFunction m_TaskFunction;

		private bool m_DTETask;

		private AsyncTask.Context m_TaskContext;

		private volatile bool m_Exiting;

		private volatile int m_StartTime;

		private AutoResetEvent m_WakeEvent = new AutoResetEvent(false);

		private object m_Lock = new object();

		public AsyncTask(AsyncTask.TaskFunction task_function, string name) : this(task_function, name, false)
		{
		}

		public AsyncTask(AsyncTask.TaskFunction task_function, string name, bool dte_task)
		{
			this.m_TaskFunction = task_function;
			this.m_DTETask = dte_task;
			this.m_Thread = new Thread(new ThreadStart(this.ThreadMain));
			this.m_Thread.Name = name;
			if (dte_task)
			{
				this.m_Thread.SetApartmentState(ApartmentState.STA);
			}
			this.m_Thread.Start();
		}

		public void Dispose()
		{
			this.m_WakeEvent.Dispose();
		}

		public void Start()
		{
			this.Start(0);
		}

		public void Start(int delay)
		{
			this.Start(new AsyncTask.Context(null), delay);
		}

		public void Start(AsyncTask.Context task_context)
		{
			this.Start(task_context, 0);
		}

		public void Start(AsyncTask.Context task_context, int delay)
		{
			int start_time = Environment.TickCount + delay;
			object @lock = this.m_Lock;
			lock (@lock)
			{
				bool already_started = false;
				if (this.m_TaskContext != null)
				{
					this.m_TaskContext.Cancel();
					already_started = true;
				}
				this.m_TaskContext = task_context;
				if (!already_started || start_time > this.m_StartTime)
				{
					this.m_StartTime = start_time;
				}
				this.m_WakeEvent.Set();
			}
		}

		public void Cancel()
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				if (this.m_TaskContext != null)
				{
					this.m_TaskContext.Cancel();
					this.m_TaskContext = null;
					this.m_StartTime = 0;
				}
			}
		}

		public void Exit()
		{
			this.Cancel();
			this.m_Exiting = true;
			this.m_WakeEvent.Set();
		}

		private void ThreadMain()
		{
			while (!this.m_Exiting)
			{
				object @lock = this.m_Lock;
				int start_time;
				lock (@lock)
				{
					start_time = this.m_StartTime;
					AsyncTask.Context task_context = this.m_TaskContext;
					goto IL_82;
				}
				goto IL_32;
				IL_82:
				if ((start_time != 0 && start_time <= Environment.TickCount) || this.m_Exiting)
				{
                    // AsyncTask.Context task_context;
                    AsyncTask.Context task_context = this.m_TaskContext;//mariotodo
					if (!this.m_Exiting && task_context != null)
					{
						if (this.m_DTETask)
						{
							using (new MessageFilter())
							{
								this.m_TaskFunction(task_context);
								goto IL_D9;
							}
							goto IL_CD;
						}
						goto IL_CD;
						IL_D9:
						@lock = this.m_Lock;
						lock (@lock)
						{
							if (!task_context.Cancelled)
							{
								this.m_TaskContext = null;
								this.m_StartTime = 0;
							}
						}
						continue;
						IL_CD:
						this.m_TaskFunction(task_context);
						goto IL_D9;
					}
					continue;
				}
				IL_32:
				int sleep_time = (start_time != 0) ? Math.Max(0, start_time - Environment.TickCount) : -1;
				this.m_WakeEvent.WaitOne(sleep_time);
				@lock = this.m_Lock;
				lock (@lock)
				{
					start_time = this.m_StartTime;
					AsyncTask.Context task_context = this.m_TaskContext;
				}
				goto IL_82;
			}
		}
	}
}
