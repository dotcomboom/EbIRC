using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using EbiSoft.EbIRC.Settings;

namespace EbiSoft.EbIRC
{
    /// <summary>
    /// �`�����l��������MenuItem
    /// </summary>
    class ChannelMenuItem : MenuItem, IComparable
    {
        private Channel m_channel;
        private int m_index;

        /// <summary>
        /// �`�����l�����w�肵�ă��j���[�����������܂��B
        /// </summary>
        /// <param name="channel">���̃��j���[�ɑΉ�����`�����l��</param>
        public ChannelMenuItem(Channel channel) : base()
        {
            Channel = channel;
        }

        /// <summary>
        /// ���̃C���X�^���X�Ɋ֘A�Â����Ă��� Channel ���擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public Channel Channel
        {
            get { return m_channel; }
            set
            {
                m_channel = value;
                UpdateText();
            }
        }

        /// <summary>
        /// �\�[�g�p�̃C���f�b�N�X���擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        public void UpdateText()
        {
            if (Channel.UnreadCount > 0)
            {
                base.Text = string.Format("{0} ({1})", Channel.Name.Replace("&", "&&"), Channel.UnreadCount);
            }
            else
            {
                base.Text = Channel.Name.Replace("&", "&&");
            }
        }

        #region IComparable �����o

        public int CompareTo(object obj)
        {
            if (obj is ChannelMenuItem)
            {
                ChannelMenuItem chItem = obj as ChannelMenuItem;
                Channel ch1 = m_channel;
                Channel ch2 = chItem.Channel;

                // �n�C���C�g�D��
                if (SettingManager.Data.QuickSwitchHilightsSort)
                {
                    if ((this.Checked && ch1.IsSortTarget) && !(chItem.Checked && ch2.IsSortTarget))
                    {
                        return 1;
                    }
                    else if (!(this.Checked && ch1.IsSortTarget) && (chItem.Checked && ch2.IsSortTarget))
                    {
                        return -1;
                    }
                }
                // ���ǐ��D��
                if (SettingManager.Data.QuickSwitchUnreadCountSort)
                {
                    int count1 = ch1.IsSortTarget ? ch1.UnreadCount : 0;
                    int count2 = ch2.IsSortTarget ? ch2.UnreadCount : 0;
                    int dx = count1.CompareTo(count2);
                    if (dx != 0)
                        return dx;
                }
                return chItem.Index.CompareTo(this.Index);
            }
            else
            {
                return 0;
            }
        }

        #endregion
    }
}
