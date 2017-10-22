using SCLCoreCLR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace Company.VSAnything
{
	internal class TextFinder : IDisposable
	{
		private class TextFinderJob
		{
			public FindTextRequest m_Request;

			public Dictionary<string, UnsavedDocument> m_UnsavedDocuments = new Dictionary<string, UnsavedDocument>();

			public List<string> m_Files = new List<string>();

			public PathMode m_PathMode;

			public string m_RootPath;

			public List<string> m_ExtOverride;

			public bool m_MatchWholeWord;

			public bool m_RegExpression;

			public List<List<TextFinderResult>> m_MatchingWordsLists = new List<List<TextFinderResult>>();
		}

		private class FindThreadMainArg
		{
			public int m_ThreadIndex;

			public List<string> m_Files;

			public TextFinder.TextFinderJob m_Job;

			public List<TextFinderResult> m_MatchingWords;

			public AsyncTask.Context m_Context;
		}

		private List<string> m_FilesToScan = new List<string>();

		private object m_FilestoScanLock = new object();

		private volatile bool m_ScanInProgress;

		private Dictionary<string, CachedFile> m_FileCache = new Dictionary<string, CachedFile>();

		private volatile int m_ScanPercentComplete;

		private volatile bool m_FindInProgress;

		private int m_TotalResultCount;

		private int m_ThreadCount = 1;  // 方便调试  Math.Max(1, Environment.ProcessorCount - 1);

		private AutoResetEvent m_FindthreadFinished = new AutoResetEvent(false);

		private List<AsyncTask> m_FindTextThreads = new List<AsyncTask>();

		private int m_ActiveThreadCount;

		private volatile int[] m_ThreadFilesProcessedCount;

		private object m_ThreadFilesProcessedCountLock = new object();

		private const int m_FileRetryCount = 10;

		private const int m_FileVersion = 1;

		private string m_SolutionPath;

		private AsyncTask m_ReadTask;

		private AsyncTask m_ScanTask;

		private AsyncTask m_FindTask;

		private AsyncTask m_FileChangedHandlerTask;

		private DirectoryWatcher m_DirectoryWatcher = new DirectoryWatcher();

		private Set<string> m_WatchedDirectories = new Set<string>();

		private Set<string> m_ModifiedFiles = new Set<string>();

		private const int m_FileChangedDelay = 100;

		private bool[] m_WordCharMap = new bool[65535];

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event ScanFinishedHandler ScanFinished;

		public int ScanPercentComplete
		{
			get
			{
				return this.m_ScanPercentComplete;
			}
		}

		public int PercentComplete
		{
			get
			{
				int count = 0;
				object threadFilesProcessedCountLock = this.m_ThreadFilesProcessedCountLock;
				lock (threadFilesProcessedCountLock)
				{
					if (this.m_ThreadFilesProcessedCount != null)
					{
						for (int i = 0; i < this.m_ThreadCount; i++)
						{
							count += this.m_ThreadFilesProcessedCount[i];
						}
					}
				}
				if (this.m_TotalResultCount == 0)
				{
					return 0;
				}
				return count * 100 / this.m_TotalResultCount;
			}
		}

		public bool ScanInProgress
		{
			get
			{
				return this.m_ScanInProgress;
			}
		}

		public bool FindInProgress
		{
			get
			{
				return this.m_FindInProgress;
			}
		}

		public TextFinder()
		{
			this.SetupWordCharMap();
			this.m_FileChangedHandlerTask = new AsyncTask(new AsyncTask.TaskFunction(this.HandleFileChangedEventsTask), "File Changed Thread");
			this.m_DirectoryWatcher.FileChanged += new FileChangedHandler(this.DirectoryWatcherFileChanged);
			this.m_ReadTask = new AsyncTask(new AsyncTask.TaskFunction(this.Read), "TextFinder Read Thread");
			this.m_ScanTask = new AsyncTask(new AsyncTask.TaskFunction(this.ScanTask), "TextFinder Scan Thread");
			this.m_FindTask = new AsyncTask(new AsyncTask.TaskFunction(this.FindTask), "TextFinder Find Thread");
			for (int i = 0; i < this.m_ThreadCount; i++)
			{
				this.m_FindTextThreads.Add(new AsyncTask(new AsyncTask.TaskFunction(this.FindWorkerThreadMain), "FindTextThread" + i));
			}
		}

		public void Dispose()
		{
			if (this.m_FileChangedHandlerTask != null)
			{
				this.m_FileChangedHandlerTask.Dispose();
			}
			if (this.m_ReadTask != null)
			{
				this.m_ReadTask.Dispose();
			}
			if (this.m_ScanTask != null)
			{
				this.m_ScanTask.Dispose();
			}
			if (this.m_FindTask != null)
			{
				this.m_FindTask.Dispose();
			}
			using (List<AsyncTask>.Enumerator enumerator = this.m_FindTextThreads.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					enumerator.Current.Dispose();
				}
			}
			if (this.m_FindthreadFinished != null)
			{
				this.m_FindthreadFinished.Dispose();
				this.m_FindthreadFinished = null;
			}
		}

		private void SetupWordCharMap()
		{
			for (int i = 97; i <= 122; i++)
			{
				this.m_WordCharMap[i] = true;
			}
			for (int j = 65; j <= 90; j++)
			{
				this.m_WordCharMap[j] = true;
			}
			for (int k = 48; k <= 57; k++)
			{
				this.m_WordCharMap[k] = true;
			}
			this.m_WordCharMap[95] = true;
		}

		public void SetSolutionPath(string solution_path)
		{
			if (this.m_SolutionPath != solution_path)
			{
				this.Write();
				this.m_SolutionPath = solution_path;
				if (solution_path != null)
				{
					this.m_ReadTask.Start(new AsyncTask.Context(solution_path));
				}
			}
		}

		public void SetSolutionFiles(ICollection<string> projects, ICollection<string> solution_files)
		{
			try
			{
				this.UpdateWatchedDirectories(projects, solution_files);
				this.UpdateFilestoScanList(solution_files);
			}
			catch (Exception arg_11_0)
			{
				Utils.LogException(arg_11_0);
			}
			this.StartFileScan();
		}

		private void UpdateWatchedDirectories(ICollection<string> projects, ICollection<string> solution_files)
		{
			try
			{
				Set<string> new_dirs = new Set<string>();
				foreach (string project in projects)
				{
					new_dirs.Add(Utils.NormalisePath(Path.GetDirectoryName(project)));
				}
				this.FindExtraDirsToWatch(solution_files, new_dirs);
				foreach (string new_dir in new_dirs)
				{
					if (!this.m_WatchedDirectories.Contains(new_dir))
					{
						if (Directory.Exists(new_dir))
						{
							this.m_DirectoryWatcher.StartWatching(new_dir);
						}
						this.m_WatchedDirectories.Add(new_dir);
					}
				}
				foreach (string old_dir in new Set<string>(this.m_WatchedDirectories))
				{
					if (!new_dirs.Contains(old_dir))
					{
						this.m_DirectoryWatcher.StopWatching(old_dir);
						this.m_WatchedDirectories.Remove(old_dir);
					}
				}
			}
			catch (Exception arg_EA_0)
			{
				Utils.LogExceptionQuiet(arg_EA_0);
			}
		}

		private void UpdateFilestoScanList(ICollection<string> solution_files)
		{
            /// 只留下指定拓展名的文件列表
			List<string> files_to_scan = new List<string>();
			ICollection<string> ext_to_scan = VSAnythingPackage.Inst.GetSettingsDialogPage().ExtList;
			foreach (string file in solution_files)
			{
				string ext = Path.GetExtension(file).ToLower();
				if (ext_to_scan.Contains(ext))
				{
					files_to_scan.Add(file);
				}
			}
			object filestoScanLock = this.m_FilestoScanLock;
			lock (filestoScanLock)
			{
				this.m_FilesToScan = files_to_scan;
			}
		}

		private void FindExtraDirsToWatch(ICollection<string> solution_files, Set<string> dirs)
		{
			foreach (string arg_11_0 in solution_files)
			{
				bool already_watching = false;
				string norm_filename = Utils.NormalisePath(arg_11_0);
				foreach (string dir in dirs)
				{
					if (norm_filename.StartsWith(dir))
					{
						already_watching = true;
						break;
					}
				}
				if (!already_watching)
				{
					dirs.Add(Path.GetDirectoryName(norm_filename));
				}
			}
		}

		public void StartFileScan()
		{
			this.m_ScanTask.Start();
		}

		private void CancelScan()
		{
			Log.WriteLine("cancelling scan");
			this.m_ScanTask.Cancel();
		}

		private void ScanTask(AsyncTask.Context context)
		{
            /// 查找文件，如果跟缓存不一致，则重新读取
			this.m_ScanInProgress = true;
			int maximum_file_size = VSAnythingPackage.Inst.GetSettingsDialogPage().MaximumFileSize;
			Log.WriteLine("Scan in progress");
			try
			{
				this.m_ScanPercentComplete = 0;
				object filestoScanLock = this.m_FilestoScanLock;
				List<string> files_to_scan;
				lock (filestoScanLock)
				{
					files_to_scan = new List<string>(this.m_FilesToScan);
				}
				int i = 0;
				foreach (string file_path in files_to_scan)
				{
					this.ScanFile(file_path, maximum_file_size);
					i++;
					this.m_ScanPercentComplete = i * 100 / files_to_scan.Count;
					if (context.Cancelled)
					{
						Log.WriteLine("Scan cancelled");
						break;
					}
				}
				this.m_ScanInProgress = false;
				if (!context.Cancelled)
				{
					Dictionary<string, CachedFile> fileCache = this.m_FileCache;
					lock (fileCache)
					{
						Set<string> files_to_scan_set = new Set<string>();
						foreach (string file in files_to_scan)
						{
							files_to_scan_set.Add(file.ToLower());
						}
						List<string> files_to_remove = new List<string>();
						foreach (string file2 in this.m_FileCache.Keys)
						{
							if (!files_to_scan_set.Contains(file2))
							{
								files_to_remove.Add(file2);
							}
						}
						foreach (string file3 in files_to_remove)
						{
							this.m_FileCache.Remove(file3);
						}
					}
					if (this.ScanFinished != null)
					{
						this.ScanFinished();
					}
				}
			}
			catch (Exception arg_1D1_0)
			{
				Utils.LogException(arg_1D1_0);
			}
		}

		public void MarkFileAsChanged(string path)
		{
			Utils.CheckNormalised(path);
			Dictionary<string, CachedFile> fileCache = this.m_FileCache;
			lock (fileCache)
			{
				CachedFile cached_file;
				if (this.m_FileCache.TryGetValue(path, out cached_file))
				{
					cached_file.m_ModifiedTime = DateTime.MinValue;
				}
			}
		}

		public void ScanFile(string file_path)
		{
			this.ScanFile(file_path, VSAnythingPackage.Inst.GetSettingsDialogPage().MaximumFileSize);
		}

		private void ScanFile(string file_path, int maximum_file_size)
		{
			string path = Utils.NormalisePath(file_path);
			Dictionary<string, CachedFile> fileCache = this.m_FileCache;
			CachedFile cached_file;
			lock (fileCache)
			{
				if (!this.m_FileCache.TryGetValue(path, out cached_file))
				{
					cached_file = new CachedFile();
					cached_file.m_Filename = file_path;
					cached_file.m_ModifiedTime = DateTime.MinValue;
					this.m_FileCache[path] = cached_file;
				}
			}
			try
			{
				DateTime last_write_time = File.GetLastWriteTime(path);
				if (cached_file.m_ModifiedTime != last_write_time)
				{
					bool deleted = false;
					CachedFile obj = cached_file;
					lock (obj)
					{
						if (File.Exists(path))
						{
							cached_file.m_ModifiedTime = last_write_time;
							cached_file.m_Lines.Clear();
							cached_file.m_LinesLowercase.Clear();
							if (new FileInfo(path).Length <= (long)maximum_file_size)
							{
								bool succeeded = false;
								int i = 0;
								while (!succeeded)
								{
									if (i++ >= 5)
									{
										break;
									}
									try
									{
										StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
										for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
										{
											line = line.TrimEnd(Array.Empty<char>());
											cached_file.m_Lines.Add(Utils.Intern(line));
											cached_file.m_LinesLowercase.Add(Utils.Intern(line.ToLower()));
										}
										reader.Close();
										succeeded = true;
									}
									catch (Exception arg_134_0)
									{
										Utils.LogExceptionQuiet(arg_134_0);
									}
								}
							}
							else
							{
								Log.WriteLine(string.Concat(new object[]
								{
									"Warning: ignoring large file ",
									path,
									" because it is larger than the maximum file size set in the settings (",
									maximum_file_size,
									")"
								}));
							}
						}
						else
						{
							deleted = true;
						}
					}
					if (deleted)
					{
						fileCache = this.m_FileCache;
						lock (fileCache)
						{
							this.m_FileCache.Remove(path);
						}
					}
				}
			}
			catch (Exception e)
			{
				Log.WriteLine("Warning: ScanFile failed on " + file_path + " : " + e.Message);
			}
		}

		public void Exit()
		{
			this.m_DirectoryWatcher.FileChanged -= new FileChangedHandler(this.DirectoryWatcherFileChanged);
			this.SetSolutionPath(null);
			this.m_FileChangedHandlerTask.Exit();
			this.m_ReadTask.Exit();
			this.m_ScanTask.Exit();
			for (int i = 0; i < this.m_ThreadCount; i++)
			{
				this.m_FindTextThreads[i].Exit();
			}
		}

		public void Find(FindTextRequest request, List<string> files_to_search, List<UnsavedDocument> unsaved_documents, PathMode path_mode, string solution_path, List<string> ext_override, bool match_whole_word, bool reg_expression)
		{
			TextFinder.TextFinderJob job = new TextFinder.TextFinderJob();
			job.m_Request = request;
			job.m_Files = files_to_search;
			job.m_PathMode = path_mode;
			job.m_RootPath = solution_path.ToLower();
			job.m_ExtOverride = ext_override;
			job.m_MatchWholeWord = match_whole_word;
			job.m_RegExpression = reg_expression;
			foreach (UnsavedDocument unsaved_document in unsaved_documents)
			{
				string filename = Utils.NormalisePath(unsaved_document.Filename);
				job.m_UnsavedDocuments[filename] = unsaved_document;
			}
			this.m_FindTask.Start(new AsyncTask.Context(job));
		}

		private void FindTask(AsyncTask.Context context)
		{
			TextFinder.TextFinderJob job = (TextFinder.TextFinderJob)context.Arg;
			this.m_FindInProgress = true;
			List<string> files = job.m_Files;
			if (job.m_ExtOverride != null)
			{
				List<string> filtered_files = new List<string>();
				foreach (string file in files)
				{
					string ext = Path.GetExtension(file);
					if (job.m_ExtOverride.Contains(ext))
					{
						filtered_files.Add(file);
					}
				}
				files = filtered_files;
			}
			this.m_ActiveThreadCount = this.m_ThreadCount;
			this.m_TotalResultCount = files.Count;
			int key_count_per_thread = files.Count / this.m_ActiveThreadCount;
			int file_index = 0;
			object threadFilesProcessedCountLock = this.m_ThreadFilesProcessedCountLock;
			lock (threadFilesProcessedCountLock)
			{
				this.m_ThreadFilesProcessedCount = new int[this.m_ThreadCount];
			}
			for (int i = 0; i < this.m_ThreadCount; i++)
			{
				List<TextFinderResult> matching_words = new List<TextFinderResult>();
				job.m_MatchingWordsLists.Add(matching_words);
				List<string> thread_files = new List<string>(key_count_per_thread);
				int thread_file_count = (i < this.m_ThreadCount - 1) ? key_count_per_thread : (files.Count - file_index);
				int end_inddex = file_index + thread_file_count;
				for (int a = file_index; a < end_inddex; a++)
				{
					thread_files.Add(files[a]);
				}
				TextFinder.FindThreadMainArg arg = new TextFinder.FindThreadMainArg();
				arg.m_ThreadIndex = i;
				arg.m_Files = thread_files;
				arg.m_Job = job;
				arg.m_MatchingWords = matching_words;
				arg.m_Context = context;
				this.m_FindTextThreads[i].Start(new AsyncTask.Context(arg));
				file_index += thread_file_count;
			}
			while (this.m_ActiveThreadCount != 0)
			{
				this.m_FindthreadFinished.WaitOne();
			}
			if (!context.Cancelled)
			{
				foreach (List<TextFinderResult> matching_words2 in job.m_MatchingWordsLists)
				{
					job.m_Request.m_MatchingWords.AddRange(matching_words2);
				}
				int max_result_count = job.m_Request.m_MaxResultCount;
				if (job.m_Request.m_MatchingWords.Count > max_result_count)
				{
					job.m_Request.m_MatchingWords.RemoveRange(max_result_count, job.m_Request.m_MatchingWords.Count - max_result_count);
				}
			}
			this.m_FindInProgress = false;
			if (!context.Cancelled)
			{
				job.m_Request.FindFinished(job.m_Request);
			}
		}

		private void FindWorkerThreadMain(AsyncTask.Context context)
		{
			TextFinder.FindThreadMainArg expr_0B = (TextFinder.FindThreadMainArg)context.Arg;
			TextFinder.TextFinderJob job = expr_0B.m_Job;
			int matching_word_index = expr_0B.m_ThreadIndex * job.m_Request.m_MaxResultCount;
			AsyncTask.Context parent_context = expr_0B.m_Context;
			List<TextFinderResult> matching_words = expr_0B.m_MatchingWords;
			int thread_index = expr_0B.m_ThreadIndex;
			List<string> files = expr_0B.m_Files;
			bool match_whole_word = job.m_MatchWholeWord;
			bool reg_expression = job.m_RegExpression;
			try
			{
				if (reg_expression)
				{
					this.SearchFiles_RegExp(job, files, parent_context, match_whole_word, matching_words, ref matching_word_index, thread_index);
				}
				else
				{
					bool patterns_have_wildcards = false;
					Pattern[] patterns = job.m_Request.m_Patterns;
					for (int i = 0; i < patterns.Length; i++)
					{
						if (patterns[i].m_UseWildcard)
						{
							patterns_have_wildcards = true;
							break;
						}
					}
					if (patterns_have_wildcards)
					{
						this.SearchFiles_Wildcards(job, files, parent_context, match_whole_word, matching_words, ref matching_word_index, thread_index);
					}
					else
					{
						this.SearchFiles(job, files, parent_context, match_whole_word, matching_words, ref matching_word_index, thread_index);
					}
				}
			}
			catch (Exception arg_CB_0)
			{
				Utils.LogException(arg_CB_0);
			}
			Interlocked.Decrement(ref this.m_ActiveThreadCount);
			this.m_FindthreadFinished.Set();
		}

		private void SearchFiles(TextFinder.TextFinderJob job, List<string> files, AsyncTask.Context parent_context, bool match_whole_word, List<TextFinderResult> matching_words, ref int matching_word_index, int thread_index)
		{
			FindTextRequest request = job.m_Request;
			if (request.m_Patterns.Length != 0)
			{
				Pattern[] patterns = (Pattern[])request.m_Patterns.Clone();
				if (!request.m_MatchCase)
				{
					for (int i = 0; i < patterns.Length; i++)
					{
						patterns[i].m_Pattern = patterns[i].m_Pattern.ToLower();
					}
				}
				int maximum_file_size = VSAnythingPackage.Inst.GetSettingsDialogPage().MaximumFileSize;
				foreach (string file in files)
				{
					if (parent_context.Cancelled)
					{
						break;
					}
					UnsavedDocument unsaved_document;
					if (job.m_UnsavedDocuments.TryGetValue(file, out unsaved_document))
					{
						this.SearchFile(job, unsaved_document.Filename, unsaved_document.Lines, unsaved_document.LinesLowercase, patterns, request.m_MatchCase, match_whole_word, parent_context, matching_words, ref matching_word_index);
					}
					else
					{
						CachedFile cached_file = null;
						Dictionary<string, CachedFile> fileCache = this.m_FileCache;
						lock (fileCache)
						{
							this.m_FileCache.TryGetValue(file, out cached_file);
						}
						if (cached_file == null)
						{
							this.ScanFile(file, maximum_file_size);
							fileCache = this.m_FileCache;
							lock (fileCache)
							{
								this.m_FileCache.TryGetValue(file, out cached_file);
							}
						}
						if (cached_file != null)
						{
							CachedFile obj = cached_file;
							lock (obj)
							{
								this.SearchFile(job, cached_file.m_Filename, cached_file.m_Lines, cached_file.m_LinesLowercase, patterns, request.m_MatchCase, match_whole_word, parent_context, matching_words, ref matching_word_index);
							}
						}
						if (parent_context.Cancelled)
						{
							break;
						}
						if (matching_words.Count == job.m_Request.m_MaxResultCount)
						{
							break;
						}
					}
					this.m_ThreadFilesProcessedCount[thread_index]++;
				}
			}
		}

		private void SearchFiles_Wildcards(TextFinder.TextFinderJob job, List<string> files, AsyncTask.Context parent_context, bool match_whole_word, List<TextFinderResult> matching_words, ref int matching_word_index, int thread_index)
		{
			FindTextRequest request = job.m_Request;
			if (request.m_Patterns.Length != 0)
			{
				Pattern[] patterns = (Pattern[])request.m_Patterns.Clone();
				if (!request.m_MatchCase)
				{
					for (int i = 0; i < patterns.Length; i++)
					{
						patterns[i].m_Pattern = patterns[i].m_Pattern.ToLower();
					}
				}
				int maximum_file_size = VSAnythingPackage.Inst.GetSettingsDialogPage().MaximumFileSize;
				foreach (string file in files)
				{
					if (parent_context.Cancelled)
					{
						break;
					}
					UnsavedDocument unsaved_document;
					if (job.m_UnsavedDocuments.TryGetValue(file, out unsaved_document))
					{
						this.SearchFile_Wildcards(job, unsaved_document.Filename, unsaved_document.Lines, unsaved_document.LinesLowercase, patterns, request.m_MatchCase, match_whole_word, parent_context, matching_words, ref matching_word_index);
					}
					else
					{
						CachedFile cached_file = null;
						Dictionary<string, CachedFile> fileCache = this.m_FileCache;
						lock (fileCache)
						{
							this.m_FileCache.TryGetValue(file, out cached_file);
						}
						if (cached_file == null)
						{
							this.ScanFile(file, maximum_file_size);
							fileCache = this.m_FileCache;
							lock (fileCache)
							{
								this.m_FileCache.TryGetValue(file, out cached_file);
							}
						}
						if (cached_file != null)
						{
							CachedFile obj = cached_file;
							lock (obj)
							{
								this.SearchFile_Wildcards(job, cached_file.m_Filename, cached_file.m_Lines, cached_file.m_LinesLowercase, patterns, request.m_MatchCase, match_whole_word, parent_context, matching_words, ref matching_word_index);
							}
						}
						if (parent_context.Cancelled)
						{
							break;
						}
						if (matching_words.Count == job.m_Request.m_MaxResultCount)
						{
							break;
						}
					}
					this.m_ThreadFilesProcessedCount[thread_index]++;
				}
			}
		}

		private void SearchFiles_RegExp(TextFinder.TextFinderJob job, List<string> files, AsyncTask.Context parent_context, bool match_whole_word, List<TextFinderResult> matching_words, ref int matching_word_index, int thread_index)
		{
			FindTextRequest request = job.m_Request;
			if (request.m_Patterns.Length != 0)
			{
				Pattern[] patterns = (Pattern[])request.m_Patterns.Clone();
				if (!request.m_MatchCase)
				{
					for (int i = 0; i < patterns.Length; i++)
					{
						patterns[i].m_Pattern = patterns[i].m_Pattern.ToLower();
					}
				}
				List<Regex> reg_exps = new List<Regex>();
				Pattern[] array = patterns;
				for (int j = 0; j < array.Length; j++)
				{
					Pattern pattern = array[j];
					Regex regex = null;
					try
					{
						regex = new Regex(pattern.m_Pattern);
					}
					catch (Exception arg_89_0)
					{
						Log.WriteLine(arg_89_0.Message);
						regex = new Regex(".*");
					}
					reg_exps.Add(regex);
				}
				int maximum_file_size = VSAnythingPackage.Inst.GetSettingsDialogPage().MaximumFileSize;
				foreach (string file in files)
				{
					if (parent_context.Cancelled)
					{
						break;
					}
					UnsavedDocument unsaved_document;
					if (job.m_UnsavedDocuments.TryGetValue(file, out unsaved_document))
					{
						this.SearchFile_RegExp(job, unsaved_document.Filename, unsaved_document.Lines, unsaved_document.LinesLowercase, patterns, reg_exps, request.m_MatchCase, match_whole_word, parent_context, matching_words, ref matching_word_index);
					}
					else
					{
						CachedFile cached_file = null;
						Dictionary<string, CachedFile> fileCache = this.m_FileCache;
						lock (fileCache)
						{
							this.m_FileCache.TryGetValue(file, out cached_file);
						}
						if (cached_file == null)
						{
							this.ScanFile(file, maximum_file_size);
							fileCache = this.m_FileCache;
							lock (fileCache)
							{
								this.m_FileCache.TryGetValue(file, out cached_file);
							}
						}
						if (cached_file != null)
						{
							CachedFile obj = cached_file;
							lock (obj)
							{
								this.SearchFile_RegExp(job, cached_file.m_Filename, cached_file.m_Lines, cached_file.m_LinesLowercase, patterns, reg_exps, request.m_MatchCase, match_whole_word, parent_context, matching_words, ref matching_word_index);
							}
						}
						if (parent_context.Cancelled)
						{
							break;
						}
						if (matching_words.Count == job.m_Request.m_MaxResultCount)
						{
							break;
						}
					}
					this.m_ThreadFilesProcessedCount[thread_index]++;
				}
			}
		}

		private bool IsWordChar(char c)
		{
			return this.m_WordCharMap[(int)c];
		}

		private bool IsWord(string line, int start, int end)
		{
			if (start != 0 && this.IsWordChar(line[start - 1]))
			{
				return false;
			}
			if (end != line.Length && this.IsWordChar(line[end]))
			{
				return false;
			}
			for (int i = start; i < end; i++)
			{
				if (!this.IsWordChar(line[i]))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsWordIgnoringInnards(string line, int start, int end)
		{
			return (start == 0 || !this.IsWordChar(line[start - 1])) && (end == line.Length || !this.IsWordChar(line[end]));
		}

		private void SearchFile(TextFinder.TextFinderJob job, string filename, List<string> lines, List<string> lines_lowercase, Pattern[] patterns, bool match_case, bool match_whole_word, AsyncTask.Context context, List<TextFinderResult> matching_words, ref int matching_word_index)
		{
			try
			{
				FindTextRequest arg_06_0 = job.m_Request;
				List<string> arg_28_0 = match_case ? lines : lines_lowercase;
				int max_result_count = job.m_Request.m_MaxResultCount;
				bool[] pattern_match_results = new bool[patterns.Length];
				int index = 0;
				foreach (string line in arg_28_0)
				{
					int match_start = 2147483647;
					int match_end = -2147483648;
					if (this.MatchLine(line, patterns, match_whole_word, pattern_match_results, ref match_start, ref match_end) && this.ApplyLogicalOperators(patterns, pattern_match_results))
					{
						TextFinderResult matching_word = default(TextFinderResult);
						matching_word.m_Filename = filename;
						matching_word.m_LineIndex = index;
						matching_word.m_Line = lines[index];
						matching_word.m_StartIndex = match_start;
						matching_word.m_EndIndex = match_end;
						int num = matching_word_index;
						matching_word_index = num + 1;
						matching_word.m_Index = num;
						if (job.m_PathMode == PathMode.Relative)
						{
							matching_word.m_Filename = Misc.GetRelativePath(job.m_RootPath, matching_word.m_Filename);
						}
						matching_words.Add(matching_word);
						if (matching_words.Count == max_result_count)
						{
							break;
						}
					}
					if (context.Cancelled)
					{
						break;
					}
					index++;
				}
			}
			catch (Exception arg_11B_0)
			{
				Utils.LogException(arg_11B_0);
			}
		}

		private void SearchFile_Wildcards(TextFinder.TextFinderJob job, string filename, List<string> lines, List<string> lines_lowercase, Pattern[] patterns, bool match_case, bool match_whole_word, AsyncTask.Context context, List<TextFinderResult> matching_words, ref int matching_word_index)
		{
			try
			{
				FindTextRequest arg_06_0 = job.m_Request;
				List<string> arg_28_0 = match_case ? lines : lines_lowercase;
				int max_result_count = job.m_Request.m_MaxResultCount;
				bool[] pattern_match_results = new bool[patterns.Length];
				int index = 0;
				foreach (string line in arg_28_0)
				{
					int match_start = 2147483647;
					int match_end = -2147483648;
					if (this.MatchLine_Wildcards(line, patterns, match_whole_word, pattern_match_results, ref match_start, ref match_end) && this.ApplyLogicalOperators(patterns, pattern_match_results))
					{
						TextFinderResult matching_word = default(TextFinderResult);
						matching_word.m_Filename = filename;
						matching_word.m_LineIndex = index;
						matching_word.m_Line = lines[index];
						matching_word.m_StartIndex = match_start;
						matching_word.m_EndIndex = match_end;
						int num = matching_word_index;
						matching_word_index = num + 1;
						matching_word.m_Index = num;
						if (job.m_PathMode == PathMode.Relative)
						{
							matching_word.m_Filename = Misc.GetRelativePath(job.m_RootPath, matching_word.m_Filename);
						}
						matching_words.Add(matching_word);
						if (matching_words.Count == max_result_count)
						{
							break;
						}
					}
					if (context.Cancelled)
					{
						break;
					}
					index++;
				}
			}
			catch (Exception arg_11B_0)
			{
				Utils.LogException(arg_11B_0);
			}
		}

		private void SearchFile_RegExp(TextFinder.TextFinderJob job, string filename, List<string> lines, List<string> lines_lowercase, Pattern[] patterns, List<Regex> reg_exps, bool match_case, bool match_whole_word, AsyncTask.Context context, List<TextFinderResult> matching_words, ref int matching_word_index)
		{
			try
			{
				FindTextRequest arg_06_0 = job.m_Request;
				List<string> arg_28_0 = match_case ? lines : lines_lowercase;
				int max_result_count = job.m_Request.m_MaxResultCount;
				bool[] pattern_match_results = new bool[patterns.Length];
				int index = 0;
				foreach (string line in arg_28_0)
				{
					int match_start = 2147483647;
					int match_end = -2147483648;
					if (this.MatchLine_RegExp(line, reg_exps, match_whole_word, pattern_match_results, ref match_start, ref match_end) && this.ApplyLogicalOperators(patterns, pattern_match_results))
					{
						TextFinderResult matching_word = default(TextFinderResult);
						matching_word.m_Filename = filename;
						matching_word.m_LineIndex = index;
						matching_word.m_Line = lines[index];
						matching_word.m_StartIndex = match_start;
						matching_word.m_EndIndex = match_end;
						int num = matching_word_index;
						matching_word_index = num + 1;
						matching_word.m_Index = num;
						if (job.m_PathMode == PathMode.Relative)
						{
							matching_word.m_Filename = Misc.GetRelativePath(job.m_RootPath, matching_word.m_Filename);
						}
						matching_words.Add(matching_word);
						if (matching_words.Count == max_result_count)
						{
							break;
						}
					}
					if (context.Cancelled)
					{
						break;
					}
					index++;
				}
			}
			catch (Exception arg_11B_0)
			{
				Utils.LogException(arg_11B_0);
			}
		}

		private int FindNextNonWordChar(string line, int index)
		{
			int len = line.Length;
			while (index < len && this.IsWordChar(line[index]))
			{
				index++;
			}
			return index;
		}

		private bool MatchLine(string line, Pattern[] patterns, bool match_whole_word, bool[] pattern_match_results, ref int match_start, ref int match_end)
		{
			bool found_match = false;
			int i = 0;
			for (int j = 0; j < patterns.Length; j++)
			{
				Pattern pattern = patterns[j];
				bool pattern_match = false;
				int offset = 0;
				while (line.Length != 0)
				{
					int start = -1;
					int end = -1;
					int index = line.IndexOf(pattern.m_Pattern, StringComparison.Ordinal);
					if (index != -1)
					{
						pattern_match = true;
						start = index;
						end = start + pattern.m_Pattern.Length;
					}
					bool try_again = false;
					if ((match_whole_word & pattern_match) && !this.IsWordIgnoringInnards(line, start, end))
					{
						pattern_match = false;
						try_again = true;
						line = line.Substring(this.FindNextNonWordChar(line, end));
						offset = end;
					}
					if (pattern_match)
					{
						found_match = true;
						start += offset;
						end += offset;
						if (start < match_start)
						{
							match_start = start;
						}
						if (end > match_end)
						{
							match_end = end;
						}
					}
					if (!try_again)
					{
						break;
					}
				}
				pattern_match_results[i] = pattern_match;
				i++;
			}
			return found_match;
		}

		private bool MatchLine_Wildcards(string line, Pattern[] patterns, bool match_whole_word, bool[] pattern_match_results, ref int match_start, ref int match_end)
		{
			bool found_match = false;
			int i = 0;
			for (int j = 0; j < patterns.Length; j++)
			{
				Pattern pattern = patterns[j];
				bool pattern_match = false;
				int offset = 0;
				while (line.Length != 0)
				{
					int start = -1;
					int end = -1;
					if (pattern.m_UseWildcard)
					{
						pattern_match = Wildcard.Match(line, pattern.m_Pattern, ref start, ref end);
					}
					else
					{
						int index = line.IndexOf(pattern.m_Pattern, StringComparison.Ordinal);
						if (index != -1)
						{
							pattern_match = true;
							start = index;
							end = start + pattern.m_Pattern.Length;
						}
					}
					bool try_again = false;
					if ((match_whole_word & pattern_match) && !this.IsWord(line, start, end))
					{
						pattern_match = false;
						try_again = true;
						line = line.Substring(this.FindNextNonWordChar(line, end));
						offset = end;
					}
					if (pattern_match)
					{
						found_match = true;
						start += offset;
						end += offset;
						if (start < match_start)
						{
							match_start = start;
						}
						if (end > match_end)
						{
							match_end = end;
						}
					}
					if (!try_again)
					{
						break;
					}
				}
				pattern_match_results[i] = pattern_match;
				i++;
			}
			return found_match;
		}

		private bool MatchLine_RegExp(string line, List<Regex> reg_exps, bool match_whole_word, bool[] pattern_match_results, ref int match_start, ref int match_end)
		{
			bool found_match = false;
			int i = 0;
			foreach (Regex reg_exp in reg_exps)
			{
				bool pattern_match = false;
				int offset = 0;
				while (line.Length != 0)
				{
					int start = -1;
					int end = -1;
					try
					{
						Match match = reg_exp.Match(line);
						pattern_match = match.Success;
						start = match.Index;
						end = start + match.Length;
					}
					catch (Exception)
					{
					}
					bool try_again = false;
					if ((match_whole_word & pattern_match) && !this.IsWord(line, start, end))
					{
						pattern_match = false;
						try_again = true;
						line = line.Substring(this.FindNextNonWordChar(line, end));
						offset = end;
					}
					if (pattern_match)
					{
						found_match = true;
						start += offset;
						end += offset;
						if (start < match_start)
						{
							match_start = start;
						}
						if (end > match_end)
						{
							match_end = end;
						}
					}
					if (!try_again)
					{
						break;
					}
				}
				pattern_match_results[i] = pattern_match;
				i++;
			}
			return found_match;
		}

		private bool ApplyLogicalOperators(Pattern[] patterns, bool[] pattern_match_results)
		{
			bool match = true;
            for (int i = 0; i < patterns.Length; i++)
            {
                Pattern arg_11_0 = patterns[i];
                bool pattern_match = pattern_match_results[i];
                switch (arg_11_0.m_Operator)
                {
                    case Pattern.Operator.AND:
                        match &= pattern_match;
                        break;
                    case Pattern.Operator.OR:
                        match |= pattern_match;
                        break;
                    case Pattern.Operator.AND_NOT:
                        match = (match && !pattern_match);
                        break;
                }
            }
			return match;
		}

		public void RemoveFile(string filename)
		{
			string norm_filename = Utils.NormalisePath(filename);
			Dictionary<string, CachedFile> fileCache = this.m_FileCache;
			lock (fileCache)
			{
				if (this.m_FileCache.ContainsKey(norm_filename))
				{
					this.m_FileCache.Remove(norm_filename);
				}
			}
		}

		private static string GetCacheFilename(string solution_path)
		{
			solution_path = solution_path.Replace('\\', '_');
			solution_path = solution_path.Replace(':', '_');
			solution_path = solution_path.Replace('.', '_');
			return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FastFind\\" + solution_path + ".ff_cache";
		}

		private void Read(AsyncTask.Context context)
		{
            /// 读取缓存文件内容，直接用于文本查找
			string cache_filename = TextFinder.GetCacheFilename((string)context.Arg);
			Dictionary<string, CachedFile> file_cache = new Dictionary<string, CachedFile>();
			try
			{
				if (!File.Exists(cache_filename))
				{
					return;
				}
				for (int i = 0; i < 10; i++)
				{
					try
					{
						BinaryReader reader = new BinaryReader(new FileStream(cache_filename, FileMode.Open, FileAccess.Read));
						if (reader.ReadInt32() != 1)
						{
							return;
						}
						int count = reader.ReadInt32();
						for (int j = 0; j < count; j++)
						{
							string filename = reader.ReadString();
							CachedFile cached_file = new CachedFile();
							cached_file.Read(reader);
							file_cache[filename] = cached_file;
							if (context.Cancelled)
							{
								break;
							}
						}
						reader.Close();
						break;
					}
					catch (Exception arg_8F_0)
					{
						Utils.LogExceptionQuiet(arg_8F_0);
						Thread.Sleep(100);
					}
				}
			}
			catch (Exception arg_A8_0)
			{
				Utils.LogExceptionQuiet(arg_A8_0);
			}
			Dictionary<string, CachedFile> fileCache = this.m_FileCache;
			lock (fileCache)
			{
				ProfileTimer timer = new ProfileTimer("Read File Cache Finish");
				foreach (KeyValuePair<string, CachedFile> entry in file_cache)
				{
					if (!this.m_FileCache.ContainsKey(entry.Key))
					{
						this.m_FileCache[entry.Key] = entry.Value;
					}
				}
				timer.Stop();
			}
		}

		public void Write()
		{
			if (this.m_SolutionPath == null)
			{
				return;
			}
			string cache_filename = TextFinder.GetCacheFilename(this.m_SolutionPath);
			Dictionary<string, CachedFile> fileCache = this.m_FileCache;
			lock (fileCache)
			{
				bool success = false;
				string tmp_filename = cache_filename + ".tmp";
				for (int i = 0; i < 10; i++)
				{
					try
					{
						BinaryWriter writer = new BinaryWriter(new FileStream(tmp_filename, FileMode.Create, FileAccess.Write));
						writer.Write(1);
						writer.Write(this.m_FileCache.Count);
						foreach (KeyValuePair<string, CachedFile> entry in this.m_FileCache)
						{
							writer.Write(entry.Key);
							entry.Value.Write(writer);
						}
						writer.Close();
						success = true;
						break;
					}
					catch (Exception arg_C0_0)
					{
						Utils.LogExceptionQuiet(arg_C0_0);
						Thread.Sleep(100);
					}
				}
				if (success)
				{
					for (int j = 0; j < 10; j++)
					{
						try
						{
							File.Delete(cache_filename);
							File.Copy(tmp_filename, cache_filename);
							break;
						}
						catch (Exception arg_F6_0)
						{
							Utils.LogExceptionQuiet(arg_F6_0);
							Thread.Sleep(100);
						}
					}
					try
					{
						File.Delete(tmp_filename);
					}
					catch (Exception arg_11A_0)
					{
						Utils.LogExceptionQuiet(arg_11A_0);
					}
				}
			}
		}

		private void DirectoryWatcherFileChanged(string path)
		{
			Set<string> modifiedFiles = this.m_ModifiedFiles;
			lock (modifiedFiles)
			{
				this.m_ModifiedFiles.Add(Utils.NormalisePath(path));
			}
			this.m_FileChangedHandlerTask.Start(100);
		}

		private void HandleFileChangedEventsTask(AsyncTask.Context context)
		{
			try
			{
				Set<string> modifiedFiles = this.m_ModifiedFiles;
				Set<string> modified_files;
				lock (modifiedFiles)
				{
					modified_files = new Set<string>(this.m_ModifiedFiles);
					this.m_ModifiedFiles.Clear();
				}
				int maximum_file_size = VSAnythingPackage.Inst.GetSettingsDialogPage().MaximumFileSize;
				foreach (string modified_file in modified_files)
				{
					string ext = Path.GetExtension(modified_file).ToLower();
					if (VSAnythingPackage.Inst.GetSettingsDialogPage().ExtList.Contains(ext))
					{
						this.ScanFile(modified_file, maximum_file_size);
					}
				}
			}
			catch (Exception arg_9F_0)
			{
				Utils.LogException(arg_9F_0);
			}
		}
	}
}
