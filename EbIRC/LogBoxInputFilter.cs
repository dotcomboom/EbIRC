using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace EbiSoft.EbIRC
{
    /// <summary>
    /// ���O�{�b�N�X��IME���͏�Ԃ��Ď�����N���X
    /// </summary>
    class LogBoxInputFilter : TextBoxInputFilter
    {
        #region P/Invoke �錾

        // �E�B���h�E���b�Z�[�W
        private const uint WM_LBUTTONUP   = 0x202;  // IME�ϊ��J�n
        private const uint WM_LBUTTONDOWN = 0x201;  // IME�ϊ��I��
        private const uint WM_SIZE        = 0x0005; // �T�C�Y�ύX�C�x���g

        #endregion

        /// <summary>
        /// �^�b�v���ꂽ�Ƃ��ɔ�������C�x���g
        /// </summary>
        public event EventHandler TapDown;

        /// <summary>
        /// �^�b�v�������ꂽ�Ƃ��ɔ�������C�x���g
        /// </summary>
        public event EventHandler TapUp;

        /// <summary>
        /// ���T�C�Y�C�x���g
        /// </summary>
        public event EventHandler Resize;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="targetTextBox">����Ώۂ̃e�L�X�g�{�b�N�X</param>
        public LogBoxInputFilter(TextBox targetTextBox) : base(targetTextBox)
        {

        }

        /// <summary>
        /// �E�B���h�E�v���V�[�W��
        /// </summary>
        protected override int WndProc(IntPtr hwnd, uint msg, uint wParam, int lParam)
        {
            // �f�t�H���g����O�ɔ���
            switch (msg)
            {
                case WM_LBUTTONDOWN:  // �^�b�v������
                    if (TapDown != null)
                        TapDown(this, EventArgs.Empty);
                    break;

                case WM_LBUTTONUP:  // �^�b�v������
                    if (TapUp != null)
                        TapUp(this, EventArgs.Empty);
                    break;
            }

            // �f�t�H���g�̃v���V�[�W����
            int hr = base.WndProc(hwnd, msg, wParam, lParam);

            // �f�t�H���g�����ɔ���
            switch (msg)
            {
                case WM_SIZE:
                    if (Resize != null)
                        Resize(this, EventArgs.Empty);
                    break;
            }

            return hr;
        }
    }
}
