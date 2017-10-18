using EnvDTE;
using SCLCoreCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Company.VSAnything
{
	internal class GetSolutionFilesThread : IDisposable
	{
		public delegate void FinishedCallbackHandler(List<string> solution_files, List<string> projects);

		private EnvDTE.DTE m_DTE;

		private AsyncTask m_AsyncTask;

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event GetSolutionFilesThread.FinishedCallbackHandler FinishedCallback;

		public void Dispose()
		{
			if (this.m_AsyncTask != null)
			{
				this.m_AsyncTask.Dispose();
			}
		}

		public GetSolutionFilesThread(EnvDTE.DTE dte)
		{
			this.m_DTE = dte;
			this.m_AsyncTask = new AsyncTask(new AsyncTask.TaskFunction(this.Execute), "GetSolutionFiles Thread", true);
		}

		public void Start(int delay)
		{
			this.m_AsyncTask.Start(delay);
		}

		private void Execute(AsyncTask.Context context)
		{
			Log.WriteLine("GetSolutionFiles STARTED");
			List<string> solution_files = new List<string>();
			List<string> projects = new List<string>();
			try
			{
				foreach (Project project in this.m_DTE.Solution.Projects)
				{
					this.GetFilesRecursive(project, context, solution_files, projects);
					if (context.Cancelled)
					{
						break;
					}
				}
				if (context.Cancelled)
				{
					Log.WriteLine("GetSolutionFiles cancelled");
				}
			}
			catch (Exception arg_7E_0)
			{
				Utils.LogExceptionQuiet(arg_7E_0);
			}
			if (!context.Cancelled)
			{
				Log.WriteLine("GetSolutionFiles FINISHED");
				if (this.FinishedCallback != null)
				{
					this.FinishedCallback(solution_files, projects);
				}
			}
		}

		public void Exit()
		{
			this.m_AsyncTask.Exit();
		}

		private void GetFilesRecursive(Project project, AsyncTask.Context context, List<string> solution_files, List<string> projects)
		{
            ///mariotodo
            if (context.Cancelled)
            {
                return;
            }
            string project_kind = project.Kind.ToLower();
            ProjectItems project_items = project.ProjectItems;
            if (project_kind == "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}".ToLower())
            {
                if (project_items == null)
                {
                    return;
                }
                {
                    IEnumerator enumerator = project_items.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Project sub_project = ((ProjectItem)enumerator.Current).SubProject;
                        if (sub_project != null)
                        {
                            this.GetFilesRecursive(sub_project, context, solution_files, projects);
                        }
                        if (context.Cancelled)
                        {
                            break;
                        }
                    }
                    return;
                }
            }
            try
            {
                if (project_kind != "{66A2671D-8FB5-11D2-AA7E-00C04F688DDE}".ToLower())
                {
                    string project_full_name = project.FullName;
                    if (!string.IsNullOrEmpty(project_full_name))
                    {
                        projects.Add(project_full_name);
                    }
                }
            }
            catch (Exception arg_B9_0)
            {
                Utils.LogExceptionQuiet(arg_B9_0);
            }
            try
            {
                if (project_items != null)
                {
                    foreach (ProjectItem project_item in project_items)
                    {
                        this.GetProjectFilesRecursive(project_item, context, solution_files, projects);
                        if (context.Cancelled)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception arg_10E_0)
            {
                Utils.LogExceptionQuiet(arg_10E_0);
            }
		}

		private void GetProjectFilesRecursive(ProjectItem project_item, AsyncTask.Context context, List<string> solution_files, List<string> projects)
		{
			if (project_item == null || project_item.GetType().ToString().Contains("OAReferenceItem"))
			{
				return;
			}
			try
			{
				string kind = "";
				try
				{
					kind = project_item.Kind.ToLower();
				}
				catch (Exception arg_30_0)
				{
					Utils.LogExceptionQuiet(arg_30_0);
				}
				if (kind == "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}".ToLower() || kind == "{66A2671F-8FB5-11D2-AA7E-00C04F688DDE}".ToLower())
				{
					int file_count = (int)project_item.FileCount;
					short i = 1;
					while ((int)i <= file_count)
					{
						try
						{
							string filename = project_item.get_FileNames(i);
							if (!string.IsNullOrEmpty(filename) && Path.GetExtension(filename).ToLower() != ".dll")
							{
								solution_files.Add(filename);
							}
						}
						catch (Exception arg_9B_0)
						{
							Utils.LogExceptionQuiet(arg_9B_0);
						}
						if (context.Cancelled)
						{
							break;
						}
						i += 1;
					}
				}
				ProjectItems sub_project_items = null;
				try
				{
					sub_project_items = project_item.ProjectItems;
				}
				catch (Exception arg_CE_0)
				{
					Log.WriteLine("WARNING: Project item doesn't have ProjectItems: Kind: " + kind);
					Utils.LogExceptionQuiet(arg_CE_0);
				}
				if (sub_project_items != null)
				{
					foreach (ProjectItem sub_project_item in sub_project_items)
					{
						this.GetProjectFilesRecursive(sub_project_item, context, solution_files, projects);
					}
				}
			}
			catch (Exception arg_11E_0)
			{
				Utils.LogExceptionQuiet(arg_11E_0);
			}
		}
	}
}
