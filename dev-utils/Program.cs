using System.Text;

namespace dev_utils;
internal class Program
{
    static void Main(string[] args)
    {
        var result = Encoding.Default.GetBytes(Sec.GOOGLE_API_KEY).Select((value, idx) => value + 7);
        Console.Write($"byte[] GOOGLE_API_KEY_BYTES = {{{string.Join(',', result)}}};");
    }
}
