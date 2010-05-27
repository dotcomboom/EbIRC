using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace EbiSoft.EbIRC
{
    /// <summary>
    /// ���̓e�L�X�g�{�b�N�X��IME���͏�Ԃ��Ď�����N���X
    /// </summary>
    class InputBoxInputFilter : TextBoxInputFilter
    {
        #region P/Invoke �錾

        [DllImport("coredll.dll")]
        public static extern IntPtr FindWindow(string className, string WindowsName);

        [DllImport("coredll.dll")]
        private static extern int IsWindowVisible(IntPtr hWnd);

        // IME�ϊ��̃E�B���h�E���b�Z�[�W
        private const uint WM_IME_STARTCOMPOSITION = 0x10D; // IME�ϊ��J�n
        private const uint WM_IME_ENDCOMPOSITION = 0x10E;   // IME�ϊ��I��

        // �}�E�X�z�C�[���֘A
        private const int WM_MOUSEWHEEL = 0x20A;

        // ATOK�����ϊ��E�B���h�E�̃E�B���h�E�N���X
        private const string ATOK_CONJECTURE_CLASS = "ATOKMConjecture";

        #endregion

        /// <summary>
        /// IME�̓��͂��J�n�����Ƃ��ɔ�������C�x���g
        /// </summary>
        public event EventHandler StartComposition;

        /// <summary>
        /// IME�̓��͂��I�������Ƃ��ɔ�������C�x���g
        /// </summary>
        public event EventHandler EndComposition;

        /// <summary>
        /// �}�E�X�z�C�[���E�W���O�z�C�[����������ɓ������Ƃ��ɔ�������C�x���g
        /// </summary>
        public event EventHandler MouseWheelMoveUp;

        /// <summary>
        /// �}�E�X�z�C�[���E�W���O�z�C�[�����������ɓ������Ƃ��ɔ�������C�x���g
        /// </summary>
        public event EventHandler MouseWheelMoveDown;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="targetTextBox">����Ώۂ̃e�L�X�g�{�b�N�X</param>
        public InputBoxInputFilter(TextBox targetTextBox) : base(targetTextBox)
        {

        }

        /// <summary>
        /// �E�B���h�E�v���V�[�W��
        /// </summary>
        protected override int WndProc(IntPtr hwnd, uint msg, uint wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_IME_STARTCOMPOSITION:  // IME �ϊ��J�n
                    m_compositioning = true;
                    if (StartComposition != null)
                        StartComposition(this, EventArgs.Empty);
                    break;

                case WM_IME_ENDCOMPOSITION:    // IME �ϊ��I��
                    m_compositioning = false;
                    if (EndComposition != null)
                        EndComposition(this, EventArgs.Empty);
                    break;

                case WM_MOUSEWHEEL:

                    int delta = GetWheelDeltaWParam(wParam);

                    if (WHEEL_DELTA <= delta)
                    {
                        if (MouseWheelMoveUp != null)
                            MouseWheelMoveUp(this, EventArgs.Empty);
                    }
                    else if (delta <= -WHEEL_DELTA)
                    {
                        if (MouseWheelMoveDown != null)
                            MouseWheelMoveDown(this, EventArgs.Empty);
                    }

                    break;

            }

            // �f�t�H���g�̃v���V�[�W����
            return base.WndProc(hwnd, msg, wParam, lParam);
        }

        /// <summary>
        /// ATOK�����ϊ����A�N�e�B�u���ǂ���
        /// </summary>
        /// <returns>ATOK�����ϊ����A�N�e�B�u�Ȃ� true</returns>
        public bool IsAtokConjectureActive()
        {
            IntPtr atokPtr = FindWindow(ATOK_CONJECTURE_CLASS, null);

            if (atokPtr != IntPtr.Zero)
            {
                int hr = IsWindowVisible(atokPtr);
                return hr == 1;
            }
            else
            {
                return false;
            }
        }

        #region �}�E�X�z�C�[��(�W���O�_�C�A��)�֘A

        private int GetWheelDeltaWParam(uint wParam)
        {
            return (int)HiWord(wParam);
        }

        private int HiWord(uint l)
        {
            return (int)l / 65536;
        }

        //int deltaSum = 0;
        int WHEEL_DELTA = 120;

        #endregion  

        #region �v���p�e�B

        /// <summary>
        /// �ϊ������ǂ���
        /// </summary>
        public bool Conpositioning
        {
            get { return m_compositioning || IsAtokConjectureActive(); }
        }
        private bool m_compositioning = false;

        #endregion
    }
}
