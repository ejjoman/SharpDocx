using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpDocx.Extensions
{
    public static class StreamExtensions
    {
#if NET35
        /// <summary>Reads the bytes from the current stream and writes them to another stream, using a specified buffer size.</summary>
        /// <param name="source"></param>
        /// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
        /// <param name="bufferSize">The size of the buffer. This value must be greater than zero. The default size is 81920.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="destination" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is negative or zero.</exception>
        /// <exception cref="T:System.NotSupportedException">The current stream does not support reading.-or-
        /// <paramref name="destination" /> does not support writing.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Either the current stream or <paramref name="destination" /> were closed before the <see cref="M:System.IO.Stream.CopyTo(System.IO.Stream)" /> method was called.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
        public static void CopyTo(this Stream source, Stream destination)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            if (!source.CanRead && !source.CanWrite)
                throw new ObjectDisposedException(nameof(source), "Object is disposed or stream is closed.");

            if (!destination.CanRead && !destination.CanWrite)
                throw new ObjectDisposedException(nameof(destination), "Object is disposed or stream is closed.");

            if (!source.CanRead)
                throw new NotSupportedException("Source is not readable.");

            if (!destination.CanWrite)
                throw new NotSupportedException("Destination is not writable.");

            var buffer = new byte[81920];
            int count;
            while ((count = source.Read(buffer, 0, buffer.Length)) != 0)
                destination.Write(buffer, 0, count);
        }
#endif
    }
}
