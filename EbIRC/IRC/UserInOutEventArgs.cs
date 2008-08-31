using System;

namespace EbiSoft.EbIRC.IRC {
	/// <summary>
	/// ���[�U�[�̓��ގ��C�x���g�̃f�[�^
	/// </summary>
	public class UserInOutEventArgs : EventArgs
    {
		string        m_user;
		string        m_channel;
		InOutCommands m_command;

        /// <summary>
        /// ���ގ��������[�U�[
        /// </summary>
		public string User
		{
			get{ return m_user;  }
		}

        /// <summary>
        /// ���ގ������`�����l��
        /// </summary>
		public string Channel
		{
			get{ return m_channel;  }
		}

        /// <summary>
        /// ���ގ��C�x���g�̎��
        /// </summary>
		public InOutCommands Command
		{
			get{ return m_command;  }
		}
		
        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="user">���ގ��������[�U�[</param>
        /// <param name="command">���ގ��C�x���g�̎��</param>
        /// <param name="channel">���ގ������`�����l��</param>
		public UserInOutEventArgs(string user, InOutCommands command, string channel)
		{
			m_user    = user;
			m_command = command;
			m_channel = channel;
		}
	}

    /// <summary>
    /// ���ގ��C�x���g�̎��
    /// </summary>
	public enum InOutCommands
	{
		Join,
		Leave,
		Quit,
	}

    /// <summary>
    /// ���ގ��C�x���g�̃f���Q�[�g
    /// </summary>
	public delegate void UserInOutEventHandler(object sender, UserInOutEventArgs e);
}
