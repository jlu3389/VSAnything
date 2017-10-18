using Registration;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Company.VSAnything
{
	internal class SettingsDialog : Form
	{
		private IContainer components;

		private FontDialog m_FontDialog;

		private ColorDialog m_ColorDialog;

		private Button button2;

		private TabPage RegistrationTab;

		private Button m_ViewLicenseButton;

		private Button m_PurchaseLicenseButton;

		private Label m_ExpireLabel;

		private Label m_RegisteredLabel;

		private Button m_RegisterLicenseButton;

		private TabPage AboutTab;

		private TextBox m_AboutTextBox;

		private Label label14;

		private PictureBox pictureBox1;

		private PictureBox pictureBox2;

		private TabControl tabControl1;

		private TabPage SettingsTab;

		private Label label2;

		private Label label1;

		public SettingsDialog()
		{
			this.InitializeComponent();
			this.InitialiseAboutTab();
		}

		private void InitialiseAboutTab()
		{
			this.m_AboutTextBox.Text = this.m_AboutTextBox.Text.Replace("##.##", "4.8");
            //bool registered = Registration.Registered;
            bool registered = true;
			this.m_PurchaseLicenseButton.Visible = !registered;
			this.m_RegisterLicenseButton.Visible = !registered;
			this.m_ViewLicenseButton.Visible = registered;
			this.m_ExpireLabel.Visible = !registered;
			this.m_RegisteredLabel.Visible = registered;
			this.m_ExpireLabel.Text = this.m_ExpireLabel.Text.Replace("#", Demo.DaysLeft.ToString());
		}

		private void RegistrationButtonClicked(object sender, EventArgs e)
		{
			VSAnythingPackage.Inst.ShowRegistrationForm();
		}

		private void PurchaseLicenseButtonClicked(object sender, EventArgs e)
		{
			VSAnythingPackage.OpenPurchaseWebPage();
		}

		private void ViewLicenseButtonClicked(object sender, EventArgs e)
		{
			VSAnythingPackage.Inst.ShowRegistrationForm();
		}

		private void RegisterButtonClicked(object sender, EventArgs e)
		{
			VSAnythingPackage.Inst.ShowRegistrationForm();
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(SettingsDialog));
			this.m_FontDialog = new FontDialog();
			this.m_ColorDialog = new ColorDialog();
			this.button2 = new Button();
			this.RegistrationTab = new TabPage();
			this.m_ViewLicenseButton = new Button();
			this.m_PurchaseLicenseButton = new Button();
			this.m_ExpireLabel = new Label();
			this.m_RegisteredLabel = new Label();
			this.m_RegisterLicenseButton = new Button();
			this.AboutTab = new TabPage();
			this.m_AboutTextBox = new TextBox();
			this.label14 = new Label();
			this.pictureBox1 = new PictureBox();
			this.pictureBox2 = new PictureBox();
			this.tabControl1 = new TabControl();
			this.SettingsTab = new TabPage();
			this.label1 = new Label();
			this.label2 = new Label();
			this.RegistrationTab.SuspendLayout();
			this.AboutTab.SuspendLayout();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			this.tabControl1.SuspendLayout();
			this.SettingsTab.SuspendLayout();
			base.SuspendLayout();
			this.m_FontDialog.Color = SystemColors.ControlText;
			this.button2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.button2.DialogResult = DialogResult.OK;
			this.button2.Location = new Point(390, 265);
			this.button2.Margin = new Padding(2);
			this.button2.Name = "button2";
			this.button2.Size = new Size(56, 19);
			this.button2.TabIndex = 21;
			this.button2.Text = "OK";
			this.button2.UseVisualStyleBackColor = true;
			this.RegistrationTab.BackColor = Color.White;
			this.RegistrationTab.Controls.Add(this.m_ViewLicenseButton);
			this.RegistrationTab.Controls.Add(this.m_PurchaseLicenseButton);
			this.RegistrationTab.Controls.Add(this.m_ExpireLabel);
			this.RegistrationTab.Controls.Add(this.m_RegisteredLabel);
			this.RegistrationTab.Controls.Add(this.m_RegisterLicenseButton);
			this.RegistrationTab.Location = new Point(4, 22);
			this.RegistrationTab.Name = "RegistrationTab";
			this.RegistrationTab.Size = new Size(442, 235);
			this.RegistrationTab.TabIndex = 4;
			this.RegistrationTab.Text = "Registration";
			this.m_ViewLicenseButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.m_ViewLicenseButton.BackColor = Color.FromArgb(250, 250, 250);
			this.m_ViewLicenseButton.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
			this.m_ViewLicenseButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 128, 0);
			this.m_ViewLicenseButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(224, 224, 224);
			this.m_ViewLicenseButton.FlatStyle = FlatStyle.Flat;
			this.m_ViewLicenseButton.Font = new Font("Microsoft Sans Serif", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.m_ViewLicenseButton.ForeColor = Color.Black;
			this.m_ViewLicenseButton.Location = new Point(0, 120);
			this.m_ViewLicenseButton.Name = "m_ViewLicenseButton";
			this.m_ViewLicenseButton.Size = new Size(442, 40);
			this.m_ViewLicenseButton.TabIndex = 34;
			this.m_ViewLicenseButton.Text = "View License...";
			this.m_ViewLicenseButton.UseVisualStyleBackColor = false;
			this.m_ViewLicenseButton.Click += new EventHandler(this.ViewLicenseButtonClicked);
			this.m_PurchaseLicenseButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.m_PurchaseLicenseButton.BackColor = Color.FromArgb(250, 250, 250);
			this.m_PurchaseLicenseButton.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
			this.m_PurchaseLicenseButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 128, 0);
			this.m_PurchaseLicenseButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(224, 224, 224);
			this.m_PurchaseLicenseButton.FlatStyle = FlatStyle.Flat;
			this.m_PurchaseLicenseButton.Font = new Font("Microsoft Sans Serif", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.m_PurchaseLicenseButton.ForeColor = Color.Black;
			this.m_PurchaseLicenseButton.Location = new Point(0, 104);
			this.m_PurchaseLicenseButton.Name = "m_PurchaseLicenseButton";
			this.m_PurchaseLicenseButton.Size = new Size(442, 40);
			this.m_PurchaseLicenseButton.TabIndex = 33;
			this.m_PurchaseLicenseButton.Text = "Purchase License...";
			this.m_PurchaseLicenseButton.UseVisualStyleBackColor = false;
			this.m_PurchaseLicenseButton.Click += new EventHandler(this.PurchaseLicenseButtonClicked);
			this.m_ExpireLabel.AutoSize = true;
			this.m_ExpireLabel.Font = new Font("Microsoft Sans Serif", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.m_ExpireLabel.ForeColor = Color.Black;
			this.m_ExpireLabel.Location = new Point(63, 53);
			this.m_ExpireLabel.Name = "m_ExpireLabel";
			this.m_ExpireLabel.Size = new Size(306, 25);
			this.m_ExpireLabel.TabIndex = 32;
			this.m_ExpireLabel.Text = "This demo will expire in # days";
			this.m_RegisteredLabel.AutoSize = true;
			this.m_RegisteredLabel.Font = new Font("Microsoft Sans Serif", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.m_RegisteredLabel.ForeColor = Color.Black;
			this.m_RegisteredLabel.Location = new Point(70, 40);
			this.m_RegisteredLabel.Name = "m_RegisteredLabel";
			this.m_RegisteredLabel.Size = new Size(293, 25);
			this.m_RegisteredLabel.TabIndex = 31;
			this.m_RegisteredLabel.Text = "FastFind has been registered";
			this.m_RegisterLicenseButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.m_RegisterLicenseButton.BackColor = Color.FromArgb(250, 250, 250);
			this.m_RegisterLicenseButton.FlatAppearance.BorderColor = Color.FromArgb(224, 224, 224);
			this.m_RegisterLicenseButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 128, 0);
			this.m_RegisterLicenseButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(224, 224, 224);
			this.m_RegisterLicenseButton.FlatStyle = FlatStyle.Flat;
			this.m_RegisterLicenseButton.Font = new Font("Microsoft Sans Serif", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.m_RegisterLicenseButton.ForeColor = Color.Black;
			this.m_RegisterLicenseButton.Location = new Point(0, 166);
			this.m_RegisterLicenseButton.Name = "m_RegisterLicenseButton";
			this.m_RegisterLicenseButton.Size = new Size(442, 40);
			this.m_RegisterLicenseButton.TabIndex = 30;
			this.m_RegisterLicenseButton.Text = "Register FastFind...";
			this.m_RegisterLicenseButton.UseVisualStyleBackColor = false;
			this.m_RegisterLicenseButton.Click += new EventHandler(this.RegisterButtonClicked);
			this.AboutTab.Controls.Add(this.m_AboutTextBox);
			this.AboutTab.Controls.Add(this.label14);
			this.AboutTab.Controls.Add(this.pictureBox1);
			this.AboutTab.Controls.Add(this.pictureBox2);
			this.AboutTab.Location = new Point(4, 22);
			this.AboutTab.Name = "AboutTab";
			this.AboutTab.Size = new Size(442, 235);
			this.AboutTab.TabIndex = 3;
			this.AboutTab.Text = "About";
			this.AboutTab.UseVisualStyleBackColor = true;
			this.m_AboutTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.m_AboutTextBox.BackColor = Color.White;
			this.m_AboutTextBox.BorderStyle = BorderStyle.None;
			this.m_AboutTextBox.Location = new Point(8, 103);
			this.m_AboutTextBox.Multiline = true;
			this.m_AboutTextBox.Name = "m_AboutTextBox";
			this.m_AboutTextBox.ReadOnly = true;
			this.m_AboutTextBox.Size = new Size(207, 73);
			this.m_AboutTextBox.TabIndex = 11;
			this.m_AboutTextBox.TabStop = false;
			this.m_AboutTextBox.Text = "Version: ##.##\r\n\r\nCopyright: Stewart Lynch\r\nCompany: PureDev Software\r\nContact: slynch@puredevsoftware.com";
			this.label14.AutoSize = true;
			this.label14.Font = new Font("Microsoft Sans Serif", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.label14.ForeColor = Color.Black;
			this.label14.Location = new Point(55, 24);
			this.label14.Name = "label14";
			this.label14.Size = new Size(96, 25);
			this.label14.TabIndex = 10;
			this.label14.Text = "FastFind";
			this.pictureBox1.Image = Resource1.Package;
			this.pictureBox1.Location = new Point(8, 18);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(41, 38);
			this.pictureBox1.TabIndex = 9;
			this.pictureBox1.TabStop = false;
			this.pictureBox2.Image = Resource1.puredev_software_logo_email;
			this.pictureBox2.Location = new Point(231, 18);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new Size(194, 73);
			this.pictureBox2.TabIndex = 8;
			this.pictureBox2.TabStop = false;
			this.tabControl1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.tabControl1.Controls.Add(this.SettingsTab);
			this.tabControl1.Controls.Add(this.RegistrationTab);
			this.tabControl1.Controls.Add(this.AboutTab);
			this.tabControl1.Location = new Point(0, 0);
			this.tabControl1.Margin = new Padding(2);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new Size(450, 261);
			this.tabControl1.TabIndex = 4;
			this.SettingsTab.Controls.Add(this.label2);
			this.SettingsTab.Controls.Add(this.label1);
			this.SettingsTab.Location = new Point(4, 22);
			this.SettingsTab.Name = "SettingsTab";
			this.SettingsTab.Size = new Size(442, 235);
			this.SettingsTab.TabIndex = 5;
			this.SettingsTab.Text = "Settings";
			this.SettingsTab.UseVisualStyleBackColor = true;
			this.label1.AutoSize = true;
			this.label1.Location = new Point(73, 94);
			this.label1.Name = "label1";
			this.label1.Size = new Size(295, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "FastFind Settings are now stored in the Visual Studio Settings";
			this.label2.AutoSize = true;
			this.label2.Location = new Point(97, 126);
			this.label2.Name = "label2";
			this.label2.Size = new Size(248, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Please go to the Tools menu -> Options -> FastFind";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(450, 289);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.tabControl1);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.Icon = (Icon)resources.GetObject("$this.Icon");
			base.Margin = new Padding(2);
			base.Name = "SettingsDialog";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "FastFind Settings";
			this.RegistrationTab.ResumeLayout(false);
			this.RegistrationTab.PerformLayout();
			this.AboutTab.ResumeLayout(false);
			this.AboutTab.PerformLayout();
			((ISupportInitialize)this.pictureBox1).EndInit();
			((ISupportInitialize)this.pictureBox2).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.SettingsTab.ResumeLayout(false);
			this.SettingsTab.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
