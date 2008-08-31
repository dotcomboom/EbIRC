using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;
using System.Text.RegularExpressions;

namespace EbiSoft.EbIRC
{
    [XmlType(Namespace="EbIRC", TypeName="Settings")]
    public class SettingData
    {
        private const string DEFAULT_FONT = "Tahoma";

        private ConnectionProfileData m_profiles = new ConnectionProfileData();
        private string[] m_defchannels = new string[] { };
        private int m_maxLog = 60;
        private string m_fontName = DEFAULT_FONT;
        private int m_fontSize = 9;
        private bool m_topicVisible = true;
        private bool m_selectChannelAtConnect = false;
        private int m_horizontalKey = 1;
        private int m_verticalKey = 2;
        private int m_horizontalKey2 = 0;
        private int m_verticalKey2 = 3;
        private string[] m_subnick = new string[] { };
        private int m_inputLogBuffer = 10;
        private bool m_confimDisconnect = false;
        private bool m_confimExit = false;
        private string[] m_defaultServers = new string[] { };
        private bool m_useNetworkControl = true;
        private bool m_cacheConnection = true;
/*
        private int m_channelControlSplitVertical = 320 / 2;
        private int m_channelControlSplitHorizonal = 240 / 2;
*/
        private bool m_reverseSoftKey = false;
        private int m_scrollLines = 5;
        private bool m_forcePong = false;
        private string[] m_dislikeKeywords = new string[] { };
        private string[] m_highlightKeywords = new string[] { };
        private bool m_regexHighlight = false;
        private bool m_regexDislike = false;
        private EbIRCHilightMethod m_highlightMethod = EbIRCHilightMethod.None;
        private bool m_highlightChannelChange = false;
        private int m_highlightContinueTime = 1500;
        private int m_channelShortcutIgnoreTimes = 400;


        /// <summary>
        /// �ڑ��v���t�@�C��
        /// </summary>
        public ConnectionProfileData Profiles
        {
            get { return m_profiles;  }
            set { m_profiles = value; }
        }

        /// <summary>
        /// �ڑ�����JOIN����`�����l��
        /// </summary>
        [Obsolete("Profiles.ActiveProfile.DefaultChannels���g�p���Ă��������B")]
        public string[] DefaultChannels
        {
            get { return m_defchannels; }
            set { m_defchannels = value; }
        }

        /// <summary>
        /// 1�`�����l��������̍ő働�O�s��
        /// </summary>
        public int MaxLogs
        {
            get { return m_maxLog; }
            set { m_maxLog = value; }
        }

        /// <summary>
        /// �t�H���g��
        /// </summary>
        public string FontName
        {
            get { return m_fontName; }
            set { m_fontName = value; }
        }

        /// <summary>
        /// �t�H���g�T�C�Y
        /// </summary>
        public int FontSize
        {
            get { return m_fontSize; }
            set { m_fontSize = value; }
        }

        /// <summary>
        /// �g�s�b�N�p�l���̕\�����
        /// </summary>
        public bool TopicVisible
        {
            get { return m_topicVisible; }
            set { m_topicVisible = value; }
        }

        /// <summary>
        /// �ڑ����Ɉ�ԏ�̃`�����l����I��
        /// </summary>
        public bool SelectChannelAtConnect
        {
            get { return m_selectChannelAtConnect; }
            set { m_selectChannelAtConnect = value; }
        }

        /// <summary>
        /// �ؒf�m�F���b�Z�[�W��\�����邩�ǂ���
        /// </summary>
        public bool ConfimDisconnect
        {
            get { return m_confimDisconnect; }
            set { m_confimDisconnect = value; }
        }

        /// <summary>
        /// �I���m�F���b�Z�[�W��\�����邩�ǂ���
        /// </summary>
        public bool ConfimExit
        {
            get { return m_confimExit; }
            set { m_confimExit = value; }
        }

        /// <summary>
        /// �����ݒ�T�[�o�[�ꗗ
        /// </summary>
        [XmlIgnore]
        public string[] DefaultServers
        {
            get { return m_defaultServers; }
            set { m_defaultServers = value; }
        }

        /// <summary>
        /// ���E�L�[�I�y���[�V����
        /// </summary>
        /// <remarks>
        /// 0:�ʏ퓮��
        /// 1:�N�C�b�N�`�����l���Z���N�g
        /// 2:�y�[�W����
        /// 3:�������O�u���E�Y
        /// 4:�t�H���g�T�C�Y�ύX
        /// 5:����Ȃ�
        /// </remarks>
        public int HorizontalKeyOperation
        {
            get { return m_horizontalKey; }
            set { m_horizontalKey = value; }
        }

        /// <summary>
        /// �㉺�L�[�I�y���[�V����
        /// </summary>
        /// <remarks>
        /// 0:�ʏ퓮��
        /// 1:�N�C�b�N�`�����l���Z���N�g
        /// 2:�y�[�W����
        /// 3:�������O�u���E�Y
        /// 4:�t�H���g�T�C�Y�ύX
        /// 5:����Ȃ�
        /// </remarks>
        public int VerticalKeyOperation
        {
            get { return m_verticalKey; }
            set { m_verticalKey = value; }
        }

        /// <summary>
        /// Ctrl+���E�L�[�I�y���[�V����
        /// </summary>
        /// <remarks>
        /// 0:�ʏ퓮��
        /// 1:�N�C�b�N�`�����l���Z���N�g
        /// 2:�y�[�W����
        /// 3:�������O�u���E�Y
        /// </remarks>
        public int HorizontalKeyWithCtrlOperation
        {
            get { return m_horizontalKey2; }
            set { m_horizontalKey2 = value; }
        }

        /// <summary>
        /// Ctrl+�㉺�L�[�I�y���[�V����
        /// </summary>
        /// <remarks>
        /// 0:�ʏ퓮��
        /// 1:�N�C�b�N�`�����l���Z���N�g
        /// 2:�y�[�W����
        /// 3:�������O�u���E�Y
        /// </remarks>
        public int VerticalKeyWithCtrlOperation
        {
            get { return m_verticalKey2; }
            set { m_verticalKey2 = value; }
        }

        /// <summary>
        /// �T�u�j�b�N�l�[�����X�g
        /// </summary>
        public string[] SubNicknames
        {
            get { return m_subnick; }
            set { m_subnick = value; }
        }

        /// <summary>
        /// ���̓��O�o�b�t�@�T�C�Y
        /// </summary>
        public int InputLogBufferSize
        {
            get { return m_inputLogBuffer; }
            set { m_inputLogBuffer = value; }
        }

        /// <summary>
        /// �l�b�g���[�N�ڑ�������g�p���邩�ǂ���
        /// </summary>
        public bool UseNetworkControl
        {
            get { return m_useNetworkControl; }
            set { m_useNetworkControl = value; }
        }

        /// <summary>
        /// �l�b�g���[�N�ڑ����L���b�V�����邩�ǂ���
        /// </summary>
        public bool CacheConnection
        {
            get { return m_cacheConnection; }
            set { m_cacheConnection = value; }
        }
/*
        /// <summary>
        /// �`�����l������_�C�A���O�c��ʎ��̕�����
        /// </summary>
        public int ChannelControlSplitHorizonal
        {
            get { return m_channelControlSplitHorizonal; }
            set { m_channelControlSplitHorizonal = value; }
        }

        /// <summary>
        /// �`�����l������_�C�A���O����ʎ��̕�����
        /// </summary>
        public int ChannelControlSplitVertical
        {
            get { return m_channelControlSplitVertical; }
            set { m_channelControlSplitVertical = value; }
        }
*/
        /// <summary>
        /// �\�t�g�L�[�̓���ւ�
        /// </summary>
        public bool ReverseSoftKey
        {
            get { return m_reverseSoftKey; }
            set { m_reverseSoftKey = value; }
        }

        /// <summary>
        /// �X�N���[������s��
        /// </summary>
        public int ScrollLines
        {
            get { return m_scrollLines; }
            set { m_scrollLines = value; }
        }

        /// <summary>
        /// ����PONG���M
        /// </summary>
        public bool ForcePong
        {
            get { return m_forcePong; }
            set { m_forcePong = value; }
        }

        /// <summary>
        /// �n�C���C�g�̃}�b�`���O�L�[���[�h
        /// </summary>
        public string[] HighlightKeywords
        {
            get { return m_highlightKeywords; }
            set { m_highlightKeywords = value; }
        }

        /// <summary>
        /// �n�C���C�g�̃}�b�`���O�Ő��K�\�����g�p����
        /// </summary>
        public bool UseRegexHighlight
        {
            get { return m_regexHighlight; }
            set { m_regexHighlight = value; }
        }

        /// <summary>
        /// �n�C���C�g���
        /// </summary>
        public EbIRCHilightMethod HighlightMethod
        {
            get { return m_highlightMethod; }
            set { m_highlightMethod = value; }
        }

        /// <summary>
        /// �n�C���C�g���`�����l���؂�ւ�
        /// </summary>
        public bool HighlightChannelChange
        {
            get { return m_highlightChannelChange; }
            set { m_highlightChannelChange = value; }
        }

        /// <summary>
        /// �n�C���C�g�p������(ms)
        /// </summary>
        public int HighlightContinueTime
        {
            get { return m_highlightContinueTime; }
            set { m_highlightContinueTime = value; }
        }
	

        /// <summary>
        /// �����L�[���[�h
        /// </summary>
        public string[] DislikeKeywords
        {
            get { return m_dislikeKeywords; }
            set { m_dislikeKeywords = value; }
        }

        /// <summary>
        /// �������[�h�̃}�b�`���O�Ő��K�\�����g�p����
        /// </summary>
        public bool UseRegexDislike
        {
            get { return m_regexDislike; }
            set { m_regexDislike = value; }
        }

        /// <summary>
        /// �Ō�̔�����A��ł��V���[�g�J�b�g�������Ȏ���(ms)
        /// </summary>
        public int ChannelShortcutIgnoreTimes
        {
            get { return m_channelShortcutIgnoreTimes; }
            set { m_channelShortcutIgnoreTimes = value; }
        }
	

        #region �ݒ肩��f�[�^���쐬���郁�\�b�h�ƃv���p�e�B

        /// <summary>
        /// �t�H���g���擾����
        /// </summary>
        /// <returns>�ݒ肩����ꂽ�t�H���g</returns>
        public Font GetFont()
        {
            try
            {
                return new Font(FontName, (float)FontSize, FontStyle.Regular);
            }
            catch (Exception)
            {
                return new Font(FontFamily.GenericMonospace, 9, FontStyle.Regular);
            }
        }

        /// <summary>
        /// �n�C���C�g�L�[���[�h�Ƀ}�b�`���鐳�K�\���N���X���쐬���܂�
        /// </summary>
        /// <returns>�w�肳�ꂽ�L�[���[�h�Ƀ}�b�`���鐳�K�\���B�`�����������Ȃ���� null</returns>
        public Regex GetHighlightKeywordMatcher()
        {
            return GetKeywordMatcher(HighlightKeywords, UseRegexHighlight);
        }

        /// <summary>
        /// �����L�[���[�h�Ƀ}�b�`���鐳�K�\���N���X���쐬���܂�
        /// </summary>
        /// <returns>�w�肳�ꂽ�L�[���[�h�Ƀ}�b�`���鐳�K�\���B�`�����������Ȃ���� null</returns>
        public Regex GetDislikeKeywordMatcher()
        {
            return GetKeywordMatcher(DislikeKeywords, UseRegexDislike);
        }

        /// <summary>
        /// �L�[���[�h�}�b�`�p�̐��K�\���N���X���쐬���܂�
        /// </summary>
        /// <param name="keywords">�L�[���[�h</param>
        /// <param name="useRegex">���K�\�����g�p���邩�ǂ���</param>
        /// <returns>�w�肳�ꂽ�L�[���[�h�Ƀ}�b�`���鐳�K�\���B�`�����������Ȃ���� null</returns>
        private Regex GetKeywordMatcher(string[] keywords, bool useRegex)
        {
            if (keywords == null) return null;
            if (keywords.Length == 0) return null;
            if ((keywords.Length == 1) && (string.IsNullOrEmpty(keywords[0]))) return null;

            if (useRegex)
            {
                return new Regex(keywords[0]);
            }
            else
            {
                List<string> escapedKeywords = new List<string>();
                foreach (string keyword in keywords)
                {
                    if (!string.IsNullOrEmpty(keyword)) 
                        escapedKeywords.Add(keyword);
                }
                return new Regex(string.Join("|", escapedKeywords.ToArray()));
            }
        }

        /// <summary>
        /// ��L�[�̃I�y���[�V����
        /// </summary>
        [XmlIgnore]
        public EbIRCKeyOperations UpKeyOperation
        {
            get
            {
                switch (VerticalKeyOperation)
                {
                    case 1:
                        return EbIRCKeyOperations.QuickChannelPrev;

                    case 2:
                        return EbIRCKeyOperations.PageUp;

                    case 3:
                        return EbIRCKeyOperations.InputLogPrev;

                    case 4:
                        return EbIRCKeyOperations.FontSizeUp;

                    case 5:
                        return EbIRCKeyOperations.NoOperation;

                    default:
                        return EbIRCKeyOperations.Default;
                }
            }
        }

        /// <summary>
        /// ���L�[�̃I�y���[�V����
        /// </summary>
        [XmlIgnore]
        public EbIRCKeyOperations DownKeyOperation
        {
            get
            {
                switch (VerticalKeyOperation)
                {
                    case 1:
                        return EbIRCKeyOperations.QuickChannelNext;

                    case 2:
                        return EbIRCKeyOperations.PageDown;

                    case 3:
                        return EbIRCKeyOperations.InputLogNext;

                    case 4:
                        return EbIRCKeyOperations.FontSizeDown;

                    case 5:
                        return EbIRCKeyOperations.NoOperation;

                    default:
                        return EbIRCKeyOperations.Default;
                }
            }
        }

        /// <summary>
        /// ���L�[�̃I�y���[�V����
        /// </summary>
        [XmlIgnore]
        public EbIRCKeyOperations LeftKeyOperation
        {
            get
            {
                switch (HorizontalKeyOperation)
                {
                    case 1:
                        return EbIRCKeyOperations.QuickChannelPrev;

                    case 2:
                        return EbIRCKeyOperations.PageUp;

                    case 3:
                        return EbIRCKeyOperations.InputLogPrev;

                    case 4:
                        return EbIRCKeyOperations.FontSizeUp;

                    case 5:
                        return EbIRCKeyOperations.NoOperation;

                    default:
                        return EbIRCKeyOperations.Default;
                }
            }
        }

        /// <summary>
        /// �E�L�[�̃I�y���[�V����
        /// </summary>
        [XmlIgnore]
        public EbIRCKeyOperations RightKeyOperation
        {
            get
            {
                switch (HorizontalKeyOperation)
                {
                    case 1:
                        return EbIRCKeyOperations.QuickChannelNext;

                    case 2:
                        return EbIRCKeyOperations.PageDown;

                    case 3:
                        return EbIRCKeyOperations.InputLogNext;

                    case 4:
                        return EbIRCKeyOperations.FontSizeDown;

                    case 5:
                        return EbIRCKeyOperations.NoOperation;

                    default:
                        return EbIRCKeyOperations.Default;
                }
            }
        }

        /// <summary>
        /// Ctrl+��L�[�̃I�y���[�V����
        /// </summary>
        [XmlIgnore]
        public EbIRCKeyOperations CtrlUpKeyOperation
        {
            get
            {
                switch (VerticalKeyWithCtrlOperation)
                {
                    case 1:
                        return EbIRCKeyOperations.QuickChannelPrev;

                    case 2:
                        return EbIRCKeyOperations.PageUp;

                    case 3:
                        return EbIRCKeyOperations.InputLogPrev;

                    case 4:
                        return EbIRCKeyOperations.FontSizeUp;

                    case 5:
                        return EbIRCKeyOperations.NoOperation;

                    default:
                        return EbIRCKeyOperations.Default;
                }
            }
        }

        /// <summary>
        /// Ctrl+���L�[�̃I�y���[�V����
        /// </summary>
        [XmlIgnore]
        public EbIRCKeyOperations CtrlDownKeyOperation
        {
            get
            {
                switch (VerticalKeyWithCtrlOperation)
                {
                    case 1:
                        return EbIRCKeyOperations.QuickChannelNext;

                    case 2:
                        return EbIRCKeyOperations.PageDown;

                    case 3:
                        return EbIRCKeyOperations.InputLogNext;

                    case 4:
                        return EbIRCKeyOperations.FontSizeDown;

                    case 5:
                        return EbIRCKeyOperations.NoOperation;

                    default:
                        return EbIRCKeyOperations.Default;
                }
            }
        }

        /// <summary>
        /// Ctrl+���L�[�̃I�y���[�V����
        /// </summary>
        [XmlIgnore]
        public EbIRCKeyOperations CtrlLeftKeyOperation
        {
            get
            {
                switch (HorizontalKeyWithCtrlOperation)
                {
                    case 1:
                        return EbIRCKeyOperations.QuickChannelPrev;

                    case 2:
                        return EbIRCKeyOperations.PageUp;

                    case 3:
                        return EbIRCKeyOperations.InputLogPrev;

                    case 4:
                        return EbIRCKeyOperations.FontSizeUp;

                    case 5:
                        return EbIRCKeyOperations.NoOperation;

                    default:
                        return EbIRCKeyOperations.Default;
                }
            }
        }

        /// <summary>
        /// Ctrl+�E�L�[�̃I�y���[�V����
        /// </summary>
        [XmlIgnore]
        public EbIRCKeyOperations CtrlRightKeyOperation
        {
            get
            {
                switch (HorizontalKeyWithCtrlOperation)
                {
                    case 1:
                        return EbIRCKeyOperations.QuickChannelNext;

                    case 2:
                        return EbIRCKeyOperations.PageDown;

                    case 3:
                        return EbIRCKeyOperations.InputLogNext;

                    case 4:
                        return EbIRCKeyOperations.FontSizeDown;

                    case 5:
                        return EbIRCKeyOperations.NoOperation;

                    default:
                        return EbIRCKeyOperations.Default;
                }
            }
        }

        #endregion

    }
}
