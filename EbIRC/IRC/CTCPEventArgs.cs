using System;

namespace EbiSoft.EbIRC.IRC 
{
	/// <summary>
	/// CTCP�N�G���C�x���g�̃f�[�^
	/// </summary>
	public class CtcpEventArgs : EventArgs
	{

		private string m_sender;
		private string m_command;
		private string m_param;
		private string m_reply;

        /// <summary>
        /// ���M��
        /// </summary>
		public string Sender
		{
			get{ return m_sender;  }
		}

        /// <summary>
        /// CTCP�R�}���h
        /// </summary>
		public string Command
		{
			get{ return m_command;  }
			set{ m_command = value; }
		}

        /// <summary>
        /// CTCP�R�}���h �p�����[�^
        /// </summary>
		public string Parameter
		{
			get{ return m_param;  }
		}

        /// <summary>
        /// �ԐM�f�[�^
        /// </summary>
		public string Reply
		{
			get{ return m_reply;  }
			set{ m_reply = value; }
		}

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="sender">���M��</param>
        /// <param name="command">CTCP�R�}���h</param>
        /// <param name="param">CTCP�R�}���h �p�����[�^</param>
		public CtcpEventArgs(string sender, string command, string param)
		{
			m_sender  = sender;
			m_command = command;
			m_param   = param;
			m_reply   = string.Empty;
		}
	}

    /// <summary>
    /// CTCP�N�G���C�x���g�̃f���Q�[�g
    /// </summary>
	public delegate void CtcpEventHandler(object sender, CtcpEventArgs e);
}
