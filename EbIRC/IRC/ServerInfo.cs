using System;
using System.Net;

namespace EbiSoft.EbIRC.IRC {
	/// <summary>
	/// �T�[�o�[���
	/// </summary>
	public struct ServerInfo {
		string m_name;
		int    m_port;
		string m_password;
        private bool m_useSsl;
        private bool m_noValidation;

        /// <summary>
        /// �T�[�o�[��
        /// </summary>
		public string Name
		{
			get{ return m_name;  }
		}

        /// <summary>
        /// �|�[�g
        /// </summary>
		public int Port
		{
			get{ return m_port;  }
		}

        /// <summary>
        /// �T�[�o�[�p�X���[�h
        /// </summary>
		public string Password
		{
			get{ return m_password;  }
		}

        /// <summary>
        /// SSL�̎g�p
        /// </summary>
        public bool UseSsl
        {
            get { return m_useSsl; }
            set { m_useSsl = value; }
        }

        /// <summary>
        /// �������؂��Ȃ�
        /// </summary>
        public bool NoValidation
        {
            get { return m_noValidation; }
            set { m_noValidation = value; }
        }

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="name">�T�[�o�[��</param>
        /// <param name="port">�|�[�g</param>
        /// <param name="password">�p�X���[�h</param>
		public ServerInfo(string name, int port, string password) : this (name,port,password,false,false)
        {
		}

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="name">�T�[�o�[��</param>
        /// <param name="port">�|�[�g</param>
        /// <param name="password">�p�X���[�h</param>
        /// <param name="ssl">SSL�̎g�p</param>
        public ServerInfo(string name, int port, string password, bool useSsl, bool noValidation)
        {
            m_name = name;
            m_port = port;
            m_password = password;
            m_useSsl = useSsl;
            m_noValidation = noValidation;
        }

        /// <summary>
        /// �ݒ肳��Ă��񂩂� IPEndPoint �����
        /// </summary>
        /// <returns>�ݒ肳�ꂽ��������IPEndPoint</returns>
		public IPEndPoint GetEndPoint()
		{
			IPHostEntry entry = Dns.GetHostEntry(m_name);
			IPAddress   addr = entry.AddressList[0];

            if (IPAddress.IsLoopback(addr))
            {
                return new IPEndPoint(IPAddress.Loopback, m_port);
            }
            else
            {
                return new IPEndPoint(addr, m_port);
            }
		}
	}
}
