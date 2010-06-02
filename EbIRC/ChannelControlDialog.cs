using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EbiSoft.EbIRC.Properties;
using EbiSoft.EbIRC.Settings;

namespace EbiSoft.EbIRC
{
    public partial class ChannelControlDialog : Form
    {
        const int SPLITTER_SIZE = 4;
        const int SCROLLBAR_MARGIN = 30;

        const int IMAGE_NORMAL_MEMBER = 0;
        const int IMAGE_OPERATOR_MEMBER = 1;
        const int IMAGE_OFFLINE_CHANNEL = 2;
        const int IMAGE_ONLINE_CHANNEL = 3;

        #region ������

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public ChannelControlDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// �t�H�[���ǂݍ��ݎ��C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelControlDialog_Load(object sender, EventArgs e)
        {
            LoadChannelList();
            openedChannelListview.Focus();
            SelectedIndexReset();
        }

        #endregion

        #region �ꗗ�쐬

        /// <summary>
        /// �ǂݍ��ݍς݃`�����l���̈ꗗ���쐬���܂��B
        /// </summary>
        private void LoadChannelList()
        {
            // �`�����l���̈ꗗ���쐬
            openedChannelListview.Items.Clear();
            addChannel((Owner as EbIrcMainForm).ServerChannel);
            foreach (Channel ch in (Owner as EbIrcMainForm).Channels.Values)
            {
                addChannel(ch);
            }
        }

        /// <summary>
        /// ���X�g�Ƀ`�����l����ǉ�����
        /// </summary>
        /// <param name="ch"></param>
        private void addChannel(Channel ch)
        {
            ListViewItem item = new ListViewItem(ch.Name);
            item.Tag = ch;
            if (ch.IsChannel)
            {
                item.ImageIndex = ch.IsJoin ? IMAGE_ONLINE_CHANNEL : IMAGE_OFFLINE_CHANNEL;
            }
            else
            {
                item.ImageIndex = IMAGE_NORMAL_MEMBER;
            }
            openedChannelListview.Items.Add(item);

            // �I������Ă����烁���o�[�ꗗ��o���ō쐬
            if (((Owner as EbIrcMainForm).CurrentChannel == ch))
            {
                item.Selected = true;
                LoadMemberList();
            }
        }

        /// <summary>
        /// �`�����l���I�����X�g�őI������Ă���`�����l���̃����o�[�ꗗ���쐬���܂��B
        /// </summary>
        private void LoadMemberList()
        {
            memberListView.Items.Clear();
            
            // �`�����l�������I���̏ꍇ�͔�����
            if (openedChannelListview.SelectedIndices.Count == 0) return;

            // �`�����l�����擾
            Channel ch = openedChannelListview.Items[openedChannelListview.SelectedIndices[0]].Tag as Channel;

            // �`�����l�����̃����o�ꗗ���쐬
            memberListView.SuspendLayout();
            List<string> members = new List<string>(ch.Members);
            members.Sort();
            foreach (string member in members)
            {
                ListViewItem item;
                if (member.StartsWith("@"))
                {
                    item = new ListViewItem(member.Substring(1));
                    item.ImageIndex = IMAGE_OPERATOR_MEMBER;
                }
                else
                {
                    item = new ListViewItem(member);
                    item.ImageIndex = IMAGE_NORMAL_MEMBER;
                }

                memberListView.Items.Add(item);
            }
            memberListView.ResumeLayout();
        }

        #endregion

        /// <summary>
        /// ���郁�j���[
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// �`�����l���ǉ����j���[
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addSenderMenuItem_Click(object sender, EventArgs e)
        {
            using (InputBoxForm form = new InputBoxForm()) {
                form.Text = Resources.ChannelAddDialogTitle;
                form.Description = Resources.ChannelAddDialogCaption;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    (Owner as EbIrcMainForm).AddChannel(form.Value, false, null);
                    LoadChannelList();
                }
            }
        }

        #region �J���Ă���`�����l��/PM �R���e�L�X�g���j���[

        /// <summary>
        /// �`�����l�� / PM �ꗗ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openedChannelContextMenu_Popup(object sender, EventArgs e)
        {
            if (openedChannelListview.SelectedIndices.Count == 0)
            {
                // �`�����l�����I������ĂȂ��ꍇ
                joinMenuItem.Enabled = false;
                leaveMenuItem.Enabled = false;
                removeChannelMenuItem.Enabled = false;
            }
            else
            {
                // �`�����l�����I������Ă���ꍇ
                Channel ch = openedChannelListview.Items[openedChannelListview.SelectedIndices[0]].Tag as Channel;
                if (ch.IsChannel)
                {
                    // �`�����l�����`�����l���̏ꍇ
                    joinMenuItem.Enabled = !ch.IsJoin;
                    leaveMenuItem.Enabled = ch.IsJoin;
                    removeChannelMenuItem.Enabled = !ch.IsDefaultChannel;
                }
                else
                {
                    // �`�����l����PM�̏ꍇ
                    joinMenuItem.Enabled = false;
                    leaveMenuItem.Enabled = false;
                    removeChannelMenuItem.Enabled = true;
                }
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void joinMenuItem_Click(object sender, EventArgs e)
        {
            // �`�����l�����I������Ă��Ȃ��ꍇ�A������
            if (openedChannelListview.SelectedIndices.Count == 0) return;
            Channel ch = openedChannelListview.Items[openedChannelListview.SelectedIndices[0]].Tag as Channel;
            if (ch.IsChannel && (!ch.IsJoin))
            {
                // �`�����l���p�X���[�h�w��ڑ�
                ChannelSetting chSetting = SettingManager.Data.Profiles.ActiveProfile.Channels.SearchChannel(ch.Name);
                if ((chSetting != null) && (!string.IsNullOrEmpty(chSetting.Password)))
                {
                    (Owner as EbIrcMainForm).IRCClient.JoinChannel(ch.Name, chSetting.Password);
                }
                else
                {
                    (Owner as EbIrcMainForm).IRCClient.JoinChannel(ch.Name, ch.Password);
                }
            }
        }

        /// <summary>
        /// �ގ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void leaveMenuItem_Click(object sender, EventArgs e)
        {
            // �`�����l�����I������Ă��Ȃ��ꍇ�A������
            if (openedChannelListview.SelectedIndices.Count == 0) return;
            Channel ch = openedChannelListview.Items[openedChannelListview.SelectedIndices[0]].Tag as Channel;
            if (ch.IsChannel && ch.IsJoin)
            {
                (Owner as EbIrcMainForm).IRCClient.LeaveChannel(ch.Name);
            }
        }

        /// <summary>
        /// ���X�g����폜����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeChannelMenuItem_Click(object sender, EventArgs e)
        {
            if (openedChannelListview.SelectedIndices.Count == 0) return;
            Channel ch = openedChannelListview.Items[openedChannelListview.SelectedIndices[0]].Tag as Channel;

            // �`�����l����Join���Ă�ꍇ�͔�����
            if (ch.IsChannel && ch.IsJoin)
            {
                (Owner as EbIrcMainForm).IRCClient.LeaveChannel(ch.Name);
            }

            // �`�����l���ꗗ����폜����
            (Owner as EbIrcMainForm).RemoveChannel(ch.Name);

            // �`�����l���ꗗ���č\�z����
            LoadChannelList();
            memberListView.Clear();
        }

        #endregion

        #region �`�����l���̎Q���� �R���e�L�X�g���j���[

        /// <summary>
        /// �`�����l���̎Q���� �R���e�L�X�g���j���[�J���Ƃ��̃C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void memberContextMenu_Popup(object sender, EventArgs e)
        {
            addPMMemberMenuItem.Enabled = (memberListView.SelectedIndices.Count != 0);
        }

        /// <summary>
        /// PM���M��ǉ����j���[
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addPMMemberMenuItem_Click(object sender, EventArgs e)
        {
            if (memberListView.SelectedIndices.Count == 0) return; 
            if (openedChannelListview.SelectedIndices.Count == 0) return;
            
            // ���ݑI�𒆂̃`�����l�����擾����
            Channel ch = openedChannelListview.Items[openedChannelListview.SelectedIndices[0]].Tag as Channel;

            // �`�����l����ǉ�����
            (Owner as EbIrcMainForm).AddChannel(memberListView.Items[memberListView.SelectedIndices[0]].Text, false, null);

            // �`�����l���ꗗ�̍č\�z
            LoadChannelList();

            // �`�����l�����đI������
            for (int i = 0; i < openedChannelListview.Items.Count; i++)
            {
                if ((openedChannelListview.Items[i].Tag as Channel) == ch)
                {
                    openedChannelListview.Items[i].Selected = true;
                    break;
                }
            }
        }

        #endregion

        #region ���T�C�Y�C�x���g

        /// <summary>
        /// �t�H�[�����T�C�Y
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelControlDialog_Resize(object sender, EventArgs e)
        {
            if (Width < Height)
            {
                // �c����
                openedChannelPanel.Dock = DockStyle.Top;
                openedChannelPanel.Height = (int)(Height / 3);
            }
            else
            {
                // ������
                openedChannelPanel.Dock = DockStyle.Left;
                openedChannelPanel.Width = (int)(Width / 2);
            }
            ListBoxImageResize();
        }

        /// <summary>
        /// �X�v���b�g�T�C�Y
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitter_Resize(object sender, EventArgs e)
        {
            ListBoxImageResize();
        }

        /// <summary>
        /// ���X�g�{�b�N�X�̃J�����̒���
        /// </summary>
        void ListBoxImageResize()
        {
            memberHeader.Width = memberListView.Width - SCROLLBAR_MARGIN;
            openedChannelHeader.Width = openedChannelListview.Width - SCROLLBAR_MARGIN;
        }

        #endregion

        /// <summary>
        /// �`�����l���I���C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openedChannelListview_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMemberList();
        }

        /// <summary>
        /// �`�����l���ꗗ�L�[����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openedChannelListview_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Left:
                case Keys.Right:
                    memberListView.Focus();
                    break;
                case Keys.Enter:
                    openedChannelContextMenu.Show(openedChannelListview, new Point(0, 0));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// �����o�[�ꗗ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void memberListView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Left:
                case Keys.Right:
                    openedChannelListview.Focus();
                    break;
                case Keys.Enter:
                    memberContextMenu.Show(memberListView, new Point(0, 0));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// �`�����l���ꗗ�Ƀt�H�[�J�X
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openedChannelListview_GotFocus(object sender, EventArgs e)
        {
            SelectedIndexReset();
        }

        /// <summary>
        /// �����o�ꗗ�Ƀt�H�[�J�X
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void memberListView_GotFocus(object sender, EventArgs e)
        {
            SelectedIndexReset();
        }

        /// <summary>
        /// �I���C���f�b�N�X��������
        /// </summary>
        private void SelectedIndexReset()
        {
            if ((openedChannelListview.Items.Count != 0))
            {
                if (openedChannelListview.SelectedIndices.Count == 0)
                {
                    openedChannelListview.Items[0].Selected = true;
                }
                openedChannelListview.Items[openedChannelListview.SelectedIndices[0]].Focused = true;
            }
            if ((memberListView.Items.Count != 0))
            {
                if (memberListView.SelectedIndices.Count == 0)
                {
                    memberListView.Items[0].Selected = true;
                }
                memberListView.Items[memberListView.SelectedIndices[0]].Focused = true;
            }
        }

        /// <summary>
        /// �I�����ꂽ�`�����l��
        /// </summary>
        internal Channel SelectedChannel
        {
            get
            {
                if (openedChannelListview.SelectedIndices.Count != 0)
                {
                    return openedChannelListview.Items[openedChannelListview.SelectedIndices[0]].Tag as Channel;
                }
                else
                {
                    return null;
                }
            }
        }
	

    }
}