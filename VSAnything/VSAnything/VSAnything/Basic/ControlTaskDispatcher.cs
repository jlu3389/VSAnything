using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace SCLCoreCLR
{
	public class ControlTaskDispatcher
	{
		public delegate void TaskFunction();

		public delegate void TaskFunction1Arg(object arg);

		public delegate void TaskFunction2Arg(object arg1, object arg2);

		private class Task
		{
			private object m_Function;

			private List<object> m_Args = new List<object>();

			public Task(ControlTaskDispatcher.TaskFunction function)
			{
				this.m_Function = function;
			}

			public Task(ControlTaskDispatcher.TaskFunction1Arg function, object arg)
			{
				this.m_Function = function;
				this.m_Args.Add(arg);
			}

			public Task(ControlTaskDispatcher.TaskFunction2Arg function, object arg1, object arg2)
			{
				this.m_Function = function;
				this.m_Args.Add(arg1);
				this.m_Args.Add(arg2);
			}

			public void Do()
			{
				switch (this.m_Args.Count)
				{
				case 0:
					((ControlTaskDispatcher.TaskFunction)this.m_Function)();
					return;
				case 1:
					((ControlTaskDispatcher.TaskFunction1Arg)this.m_Function)(this.m_Args[0]);
					return;
				case 2:
					((ControlTaskDispatcher.TaskFunction2Arg)this.m_Function)(this.m_Args[0], this.m_Args[1]);
					return;
				default:
					return;
				}
			}
		}

		private enum WM
		{
			WM_USER = 1024
		}

		private enum CustomMessages
		{
			WM_TASK_MANAGER_WAKE_UP = 1126
		}

		private Queue<ControlTaskDispatcher.Task> m_Tasks = new Queue<ControlTaskDispatcher.Task>();

		private IntPtr m_hWnd;

		private bool m_ProcessingTask;

		[DllImport("user32.dll")]
		private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

		public void QueueTask(ControlTaskDispatcher.TaskFunction function)
		{
			this.QueueTask(new ControlTaskDispatcher.Task(function));
		}

		public void QueueTask(ControlTaskDispatcher.TaskFunction1Arg function, object arg)
		{
			this.QueueTask(new ControlTaskDispatcher.Task(function, arg));
		}

		public void QueueTask(ControlTaskDispatcher.TaskFunction2Arg function, object arg1, object arg2)
		{
			this.QueueTask(new ControlTaskDispatcher.Task(function, arg1, arg2));
		}

		private void QueueTask(ControlTaskDispatcher.Task task)
		{
			Queue<ControlTaskDispatcher.Task> tasks = this.m_Tasks;
			lock (tasks)
			{
				this.m_Tasks.Enqueue(task);
				this.Wakeup();
			}
		}

		private void Wakeup()
		{
			if (this.m_hWnd != IntPtr.Zero)
			{
				ControlTaskDispatcher.PostMessage(this.m_hWnd, 1126u, 0, 0);
			}
		}

		public void ProcessMessage(IntPtr hWnd, ref Message m)
		{
			bool flag = this.m_hWnd == IntPtr.Zero;
			this.m_hWnd = hWnd;
			if (this.m_ProcessingTask)
			{
				return;
			}
			this.m_ProcessingTask = true;
			if (m.Msg == 1126 | flag)
			{
				Monitor.Enter(this.m_Tasks);
				while (this.m_Tasks.Count != 0)
				{
					ControlTaskDispatcher.Task arg_5C_0 = this.m_Tasks.Dequeue();
					Monitor.Exit(this.m_Tasks);
					arg_5C_0.Do();
					Monitor.Enter(this.m_Tasks);
				}
				Monitor.Exit(this.m_Tasks);
			}
			this.m_ProcessingTask = false;
		}
	}
}
