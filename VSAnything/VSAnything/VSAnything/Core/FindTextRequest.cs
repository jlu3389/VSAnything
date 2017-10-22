using System;
using System.Collections.Generic;

namespace Company.VSAnything
{
	internal class FindTextRequest
	{
		public Pattern[] m_Patterns;

		public List<TextFinderResult> m_MatchingWords = new List<TextFinderResult>();

		public bool m_MatchCase;

		public int m_MaxResultCount;

		public string m_TextBoxText;

		public FindFinishedHandler FindFinished;
	}
}
