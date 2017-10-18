using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Company.VSAnything
{
	internal class AppearanceControl : UserControl
	{
		private AppearanceDialogPage m_AppearanceDialogPage;

		private IContainer components;

		private Label label2;

		private Button m_ControlColourButton;

		private Button m_FontButton;

		private Button m_CodeFilenameColourButton;

		private Label label1;

		private Label label8;

		private Label label11;

		private Label label7;

		private Button m_BackColourButton;

		private Button m_CodeColourButton;

		private Button m_SelectedHighlightTextColourButton;

		private ComboBox m_ColourThemeComboBox;

		private Label label3;

		private Label label6;

		private Label label10;

		private Label label9;

		private Button m_ForeColourButton;

		private Button m_HighlightColourButton;

		private Button m_HighlightTextColourButton;

		private Label label5;

		private Label label4;

		private Button m_SelectColourButton;

		private ColorDialog m_ColourDialog;

		private FontDialog m_FontDialog;

		public AppearanceControl(AppearanceDialogPage page)
		{
			this.InitializeComponent();
			this.m_AppearanceDialogPage = page;
			this.UpdateButtonColours();
			this.m_ColourThemeComboBox.Items.Clear();
			this.m_ColourThemeComboBox.Items.AddRange(Enum.GetNames(typeof(ColourTheme)));
			this.UpdateColourThemeComboBox();
			this.UpdateFontButton();
		}

		private void UpdateColourThemeComboBox()
		{
			this.m_ColourThemeComboBox.Text = this.m_AppearanceDialogPage.ColourTheme.ToString();
		}

		private void UpdateButtonColours()
		{
			ColourSettings colour_settings = this.m_AppearanceDialogPage.Colours;
			this.m_BackColourButton.BackColor = colour_settings.m_BackColour;
			this.m_ForeColourButton.BackColor = colour_settings.m_ForeColour;
			this.m_ControlColourButton.BackColor = colour_settings.m_ControlColour;
			this.m_SelectColourButton.BackColor = colour_settings.m_SelectColour;
			this.m_HighlightColourButton.BackColor = colour_settings.m_HighlightColour;
			this.m_HighlightTextColourButton.BackColor = colour_settings.m_HighlightTextColour;
			this.m_SelectedHighlightTextColourButton.BackColor = colour_settings.m_SelectedHighlightTextColour;
			this.m_CodeColourButton.BackColor = colour_settings.m_CodeColour;
			this.m_CodeFilenameColourButton.BackColor = colour_settings.m_CodeFilenameColour;
		}

		private void UpdateColoursFromButtons()
		{
			if (this.m_AppearanceDialogPage.ColourTheme == ColourTheme.Custom)
			{
				this.m_AppearanceDialogPage.BackColour = this.m_BackColourButton.BackColor;
				this.m_AppearanceDialogPage.ForeColour = this.m_ForeColourButton.BackColor;
				this.m_AppearanceDialogPage.ControlColour = this.m_ControlColourButton.BackColor;
				this.m_AppearanceDialogPage.SelectColour = this.m_SelectColourButton.BackColor;
				this.m_AppearanceDialogPage.HighlightColour = this.m_HighlightColourButton.BackColor;
				this.m_AppearanceDialogPage.HighlightTextColour = this.m_HighlightTextColourButton.BackColor;
				this.m_AppearanceDialogPage.SelectedHighlightTextColour = this.m_SelectedHighlightTextColourButton.BackColor;
				this.m_AppearanceDialogPage.CodeColour = this.m_CodeColourButton.BackColor;
				this.m_AppearanceDialogPage.CodeFilenameColour = this.m_CodeFilenameColourButton.BackColor;
			}
		}

		private void ColourButtonClicked(object sender, EventArgs e)
		{
			Button button = (Button)sender;
			if (this.m_AppearanceDialogPage.ColourTheme != ColourTheme.Custom)
			{
				if (MessageBox.Show("Switch to custom colour theme using these colours?", "Colour Theme", MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
				ColourSettings colour_settings = this.m_AppearanceDialogPage.Colours;
				this.m_AppearanceDialogPage.BackColour = colour_settings.m_BackColour;
				this.m_AppearanceDialogPage.ForeColour = colour_settings.m_ForeColour;
				this.m_AppearanceDialogPage.ControlColour = colour_settings.m_ControlColour;
				this.m_AppearanceDialogPage.SelectColour = colour_settings.m_SelectColour;
				this.m_AppearanceDialogPage.HighlightColour = colour_settings.m_HighlightColour;
				this.m_AppearanceDialogPage.HighlightTextColour = colour_settings.m_HighlightTextColour;
				this.m_AppearanceDialogPage.SelectedHighlightTextColour = colour_settings.m_SelectedHighlightTextColour;
				this.m_AppearanceDialogPage.CodeColour = colour_settings.m_CodeColour;
				this.m_AppearanceDialogPage.CodeFilenameColour = colour_settings.m_CodeFilenameColour;
				this.m_AppearanceDialogPage.ColourTheme = ColourTheme.Custom;
				this.UpdateColourThemeComboBox();
			}
			this.m_ColourDialog.Color = button.BackColor;
			this.m_ColourDialog.ShowDialog(this);
			button.BackColor = this.m_ColourDialog.Color;
			this.UpdateColoursFromButtons();
		}

		private void FontButtonClicked(object sender, EventArgs e)
		{
			this.m_FontDialog.Font = new Font(this.m_AppearanceDialogPage.FontName, this.m_AppearanceDialogPage.FontSize);
			this.m_FontDialog.ShowDialog(this);
			this.m_AppearanceDialogPage.FontName = this.m_FontDialog.Font.Name;
			this.m_AppearanceDialogPage.FontSize = this.m_FontDialog.Font.SizeInPoints;
			this.UpdateFontButton();
		}

		private void UpdateFontButton()
		{
			this.m_FontButton.Text = this.m_AppearanceDialogPage.FontName + " size: " + this.m_AppearanceDialogPage.FontSize;
		}

		private void ColourThemeChanged(object sender, EventArgs e)
		{
			try
			{
				this.m_AppearanceDialogPage.ColourTheme = (ColourTheme)Enum.Parse(typeof(ColourTheme), this.m_ColourThemeComboBox.Text);
				this.UpdateButtonColours();
			}
			catch (Exception)
			{
			}
		}

		public void SubmitData()
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.label2 = new Label();
			this.m_ControlColourButton = new Button();
			this.m_FontButton = new Button();
			this.m_CodeFilenameColourButton = new Button();
			this.label1 = new Label();
			this.label8 = new Label();
			this.label11 = new Label();
			this.label7 = new Label();
			this.m_BackColourButton = new Button();
			this.m_CodeColourButton = new Button();
			this.m_SelectedHighlightTextColourButton = new Button();
			this.m_ColourThemeComboBox = new ComboBox();
			this.label3 = new Label();
			this.label6 = new Label();
			this.label10 = new Label();
			this.label9 = new Label();
			this.m_ForeColourButton = new Button();
			this.m_HighlightColourButton = new Button();
			this.m_HighlightTextColourButton = new Button();
			this.label5 = new Label();
			this.label4 = new Label();
			this.m_SelectColourButton = new Button();
			this.m_ColourDialog = new ColorDialog();
			this.m_FontDialog = new FontDialog();
			base.SuspendLayout();
			this.label2.AutoSize = true;
			this.label2.Location = new Point(241, 197);
			this.label2.Margin = new Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new Size(40, 13);
			this.label2.TabIndex = 49;
			this.label2.Text = "Control";
			this.m_ControlColourButton.FlatStyle = FlatStyle.Flat;
			this.m_ControlColourButton.Location = new Point(285, 194);
			this.m_ControlColourButton.Margin = new Padding(2);
			this.m_ControlColourButton.Name = "m_ControlColourButton";
			this.m_ControlColourButton.Size = new Size(34, 19);
			this.m_ControlColourButton.TabIndex = 48;
			this.m_ControlColourButton.UseVisualStyleBackColor = true;
			this.m_ControlColourButton.Click += new EventHandler(this.ColourButtonClicked);
			this.m_FontButton.Location = new Point(156, 20);
			this.m_FontButton.Margin = new Padding(2);
			this.m_FontButton.Name = "m_FontButton";
			this.m_FontButton.Size = new Size(157, 19);
			this.m_FontButton.TabIndex = 28;
			this.m_FontButton.Text = "button1";
			this.m_FontButton.TextAlign = ContentAlignment.MiddleLeft;
			this.m_FontButton.UseVisualStyleBackColor = true;
			this.m_FontButton.Click += new EventHandler(this.FontButtonClicked);
			this.m_CodeFilenameColourButton.FlatStyle = FlatStyle.Flat;
			this.m_CodeFilenameColourButton.Location = new Point(285, 170);
			this.m_CodeFilenameColourButton.Margin = new Padding(2);
			this.m_CodeFilenameColourButton.Name = "m_CodeFilenameColourButton";
			this.m_CodeFilenameColourButton.Size = new Size(34, 19);
			this.m_CodeFilenameColourButton.TabIndex = 39;
			this.m_CodeFilenameColourButton.UseVisualStyleBackColor = true;
			this.m_CodeFilenameColourButton.Click += new EventHandler(this.ColourButtonClicked);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(124, 23);
			this.label1.Margin = new Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new Size(28, 13);
			this.label1.TabIndex = 27;
			this.label1.Text = "Font";
			this.label8.AutoSize = true;
			this.label8.Location = new Point(249, 149);
			this.label8.Margin = new Padding(2, 0, 2, 0);
			this.label8.Name = "label8";
			this.label8.Size = new Size(32, 13);
			this.label8.TabIndex = 38;
			this.label8.Text = "Code";
			this.label11.AutoSize = true;
			this.label11.Location = new Point(196, 126);
			this.label11.Margin = new Padding(2, 0, 2, 0);
			this.label11.Name = "label11";
			this.label11.Size = new Size(87, 13);
			this.label11.TabIndex = 47;
			this.label11.Text = "Highlight and Sel";
			this.label7.AutoSize = true;
			this.label7.Location = new Point(232, 173);
			this.label7.Margin = new Padding(2, 0, 2, 0);
			this.label7.Name = "label7";
			this.label7.Size = new Size(49, 13);
			this.label7.TabIndex = 40;
			this.label7.Text = "Filename";
			this.m_BackColourButton.FlatStyle = FlatStyle.Flat;
			this.m_BackColourButton.Location = new Point(156, 99);
			this.m_BackColourButton.Margin = new Padding(2);
			this.m_BackColourButton.Name = "m_BackColourButton";
			this.m_BackColourButton.Size = new Size(34, 19);
			this.m_BackColourButton.TabIndex = 29;
			this.m_BackColourButton.UseVisualStyleBackColor = true;
			this.m_BackColourButton.Click += new EventHandler(this.ColourButtonClicked);
			this.m_CodeColourButton.FlatStyle = FlatStyle.Flat;
			this.m_CodeColourButton.Location = new Point(285, 146);
			this.m_CodeColourButton.Margin = new Padding(2);
			this.m_CodeColourButton.Name = "m_CodeColourButton";
			this.m_CodeColourButton.Size = new Size(34, 19);
			this.m_CodeColourButton.TabIndex = 37;
			this.m_CodeColourButton.UseVisualStyleBackColor = true;
			this.m_CodeColourButton.Click += new EventHandler(this.ColourButtonClicked);
			this.m_SelectedHighlightTextColourButton.FlatStyle = FlatStyle.Flat;
			this.m_SelectedHighlightTextColourButton.Location = new Point(285, 123);
			this.m_SelectedHighlightTextColourButton.Margin = new Padding(2);
			this.m_SelectedHighlightTextColourButton.Name = "m_SelectedHighlightTextColourButton";
			this.m_SelectedHighlightTextColourButton.Size = new Size(34, 19);
			this.m_SelectedHighlightTextColourButton.TabIndex = 46;
			this.m_SelectedHighlightTextColourButton.UseVisualStyleBackColor = true;
			this.m_SelectedHighlightTextColourButton.Click += new EventHandler(this.ColourButtonClicked);
			this.m_ColourThemeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_ColourThemeComboBox.FormattingEnabled = true;
			this.m_ColourThemeComboBox.Location = new Point(156, 63);
			this.m_ColourThemeComboBox.Margin = new Padding(2);
			this.m_ColourThemeComboBox.Name = "m_ColourThemeComboBox";
			this.m_ColourThemeComboBox.Size = new Size(157, 21);
			this.m_ColourThemeComboBox.TabIndex = 41;
			this.m_ColourThemeComboBox.SelectedIndexChanged += new EventHandler(this.ColourThemeChanged);
			this.label3.AutoSize = true;
			this.label3.Location = new Point(87, 102);
			this.label3.Margin = new Padding(2, 0, 2, 0);
			this.label3.Name = "label3";
			this.label3.Size = new Size(65, 13);
			this.label3.TabIndex = 30;
			this.label3.Text = "Background";
			this.label6.AutoSize = true;
			this.label6.Location = new Point(104, 173);
			this.label6.Margin = new Padding(2, 0, 2, 0);
			this.label6.Name = "label6";
			this.label6.Size = new Size(48, 13);
			this.label6.TabIndex = 36;
			this.label6.Text = "Highlight";
			this.label10.AutoSize = true;
			this.label10.Location = new Point(209, 102);
			this.label10.Margin = new Padding(2, 0, 2, 0);
			this.label10.Name = "label10";
			this.label10.Size = new Size(72, 13);
			this.label10.TabIndex = 45;
			this.label10.Text = "Highlight Text";
			this.label9.AutoSize = true;
			this.label9.Location = new Point(78, 65);
			this.label9.Margin = new Padding(2, 0, 2, 0);
			this.label9.Name = "label9";
			this.label9.Size = new Size(73, 13);
			this.label9.TabIndex = 42;
			this.label9.Text = "Colour Theme";
			this.m_ForeColourButton.FlatStyle = FlatStyle.Flat;
			this.m_ForeColourButton.Location = new Point(156, 123);
			this.m_ForeColourButton.Margin = new Padding(2);
			this.m_ForeColourButton.Name = "m_ForeColourButton";
			this.m_ForeColourButton.Size = new Size(34, 19);
			this.m_ForeColourButton.TabIndex = 31;
			this.m_ForeColourButton.UseVisualStyleBackColor = true;
			this.m_ForeColourButton.Click += new EventHandler(this.ColourButtonClicked);
			this.m_HighlightColourButton.FlatStyle = FlatStyle.Flat;
			this.m_HighlightColourButton.Location = new Point(156, 170);
			this.m_HighlightColourButton.Margin = new Padding(2);
			this.m_HighlightColourButton.Name = "m_HighlightColourButton";
			this.m_HighlightColourButton.Size = new Size(34, 19);
			this.m_HighlightColourButton.TabIndex = 35;
			this.m_HighlightColourButton.UseVisualStyleBackColor = true;
			this.m_HighlightColourButton.Click += new EventHandler(this.ColourButtonClicked);
			this.m_HighlightTextColourButton.FlatStyle = FlatStyle.Flat;
			this.m_HighlightTextColourButton.Location = new Point(285, 99);
			this.m_HighlightTextColourButton.Margin = new Padding(2);
			this.m_HighlightTextColourButton.Name = "m_HighlightTextColourButton";
			this.m_HighlightTextColourButton.Size = new Size(34, 19);
			this.m_HighlightTextColourButton.TabIndex = 44;
			this.m_HighlightTextColourButton.UseVisualStyleBackColor = true;
			this.m_HighlightTextColourButton.Click += new EventHandler(this.ColourButtonClicked);
			this.label5.AutoSize = true;
			this.label5.Location = new Point(101, 150);
			this.label5.Margin = new Padding(2, 0, 2, 0);
			this.label5.Name = "label5";
			this.label5.Size = new Size(51, 13);
			this.label5.TabIndex = 34;
			this.label5.Text = "Selection";
			this.label4.AutoSize = true;
			this.label4.Location = new Point(91, 126);
			this.label4.Margin = new Padding(2, 0, 2, 0);
			this.label4.Name = "label4";
			this.label4.Size = new Size(61, 13);
			this.label4.TabIndex = 32;
			this.label4.Text = "Foreground";
			this.m_SelectColourButton.FlatStyle = FlatStyle.Flat;
			this.m_SelectColourButton.Location = new Point(156, 147);
			this.m_SelectColourButton.Margin = new Padding(2);
			this.m_SelectColourButton.Name = "m_SelectColourButton";
			this.m_SelectColourButton.Size = new Size(34, 19);
			this.m_SelectColourButton.TabIndex = 33;
			this.m_SelectColourButton.UseVisualStyleBackColor = true;
			this.m_SelectColourButton.Click += new EventHandler(this.ColourButtonClicked);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.label2);
			base.Controls.Add(this.m_ControlColourButton);
			base.Controls.Add(this.m_FontButton);
			base.Controls.Add(this.m_CodeFilenameColourButton);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.label8);
			base.Controls.Add(this.label11);
			base.Controls.Add(this.label7);
			base.Controls.Add(this.m_BackColourButton);
			base.Controls.Add(this.m_CodeColourButton);
			base.Controls.Add(this.m_SelectedHighlightTextColourButton);
			base.Controls.Add(this.m_ColourThemeComboBox);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.label10);
			base.Controls.Add(this.label9);
			base.Controls.Add(this.m_ForeColourButton);
			base.Controls.Add(this.m_HighlightColourButton);
			base.Controls.Add(this.m_HighlightTextColourButton);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.m_SelectColourButton);
			base.Name = "AppearanceControl";
			base.Size = new Size(467, 329);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
