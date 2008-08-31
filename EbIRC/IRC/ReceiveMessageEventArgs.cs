using System;

namespace EbiSoft.EbIRC.IRC 
{
	/// <summary>
	/// ���b�Z�[�W��M�C�x���g
	/// </summary>
	public class ReceiveMessageEventArgs : EventArgs 
	{
		string m_sender;
		string m_receiver;
		string m_message;
		
        /// <summary>
        /// ���M��
        /// </summary>
		public string Receiver
		{
			get{ return m_receiver;  }
		}

        /// <summary>
        /// ���M��
        /// </summary>
		public string Sender{
			get{ return m_sender; }
		}

        /// <summary>
        /// ���b�Z�[�W
        /// </summary>
		public string Message
		{
			get{ return m_message;  }
		}
		
        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="sender">���M��</param>
        /// <param name="receiver">���M��</param>
        /// <param name="message">���b�Z�[�W</param>
		public ReceiveMessageEventArgs(string sender, string receiver, string message) 
		{
			m_sender   = sender;
			m_receiver = receiver;
			m_message  = message;
			
		}
	}

    /// <summary>
    /// ���b�Z�[�W��M�C�x���g�̃f���Q�[�g
    /// </summary>
	public delegate void ReceiveMessageEventHandler(object sender, ReceiveMessageEventArgs e);
}
