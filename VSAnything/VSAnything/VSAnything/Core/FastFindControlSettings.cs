using SCLCoreCLR;
using System;

namespace Company.VSAnything
{
	internal class FastFindControlSettings
	{
		public bool m_OptionsPanelVisible;

		public bool m_FastFindShowFiles;

		public bool m_FastFindFindText;

		public bool m_MatchWholeWord;

		public bool m_RegExpression;

		public bool m_FastFindWildcards;

		public bool m_FindTextMatchCase;

		public bool m_UseLogicalOperators;

		public bool m_SolutionFilesMatchCase;

		public string m_FastFindFileExt;

		public void SetDefaults()
		{
			this.m_OptionsPanelVisible = false;
			this.m_FastFindShowFiles = true;
			this.m_FastFindFindText = true;
			this.m_FastFindWildcards = true;
			this.m_MatchWholeWord = false;
			this.m_RegExpression = false;
			this.m_SolutionFilesMatchCase = false;
			this.m_FindTextMatchCase = false;
			this.m_UseLogicalOperators = true;
			this.m_FastFindFileExt = "Default";
		}

		public void Read(string name, XmlReadStream stream)
		{
			if (stream.StartElement(name))
			{
				stream.Read("OptionsPanelVisible", ref this.m_OptionsPanelVisible);
				stream.Read("FastFindShowFiles", ref this.m_FastFindShowFiles);
				stream.Read("FastFindShowLines", ref this.m_FastFindFindText);
				stream.Read("FastFindWildcards", ref this.m_FastFindWildcards);
				stream.Read("MatchWholeWord", ref this.m_MatchWholeWord);
				stream.Read("RegExpression", ref this.m_RegExpression);
				stream.Read("SolutionFilesMatchCase", ref this.m_SolutionFilesMatchCase);
				stream.Read("FindTextMatchCase", ref this.m_FindTextMatchCase);
				stream.Read("UseLogicalOperators", ref this.m_UseLogicalOperators);
				stream.Read("FastFindFileExt", ref this.m_FastFindFileExt);
				stream.EndElement();
			}
		}

		public void Write(string name, XmlWriteStream stream)
		{
			stream.StartElement(name);
			stream.Write<bool>("OptionsPanelVisible", this.m_OptionsPanelVisible);
			stream.Write<bool>("FastFindShowFiles", this.m_FastFindShowFiles);
			stream.Write<bool>("FastFindShowLines", this.m_FastFindFindText);
			stream.Write<bool>("FastFindWildcards", this.m_FastFindWildcards);
			stream.Write<bool>("MatchWholeWord", this.m_MatchWholeWord);
			stream.Write<bool>("RegExpression", this.m_RegExpression);
			stream.Write<bool>("SolutionFilesMatchCase", this.m_SolutionFilesMatchCase);
			stream.Write<bool>("FindTextMatchCase", this.m_FindTextMatchCase);
			stream.Write<bool>("UseLogicalOperators", this.m_UseLogicalOperators);
			stream.Write<string>("FastFindFileExt", this.m_FastFindFileExt);
			stream.EndElement();
		}
	}
}
