using System;
using System.Collections.Generic;
using System.Text;

namespace EbiSoft.EbIRC.IRC
{
    /// <summary>
    /// �g�s�b�N�ύX�C�x���g�̃C�x���g�f�[�^
    /// </summary>
    public class TopicChangeEventArgs : EventArgs
    {
        private string m_channel;
        private string m_topic;
        private string m_sender;

        /// <summary>
        /// �`�����l��
        /// </summary>
        public string Channel
        {
            get { return m_channel; }
            set { m_channel = value; }
        }

        /// <summary>
        /// �g�s�b�N
        /// </summary>
        public string Topic
        {
            get { return m_topic; }
            set { m_topic = value; }
        }

        /// <summary>
        /// �ύX�����l
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
        /// <param name="topic">�g�s�b�N</param>
        public TopicChangeEventArgs(string channel, string topic)
            : this(string.Empty, channel, topic)
        {
        }

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="sender">���M��</param>
        /// <param name="channel">�`�����l��</param>
        /// <param name="topic">�g�s�b�N</param>
        public TopicChangeEventArgs(string sender, string channel, string topic)
        {
            m_channel = channel;
            m_topic = topic;
            m_sender = sender;
        }
	
    }

    /// <summary>
    /// �g�s�b�N�ύX�C�x���g�̃f���Q�[�g
    /// </summary>
    public delegate void TopicChangeEventDelegate(object sender, TopicChangeEventArgs e);
}
