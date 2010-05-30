using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EbiSoft.EbIRC.Properties;

namespace EbiSoft.EbIRC.Settings
{
    public class ConnectionProfileCollection : ICloneable
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

        #region ICloneable �����o

        /// <summary>
        /// ���̃I�u�W�F�N�g�̃N���[�����쐬���܂�
        /// </summary>
        /// <returns>�쐬���ꂽ�N���[��</returns>
        public object Clone()
        {
            ConnectionProfileCollection clone = new ConnectionProfileCollection();
            clone.m_activeProfileIndex = m_activeProfileIndex;
            foreach (ConnectionProfile prof in m_profiles)
            {
                clone.m_profiles.Add((ConnectionProfile)prof.Clone());
            }
            return clone;
        }

        #endregion
    }
}
