using System;

namespace EbiSoft.EbIRC.IRC
{

	/// <summary>
	/// �G���[�C�x���g���
	/// </summary>
	public class ReceiveServerReplyEventArgs : EventArgs {
		ReplyNumbers m_number;
        string[] m_param;
		string m_message;

        /// <summary>
        /// ���v���C�ԍ�
        /// </summary>
		public ReplyNumbers Number
		{
			get{ return m_number;  }
		}

        /// <summary>
        /// ���b�Z�[�W
        /// </summary>
		public string Message
		{
			get{ return m_message;  }
		}

        /// <summary>
        /// �p�����[�^
        /// </summary>
        public string[] Parameter
        {
            get { return m_param; }
            set { m_param = value; }
        }

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="number">���v���C�ԍ�</param>
        /// <param name="param">�T�[�o�[����n���ꂽ�p�����[�^</param>
		public ReceiveServerReplyEventArgs(ReplyNumbers number, string[] param) {
			m_number  = number;
			m_param   = param;
			m_message = param[param.Length - 1];
		}
	}

    /// <summary>
    /// �G���[�C�x���g�̃f���Q�[�g
    /// </summary>
	public delegate void ReceiveServerReplyEventHandler(object sender, ReceiveServerReplyEventArgs e);

}
