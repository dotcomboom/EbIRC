using System;
using System.Collections.Generic;
using System.Text;

namespace EbiSoft.Library
{
    /// <summary>
    /// �Œ�T�C�Y�̔z����g���������O�o�b�t�@
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class RingBuffer<T> : IEnumerable<T>, IList<T>
    {
        private T[] m_array;    // �i�[���Ă���f�[�^�̔z��
        private int m_count;    // �i�[���Ă���f�[�^�̌�
        private int m_min;      // �i�[���Ă���f�[�^�̊J�n�ʒu
        private int m_max;      // �i�[���Ă���f�[�^�̏I���ʒu

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="length">�z��̒���</param>
        public RingBuffer(int length)
        {
            m_array = new T[length];
            Clear();
        }

        /// <summary>
        /// �w�肳�ꂽ�_���C���f�b�N�X����A���C���f�b�N�X���Z�o����
        /// </summary>
        /// <param name="index">�_���C���f�b�N�X(�O���炱�̃N���X�������Ƃ��̃C���f�b�N�X)</param>
        /// <returns>���C���f�b�N�X(m_array�̃C���f�b�N�X)</returns>
        private int GetRealIndex(int index)
        {
            // �͈͊O
            if ((index < 0) || (index >= Count))
            {
                return -1;
            }

            if (m_max < m_min)
            {
                int rindex = index + m_min;
                if (rindex >= m_array.Length)
                {
                    return rindex - m_array.Length;
                }
                else
                {
                    return rindex;
                }
            }
            else
            {
                return index - m_min;
            }
        }

        #region IList<T> �����o

        /// <summary>
        /// �w�肵�����ڂ̃C���f�b�N�X�𒲂ׂ܂��B
        /// </summary>
        /// <param name="item">�������� T</param>
        /// <returns>���X�g�ɑ��݂���ꍇ�� item �̃C���f�b�N�X�B����ȊO�̏ꍇ�� -1�B</returns>
        public int IndexOf(T item)
        {
            // ����ۂȂ猟�����s
            if (Count == 0) return -1;

            // ��������
            for (int i = 0; i < this.Count; i++)
            {
                if (item.Equals(this[i]))
                {
                    return i;
                }
            }

            // �Y������A�C�e���Ȃ�
            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException("The method or operation is not implemented."); ;
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// �w�肵���C���f�b�N�X�ɂ���v�f���擾�܂��͐ݒ肵�܂��B
        /// </summary>
        /// <param name="index">�擾�܂��͐ݒ肷��v�f�́A0 ����n�܂�C���f�b�N�X�ԍ��B</param>
        /// <returns> �w�肵���C���f�b�N�X�ɂ���v�f�B</returns>
        public T this[int index]
        {
            get
            {
                int rindex = GetRealIndex(index);
                if (rindex == -1)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return m_array[rindex];
            }
            set
            {
                int rindex = GetRealIndex(index);
                if (rindex == -1)
                {
                    throw new ArgumentOutOfRangeException();
                }
                m_array[rindex] = value;
            }
        }

        #endregion

        #region ICollection<T> �����o

        /// <summary>
        /// �A�C�e����ǉ�����
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            // �C���f�b�N�X�����ɐi�߂�
            m_max++;
            if (m_max == m_array.Length)
            {
                m_max = 0;
                m_min = 1;
            }
            if (m_min == m_max)
            {
                m_min++;
            }
            if (m_count == 0)
            {
                m_min = m_max;
            }

            // �J�E���g�𑝂₷
            if (m_count < m_array.Length)
            {
                m_count++;
            }

            // �Z�b�g
            m_array[m_max] = item;
        }

        /// <summary>
        /// ���̃��X�g���N���A����
        /// </summary>
        public void Clear()
        {
            m_array = new T[m_array.Length];
            m_count = 0;
            m_min = -1;
            m_max = -1;
        }

        /// <summary>
        /// �A�C�e�������݂��邩�ǂ������ׂ�
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            foreach (T target in this)
            {
                if (item.Equals(target))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// �f�[�^�̌�
        /// </summary>
        public int Count
        {
            get { return m_count; }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsReadOnly
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool Remove(T item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<T> �����o

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IEnumerable �����o

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        /// <summary>
        /// �����O�o�b�t�@�̓��e��z��ɕϊ�����
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] returnValue = new T[this.Count];

            for (int i = 0; i < this.Count; i++)
            {
                returnValue[i] = this[i];
            }

            return returnValue;
        }

        private class Enumerator : IEnumerator<T>
        {
            RingBuffer<T> m_list;
            T m_current;
            int index;

            #region IEnumerator<T> �����o

            public T Current
            {
                get
                {
                    return m_current;
                }
            }

            #endregion

            #region IDisposable �����o

            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator �����o

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return m_current;
                }
            }

            public bool MoveNext()
            {
                index++;
                if (index < m_list.Count)
                {
                    m_current = m_list[index];
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                index = 0;
                m_current = m_list[index];
            }

            #endregion

            public Enumerator(RingBuffer<T> list)
            {
                m_list = list;
                Reset();
            }
        }
    }
}
