using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Company.VSAnything
{
	internal class GetOpenFilesThread : IDisposable
	{
		public delegate void FinishedHandler(List<string> open_files, List<UnsavedDocument> unsaved_documents);

		private EnvDTE.DTE m_DTE;

		private AsyncTask m_AsyncTask;

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event GetOpenFilesThread.FinishedHandler Finished;

		public void Dispose()
		{
			if (this.m_AsyncTask != null)
			{
				this.m_AsyncTask.Dispose();
			}
		}

		public GetOpenFilesThread(EnvDTE.DTE dte)
		{
			this.m_DTE = dte;
			this.m_AsyncTask = new AsyncTask(new AsyncTask.TaskFunction(this.Execute), "GetOpenFiles Thread", true);
		}

		public void Start(Set<string> ext_to_scan)
		{
			this.m_AsyncTask.Start(new AsyncTask.Context(ext_to_scan));
		}

		public void Exit()
		{
			this.m_AsyncTask.Exit();
		}

		private void Execute(AsyncTask.Context context)
		{
			ProfileTimer timer = new ProfileTimer("GetOpenFilesEvent");
			Set<string> ext_to_scan = (Set<string>)context.Arg;
			List<string> m_OpenFiles = new List<string>();
			List<UnsavedDocument> m_UnsavedDocuments = new List<UnsavedDocument>();
			List<Document> documents = new List<Document>();
			ProfileTimer get_documents_timer = new ProfileTimer("GetDocuments");
			try
			{
				foreach (Document document in this.m_DTE.Documents)
				{
					documents.Add(document);
				}
			}
			catch (Exception arg_83_0)
			{
				Utils.LogExceptionQuiet(arg_83_0);
			}
			get_documents_timer.Stop();
			foreach (Document document2 in documents)
			{
				try
				{
					string fullname = document2.FullName;
					string ext = Path.GetExtension(fullname).ToLower();
					if (ext_to_scan.Contains(ext))
					{
						m_OpenFiles.Add(fullname);
						if (!document2.Saved)
						{
							m_UnsavedDocuments.Add(new UnsavedDocument(document2));
						}
					}
				}
				catch (Exception arg_E6_0)
				{
					Utils.LogExceptionQuiet(arg_E6_0);
				}
				if (context.Cancelled)
				{
					break;
				}
			}
			timer.Stop();
			if (!context.Cancelled && this.Finished != null)
			{
				this.Finished(m_OpenFiles, m_UnsavedDocuments);
			}
		}
	}
}
