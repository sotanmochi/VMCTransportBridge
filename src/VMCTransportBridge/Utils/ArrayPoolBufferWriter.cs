// The original source code is available on GitHub.
// https://github.com/Cysharp/MagicOnion/blob/4.5.2/src/MagicOnion.Shared/Utils/ArrayPoolBufferWriter.cs
//
// -----
//
// MIT License
//
// Copyright (c) Yoshifumi Kawai
// Copyright (c) Cysharp, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// -----

using System;
using System.Buffers;

namespace VMCTransportBridge.Utils
// namespace MagicOnion.Utils
{
    public sealed class ArrayPoolBufferWriter : IBufferWriter<byte>, IDisposable
    // internal sealed class ArrayPoolBufferWriter : IBufferWriter<byte>, IDisposable
    {
        [ThreadStatic]
        static ArrayPoolBufferWriter staticInstance;

        public static ArrayPoolBufferWriter RentThreadStaticWriter()
        {
            if (staticInstance == null)
            {
                staticInstance = new ArrayPoolBufferWriter();
            }
            staticInstance.Prepare();
            return staticInstance;
        }

        const int MinimumBufferSize = 32767; // use 32k buffer.

        byte[] buffer;
        int index;

        void Prepare()
        {
            if (buffer == null)
            {
                buffer = ArrayPool<byte>.Shared.Rent(MinimumBufferSize);
            }
            index = 0;
        }

        public ReadOnlyMemory<byte> WrittenMemory => buffer.AsMemory(0, index);
        public ReadOnlySpan<byte> WrittenSpan => buffer.AsSpan(0, index);

        public int WrittenCount => index;

        public int Capacity => buffer.Length;

        public int FreeCapacity => buffer.Length - index;

        public void Advance(int count)
        {
            if (count < 0) throw new ArgumentException(nameof(count));
            index += count;
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            CheckAndResizeBuffer(sizeHint);
            return buffer.AsMemory(index);
        }

        public Span<byte> GetSpan(int sizeHint = 0)
        {
            CheckAndResizeBuffer(sizeHint);
            return buffer.AsSpan(index);
        }

        void CheckAndResizeBuffer(int sizeHint)
        {
            if (sizeHint < 0) throw new ArgumentException(nameof(sizeHint));

            if (sizeHint == 0)
            {
                sizeHint = MinimumBufferSize;
            }

            int availableSpace = buffer.Length - index;

            if (sizeHint > availableSpace)
            {
                int growBy = Math.Max(sizeHint, buffer.Length);

                int newSize = checked(buffer.Length + growBy);

                byte[] oldBuffer = buffer;

                buffer = ArrayPool<byte>.Shared.Rent(newSize);

                Span<byte> previousBuffer = oldBuffer.AsSpan(0, index);
                previousBuffer.CopyTo(buffer);
                ArrayPool<byte>.Shared.Return(oldBuffer);
            }
        }

        public void Dispose()
        {
            if (buffer == null)
            {
                return;
            }

            ArrayPool<byte>.Shared.Return(buffer);
            buffer = null;
        }
    }
}