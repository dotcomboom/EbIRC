using System;
using System.Collections.Generic;
using System.Text;

namespace EbiSoft.EbIRC
{
    /// <summary>
    /// �L�[�I�y���[�V�����̎��
    /// </summary>
    public enum EbIRCKeyOperations
    {
        /// <summary>
        /// �f�t�H���g����
        /// </summary>
        Default,
        /// <summary>
        /// �N�C�b�N�`�����l���Z���N�g�A����
        /// </summary>
        QuickChannelNext,
        /// <summary>
        /// �N�C�b�N�`�����l���Z���N�g�A�O��
        /// </summary>
        QuickChannelPrev,
        /// <summary>
        /// ���y�[�W��
        /// </summary>
        PageUp,
        /// <summary>
        /// �O�y�[�W��
        /// </summary>
        PageDown,
        /// <summary>
        /// ���̓��O�����߂�
        /// </summary>
        InputLogPrev,
        /// <summary>
        /// ���̓��O�摗��
        /// </summary>
        InputLogNext,
        /// <summary>
        /// �t�H���g�T�C�Y�g��
        /// </summary>
        FontSizeUp,
        /// <summary>
        /// �t�H���g�T�C�Y�k��
        /// </summary>
        FontSizeDown,
        /// <summary>
        /// ����Ȃ�
        /// </summary>
        NoOperation
    }

    /// <summary>
    /// �L�[���[�h�����̕��@
    /// </summary>
    public enum EbIRCHighlightMethod
    {
        /// <summary>
        /// �����Ȃ�
        /// </summary>
        None,
        /// <summary>
        /// �o�C�u���[�V����
        /// </summary>
        Vibration,
        /// <summary>
        /// LED�_��
        /// </summary>
        Led,
        /// <summary>
        /// �o�C�u�{LED
        /// </summary>
        VibrationAndLed
    }

}
