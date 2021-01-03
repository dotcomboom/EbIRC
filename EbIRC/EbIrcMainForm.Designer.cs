﻿namespace EbiSoft.EbIRC
{
    partial class EbIrcMainForm
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EbIrcMainForm));
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.multiMenuItem = new System.Windows.Forms.MenuItem();
            this.menuMenuItem = new System.Windows.Forms.MenuItem();
            this.menuChannelListMenuItem = new System.Windows.Forms.MenuItem();
            this.menuChannelListServerMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuAllChannelMenuItem = new System.Windows.Forms.MenuItem();
            this.menuAllHighlightsMenuItem = new System.Windows.Forms.MenuItem();
            this.menuChannelControlMenuItem = new System.Windows.Forms.MenuItem();
            this.connectionMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuEditMenuItem = new System.Windows.Forms.MenuItem();
            this.menuEditCopyMenuItem = new System.Windows.Forms.MenuItem();
            this.menuEditGoogleMenuItem = new System.Windows.Forms.MenuItem();
            this.menuEditOpenURLMenuItem = new System.Windows.Forms.MenuItem();
            this.menuEditPasteMenuItem = new System.Windows.Forms.MenuItem();
            this.menuEditClearMenuItem = new System.Windows.Forms.MenuItem();
            this.nicknameSwitchMenuItem = new System.Windows.Forms.MenuItem();
            this.menuNicknameInputMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuSettingMenuItem = new System.Windows.Forms.MenuItem();
            this.menuExitMenuItem = new System.Windows.Forms.MenuItem();
            this.infomationPanel = new System.Windows.Forms.Panel();
            this.topicLabel = new System.Windows.Forms.Label();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.pongTimer = new System.Windows.Forms.Timer();
            this.clearHighlightTimer = new System.Windows.Forms.Timer();
            this.infomationPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.multiMenuItem);
            this.mainMenu1.MenuItems.Add(this.menuMenuItem);
            // 
            // multiMenuItem
            // 
            resources.ApplyResources(this.multiMenuItem, "multiMenuItem");
            this.multiMenuItem.Click += new System.EventHandler(this.multiMenuItem_Click);
            // 
            // menuMenuItem
            // 
            this.menuMenuItem.MenuItems.Add(this.menuChannelListMenuItem);
            this.menuMenuItem.MenuItems.Add(this.menuAllChannelMenuItem);
            this.menuMenuItem.MenuItems.Add(this.menuAllHighlightsMenuItem);
            this.menuMenuItem.MenuItems.Add(this.menuChannelControlMenuItem);
            this.menuMenuItem.MenuItems.Add(this.connectionMenuItem);
            this.menuMenuItem.MenuItems.Add(this.menuItem8);
            this.menuMenuItem.MenuItems.Add(this.menuEditMenuItem);
            this.menuMenuItem.MenuItems.Add(this.nicknameSwitchMenuItem);
            this.menuMenuItem.MenuItems.Add(this.menuItem1);
            this.menuMenuItem.MenuItems.Add(this.menuSettingMenuItem);
            this.menuMenuItem.MenuItems.Add(this.menuExitMenuItem);
            resources.ApplyResources(this.menuMenuItem, "menuMenuItem");
            // 
            // menuChannelListMenuItem
            // 
            this.menuChannelListMenuItem.MenuItems.Add(this.menuChannelListServerMenuItem);
            this.menuChannelListMenuItem.MenuItems.Add(this.menuItem3);
            resources.ApplyResources(this.menuChannelListMenuItem, "menuChannelListMenuItem");
            // 
            // menuChannelListServerMenuItem
            // 
            resources.ApplyResources(this.menuChannelListServerMenuItem, "menuChannelListServerMenuItem");
            this.menuChannelListServerMenuItem.Click += new System.EventHandler(this.menuChannelListServerMenuItem_Click);
            // 
            // menuItem3
            // 
            resources.ApplyResources(this.menuItem3, "menuItem3");
            // 
            // menuAllChannelMenuItem
            // 
            resources.ApplyResources(this.menuAllChannelMenuItem, "menuAllChannelMenuItem");
            this.menuAllChannelMenuItem.Click += new System.EventHandler(this.menuAllChannelMessage_Click);
            // 
            // menuAllHighlightsMenuItem
            // 
            resources.ApplyResources(this.menuAllHighlightsMenuItem, "menuAllHighlightsMenuItem");
            this.menuAllHighlightsMenuItem.Click += new System.EventHandler(this.menuAllHighlightsMessage_Click);
            // 
            // menuChannelControlMenuItem
            // 
            resources.ApplyResources(this.menuChannelControlMenuItem, "menuChannelControlMenuItem");
            this.menuChannelControlMenuItem.Click += new System.EventHandler(this.menuChannelControlMenuItem_Click);
            // 
            // connectionMenuItem
            // 
            resources.ApplyResources(this.connectionMenuItem, "connectionMenuItem");
            this.connectionMenuItem.Click += new System.EventHandler(this.connectionMenuItem_Click);
            // 
            // menuItem8
            // 
            resources.ApplyResources(this.menuItem8, "menuItem8");
            // 
            // menuEditMenuItem
            // 
            this.menuEditMenuItem.MenuItems.Add(this.menuEditCopyMenuItem);
            this.menuEditMenuItem.MenuItems.Add(this.menuEditGoogleMenuItem);
            this.menuEditMenuItem.MenuItems.Add(this.menuEditOpenURLMenuItem);
            this.menuEditMenuItem.MenuItems.Add(this.menuEditPasteMenuItem);
            this.menuEditMenuItem.MenuItems.Add(this.menuEditClearMenuItem);
            resources.ApplyResources(this.menuEditMenuItem, "menuEditMenuItem");
            // 
            // menuEditCopyMenuItem
            // 
            resources.ApplyResources(this.menuEditCopyMenuItem, "menuEditCopyMenuItem");
            this.menuEditCopyMenuItem.Click += new System.EventHandler(this.menuEditCopyMenuItem_Click);
            // 
            // menuEditGoogleMenuItem
            // 
            resources.ApplyResources(this.menuEditGoogleMenuItem, "menuEditGoogleMenuItem");
            this.menuEditGoogleMenuItem.Click += new System.EventHandler(this.menuEditGoogleMenuItem_Click);
            // 
            // menuEditOpenURLMenuItem
            // 
            resources.ApplyResources(this.menuEditOpenURLMenuItem, "menuEditOpenURLMenuItem");
            this.menuEditOpenURLMenuItem.Click += new System.EventHandler(this.menuEditOpenURLMenuItem_Click);
            // 
            // menuEditPasteMenuItem
            // 
            resources.ApplyResources(this.menuEditPasteMenuItem, "menuEditPasteMenuItem");
            this.menuEditPasteMenuItem.Click += new System.EventHandler(this.menuEditPasteMenuItem_Click);
            // 
            // menuEditClearMenuItem
            // 
            resources.ApplyResources(this.menuEditClearMenuItem, "menuEditClearMenuItem");
            this.menuEditClearMenuItem.Click += new System.EventHandler(this.menuEditClearMenuItem_Click);
            // 
            // nicknameSwitchMenuItem
            // 
            this.nicknameSwitchMenuItem.MenuItems.Add(this.menuNicknameInputMenuItem);
            resources.ApplyResources(this.nicknameSwitchMenuItem, "nicknameSwitchMenuItem");
            // 
            // menuNicknameInputMenuItem
            // 
            resources.ApplyResources(this.menuNicknameInputMenuItem, "menuNicknameInputMenuItem");
            this.menuNicknameInputMenuItem.Click += new System.EventHandler(this.menuNicknameInputMenuItem_Click);
            // 
            // menuItem1
            // 
            resources.ApplyResources(this.menuItem1, "menuItem1");
            // 
            // menuSettingMenuItem
            // 
            resources.ApplyResources(this.menuSettingMenuItem, "menuSettingMenuItem");
            this.menuSettingMenuItem.Click += new System.EventHandler(this.menuSettingMenuItem_Click);
            // 
            // menuExitMenuItem
            // 
            resources.ApplyResources(this.menuExitMenuItem, "menuExitMenuItem");
            this.menuExitMenuItem.Click += new System.EventHandler(this.menuExitMenuItem_Click);
            // 
            // infomationPanel
            // 
            this.infomationPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.infomationPanel.Controls.Add(this.topicLabel);
            resources.ApplyResources(this.infomationPanel, "infomationPanel");
            this.infomationPanel.Name = "infomationPanel";
            // 
            // topicLabel
            // 
            resources.ApplyResources(this.topicLabel, "topicLabel");
            this.topicLabel.Name = "topicLabel";
            // 
            // inputTextBox
            // 
            resources.ApplyResources(this.inputTextBox, "inputTextBox");
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputTextBox_KeyDown);
            this.inputTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.inputTextBox_KeyUp);
            this.inputTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.inputTextBox_KeyPress);
            // 
            // logTextBox
            // 
            resources.ApplyResources(this.logTextBox, "logTextBox");
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.logTextBox);
            this.mainPanel.Controls.Add(this.inputTextBox);
            this.mainPanel.Controls.Add(this.infomationPanel);
            resources.ApplyResources(this.mainPanel, "mainPanel");
            this.mainPanel.Name = "mainPanel";
            // 
            // pongTimer
            // 
            this.pongTimer.Interval = 15000;
            this.pongTimer.Tick += new System.EventHandler(this.pongTimer_Tick);
            // 
            // clearHighlightTimer
            // 
            this.clearHighlightTimer.Tick += new System.EventHandler(this.clearHighlightTimer_Tick);
            // 
            // EbIrcMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.mainPanel);
            this.Menu = this.mainMenu1;
            this.Name = "EbIrcMainForm";
            this.Deactivate += new System.EventHandler(this.EbIrcMainForm_Deactivate);
            this.Closed += new System.EventHandler(this.EbIrcMainForm_Closed);
            this.Activated += new System.EventHandler(this.EbIrcMainForm_Activated);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.EbIrcMainForm_Closing);
            this.infomationPanel.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuMenuItem;
        private System.Windows.Forms.MenuItem connectionMenuItem;
        private System.Windows.Forms.MenuItem menuSettingMenuItem;
        private System.Windows.Forms.MenuItem menuChannelControlMenuItem;
        private System.Windows.Forms.Panel infomationPanel;
        private System.Windows.Forms.Label topicLabel;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.MenuItem menuChannelListMenuItem;
        private System.Windows.Forms.MenuItem menuItem8;
        private System.Windows.Forms.MenuItem menuExitMenuItem;
        private System.Windows.Forms.MenuItem menuChannelListServerMenuItem;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuEditMenuItem;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuEditCopyMenuItem;
        private System.Windows.Forms.MenuItem menuEditGoogleMenuItem;
        private System.Windows.Forms.MenuItem menuEditPasteMenuItem;
        private System.Windows.Forms.MenuItem menuEditOpenURLMenuItem;
        private System.Windows.Forms.MenuItem nicknameSwitchMenuItem;
        private System.Windows.Forms.MenuItem menuNicknameInputMenuItem;
        private System.Windows.Forms.MenuItem menuEditClearMenuItem;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Timer pongTimer;
        private System.Windows.Forms.Timer clearHighlightTimer;
        private System.Windows.Forms.MenuItem multiMenuItem;
        private System.Windows.Forms.MenuItem menuAllChannelMenuItem;
        private System.Windows.Forms.MenuItem menuAllHighlightsMenuItem;
    }
}

