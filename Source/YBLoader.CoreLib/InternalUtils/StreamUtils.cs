using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YBLoader.CoreLib.InternalUtils
{
    internal static class StreamUtils
    {
        public static IEnumerable<Stream> DuplicateStream(Stream source, int count)
        {
            var bufferStream = new MemoryStream();

            using (var br = new BinaryReader(source, Encoding.ASCII, true))
            using (var bw = new BinaryWriter(bufferStream, Encoding.ASCII, true))
                bw.Write(br.ReadBytes((int)source.Length));

            for (var i = 0; i < count; i++)
            {
                var duplicatedStream = new MemoryStream();
                bufferStream.Seek(0, SeekOrigin.Begin);

                using (var br = new BinaryReader(bufferStream, Encoding.ASCII, true))
                using (var bw = new BinaryWriter(duplicatedStream, Encoding.ASCII, true))
                    bw.Write(br.ReadBytes((int)source.Length));

                duplicatedStream.Seek(0, SeekOrigin.Begin);
                yield return duplicatedStream;
            }
        }
    }
}
