using System;
using System.Collections.Generic;
using System.Text;

namespace EbiSoft.EbIRC.IRC
{
    public class NickNameChangeEventArgs : EventArgs
    {
        private string m_before;
        private string m_after;

        /// <summary>
        /// �ύX�O�̖��O
        /// </summary>
        public string Before
        {
            get { return m_before; }
            set { m_before = value; }
        }

        /// <summary>
        /// �ύX��̖��O
        /// </summary>
        public string After
        {
            get { return m_after; }
            set { m_after = value; }
        }
	
        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="before">�ύX�O�̖��O</param>
        /// <param name="after">�ύX��̖��O</param>
        public NickNameChangeEventArgs(string before, string after)
        {
            m_before = before;
            m_after = after;
        }
    }
    public delegate void NickNameChangeEventHandler(object sender, NickNameChangeEventArgs e);
}
