using System;

namespace EbiSoft.EbIRC.IRC
{
	/// <summary>
	/// ���O�ꗗ��M�C�x���g
	/// </summary>
	public class ReceiveNamesEventArgs : EventArgs {
		string[] m_names;
		string   m_channel;

        /// <summary>
        /// ���O�̈ꗗ
        /// </summary>
		public string[] Names
		{
			get{ return m_names;  }
		}

        /// <summary>
        /// �`�����l��
        /// </summary>
		public string Channel
		{
			get{ return m_channel;  }
		}

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="channel">�`�����l��</param>
        /// <param name="names">���O�̈ꗗ</param>
		public ReceiveNamesEventArgs(string channel, string[] names) {
			m_channel = channel;
			m_names   = names;
		}
	}

    /// <summary>
    /// ���O�̈ꗗ��M�C�x���g�̃f���Q�[�g
    /// </summary>
	public delegate void ReceiveNamesEventHandler(object sender, ReceiveNamesEventArgs e);
}
