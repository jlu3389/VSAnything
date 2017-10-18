using System;
using System.IO;

namespace Company.VSAnything
{
	internal class FindTextResult
	{
		public string m_Path;

		public int m_LineIndex;

		public string m_Line;

		public int m_StartIndex;

		public int m_EndIndex;

		public string m_Filename;

		public FindTextResult(string path, int line_index, string line, int start_index, int end_index)
		{
			this.m_Path = path;
			this.m_LineIndex = line_index;
			this.m_Line = line;
			this.m_StartIndex = start_index;
			this.m_EndIndex = end_index;
			this.m_Filename = Path.GetFileName(this.m_Path);
		}

		public bool MyEquals(object obj)
		{
			if (!(obj is FindTextResult))
			{
				return false;
			}
			FindTextResult other = (FindTextResult)obj;
			return this.m_Line == other.m_Line && this.m_StartIndex == other.m_StartIndex && this.m_EndIndex == other.m_EndIndex && this.m_Filename == other.m_Filename;
		}
	}
}
