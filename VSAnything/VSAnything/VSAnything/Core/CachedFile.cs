using System;
using System.Collections.Generic;
using System.IO;

namespace Company.VSAnything
{
	internal class CachedFile
	{
		public string m_Filename;

		public DateTime m_ModifiedTime;

		public List<string> m_Lines = new List<string>();

		public List<string> m_LinesLowercase = new List<string>();

		public void Read(BinaryReader reader)
		{
			this.m_Filename = reader.ReadString();
			this.m_ModifiedTime = DateTime.FromBinary(reader.ReadInt64());
			Utils.Read(this.m_Lines, reader);
			Utils.Read(this.m_LinesLowercase, reader);
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(this.m_Filename);
			writer.Write(this.m_ModifiedTime.ToBinary());
			Utils.Write(this.m_Lines, writer);
			Utils.Write(this.m_LinesLowercase, writer);
		}
	}
}
