using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EbiSoft.EbIRC.Settings
{
    /// <summary>
    /// �ڑ��v���t�@�C��
    /// </summary>
    public class ConnectionProfile : ICloneable
    {
        /// <summary>
        /// �v���t�@�C����
        /// </summary>
        [XmlAttribute]
        public string ProfileName
        {
            get { return m_profileName; }
            set { m_profileName = value; }
        }
        private string m_profileName = string.Empty;

        /// <summary>
        /// �T�[�o�[
        /// </summary>
        public string Server
        {
            get { return m_server; }
            set { m_server = value; }
        }
        private string m_server = string.Empty;

        /// <summary>
        /// �|�[�g
        /// </summary>
        public int Port
        {
            get { return m_port; }
            set { m_port = value; }
        }
        private int m_port = 6667;

        /// <summary>
        /// SSL�̎g�p
        /// </summary>
        public bool UseSsl
        {
            get { return m_useSsl; }
            set { m_useSsl = value; }
        }
        private bool m_useSsl;

        /// <summary>
        /// SSL�̎g�p
        /// </summary>
        public bool NoValidation
        {
            get { return m_noValidation; }
            set { m_noValidation = value; }
        }
        private bool m_noValidation;

        /// <summary>
        /// �p�X���[�h
        /// </summary>
        public string Password
        {
            get { return m_password; }
            set { m_password = value; }
        }
        private string m_password = string.Empty;

        /// <summary>
        /// �j�b�N�l�[��
        /// </summary>
        public string Nickname
        {
            get { return m_nickname; }
            set { m_nickname = value; }
        }
        private string m_nickname = string.Empty;

        /// <summary>
        /// ���O
        /// </summary>
        public string Realname
        {
            get { return m_realname; }
            set { m_realname = value; }
        }
        private string m_realname = string.Empty;

        /// <summary>
        /// �G���R�[�f�B���O
        /// </summary>
        public string Encoding
        {
            get { return m_encoding; }
            set { m_encoding = value; }
        }
        private string m_encoding = Properties.Resources.DefaultEncoding;

        /// <summary>
        /// �ڑ�����JOIN����`�����l��
        /// </summary>
        [Obsolete("Channels���g�p���Ă��������B")]
        public string[] DefaultChannels
        {
            get { return m_defchannels; }
            set { m_defchannels = value; }
        }
        private string[] m_defchannels = new string[] { };

        /// <summary>
        /// �ڑ�����JOIN����`�����l��
        /// </summary>
        public ChannelSettingCollection Channels
        {
            get { return m_channels; }
            set { m_channels = value; }
        }
        private ChannelSettingCollection m_channels = new ChannelSettingCollection();

        /// <summary>
        /// ���O�C���l�[��
        /// </summary>
        public string LoginName
        {
            get { return m_loginName; }
            set { m_loginName = value; }
        }
        private string m_loginName;

        /// <summary>
        /// Nickserv �p�X���[�h
        /// </summary>
        public string NickServPassword
        {
            get { return m_nickServPassword; }
            set { m_nickServPassword = value; }
        }
        private string m_nickServPassword;


        #region �R���X�g���N�^

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="profileName">�v���t�@�C����</param>
        public ConnectionProfile(string profileName)
        {
            m_profileName = profileName;
        }

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public ConnectionProfile()
        {

        }

        #endregion

        /// <summary>
        /// �G���R�[�f�B���O���擾���܂��B
        /// </summary>
        /// <returns></returns>
        public Encoding GetEncoding()
        {
            try
            {
                if (Encoding.ToLower().Replace("-", string.Empty) == "utf8")
                {
                    return new UTF8Encoding(false);
                }
                else
                {
                    return System.Text.Encoding.GetEncoding(Encoding);
                }
            }
            catch (Exception)
            {
                return System.Text.Encoding.GetEncoding(Properties.Resources.DefaultEncoding);
            }
        }

        public override string ToString()
        {
            return ProfileName;
        }

        #region ICloneable �����o

        /// <summary>
        /// ���̃I�u�W�F�N�g�̃N���[�����쐬���܂�
        /// </summary>
        /// <returns>�쐬���ꂽ�N���[��</returns>
        public object Clone()
        {
            ConnectionProfile prof = new ConnectionProfile();
            prof.m_channels = m_channels;
            prof.m_defchannels = m_defchannels;
            prof.m_encoding = m_encoding;
            prof.m_nickname = m_nickname;
            prof.m_noValidation = m_noValidation;
            prof.m_password = m_password;
            prof.m_port = m_port;
            prof.m_profileName = m_profileName;
            prof.m_realname = m_realname;
            prof.m_server = m_server;
            prof.m_useSsl = m_useSsl;
            return prof;
        }

        #endregion
    }
}
