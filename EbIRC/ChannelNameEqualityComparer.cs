using System;
using System.Collections.Generic;
using System.Text;

namespace EbiSoft.EbIRC
{
    /// <summary>
    /// �`�����l������r�p
    /// </summary>
    class ChannelNameEqualityComparer : IEqualityComparer<string>
    {
        #region IEqualityComparer<string> �����o

        public bool Equals(string x, string y)
        {
            return x.ToLower().Equals(y.ToLower());
        }

        public int GetHashCode(string obj)
        {
            return obj.ToLower().GetHashCode();
        }

        #endregion
    }
}
