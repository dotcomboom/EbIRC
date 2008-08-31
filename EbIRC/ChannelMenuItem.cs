using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace EbiSoft.EbIRC
{
    /// <summary>
    /// �`�����l��������MenuItem
    /// </summary>
    class ChannelMenuItem : MenuItem
    {
        private Channel m_channel;

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
                base.Text = value.Name.Replace("&", "&&");
                m_channel = value;
            }
        }
    }
}
