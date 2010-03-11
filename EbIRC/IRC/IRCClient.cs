using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;
using SslTest;

namespace EbiSoft.EbIRC.IRC {
	/// <summary>
	/// IRCClient �̊T�v�̐����ł��B
	/// </summary>
    public class IRCClient : Component
    {
        private Thread m_thread;        // ���M�X���b�h
        private Socket m_socket;        // �\�P�b�g
        private SslHelper m_sslHelper;  // SSL�w���p
        private NetworkStream m_stream; // �X�g���[��
        private StreamReader m_reader;  // ��M�p�X�g���[�����[�_�[
        private StreamWriter m_writer;  // ���M�p�X�g���[�����C�^�[

        private Queue m_sendQueue;      // ���M�L���[

        private ManualResetEvent m_threadStopSignal;    // �X���b�h��~�V�O�i��
        private bool m_threadStopFlag;                  // �X���b�h��~�t���O

        private ServerInfo m_server;    // �ڑ�����
        private UserInfo m_user;        // ���[�U�[���
        private string m_userString;    // ���̃N���C�A���g�̃��[�U�[������
        private Encoding m_encoding;    // �G���R�[�f�B���O

        private IAsyncResult m_connectAsync;        // �ڑ����V���N���I�u�W�F�N�g
        Dictionary<string, string[]> m_namelist;    // ���O���X�g�ꎞ�ۑ��p
        private bool m_online;                      // �ڑ������t���O
        private const int MaxNickLength = 9;        // NICK�ő�T�C�Y

        private Control m_ownerControl;             // �e�R���g���[��

        #region �C�x���g��`

        #region Connected

        private static readonly object eventKeyOfConnected = new object();

        /// <summary>
        /// �T�[�o�[�ɐڑ������Ƃ��ɔ������܂��B
        /// </summary>
        public event EventHandler Connected
        {
            add
            {
                Events.AddHandler(eventKeyOfConnected, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfConnected, value);
            }
        }

        /// <summary>
        /// Connected �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnConnected()
        {
            EventHandler handler = (EventHandler)Events[eventKeyOfConnected];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, EventArgs.Empty);
                }
                else
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region ConnectionFailed

        private static readonly object eventKeyOfConnectionFailed = new object();

        /// <summary>
        /// �T�[�o�[�ւ̐ڑ������s�����Ƃ��ɔ������܂��B
        /// </summary>
        public event EventHandler ConnectionFailed
        {
            add
            {
                Events.AddHandler(eventKeyOfConnectionFailed, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfConnectionFailed, value);
            }
        }

        /// <summary>
        /// ConnectionFailed �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnConnectionFailed()
        {
            EventHandler handler = (EventHandler)Events[eventKeyOfConnectionFailed];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, EventArgs.Empty);
                }
                else
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Disconnected

        private static readonly object eventKeyOfDisconnected = new object();

        /// <summary>
        /// �ڑ����ؒf�������ɔ������܂��B
        /// </summary>
        public event EventHandler Disconnected
        {
            add
            {
                Events.AddHandler(eventKeyOfDisconnected, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfDisconnected, value);
            }
        }

        /// <summary>
        /// Disconnected �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnDisconnected()
        {
            EventHandler handler = (EventHandler)Events[eventKeyOfDisconnected];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, EventArgs.Empty);
                }
                else
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region StartMessageEvents

        private static object eventKeyOfStartMessageEvents = new object();

        /// <summary>
        /// ���b�Z�[�W�C�x���g�̏������n�܂�O�ɔ������܂��B
        /// </summary>
        public event EventHandler StartMessageEvents
        {
            add
            {
                Events.AddHandler(eventKeyOfStartMessageEvents, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfStartMessageEvents, value);
            }
        }

        /// <summary>
        /// StartMessageEvents �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnStartMessageEvents()
        {
            EventHandler handler = (EventHandler)Events[eventKeyOfStartMessageEvents];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, EventArgs.Empty);
                }
                else
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region FinishMessageEvents

        private static object eventKeyOfFinishMessageEvents = new object();

        /// <summary>
        /// ���b�Z�[�W�C�x���g�̏���������������ɂɔ������܂��B
        /// </summary>
        public event EventHandler FinishMessageEvents
        {
            add
            {
                Events.AddHandler(eventKeyOfFinishMessageEvents, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfFinishMessageEvents, value);
            }
        }

        /// <summary>
        /// FinishMessageEvents �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnFinishMessageEvents()
        {
            EventHandler handler = (EventHandler)Events[eventKeyOfFinishMessageEvents];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, EventArgs.Empty);
                }
                else
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region ProcessedConnection

        private static readonly object eventKeyOfProcessedConnection = new object();

        /// <summary>
        /// �ڑ������������Ƃ��ɂɔ������܂��B
        /// </summary>
        public event EventHandler ProcessedConnection
        {
            add
            {
                Events.AddHandler(eventKeyOfProcessedConnection, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfProcessedConnection, value);
            }
        }

        /// <summary>
        /// ProcessedConnection �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnProcessedConnection()
        {
            EventHandler handler = (EventHandler)Events[eventKeyOfProcessedConnection];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, EventArgs.Empty);
                }
                else
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region ChangedMyNickname

        private static readonly object eventKeyOfChangedMyNickname = new object();

        /// <summary>
        /// ���[�U�[�̃j�b�N�l�[�����ύX���ꂽ�Ƃ��ɔ������܂��B
        /// </summary>
        public event NickNameChangeEventHandler ChangedMyNickname
        {
            add
            {
                Events.AddHandler(eventKeyOfChangedMyNickname, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfChangedMyNickname, value);
            }
        }

        /// <summary>
        /// ChangedMyNickname �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnChangedMyNickname(NickNameChangeEventArgs e)
        {
            NickNameChangeEventHandler handler = (NickNameChangeEventHandler)Events[eventKeyOfChangedMyNickname];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, e);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        #endregion

        #region ChangedNickname

        private static readonly object eventKeyOfChangedNickname = new object();

        /// <summary>
        /// ���[�U�[�̃j�b�N�l�[�����ύX���ꂽ�Ƃ��ɔ������܂��B
        /// </summary>
        public event NickNameChangeEventHandler ChangedNickname
        {
            add
            {
                Events.AddHandler(eventKeyOfChangedNickname, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfChangedNickname, value);
            }
        }

        /// <summary>
        /// ChangedMyNickname �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnChangedNickname(NickNameChangeEventArgs e)
        {
            NickNameChangeEventHandler handler = (NickNameChangeEventHandler)Events[eventKeyOfChangedNickname];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, e);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        #endregion

        #region ReceiveServerReply

        private static readonly object eventKeyOfReceiveServerReply = new object();

        /// <summary>
        /// �T�[�o�[���b�Z�[�W����M�����Ƃ��ɔ������܂��B
        /// </summary>
        public event ReceiveServerReplyEventHandler ReceiveServerReply
        {
            add
            {
                Events.AddHandler(eventKeyOfReceiveServerReply, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfReceiveServerReply, value);
            }
        }

        /// <summary>
        /// ReceiveServerReply �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnReceiveServerReply(ReceiveServerReplyEventArgs e)
        {
            ReceiveServerReplyEventHandler handler = (ReceiveServerReplyEventHandler)Events[eventKeyOfReceiveServerReply];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, e);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        #endregion

        #region ReceiveMotdMesage

        private static readonly object eventKeyOfReceiveMotdMesage = new object();

        /// <summary>
        /// MOTD����M�����Ƃ��ɔ������܂��B
        /// </summary>
        public event ReceiveMessageEventHandler ReceiveMotdMesage
        {
            add
            {
                Events.AddHandler(eventKeyOfReceiveMotdMesage, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfReceiveMotdMesage, value);
            }
        }

        /// <summary>
        /// ReceiveMotdMesage �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnReceiveMotdMesage(ReceiveMessageEventArgs e)
        {
            ReceiveMessageEventHandler handler = (ReceiveMessageEventHandler)Events[eventKeyOfReceiveMotdMesage];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, e);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        #endregion

        #region ReceiveMessage

        private static readonly object eventKeyOfReceiveMessage = new object();

        /// <summary>
        /// PRIVMSG���b�Z�[�W����M�����Ƃ��ɔ������܂��B
        /// </summary>
        public event ReceiveMessageEventHandler ReceiveMessage
        {
            add
            {
                Events.AddHandler(eventKeyOfReceiveMessage, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfReceiveMessage, value);
            }
        }

        /// <summary>
        /// ReceiveMessage �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnReceiveMessage(ReceiveMessageEventArgs e)
        {
            ReceiveMessageEventHandler handler = (ReceiveMessageEventHandler)Events[eventKeyOfReceiveMessage];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, e);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        #endregion

        #region ReceiveNotice

        private static readonly object eventKeyOfReceiveNotice = new object();

        /// <summary>
        /// NOTICE���b�Z�[�W����M�����Ƃ��ɂɔ������܂��B
        /// </summary>
        public event ReceiveMessageEventHandler ReceiveNotice
        {
            add
            {
                Events.AddHandler(eventKeyOfReceiveNotice, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfReceiveNotice, value);
            }
        }

        /// <summary>
        /// ReceiveNotice �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnReceiveNotice(ReceiveMessageEventArgs e)
        {
            ReceiveMessageEventHandler handler = (ReceiveMessageEventHandler)Events[eventKeyOfReceiveNotice];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, e);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        #endregion

        #region ReceiveNames

        private static readonly object eventKeyOfReceiveNames = new object();

        /// <summary>
        /// �Q���҃��X�g����M�����Ƃ��ɔ������܂��B
        /// </summary>
        public event ReceiveNamesEventHandler ReceiveNames
        {
            add
            {
                Events.AddHandler(eventKeyOfReceiveNames, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfReceiveNames, value);
            }
        }

        /// <summary>
        /// ReceiveNames �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnReceiveNames(ReceiveNamesEventArgs e)
        {
            ReceiveNamesEventHandler handler = (ReceiveNamesEventHandler)Events[eventKeyOfReceiveNames];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, e);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        #endregion

        #region ReceiveCtcpQuery

        private static readonly object eventKeyOfReceiveCtcpQuery = new object();

        /// <summary>
        /// CTCP�N�G������M�����Ƃ��ɔ������܂��B
        /// </summary>
        public event CtcpEventHandler ReceiveCtcpQuery
        {
            add
            {
                Events.AddHandler(eventKeyOfReceiveCtcpQuery, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfReceiveCtcpQuery, value);
            }
        }

        /// <summary>
        /// ReceiveCtcpQuery �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnReceiveCtcpQuery(CtcpEventArgs e)
        {
            CtcpEventHandler handler = (CtcpEventHandler)Events[eventKeyOfReceiveCtcpQuery];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, e);
                }
                else
                {
                    handler(this, e);
                }
                if (e.Reply != string.Empty)
                {
                    SendCtcpReply(e.Sender, string.Format("{0} {1}", e.Command, e.Reply));
                }
            }
        }

        #endregion

        #region ReceiveCtcpReply

        private static readonly object eventKeyOfReceiveCtcpReply = new object();

        /// <summary>
        /// CTCP���v���C����M�����Ƃ��ɔ������܂��B
        /// </summary>
        public event CtcpEventHandler ReceiveCtcpReply
        {
            add
            {
                Events.AddHandler(eventKeyOfReceiveCtcpReply, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfReceiveCtcpReply, value);
            }
        }

        /// <summary>
        /// ReceiveCtcpReply �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnReceiveCtcpReply(CtcpEventArgs e)
        {
            CtcpEventHandler handler = (CtcpEventHandler)Events[eventKeyOfReceiveCtcpReply];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, e);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        #endregion

        #region UserInOut

        private static readonly object eventKeyOfUserInOut = new object();

        /// <summary>
        /// ���[�U�[�̏o���肪�������Ƃ��ɔ������܂��B
        /// </summary>
        public event UserInOutEventHandler UserInOut
        {
            add
            {
                Events.AddHandler(eventKeyOfUserInOut, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfUserInOut, value);
            }
        }

        /// <summary>
        /// UserInOut �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnUserInOut(UserInOutEventArgs e)
        {
            UserInOutEventHandler handler = (UserInOutEventHandler)Events[eventKeyOfUserInOut];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, e);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        #endregion

        #region Kick

        private static readonly object eventKeyOfKick = new object();

        /// <summary>
        /// �L�b�N�����s���ꂽ�Ƃ��ɔ������܂��B
        /// </summary>
        public event KickEventHandler Kick
        {
            add
            {
                Events.AddHandler(eventKeyOfKick, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfKick, value);
            }
        }

        /// <summary>
        /// Kick �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnKick(KickEventArgs e)
        {
            KickEventHandler handler = (KickEventHandler)Events[eventKeyOfKick];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, e);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        #endregion

        #region TopicChange

        private static readonly object eventKeyOfTopicChange = new object();

        /// <summary>
        /// �g�s�b�N���ύX���ꂽ�Ƃ��ɔ������܂��B
        /// </summary>
        public event TopicChangeEventDelegate TopicChange
        {
            add
            {
                Events.AddHandler(eventKeyOfTopicChange, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfTopicChange, value);
            }
        }

        /// <summary>
        /// TopicChange �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnTopicChange(TopicChangeEventArgs e)
        {
            TopicChangeEventDelegate handler = (TopicChangeEventDelegate)Events[eventKeyOfTopicChange];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, e);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        #endregion

        #region ModeChange

        private static readonly object eventKeyOfModeChange = new object();

        /// <summary>
        /// ���[�h���ύX���ꂽ�Ƃ��ɔ������܂��B
        /// </summary>
        public event ModeChangeEventHandler ModeChange
        {
            add
            {
                Events.AddHandler(eventKeyOfModeChange, value);
            }
            remove
            {
                Events.RemoveHandler(eventKeyOfModeChange, value);
            }
        }

        /// <summary>
        /// ModeChange �C�x���g�𔭐������܂��B
        /// </summary>
        protected void OnModeChange(ModeChangeEventArgs e)
        {
            ModeChangeEventHandler handler = (ModeChangeEventHandler)Events[eventKeyOfModeChange];
            if (handler != null)
            {
                Control owner = GetOwner();
                if ((owner != null) && owner.InvokeRequired)
                {
                    owner.Invoke(handler, this, e);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        #endregion

        private Control GetOwner()
        {
            return OwnerControl;
        }

        #endregion

        #region �R���X�g���N�^

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public IRCClient()
        {
            m_encoding = new UTF8Encoding(false);

            m_sendQueue = new Queue();
            m_namelist = new Dictionary<string, string[]>();

            m_threadStopSignal = new ManualResetEvent(false);
            m_threadStopFlag = false;

            m_ownerControl = null;
        }

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="ownerControl">�ʒm��R���g���[��</param>
        public IRCClient(Control ownerControl) : this()
        {
            m_ownerControl = ownerControl;
        }

        #endregion

        #region �ڑ�

        /// <summary>
        /// �ڑ�
        /// </summary>
        /// <param name="name">�T�[�o�[��</param>
        /// <param name="port">�ڑ��|�[�g</param>
        /// <param name="password">�T�[�o�[�p�X���[�h</param>
        /// <param name="nickname">�j�b�N�l�[��</param>
        /// <param name="realname">���O</param>
        public void Connect(string name, int port, string password, string nickname, string realname)
        {
            Connect(new ServerInfo(name, port, password), new UserInfo(nickname, realname));
        }

        /// <summary>
        /// �ڑ�
        /// </summary>
        /// <param name="name">�T�[�o�[��</param>
        /// <param name="port">�ڑ��|�[�g</param>
        /// <param name="password">�T�[�o�[�p�X���[�h</param>
        /// <param name="nickname">�j�b�N�l�[��</param>
        /// <param name="realname">���O</param>
        /// <param name="useSsl">SSL�g�p</param>
        public void Connect(string name, int port, string password, bool useSsl, string nickname, string realname)
        {
            Connect(new ServerInfo(name, port, password, useSsl), new UserInfo(nickname, realname));
        }


        /// <summary>
        /// �ڑ�
        /// </summary>
        /// <param name="name">�T�[�o�[��</param>
        /// <param name="nickname">�j�b�N�l�[��</param>
        /// <param name="realname">���O</param>
        public void Connect(ServerInfo server, string nickname, string realname)
        {
            Connect(server, new UserInfo(nickname, realname));
        }

        /// <summary>
        /// �ڑ�
        /// </summary>
        /// <param name="name">�T�[�o�[��</param>
        /// <param name="port">�ڑ��|�[�g</param>
        /// <param name="password">�T�[�o�[�p�X���[�h</param>
        /// <param name="user">���[�U�[�f�[�^</param>
        public void Connect(string name, int port, string password, UserInfo user)
        {
            Connect(new ServerInfo(name, port, password), user);
        }

        /// <summary>
        /// �ڑ�
        /// </summary>
        /// <param name="server">�T�[�o�[�f�[�^</param>
        /// <param name="user">���[�U�[�f�[�^</param>
        public void Connect(ServerInfo server, UserInfo user)
        {
            if (Status != IRCClientStatus.Disconnected)
            {
                throw new InvalidOperationException();
            }

            try
            {
                // �����B
                m_server = server;
                m_user = user;
                m_online = false;
                m_threadStopFlag = false;
                lock (m_sendQueue) { m_sendQueue.Clear(); }
                lock (m_namelist) { m_namelist.Clear(); }

                // �\�P�b�g�쐬�E�ڑ�
                Debug.WriteLine("�ڑ����J�n���܂��B", "IRCClient");
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (Server.UseSsl)
                {
                    m_sslHelper = new SslHelper(m_socket, Server.Name);
                }
                m_connectAsync = m_socket.BeginConnect(server.GetEndPoint(), new AsyncCallback(OnConnected), m_socket);
            }
            catch
            {
                OnConnectionFailed();
                Close();
            }
        }

        /// <summary>
        /// �ڑ������R�[���o�b�N
        /// </summary>
        /// <param name="ar">�񓯊������X�e�[�^�X</param>
        protected void OnConnected(IAsyncResult ar)
        {
            try
            {
                // �ڑ������̊���
                // (�ڑ��G���[�̏ꍇ�AEndConnect�̒i�K��SocketException���X���[�����)
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);

                // �ڑ������C�x���g���s
                Debug.WriteLine("�ڑ����܂����B", "IRCClient");
                OnConnected();

                // �X�g���[���쐬
                m_stream = new NetworkStream(socket);

                // �X�g���[�����[�_�E���C�^�쐬
                m_reader = new StreamReader(m_stream, this.Encoding);
                m_writer = new StreamWriter(m_stream, this.Encoding);
                m_writer.NewLine = "\r\n";
                m_writer.AutoFlush = true;

                // ���M�X���b�h�쐬
                m_thread = new Thread(new ThreadStart(ThreadAction));
                m_thread.Name = "IRCThread";
                m_thread.IsBackground = true;
                m_thread.Start();
                Debug.WriteLine("�X���b�h���J�n���܂����B", "IRCClient");

                // ���O�C���R�}���h���M
                Debug.WriteLine("���O�C���R�}���h���M���܂��B", "IRCClient");
                SendConnectCommand();
            }
            catch
            {
                // �ڑ����s���̏���
                OnConnectionFailed();
                Close();
            }
        }

        #endregion

        #region �ؒf

        /// <summary>
        /// �ؒf
        /// </summary>
        /// <param name="message">�ؒf�����b�Z�[�W</param>
        public void Disconnect(string message)
        {
            // �I�����C����Ԃ̂Ƃ���QUIT�𑗂�
            if (Status == IRCClientStatus.Online)
            {
                SendCommand(string.Format("QUIT :{0}", message));
            }
            Close();
        }

        /// <summary>
        /// �ؒf
        /// </summary>
        public void Disconnect()
        {
            Disconnect("EOF From client.");
        }

        /// <summary>
        /// �����ؒf
        /// </summary>
        public void Close()
        {
            bool isDisconnected = (Status != IRCClientStatus.Disconnected);

            // �ڑ����Ȃ�A�ڑ�������������
            if ((m_connectAsync != null) && (!m_connectAsync.IsCompleted))
            {
                ((Socket)m_connectAsync.AsyncState).EndConnect(m_connectAsync);
            }

            // �X���b�h���~�߂�
            if (!m_threadStopFlag)
            {
                m_threadStopFlag = true;
                if (m_threadStopSignal.WaitOne(5000, false))
                {
                    // 5�b�Ŏ~�܂�Ȃ���΋����I��
                    m_thread.Abort();
                }
            }
        }

        /// <summary>
        /// �����ؒf
        /// </summary>
        [Obsolete]
        public void Close(bool report)
        {
            Close();
        }

        #endregion

        #region �X���b�h����

        /// <summary>
        /// �X���b�h���C������
        /// </summary>
        private void ThreadAction()
        {
            try
            {
                m_threadStopSignal.Reset();
                Queue<EventData> eventQueue = new Queue<EventData>();

                while (!m_threadStopFlag)
                {
                    // ���M����
                    lock (m_sendQueue)
                    {
                        // ���M�L���[���Ȃ��Ȃ�܂ŁA�L���[��ǂݎ���đ��M����
                        while (m_sendQueue.Count > 0)
                        {
                            string sendLine = m_sendQueue.Dequeue() as string;
                            if (sendLine != null)
                            {
                                Debug.WriteLine("Send> " + sendLine, "IRCClient");
                                m_writer.WriteLine(sendLine);

                                if (sendLine.StartsWith("QUIT ", StringComparison.OrdinalIgnoreCase))
                                {
                                    m_threadStopFlag = true;
                                }
                            }
                        }
                    }

                    // ��M����
                    if (Server.UseSsl && !m_stream.DataAvailable)
                    {
                        m_socket.Poll(100, SelectMode.SelectRead);
                    }
                    while (m_stream.DataAvailable)
                    {
                        try
                        {
                            ProcessIRCMessage(eventQueue, m_reader.ReadLine());
                        }
                        catch (MessageParseException)
                        {
                            // TODO:�s�����b�Z�[�W��M
                        }
                    }
                    

                    // �C�x���g��������
                    if (eventQueue.Count > 0)
                    {
                        OnStartMessageEvents();
                        try
                        {
                            // �L���[�ɂ���C�x���g�����ׂĔ���������
                            while (eventQueue.Count > 0)
                            {
                                EventData eventData = eventQueue.Dequeue();

                                #region �C�x���g�f�B�X�p�b�`

                                if (eventData.EventKey == eventKeyOfProcessedConnection)
                                {
                                    OnProcessedConnection();
                                }
                                else if (eventData.EventKey == eventKeyOfChangedMyNickname)
                                {
                                    OnChangedMyNickname(eventData.Argument as NickNameChangeEventArgs);
                                }
                                else if (eventData.EventKey == eventKeyOfChangedNickname)
                                {
                                    OnChangedNickname(eventData.Argument as NickNameChangeEventArgs);
                                }
                                else if (eventData.EventKey == eventKeyOfReceiveServerReply)
                                {
                                    OnReceiveServerReply(eventData.Argument as ReceiveServerReplyEventArgs);
                                }
                                else if (eventData.EventKey == eventKeyOfReceiveMotdMesage)
                                {
                                    OnReceiveMotdMesage(eventData.Argument as ReceiveMessageEventArgs);
                                }
                                else if (eventData.EventKey == eventKeyOfReceiveMessage)
                                {
                                    OnReceiveMessage(eventData.Argument as ReceiveMessageEventArgs);
                                }
                                else if (eventData.EventKey == eventKeyOfReceiveNotice)
                                {
                                    OnReceiveNotice(eventData.Argument as ReceiveMessageEventArgs);
                                }
                                else if (eventData.EventKey == eventKeyOfReceiveNames)
                                {
                                    OnReceiveNames(eventData.Argument as ReceiveNamesEventArgs);
                                }
                                else if (eventData.EventKey == eventKeyOfReceiveCtcpQuery)
                                {
                                    OnReceiveCtcpQuery(eventData.Argument as CtcpEventArgs);
                                }
                                else if (eventData.EventKey == eventKeyOfReceiveCtcpReply)
                                {
                                    OnReceiveCtcpReply(eventData.Argument as CtcpEventArgs);
                                }
                                else if (eventData.EventKey == eventKeyOfUserInOut)
                                {
                                    OnUserInOut(eventData.Argument as UserInOutEventArgs);
                                }
                                else if (eventData.EventKey == eventKeyOfModeChange)
                                {
                                    OnModeChange(eventData.Argument as ModeChangeEventArgs);
                                }
                                else if (eventData.EventKey == eventKeyOfTopicChange)
                                {
                                    OnTopicChange(eventData.Argument as TopicChangeEventArgs);
                                }
                                else if (eventData.EventKey == eventKeyOfKick)
                                {
                                    OnKick(eventData.Argument as KickEventArgs);
                                }
                                else
                                {
                                    Debug.WriteLine("Undefined event:" + eventData.Argument.GetType().ToString());
                                }

                                #endregion
                            }
                        }
                        catch (Exception) { }
                        finally
                        {
                            OnFinishMessageEvents();
                        }
                    }

                    Thread.Sleep(50);
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception)
            {
            }
            finally
            {
                // �ڑ����ł���΃\�P�b�g�����
                if (m_socket.Connected)
                {
                    try
                    {
                        m_socket.Close();
                    }
                    catch (Exception) { }
                }
                if (m_sslHelper != null)
                {
                    m_sslHelper.Dispose();
                }

                // �g�p�����f�[�^���N���A����
                m_socket = null;
                m_sslHelper = null;
                m_reader = null;
                m_writer = null;
                m_online = false;

                OnDisconnected();

                // ���������V�O�i�����M
                m_threadStopSignal.Set();
            }
        }

        #endregion

        #region ��M���b�Z�[�W���

        /// <summary>
        /// ��M�������b�Z�[�W������
        /// </summary>
        /// <param name="queue">�ǉ���̃C�x���g�L���[</param>
        /// <param name="message">��M���b�Z�[�W</param>
        /// <returns>����������C�x���g�̏��</returns>
        protected void ProcessIRCMessage(Queue<EventData> queue, string message)
        {
            try
            {
                #region ���b�Z�[�W�p�[�X
                Debug.WriteLine("Recv> " + message, "IRCClient");

                // �Ƃ肠�����A���p�X�y�[�X�ŋ�؂�
                string[] messageParams = message.TrimEnd(' ').Split(' ');

                // ���b�Z�[�W���i�[�ϐ�
                string sender;       // ���M��
                string command;      // �R�}���h
                string[] parameters; // �p�����[�^

                int parameterStartIndex; // �p�����[�^�̊J�n�C���f�b�N�X

                // ��{���𓾂�
                if (messageParams[0].StartsWith(":"))
                {
                    // �ʏ�̃��b�Z�[�W
                    sender = messageParams[0].TrimStart(':');
                    command = messageParams[1];
                    parameterStartIndex = 2;
                }
                else
                {
                    // �ʏ�ȊO�̃��b�Z�[�W
                    sender = string.Empty;
                    command = messageParams[0];
                    parameterStartIndex = 1;
                }

                // �p�����[�^���܂��c���Ă���ꍇ�A�c��̃p�����[�^����������
                if (parameterStartIndex < messageParams.Length)
                {
                    // �p�����[�^���ڂ̍ő吔�͔��p�X�y�[�X�X�v���b�g�ς݂ƊJ�n�C���f�b�N�X�̍��������Ȃ��Ȃ�
                    ArrayList parameterList = new ArrayList(messageParams.Length - parameterStartIndex);
                    for (int i = parameterStartIndex; i < messageParams.Length; i++)
                    {
                        // : �Ŏn�܂�ꍇ�A���������͈�̃p�����[�^�Ƃ��ď�������
                        if (messageParams[i].StartsWith(":"))
                        {
                            string tempParam = messageParams[i].Substring(1);
                            // �܂��p�����[�^�̎c���Ă���ꍇ�͈�C�ɂȂ�
                            if ((i + 1) < messageParams.Length)
                            {
                                tempParam += " " + string.Join(" ", messageParams, (i + 1), messageParams.Length - (i + 1));
                            }
                            parameterList.Add(tempParam);
                            break;
                        }
                        else
                        {
                            parameterList.Add(messageParams[i]);
                        }
                    }
                    parameters = new string[parameterList.Count];
                    parameterList.CopyTo(parameters);
                }
                else
                {
                    // �p�����[�^�Ȃ�
                    parameters = new string[] { };
                }
                #endregion

                // ���b�Z�[�W�U�蕪���ŋ��ʎg�p����ϐ�
                string channel;        // �`�����l��
                string receiver;       // ��M��
                string[] receivers;      // ��M�҈ꗗ

                if (char.IsNumber(command[0]))
                {
                    #region �j���[�����b�N���v���C

                    // ���v���C�ԍ�
                    ReplyNumbers number = (ReplyNumbers)int.Parse(command);

                    // �j���[�����b�N���v���C�� parameter[0] �͕K����M��
                    receiver = parameters[0];

                    switch (number)
                    {
                        // �ڑ�����
                        case ReplyNumbers.RPL_WELCOME:
                            if (Status != IRCClientStatus.Online)
                            {
                                m_online = true;

                                // �j�b�N�l�[�����Z�b�g
                                User.setNick(GetUserName(receiver));

                                // ���[�U�[�X�g�����O�𓾂� (�Ō�̃p�����[�^���X�y�[�X�Ő؂����Ō�̍���)
                                string[] userStringParseTemp = parameters[parameters.Length - 1].Split(' ');
                                m_userString = userStringParseTemp[userStringParseTemp.Length - 1];

                                // �C�x���g��ʒm
                                queue.Enqueue(new EventData(eventKeyOfProcessedConnection, EventArgs.Empty));
                                queue.Enqueue(new EventData(eventKeyOfReceiveServerReply, new ReceiveServerReplyEventArgs(number, SliceArray(parameters, 1))));
                            }
                            break;

                        // ���O�̃R���t���N�g
                        case ReplyNumbers.ERR_NICKNAMEINUSE:
                            queue.Enqueue(new EventData(eventKeyOfReceiveServerReply, new ReceiveServerReplyEventArgs(number, SliceArray(parameters, 1))));
                            // �V����Nickname ��p�ӂ���B
                            string newNickname = GetNextNick(m_user.NickName);
                            m_user.setNick(newNickname);
                            ChangeNickname(newNickname);
                            break;

                        #region MOTD���b�Z�[�W
                        case ReplyNumbers.RPL_MOTDSTART:
                            queue.Enqueue(new EventData(eventKeyOfReceiveMotdMesage, new ReceiveMessageEventArgs(sender, receiver, parameters[1])));
                            break;
                        case ReplyNumbers.RPL_MOTD:
                            queue.Enqueue(new EventData(eventKeyOfReceiveMotdMesage, new ReceiveMessageEventArgs(sender, receiver, parameters[1])));
                            break;
                        case ReplyNumbers.RPL_ENDOFMOTD:
                            queue.Enqueue(new EventData(eventKeyOfReceiveMotdMesage, new ReceiveMessageEventArgs(sender, receiver, parameters[1])));
                            break;
                        #endregion

                        #region �Q���҂̎�M

                        // �Q���҈ꗗ�̎�M
                        case ReplyNumbers.RPL_NAMREPLY:
                            // �`�����l�����𖼑O���X�g���擾
                            channel = parameters[2];
                            string[] names = parameters[3].Split(' ');

                            // ���O���X�g�ɍ��ڂ�����Βǉ�
                            if (m_namelist.ContainsKey(channel))
                            {
                                // �V�����z�������āA�����ɐV���̃f�[�^��\��t����
                                string[] tempArr = new string[m_namelist[channel].Length + names.Length];
                                m_namelist[channel].CopyTo(tempArr, 0);
                                names.CopyTo(tempArr, m_namelist[channel].Length);
                                m_namelist[channel] = tempArr;
                            }
                            // ���O���X�g�ɍ��ڂ��Ȃ���Βǉ�
                            else
                            {
                                m_namelist.Add(channel, names);
                            }
                            break;

                        // �Q���Ҏ�M����
                        case ReplyNumbers.RPL_ENDOFNAMES:
                            channel = parameters[1];

                            // �`�����l���̃f�[�^������Ƃ��̂�
                            if (m_namelist.ContainsKey(channel))
                            {
                                // �C�x���g�ňꗗ��ʒm������A���X�g����폜
                                queue.Enqueue(new EventData(eventKeyOfReceiveNames, new ReceiveNamesEventArgs(channel, m_namelist[channel])));
                                m_namelist.Remove(channel);
                            }
                            break;
                        #endregion

                        // �g�s�b�N�̎�M
                        case ReplyNumbers.RPL_TOPIC:
                            string topic;
                            channel = parameters[1];
                            topic = parameters[2];
                            queue.Enqueue(new EventData(eventKeyOfTopicChange, new TopicChangeEventArgs(channel, topic)));
                            break;

                        // �`�����l���̃��[�h�ύX
                        case ReplyNumbers.RPL_CHANNELMODEIS:
                            channel = parameters[1];
                            queue.Enqueue(new EventData(eventKeyOfModeChange, new ModeChangeEventArgs(string.Empty, channel, parameters[2])));
                            break;

                        // ����ȊO
                        default:
                            queue.Enqueue(new EventData(eventKeyOfReceiveServerReply, new ReceiveServerReplyEventArgs(number, SliceArray(parameters, 1))));
                            break;
                    }

                    #endregion
                }
                else
                {
                    #region �R�}���h

                    // �Ƃ肠���� parameter[0] ����M�҂Ƃ���
                    receiver = parameters[0];

                    switch (command.ToUpper())
                    {
                        #region ���b�Z�[�W��M�R�}���h

                        case "PRIVMSG":
                            receivers = receiver.Split(',');
                            if (parameters[1].StartsWith("\x001") && parameters[1].EndsWith("\x001"))
                            {
                                // CTCP�N�G������M�����ꍇ
                                CtcpEventArgs arg = CreateCtcpEventArgs(sender, parameters[1]);
                                queue.Enqueue(new EventData(eventKeyOfReceiveCtcpQuery, arg));
                            }
                            else
                            {
                                foreach (string r in receivers)
                                {
                                    queue.Enqueue(new EventData(eventKeyOfReceiveMessage, 
                                        new ReceiveMessageEventArgs(sender, r, parameters[1].Replace("\x001", ""))));
                                }
                            }
                            break;

                        case "NOTICE":
                            receivers = receiver.Split(',');

                            if (parameters[1].StartsWith("\x001") && parameters[1].EndsWith("\x001"))
                            {
                                // CTCP Reply �̂Ƃ�
                                queue.Enqueue(new EventData(eventKeyOfReceiveCtcpReply,
                                    CreateCtcpEventArgs(sender, parameters[1])));
                            }
                            else
                            {
                                foreach (string r in receivers)
                                {
                                    // �C�x���g��ʒm
                                    queue.Enqueue(new EventData(eventKeyOfReceiveNotice, 
                                        new ReceiveMessageEventArgs(sender, r, parameters[1].Replace("\x001", ""))));
                                }
                            }
                            break;

                        #endregion

                        case "PING":
                            SendCommand("PONG :" + string.Join(" ", parameters));
                            break;

                        case "JOIN":
                            ProcessUserInOut(queue, InOutCommands.Join, sender, receiver);
                            break;

                        case "QUIT":
                            ProcessUserInOut(queue, InOutCommands.Quit, sender, receiver);
                            break;

                        case "PART":
                            ProcessUserInOut(queue, InOutCommands.Leave, sender, receiver);
                            break;

                        case "KICK":
                            // �C�x���g��ʒm
                            queue.Enqueue(new EventData(eventKeyOfKick, 
                                new KickEventArgs(sender, receiver, parameters[1])));
                            break;

                        case "NICK":
                            if (sender == m_userString)
                            {
                                // �����̕ύX����M�����Ƃ�

                                // ���[�U�[�X�g�����O���X�V
                                m_userString = receiver + m_userString.Substring(m_userString.IndexOf("!"));

                                // �����̃j�b�N�l�[����ύX
                                User.setNick(IRCClient.GetUserName(m_userString));
                                queue.Enqueue(new EventData(eventKeyOfChangedMyNickname, 
                                    new NickNameChangeEventArgs(IRCClient.GetUserName(sender), receiver)));
                            }
                            else
                            {
                                // ���l�̖��O�̕ύX����M�����Ƃ�
                                queue.Enqueue(new EventData(eventKeyOfChangedNickname, 
                                    new NickNameChangeEventArgs(IRCClient.GetUserName(sender), receiver)));
                            }
                            break;

                        case "TOPIC":
                            queue.Enqueue(new EventData(eventKeyOfTopicChange,
                                new TopicChangeEventArgs(sender, receiver, parameters[1])));
                            break;

                        case "MODE":
                            if (parameters.Length > 1)
                            {
                                receivers = new string[parameters.Length - 1];
                                Array.Copy(parameters, 1, receivers, 0, receivers.Length);
                                queue.Enqueue(new EventData(eventKeyOfModeChange, 
                                    new ModeChangeEventArgs(sender, receiver, parameters[1], receivers)));
                            }
                            else if (parameters.Length == 1)
                            {
                                queue.Enqueue(new EventData(eventKeyOfModeChange, 
                                    new ModeChangeEventArgs(sender, receiver, parameters[1])));
                            }
                            break;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MessageParseException:" + ex.Message);
                throw new MessageParseException(ex);
            }
        }

        /// <summary>
        /// ���[�U�[���ގ����b�Z�[�W����������
        /// </summary>
        /// <param name="command">�R�}���h</param>
        /// <param name="sender">���M��</param>
        /// <param name="receiver">���M��</param>
        protected void ProcessUserInOut(Queue<EventData> queue, InOutCommands command, string sender, string receiver)
        {

            string[] targets = receiver.Split(',');

            // ��M�悲�ƂɃC�x���g���Ă�
            foreach (string target in targets)
            {
                queue.Enqueue(new EventData(eventKeyOfUserInOut, new UserInOutEventArgs(sender, command, target)));
            }
        }

        /// <summary>
        /// CTCP�C�x���g�f�[�^�𐶐�����
        /// </summary>
        /// <param name="sender">���M��</param>
        /// <param name="parameter">�p�����[�^</param>
        /// <returns>�������ꂽ�C�x���g�f�[�^</returns>
        private CtcpEventArgs CreateCtcpEventArgs(string sender, string parameter)
        {
            // CTCP Query
            string query = parameter.Trim('\x001');
            string cmd;
            string param;
            int idx = query.IndexOf(" ");
            if (idx < 1)
            {
                cmd = query;
                param = string.Empty;
            }
            else
            {
                cmd = query.Substring(0, idx);
                param = query.Substring(idx + 1);
            }
            return new CtcpEventArgs(sender, cmd, param);
        }

        #endregion

        #region ���M�n���\�b�h

        /// <summary>
        /// �ڑ��R�}���h�̑��M
        /// </summary>
        private void SendConnectCommand()
        {
            // �p�X���[�h���ݒ肳��Ă���Α��M
            if (m_server.Password != string.Empty)
            {
                SendCommand(string.Format("PASS {0}", m_server.Password));
            }
            
            // �j�b�N�l�[���𑗐M
            ChangeNickname(m_user.NickName);

            // ���[�U�[��񑗐M
            SendCommand(string.Format("USER {0} {1} {2} :{3}", m_user.NickName, "LocalEndPoint", "RemoteEndPoint", m_user.RealName));
        }

        /// <summary>
        /// �j�b�N�l�[���ύX�R�}���h�̑��M
        /// </summary>
        /// <param name="newnickname">�V�����j�b�N�l�[��</param>
        public void ChangeNickname(string newnickname)
        {
            // �j�b�N�l�[�����M
            SendCommand(string.Format("NICK {0}", newnickname));
        }

        /// <summary>
        /// �R�}���h�̑��M
        /// </summary>
        /// <param name="message">���M����R�}���h</param>
        public void SendCommand(string message)
        {
            // �ڑ��ς݂̂Ƃ��͒ǉ�
            if ((Status == IRCClientStatus.Connected) || (Status == IRCClientStatus.Online))
            {
                lock (m_sendQueue)
                {
                    m_sendQueue.Enqueue(message);
                }
                
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// �`�����l���ɎQ��
        /// </summary>
        /// <param name="channel">�Q������`�����l��</param>
        public void JoinChannel(string channel)
        {
            SendCommand(string.Format("JOIN {0}", channel));
        }

        /// <summary>
        /// �`�����l������ގ�
        /// </summary>
        /// <param name="channel">�ގ�����`�����l��</param>
        public void LeaveChannel(string channel)
        {
            SendCommand(string.Format("PART {0}", channel));
        }

        /// <summary>
        /// �v���C�x�[�g���b�Z�[�W���M
        /// </summary>
        /// <param name="receiver">���M��</param>
        /// <param name="message">���b�Z�[�W</param>
        public void SendPrivateMessage(string receiver, string message)
        {
            SendCommand(string.Format("PRIVMSG {0} :{1}", receiver, message));
        }

        /// <summary>
        /// notice ���b�Z�[�W���M
        /// </summary>
        /// <param name="receiver">���M��</param>
        /// <param name="message">���b�Z�[�W</param>
        public void SendNoticeMessage(string receiver, string message)
        {
            SendCommand(string.Format("NOTICE {0} :{1}", receiver, message));
        }

        /// <summary>
        /// CTCP�N�G�����M
        /// </summary>
        /// <param name="receiver">���M��</param>
        /// <param name="message">���b�Z�[�W</param>
        public void SendCtcpQuery(string receiver, string message)
        {
            SendPrivateMessage(receiver, string.Format("{0}{1}{0}", '\x001', message));
        }

        /// <summary>
        /// CTCP���v���C���M
        /// </summary>
        /// <param name="receiver">���M��</param>
        /// <param name="message">���b�Z�[�W</param>
        public void SendCtcpReply(string receiver, string message)
        {
            SendNoticeMessage(receiver, string.Format("{0}{1}{0}", '\x001', message));
        }

        #endregion

        #region �v���p�e�B

        /// <summary>
        /// �ڑ��X�e�[�^�X���擾���܂�
        /// </summary>
        public IRCClientStatus Status
        {
            get
            {
                // �I�����C��
                if (m_online)
                {
                    return IRCClientStatus.Online;
                }

                if (m_socket != null)
                {
                    // �ڑ��ς�
                    if (m_socket.Connected)
                    {
                        return IRCClientStatus.Connected;
                    }
                    // �ڑ���
                    if ((m_connectAsync != null) && (!m_connectAsync.IsCompleted))
                    {
                        return IRCClientStatus.EstablishConnection;
                    }
                }

                // �ڑ�����Ă��Ȃ�
                return IRCClientStatus.Disconnected;
            }
        }

        /// <summary>
        /// �T�[�o�[�����擾���܂�
        /// </summary>
        public ServerInfo Server
        {
            get { return m_server; }
        }

        /// <summary>
        /// ���[�U�[�����擾���܂�
        /// </summary>
        public UserInfo User
        {
            get { return m_user; }
        }

        /// <summary>
        /// ���[�U�[�t���l�[�����擾���܂�
        /// </summary>
        public string UserString
        {
            get { return m_userString; }
        }

        /// <summary>
        /// �G���R�[�f�B���O���擾�܂��͐ݒ肵�܂�
        /// </summary>
        public Encoding Encoding
        {
            get { return m_encoding; }
            set { m_encoding = value; }
        }

        /// <summary>
        /// �C�x���g�ʒm��̃R���g���[�����擾�܂��͐ݒ肵�܂�
        /// </summary>
        public Control OwnerControl
        {
            get { return m_ownerControl; }
            set { m_ownerControl = value; }
        }
	

        #endregion

        #region �X�^�e�B�b�N���\�b�h

        /// <summary>
        /// �z��̐擪�̗v�f���w�肳�ꂽ�������؂藎�Ƃ��܂�
        /// </summary>
        /// <param name="array"></param>
        /// <param name="sliceLength"></param>
        /// <returns></returns>
        private static string[] SliceArray(string[] array, int sliceLength)
        {
            // ��M�҂̕��̃p�����[�^��؂�l�߂�
            string[] sliceTempArray = new string[array.Length - 1];
            Array.Copy(array, sliceLength, sliceTempArray, 0, sliceTempArray.Length);
            return sliceTempArray;
        }

        /// <summary>
        /// ���[�U�[�t���l�[�����烆�[�U�[�����擾���܂�
        /// </summary>
        /// <param name="userstring"></param>
        /// <returns></returns>
        public static string GetUserName(string userstring)
        {
            int temp = userstring.IndexOf("!");
            if (temp > 0)
            {
                return userstring.Substring(0, temp);
            }
            else
            {
                return userstring;
            }
        }

        /// <summary>
        /// NICK�R���t���N�g���p�̐V�����j�b�N�l�[���𐶐����܂��B
        /// </summary>
        /// <param name="nick"></param>
        /// <returns></returns>
        public static string GetNextNick(string nick)
        {
            // �ő啶�����I�[�o�[���́A�ő啶�����ȓ��ɂȂ�悤�ɍ��
            if (nick.Length >= MaxNickLength)
            {
                nick = nick.Substring(0, MaxNickLength);
            }

            // �����̐������擾����
            string name;
            string number;
            Match match = Regex.Match(nick, @"([^\d]*)(\d+)$");
            if (match.Success)
            {
                name = match.Groups[1].Value;
                number = (int.Parse(match.Groups[2].Value) + 1).ToString();
            }
            else
            {
                name = nick;
                number = "0";
            }

            // ���O�𐶐�����
            if ((name.Length + number.Length) > MaxNickLength)
            {
                return name.Substring(0, MaxNickLength - number.Length) + number;
            }
            else
            {
                return name + number;
            }

        }

        /// <summary>
        /// �w�肳�ꂽ�����񂪃`�����l���`�����ǂ������ׂ܂��B
        /// </summary>
        /// <param name="text">���ׂ镶����</param>
        /// <returns>�`�����l���̌`���̏ꍇ�� true</returns>
        public static bool IsChannelString(string text)
        {
            if (text.StartsWith("#") || text.StartsWith("&")
                || text.StartsWith("+") || text.StartsWith("!"))
            {
                if (text.Length <= 50)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        #endregion

        #region ��M�����p�����N���X

        /// <summary>
        /// ��M�����̃p�[�X���ʂ̃C�x���g�̃f�[�^��\���N���X
        /// </summary>
        protected class EventData
        {
            private object m_key;
            private EventArgs m_arg;

            /// <summary>
            /// �C�x���g�f�[�^�̃L�[
            /// </summary>
            public object EventKey
            {
                get { return m_key; }
            }

            /// <summary>
            /// �C�x���g�f�[�^
            /// </summary>
            public EventArgs Argument
            {
                get { return m_arg; }
            }

            /// <summary>
            /// �R���X�g���N�^
            /// </summary>
            /// <param name="eventKey">�C�x���g�L�[</param>
            /// <param name="argument">�C�x���g�f�[�^</param>
            public EventData(object eventKey, EventArgs argument)
            {
                m_key = eventKey;
                m_arg = argument;
            }	
        }

        #endregion
    }

    /// <summary>
    /// IRCClient �̃X�e�[�^�X������킷�萔
    /// </summary>
    public enum IRCClientStatus
    {
        /// <summary>
        /// �ڑ����Ă��Ȃ�
        /// </summary>
        Disconnected,

        /// <summary>
        /// �ڑ�������
        /// </summary>
        EstablishConnection,

        /// <summary>
        /// �T�[�o�[�ɐڑ��ς�
        /// </summary>
        Connected,

        /// <summary>
        /// �ڑ���������
        /// </summary>
        Online
    }
}
