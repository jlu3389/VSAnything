using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Company.VSAnything
{
	internal class OldItemsDropDownForm : Form
	{
		public delegate void SelectionchangedHandler();

		private List<string> m_Items = new List<string>();

		private Brush m_FontBrush;

		private Brush m_SelectBarBrush;

		private Brush m_HighlightBarBrush;

		private int m_SelectedIndex;

		private int m_HighlightIndex = -1;

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event OldItemsDropDownForm.SelectionchangedHandler Selectionchanged;

		private int ItemHeight
		{
			get
			{
				return this.Font.Height + 1;
			}
		}

		public string SelectedItem
		{
			get
			{
				if (this.m_SelectedIndex == -1)
				{
					return "";
				}
				return this.m_Items[this.m_SelectedIndex];
			}
		}

		public OldItemsDropDownForm()
		{
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.SetStyle(ControlStyles.UserPaint, true);
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.Manual;
			base.FormBorderStyle = FormBorderStyle.None;
			this.m_FontBrush = new SolidBrush(this.ForeColor);
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			this.m_FontBrush = new SolidBrush(this.ForeColor);
			base.OnForeColorChanged(e);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			base.Close();
		}

		public void SetItems(ICollection<string> items)
		{
			this.m_Items = new List<string>(items);
			base.Size = new Size(base.Width, items.Count * this.ItemHeight);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.Clear(this.BackColor);
			e.Graphics.FillRectangle(this.m_SelectBarBrush, 0, this.m_SelectedIndex * this.ItemHeight, base.Width, this.ItemHeight);
			if (this.m_HighlightIndex != -1 && this.m_HighlightIndex != this.m_SelectedIndex)
			{
				e.Graphics.FillRectangle(this.m_HighlightBarBrush, 0, this.m_HighlightIndex * this.ItemHeight, base.Width, this.ItemHeight);
			}
			int y = 0;
			foreach (string item in this.m_Items)
			{
				e.Graphics.DrawString(item, this.Font, this.m_FontBrush, 0f, (float)(y + 1));
				y += this.ItemHeight;
			}
			base.OnPaint(e);
		}

		public void SetSelectBarColour(Color colour)
		{
			this.m_SelectBarBrush = new SolidBrush(colour);
			this.m_HighlightBarBrush = new SolidBrush(Utils.Lerp(this.BackColor, colour, 0.1f));
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
            /// mariotodo ÀúÊ·¼ÇÂ¼¿ò
            base.OnKeyDown(e);
            //Keys keyCode = e.KeyCode;
            //if (keyCode <= Keys.Return)
            //{
            //    if (keyCode != Keys.Back)
            //    {
            //        if (keyCode != Keys.Return)
            //        {
            //            goto IL_7E;
            //        }
            //        base.Close();
            //        goto IL_7E;
            //    }
            //}
            //else if (keyCode != Keys.Up)
            //{
            //    if (keyCode == Keys.Down)
            //    {
            //        this.SetSelectedIndex(Math.Min(this.m_SelectedIndex + 1, this.m_Items.Count - 1));
            //        goto IL_7E;
            //    }
            //    if (keyCode != Keys.Delete)
            //    {
            //        goto IL_7E;
            //    }
            //}
            //else
            //{
            //    this.SetSelectedIndex(this.m_SelectedIndex - 1);
            //    if (this.m_SelectedIndex == -1)
            //    {
            //        base.Close();
            //        goto IL_7E;
            //    }
            //    goto IL_7E;
            //}
            //this.SetSelectedIndex(-1);
            //base.Close();
            //IL_7E:
            //base.OnKeyDown(e);
		}

		private void SetSelectedIndex(int index)
		{
			if (index != this.m_SelectedIndex)
			{
				this.m_SelectedIndex = index;
				if (this.Selectionchanged != null)
				{
					this.Selectionchanged();
				}
				this.Refresh();
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			this.m_HighlightIndex = -1;
			base.OnMouseLeave(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (this.m_Items.Count != 0)
			{
				int new_selected_index = Math.Min(this.m_Items.Count - 1, e.Y / this.ItemHeight);
				this.SetSelectedIndex(new_selected_index);
			}
			base.Close();
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			int highlight_index = e.Y / this.ItemHeight;
			if (highlight_index != this.m_HighlightIndex)
			{
				this.m_HighlightIndex = highlight_index;
				this.Refresh();
			}
			base.OnMouseMove(e);
		}
	}
}
