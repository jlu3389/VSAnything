using System;
using System.Collections.Generic;
using System.IO;

namespace Company.VSAnything
{
	internal class Solution
	{
		public bool m_FullScanComplete;

		public List<string> m_Files = new List<string>();

		public List<string> m_Projects = new List<string>();

		public void Read(BinaryReader reader)
		{
			this.m_FullScanComplete = reader.ReadBoolean();
			Utils.Read(this.m_Files, reader);
			Utils.Read(this.m_Projects, reader);
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(this.m_FullScanComplete);
			Utils.Write(this.m_Files, writer);
			Utils.Write(this.m_Projects, writer);
		}
	}
}
