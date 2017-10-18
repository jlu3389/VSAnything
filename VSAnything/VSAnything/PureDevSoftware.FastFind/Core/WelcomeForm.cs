using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Company.VSAnything
{
	public class WelcomeForm : Form
	{
		private IContainer components;

		private Label label1;

		private Panel panel1;

		private PictureBox pictureBox1;

		private Button button2;

		private Button button1;

		private RichTextBox richTextBox2;

		private RichTextBox richTextBox1;

		public WelcomeForm()
		{
			this.InitializeComponent();
		}

		private void CloseButtonClicked(object sender, EventArgs e)
		{
			base.Close();
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(WelcomeForm));
			this.label1 = new Label();
			this.panel1 = new Panel();
			this.button1 = new Button();
			this.button2 = new Button();
			this.pictureBox1 = new PictureBox();
			this.richTextBox1 = new RichTextBox();
			this.richTextBox2 = new RichTextBox();
			this.panel1.SuspendLayout();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Font = new Font("Segoe UI", 18f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.label1.ForeColor = Color.Black;
			this.label1.Location = new Point(99, 32);
			this.label1.Name = "label1";
			this.label1.Size = new Size(239, 32);
			this.label1.TabIndex = 0;
			this.label1.Text = "Welcome to FastFind";
			this.panel1.BackColor = Color.White;
			this.panel1.BorderStyle = BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.richTextBox2);
			this.panel1.Controls.Add(this.richTextBox1);
			this.panel1.Controls.Add(this.pictureBox1);
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = DockStyle.Fill;
			this.panel1.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.panel1.Location = new Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(404, 424);
			this.panel1.TabIndex = 1;
			this.button1.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.button1.FlatAppearance.BorderSize = 0;
			this.button1.FlatStyle = FlatStyle.Flat;
			this.button1.Location = new Point(368, -1);
			this.button1.Name = "button1";
			this.button1.Size = new Size(35, 23);
			this.button1.TabIndex = 3;
			this.button1.Text = "X";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new EventHandler(this.CloseButtonClicked);
			this.button2.Location = new Point(159, 385);
			this.button2.Name = "button2";
			this.button2.Size = new Size(75, 23);
			this.button2.TabIndex = 4;
			this.button2.Text = "Close";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new EventHandler(this.CloseButtonClicked);
			this.pictureBox1.Image = Resource1.Package;
			this.pictureBox1.Location = new Point(59, 29);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(34, 35);
			this.pictureBox1.TabIndex = 5;
			this.pictureBox1.TabStop = false;
			this.richTextBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.richTextBox1.BackColor = Color.WhiteSmoke;
			this.richTextBox1.BorderStyle = BorderStyle.FixedSingle;
			this.richTextBox1.Enabled = false;
			this.richTextBox1.ForeColor = Color.Black;
			this.richTextBox1.Location = new Point(11, 247);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new Size(380, 132);
			this.richTextBox1.TabIndex = 6;
			this.richTextBox1.Text = "To get the most out of FastFind please read the Getting Started Guide:\nhttp://www.puredevsoftware.com/fastfind/getting_started.htm\n\nPlease send bugs and feature requests to slynch@puredevsoftware.com\n";
			this.richTextBox2.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.richTextBox2.BackColor = Color.WhiteSmoke;
			this.richTextBox2.BorderStyle = BorderStyle.FixedSingle;
			this.richTextBox2.Enabled = false;
			this.richTextBox2.ForeColor = Color.Black;
			this.richTextBox2.HideSelection = false;
			this.richTextBox2.Location = new Point(11, 83);
			this.richTextBox2.Name = "richTextBox2";
			this.richTextBox2.ReadOnly = true;
			this.richTextBox2.Size = new Size(380, 143);
			this.richTextBox2.TabIndex = 7;
			this.richTextBox2.Text = resources.GetString("richTextBox2.Text");
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(404, 424);
			base.Controls.Add(this.panel1);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Name = "WelcomeForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "WelcomeForm";
			base.TopMost = true;
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
