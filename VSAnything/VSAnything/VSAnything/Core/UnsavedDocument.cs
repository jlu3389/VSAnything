using EnvDTE;
using System;
using System.Collections.Generic;

namespace Company.VSAnything
{
	internal class UnsavedDocument
	{
		private string m_Filename;

		private List<string> m_Lines = new List<string>();

		private List<string> m_LinesLowercase = new List<string>();

		public string Filename
		{
			get
			{
				return this.m_Filename;
			}
		}

		public List<string> Lines
		{
			get
			{
				return this.m_Lines;
			}
		}

		public List<string> LinesLowercase
		{
			get
			{
				return this.m_LinesLowercase;
			}
		}

		public UnsavedDocument(Document document)
		{
			this.m_Filename = document.FullName;
			try
			{
				TextDocument text_doc = document.Object("TextDocument") as TextDocument;
				if (text_doc != null)
				{
					EditPoint edit_point = text_doc.CreateEditPoint(null);
					int line_count = text_doc.EndPoint.Line;
					this.m_Lines = new List<string>(line_count);
					this.m_LinesLowercase = new List<string>(line_count);
					for (int i = 1; i <= line_count; i++)
					{
						string line = edit_point.GetLines(i, i + 1);
						this.m_Lines.Add(line);
						this.m_LinesLowercase.Add(line.ToLower());
					}
				}
			}
			catch (Exception arg_A1_0)
			{
				Utils.LogExceptionQuiet(arg_A1_0);
			}
		}
	}
}
