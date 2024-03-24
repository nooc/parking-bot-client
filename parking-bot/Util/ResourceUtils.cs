using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingBot.Util;

public sealed class ResourceUtils
{
    private static readonly int BUF_SIZE = 2048;
    private record BufferSpec(byte[] Buf, int Len);

    public static async Task<byte[]> GetRawResourceBytes(string name)
    {
        List<BufferSpec> bufferList = [];
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(name);
            var buf = new byte[BUF_SIZE];
            int readLength;
            while ((readLength = stream.Read(buf, 0, BUF_SIZE)) > 0)
            {
                bufferList.Add(new BufferSpec(buf, readLength));
            }
        } catch { }

        var totalLength = bufferList.Sum(spec => spec.Len);
        var concatBuf = new byte[totalLength];
        int pos = 0;
        foreach(var spec in bufferList)
        {
            Array.Copy(spec.Buf, 0, concatBuf, pos, spec.Len);
            pos += spec.Len;
        }
        return concatBuf;
    }

    public static async Task<string> GetRawResourceText(string name)
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(name);
            var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
        catch { }
        return string.Empty;
    }
}
