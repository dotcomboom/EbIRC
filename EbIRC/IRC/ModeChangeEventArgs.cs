using System;
using System.Collections.Generic;
using System.Text;

namespace EbiSoft.EbIRC.IRC
{
    /// <summary>
    /// ���[�h�ύX�C�x���g
    /// </summary>
    public class ModeChangeEventArgs : EventArgs
    {
        private string m_channel;
        private string[] m_target;
        private string m_mode;
        private string m_sender;

        /// <summary>
        /// �Ώۃ`�����l��
        /// </summary>
        public string Channel
        {
            get { return m_channel; }
            set { m_channel = value; }
        }

        /// <summary>
        /// �ύX�Ώ�
        /// </summary>
        public string[] Target
        {
            get { return m_target; }
            set { m_target = value; }
        }

        /// <summary>
        /// �ύX���[�h
        /// </summary>
        public string Mode
        {
            get { return m_mode; }
            set { m_mode = value; }
        }

        /// <summary>
        /// ���M��
        /// </summary>
        public string Sender
        {
            get { return m_sender; }
            set { m_sender = value; }
        }
	

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="channel">�`�����l��</param>
        /// <param name="mode">���[�h</param>
        public ModeChangeEventArgs(string sender, string channel, string mode)
            : this(sender, channel, mode, new string[] { })
        {
        }

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="channel">�`�����l��</param>
        /// <param name="mode">���[�h</param>
        /// <param name="target">�ΏۂɂȂ�l</param>
        public ModeChangeEventArgs(string sender, string channel, string mode, string[] target)
        {
            m_channel = channel;
            m_mode = mode;
            m_target = target;
            m_sender = sender;
        }
    }

    /// <summary>
    /// ���[�h�ύX�C�x���g�n���h��
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ModeChangeEventHandler(object sender, ModeChangeEventArgs e);
}
