using SCLCoreCLR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Company.VSAnything
{
	internal class FileFinder : IDisposable
	{
		private class Job
		{
			public string m_TextBoxValue = "";

			public string m_RootPath;

			public bool m_MatchCase;

			public bool m_UseWildcards;

			public bool m_IsEnteringPath;

			public bool m_IsModal;

			public FindFilesFinishedHandler m_FinishedHandler;
		}

		private class SolutionFiles
		{
			public List<string> m_SolutionFilePaths = new List<string>();

			public List<string> m_SolutionFileNames = new List<string>();

			public List<string> m_SolutionFilePathsLowerCase = new List<string>();

			public List<string> m_SolutionFileNamesLowercase = new List<string>();
		}

		private Settings m_Settings;

		private AsyncTask m_AsyncTask;

		private object m_Lock = new object();

		private FileFinder.SolutionFiles m_SolutionFiles = new FileFinder.SolutionFiles();

		public FileFinder(Settings settings)
		{
			this.m_Settings = settings;
			this.m_AsyncTask = new AsyncTask(new AsyncTask.TaskFunction(this.ExecuteTask), "FileFinger Thread");
		}

		public void Dispose()
		{
			if (this.m_AsyncTask != null)
			{
				this.m_AsyncTask.Dispose();
			}
		}

		public void Exit()
		{
			this.m_AsyncTask.Exit();
		}

		public void SetSolutionFiles(List<string> solution_file_paths)
		{
			FileFinder.SolutionFiles solution_files = new FileFinder.SolutionFiles();
			solution_files.m_SolutionFilePaths = new List<string>(solution_file_paths);
			foreach (string file in solution_files.m_SolutionFilePaths)
			{
				solution_files.m_SolutionFileNames.Add(Path.GetFileName(file));
			}
			solution_files.m_SolutionFilePathsLowerCase = Utils.ToLower(solution_files.m_SolutionFilePaths);
			solution_files.m_SolutionFileNamesLowercase = Utils.ToLower(solution_files.m_SolutionFileNames);
			object @lock = this.m_Lock;
			lock (@lock)
			{
				this.m_SolutionFiles = solution_files;
			}
		}

		public void Find(string text_box_value, string root_path, bool match_case, bool use_wildcards, bool is_entering_path, bool is_modal, FindFilesFinishedHandler finished_handler)
		{
			Log.WriteLine("FildFinder.Find Started");
			FileFinder.Job job = new FileFinder.Job();
			job.m_TextBoxValue = text_box_value;
			job.m_RootPath = root_path;
			job.m_MatchCase = match_case;
			job.m_UseWildcards = use_wildcards;
			job.m_IsEnteringPath = is_entering_path;
			job.m_IsModal = is_modal;
			job.m_FinishedHandler = finished_handler;
			this.m_AsyncTask.Start(new AsyncTask.Context(job));
		}

		private void ExecuteTask(AsyncTask.Context context)
		{
			Log.WriteLine("FildFinder.ExecuteTask started");
			FileFinder.Job job = (FileFinder.Job)context.Arg;
			List<FindFileResult> found_files;
			if (job.m_TextBoxValue == "")
			{
				found_files = new List<FindFileResult>();
			}
			else if (job.m_IsEnteringPath)
			{
				found_files = this.FindFilesInPath(job, context);
			}
			else
			{
				found_files = this.FindFilesInSolution(job, context);
			}
			Log.WriteLine("FildFinder.ExecuteTask finished");
			job.m_FinishedHandler(found_files, job.m_TextBoxValue);
		}

		private List<FindFileResult> FindFilesInPath(FileFinder.Job job, AsyncTask.Context context)
		{
			List<FindFileResult> found_files = new List<FindFileResult>();
			string path = job.m_TextBoxValue;
			string dir;
			try
			{
				dir = Path.GetDirectoryName(path);
			}
			catch (Exception arg_16_0)
			{
				Log.WriteLine(arg_16_0.Message);
				dir = path;
			}
			if (!Directory.Exists(dir))
			{
				dir = Path.GetDirectoryName(dir);
			}
			if (!Directory.Exists(dir))
			{
				return found_files;
			}
			List<string> items = new List<string>();
			if (!context.Cancelled)
			{
				string[] array = Directory.GetDirectories(dir);
				for (int i = 0; i < array.Length; i++)
				{
					string child_dir = array[i];
					items.Add(Path.GetFileName(child_dir));
					if (context.Cancelled)
					{
						break;
					}
				}
			}
			if (!context.Cancelled)
			{
				string[] array = Directory.GetFiles(dir);
				for (int i = 0; i < array.Length; i++)
				{
					string file = array[i];
					items.Add(Path.GetFileName(file));
					if (context.Cancelled)
					{
						break;
					}
				}
			}
			string pattern = Path.GetFileName(path).Trim().ToLower();
			if (!string.IsNullOrEmpty(pattern))
			{
				bool use_wildcard = job.m_UseWildcards && pattern.Contains('*');
				List<string> filtered_items = new List<string>();
				foreach (string item in items)
				{
					bool match;
					if (use_wildcard)
					{
						match = Wildcard.Match(item.ToLower(), pattern);
					}
					else
					{
						match = Utils.StrStr(item.ToLower(), pattern);
					}
					if (match)
					{
						filtered_items.Add(item);
					}
					if (context.Cancelled)
					{
						break;
					}
				}
				items = filtered_items;
			}
			if (!context.Cancelled)
			{
				foreach (string item2 in items)
				{
					found_files.Add(new FindFileResult
					{
						m_FileName = item2,
						m_PathMatch = true
					});
				}
			}
			return found_files;
		}

		private List<FindFileResult> FindFilesInSolution(FileFinder.Job job, AsyncTask.Context context)
		{
			FileFinder.SolutionFiles solution_files = null;
			object @lock = this.m_Lock;
			lock (@lock)
			{
				solution_files = this.m_SolutionFiles;
			}
			string pattern = job.m_TextBoxValue;
			if (!this.m_Settings.GetSolutionFilesMatchCase(job.m_IsModal))
			{
				pattern = pattern.ToLower();
			}
			SettingsDialogPage arg_5D_0 = VSAnythingPackage.Inst.GetSettingsDialogPage();
			bool match_case = job.m_MatchCase;
			if (arg_5D_0.SpacesAsWildcardsForFindFile && pattern.Contains(' '))
			{
				pattern = "*" + pattern.Replace(' ', '*') + "*";
			}
			bool use_wildacrd = job.m_UseWildcards && pattern.Contains('*');
			if (use_wildacrd)
			{
				if (!pattern.StartsWith("*"))
				{
					pattern = "*" + pattern;
				}
				if (!pattern.EndsWith("*"))
				{
					pattern += "*";
				}
			}
			bool use_full_paths = pattern.Contains("\\");
			IEnumerable<string> arg_121_0 = this.m_Settings.GetSolutionFilesMatchCase(job.m_IsModal) ? (use_full_paths ? solution_files.m_SolutionFilePaths : solution_files.m_SolutionFileNames) : (use_full_paths ? solution_files.m_SolutionFilePathsLowerCase : solution_files.m_SolutionFileNamesLowercase);
			List<FindFileResult> found_files = new List<FindFileResult>();
			int i = 0;
			foreach (string file in arg_121_0)
			{
				bool match;
				if (use_wildacrd)
				{
					match = Wildcard.Match(file, pattern);
				}
				else if (match_case)
				{
					match = file.Contains(pattern);
				}
				else
				{
					match = Utils.StrStr(file, pattern);
				}
				if (match)
				{
					string file_path = solution_files.m_SolutionFilePaths[i];
					string file_name = solution_files.m_SolutionFileNames[i];
					FindFileResult found_file = default(FindFileResult);
					found_file.m_FilePath = file_path;
					found_file.m_FileName = file_name;
					if (Path.IsPathRooted(job.m_RootPath) && Path.IsPathRooted(file_path) && Path.GetPathRoot(job.m_RootPath) == Path.GetPathRoot(file_path) && !Utils.IsSolutionFile(file_path))
					{
						found_file.m_RelativeFilePath = Misc.GetRelativePath(job.m_RootPath, file_path);
					}
					else
					{
						found_file.m_RelativeFilePath = file_path;
					}
					found_file.m_PathMatch = false;
					found_files.Add(found_file);
				}
				i++;
				if (context.Cancelled)
				{
					break;
				}
			}
			return found_files;
		}
	}
}
