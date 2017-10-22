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
		}

		private void PurchaseLicenseButtonClicked(object sender, EventArgs e)
		{
		}

		private void ViewLicenseButtonClicked(object sender, EventArgs e)
		{
		}

		private void RegisterButtonClicked(object sender, EventArgs e)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
            this.m_FontDialog = new System.Windows.Forms.FontDialog();
            this.m_ColorDialog = new System.Windows.Forms.ColorDialog();
            this.button2 = new System.Windows.Forms.Button();
            this.RegistrationTab = new System.Windows.Forms.TabPage();
            this.m_ViewLicenseButton = new System.Windows.Forms.Button();
            this.m_PurchaseLicenseButton = new System.Windows.Forms.Button();
            this.m_ExpireLabel = new System.Windows.Forms.Label();
            this.m_RegisteredLabel = new System.Windows.Forms.Label();
            this.m_RegisterLicenseButton = new System.Windows.Forms.Button();
            this.AboutTab = new System.Windows.Forms.TabPage();
            this.m_AboutTextBox = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.SettingsTab = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.RegistrationTab.SuspendLayout();
            this.AboutTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SettingsTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_FontDialog
            // 
            this.m_FontDialog.Color = System.Drawing.SystemColors.ControlText;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(1272, 631);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(56, 19);
            this.button2.TabIndex = 21;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // RegistrationTab
            // 
            this.RegistrationTab.BackColor = System.Drawing.Color.White;
            this.RegistrationTab.Controls.Add(this.m_ViewLicenseButton);
            this.RegistrationTab.Controls.Add(this.m_PurchaseLicenseButton);
            this.RegistrationTab.Controls.Add(this.m_ExpireLabel);
            this.RegistrationTab.Controls.Add(this.m_RegisteredLabel);
            this.RegistrationTab.Controls.Add(this.m_RegisterLicenseButton);
            this.RegistrationTab.Location = new System.Drawing.Point(8, 39);
            this.RegistrationTab.Name = "RegistrationTab";
            this.RegistrationTab.Size = new System.Drawing.Size(434, 214);
            this.RegistrationTab.TabIndex = 4;
            this.RegistrationTab.Text = "Registration";
            // 
            // m_ViewLicenseButton
            // 
            this.m_ViewLicenseButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_ViewLicenseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.m_ViewLicenseButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.m_ViewLicenseButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.m_ViewLicenseButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.m_ViewLicenseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_ViewLicenseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_ViewLicenseButton.ForeColor = System.Drawing.Color.Black;
            this.m_ViewLicenseButton.Location = new System.Drawing.Point(0, 120);
            this.m_ViewLicenseButton.Name = "m_ViewLicenseButton";
            this.m_ViewLicenseButton.Size = new System.Drawing.Size(434, 40);
            this.m_ViewLicenseButton.TabIndex = 34;
            this.m_ViewLicenseButton.Text = "View License...";
            this.m_ViewLicenseButton.UseVisualStyleBackColor = false;
            this.m_ViewLicenseButton.Click += new System.EventHandler(this.ViewLicenseButtonClicked);
            // 
            // m_PurchaseLicenseButton
            // 
            this.m_PurchaseLicenseButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_PurchaseLicenseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.m_PurchaseLicenseButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.m_PurchaseLicenseButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.m_PurchaseLicenseButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.m_PurchaseLicenseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_PurchaseLicenseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_PurchaseLicenseButton.ForeColor = System.Drawing.Color.Black;
            this.m_PurchaseLicenseButton.Location = new System.Drawing.Point(0, 104);
            this.m_PurchaseLicenseButton.Name = "m_PurchaseLicenseButton";
            this.m_PurchaseLicenseButton.Size = new System.Drawing.Size(434, 40);
            this.m_PurchaseLicenseButton.TabIndex = 33;
            this.m_PurchaseLicenseButton.Text = "Purchase License...";
            this.m_PurchaseLicenseButton.UseVisualStyleBackColor = false;
            this.m_PurchaseLicenseButton.Click += new System.EventHandler(this.PurchaseLicenseButtonClicked);
            // 
            // m_ExpireLabel
            // 
            this.m_ExpireLabel.AutoSize = true;
            this.m_ExpireLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_ExpireLabel.ForeColor = System.Drawing.Color.Black;
            this.m_ExpireLabel.Location = new System.Drawing.Point(63, 53);
            this.m_ExpireLabel.Name = "m_ExpireLabel";
            this.m_ExpireLabel.Size = new System.Drawing.Size(588, 48);
            this.m_ExpireLabel.TabIndex = 32;
            this.m_ExpireLabel.Text = "This demo will expire in # days";
            // 
            // m_RegisteredLabel
            // 
            this.m_RegisteredLabel.AutoSize = true;
            this.m_RegisteredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_RegisteredLabel.ForeColor = System.Drawing.Color.Black;
            this.m_RegisteredLabel.Location = new System.Drawing.Point(70, 40);
            this.m_RegisteredLabel.Name = "m_RegisteredLabel";
            this.m_RegisteredLabel.Size = new System.Drawing.Size(562, 48);
            this.m_RegisteredLabel.TabIndex = 31;
            this.m_RegisteredLabel.Text = "FastFind has been registered";
            // 
            // m_RegisterLicenseButton
            // 
            this.m_RegisterLicenseButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_RegisterLicenseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.m_RegisterLicenseButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.m_RegisterLicenseButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.m_RegisterLicenseButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.m_RegisterLicenseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_RegisterLicenseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_RegisterLicenseButton.ForeColor = System.Drawing.Color.Black;
            this.m_RegisterLicenseButton.Location = new System.Drawing.Point(0, 166);
            this.m_RegisterLicenseButton.Name = "m_RegisterLicenseButton";
            this.m_RegisterLicenseButton.Size = new System.Drawing.Size(434, 40);
            this.m_RegisterLicenseButton.TabIndex = 30;
            this.m_RegisterLicenseButton.Text = "Register FastFind...";
            this.m_RegisterLicenseButton.UseVisualStyleBackColor = false;
            this.m_RegisterLicenseButton.Click += new System.EventHandler(this.RegisterButtonClicked);
            // 
            // AboutTab
            // 
            this.AboutTab.Controls.Add(this.m_AboutTextBox);
            this.AboutTab.Controls.Add(this.label14);
            this.AboutTab.Controls.Add(this.pictureBox1);
            this.AboutTab.Controls.Add(this.pictureBox2);
            this.AboutTab.Location = new System.Drawing.Point(8, 39);
            this.AboutTab.Name = "AboutTab";
            this.AboutTab.Size = new System.Drawing.Size(434, 214);
            this.AboutTab.TabIndex = 3;
            this.AboutTab.Text = "About";
            this.AboutTab.UseVisualStyleBackColor = true;
            // 
            // m_AboutTextBox
            // 
            this.m_AboutTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_AboutTextBox.BackColor = System.Drawing.Color.White;
            this.m_AboutTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.m_AboutTextBox.Location = new System.Drawing.Point(8, 103);
            this.m_AboutTextBox.Multiline = true;
            this.m_AboutTextBox.Name = "m_AboutTextBox";
            this.m_AboutTextBox.ReadOnly = true;
            this.m_AboutTextBox.Size = new System.Drawing.Size(199, 73);
            this.m_AboutTextBox.TabIndex = 11;
            this.m_AboutTextBox.TabStop = false;
            this.m_AboutTextBox.Text = "Version: ##.##\r\n\r\nCopyright: Stewart Lynch\r\nCompany: PureDev Software\r\nContact: s" +
    "lynch@puredevsoftware.com";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.Black;
            this.label14.Location = new System.Drawing.Point(55, 24);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(184, 48);
            this.label14.TabIndex = 10;
            this.label14.Text = "FastFind";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(8, 18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(41, 38);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(231, 18);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(194, 73);
            this.pictureBox2.TabIndex = 8;
            this.pictureBox2.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.SettingsTab);
            this.tabControl1.Controls.Add(this.RegistrationTab);
            this.tabControl1.Controls.Add(this.AboutTab);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1332, 627);
            this.tabControl1.TabIndex = 4;
            // 
            // SettingsTab
            // 
            this.SettingsTab.Controls.Add(this.label2);
            this.SettingsTab.Controls.Add(this.label1);
            this.SettingsTab.Location = new System.Drawing.Point(8, 39);
            this.SettingsTab.Name = "SettingsTab";
            this.SettingsTab.Size = new System.Drawing.Size(1316, 580);
            this.SettingsTab.TabIndex = 5;
            this.SettingsTab.Text = "Settings";
            this.SettingsTab.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(97, 126);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(610, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "Please go to the Tools menu -> Options -> FastFind";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(73, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(754, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "FastFind Settings are now stored in the Visual Studio Settings";
            // 
            // SettingsDialog
            // 
            this.ClientSize = new System.Drawing.Size(1332, 655);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FastFind Settings";
            this.RegistrationTab.ResumeLayout(false);
            this.RegistrationTab.PerformLayout();
            this.AboutTab.ResumeLayout(false);
            this.AboutTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.SettingsTab.ResumeLayout(false);
            this.SettingsTab.PerformLayout();
            this.ResumeLayout(false);

		}
	}
}
