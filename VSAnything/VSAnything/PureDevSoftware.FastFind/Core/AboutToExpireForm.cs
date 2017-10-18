using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Company.VSAnything
{
	public class AboutToExpireForm : Form
	{
		private bool m_Purchased;

		private IContainer components;

		private Label m_MessageLabel;

		private PictureBox pictureBox1;

		private Button button1;

		private Button button2;

		private Label label2;

		private Label label1;

		private PictureBox pictureBox2;

		public bool Purchased
		{
			get
			{
				return this.m_Purchased;
			}
		}

		public AboutToExpireForm(int days_left)
		{
			this.InitializeComponent();
			days_left = Math.Max(0, days_left);
			this.m_MessageLabel.Text = this.m_MessageLabel.Text.Replace("#", days_left.ToString());
		}

		private void BuyaLicenseButtonClicked(object sender, EventArgs e)
		{
			VSAnythingPackage.OpenPurchaseWebPage();
			this.m_Purchased = true;
			base.Close();
		}

		private void ContinueUsingFastFindButtonClicked(object sender, EventArgs e)
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(AboutToExpireForm));
			this.m_MessageLabel = new Label();
			this.pictureBox1 = new PictureBox();
			this.button1 = new Button();
			this.button2 = new Button();
			this.label2 = new Label();
			this.label1 = new Label();
			this.pictureBox2 = new PictureBox();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			base.SuspendLayout();
			this.m_MessageLabel.AutoSize = true;
			this.m_MessageLabel.Font = new Font("Microsoft Sans Serif", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.m_MessageLabel.ForeColor = Color.Black;
			this.m_MessageLabel.Location = new Point(65, 122);
			this.m_MessageLabel.Name = "m_MessageLabel";
			this.m_MessageLabel.Size = new Size(306, 25);
			this.m_MessageLabel.TabIndex = 0;
			this.m_MessageLabel.Text = "This demo will expire in # days";
			this.pictureBox1.Image = Resource1.Package;
			this.pictureBox1.Location = new Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(41, 38);
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			this.button1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.button1.BackColor = Color.FromArgb(250, 250, 250);
			this.button1.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
			this.button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 128, 0);
			this.button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(224, 224, 224);
			this.button1.FlatStyle = FlatStyle.Flat;
			this.button1.Font = new Font("Microsoft Sans Serif", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.button1.ForeColor = Color.Black;
			this.button1.Location = new Point(-3, 207);
			this.button1.Name = "button1";
			this.button1.Size = new Size(442, 57);
			this.button1.TabIndex = 3;
			this.button1.Text = "Buy a license";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new EventHandler(this.BuyaLicenseButtonClicked);
			this.button2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.button2.BackColor = Color.FromArgb(250, 250, 250);
			this.button2.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
			this.button2.FlatAppearance.MouseOverBackColor = Color.FromArgb(224, 224, 224);
			this.button2.FlatStyle = FlatStyle.Flat;
			this.button2.Font = new Font("Microsoft Sans Serif", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.button2.ForeColor = Color.Black;
			this.button2.Location = new Point(118, 292);
			this.button2.Name = "button2";
			this.button2.Size = new Size(200, 33);
			this.button2.TabIndex = 4;
			this.button2.Text = "Continue using FastFind";
			this.button2.UseVisualStyleBackColor = false;
			this.button2.Click += new EventHandler(this.ContinueUsingFastFindButtonClicked);
			this.label2.AutoSize = true;
			this.label2.Font = new Font("Microsoft Sans Serif", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.label2.ForeColor = Color.Black;
			this.label2.Location = new Point(61, 163);
			this.label2.Name = "label2";
			this.label2.Size = new Size(344, 25);
			this.label2.TabIndex = 5;
			this.label2.Text = "Licences available from only $12.00";
			this.label1.AutoSize = true;
			this.label1.Font = new Font("Microsoft Sans Serif", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.label1.ForeColor = Color.Black;
			this.label1.Location = new Point(59, 18);
			this.label1.Name = "label1";
			this.label1.Size = new Size(96, 25);
			this.label1.TabIndex = 6;
			this.label1.Text = "FastFind";
			this.pictureBox2.Image = Resource1.puredev_software_logo_email;
			this.pictureBox2.Location = new Point(231, 12);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new Size(194, 73);
			this.pictureBox2.TabIndex = 7;
			this.pictureBox2.TabStop = false;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.ClientSize = new Size(436, 337);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.pictureBox1);
			base.Controls.Add(this.m_MessageLabel);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)resources.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AboutToExpireForm";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "FastFind";
			((ISupportInitialize)this.pictureBox1).EndInit();
			((ISupportInitialize)this.pictureBox2).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
