using System;

namespace Company.VSAnything
{
	internal struct TextFinderResult
	{
		public string m_Filename;

		public int m_LineIndex;

		public string m_Line;

		public int m_StartIndex;

		public int m_EndIndex;

		public int m_Index;

        bool bConsiderFileName; // 有部分是通过文件名匹配到的
	}
}
