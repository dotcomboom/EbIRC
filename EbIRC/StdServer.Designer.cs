namespace EbiSoft.EbIRC
{
    partial class StdServer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.tServer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tPort = new System.Windows.Forms.TextBox();
            this.tServerPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cSSL = new System.Windows.Forms.CheckBox();
            this.cValidate = new System.Windows.Forms.CheckBox();
            this.tNick = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tRealName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tLoginName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tNickPass = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Cancel";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "OK";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // tServer
            // 
            this.tServer.Location = new System.Drawing.Point(5, 35);
            this.tServer.Name = "tServer";
            this.tServer.Size = new System.Drawing.Size(109, 22);
            this.tServer.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 22);
            this.label2.Text = "Server";
            // 
            // tPort
            // 
            this.tPort.Location = new System.Drawing.Point(120, 35);
            this.tPort.Name = "tPort";
            this.tPort.Size = new System.Drawing.Size(42, 22);
            this.tPort.TabIndex = 3;
            this.tPort.Text = "6669";
            // 
            // tServerPassword
            // 
            this.tServerPassword.Location = new System.Drawing.Point(5, 85);
            this.tServerPassword.Name = "tServerPassword";
            this.tServerPassword.Size = new System.Drawing.Size(109, 22);
            this.tServerPassword.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 22);
            this.label1.Text = "Server password";
            // 
            // cSSL
            // 
            this.cSSL.Location = new System.Drawing.Point(120, 85);
            this.cSSL.Name = "cSSL";
            this.cSSL.Size = new System.Drawing.Size(49, 22);
            this.cSSL.TabIndex = 7;
            this.cSSL.Text = "SSL?";
            // 
            // cValidate
            // 
            this.cValidate.Location = new System.Drawing.Point(5, 113);
            this.cValidate.Name = "cValidate";
            this.cValidate.Size = new System.Drawing.Size(130, 22);
            this.cValidate.TabIndex = 8;
            this.cValidate.Text = "Do not validate";
            // 
            // tNick
            // 
            this.tNick.Location = new System.Drawing.Point(5, 161);
            this.tNick.Name = "tNick";
            this.tNick.Size = new System.Drawing.Size(109, 22);
            this.tNick.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(5, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 22);
            this.label3.Text = "Nickname";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(4, 187);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(152, 22);
            this.label4.Text = "Real name";
            // 
            // tRealName
            // 
            this.tRealName.Location = new System.Drawing.Point(5, 209);
            this.tRealName.Name = "tRealName";
            this.tRealName.Size = new System.Drawing.Size(109, 22);
            this.tRealName.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(5, 238);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(152, 22);
            this.label5.Text = "Login name";
            // 
            // tLoginName
            // 
            this.tLoginName.Location = new System.Drawing.Point(6, 260);
            this.tLoginName.Name = "tLoginName";
            this.tLoginName.Size = new System.Drawing.Size(109, 22);
            this.tLoginName.TabIndex = 19;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(4, 287);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(152, 22);
            this.label6.Text = "Nickserv password";
            // 
            // tNickPass
            // 
            this.tNickPass.Location = new System.Drawing.Point(5, 309);
            this.tNickPass.Name = "tNickPass";
            this.tNickPass.Size = new System.Drawing.Size(109, 22);
            this.tNickPass.TabIndex = 22;
            // 
            // StdServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(169, 180);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tNickPass);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tLoginName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tRealName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tNick);
            this.Controls.Add(this.cValidate);
            this.Controls.Add(this.cSSL);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tServerPassword);
            this.Controls.Add(this.tPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tServer);
            this.Menu = this.mainMenu1;
            this.Name = "StdServer";
            this.Text = "Server Settings";
            this.Load += new System.EventHandler(this.StdServer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.TextBox tServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tPort;
        private System.Windows.Forms.TextBox tServerPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cSSL;
        private System.Windows.Forms.CheckBox cValidate;
        private System.Windows.Forms.TextBox tNick;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tRealName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tLoginName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tNickPass;
    }
}