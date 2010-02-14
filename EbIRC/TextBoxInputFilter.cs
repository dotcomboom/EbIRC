using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace EbiSoft.EbIRC
{
    /// <summary>
    /// �e�L�X�g�{�b�N�X���̓t�B���^�̊��N���X
    /// </summary>
    abstract class TextBoxInputFilter : IDisposable
    {
        #region P/Invoke �錾

        [DllImport("coredll.dll")]
        private extern static IntPtr SetWindowLong(IntPtr hwnd, int nIndex, IntPtr dwNewLong);
        private const int GWL_WNDPROC = -4;

        [DllImport("coredll.dll")]
        private extern static int CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hwnd, uint msg, uint wParam, int lParam);

        #endregion

        /// <summary>
        /// �T�u�N���X������E�B���h�E�v���V�[�W���̃f���Q�[�g
        /// </summary>
        private delegate int WndProcDelegate(IntPtr hwnd, uint msg, uint wParam, int lParam);

        private bool disposed = false;    // Dispose ���Ă΂ꂽ��
        private IntPtr oldWndProc;        // �O�̃n���h��
        private IntPtr targetHandle;      // �e�L�X�g�{�b�N�X�̃n���h��
        private WndProcDelegate wndProc;
        private IntPtr wndProcPtr;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="targetTextBox">�����Ώۂ̃e�L�X�g�{�b�N�X</param>
        public TextBoxInputFilter(TextBox targetTextBox)
        {
            targetHandle = targetTextBox.Handle;
            wndProc = new WndProcDelegate(WndProc);
            wndProcPtr = Marshal.GetFunctionPointerForDelegate(wndProc);

            // �T�u�N���X������
            oldWndProc = SetWindowLong(targetHandle, GWL_WNDPROC, wndProcPtr);
        }

        /// <summary>
        /// �j��
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                // �T�u�N���X����������
                SetWindowLong(targetHandle, GWL_WNDPROC, oldWndProc);
                disposed = true;
            }
        }

        /// <summary>
        /// �E�B���h�E�v���V�[�W��
        /// </summary>
        protected virtual int WndProc(IntPtr hwnd, uint msg, uint wParam, int lParam)
        {
            // �f�t�H���g�̃v���V�[�W����
            return CallWindowProc(oldWndProc, hwnd, msg, wParam, lParam);
        }

        #region �v���p�e�B

        /// <summary>
        /// �����ΏۂɂȂ��Ă���e�L�X�g�{�b�N�X
        /// </summary>
        public TextBox TextBox
        {
            get { return m_textBox; }
        }
        private TextBox m_textBox = null;

        #endregion

    }
}
