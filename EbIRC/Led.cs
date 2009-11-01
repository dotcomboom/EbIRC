using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

// EbIRC Custom �̃\�[�X���炢�������܂����B
// Special Thanks to icchu

namespace EbiSoft.EbIRC
{
    /// <summary>
    /// LED�A�o�C�u���[�V�����̐�����s���N���X
    /// </summary>
    public class Led
    {
        //W-ZERO3
        //LED,�o�C�u�̐��� API���Ăяo���N���X
        //
        //�g����
        //����.cs�t�@�C�����u�������ڂ̒ǉ��v�ł����ăv���W�F�N�g�ɑg�ݍ���ł���Ă��������B
        //�֐��̌Ăяo���̓X�^�e�B�b�N�ŁB
        //
        //led.SetLedStatus(int wLed, int wStatus)
        //
        // �����F
        //       wLed       �ǂ�LED���̏��� 0 = �d��LED�����F�� 1 = �o�C�u���[�V����
        //       wStatus    0 = Off, 1 = On, 2 = Blink 

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern bool NLedGetDeviceInfo(uint h, ref NLED_COUNT_INFO pOutput);
        [DllImport("coredll.dll", SetLastError = true)]
        private static extern bool NLedSetDevice(uint h, ref NLED_SETTINGS_INFO pOutput);

        [StructLayout(LayoutKind.Sequential)]
        private struct NLED_SETTINGS_INFO
        {
            public uint LedNum;
            public int OffOnBlink;
            public long TotalCycleTime;
            public long OnTime;
            public long OffTime;
            public int MetaCycleOn;
            public int MetaCycleOff;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct NLED_COUNT_INFO
        {
            public uint cLeds;
        }

        private const uint NLED_COUNT_INFO_ID = 0;
        private const uint NLED_SETTINGS_INFO_ID = 2;

        public static int GetLedCount()
        {
            NLED_COUNT_INFO countInfo = new NLED_COUNT_INFO();
            int LEDCount = 0;
            if (NLedGetDeviceInfo(NLED_COUNT_INFO_ID, ref countInfo))
            {
                LEDCount = (int)countInfo.cLeds;
            }
            return LEDCount;
        }

        public static bool AvailableLed(LedType led)
        {
            return AvailableLed((int)led);
        }

        public static bool AvailableLed(int led)
        {
            return led < GetLedCount();
        }

        public static void SetLedStatus(LedType led, LedStatus status)
        {
            SetLedStatus((int)led, (int)status);
        }

        public static void SetLedStatus(int wLed, int wStatus)
        {
            if (!AvailableLed(wLed)) throw new InvalidOperationException();

            NLED_SETTINGS_INFO settingInfo = new NLED_SETTINGS_INFO();
            settingInfo.LedNum = System.Convert.ToUInt32(wLed);
            settingInfo.OffOnBlink = System.Convert.ToUInt16(wStatus);
            settingInfo.OnTime = 1000000;
            settingInfo.OffTime = 500000;
            //settingInfo.MetaCycleOn = 1;
            //settingInfo.MetaCycleOff = 20;
            NLedSetDevice(NLED_SETTINGS_INFO_ID, ref settingInfo);

        }
    }

    /// <summary>
    /// LED�̎��
    /// </summary>
    public enum LedType
    {
        /// <summary>
        /// ���F
        /// </summary>
        Yellow = 0,
        /// <summary>
        /// �o�C�u
        /// </summary>
        Vibrartion = 1
    }

    public enum LedStatus
    {
        /// <summary>
        /// �I�t
        /// </summary>
        Off = 0,
        /// <summary>
        /// �I��
        /// </summary>
        On = 1,
        /// <summary>
        /// �_��
        /// </summary>
        Blink = 2
    }
}
