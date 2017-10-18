using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Company.VSAnything
{
	[ClassInterface(ClassInterfaceType.AutoDual), Guid("c7efce5e-f6c0-42d9-9c05-82faa19781a5")]
	internal class SettingsDialogPage : DialogPage
	{
		private bool m_EnableTips;

		private bool m_RememberLastFind = true;

		private bool m_UseCurrentWordAsFindText = true;

		private bool m_SpacesAsWildcardsForFindFile = true;

		private PathMode m_FindFilesPathMode = PathMode.Relative;

		private PathMode m_FindTextPathMode = PathMode.Relative;

		private int m_MaximumFileSize = 1048576;

		private string[] m_ExtList;

		private int m_MaxResults = 1000;

		[Category("Settings"), Description("Show tips on how to use FastFind at the top of the window"), DisplayName("Enable Tips")]
		public bool EnableTips
		{
			get
			{
				return this.m_EnableTips;
			}
			set
			{
				this.m_EnableTips = value;
			}
		}

		[Category("Settings"), Description("The FastFind window remember the last search value when re-opened"), DisplayName("Remember Last Find")]
		public bool RememberLastFind
		{
			get
			{
				return this.m_RememberLastFind;
			}
			set
			{
				this.m_RememberLastFind = value;
			}
		}

		[Category("Settings"), Description("Uses the current word under the cursor as the initial find text"), DisplayName("Use current word as Find text")]
		public bool UseCurrentWordAsFindText
		{
			get
			{
				return this.m_UseCurrentWordAsFindText;
			}
			set
			{
				this.m_UseCurrentWordAsFindText = value;
			}
		}

		[Category("Settings"), Description("Spaces count as wildcards when matching filenames"), DisplayName("Spaces are wildcards in find file")]
		public bool SpacesAsWildcardsForFindFile
		{
			get
			{
				return this.m_SpacesAsWildcardsForFindFile;
			}
			set
			{
				this.m_SpacesAsWildcardsForFindFile = value;
			}
		}

		[Category("Settings"), Description("How to display the find file paths"), DisplayName("Find-file Paths")]
		public PathMode FindFilesPathMode
		{
			get
			{
				return this.m_FindFilesPathMode;
			}
			set
			{
				this.m_FindFilesPathMode = value;
			}
		}

		[Category("Settings"), Description("How to display the find text file paths"), DisplayName("Find-text file Paths")]
		public PathMode FindTextPathMode
		{
			get
			{
				return this.m_FindTextPathMode;
			}
			set
			{
				this.m_FindTextPathMode = value;
			}
		}

		[Category("Settings"), Description("Do not scan files over this size"), DisplayName("Maximum file size for scan")]
		public int MaximumFileSize
		{
			get
			{
				return this.m_MaximumFileSize;
			}
			set
			{
				this.m_MaximumFileSize = value;
			}
		}

		[Category("Settings"), Description("FastFind will only search for text in files with these extensions"), DisplayName("File Extensions"), TypeConverter(typeof(StringArrayConverter))]
		public string[] ExtList
		{
			get
			{
				return this.m_ExtList;
			}
			set
			{
				this.m_ExtList = value;
			}
		}

		[Category("Settings"), Description("FastFind will limit the number of matches to this value. Increasing this value may slow fastfind down."), DisplayName("Max Results")]
		public int MaxResults
		{
			get
			{
				return this.m_MaxResults;
			}
			set
			{
				this.m_MaxResults = value;
			}
		}

		public override void SaveSettingsToStorage()
		{
			base.SaveSettingsToStorage();
		}

		public override void SaveSettingsToXml(IVsSettingsWriter writer)
		{
			base.SaveSettingsToXml(writer);
		}

		public SettingsDialogPage()
		{
			this.m_ExtList = Utils.ToLower(new List<string>(FileExt.m_ExtToScan)).ToArray();
		}
	}
}
