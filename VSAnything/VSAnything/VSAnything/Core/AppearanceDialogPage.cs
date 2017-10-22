using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Company.VSAnything
{
	[ClassInterface(ClassInterfaceType.AutoDual), Guid("f66d4531-7bcb-4d60-9fa6-a4a0865b9336")]
	internal class AppearanceDialogPage : DialogPage
	{
		private string m_FontName = "Consolas";

		private float m_FontSize = 9f;

		private ColourTheme m_ColourTheme;

		private ColourSettings m_LightColours = new ColourSettings();

		private ColourSettings m_DarkColours = new ColourSettings();

		private ColourSettings m_CustomColours = new ColourSettings();

		private AppearanceControl m_AppearanceControl;

		public string FontName
		{
			get
			{
				return this.m_FontName;
			}
			set
			{
				this.m_FontName = value;
			}
		}

		public float FontSize
		{
			get
			{
				return this.m_FontSize;
			}
			set
			{
				this.m_FontSize = value;
			}
		}

		public ColourTheme ColourTheme
		{
			get
			{
				return this.m_ColourTheme;
			}
			set
			{
				this.m_ColourTheme = value;
			}
		}

		public Color BackColour
		{
			get
			{
				return this.m_CustomColours.m_BackColour;
			}
			set
			{
				this.m_CustomColours.m_BackColour = value;
			}
		}

		public Color ForeColour
		{
			get
			{
				return this.m_CustomColours.m_ForeColour;
			}
			set
			{
				this.m_CustomColours.m_ForeColour = value;
			}
		}

		public Color ControlColour
		{
			get
			{
				return this.m_CustomColours.m_ControlColour;
			}
			set
			{
				this.m_CustomColours.m_ControlColour = value;
			}
		}

		public Color SelectColour
		{
			get
			{
				return this.m_CustomColours.m_SelectColour;
			}
			set
			{
				this.m_CustomColours.m_SelectColour = value;
			}
		}

		public Color HighlightColour
		{
			get
			{
				return this.m_CustomColours.m_HighlightColour;
			}
			set
			{
				this.m_CustomColours.m_HighlightColour = value;
			}
		}

		public Color HighlightTextColour
		{
			get
			{
				return this.m_CustomColours.m_HighlightTextColour;
			}
			set
			{
				this.m_CustomColours.m_HighlightTextColour = value;
			}
		}

		public Color SelectedHighlightTextColour
		{
			get
			{
				return this.m_CustomColours.m_SelectedHighlightTextColour;
			}
			set
			{
				this.m_CustomColours.m_SelectedHighlightTextColour = value;
			}
		}

		public Color CodeColour
		{
			get
			{
				return this.m_CustomColours.m_CodeColour;
			}
			set
			{
				this.m_CustomColours.m_CodeColour = value;
			}
		}

		public Color CodeFilenameColour
		{
			get
			{
				return this.m_CustomColours.m_CodeFilenameColour;
			}
			set
			{
				this.m_CustomColours.m_CodeFilenameColour = value;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected override IWin32Window Window
		{
			get
			{
				if (this.m_AppearanceControl == null)
				{
					this.m_AppearanceControl = new AppearanceControl(this);
				}
				return this.m_AppearanceControl;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ColourSettings Colours
		{
			get
			{
				switch (this.m_ColourTheme)
				{
				case ColourTheme.Light:
					return this.m_LightColours;
				case ColourTheme.Dark:
					return this.m_DarkColours;
				case ColourTheme.Custom:
					return this.m_CustomColours;
				default:
					return this.m_CustomColours;
				}
			}
		}

		public AppearanceDialogPage()
		{
			this.SetupDefaultColours();
		}

		private void SetupDefaultColours()
		{
			this.m_LightColours.m_BackColour = Color.White;
			this.m_LightColours.m_ForeColour = Color.Black;
			this.m_LightColours.m_ControlColour = Color.FromArgb(192, 192, 192);
			this.m_LightColours.m_SelectColour = Color.FromArgb(64, 0, 128);
			this.m_LightColours.m_HighlightColour = Color.Blue;
			this.m_LightColours.m_HighlightTextColour = Color.White;
			this.m_LightColours.m_SelectedHighlightTextColour = Color.Yellow;
			this.m_LightColours.m_CodeColour = Color.Black;
			this.m_LightColours.m_CodeFilenameColour = Color.FromArgb(74, 74, 74);
			this.m_DarkColours.m_BackColour = Color.Black;
			this.m_DarkColours.m_ForeColour = Color.White;
			this.m_DarkColours.m_ControlColour = Color.FromArgb(37, 37, 38);
			this.m_DarkColours.m_SelectColour = Color.FromArgb(149, 47, 15);
			this.m_DarkColours.m_HighlightColour = Color.Cyan;
			this.m_DarkColours.m_HighlightTextColour = this.m_DarkColours.m_ForeColour;
			this.m_DarkColours.m_SelectedHighlightTextColour = this.m_DarkColours.m_HighlightColour;
			this.m_DarkColours.m_CodeColour = Color.White;
			this.m_DarkColours.m_CodeFilenameColour = Color.FromArgb(192, 192, 192);
			this.m_CustomColours.m_BackColour = this.m_DarkColours.m_BackColour;
			this.m_CustomColours.m_ForeColour = this.m_DarkColours.m_ForeColour;
			this.m_CustomColours.m_ControlColour = this.m_DarkColours.m_ControlColour;
			this.m_CustomColours.m_SelectColour = this.m_DarkColours.m_SelectColour;
			this.m_CustomColours.m_HighlightColour = this.m_DarkColours.m_HighlightColour;
			this.m_CustomColours.m_HighlightTextColour = this.m_DarkColours.m_ForeColour;
			this.m_CustomColours.m_SelectedHighlightTextColour = this.m_DarkColours.m_HighlightColour;
			this.m_CustomColours.m_CodeColour = this.m_DarkColours.m_CodeColour;
			this.m_CustomColours.m_CodeFilenameColour = this.m_DarkColours.m_CodeFilenameColour;
		}

		protected override void OnDeactivate(CancelEventArgs e)
		{
			this.m_AppearanceControl.SubmitData();
			base.OnDeactivate(e);
		}
	}
}
