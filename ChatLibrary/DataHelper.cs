using System.Net.Sockets;
using System.Text;

namespace ChatLibrary
{
    public static class DataHelper
    {
        public static string GetData(NetworkStream stream)
        {
            var data = new byte[64];
            var builder = new StringBuilder();
            do
            {
                var bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (stream.DataAvailable);

            return builder.ToString();
        }

        public static void SendData(NetworkStream stream, string data)
        {
            var buffer = Encoding.Unicode.GetBytes(data ?? string.Empty);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}