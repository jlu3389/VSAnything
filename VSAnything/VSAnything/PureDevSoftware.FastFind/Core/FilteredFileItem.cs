using System;

namespace Company.VSAnything
{
	internal class FilteredFileItem
	{
		public string m_DisplayFilename;

		public string m_FullPath;

		public FilteredFileItem(string display_filename, string full_path)
		{
			this.m_DisplayFilename = display_filename;
			this.m_FullPath = full_path;
		}
	}
}
