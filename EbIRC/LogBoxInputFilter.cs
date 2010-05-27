using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

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
        private const int WM_GESTURE = 0x0119;

        // �W�F�X�`���֘A
        private const int GID_BEGIN = 1;
        private const int GID_END = 2;
        private const int GID_PAN = 4;
        private const int GID_SCROLL = 8;
        private const int GID_HOLD = 9;
        private const int GID_SELECT = 10;
        private const int GID_DOUBLESELECT = 11;
        private const int GID_LAST = 11;

        private const int ARG_SCROLL_NONE = 0;
        private const int ARG_SCROLL_DOWN = 1;
        private const int ARG_SCROLL_LEFT = 2;
        private const int ARG_SCROLL_UP = 3;
        private const int ARG_SCROLL_RIGHT = 4;

        [StructLayout(LayoutKind.Sequential)]
        private struct GESTUREINFO
        {
            public uint cbSize;
            public uint dwFlags;              /* Gesture Flags */
            public uint dwID;                 /* Gesture ID */
            public IntPtr hwndTarget;            /* HWND of target window */
            public POINTS ptsLocation;         /* Coordinates of start of gesture */
            public uint dwInstanceID;         /* Gesture Instance ID */
            public uint dwSequenceID;         /* Gesture Sequence ID */
            public ulong ullArguments;     /* Arguments specific to gesture */
            public uint cbExtraArguments;      /* Size of extra arguments in bytes */
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINTS
        {
            public short x;
            public short y;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("coredll", SetLastError = true)]
        private static extern bool GetGestureInfo(IntPtr hGestureInfo, ref GESTUREINFO pGestureInfo);

        private int GetScrollAngle(ulong x)
        {
            unchecked
            {
                return (ushort)((ushort)(x >> 48) & 0xfff0);
            }
        }

        private int GetScrollDirection(ulong x)
        {
            unchecked
            {
                return (ushort)((ushort)(x >> 48) & 0x000f);
            }
        }

        private int GetScrollVelocity(ulong x)
        {
            unchecked
            {
                return (int)(short)(ushort)(x >> 32);
            }
        }

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
        /// �^�b�v�������ꂽ�Ƃ��ɔ�������C�x���g
        /// </summary>
        public event EventHandler<FlickEventArgs> Flick;


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
        protected override int WndProc(IntPtr hwnd, uint msg, uint wParam, IntPtr lParam)
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
                case WM_GESTURE:
                    ProcessGuesture(hwnd, lParam);
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

        private void ProcessGuesture(IntPtr hwnd, IntPtr lParam)
        {
            GESTUREINFO gestureInfo = new GESTUREINFO();
            gestureInfo.cbSize = (uint)Marshal.SizeOf(typeof(GESTUREINFO));

            // Windows Mobile 6.5 �����ł͓����Ȃ��̂�return�B
            // TODO �����ƃr���h���ׂ�
            if (System.Environment.OSVersion.Version.Build < 20000)
                return;

            if (GetGestureInfo(lParam, ref gestureInfo))
            {
                switch (gestureInfo.dwFlags)
                {
                    // �t���b�N
                    case GID_SCROLL:
                        FlickDirection direction = (FlickDirection)GetScrollDirection(gestureInfo.ullArguments);
                        if (this.Flick != null)
                            Flick(this, new FlickEventArgs(direction));
                        break;
                }
            }
        }
    }

    /// <summary>
    /// �t���b�N�C�x���g�̃f�[�^
    /// </summary>
    public class FlickEventArgs : EventArgs
    {
        public readonly FlickDirection Direction;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public FlickEventArgs(FlickDirection direction)
        {
            this.Direction = direction;
        }
    }

    /// <summary>
    /// �t���b�N�̌���
    /// </summary>
    public enum FlickDirection
    {
        None,
        Down,
        Left,
        Up,
        Right
    }
}
