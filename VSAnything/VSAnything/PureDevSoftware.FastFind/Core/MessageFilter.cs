using System;
using System.Runtime.InteropServices;

namespace Company.VSAnything
{
	internal class MessageFilter : MarshalByRefObject, IDisposable, IMessageFilter
	{
		private IMessageFilter oldFilter;

		private const int SERVERCALL_ISHANDLED = 0;

		private const int PENDINGMSG_WAITNOPROCESS = 2;

		private const int SERVERCALL_RETRYLATER = 2;

		[DllImport("ole32.dll")]
		private static extern int CoRegisterMessageFilter(IMessageFilter lpMessageFilter, out IMessageFilter lplpMessageFilter);

		public MessageFilter()
		{
			MessageFilter.CoRegisterMessageFilter(this, out this.oldFilter);
		}

		public void Dispose()
		{
			IMessageFilter dummy;
			MessageFilter.CoRegisterMessageFilter(this.oldFilter, out dummy);
			GC.SuppressFinalize(this);
		}

		int IMessageFilter.HandleInComingCall(int dwCallType, IntPtr threadIdCaller, int dwTickCount, IntPtr lpInterfaceInfo)
		{
			return 0;
		}

		int IMessageFilter.RetryRejectedCall(IntPtr threadIDCallee, int dwTickCount, int dwRejectType)
		{
			if (dwRejectType == 2)
			{
				return 150;
			}
			return -1;
		}

		int IMessageFilter.MessagePending(IntPtr threadIDCallee, int dwTickCount, int dwPendingType)
		{
			return 2;
		}
	}
}
