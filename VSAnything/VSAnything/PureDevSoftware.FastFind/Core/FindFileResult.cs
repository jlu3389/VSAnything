using System;

namespace Company.VSAnything
{
	internal struct FindFileResult
	{
		public string m_FilePath;

		public string m_FileName;

		public string m_RelativeFilePath;

		public bool m_PathMatch;

		public bool MyEquals(object obj)
		{
			return obj is FindFileResult && this.m_FilePath == ((FindFileResult)obj).m_FilePath;
		}
	}
}
