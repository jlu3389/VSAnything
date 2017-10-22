using System;
using System.Drawing;

namespace SCLCoreCLR
{
	public struct ColourF
	{
		public float R;

		public float G;

		public float B;

		public float A;

		public static ColourF Black = new ColourF(0f, 0f, 0f);

		public static ColourF White = new ColourF(1f, 1f, 1f);

		public static ColourF Red = new ColourF(1f, 0f, 0f);

		public static ColourF Green = new ColourF(0f, 1f, 0f);

		public static ColourF Blue = new ColourF(0f, 0f, 1f);

		public static ColourF Yellow = new ColourF(1f, 1f, 0f);

		public static ColourF Orange = new ColourF(1f, 0.5f, 0f);

		public ColourF(float r, float g, float b)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = 1f;
		}

		public ColourF(float r, float g, float b, float a)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = a;
		}

		public ColourF(ColourF colour, float a)
		{
			this.R = colour.R;
			this.G = colour.G;
			this.B = colour.B;
			this.A = a;
		}

		public ColourF(Color colour)
		{
			this.R = (float)colour.R / 255f;
			this.G = (float)colour.G / 255f;
			this.B = (float)colour.B / 255f;
			this.A = (float)colour.A / 255f;
		}

		public ColourF(Color colour, float a)
		{
			this.R = (float)colour.R / 255f;
			this.G = (float)colour.G / 255f;
			this.B = (float)colour.B / 255f;
			this.A = a;
		}

		public Color ToColor()
		{
			return Color.FromArgb((int)(this.A * 255f), (int)(this.R * 255f), (int)(this.G * 255f), (int)(this.B * 255f));
		}
	}
}
