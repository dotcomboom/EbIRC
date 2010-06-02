using System;

namespace EbiSoft.EbIRC.IRC {
	/// <summary>
	/// ���[�U�[���
	/// </summary>
	public class UserInfo 
	{
		string m_nickName;
		string m_realName;
        string m_loginName;
        string m_nickservPass;

        /// <summary>
        /// �j�b�N�l�[��
        /// </summary>
		public string NickName
		{
			get{ return m_nickName;  }
		}

        /// <summary>
        /// �j�b�N�l�[����ݒ肷��
        /// </summary>
        /// <param name="value">�ݒ肷��j�b�N�l�[��</param>
		internal void setNick(string value)
		{
			m_nickName = value;
		}

        /// <summary>
        /// ���O�C���l�[��
        /// </summary>
		public string RealName
		{
			get{ return m_realName;  }
		}

        /// <summary>
        /// ���O�C���l�[��
        /// </summary>
        public string LoginName
        {
            get { return m_loginName; }
        }

        /// <summary>
        /// Nickserv�p�X���[�h
        /// </summary>
        public string NickservPass
        {
            get { return m_nickservPass; }
        }

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="nickName">�j�b�N�l�[��</param>
        /// <param name="realName">���O�C���l�[��</param>
		public UserInfo(string nickName, string realName)
		{
			m_nickName = nickName;
			m_realName = realName;
		}

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="nickName">�j�b�N�l�[��</param>
        /// <param name="realName">���O�C���l�[��</param>
        public UserInfo(string nickName, string realName, string loginName, string nickServPass)
        {
            m_nickName = nickName;
            m_realName = realName;
            m_loginName = loginName;
            m_nickservPass = nickServPass;
        }
    }
}
