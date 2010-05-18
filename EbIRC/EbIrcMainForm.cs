using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EbiSoft.EbIRC.IRC;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.WindowsCE.Forms;
using EbiSoft.EbIRC.Properties;
using EbiSoft.EbIRC.Settings;

namespace EbiSoft.EbIRC
{
    /// <summary>
    /// EbIRC ���C���t�H�[��
    /// </summary>
    public partial class EbIrcMainForm : Form
    {
        private readonly string CONNMGR_URL_FORMAT;
        private readonly string LOG_KEY_SERVER;

        private readonly Regex UrlRegex = new Regex(@"([A-Za-z]+)://([^:/]+)(:(\d+))?(/[^#\s]*)(#(\S+))?", RegexOptions.Compiled);

        private const int XCRAWL_KEYCODE = 0x83;  // Xcrawl �̃X�N���[���C�x���g

        #region P/Invoke ��`

#if Win32PInvoke
        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
        [DllImport("user32", CharSet = CharSet.Auto, EntryPoint="SendMessage")]
        private extern static IntPtr SendMessage2(IntPtr hWnd, int msg, int wParam, int lParam);
#else
        [DllImport("coredll", CharSet = CharSet.Auto)]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
        [DllImport("coredll", CharSet = CharSet.Auto, EntryPoint="SendMessage")]
        private extern static IntPtr SendMessage2(IntPtr hWnd, int msg, int wParam, int lParam);
        /*
        [DllImport("coredll", CharSet = CharSet.Auto)]
        private static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);
        */
#endif
        private const int WM_SETREDRAW = 0x000B;
        private const int WM_USER = 0x400;
        private const int EM_GETEVENTMASK = (WM_USER + 59);
        private const int EM_SETEVENTMASK = (WM_USER + 69);
        private const int WM_KEYDOWN = 0x0100;
        private const int VK_PRIOR = 0x21; // Page UP
        private const int VK_NEXT = 0x22;  // Page Down
        private const int EM_LINESCROLL = 0xB6;
        private const int EM_SCROLL = 0xB5;
        private const int SB_LINEUP = 0;        // �P�s��ɃX�N���[��
        private const int SB_LINEDOWN = 1;      // ���A���ɃX�N���[��
        private const int SB_PAGEUP = 2;        // �P�y�[�W��ɃX�N���[��
        private const int SB_PAGEDOWN = 3;      //  ���A���ɃX�N���[��
        //private const int WM_LBUTTONDOWN = 0x201;
        //private const int WM_LBUTTONUP = 0x202;

        #endregion

        IRCClient ircClient;
        Dictionary<string, Channel> m_channel;         // �`�����l�����X�g
        Channel m_currentCh;                           // ���݂̃`�����l��
        Channel m_serverCh;                            // �T�[�o�[���b�Z�[�W
        string m_nickname = string.Empty;              // �����̌��݂̃j�b�N�l�[��
        List<string> m_inputlog;                       // ���̓e�L�X�g���O
        int m_inputlogPtr;                             // �e�L�X�g���O���݈ʒu
        bool m_scrollFlag = false;                     // �X�N���[���C�x���g�t���O

        InputBoxInputFilter m_inputBoxFilter;          // ���̓t�B���^
        LogBoxInputFilter   m_logBoxFilter;              // ���O�{�b�N�X�t�B���^
        bool m_rightFilterling = false;                // �E�L�[�����t�B���^ON

        bool m_storeFlag = false;                           // ���O�~�σ��[�h�t���O
        StringBuilder m_storedLog = new StringBuilder();    // �~�σ��O

        Regex highlightMatcher = null;   // �n�C���C�g�}�b�`�I�u�W�F�N�g
        Regex dislikeMatcher = null;     // �����}�b�`�I�u�W�F�N�g
        bool highlightFlag = false;      // �n�C���C�g����t���O
        Channel highlightChannel = null; // �n�C���C�g����`�����l��

        List<ChannelMenuItem> m_channelPopupMenus; // �`�����l���|�b�v�A�b�v���j���[�ɏo���`�����l���̍���
        int m_lastSendTick = 0;                    // �Ō�ɔ�������Ticktime (�������Ԕ���Ɏg�p)

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public EbIrcMainForm()
        {
            InitializeComponent();

            // �萔���[�h
            CONNMGR_URL_FORMAT = "http://{0}:{1}/";
            LOG_KEY_SERVER     = Resources.ChannelSelecterServerCaption;

            // IRC�N���C�A���g�������A�C�x���g�n���h������
            ircClient = new IRCClient();
            ircClient.OwnerControl = this;
            ircClient.ChangedMyNickname   += new NickNameChangeEventHandler(ircClient_ChangeMyNickname);
            ircClient.Connected           += new EventHandler(ircClient_Connected);
            ircClient.ConnectionFailed    += new EventHandler(ircClient_ConnectionFailed);
            ircClient.Disconnected        += new EventHandler(ircClient_Disconnected);
            ircClient.Kick                += new KickEventHandler(ircClient_Kick);
            ircClient.ProcessedConnection += new EventHandler(ircClient_ProcessedConnection);
            ircClient.ReceiveCtcpQuery    += new CtcpEventHandler(ircClient_ReceiveCtcpQuery);
            ircClient.ReceiveCtcpReply    += new CtcpEventHandler(ircClient_ReceiveCtcpReply);
            ircClient.ReceiveMessage      += new ReceiveMessageEventHandler(ircClient_ReceiveMessage);
            ircClient.ReceiveNames        += new ReceiveNamesEventHandler(ircClient_ReceiveNames);
            ircClient.ReceiveNotice       += new ReceiveMessageEventHandler(ircClient_ReceiveNotice);
            ircClient.ReceiveServerReply  += new ReceiveServerReplyEventHandler(ircClient_ReceiveServerReply);
            ircClient.UserInOut           += new UserInOutEventHandler(ircClient_UserInOut);
            ircClient.TopicChange         += new TopicChangeEventDelegate(ircClient_TopicChange);
            ircClient.ChangedNickname     += new NickNameChangeEventHandler(ircClient_ChangedNickname);
            ircClient.ModeChange          += new ModeChangeEventHandler(ircClient_ModeChange);
            ircClient.StartMessageEvents  += new EventHandler(ircClient_StartMessageEvents);
            ircClient.FinishMessageEvents += new EventHandler(ircClient_FinishMessageEvents);

            // �ݒ��ǂݍ���
            SettingManager.ReadSetting();

            // �`�����l�����X�g������
            m_channel = new Dictionary<string, Channel>(new ChannelNameEqualityComparer());
            m_channelPopupMenus = new List<ChannelMenuItem>();

            // �T�[�o�[�`�����l����p�ӂ���
            m_serverCh = new Channel(LOG_KEY_SERVER, false);

            // �o�[�W�������o��
            Assembly asm = Assembly.GetExecutingAssembly();
            AssemblyName asmName = asm.GetName();
            AddLog(m_serverCh, string.Format(Resources.VersionInfomation, asmName.Version.Major, asmName.Version.Minor, asmName.Version.Build));

            // �e�L�X�g���O������
            m_inputlog = new List<string>(SettingManager.Data.InputLogBufferSize);
            m_inputlogPtr = 0;

            // ���b�Z�[�W�t�B���^�ݒ�
            m_inputBoxFilter = new InputBoxInputFilter(inputTextBox);
            m_inputBoxFilter.EndComposition += new EventHandler(m_inputBoxFilter_EndComposition);
            m_inputBoxFilter.MouseWheelMoveDown += new EventHandler(m_inputBoxFilter_MouseWheelMoveDown);
            m_inputBoxFilter.MouseWheelMoveUp += new EventHandler(m_inputBoxFilter_MouseWheelMoveUp);
            m_logBoxFilter = new LogBoxInputFilter(logTextBox);
            m_logBoxFilter.TapUp += new EventHandler(m_logBoxFilter_TapUp);
            m_logBoxFilter.Resize += new EventHandler(m_logBoxFilter_Resize);

            // UI�ݒ���A�b�v�f�[�g����
            SetConnectionMenuText(); // �ڑ����j���[
            SetDefaultChannel();     // �f�t�H���g�ڑ��`�����l���̓ǂݍ���
            LoadChannel(m_serverCh); // �T�[�o�[���b�Z�[�W�ɐ؂�ւ�
            UpdateUISettings();      // ���̑���UI�ݒ�̃A�b�v�f�[�g

            // �\�t�g�L�[�̓���ւ�
            if (SettingManager.Data.ReverseSoftKey)
            {
                mainMenu1.MenuItems.Remove(connectionMenuItem);
                mainMenu1.MenuItems.Add(connectionMenuItem);

            }
        }

        #region �v���p�e�B

        /// <summary>
        /// �`�����l���ꗗ
        /// </summary>
        internal Dictionary<string, Channel> Channels
        {
            get { return m_channel; }
        }

        /// <summary>
        /// IRC�N���C�A���g
        /// </summary>
        internal IRCClient IRCClient
        {
            get { return ircClient; }
        }

        /// <summary>
        /// ���ݑI������Ă���`�����l��
        /// </summary>
        internal Channel CurrentChannel
        {
            get { return m_currentCh; }
            set
            {
                LoadChannel(value);
            }
        }

        /// <summary>
        /// �T�[�o�[�`�����l���̃C���X�^���X
        /// </summary>
        internal Channel ServerChannel
        {
            get { return m_serverCh; }
        }

        #endregion

        #region �t�H�[���C�x���g

        /// <summary>
        /// �t�H�[����������Ƃ�
        /// </summary>
        private void EbIrcMainForm_Closing(object sender, CancelEventArgs e)
        {
            ircClient.ChangedMyNickname   -= new NickNameChangeEventHandler(ircClient_ChangeMyNickname);
            ircClient.Connected           -= new EventHandler(ircClient_Connected);
            ircClient.ConnectionFailed    -= new EventHandler(ircClient_ConnectionFailed);
            ircClient.Disconnected        -= new EventHandler(ircClient_Disconnected);
            ircClient.Kick                -= new KickEventHandler(ircClient_Kick);
            ircClient.ProcessedConnection -= new EventHandler(ircClient_ProcessedConnection);
            ircClient.ReceiveCtcpQuery    -= new CtcpEventHandler(ircClient_ReceiveCtcpQuery);
            ircClient.ReceiveCtcpReply    -= new CtcpEventHandler(ircClient_ReceiveCtcpReply);
            ircClient.ReceiveMessage      -= new ReceiveMessageEventHandler(ircClient_ReceiveMessage);
            ircClient.ReceiveNames        -= new ReceiveNamesEventHandler(ircClient_ReceiveNames);
            ircClient.ReceiveNotice       -= new ReceiveMessageEventHandler(ircClient_ReceiveNotice);
            ircClient.ReceiveServerReply  -= new ReceiveServerReplyEventHandler(ircClient_ReceiveServerReply);
            ircClient.UserInOut           -= new UserInOutEventHandler(ircClient_UserInOut);
            ircClient.TopicChange         -= new TopicChangeEventDelegate(ircClient_TopicChange);
            ircClient.ChangedNickname     -= new NickNameChangeEventHandler(ircClient_ChangedNickname);
            ircClient.ModeChange          -= new ModeChangeEventHandler(ircClient_ModeChange);
            ircClient.StartMessageEvents  -= new EventHandler(ircClient_StartMessageEvents);
            ircClient.FinishMessageEvents -= new EventHandler(ircClient_FinishMessageEvents);

            ircClient.OwnerControl = null;
        }

        /// <summary>
        /// �t�H�[��������ꂽ�Ƃ�
        /// </summary>
        private void EbIrcMainForm_Closed(object sender, EventArgs e)
        {
            ircClient.Close();
        }

        /// <summary>
        /// �t�H�[�����A�N�e�B�u�����ꂽ�Ƃ�
        /// </summary>
        private void EbIrcMainForm_Activated(object sender, EventArgs e)
        {
            inputTextBox.Focus();
            logTextBox.Enabled = true;
        }

        /// <summary>
        /// �t�H�[������A�N�e�B�u�����ꂽ�Ƃ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EbIrcMainForm_Deactivate(object sender, EventArgs e)
        {
            logTextBox.Enabled = false;
            inputTextBox.Focus();
        }

        /// <summary>
        /// �t�H�[�������T�C�Y���ꂽ�Ƃ��̓���
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            ProcessResize();
        }

        /// <summary>
        /// SIP�p�l���̊J��Ԃ��ς�����Ƃ�
        /// </summary>
        private void inputPanel_EnabledChanged(object sender, EventArgs e)
        {
            ProcessResize();
        }

        private void ProcessResize()
        {
            // SIP�p�l�����̑傫���ύX
            if (inputPanel.Enabled)
            {
                mainPanel.Location = new Point(0, 0);
                mainPanel.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - inputPanel.Bounds.Height);
            }
            else
            {
                mainPanel.Location = new Point(0, 0);
                mainPanel.Size = this.ClientSize;
            }
        }


        #endregion

        #region ���j���[�C�x���g

        /// <summary>
        /// �ڑ��E�ؒf�g�O��
        /// </summary>
        private void connectionMenuItem_Click(object sender, EventArgs e)
        {
            // �K�x�[�W�R���N�g
            GC.Collect();

            // �X�e�[�^�X���ؒf�̂Ƃ����ڑ�����
            if (ircClient.Status == IRCClientStatus.Disconnected)
            {
                // �T�[�o�[���󗓂̂Ƃ��ɂ͐ڑ��������s��Ȃ��B
                if (SettingManager.Data.Profiles.ActiveProfile.Server == string.Empty)
                {
                    MessageBox.Show(Resources.NullServerSettingError,
                        Resources.ConnectionError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    return;
                }

                try
                {
                    // �_�C�A���A�b�v����
                    if (SettingManager.Data.UseNetworkControl)
                    {
                        ConnectionManager.Connection(string.Format(CONNMGR_URL_FORMAT,
                            SettingManager.Data.Profiles.ActiveProfile.Server, SettingManager.Data.Profiles.ActiveProfile.Port.ToString()));
                    }

                    BroadcastLog(Resources.BeginConnection);
                    IRCClient.Encoding = SettingManager.Data.Profiles.ActiveProfile.GetEncoding();
                    ircClient.Connect(SettingManager.Data.Profiles.ActiveProfile.Server, (int)SettingManager.Data.Profiles.ActiveProfile.Port,
                        SettingManager.Data.Profiles.ActiveProfile.Password, SettingManager.Data.Profiles.ActiveProfile.UseSsl,
                        SettingManager.Data.Profiles.ActiveProfile.Nickname, SettingManager.Data.Profiles.ActiveProfile.Realname);
                    SetConnectionMenuText();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    ircClient.Close();
                }
                return;
            }

            // �ؒf�ȊO�̂Ƃ����ؒf����
            else
            {
                // �m�F���b�Z�[�W�\���ݒ肪ON�Ȃ�m�F���b�Z�[�W��\������
                if (SettingManager.Data.ConfimDisconnect)
                {
                    // ���������I�����ꂽ�ꍇ�͔�����
                    if (MessageBox.Show(Resources.DisconnectConfim, Resources.Confim,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1) == DialogResult.No)
                    {
                        return;
                    }
                }

                ircClient.Disconnect();
                return;
            }
        }

        /// <summary>
        /// �ݒ���
        /// </summary>
        private void menuSettingMenuItem_Click(object sender, EventArgs e)
        {
            using (SettingForm settingForm = new SettingForm())
            {
                // �ݒ��ʊJ��
                settingForm.ShowDialog();
                // �f�t�H���g�`�����l���X�V
                SetDefaultChannel();
                // UI�X�V
                UpdateUISettings();
            }
        }

        /// <summary>
        /// �`�����l�����X�g�̃T�[�o�[����
        /// </summary>
        private void menuChannelListServerMenuItem_Click(object sender, EventArgs e)
        {
            LoadChannel(m_serverCh);
        }

        /// <summary>
        /// �`�����l�����X�g�̃`�����l��
        /// </summary>
        private void menuChannelListChannelsMenuItem_Click(object sender, EventArgs e)
        {
            // �I�����ꂽ���j���[���擾
            ChannelMenuItem menu = (ChannelMenuItem)sender;

            foreach (Channel channel in m_channel.Values)
            {
                // ���̃`�����l���̃��j���[��������A���̃`�����l�������[�h
                if (channel == menu.Channel)
                    LoadChannel(channel);
            }
        }

        /// <summary>
        /// �I��
        /// </summary>
        private void menuExitMenuItem_Click(object sender, EventArgs e)
        {
            if (SettingManager.Data.ConfimExit)
            {
                // ���������I�����ꂽ�ꍇ�͔�����
                if (MessageBox.Show(Resources.ExitConfim, Resources.Confim,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }
            }
            Close();
        }

        /// <summary>
        /// �I��͈̓R�s�[
        /// </summary>
        private void menuEditCopyMenuItem_Click(object sender, EventArgs e)
        {
            // �I������Ă��Ȃ��Ȃ甲����
            if (logTextBox.SelectionLength == 0) return;
            Clipboard.SetDataObject(logTextBox.SelectedText);

        }

        /// <summary>
        /// �\��t��
        /// </summary>
        private void menuEditPasteMenuItem_Click(object sender, EventArgs e)
        {
            IDataObject obj = Clipboard.GetDataObject();
            if (obj.GetDataPresent(typeof(string)))
            {
                inputTextBox.Text += (string)obj.GetData(typeof(string));
            }
        }

        /// <summary>
        /// Google��
        /// </summary>
        private void menuEditGoogleMenuItem_Click(object sender, EventArgs e)
        {
            // �I������Ă��Ȃ��Ȃ甲����
            if (logTextBox.SelectionLength == 0) return;
            string url = string.Format(Resources.GoogleURL, Uri.EscapeUriString(logTextBox.SelectedText));

            try
            {
                // URL�I�[�v��
                System.Diagnostics.Process.Start(url, string.Empty);
            }
            catch (Win32Exception)
            {
                MessageBox.Show(Resources.CannotOpenURL, Resources.FaildBoot,
                    MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// �J�[�\���ʒu��URL���J��
        /// </summary>
        private void menuEditOpenURLMenuItem_Click(object sender, EventArgs e)
        {
            string url = GetSelectedURL();
            if (!string.IsNullOrEmpty(url))
            {
                OpenUrl(url);
            }
        }

        /// <summary>
        /// �j�b�N�l�[���؂�ւ�
        /// </summary>
        void nicknameSwitcher_Click(object sender, EventArgs e)
        {
            if (sender is MenuItem)
            {
                MenuItem item = (MenuItem)sender;

                if (ircClient.Status == IRCClientStatus.Online)
                {
                    ircClient.ChangeNickname(item.Text);
                }
            }
        }

        /// <summary>
        /// �j�b�N�l�[���V�K����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuNicknameInputMenuItem_Click(object sender, EventArgs e)
        {
            if (ircClient.Status == IRCClientStatus.Online)
            {
                using (InputBoxForm form = new InputBoxForm())
                {
                    form.Text = Resources.InputNewNicknameTitle;
                    form.Description = Resources.InputNewNicknamePrompt;
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        if (!string.IsNullOrEmpty(form.Value))
                        {
                            ircClient.ChangeNickname(form.Value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ���O�N���A
        /// </summary>
        private void menuEditClearMenuItem_Click(object sender, EventArgs e)
        {
            ClearStoredLog();
            m_currentCh.ClearLog();
            logTextBox.Text = string.Empty;
            AddLog(m_currentCh, Resources.ClearedLog);
        }

        /// <summary>
        /// �`�����l�����상�j���[����
        /// </summary>
        private void menuChannelControlMenuItem_Click(object sender, EventArgs e)
        {
            using (ChannelControlDialog dialog = new ChannelControlDialog())
            {
                dialog.Owner = this;
                dialog.ShowDialog();
                if (dialog.SelectedChannel != null)
                {
                    LoadChannel(dialog.SelectedChannel);
                }
            }
        }

        /// <summary>
        /// ���O���̓{�b�N�X�^�b�v�z�[���h���j���[�I�[�v��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logContextMenu_Popup(object sender, EventArgs e)
        {
            logContextMenu.MenuItems.Clear();

            if (!string.IsNullOrEmpty(GetSelectedURL()))
            {
                logContextMenu.MenuItems.Add(contextUrlOpenMenuItem);
            }
            if (!string.IsNullOrEmpty(logTextBox.SelectedText))
            {
                logContextMenu.MenuItems.Add(contextGoogleMenuItem);
                logContextMenu.MenuItems.Add(contextCopyMenuItem);
            }
        }

        /// <summary>
        /// �`�����l���R���e�L�X�g���j���[�I�[�v��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void channelContextMenu_Popup(object sender, EventArgs e)
        {
            // ��x�N���A
            channelContextMenu.MenuItems.Clear();

            // �n�C���C�g���b�Z�[�W�̏���
            if (menuHilightedMessages.MenuItems.Count > 0)
            {
                channelContextMenu.MenuItems.Add(menuHilightedMessages);
                channelContextMenu.MenuItems.Add(menuHilightedSeparator);
            }

            // ���̑��̃`�����l���Z���N�^�̏���
            List<ChannelMenuItem> menus = new List<ChannelMenuItem>(m_channelPopupMenus);

            // �C���f�b�N�X�t�^
            for (int i = 0; i < menus.Count; i++)
            {
                menus[i].Index = i;
            }

            // �\�[�g���Ēǉ�
            menus.Sort();
            menus.Reverse(); // �~����
            foreach (ChannelMenuItem menu in menus)
            {
                menu.UpdateText();
                channelContextMenu.MenuItems.Add(menu);
            }
        }

        #endregion

        #region ���̓{�b�N�X �C�x���g

        /// <summary>
        /// �e�L�X�g�{�b�N�X�ŃL�[�������ꂽ�Ƃ�
        /// </summary>
        private void inputTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\r':
                    if (inputTextBox.TextLength > 0)
                    {
                        if (ircClient.Status == IRCClientStatus.Online)
                        {
                            if (m_currentCh != m_serverCh)
                            {
                                // ���̓��O���L�^
                                if (m_inputlog.Count == m_inputlog.Capacity)
                                {
                                    m_inputlog.RemoveAt(0);
                                }
                                m_inputlog.Add(inputTextBox.Text);
                                m_inputlogPtr = m_inputlog.Count;

                                if (inputTextBox.Text.StartsWith("/"))
                                {
                                    ircClient.SendCommand(inputTextBox.Text.Substring(1));
                                    inputTextBox.Text = string.Empty;
                                }
                                else
                                {
                                    AddLog(m_currentCh, string.Format("{0}> {1}", ircClient.User.NickName, inputTextBox.Text));
                                    if (m_storeFlag)
                                    {
                                        CommitStoredLog();
                                        BeginStoreLog();
                                    }
                                    ircClient.SendPrivateMessage(m_currentCh.Name, inputTextBox.Text);
                                    inputTextBox.Text = string.Empty;
                                }

                                e.Handled = true;
                                m_lastSendTick = Environment.TickCount;
                            }
                        }
                    }
                    else
                    {
                        // �������ԓ��łȂ���΃|�b�v�A�b�v����
                        if ((Environment.TickCount - m_lastSendTick) > SettingManager.Data.ChannelShortcutIgnoreTimes)
                        {
                            channelContextMenu.Show(logTextBox, new Point(0, 0));
                            e.Handled = true;
                        }
                    }
                    break;
                /*
                case '\t':
                    logTextBox.Focus();
                    break;
                */
                default:
                    break;
            }
        }

        /// <summary>
        /// IME�̕ϊ����I�������Ƃ�
        /// </summary>
        void m_inputBoxFilter_EndComposition(object sender, EventArgs e)
        {
            m_rightFilterling = m_inputBoxFilter.IsAtokConjectureActive()
                && (inputTextBox.TextLength == inputTextBox.SelectionStart);
        }

        /// <summary>
        /// �W���O�z�C�[����������ɓ������Ƃ�
        /// </summary>
        void m_inputBoxFilter_MouseWheelMoveUp(object sender, EventArgs e)
        {
            SendMessage2(logTextBox.Handle, EM_LINESCROLL, 0, -SettingManager.Data.ScrollLines);
        }

        /// <summary>
        /// �W���O�z�C�[�����������ɓ������Ƃ�
        /// </summary>
        void m_inputBoxFilter_MouseWheelMoveDown(object sender, EventArgs e)
        {
            SendMessage2(logTextBox.Handle, EM_LINESCROLL, 0, SettingManager.Data.ScrollLines);
        }

        /// <summary>
        /// ���̓G���A�ŃL�[�������ꂽ�Ƃ�
        /// </summary>
        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Xcrawl �̃X�N���[���L�[�R�[�h�̏ꍇ
            if (e.KeyValue == XCRAWL_KEYCODE)
            {
                m_scrollFlag = true;
                e.Handled = true;
            }

            // ���͒��͏������s��Ȃ�
            if (m_inputBoxFilter.Conpositioning)
            {
                return;
            }

            // Control �L�[��������Ă���ꍇ
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Right:
                        e.Handled = ProcessKeyOperation(SettingManager.Data.CtrlRightKeyOperation);
                        break;
                    case Keys.Left:
                        e.Handled = ProcessKeyOperation(SettingManager.Data.CtrlLeftKeyOperation);
                        break;
                    case Keys.Up:
                        e.Handled = ProcessKeyOperation(SettingManager.Data.CtrlUpKeyOperation);
                        break;
                    case Keys.Down:
                        e.Handled = ProcessKeyOperation(SettingManager.Data.CtrlDownKeyOperation);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Right:
                        if ((inputTextBox.Text == "")
                            || (inputTextBox.SelectionStart == inputTextBox.TextLength))
                        {
                            // �s���Ŋm�肵�A�����ϊ����J���Ă����ꍇ��1�񖳌��ɂ���
                            if (m_rightFilterling)
                            {
                                e.Handled = true;
                            }
                            else
                            {
                                e.Handled = ProcessKeyOperation(SettingManager.Data.RightKeyOperation);
                            }
                        }
                        break;
                    case Keys.Left:
                        if ((inputTextBox.Text == "")
                            || (inputTextBox.SelectionStart == 0))
                        {
                            e.Handled = ProcessKeyOperation(SettingManager.Data.LeftKeyOperation);
                        }
                        break;
                    case Keys.Up:
                    case Keys.Down:
                        // Xcrawl �̔���̂��߁A�C�x���g������KeyUp�ɂ܂�����
                        e.Handled = true;
                        break;
                    default:
                        break;
                }
            }

            // �E�L�[�t�B���^�����O�I��
            m_rightFilterling = false;
        }

        /// <summary>
        /// ���̓G���A�ŃL�[�������ꂽ�Ƃ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inputTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            // Xcrawl �̃X�N���[���L�[�R�[�h�̏ꍇ
            if (e.KeyValue == XCRAWL_KEYCODE)
            {
                m_scrollFlag = false;
                e.Handled = true;
                return;
            }

            // ���͒��͏������s��Ȃ�
            if (m_inputBoxFilter.Conpositioning)
            {
                return;
            }

            // Control �L�[��������Ă��Ȃ��ꍇ
            if (!e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        if (m_scrollFlag)
                        {
                            System.Diagnostics.Debug.WriteLine("Xcrawl Up");
                            SendMessage2(logTextBox.Handle, EM_LINESCROLL, 0, -SettingManager.Data.ScrollLines);
                            e.Handled = true;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Up");
                            if (!ProcessKeyOperation(SettingManager.Data.UpKeyOperation))
                            {
                                // �f�t�H���g�L�[����̃G�~�����[�V����
                                inputTextBox.SelectionStart = 0;
                            }
                            e.Handled = true;
                        }
                        break;
                    case Keys.Down:
                        if (m_scrollFlag)
                        {
                            System.Diagnostics.Debug.WriteLine("Xcrawl Down");
                            SendMessage2(logTextBox.Handle, EM_LINESCROLL, 0, SettingManager.Data.ScrollLines);
                            e.Handled = true;
                        }
                        else
                        {
                            if (!ProcessKeyOperation(SettingManager.Data.DownKeyOperation))
                            {
                                // �f�t�H���g�L�[����̃G�~�����[�V����
                                inputTextBox.SelectionStart = inputTextBox.Text.Length;
                            }
                            e.Handled = true;
                        }
                        break;
                    case Keys.Return:
                        e.Handled = true;
                        break;
                    default:
                        break;
                }
            }
        }
        
        #endregion

        #region ���O�{�b�N�X �C�x���g

        /// <summary>
        /// �^�b�v�������ꂽ�Ƃ��̃C�x���g
        /// </summary>
        void m_logBoxFilter_TapUp(object sender, EventArgs e)
        {
            Match m = UrlRegex.Match(logTextBox.SelectedText);
            if ((m.Success) && (m.Index == 0) && (m.Length == logTextBox.SelectedText.Trim().Length))
            {
                OpenUrl(m.Value);
            }
        }

        /// <summary>
        /// ���T�C�Y���ꂽ�Ƃ��̃C�x���g
        /// </summary>
        void m_logBoxFilter_Resize(object sender, EventArgs e)
        {
            logTextBox.SelectionStart = logTextBox.TextLength;
            logTextBox.ScrollToCaret();
        }

        #endregion

        #region IRC�C�x���g

        /// <summary>
        /// �ڑ������Ƃ�
        /// </summary>
        private void ircClient_Connected(object sender, EventArgs e)
        {
            // ���ׂẴ`�����l���Ƀ��O��ǉ��E�f�t�H���g�`�����l���Ȃ�ڑ�
            BroadcastLog(Resources.Connected);
            CommitStoredLog();
            SetConnectionMenuText();
        }

        /// <summary>
        /// �ڑ����s�����Ƃ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ircClient_ConnectionFailed(object sender, EventArgs e)
        {
            MessageBox.Show(Resources.CannotConnectMessage, Resources.ConnectionError, 
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            SetDisconnected();
        }

        /// <summary>
        /// �ؒf�����Ƃ�
        /// </summary>
        private void ircClient_Disconnected(object sender, EventArgs e)
        {
            // ���ׂẴ`�����l���Ƀ��O��ǉ��EJoin �� False ��
            AddLog(m_serverCh, Resources.Disconnected);
            foreach (Channel channel in m_channel.Values)
            {
                // ���O�ǉ�
                AddLog(channel, Resources.Disconnected);

                // Join��Ԃ�����
                channel.IsJoin = false;
            }
            CommitStoredLog();
            SetDisconnected();
        }

        /// <summary>
        /// ���O���b�Z�[�W�̏������͂��܂�Ƃ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ircClient_StartMessageEvents(object sender, EventArgs e)
        {
            BeginStoreLog();
        }

        /// <summary>
        /// ���O���b�Z�[�W�̏������I������Ƃ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ircClient_FinishMessageEvents(object sender, EventArgs e)
        {
            CommitStoredLog();
        }

        /// <summary>
        /// �ڑ����������������Ƃ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ircClient_ProcessedConnection(object sender, EventArgs e)
        {
            // �f�t�H���g�`�����l���ɐڑ�
            AddLog(m_serverCh, Resources.Connected);
            foreach (Channel channel in m_channel.Values)
            {
                // ���O�ǉ�
                AddLog(channel, Resources.Connected);

                // �f�t�H���g�`�����l���Ȃ�ڑ�
                if (channel.IsDefaultChannel)
                {
                    ircClient.JoinChannel(channel.Name);

                    // �f�t�H���g�`�����l���I��ݒ�ON�̂Ƃ��́A�I������
                    if (SettingManager.Data.SelectChannelAtConnect
                        && (SettingManager.Data.Profiles.ActiveProfile.DefaultChannels.Length > 0)
                        && (channel.Name == SettingManager.Data.Profiles.ActiveProfile.DefaultChannels[0]))
                    {
                        LoadChannel(channel);
                    }
                }
            }
        }

        /// <summary>
        /// �����̃j�b�N�l�[�����ύX���ꂽ�Ƃ�
        /// </summary>
        private void ircClient_ChangeMyNickname(object sender, NickNameChangeEventArgs e)
        {
            ircClient_ChangedNickname(sender, e);
        }

        /// <summary>
        /// �N���̃j�b�N�l�[�����ύX���ꂽ�Ƃ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ircClient_ChangedNickname(object sender, NickNameChangeEventArgs e)
        {
            List<string> list;

            // priv�p�`�����l���ŁA�g�[�N���Ă�l�̖��O����������ꍇ
            if (m_channel.ContainsKey(IRCClient.GetUserName(e.Before)))
            {
                // �`�����l���ړ�
                Channel talkch = m_channel[IRCClient.GetUserName(e.Before)];
                talkch.Name = IRCClient.GetUserName(e.After);
                m_channel.Add(IRCClient.GetUserName(e.After), talkch);
                m_channel.Remove(IRCClient.GetUserName(e.Before));

                // ���݂̃`�����l����������A�t�H�[���̏�Ԃ��X�V����
                if (m_currentCh == talkch)
                    LoadChannel(m_currentCh);

                // �`�����l���ύX��ʒm
                AddLog(talkch, string.Format(Resources.NicknameChangedMessage, IRCClient.GetUserName(e.Before), IRCClient.GetUserName(e.After)));
            }

            // �S�`�����l���ɂ���
            foreach (Channel ch in m_channel.Values)
            {
                list = new List<string>(ch.Members);
                // �����������
                if (list.Contains(IRCClient.GetUserName(e.Before)))
                {
                    // �����̖��O��ύX����
                    int index = list.IndexOf(IRCClient.GetUserName(e.Before));
                    list[index] = e.After;
                    ch.Members = list.ToArray();
                    // ���݂̃`�����l����������A�t�H�[���̏�Ԃ��X�V����
                    if (m_currentCh == ch)
                        LoadChannel(m_currentCh);
                    // ���O�ǉ�
                    AddLog(ch, string.Format(Resources.NicknameChangedMessage, IRCClient.GetUserName(e.Before), IRCClient.GetUserName(e.After)));
                }
                else if (list.Contains("@" + IRCClient.GetUserName(e.Before)))
                {
                    // �����̖��O��ύX����
                    int index = list.IndexOf("@" + IRCClient.GetUserName(e.Before));
                    list[index] = "@" + e.After;
                    ch.Members = list.ToArray();
                    // ���݂̃`�����l����������A�t�H�[���̏�Ԃ��X�V����
                    if (m_currentCh == ch)
                        LoadChannel(m_currentCh);
                    // ���O�ǉ�
                    AddLog(ch, string.Format(Resources.NicknameChangedMessage, IRCClient.GetUserName(e.Before), IRCClient.GetUserName(e.After)));
                }
            }
        }

        private void SetDisconnected()
        {
            SetConnectionMenuText();
            LoadChannel(m_currentCh);

            // �_�C�A���A�b�v����
            if (SettingManager.Data.UseNetworkControl)
            {
                ConnectionManager.ReleaseAll();
            }
        }

        /// <summary>
        /// �R��ꂽ�Ƃ�
        /// </summary>
        private void ircClient_Kick(object sender, EbiSoft.EbIRC.IRC.KickEventArgs e)
        {
            // ���݂��Ȃ��`�����l���̂Ƃ��͒ǉ�
            if (!m_channel.ContainsKey(e.Channel))
            {
                /*
                ���Ȃ��Ȃ�񂾂��炷��K�v�Ȃ�
                AddChannel(e.Channel, false);
                m_channel[e.Channel].IsJoin = true;
                */ 
                return;
            }

            // ���O�ǉ�
            AddLog(m_channel[e.Channel], string.Format(Resources.KickedMessage, e.User, e.Target));

            // �R��ꂽ�̂������̂Ƃ�
            if (e.Target == ircClient.User.NickName)
            {
                // �ގ�����
                m_channel[e.Channel].IsJoin = false;
                AddLog(m_channel[e.Channel], Resources.KickedMeMessage);
                // ���݂̃`�����l����������A�t�H�[���̏�Ԃ��X�V����
                if (m_currentCh == m_channel[e.Channel])
                    LoadChannel(m_currentCh);
            }
            // �R��ꂽ�̂����l�̂Ƃ�
            else
            {
                List<string> list = new List<string>(m_channel[e.Channel].Members);
                // �`�����l�����疼�O���폜����
                list.Remove(e.Target);
                list.Remove("@" + e.Target);
                m_channel[e.Channel].Members = list.ToArray();
                // ���݂̃`�����l����������A�t�H�[���̏�Ԃ��X�V����
                if (m_currentCh == m_channel[e.Channel])
                    LoadChannel(m_currentCh);    
            }
        }

        /// <summary>
        /// CTCP�N�G����M�����Ƃ�
        /// </summary>
        private void ircClient_ReceiveCtcpQuery(object sender, CtcpEventArgs e)
        {
            switch (e.Command.ToUpper())
            {
                case "VERSION":
                    e.Reply = Resources.CtcpVersion;
                    break;
                case "SOURCE":
                    e.Reply = Resources.CtcpSource;
                    break;
                case "PING":
                    e.Reply = e.Parameter;
                    break;
                case "TIME":
                    e.Reply = System.DateTime.Now.ToString();
                    break;
                case "CLIENTINFO":
                    e.Reply = "CLIENTINFO VERSION SOURCE PING TIME";
                    break;
            }
        }

        /// <summary>
        /// CTCP���v���C��M�����Ƃ�
        /// </summary>
        private void ircClient_ReceiveCtcpReply(object sender, EbiSoft.EbIRC.IRC.CtcpEventArgs e)
        {
            AddLog(m_serverCh, string.Format(Resources.CtcpReplyMessage, IRCClient.GetUserName(e.Sender), e.Command, e.Reply));
        }

        /// <summary>
        /// ���b�Z�[�W��M
        /// </summary>
        private void ircClient_ReceiveMessage(object sender, EbiSoft.EbIRC.IRC.ReceiveMessageEventArgs e)
        {
            // privmsg �Ή�
            string channel;
            if (IRCClient.IsChannelString(e.Receiver))
            {
                channel = e.Receiver;
            }
            else
            {
                if (IRCClient.GetUserName(e.Sender) == ircClient.User.NickName)
                {
                    channel = IRCClient.GetUserName(e.Receiver);
                }
                else
                {
                    channel = IRCClient.GetUserName(e.Sender);
                }
            }

            // ���݂��Ȃ��`�����l���̂Ƃ��͒ǉ�
            if (!m_channel.ContainsKey(channel))
            {
                AddChannel(channel, false);
                m_channel[channel].IsJoin = true;
            }

            // �����L�[���[�h�t�B���^
            if ((dislikeMatcher != null) && dislikeMatcher.Match(e.Message).Success) 
                return;

            // ���O�ǉ�
            string message = AddLog(m_channel[channel], string.Format(Resources.PrivmsgLogFormat, IRCClient.GetUserName(e.Sender), e.Message));

            // ���������Z
            if (m_channel[channel] != m_currentCh)
            {
                m_channel[channel].UnreadCount++;
            }

            // �n�C���C�g�L�[���[�h�t�B���^
            if ((highlightMatcher != null) && highlightMatcher.Match(e.Message).Success)
                SetHighlight(m_channel[channel], message);
        }

        /// <summary>
        /// ���O�ꗗ�̎�M
        /// </summary>
        private void ircClient_ReceiveNames(object sender, EbiSoft.EbIRC.IRC.ReceiveNamesEventArgs e)
        {
            // �`�����l���̂Ƃ�
            if (IRCClient.IsChannelString(e.Channel))
            {
                // ���݂��Ȃ��`�����l���̂Ƃ��͒ǉ�
                if (!m_channel.ContainsKey(e.Channel))
                {
                    AddChannel(e.Channel, false);
                    m_channel[e.Channel].IsJoin = true;
                }

                // ���O�ǉ�
                AddLog(m_channel[e.Channel], string.Format(Resources.UsersLogFormat, string.Join(", ", e.Names)));

                // ���O�ꗗ�X�V
                m_channel[e.Channel].Members = e.Names;
                // ���݂̃`�����l����������A�t�H�[���̏�Ԃ��X�V����
                if (m_currentCh == m_channel[e.Channel])
                    LoadChannel(m_currentCh);
            }
        }

        /// <summary>
        /// Notice ��M
        /// </summary>
        private void ircClient_ReceiveNotice(object sender, EbiSoft.EbIRC.IRC.ReceiveMessageEventArgs e)
        {
            // privmsg �Ή�
            string channel;
            if (IRCClient.IsChannelString(e.Receiver))
            {
                channel = e.Receiver;
            }
            else
            {
                if (IRCClient.GetUserName(e.Sender) == ircClient.User.NickName)
                {
                    channel = IRCClient.GetUserName(e.Receiver);
                }
                else
                {
                    channel = IRCClient.GetUserName(e.Sender);
                }
            }
            
            // ���M�҂��Z�b�g����Ă���ꍇ
            if (channel != string.Empty)
            {
                // ���݂��Ȃ��`�����l���̂Ƃ��͒ǉ�
                if (!m_channel.ContainsKey(channel))
                {
                    AddChannel(channel, false);
                    m_channel[channel].IsJoin = true;
                    return;
                }

                // �����L�[���[�h�t�B���^
                if ((dislikeMatcher != null) && dislikeMatcher.Match(e.Message).Success)
                    return;

                // �n�C���C�g�L�[���[�h�t�B���^
                //if ((highlightMatcher != null) && highlightMatcher.Match(e.Message).Success)
                //    SetHighlight(m_channel[channel]);
                
                // ���O�ǉ�
                AddLog(m_channel[channel], string.Format(Resources.NoticeLogFormat, IRCClient.GetUserName(e.Sender), e.Message));
            }
            // �T�[�o�[�̂Ƃ�
            else
            {
                // ���O�ǉ�
                AddLog(m_serverCh, string.Format(Resources.NoticeLogFormat, IRCClient.GetUserName(e.Sender), e.Message));
            }
        }

        /// <summary>
        /// �T�[�o�[���b�Z�[�W��M
        /// </summary>
        private void ircClient_ReceiveServerReply(object sender, EbiSoft.EbIRC.IRC.ReceiveServerReplyEventArgs e)
        {
            AddLog(m_serverCh, 
                    string.Format(Resources.NumericReplyLogFormat, (int) e.Number, string.Join(" ", e.Parameter) )
            );
        }

        /// <summary>
        /// ���[�U�[�̏o����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ircClient_UserInOut(object sender, EbiSoft.EbIRC.IRC.UserInOutEventArgs e)
        {
            // �����̂Ƃ�
            if (e.User == ircClient.UserString)
            {
                switch (e.Command)
                {
                    case InOutCommands.Join:
                        // ���݂��Ȃ��`�����l���̂Ƃ��͒ǉ�
                        if ((e.Channel != string.Empty)
                            && !m_channel.ContainsKey(e.Channel))
                        {
                            AddChannel(e.Channel, false);
                        }
                        // �Q���t���O�ύX
                        m_channel[e.Channel].IsJoin = true;
                        // ���O�ǉ�
                        AddLog(m_channel[e.Channel], string.Format(Resources.Joined, IRCClient.GetUserName(e.User)));
                        break;
                    case InOutCommands.Leave:
                        if (m_channel.ContainsKey(e.Channel))
                        {
                            // �Q���t���O�ύX
                            m_channel[e.Channel].IsJoin = false;
                            // ���O�ǉ�
                            AddLog(m_channel[e.Channel], string.Format(Resources.Leaved, IRCClient.GetUserName(e.User)));
                        }
                        break;
                }
            }
            // ���l�̂Ƃ�
            else
            {
                List<string> list;
                switch (e.Command)
                {
                    case InOutCommands.Join:
                        list = new List<string>(m_channel[e.Channel].Members);
                        // �`�����l���ɖ��O��ǉ�����
                        list.Add(IRCClient.GetUserName(e.User));
                        m_channel[e.Channel].Members = list.ToArray();
                        // ���݂̃`�����l����������A�t�H�[���̏�Ԃ��X�V����
                        if (m_currentCh == m_channel[e.Channel])
                            LoadChannel(m_currentCh);
                        // ���O�ǉ�
                        AddLog(m_channel[e.Channel], string.Format(Resources.JoinedUser, IRCClient.GetUserName(e.User)));
                        break;
                    case InOutCommands.Leave:
                        list = new List<string>(m_channel[e.Channel].Members);
                        // �`�����l�����疼�O���폜����
                        list.Remove(IRCClient.GetUserName(e.User));
                        list.Remove("@" + IRCClient.GetUserName(e.User));
                        m_channel[e.Channel].Members = list.ToArray();
                        // ���݂̃`�����l����������A�t�H�[���̏�Ԃ��X�V����
                        if (m_currentCh == m_channel[e.Channel])
                            LoadChannel(m_currentCh);    
                        // ���O�ǉ�
                        AddLog(m_channel[e.Channel], string.Format(Resources.LeavedUser, IRCClient.GetUserName(e.User)));
                        break;
                    case InOutCommands.Quit:
                        // �S�`�����l���ɂ���
                        foreach (Channel ch in m_channel.Values)
                        {
                            list = new List<string>(ch.Members);
                            // �����������
                            if (list.Contains(IRCClient.GetUserName(e.User))
                                || list.Contains("@" + IRCClient.GetUserName(e.User)))
                            {
                                // �`�����l�����疼�O���폜����
                                list.Remove(IRCClient.GetUserName(e.User));
                                list.Remove("@" + IRCClient.GetUserName(e.User));
                                ch.Members = list.ToArray();
                                // ���݂̃`�����l����������A�t�H�[���̏�Ԃ��X�V����
                                if (m_currentCh == ch)
                                    LoadChannel(m_currentCh);
                                // ���O�ǉ�
                                AddLog(ch, string.Format(Resources.DisconnectedUser, IRCClient.GetUserName(e.User)));
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// �g�s�b�N�ύX
        /// </summary>
        void ircClient_TopicChange(object sender, TopicChangeEventArgs e)
        {
            /*
            // ���݂��Ȃ��`�����l���̂Ƃ��͒ǉ�
            if (!m_channel.ContainsKey(e.Channel))
            {
                AddChannel(e.Channel, false);
                m_channel[e.Channel].IsJoin = true;
            }
            */
            if (!m_channel.ContainsKey(e.Channel)) return;

            // �g�s�b�N�ύX        
            m_channel[e.Channel].Topic = e.Topic;
            // ���݂̃`�����l����������A�t�H�[���̏�Ԃ��X�V����
            if (m_currentCh == m_channel[e.Channel])
                LoadChannel(m_currentCh);

            // ���O���o�͂���
            if (e.Sender != string.Empty)
            {
                AddLog(m_channel[e.Channel], string.Format(Resources.TopicChanged, IRCClient.GetUserName(e.Sender), e.Topic));
            }
            else
            {
                AddLog(m_channel[e.Channel], string.Format(Resources.TopicReceived, e.Topic));
            }
        }

        /// <summary>
        /// ���[�h�ύX
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ircClient_ModeChange(object sender, ModeChangeEventArgs e)
        {
            // �`�����l�����Ȃ���Δ�����
            if (!m_channel.ContainsKey(e.Channel))
                return;

            // ���O���o�͂���
            if (e.Sender != string.Empty)
            {
                AddLog(m_channel[e.Channel], string.Format(Resources.ModeChanged, IRCClient.GetUserName(e.Sender), e.Mode, string.Join(",", e.Target)));
            }
            else
            {
                AddLog(m_channel[e.Channel], string.Format(Resources.ModeReceived, e.Mode, string.Join(",", e.Target)));
            }
        }

        #endregion

        #region �`�����l���ǉ�

        /// <summary>
        /// �f�t�H���g���j���[��ǂݍ���
        /// </summary>
        void SetDefaultChannel()
        {
            // ���łɃ`�����l�����X�g�ɂ���`�����l���̃f�t�H���g�`�F�b�N���X�V
            foreach (Channel channel in m_channel.Values)
            {
                // ��x�f�t�H���g���͂���
                channel.IsDefaultChannel = false;
                // �f�t�H���g���X�g�𑖍�����
                foreach (string ch in SettingManager.Data.Profiles.ActiveProfile.DefaultChannels)
                {
                    // �f�t�H���g�`�����l����������true�ɂ��Ĕ�����
                    if (channel.Name == ch)
                    {
                        channel.IsDefaultChannel = true;
                        break;
                    }
                }
            }

            // �V�����ǉ����ꂽ�`�����l����ǉ�
            foreach (string ch in SettingManager.Data.Profiles.ActiveProfile.DefaultChannels)
            {
                // �`�����l�����X�g�ɑ��݂��Ȃ��Ȃ�ǉ�����
                if (!m_channel.ContainsKey(ch))
                {
                    AddChannel(ch, true);
                }
            }
        }

        /// <summary>
        /// �`�����l����ǉ�����
        /// </summary>
        /// <param name="name">�ǉ�����`�����l����</param>
        /// <param name="defaultChannel">�f�t�H���g�`�����l�����ǂ���</param>
        /// <returns>�ǉ����ꂽ�`�����l��</returns>
        internal Channel AddChannel(string name, bool defaultChannel)
        {
            // �󕶎���Ȃ甲����
            if (string.IsNullOrEmpty(name.Trim()))
            {
                return null;
            }

            // ��؂������悤�Ƃ��ꂽ�ꍇ�͔�����
            if (name == "-")
            {
                return null;
            }

            // ���ɑ��݂��Ă���A���̃`�����l����Ԃ�
            if (m_channel.ContainsKey(name))
            {
                return m_channel[name];
            }

            Channel channel = new Channel(name, defaultChannel); // �`�����l�����쐬
            m_channel.Add(name, channel);                        // ���X�g�ɒǉ�

            // ���j���[�ւ̒ǉ��p
            ChannelMenuItem menu;

            // �`�����l���ꗗ���j���[�ɒǉ�
            menu = new ChannelMenuItem(channel);
            menu.Click += new EventHandler(menuChannelListChannelsMenuItem_Click);
            menuChannelListMenuItem.MenuItems.Add(menu); // ���j���[�ɒǉ�
            
            // �|�b�v�A�b�v���j���[�ɒǉ�
            menu = new ChannelMenuItem(channel);
            menu.Click += new EventHandler(menuChannelListChannelsMenuItem_Click);
            m_channelPopupMenus.Add(menu);
            return channel;
        }

        /// <summary>
        /// �`�����l�����폜����
        /// </summary>
        /// <param name="name">�폜����`�����l��</param>
        internal void RemoveChannel(string name)
        {
            if (!Channels.ContainsKey(name)) throw new ArgumentException();

            Channel ch = this.Channels[name];
            // ���݂̃`�����l�����폜�����Ȃ�T�[�o�[�Ɉړ�����
            if (ch == m_currentCh)
            {
                LoadChannel(m_serverCh);
            }
            // �`�����l���폜
            Channels.Remove(name);
            foreach (ChannelMenuItem item in m_channelPopupMenus)
            {
                if (item.Channel == ch)
                {
                    m_channelPopupMenus.Remove(item);
                    break;
                }
            }
            foreach (MenuItem item in menuChannelListMenuItem.MenuItems)
            {
                ChannelMenuItem channelItem = (item as ChannelMenuItem);
                if ((channelItem != null) && (channelItem.Channel == ch))
                {
                    menuChannelListMenuItem.MenuItems.Remove(item);
                    break;
                }
            }
        }

        #endregion

        #region �`�����l���ړ�

        /// <summary>
        /// ���݂̃`�����l����ύX����
        /// </summary>
        /// <param name="ch">�`�����l��</param>
        void LoadChannel(Channel channel)
        {
            // �T�[�o�[�`�����l���̂Ƃ��͓���
            if (channel == m_serverCh)
            {
                // �^�C�g���o�[�ݒ�
                this.Text = Resources.ServerMessageTitlebar;
                // �g�s�b�N�o�[�ݒ�
                topicLabel.Text = Resources.ServerMessageTopicbar;
            }
            else
            {
                // �^�C�g���o�[�ݒ�
                this.Text = string.Format(Resources.TitlebarFormat, channel.Name, channel.Members.Length);
                // �g�s�b�N�o�[�ݒ�
                topicLabel.Text = string.Format(Resources.TopicbarFormat, channel.Name, channel.Topic);
            }

            // & �� && �ɒu������(�A�N�Z�X�L�[���h�~)
            topicLabel.Text = topicLabel.Text.Replace("&", "&&");

            // �`�����l�����ύX�����Ƃ��̂݃��O��ǂݍ���
            if (m_currentCh != channel)
            {
                ClearStoredLog();
                IntPtr eventMask = IntPtr.Zero;
                try
                {
                    // Stop redrawing:
                    SendMessage(logTextBox.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
                    // Stop sending of events:
                    eventMask = SendMessage(logTextBox.Handle, EM_GETEVENTMASK, 0, IntPtr.Zero);

                    logTextBox.Text = channel.GetLogs();
                    LogMoveLastLine();

                    // �������N���A
                    channel.UnreadCount = 0;
                }
                finally
                {
                    // turn on events
                    SendMessage(logTextBox.Handle, EM_SETEVENTMASK, 0, eventMask);
                    // turn on redrawing
                    SendMessage(logTextBox.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
                }
            }

            // ���̃`�����l���̃n�C���C�g���b�Z�[�W������
            for (int i = (menuHilightedMessages.MenuItems.Count - 1); i >= 0; i--)
            {
                ChannelMenuItem menu = menuHilightedMessages.MenuItems[i] as ChannelMenuItem;
                if (menu.Channel == channel)
                {
                    menuHilightedMessages.MenuItems.RemoveAt(i);
                }
            }

            // �n�C���C�g�`�F�b�N����
            foreach (ChannelMenuItem menu in m_channelPopupMenus)
            {
                if (menu.Channel == channel)
                {
                    menu.Checked = false;
                    break;
                }
            }

            // �J�����g�`�����l���X�V
            m_currentCh = channel;
        }

        /// <summary>
        /// ���̃`�����l���ֈړ�
        /// </summary>
        void SwitchNextChannel()
        {
            // �`�����l�����X�g����Ȃ牽�����Ȃ�
            if (m_channel.Count == 0) return;

            // �`�����l���̃��X�g�����
            List<Channel> list = new List<Channel>(m_channel.Values);

            // ���݂̃`�����l�����T�[�o�[�Ȃ�
            if (m_currentCh == m_serverCh)
            {
                // �ŏ��̃`�����l����
                LoadChannel(list[0]);
            }
            // ���݂̃`�����l�����T�[�o�[�ȊO�Ȃ�
            else
            {
                // ���݂̃`�����l���̃C���f�b�N�X�𓾂�
                int index = list.IndexOf(m_currentCh);
                if (index == -1) return;

                // �����Ȃ�T�[�o�[�ɐ؂�ւ���
                if (index == list.Count - 1)
                {
                    LoadChannel(m_serverCh);
                }
                // �����łȂ��Ȃ�+1
                else
                {
                    LoadChannel(list[index + 1]);
                }
            }
        }

        /// <summary>
        /// �O�̃`�����l���ֈړ�
        /// </summary>
        void SwitchPrevChannel()
        {
            // �`�����l�����X�g����Ȃ牽�����Ȃ�
            if (m_channel.Count == 0) return;

            // �`�����l���̃��X�g�����
            List<Channel> list = new List<Channel>(m_channel.Values);

            // ���݂̃`�����l�����T�[�o�[�Ȃ�
            if (m_currentCh == m_serverCh)
            {
                // �Ō�̃`�����l����
                LoadChannel(list[list.Count - 1]);
            }
            // ���݂̃`�����l�����T�[�o�[�ȊO�Ȃ�
            else
            {
                // ���݂̃`�����l���̃C���f�b�N�X�𓾂�
                int index = list.IndexOf(m_currentCh);
                if (index == -1) return;

                // �擪�Ȃ�T�[�o�[�ɐ؂�ւ���
                if (index == 0)
                {
                    LoadChannel(m_serverCh);
                }
                // �擪�łȂ��Ȃ�+1
                else
                {
                    LoadChannel(list[index - 1]);
                }

            }
        }

        #endregion

        #region ���O�ǉ�

        /// <summary>
        /// ���O�̒ǉ�
        /// </summary>
        /// <param name="targetCh">�Ώۂ̃`�����l��</param>
        /// <param name="message">���b�Z�[�W</param>
        /// <returns>�ǉ��������O���b�Z�[�W</returns>
        string AddLog(Channel channel, string message)
        {
            // ���݂̎����e�L�X�g
            string time = string.Format(Resources.TimeFormat, DateTime.Now.Hour, DateTime.Now.Minute);

            // ��������X�V
            message = string.Format(Resources.LogFormat, time, message);

            // ���O��ǉ�����
            channel.AddLogs(message);

            // ���݂̃`�����l���ɏo�͂��ꂽ���̂Ȃ�A�o�͂���
            if (channel == m_currentCh)
            {
                if (m_storeFlag)
                {
                    // �~�σ��[�h�̎��͒~�ς���
                    if (m_storedLog.Length > 0)
                        m_storedLog.Append("\r\n");
                    m_storedLog.Append(message);
                }
                else
                {
                    // �~�σ��[�h�łȂ���Ώo�͂���
                    PrintLog(message);
                }
            }

            return message;
        }

        /// <summary>
        /// ���O���o�͂���
        /// </summary>
        /// <param name="message"></param>
        private void PrintLog(string message)
        {
            IntPtr eventMask = IntPtr.Zero;
            IntPtr textBoxHandle = IntPtr.Zero;
            try
            {
                textBoxHandle = logTextBox.Handle;
                if (textBoxHandle != IntPtr.Zero)
                {
                    eventMask = SendMessage(textBoxHandle, EM_GETEVENTMASK, 0, IntPtr.Zero);
                    SendMessage(textBoxHandle, EM_SETEVENTMASK, 0, IntPtr.Zero);
                    SendMessage(textBoxHandle, WM_SETREDRAW, 0, IntPtr.Zero);
                }

                logTextBox.Text += "\r\n" + message;
                LogMoveLastLine();
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                if (textBoxHandle != IntPtr.Zero)
                {
                    SendMessage(textBoxHandle, EM_SETEVENTMASK, 0, eventMask);
                    SendMessage(textBoxHandle, WM_SETREDRAW, 1, IntPtr.Zero);
                }
            }
        }


        /// <summary>
        /// ���O�{�b�N�X���ŏI�s�ֈړ�������
        /// </summary>
        private void LogMoveLastLine()
        {
            logTextBox.SelectionStart = logTextBox.TextLength;
            logTextBox.ScrollToCaret();
        }

        /// <summary>
        /// �S�`�����l���Ƀ��b�Z�[�W���o�͂��܂�
        /// </summary>
        /// <param name="message"></param>
        private void BroadcastLog(string message)
        {
            AddLog(m_serverCh, message);
            foreach (Channel channel in m_channel.Values)
            {
                // ���O�ǉ�
                AddLog(channel, message);
            }
        }

        /// <summary>
        /// ���O���b�Z�[�W�̒~�ς��J�n
        /// </summary>
        private void BeginStoreLog()
        {
            highlightFlag = false;
            highlightChannel = null;
            m_storedLog = new StringBuilder();
            m_storeFlag = true;
        }

        /// <summary>
        /// �~�ς������b�Z�[�W�𔽉f����
        /// </summary>
        private void CommitStoredLog()
        {
            if (m_storeFlag && (m_storedLog.Length > 0))
            {
                PrintLog(m_storedLog.ToString());
                ClearStoredLog();
                DoHighlight();
            }
        }

        /// <summary>
        /// �~�ς������O���N���A����
        /// </summary>
        private void ClearStoredLog()
        {
            m_storeFlag = false;
        }

        #endregion

        #region �n�C���C�g

        /// <summary>
        /// �w�肳�ꂽ�`�����l���ł̃n�C���C�g���Z�b�g���܂�
        /// </summary>
        /// <param name="channel">�n�C���C�g����`�����l��</param>
        private void SetHighlight(Channel channel, string message)
        {
            highlightFlag = true;
            highlightChannel = channel;

            // �n�C���C�g���b�Z�[�W�ꗗ�ɒǉ�
            ChannelMenuItem mesMenu = new ChannelMenuItem(channel);
            mesMenu.Text = message;
            mesMenu.Click += new EventHandler(menuChannelListChannelsMenuItem_Click);
            menuHilightedMessages.MenuItems.Add(mesMenu);

            // �Y���`�����l�����n�C���C�g�`�F�b�N����
            foreach (ChannelMenuItem menu in m_channelPopupMenus)
            {
                if (menu.Channel == channel)
                {
                    menu.Checked = true;
                    break;
                }
            }

            if (!m_storeFlag) DoHighlight();
        }

        /// <summary>
        /// �n�C���C�g���������s���܂�
        /// </summary>
        private void DoHighlight()
        {
            if (highlightFlag && (highlightChannel != null))
            {
                if ((SettingManager.Data.HighlightMethod == EbIRCHilightMethod.Vibration) || (SettingManager.Data.HighlightMethod == EbIRCHilightMethod.VibrationAndLed))
                {
                    if (Led.AvailableLed(LedType.Vibrartion))
                    {
                        Led.SetLedStatus(LedType.Vibrartion, LedStatus.On);
                        clearHighlightTimer.Enabled = true;
                    }
                }
                if ((SettingManager.Data.HighlightMethod == EbIRCHilightMethod.Led) || (SettingManager.Data.HighlightMethod == EbIRCHilightMethod.VibrationAndLed))
                {
                    if (Led.AvailableLed(LedType.Yellow))
                    {
                        Led.SetLedStatus(LedType.Yellow, LedStatus.On);
                        clearHighlightTimer.Enabled = true;
                    }
                }
                if (SettingManager.Data.HighlightChannelChange)
                {
                    LoadChannel(highlightChannel);
                }
            }
            highlightFlag = false;
            highlightChannel = null;
        }

        /// <summary>
        /// �n�C���C�g�̐ݒ���N���A���܂�
        /// </summary>
        private void ClearHighlight()
        {
            if (Led.AvailableLed(LedType.Vibrartion))
            {
                Led.SetLedStatus(LedType.Vibrartion, LedStatus.Off);
            }
            if (Led.AvailableLed(LedType.Yellow))
            {
                Led.SetLedStatus(LedType.Yellow, LedStatus.Off);
            }
            clearHighlightTimer.Enabled = false;
        }

        #endregion

        #region UI�\���R���g���[��

        /// <summary>
        /// �ڑ��{�^���̃e�L�X�g���X�V����
        /// </summary>
        void SetConnectionMenuText()
        {
            if (ircClient.Status == IRCClientStatus.Disconnected)
            {
                connectionMenuItem.Text = Resources.ConnectionMenuCaption;
            }
            else
            {
                connectionMenuItem.Text = Resources.DisconnectMenuCaption;
            }
        }

        /// <summary>
        /// UI�ݒ�𔽉f
        /// </summary>
        void UpdateUISettings()
        {
            logTextBox.Font = SettingManager.Data.GetFont();
            infomationPanel.Visible = SettingManager.Data.TopicVisible;

            // �T�u�j�b�N�l�[�����X�g���쐬 ------------------------

            MenuItem newItem;

            // ���݂̃j�b�N�l�[�����X�g���N���A
            nicknameSwitchMenuItem.MenuItems.Clear();

            // �f�t�H���g�j�b�N�l�[�����Z�b�g
            newItem = new MenuItem();
            newItem.Text = SettingManager.Data.Profiles.ActiveProfile.Nickname;
            newItem.Click += new EventHandler(nicknameSwitcher_Click);
            nicknameSwitchMenuItem.MenuItems.Add(newItem);

            // �j�b�N�l�[�����X�g��o�^
            foreach (string itemName in SettingManager.Data.SubNicknames)
            {
                if (!string.IsNullOrEmpty(itemName.Trim()))
                {
                    newItem = new MenuItem();
                    newItem.Text = itemName;
                    newItem.Click += new EventHandler(nicknameSwitcher_Click);
                    nicknameSwitchMenuItem.MenuItems.Add(newItem);
                }
            }

            // �J�X�^�����̓��j���[�𖖔��ɒǉ�
            nicknameSwitchMenuItem.MenuItems.Add(menuNicknameInputMenuItem);

            // �ڑ��L���b�V���ݒ�
            ConnectionManager.ConnectionCacheLength = SettingManager.Data.CacheConnection ? 1 : 0;

            // ����PONG
            pongTimer.Enabled = SettingManager.Data.ForcePong;

            // �L�[���[�h�����}�b�`�I�u�W�F�N�g
            highlightMatcher = SettingManager.Data.GetHighlightKeywordMatcher();
            dislikeMatcher = SettingManager.Data.GetDislikeKeywordMatcher();

            // �n�C���C�g��~�^�C�}�[�̎���
            clearHighlightTimer.Interval = SettingManager.Data.HighlightContinueTime;
        }

        #endregion

        /// <summary>
        /// ����L�[�{�[�h�I�y���[�V�������s
        /// </summary>
        /// <param name="operation">���s����L�[�{�[�h�I�y���[�V����</param>
        /// <returns>���炩�̑��삪������ true </returns>
        private bool ProcessKeyOperation(EbIRCKeyOperations operation)
        {
            switch (operation)
            {

                case EbIRCKeyOperations.PageDown:
                    SendMessage(logTextBox.Handle, EM_SCROLL, SB_PAGEDOWN, IntPtr.Zero);
                    return true;

                case EbIRCKeyOperations.PageUp:
                    SendMessage(logTextBox.Handle, EM_SCROLL, SB_PAGEUP, IntPtr.Zero);
                    return true;

                case EbIRCKeyOperations.QuickChannelNext:
                    SwitchNextChannel();
                    return true;

                case EbIRCKeyOperations.QuickChannelPrev:
                    SwitchPrevChannel();
                    return true;

                case EbIRCKeyOperations.InputLogNext:
                    // ���͈͓��̂Ƃ�
                    if (m_inputlogPtr < m_inputlog.Count)
                    {
                        // �|�C���^�ړ�
                        m_inputlogPtr++;
                    }

                    if (m_inputlogPtr == m_inputlog.Count)
                    {
                        inputTextBox.Text = string.Empty;
                    }
                    else
                    {
                        // �e�L�X�g�Z�b�g
                        inputTextBox.Text = m_inputlog[m_inputlogPtr];
                        // �S�̂�I��
                        inputTextBox.SelectAll();
                    } 
                    return true;

                case EbIRCKeyOperations.InputLogPrev:
                    // ���͈͓��̂Ƃ�
                    if (m_inputlogPtr > 0)
                    {
                        // �|�C���^�ړ�
                        m_inputlogPtr--;
                        // �e�L�X�g�Z�b�g
                        inputTextBox.Text = m_inputlog[m_inputlogPtr];
                        // �S�̂�I��
                        inputTextBox.SelectAll();
                    }
                    return true;

                case EbIRCKeyOperations.FontSizeUp:
                    SettingManager.Data.FontSize++;
                    logTextBox.Font = SettingManager.Data.GetFont();
                    return true;

                case EbIRCKeyOperations.FontSizeDown:
                    if (SettingManager.Data.FontSize > 1)
                    {
                        SettingManager.Data.FontSize--;
                        logTextBox.Font = SettingManager.Data.GetFont();
                    }
                    return true;

                case EbIRCKeyOperations.NoOperation:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// �I������Ă���URL���擾����B
        /// </summary>
        /// <returns>�J���ׂ�URL�B�Y�����Ȃ���΋󕶎���B</returns>
        private string GetSelectedURL()
        {
            // ���K�\���p�^�[��
            MatchCollection matches;  // ���K�\���}�b�`�̃��X�g
            int lineStart, lineEnd;   // �s�̊J�n�ʒu�A�I���ʒu
            string line;              // ���o�����s�AURL

            // ���O����Ȃ甲����
            if (logTextBox.TextLength == 0) return string.Empty;

            // �s�͈̔͂𒲂ׂ�
            if (logTextBox.SelectionStart == logTextBox.TextLength)
            {
                lineStart = logTextBox.Text.LastIndexOf('\r', logTextBox.SelectionStart - 1);
                lineEnd = logTextBox.TextLength;
            }
            else
            {
                lineStart = logTextBox.Text.LastIndexOf('\r', logTextBox.SelectionStart - 1);
                lineEnd = logTextBox.Text.IndexOf('\r', lineStart + 1);
            }

            // ��v���Ȃ���΁A�s���A�s���ɂ���
            if (lineStart == -1) lineStart = 0;
            if (lineEnd == -1) lineEnd = logTextBox.TextLength;

            // �s�𒊏o����
            line = logTextBox.Text.Substring(lineStart, lineEnd - lineStart);

            // �s�̒���URL��������
            matches = UrlRegex.Matches(line);

            // �Ȃ���Δ�����
            if (matches.Count == 0)
                return string.Empty;

            // �J�[�\���̉��ɂ���URL��������
            foreach (Match match in matches)
            {
                if (((match.Groups[0].Index + lineStart) <= logTextBox.SelectionStart)
                    && ((match.Groups[0].Index + match.Groups[0].Length + lineStart) >= logTextBox.SelectionStart)
                )
                {
                    // ��v�����炱����J��
                    return match.Groups[0].Value;
                }

            }

            // �݂���Ȃ�������ŏ��̂���J��
            return matches[0].Groups[0].Value;
        }

        /// <summary>
        /// URL���J��
        /// </summary>
        /// <param name="url">�J��URL</param>
        private static void OpenUrl(string url)
        {
            try
            {
                // ttp��http �ϊ�
                if (url.StartsWith("ttp:"))
                {
                    url = "h" + url;
                }

                System.Diagnostics.Process.Start(url, "");
            }
            catch (Win32Exception)
            {
                MessageBox.Show(Resources.CannotOpenURL, Resources.FaildBoot,
                    MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// PONG���M�^�C�}�[�C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pongTimer_Tick(object sender, EventArgs e)
        {
            if (ircClient.Status == IRCClientStatus.Online)
            {
                ircClient.SendCommand("PONG :" + SettingManager.Data.Profiles.ActiveProfile.Server);
            }
        }

        /// <summary>
        /// �n�C���C�g��~�^�C�}�[
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearHighlightTimer_Tick(object sender, EventArgs e)
        {
            ClearHighlight();
        }
    }
}