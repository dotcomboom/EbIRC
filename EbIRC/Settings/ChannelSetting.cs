using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EbiSoft.EbIRC.Settings
{
    /// <summary>
    /// �ڑ���`�����l���̏���ێ�����N���X
    /// </summary>
    [XmlType("Channel")]
    public class ChannelSetting
    {
        private string m_name = string.Empty;
        private string m_password = string.Empty;
        private bool m_ignoreInUnreadCountSort = false;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public ChannelSetting()
        {

        }

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="name">�`�����l����</param>
        public ChannelSetting(string name)
        {
            m_name = name;
        }

        /// <summary>
        /// �`�����l���̖��O
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        /// <summary>
        /// �`�����l���p�X���[�h
        /// </summary>
        public string Password
        {
            get { return m_password; }
            set { m_password = value; }
        }

        /// <summary>
        /// ���ǐ��J�E���g�\�[�g���ɑΏۊO�ɂ���
        /// </summary>
        public bool IgnoreInUnreadCountSort
        {
            get { return m_ignoreInUnreadCountSort; }
            set { m_ignoreInUnreadCountSort = value; }
        }
	
    }
}
