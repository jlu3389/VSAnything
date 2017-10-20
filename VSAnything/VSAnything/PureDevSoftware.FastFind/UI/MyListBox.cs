using SCLCoreCLR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Company.VSAnything
{
	internal class MyListBox : UserControl
	{
		public delegate void SelectedIndexchangedHandler();

		public delegate void ItemDoubleClickedHandler();

		private int m_ScrollIndex;

		private int m_SelectedIndexStart = -1;

		private int m_SelectedIndexEnd = -1;

		private int m_HighlightIndex = -1;

		private const int m_ScrollBarWidth = 12;

		private List<object> m_Items = new List<object>();

		private Brush m_SelectBarBrush;

		private Brush m_HighlightTextBrush;

		private Brush m_FilenameBrush;

		private Brush m_FindTextBrush;

		private Brush m_SelectedHighlightTextBrush;

		private Brush m_HighlightBrush;

		private Brush m_BackgroundBrush;

		private Brush m_FileBrush;

		private Brush m_HighlightBarBrush;

		private Brush m_ScrollBarBrush;

		private Brush m_ScrollBarBackgroundBrush;

		private Brush m_InfoItemBrush;

		private StringFormat m_StringFormat = new StringFormat();

		private int m_Updating;

		private bool m_DraggingScrollBar;

		private int m_StartDragScrollIndex;

		private int m_StartScrollDragY;

		private bool m_ScrollBarPaging;

		private const int m_ScrollBarPagingTimerInterval = 50;

		private System.Windows.Forms.Timer m_ScrollBarPagingTimer = new System.Windows.Forms.Timer();

		private Settings m_Settings;

		private bool m_OnlyShowingMaxMatches;

		private bool m_ShowGettingSolutionFiles;

		private bool m_ShowScanningFiles;

		private bool m_ShowFindingTextMessage;

		private bool m_ShowNoItemsMessage;

		private int m_FindingTextPercent;

		private int m_ScanningFilesPercent;

		private const string m_NoItemsMessage = "No matches";

		private const string m_GettingSolutionFilesMessage = "Indexing solution files...   (this can take some time but will only happen once)";

		private const string m_ScanningFilesMessage = "Scanning solution files...";

		private const string m_FindingTextMessage = "Searching in files...";

		private bool m_HorzScrollling;

		private int m_HorzScrollMouseX;

		private int m_HorzScroll;

		private int m_MaxItemWidth;

		private int m_MaxMatchCount;

		private const int m_MaxDrawStringLength = 10000;

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event MyListBox.SelectedIndexchangedHandler SelectedIndexchanged;

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event MyListBox.ItemDoubleClickedHandler ItemDoubleClicked;

		public int MaxMatchCount
		{
			get
			{
				return this.m_MaxMatchCount;
			}
			set
			{
				this.m_MaxMatchCount = value;
			}
		}

		private bool ScrollBarVisible
		{
			get
			{
				return this.ScrollBarHeight < base.ClientSize.Height;
			}
		}

		private int ScrollBarY
		{
			get
			{
				return this.m_ScrollIndex * base.ClientSize.Height / this.m_Items.Count;
			}
		}

		private int ScrollBarHeight
		{
			get
			{
				if (this.m_Items.Count == 0)
				{
					return base.ClientSize.Height;
				}
				return Math.Max(4, base.ClientSize.Height * this.VisibleItemCount / this.m_Items.Count);
			}
		}

		public List<object> Items
		{
			get
			{
				return this.m_Items;
			}
		}

		public int ItemCount
		{
			get
			{
				return this.m_Items.Count;
			}
		}

		private int VisibleItemCount
		{
			get
			{
				return Math.Min(base.ClientSize.Height / this.ItemHeight, this.m_Items.Count);
			}
		}

		public int MaxVisibleItemCount
		{
			get
			{
				return (base.ClientSize.Height + this.ItemHeight - 1) / this.ItemHeight;
			}
		}

		public int SelectableItemCount
		{
			get
			{
				int count = this.m_Items.Count;
				if (this.m_Items.Count > 0 && (this.m_Items.Count < this.VisibleItemCount || this.m_Items.Count == 1) && this.m_Items[this.m_Items.Count - 1] is InfoListBoxItem)
				{
					count--;
				}
				return count;
			}
		}

		public int SelectedIndexStart
		{
			get
			{
				return this.m_SelectedIndexStart;
			}
		}

		public int SelectedIndex
		{
			get
			{
				if (this.m_SelectedIndexStart >= this.m_Items.Count)
				{
					this.m_SelectedIndexStart = -1;
				}
				return this.m_SelectedIndexStart;
			}
			set
			{
				if (this.m_SelectedIndexStart != value)
				{
					this.BeginUpdate();
					this.m_SelectedIndexStart = value;
					this.m_SelectedIndexEnd = value;
					if (this.ScrollIndex + this.VisibleItemCount - 1 < this.m_SelectedIndexStart)
					{
						this.ScrollIndex = Misc.Clamp(this.m_SelectedIndexStart - (this.VisibleItemCount - 1), 0, this.m_Items.Count - 1);
					}
					else if (this.ScrollIndex > this.m_SelectedIndexStart)
					{
						this.ScrollIndex = this.m_SelectedIndexStart;
					}
					if (this.SelectedIndexchanged != null)
					{
						this.SelectedIndexchanged();
					}
					this.EndUpdate();
				}
			}
		}

		public int SelectedIndexEnd
		{
			get
			{
				return this.m_SelectedIndexEnd;
			}
			set
			{
				if (this.m_SelectedIndexEnd != value)
				{
					this.BeginUpdate();
					this.m_SelectedIndexEnd = value;
					if (this.ScrollIndex + this.VisibleItemCount - 1 < this.m_SelectedIndexEnd)
					{
						this.ScrollIndex = Misc.Clamp(this.m_SelectedIndexEnd - (this.VisibleItemCount - 1), 0, this.m_Items.Count - 1);
					}
					else if (this.ScrollIndex > this.m_SelectedIndexEnd)
					{
						this.ScrollIndex = this.m_SelectedIndexEnd;
					}
					if (this.SelectedIndexchanged != null)
					{
						this.SelectedIndexchanged();
					}
					this.EndUpdate();
				}
			}
		}

		public int ScrollIndex
		{
			get
			{
				return this.m_ScrollIndex;
			}
			set
			{
				int max_clamp = Math.Max(0, this.m_Items.Count - this.VisibleItemCount);
				int clamped_index = Misc.Clamp(value, 0, max_clamp);
				if (this.m_ScrollIndex != clamped_index)
				{
					this.BeginUpdate();
					this.m_ScrollIndex = clamped_index;
					this.EndUpdate();
				}
			}
		}

		public object SelectedItem
		{
			get
			{
				if (this.m_SelectedIndexStart < 0 || this.m_SelectedIndexStart >= this.m_Items.Count)
				{
					return null;
				}
				return this.m_Items[this.m_SelectedIndexStart];
			}
			set
			{
				this.SelectedIndex = this.m_Items.IndexOf(value);
			}
		}

		public int ItemHeight
		{
			get
			{
				return this.Font.Height;
			}
		}

		public Settings Settings
		{
			get
			{
				return this.m_Settings;
			}
			set
			{
				this.m_Settings = value;
			}
		}

		public bool OnlyShowingMaxMatches
		{
			get
			{
				return this.m_OnlyShowingMaxMatches;
			}
			set
			{
				this.m_OnlyShowingMaxMatches = value;
			}
		}

		public bool ShowGettingSolutionFiles
		{
			get
			{
				return this.m_ShowGettingSolutionFiles;
			}
			set
			{
				this.m_ShowGettingSolutionFiles = value;
			}
		}

		public bool ShowScanningFiles
		{
			get
			{
				return this.m_ShowScanningFiles;
			}
			set
			{
				this.m_ShowScanningFiles = value;
			}
		}

		public bool ShowFindingTextMessage
		{
			get
			{
				return this.m_ShowFindingTextMessage;
			}
			set
			{
				this.m_ShowFindingTextMessage = value;
			}
		}

		public bool ShowNoItemsMessage
		{
			get
			{
				return this.m_ShowNoItemsMessage;
			}
			set
			{
				this.m_ShowNoItemsMessage = value;
			}
		}

		public int FindingTextPercent
		{
			get
			{
				return this.m_FindingTextPercent;
			}
			set
			{
				this.m_FindingTextPercent = value;
			}
		}

		public int ScanningFilesPercent
		{
			get
			{
				return this.m_ScanningFilesPercent;
			}
			set
			{
				this.m_ScanningFilesPercent = value;
			}
		}

		public MyListBox()
		{
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.SetStyle(ControlStyles.UserPaint, true);
			this.m_ScrollBarPagingTimer.Interval = 50;
			this.m_ScrollBarPagingTimer.Tick += new EventHandler(this.ScrollBarPagingTimerTick);
		}

		protected override void Dispose(bool disposing)
		{
			this.m_ScrollBarPagingTimer.Stop();
			this.m_ScrollBarPagingTimer.Tick -= new EventHandler(this.ScrollBarPagingTimerTick);
			if (this.m_SelectBarBrush != null)
			{
				this.m_SelectBarBrush.Dispose();
				this.m_SelectBarBrush = null;
			}
			if (this.m_HighlightTextBrush != null)
			{
				this.m_HighlightTextBrush.Dispose();
				this.m_HighlightTextBrush = null;
			}
			if (this.m_FilenameBrush != null)
			{
				this.m_FilenameBrush.Dispose();
				this.m_FilenameBrush = null;
			}
			if (this.m_FindTextBrush != null)
			{
				this.m_FindTextBrush.Dispose();
				this.m_FindTextBrush = null;
			}
			if (this.m_SelectedHighlightTextBrush != null)
			{
				this.m_SelectedHighlightTextBrush.Dispose();
				this.m_SelectedHighlightTextBrush = null;
			}
			if (this.m_HighlightBrush != null)
			{
				this.m_HighlightBrush.Dispose();
				this.m_HighlightBrush = null;
			}
			if (this.m_BackgroundBrush != null)
			{
				this.m_BackgroundBrush.Dispose();
				this.m_BackgroundBrush = null;
			}
			if (this.m_FileBrush != null)
			{
				this.m_FileBrush.Dispose();
				this.m_FileBrush = null;
			}
			if (this.m_HighlightBarBrush != null)
			{
				this.m_HighlightBarBrush.Dispose();
				this.m_HighlightBarBrush = null;
			}
			if (this.m_ScrollBarBrush != null)
			{
				this.m_ScrollBarBrush.Dispose();
				this.m_ScrollBarBrush = null;
			}
			if (this.m_ScrollBarBackgroundBrush != null)
			{
				this.m_ScrollBarBackgroundBrush.Dispose();
				this.m_ScrollBarBackgroundBrush = null;
			}
			if (this.m_InfoItemBrush != null)
			{
				this.m_InfoItemBrush.Dispose();
				this.m_InfoItemBrush = null;
			}
			if (this.m_StringFormat != null)
			{
				this.m_StringFormat.Dispose();
				this.m_StringFormat = null;
			}
			base.Dispose(disposing);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
		}

		private static void DrawString(Graphics graphics, string value, Font font, Brush brush, float x, float y, StringFormat format)
		{
			if (value.Length > 10000)
			{
				value = value.Substring(0, 10000);
			}
			graphics.DrawString(value, font, brush, x, y, format);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				e.Graphics.Clear(this.BackColor);
				int visible_item_count = Math.Min((base.ClientSize.Height + base.ClientSize.Height - 1) / this.ItemHeight, this.m_Items.Count - this.m_ScrollIndex);
				int select_index_start = Math.Min(this.m_SelectedIndexStart, this.m_SelectedIndexEnd) - this.m_ScrollIndex;
				int select_index_end = Math.Max(this.m_SelectedIndexStart, this.m_SelectedIndexEnd) - this.m_ScrollIndex;
				if (this.m_SelectedIndexStart != -1)
				{
					for (int select_index = select_index_start; select_index <= select_index_end; select_index++)
					{
						if (select_index >= 0 && select_index < visible_item_count)
						{
							e.Graphics.FillRectangle(this.m_SelectBarBrush, 0, select_index * this.ItemHeight, base.ClientSize.Width, this.ItemHeight);
						}
					}
				}
				int highlight_index = this.m_HighlightIndex - this.m_ScrollIndex;
				if (highlight_index >= 0 && highlight_index < visible_item_count && (highlight_index < select_index_start || highlight_index > select_index_end))
				{
					e.Graphics.FillRectangle(this.m_HighlightBarBrush, 0, highlight_index * this.ItemHeight, base.ClientSize.Width, this.ItemHeight);
				}
				int y = 0;
				this.m_MaxItemWidth = 0;
				int i = 0;
				while (i < visible_item_count)
				{
					object item = this.m_Items[i + this.m_ScrollIndex];
					bool selected = i >= select_index_start && i <= select_index_end;
					int x = -this.m_HorzScroll;
					int line_width = 0;
					if (item is FindTextResult)
					{
						FindTextResult find_text_result = (FindTextResult)item;
						if (find_text_result != null)
						{
							string filename = (VSAnythingPackage.Inst.GetSettingsDialogPage().FindTextPathMode == PathMode.Filename) ? find_text_result.m_Filename : find_text_result.m_Path;
							int display_line = find_text_result.m_LineIndex + 1;
							filename = string.Concat(new object[]
							{
								filename,
								"(",
								display_line,
								"): "
							});
							int start = find_text_result.m_StartIndex;
							int end = find_text_result.m_EndIndex;
							string line = find_text_result.m_Line;
							string str = line.Substring(0, start);
							string str2 = line.Substring(start, end - start);
							string str3 = line.Substring(end, line.Length - end);
							str = str.Replace("\t", "    ");
							str2 = str2.Replace("\t", "    ");
							str3 = str3.Replace("\t", "    ");
							if (str.Length > 1024)
							{
								str = "..." + str.Substring(str.Length - 50);
							}
							if (str3.Length > 1024)
							{
								str3 = str3.Substring(0, 50) + "...";
							}
							MyListBox.DrawString(e.Graphics, filename, this.Font, selected ? this.m_HighlightTextBrush : this.m_FilenameBrush, (float)x, (float)y, this.m_StringFormat);
							int filename_width = this.GetStringWidth(filename, filename.Length, e.Graphics, this.Font);
							x += filename_width;
							line_width += filename_width;
							int str1_width = this.GetStringWidth(str, str.Length, e.Graphics, this.Font);
							if (str1_width != -1)
							{
								MyListBox.DrawString(e.Graphics, str, this.Font, selected ? this.m_HighlightTextBrush : this.m_FindTextBrush, (float)x, (float)y, this.m_StringFormat);
								x += str1_width;
								line_width += str1_width;
							}
							int str2_width = this.GetStringWidth(str2, str2.Length, e.Graphics, this.Font);
							if (str2_width != -1)
							{
								MyListBox.DrawString(e.Graphics, str2, this.Font, selected ? this.m_SelectedHighlightTextBrush : this.m_HighlightBrush, (float)x, (float)y, this.m_StringFormat);
								x += str2_width;
								line_width += str2_width;
							}
							int str3_width = this.GetStringWidth(str3, str3.Length, e.Graphics, this.Font);
							if (str3_width != -1)
							{
								MyListBox.DrawString(e.Graphics, str3, this.Font, selected ? this.m_HighlightTextBrush : this.m_FindTextBrush, (float)x, (float)y, this.m_StringFormat);
								line_width += str3_width;
							}
						}
					}
					else if (item is FilteredFileItem)
					{
						FilteredFileItem filtered_file_item = (FilteredFileItem)item;
						MyListBox.DrawString(e.Graphics, filtered_file_item.m_DisplayFilename, this.Font, selected ? this.m_HighlightTextBrush : this.m_FileBrush, (float)x, (float)y, this.m_StringFormat);
						line_width += this.GetStringWidth(filtered_file_item.m_DisplayFilename, filtered_file_item.m_DisplayFilename.Length, e.Graphics, this.Font);
					}
					else if (item is InfoListBoxItem)
					{
						InfoListBoxItem info_item = (InfoListBoxItem)item;
						MyListBox.DrawString(e.Graphics, info_item.Message, this.Font, this.m_InfoItemBrush, (float)x, (float)y, this.m_StringFormat);
					}
					else
					{
						string item_str = (string)item;
						MyListBox.DrawString(e.Graphics, item_str, this.Font, selected ? this.m_HighlightTextBrush : this.m_FileBrush, (float)x, (float)y, this.m_StringFormat);
						line_width += this.GetStringWidth(item_str, item_str.Length, e.Graphics, this.Font);
					}
					this.m_MaxItemWidth = Math.Max(this.m_MaxItemWidth, line_width);
					i++;
					y += this.ItemHeight;
				}
				this.DrawScrollBar(e.Graphics);
			}
			catch (Exception arg_57D_0)
			{
				Utils.LogException(arg_57D_0);
			}
			base.OnPaint(e);
		}

		public void UpdateInfoItem()
		{
			if (this.m_OnlyShowingMaxMatches)
			{
				this.SetInfoItem("Only showing first " + this.m_MaxMatchCount + " matches");
				return;
			}
			if (this.m_ShowGettingSolutionFiles)
			{
				this.SetInfoItem("Indexing solution files...   (this can take some time but will only happen once)", this.m_FileBrush);
				return;
			}
			if (this.m_ShowScanningFiles)
			{
				this.SetInfoItem("Scanning solution files...(" + this.m_ScanningFilesPercent + "%)");
				return;
			}
			if (this.m_ShowFindingTextMessage)
			{
				this.SetInfoItem("Searching in files...(" + this.m_FindingTextPercent + "%)");
				return;
			}
			if (this.m_ShowNoItemsMessage && this.SelectableItemCount == 0)
			{
				this.SetInfoItem("No matches");
				return;
			}
			this.SetInfoItem(null);
		}

		private void SetInfoItem(string message)
		{
			this.SetInfoItem(message, this.m_InfoItemBrush);
		}

		private void SetInfoItem(string message, Brush brush)
		{
			object last_item = (this.m_Items.Count != 0) ? this.m_Items[this.m_Items.Count - 1] : null;
			if (message != null)
			{
				if (!(last_item is InfoListBoxItem))
				{
					this.m_Items.Add(new InfoListBoxItem(message, brush));
					this.RefreshIfNotUpdating();
					return;
				}
				InfoListBoxItem info_item = (InfoListBoxItem)last_item;
				if (info_item.Message != message)
				{
					info_item.Message = message;
					info_item.Brush = brush;
					this.RefreshIfNotUpdating();
					return;
				}
			}
			else if (last_item is InfoListBoxItem)
			{
				this.m_Items.RemoveAt(this.m_Items.Count - 1);
				this.RefreshIfNotUpdating();
			}
		}

		private void DrawScrollBar(Graphics graphics)
		{
			if (this.ScrollBarVisible)
			{
				int x = base.ClientSize.Width - 12;
				graphics.FillRectangle(this.m_ScrollBarBackgroundBrush, x, 0, 12, base.ClientSize.Height);
				graphics.FillRectangle(this.m_ScrollBarBrush, x, this.ScrollBarY, 12, this.ScrollBarHeight);
			}
		}

		private int GetStringWidth(string str, int len, Graphics graphics, Font font)
		{
			return Utils.GetStringWidth(str, len, graphics, font, this.m_StringFormat);
		}

		public void Clear()
		{
			this.m_Items.Clear();
			this.m_SelectedIndexStart = -1;
			this.m_SelectedIndexEnd = -1;
			this.m_HighlightIndex = -1;
			this.m_ScrollIndex = 0;
			this.RefreshIfNotUpdating();
		}

		public object GetItem(int index)
		{
			return this.m_Items[index];
		}

		public void AddItem(object item)
		{
			this.m_Items.Add(item);
			this.RefreshIfNotUpdating();
		}

		private void RefreshIfNotUpdating()
		{
			if (this.m_Updating == 0)
			{
				this.Refresh();
			}
		}

		private void ScrollBarPagingTimerTick(object sender, EventArgs e)
		{
			this.ScrollPageClick();
		}

		private void ScrollPageClick()
		{
			int y = base.PointToClient(Cursor.Position).Y;
			if (y < this.ScrollBarY)
			{
				this.ScrollIndex = this.m_ScrollIndex - this.VisibleItemCount;
				return;
			}
			if (y >= this.ScrollBarY + this.ScrollBarHeight)
			{
				this.ScrollIndex = this.m_ScrollIndex + this.VisibleItemCount;
			}
		}

		private void StopScrollDragTimer()
		{
			this.m_ScrollBarPagingTimer.Stop();
			this.m_ScrollBarPaging = false;
			base.Capture = false;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			int scroll_bar_x = base.ClientSize.Width - 12;
			if (this.ScrollBarVisible && e.X >= scroll_bar_x)
			{
				if (e.Y >= this.ScrollBarY && e.Y < this.ScrollBarY + this.ScrollBarHeight)
				{
					base.Capture = true;
					this.m_DraggingScrollBar = true;
					this.m_StartDragScrollIndex = this.m_ScrollIndex;
					this.m_StartScrollDragY = e.Y;
				}
				else
				{
					base.Capture = true;
					this.ScrollPageClick();
					this.m_ScrollBarPaging = true;
					this.m_ScrollBarPagingTimer.Start();
				}
			}
			if (!this.ScrollBarVisible || e.X < scroll_bar_x)
			{
				int sel_index = e.Y / this.ItemHeight + this.m_ScrollIndex;
				if (sel_index != this.m_SelectedIndexStart && sel_index >= 0 && sel_index < this.SelectableItemCount)
				{
					this.SelectedIndex = sel_index;
				}
				this.m_HorzScrollling = true;
				this.m_HorzScrollMouseX = e.X;
				base.Capture = true;
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (this.m_DraggingScrollBar)
			{
				int offset = e.Y - this.m_StartScrollDragY;
				this.ScrollIndex = this.m_StartDragScrollIndex + offset * this.m_Items.Count / base.ClientSize.Height;
			}
			else if (this.m_ScrollBarPaging)
			{
				Rectangle scroll_rect = new Rectangle(base.ClientSize.Width - 12, 0, 12, base.ClientSize.Height);
				if (!scroll_rect.Contains(e.Location))
				{
					this.StopScrollDragTimer();
				}
			}
			else if (this.m_HorzScrollling)
			{
				int offset2 = this.m_HorzScrollMouseX - e.X;
				this.m_HorzScrollMouseX = e.X;
				if (offset2 != 0)
				{
					this.m_HorzScroll += offset2;
					int client_width = base.ClientSize.Width;
					if (this.ScrollBarVisible)
					{
						client_width -= 12;
					}
					int max_scroll = Math.Max(0, this.m_MaxItemWidth + 10 - client_width);
					this.m_HorzScroll = Misc.Clamp(this.m_HorzScroll, 0, max_scroll);
					this.Refresh();
				}
			}
			else
			{
				int new_highlight_index = e.Y / this.ItemHeight + this.m_ScrollIndex;
				if (new_highlight_index != this.m_HighlightIndex)
				{
					this.m_HighlightIndex = new_highlight_index;
					this.Refresh();
				}
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			if ((!this.ScrollBarVisible || e.X < base.ClientSize.Width - 12) && this.ItemDoubleClicked != null)
			{
				this.ItemDoubleClicked();
			}
			base.OnMouseDoubleClick(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (this.m_DraggingScrollBar)
			{
				base.Capture = false;
				this.m_DraggingScrollBar = false;
			}
			else if (this.m_ScrollBarPaging)
			{
				this.StopScrollDragTimer();
			}
			else if (this.m_HorzScrollling)
			{
				base.Capture = false;
				this.m_HorzScrollling = false;
			}
			base.OnMouseUp(e);
		}

		public void SetupBrushes(Settings settings)
		{
			AppearanceDialogPage appearance_settings = VSAnythingPackage.Inst.GetAppearanceDialogPage();
			this.m_BackgroundBrush = new SolidBrush(appearance_settings.Colours.m_BackColour);
			this.m_SelectBarBrush = new SolidBrush(appearance_settings.Colours.m_SelectColour);
			this.m_FileBrush = new SolidBrush(appearance_settings.Colours.m_ForeColour);
			this.m_FindTextBrush = new SolidBrush(appearance_settings.Colours.m_CodeColour);
			this.m_HighlightBrush = new SolidBrush(appearance_settings.Colours.m_HighlightColour);
			this.m_HighlightTextBrush = new SolidBrush(appearance_settings.Colours.m_HighlightTextColour);
			this.m_SelectedHighlightTextBrush = new SolidBrush(appearance_settings.Colours.m_SelectedHighlightTextColour);
			this.m_FilenameBrush = new SolidBrush(appearance_settings.Colours.m_CodeFilenameColour);
			Color sel_colour = appearance_settings.Colours.m_SelectColour;
			Color highlight_bar_colour = Utils.Lerp(appearance_settings.Colours.m_BackColour, sel_colour, 0.1f);
			this.m_HighlightBarBrush = new SolidBrush(highlight_bar_colour);
			Color scroll_bar_colour = Utils.ModifyColour(appearance_settings.Colours.m_ControlColour, 50);
			this.m_ScrollBarBrush = new SolidBrush(scroll_bar_colour);
			this.m_ScrollBarBackgroundBrush = new SolidBrush(Utils.Lerp(appearance_settings.Colours.m_BackColour, scroll_bar_colour, 0.3f));
			this.m_InfoItemBrush = new SolidBrush(Utils.Lerp(appearance_settings.Colours.m_BackColour, appearance_settings.Colours.m_CodeFilenameColour, 0.3f));
		}

		public void BeginUpdate()
		{
			this.m_Updating++;
		}

		public void EndUpdate()
		{
			this.UpdateInfoItem();
			this.m_Updating--;
			this.RefreshIfNotUpdating();
		}
	}
}
