using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Company.VSAnything
{
	internal class DirectoryWatcher
	{
		private Dictionary<string, FileSystemWatcher> m_Watchers = new Dictionary<string, FileSystemWatcher>();

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event FileChangedHandler FileChanged;

		public void StartWatching(string path)
		{
			string norm_path = Utils.NormalisePath(path);
			if (!this.m_Watchers.ContainsKey(norm_path))
			{
				FileSystemWatcher watcher = new FileSystemWatcher(norm_path);
				watcher.Changed += new FileSystemEventHandler(this.FileChangedEvent);
				watcher.IncludeSubdirectories = true;
				watcher.EnableRaisingEvents = true;
				this.m_Watchers[norm_path] = watcher;
			}
		}

		public void StopWatching(string path)
		{
			string norm_path = Utils.NormalisePath(path);
			if (this.m_Watchers.ContainsKey(norm_path))
			{
				this.m_Watchers[norm_path].Changed -= new FileSystemEventHandler(this.FileChangedEvent);
				this.m_Watchers.Remove(norm_path);
			}
		}

		private void FileChangedEvent(object sender, FileSystemEventArgs e)
		{
			if (this.FileChanged != null)
			{
				this.FileChanged(e.FullPath);
			}
		}
	}
}
