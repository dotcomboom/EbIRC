using System;
using System.Text;
using System.Windows.Forms;
using EbiSoft.Library;
using System.IO;

namespace EbiSoft.EbIRC
{
    /// <summary>
    /// �`�����l��������킷�N���X
    /// </summary>
    class Channel
    {
        RingBuffer<string> m_log;
        private bool m_defaultChannel;
        private bool m_isJoin;
        private string m_name;
        private string m_topic;
        private string[] m_members;

        private static char[] escapeForFilenameTargets = "\\/:*?<>\"|".ToCharArray();

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="name">�`�����l����</param>
        /// <param name="defaultChannel">�f�t�H���g�`�����l���w��ON/OFF</param>
        public Channel(string name, bool defaultChannel)
        {
            m_log = new RingBuffer<string>(Settings.Data.MaxLogs);
            m_topic = string.Empty;
            m_name = name;
            m_defaultChannel = defaultChannel;
            m_members = new string[] { };
        }

        /// <summary>
        /// ���O����������
        /// </summary>
        /// <returns>���O</returns>
        public string GetLogs()
        {
            // �S�����s�łȂ��ĕԂ�
            return string.Join("\r\n", m_log.ToArray());
        }

        /// <summary>
        /// ���O��ǉ�����
        /// </summary>
        /// <param name="logLine">�ǉ����郍�O</param>
        public void AddLogs(string logLine)
        {
            // ���O��ǉ�����
            m_log.Add(logLine);
            if (Settings.Data.LogingEnable) {
                try
                {
                    string baseDir;
                    if (!String.IsNullOrEmpty(Settings.Data.LogDirectory)
                        && Directory.Exists(Settings.Data.LogDirectory))
                    {
                        baseDir = Path.Combine(Settings.Data.LogDirectory, "Log");
                    }
                    else
                    {
                        baseDir = Path.Combine(Program.ProgramDirectory, "Log");
                    }
                    string directory = Path.Combine(baseDir, EscapeForFilename(Name));
                    string filename = Path.Combine(directory, DateTime.Now.ToString("yyyyMMdd") + ".htm");
                    if (!Directory.Exists(baseDir)) Directory.CreateDirectory(baseDir);
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                    using (StreamWriter writer = new StreamWriter(filename, true, Encoding.Default))
                    {
                        writer.WriteLine(logLine.Replace("<", "&lt;").Replace(">", "&gt;") + "<br>");
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// ���O���N���A����
        /// </summary>
        public void ClearLog()
        {
            m_log.Clear();
        }

        private string EscapeForFilename(string str)
        {
            string returnValue = str;
            foreach (char escapeChar in escapeForFilenameTargets)
            {
                returnValue = returnValue.Replace(escapeChar, '_');
            }
            return returnValue;
        }

        /// <summary>
        /// Join ���Ă邩�ǂ���
        /// </summary>
        public bool IsJoin
        {
            get { return m_isJoin; }
            set {
                m_isJoin = value;
                if (!value)
                {
                    // �����o�[�ꗗ�N���A
                    m_members = new string[] { };
                    // �g�s�b�N�N���A
                    Topic = string.Empty;
                }
            }
        }

        /// <summary>
        /// �f�t�H���g�`�����l���ɐݒ肳��Ă邩�ǂ���
        /// </summary>
        public bool IsDefaultChannel
        {
            get { return m_defaultChannel; }
            set { m_defaultChannel = value; }
        }

        /// <summary>
        /// �`�����l����
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// �g�s�b�N
        /// </summary>
        public string Topic
        {
            get { return m_topic; }
            set { m_topic = value; }
        }

        /// <summary>
        /// �����o�[�ꗗ
        /// </summary>
        public string[] Members
        {
            get { return m_members; }
            set { m_members = value; }
        }

        /// <summary>
        /// �`�����l�����ǂ���
        /// </summary>
        public bool IsChannel
        {
            get { return IRC.IRCClient.IsChannelString(this.Name); }
        }
    }
}
