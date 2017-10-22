using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Company.VSAnything
{
	public class FastFindProgressBar : Control
	{
		private const int m_ProgressBarWidthDiv = 10;

		private const int m_MaxProgress = 200;

		private const int m_Period = 2;

		private int m_Progress;

		private SolidBrush m_BarBrush = new SolidBrush(Color.FromArgb(155, 255, 165));

		private IContainer components;

		public int Progress
		{
			get
			{
				return this.m_Progress;
			}
			set
			{
				if (this.m_Progress != value)
				{
					this.m_Progress = value;
					this.Refresh();
				}
			}
		}

		public Color Colour
		{
			get
			{
				return this.m_BarBrush.Color;
			}
			set
			{
				this.m_BarBrush = new SolidBrush(value);
			}
		}

		public FastFindProgressBar()
		{
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			if (this.m_BarBrush != null)
			{
				this.m_BarBrush.Dispose();
				this.m_BarBrush = null;
			}
			base.Dispose(disposing);
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			int progress_bar_width = base.ClientSize.Width / 10;
			int progress_x = this.m_Progress * (base.ClientSize.Width + progress_bar_width) / 100;
			pe.Graphics.Clear(this.BackColor);
			pe.Graphics.FillRectangle(this.m_BarBrush, progress_x - progress_bar_width, 0, progress_bar_width, base.ClientSize.Height);
			base.OnPaint(pe);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
		}
	}
}
