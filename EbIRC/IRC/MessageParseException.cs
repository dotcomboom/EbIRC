using System;

namespace EbiSoft.EbIRC.IRC {
	/// <summary>
	/// ���b�Z�[�W��͎��s��O
	/// </summary>
	public class MessageParseException : Exception {
		public MessageParseException(Exception innerException) : base("���b�Z�[�W�̉�͂Ɏ��s���܂����B", innerException)
		{
			
		}
	}
}
