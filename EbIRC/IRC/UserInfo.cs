using System;

namespace EbiSoft.EbIRC.IRC {
	/// <summary>
	/// ���[�U�[���
	/// </summary>
	public class UserInfo 
	{
		string m_nickname;
		string m_realname;

        /// <summary>
        /// �j�b�N�l�[��
        /// </summary>
		public string NickName
		{
			get{ return m_nickname;  }
		}

        /// <summary>
        /// �j�b�N�l�[����ݒ肷��
        /// </summary>
        /// <param name="value">�ݒ肷��j�b�N�l�[��</param>
		internal void setNick(string value)
		{
			m_nickname = value;
		}

        /// <summary>
        /// ���O�C���l�[��
        /// </summary>
		public string RealName
		{
			get{ return m_realname;  }
		}

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="nickName">�j�b�N�l�[��</param>
        /// <param name="realName">���O�C���l�[��</param>
		public UserInfo(string nickName, string realName)
		{
			m_nickname = nickName;
			m_realname = realName;
		}
	}
}
