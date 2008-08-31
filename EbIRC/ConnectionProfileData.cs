using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EbiSoft.EbIRC.Properties;

namespace EbiSoft.EbIRC
{
    public class ConnectionProfileData
    {

        /// <summary>
        /// �v���t�@�C���̃��X�g
        /// </summary>
        [XmlElement]
        public List<ConnectionProfile> Profile
        {
            get { return m_profiles; }
            set { m_profiles = value; }
        }
        private List<ConnectionProfile> m_profiles = new List<ConnectionProfile>();

        /// <summary>
        /// �I�𒆂̃v���t�@�C���̃C���f�b�N�X
        /// </summary>
        [XmlAttribute]
        public int ActiveProfileIndex
        {
            get { return m_activeProfileIndex; }
            set {
                /*
                if ((value < 0) || (value > m_profiles.Count))
                {
                    throw new ArgumentOutOfRangeException();
                }
                else
                {
                    m_activeProfileIndex = value;
                }
                */
                m_activeProfileIndex = value;
            }
        }
        private int m_activeProfileIndex = -1;

        /// <summary>
        /// �I������Ă���v���t�@�C��
        /// </summary>
        [XmlIgnore]
        public ConnectionProfile ActiveProfile
        {
            get
            {
                if (m_profiles.Count == 0)
                {
                    m_profiles.Add(new ConnectionProfile(Resources.DefaultProfileName));
                    if (m_activeProfileIndex < 0)
                    {
                        m_activeProfileIndex = 0;
                    }
                }
                return Profile[ActiveProfileIndex];
            }
        }
	}
}
