using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EbiSoft.EbIRC.Settings;
using EbiSoft.EbIRC.Properties;

namespace EbiSoft.EbIRC
{
    public partial class ServerSettingForm : Form
    {
        private ConnectionProfile m_profie;
        private int m_lastSettingIndex = -1;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public ServerSettingForm()
        {
            InitializeComponent();
            InTheHand.WindowsMobile.Forms.TabControlHelper.EnableVisualStyle(tabControl);

            // �f�t�H���g�T�[�o�[���X�g�̓ǂݍ���
            serverComboBox.Items.Clear();
            foreach (string server in SettingManager.Data.DefaultServers)
            {
                serverComboBox.Items.Add(server);
            }

            // �G���R�[�f�B���O���X�g�̓ǂݍ���
            encodingComboBox.Items.Clear();
            foreach (string encode in Resources.EncodeList.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n'))
            {
                encodingComboBox.Items.Add(encode);
            }
        }

        #region ���̑��C�x���g

        /// <summary>
        /// �ǂݍ��݃C�x���g
        /// </summary>
        private void ServerSettingForm_Load(object sender, EventArgs e)
        {
            m_lastSettingIndex = -1;
        }

        /// <summary>
        /// ���郁�j���[
        /// </summary>
        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// SSL�`�F�b�N�̐؂�ւ�
        /// </summary>
        private void serverUseSslCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            serverSslNotValidateCheckBox.Enabled = serverUseSslCheckBox.Checked;
        }

        #endregion

        #region ���T�C�Y�C�x���g

        private void ChannelSettingForm_Resize(object sender, EventArgs e)
        {
            OnResizeAction();
        }

        void OnResizeAction()
        {
            if (inputPanel.Enabled)
            {
                containerPanel.Height = this.ClientSize.Height - inputPanel.Bounds.Height;
            }
            else
            {
                containerPanel.Height = this.ClientSize.Height;
            }
        }

        #endregion

        #region �O���ڑ��v���p�e�B

        /// <summary>
        /// �ҏW���̃v���t�@�C��
        /// </summary>
        public ConnectionProfile Profile
        {
            get
            {
                if (m_profie == null)
                {
                    m_profie = new ConnectionProfile();
                }

                // Basic
                m_profie.ProfileName = profileNameTextBox.Text;
                m_profie.Server = serverComboBox.Text;
                m_profie.Port = int.Parse(portTextBox.Text);
                m_profie.UseSsl = serverUseSslCheckBox.Checked;
                m_profie.NoValidation = serverSslNotValidateCheckBox.Checked;
                m_profie.Nickname = nicknameTextBox.Text;
                m_profie.Encoding = encodingComboBox.Text;

                // Advanced
                m_profie.Password = serverPasswordTextBox.Text;
                m_profie.LoginName = loginNameTextBox.Text;
                m_profie.Realname = realnameTextBox.Text;
                m_profie.NickServPassword = nickservPasswordTextBox.Text;

                // Channels
                ApplyChannelEdit();
                m_profie.Channels.Clear();
                foreach (ListViewItem item in channelListView.Items)
                {
                    ChannelSetting ch = (ChannelSetting)item.Tag;
                    if (!string.IsNullOrEmpty(ch.Name.Trim()))
                    {
                        m_profie.Channels.Add(ch);
                    }
                }

                return m_profie;
            }
            set
            {
                m_profie = value;

                // Basic
                profileNameTextBox.Text = m_profie.ProfileName;
                serverComboBox.Text = m_profie.Server;
                portTextBox.Text = m_profie.Port.ToString();
                serverUseSslCheckBox.Checked = m_profie.UseSsl;
                serverSslNotValidateCheckBox.Checked = m_profie.NoValidation;
                nicknameTextBox.Text = m_profie.Nickname;
                encodingComboBox.Text = m_profie.Encoding;

                // Advanced
                serverPasswordTextBox.Text = m_profie.Password;
                loginNameTextBox.Text = m_profie.LoginName;
                realnameTextBox.Text = m_profie.Realname;
                nickservPasswordTextBox.Text = m_profie.NickServPassword;

                // Channels
                channelListView.Items.Clear();
                foreach (ChannelSetting ch in m_profie.Channels)
                {
                    ListViewItem item = new ListViewItem(ch.Name);
                    item.Tag = ch;
                    channelListView.Items.Add(item);
                }
            }
        }

        #endregion

        #region �`�����l���ҏW

        /// <summary>
        /// �I������Ă���`�����l�����ύX���ꂽ�Ƃ�
        /// </summary>
        private void channelListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyChannelEdit();
            if (channelListView.SelectedIndices.Count > 0)
            {
                LoadChannelEdit();
                m_lastSettingIndex = channelListView.SelectedIndices[0];
                channelUpButton.Enabled = (channelListView.SelectedIndices[0] > 0);
                channelDownButton.Enabled = (channelListView.SelectedIndices[0] < channelListView.Items.Count);
                removeChannelButton.Enabled = true;
            }
            else
            {
                m_lastSettingIndex = -1;
                channelUpButton.Enabled = false;
                channelDownButton.Enabled = false;
                removeChannelButton.Enabled = false;
            }
        }

        /// <summary>
        /// ��Ɉړ�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void channelUpButton_Click(object sender, EventArgs e)
        {
            int cur = channelListView.SelectedIndices[0];
            int tg = cur - 1;
            SwapChannel(cur, tg);
        }

        /// <summary>
        /// ���Ɉړ�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void channelDownButton_Click(object sender, EventArgs e)
        {
            int cur = channelListView.SelectedIndices[0];
            int tg = cur + 1;
            SwapChannel(cur, tg);
        }

        /// <summary>
        /// �`�����l�������ւ���
        /// </summary>
        /// <param name="cur">�I�𒆂̃C���f�b�N�X</param>
        /// <param name="tg">����ւ��Ώۂ̃C���f�b�N�X</param>
        private void SwapChannel(int cur, int tg)
        {
            // ����ւ�
            object swapTemp;
            swapTemp = channelListView.Items[tg].Tag;
            channelListView.Items[tg].Tag = channelListView.Items[cur].Tag;
            channelListView.Items[cur].Tag = swapTemp;

            // ���O���f
            channelListView.Items[cur].Text = ((ChannelSetting)channelListView.Items[cur].Tag).Name;
            channelListView.Items[tg].Text = ((ChannelSetting)channelListView.Items[tg].Tag).Name;

            // �đI��
            m_lastSettingIndex = -1;
            channelListView.Items[tg].Selected = true;
        }

        /// <summary>
        /// �ǉ�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addChannelButton_Click(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem();
            ChannelSetting ch = new ChannelSetting();
            item.Tag = ch;
            channelListView.Items.Add(item);
            item.Selected = true;
        }

        /// <summary>
        /// �폜����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeChannelButton_Click(object sender, EventArgs e)
        {
            ApplyChannelEdit();
            m_lastSettingIndex = -1;
            channelListView.Items.RemoveAt(channelListView.SelectedIndices[0]);
        }

        /// <summary>
        /// �`�����l���̕ҏW�𔽉f����
        /// </summary>
        private void ApplyChannelEdit()
        {
            if (m_lastSettingIndex != -1)
            {
                ChannelSetting ch = (ChannelSetting) channelListView.Items[m_lastSettingIndex].Tag;
                ch.Name = channelNameTextBox.Text;
                ch.Password = channelPasswordTextBox.Text;
                ch.IgnoreInUnreadCountSort = channelIgnoreUnreadSortCheckBox.Checked;
                channelListView.Items[m_lastSettingIndex].Text = ch.Name;
            }
        }

        /// <summary>
        /// �`�����l���̕ҏW���J�n����
        /// </summary>
        private void LoadChannelEdit()
        {
            if (channelListView.SelectedIndices[0] != -1)
            {
                ChannelSetting ch = (ChannelSetting)channelListView.Items[channelListView.SelectedIndices[0]].Tag;
                channelNameTextBox.Text = ch.Name;
                channelPasswordTextBox.Text = ch.Password;
                channelIgnoreUnreadSortCheckBox.Checked = ch.IgnoreInUnreadCountSort;
            }
        }

        #endregion
    }
}