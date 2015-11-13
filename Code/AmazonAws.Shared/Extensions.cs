using System.IO;
using System.Text;

namespace AmazonAws.Shared
{
    public static class Extensions
    {
        public static MemoryStream ToMemoryStream(this Stream stream)
        {
            var memoryStream = new MemoryStream();

            stream.CopyTo(memoryStream);

            return memoryStream;
        }

        public static byte[] ToByteArray(this Stream stream)
        {
            using (var memoryStream = stream.ToMemoryStream())
            {
                stream.CopyTo(memoryStream);

                return memoryStream.ToArray();
            }
        }

        public static string FromBytes(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static string ToContentString(this Stream stream)
        {
            return stream.ToByteArray().FromBytes();
        }
    }
}