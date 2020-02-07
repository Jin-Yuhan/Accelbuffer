using Accelbuffer.Properties;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Accelbuffer.Memory
{
    /// <summary>
    /// 表示一个非托管缓冲区
    /// </summary>
    public unsafe struct NativeBuffer : IEnumerable<byte>, IReadOnlyList<byte>, IDisposable
    {
        private byte* m_Ptr;
        private readonly int m_RealSize;

        /// <summary>
        /// 获取缓冲区的长度
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 获取缓冲区是否被释放
        /// </summary>
        public bool Disposed => m_Ptr == null;

        int IReadOnlyCollection<byte>.Count => Length;

        /// <summary>
        /// 获取/设置缓冲区指定索引的值
        /// </summary>
        /// <param name="index">数据的索引</param>
        /// <returns>索引处的数据</returns>
        /// <exception cref="ObjectDisposedException">当前对象已经被释放</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/>超出范围</exception>
        public byte this[int index]
        {
            get
            {
                if (Disposed)
                {
                    throw new ObjectDisposedException(nameof(NativeBuffer));
                }

                if (index < 0 || index >= Length)
                {
                    throw new IndexOutOfRangeException();
                }

                return m_Ptr[index];
            }
            set
            {
                if (Disposed)
                {
                    throw new ObjectDisposedException(nameof(NativeBuffer));
                }

                if (index < 0 || index >= Length)
                {
                    throw new IndexOutOfRangeException();
                }

                m_Ptr[index] = value;
            }
        }

        internal NativeBuffer(byte* ptr, int length, int realSize)
        {
            m_Ptr = ptr;
            Length = length;
            m_RealSize = realSize;
        }

        private byte GetByte(int index)
        {
            return m_Ptr[index];
        }

        /// <summary>
        /// 释放缓冲区内存
        /// </summary>
        public unsafe void Dispose()
        {
            MemoryAllocator.Shared.Free(m_Ptr, m_RealSize);
            m_Ptr = null;
        }

        /// <summary>
        /// 将缓冲区转换为托管字节数组
        /// </summary>
        /// <returns>与当前对象等价的托管字节数组</returns>
        /// <exception cref="ObjectDisposedException">当前对象已经被释放</exception>
        public byte[] ToArray()
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(nameof(NativeBuffer));
            }

            byte[] result = new byte[Length];
            CopyToArray(result, 0);
            return result;
        }

        /// <summary>
        /// 将缓冲区数据拷贝到托管字节数组中
        /// </summary>
        /// <param name="buffer">拷贝到的托管字节数组</param>
        /// <param name="startIndex">开始写入的索引位置</param>
        /// <returns>写入的数据长度</returns>
        /// <exception cref="ObjectDisposedException">当前对象已经被释放</exception>
        /// <exception cref="ArgumentException"><paramref name="buffer"/>长度不足</exception>
        public unsafe int CopyToArray(byte[] buffer, int startIndex)
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(nameof(NativeBuffer));
            }

            if (buffer.LongLength - startIndex < Length)
            {
                throw new ArgumentException(Resources.ByteArrayTooShort);
            }

            fixed (byte* ptr = buffer)
            {
                byte* src = m_Ptr;
                byte* dst = ptr + startIndex;
                int count = Length;

                while (count-- > 0)
                {
                    *dst++ = *src++;
                }
            }

            return Length;
        }

        /// <summary>
        /// 遍历缓冲区的所有数据
        /// </summary>
        /// <returns>数据迭代器</returns>
        public IEnumerator<byte> GetEnumerator()
        {
            if (Disposed)
            {
                yield break;
            }

            for (int i = 0; i < Length; i++)
            {
                yield return GetByte(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 获取对象内部维护的指针
        /// </summary>
        /// <param name="buffer">需要获取指针的对象</param>
        public static unsafe explicit operator byte* (NativeBuffer buffer)
        {
            return buffer.m_Ptr;
        }

        /// <summary>
        /// 获取对象是否没有被释放
        /// </summary>
        /// <param name="buffer">需要查询状态的对象</param>
        public static implicit operator bool(NativeBuffer buffer)
        {
            return !buffer.Disposed;
        }
    }
}
