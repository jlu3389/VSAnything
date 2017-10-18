using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Company.VSAnything
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	internal class Resource1
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resource1.resourceMan == null)
				{
					Resource1.resourceMan = new ResourceManager("Company.VSAnything.Resource1", typeof(Resource1).Assembly);
				}
				return Resource1.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resource1.resourceCulture;
			}
			set
			{
				Resource1.resourceCulture = value;
			}
		}

		internal static Bitmap DropDownArrow
		{
			get
			{
				return (Bitmap)Resource1.ResourceManager.GetObject("DropDownArrow", Resource1.resourceCulture);
			}
		}

		internal static Icon FastFindIcon
		{
			get
			{
				return (Icon)Resource1.ResourceManager.GetObject("FastFindIcon", Resource1.resourceCulture);
			}
		}

		internal static Icon VSAnythingPackage
		{
			get
			{
				return (Icon)Resource1.ResourceManager.GetObject("VSAnythingPackage", Resource1.resourceCulture);
			}
		}

		internal static Bitmap FastFindSettings
		{
			get
			{
				return (Bitmap)Resource1.ResourceManager.GetObject("FastFindSettings", Resource1.resourceCulture);
			}
		}

		internal static Bitmap Images
		{
			get
			{
				return (Bitmap)Resource1.ResourceManager.GetObject("Images", Resource1.resourceCulture);
			}
		}

		internal static Bitmap Package
		{
			get
			{
				return (Bitmap)Resource1.ResourceManager.GetObject("Package", Resource1.resourceCulture);
			}
		}

		internal static Bitmap puredev_software_logo_email
		{
			get
			{
				return (Bitmap)Resource1.ResourceManager.GetObject("puredev_software_logo_email", Resource1.resourceCulture);
			}
		}

		internal Resource1()
		{
		}
	}
}
