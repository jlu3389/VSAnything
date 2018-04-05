using SCLCoreCLR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Company.VSAnything
{
	public class SolutionFiles : IDisposable
	{
		public delegate void SolutionFileListChangedHandler(ICollection<string> solution_files, ICollection<string> projects);

		public delegate void SolutionFilenameChangedHandler(string solution_filename);

		private DTE m_DTE;

		private object m_Lock = new object();

		private Dictionary<string, Solution> m_Solutions = new Dictionary<string, Solution>();

		private string m_SolutionFilename;

		private string m_SolutionRootDir;

		private const int m_FileRetryCount = 10;

		private GetSolutionFilesThread m_GetSolutionFilesThread;

		private const int m_FileVersion = 3;

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event SolutionFiles.SolutionFileListChangedHandler SolutionFileListChanged;

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event SolutionFiles.SolutionFilenameChangedHandler SolutionFilenameChanged;

		public ICollection<string> Files
		{
			get
			{
				object @lock = this.m_Lock;
				ICollection<string> result;
				lock (@lock)
				{
					result = new List<string>(this.GetSolutionFiles());
				}
				return result;
			}
		}

		public ICollection<string> Projects
		{
			get
			{
				object @lock = this.m_Lock;
				ICollection<string> result;
				lock (@lock)
				{
					result = new List<string>(this.GetSolutionProjects());
				}
				return result;
			}
		}

		public bool GettingSolutionFiles
		{
			get
			{
				object @lock = this.m_Lock;
				bool result;
				lock (@lock)
				{
					Solution solution = this.GetCurrentSolution();
					result = (this.m_SolutionFilename != null && (solution == null || !solution.m_FullScanComplete));
				}
				return result;
			}
		}

		private static string CacheFilename
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VSAnything\\VSAnything_solution.vs_cache";
			}
		}

		public string SolutionFilename
		{
			get
			{
				object @lock = this.m_Lock;
				string solutionFilename;
				lock (@lock)
				{
					solutionFilename = this.m_SolutionFilename;
				}
				return solutionFilename;
			}
		}

		public string SolutionRootDir
		{
			get
			{
				return this.m_SolutionRootDir;
			}
		}

		internal SolutionFiles(DTE dte)
		{
			this.m_DTE = dte;
			this.Read();
			this.m_GetSolutionFilesThread = new GetSolutionFilesThread(dte.EnvDTE);
			this.m_GetSolutionFilesThread.FinishedCallback += new GetSolutionFilesThread.FinishedCallbackHandler(this.GetSolutionFilesFinished);
		}

		public void Dispose()
		{
			this.m_GetSolutionFilesThread.Dispose();
		}

		public void ForceUpdateFileList()
		{
			Solution solution = this.GetCurrentSolution();
			if (solution != null)
			{
				solution.m_FullScanComplete = false;
			}
			this.UpdateSolutionFileList(0);
		}

		public void OnSolutionClose()
		{
			this.SetSolutionFilename(null);
		}

		private void SetSolutionFilename(string solution_filename)
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				if (this.m_SolutionFilename != solution_filename)
				{
					this.m_SolutionFilename = solution_filename;
					if (solution_filename != null)
					{
						this.m_SolutionRootDir = Path.GetDirectoryName(this.m_SolutionFilename) + "\\";
					}
					else
					{
						this.m_SolutionRootDir = null;
					}
					if (this.SolutionFilenameChanged != null)
					{
						this.SolutionFilenameChanged(solution_filename);
					}
				}
			}
		}

		public void UpdateSolutionFileList(int delay)
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				this.SetSolutionFilename(this.m_DTE.GetSolutionFilename());
				Solution solution = this.GetCurrentSolution();
				if (solution != null && solution.m_FullScanComplete)
				{
					foreach (string project in this.m_DTE.GetProjects())
					{
						if (!solution.m_Projects.Contains(project))
						{
							Log.WriteLine("Clearing m_FullScanComplete because found new project: " + project);
							solution.m_FullScanComplete = false;
							using (List<string>.Enumerator enumerator2 = solution.m_Projects.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									string existing_project = enumerator2.Current;
									Log.WriteLine("Existing projects " + existing_project);
								}
								break;
							}
						}
					}
				}
				this.m_GetSolutionFilesThread.Start(delay);
			}
		}

		private void GetSolutionFilesFinished(List<string> solution_files, List<string> projects)
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				if (string.IsNullOrEmpty(this.m_SolutionFilename))
				{
					return;
				}
				Log.WriteLine("GetSolutionFilesFinished for " + this.m_SolutionFilename);
				foreach (string project in projects)
				{
					Log.WriteLine("project: " + project);
				}
				Solution solution = null;
				if (!this.m_Solutions.TryGetValue(this.m_SolutionFilename, out solution))
				{
					solution = new Solution();
					this.m_Solutions[this.m_SolutionFilename] = solution;
				}
				if (solution_files != null)
				{
					solution.m_Files = solution_files;
				}
				solution.m_Projects = projects;
				solution.m_FullScanComplete = true;
				this.Write();
			}
			if (this.SolutionFileListChanged != null)
			{
				this.SolutionFileListChanged(solution_files, projects);
			}
		}

		public void Exit()
		{
			this.m_GetSolutionFilesThread.Exit();
		}

		private Solution GetCurrentSolution()
		{
			object @lock = this.m_Lock;
			Solution result;
			lock (@lock)
			{
				Solution solution = null;
				if (this.m_SolutionFilename != null && this.m_Solutions.TryGetValue(this.m_SolutionFilename, out solution))
				{
					result = solution;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private ICollection<string> GetSolutionFiles()
		{
			object @lock = this.m_Lock;
			ICollection<string> result;
			lock (@lock)
			{
				Solution solution = this.GetCurrentSolution();
				result = ((solution != null) ? solution.m_Files : new List<string>());
			}
			return result;
		}

		private ICollection<string> GetSolutionProjects()
		{
			object @lock = this.m_Lock;
			ICollection<string> result;
			lock (@lock)
			{
				Solution solution = this.GetCurrentSolution();
				result = ((solution != null) ? solution.m_Projects : new List<string>());
			}
			return result;
		}

		private void Read()
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				try
				{
					if (File.Exists(SolutionFiles.CacheFilename))
					{
						for (int i = 0; i < 10; i++)
						{
							try
							{
								FileStream file_stream = new FileStream(SolutionFiles.CacheFilename, FileMode.Open, FileAccess.Read);
								BinaryReader reader = new BinaryReader(file_stream);
								if (reader.ReadInt32() != 3)
								{
									break;
								}
								int count = reader.ReadInt32();
								for (int j = 0; j < count; j++)
								{
									string solution_path = reader.ReadString();
									Solution solution = new Solution();
									solution.Read(reader);
									Log.WriteLine("Reading in solution: " + solution_path);
									this.m_Solutions[solution_path] = solution;
								}
								reader.Close();
								file_stream.Close();
								break;
							}
							catch (Exception arg_B0_0)
							{
								Utils.LogExceptionQuiet(arg_B0_0);
								Thread.Sleep(100);
							}
						}
					}
				}
				catch (Exception arg_CC_0)
				{
					Utils.LogExceptionQuiet(arg_CC_0);
				}
			}
		}

		private void Write()
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				bool success = false;
				string tmp_filename = SolutionFiles.CacheFilename + ".tmp";
				for (int i = 0; i < 10; i++)
				{
					try
					{
						FileStream file_stream = new FileStream(tmp_filename, FileMode.Create, FileAccess.Write);
						BinaryWriter writer = new BinaryWriter(file_stream);
						writer.Write(3);
						writer.Write(this.m_Solutions.Count);
						foreach (string solution_path in this.m_Solutions.Keys)
						{
							writer.Write(solution_path);
							this.m_Solutions[solution_path].Write(writer);
						}
						writer.Close();
						file_stream.Close();
						success = true;
						break;
					}
					catch (Exception arg_BE_0)
					{
						Utils.LogExceptionQuiet(arg_BE_0);
						Thread.Sleep(100);
					}
				}
				if (success)
				{
					for (int j = 0; j < 10; j++)
					{
						try
						{
							File.Delete(SolutionFiles.CacheFilename);
							File.Copy(tmp_filename, SolutionFiles.CacheFilename);
							break;
						}
						catch (Exception arg_FB_0)
						{
							Utils.LogExceptionQuiet(arg_FB_0);
							Thread.Sleep(100);
						}
					}
					try
					{
						File.Delete(tmp_filename);
					}
					catch (Exception arg_11E_0)
					{
						Utils.LogExceptionQuiet(arg_11E_0);
					}
				}
			}
		}
	}
}
