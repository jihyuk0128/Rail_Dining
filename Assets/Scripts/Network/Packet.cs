using System;
using System.IO;
using System.Text;

public class PacketWriter : IDisposable
{
    MemoryStream _stream = new MemoryStream();
    BinaryWriter _writer;

    public PacketWriter()
    {
        _writer = new BinaryWriter(_stream, Encoding.UTF8);
    }

    public void WriteInt(int value) => _writer.Write(value);

    public void WriteString(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        _writer.Write(bytes.Length);
        _writer.Write(bytes);
    }

    public byte[] ToArrayWithLengthPrefix()
    {
        byte[] body = _stream.ToArray();
        using (var final = new MemoryStream())
        {
            BinaryWriter w = new BinaryWriter(final);
            w.Write(body.Length);
            w.Write(body);
            return final.ToArray();
        }
    }

    public void Dispose() => _stream.Dispose();
}

public class PacketReader : IDisposable
{
    BinaryReader _reader;
    public PacketReader(byte[] data)
    {
        _reader = new BinaryReader(new MemoryStream(data), Encoding.UTF8);
    }

    public int ReadInt() => _reader.ReadInt32();

    public string ReadString()
    {
        int len = _reader.ReadInt32();
        byte[] bytes = _reader.ReadBytes(len);
        return Encoding.UTF8.GetString(bytes);
    }

    public void Dispose() => _reader.Dispose();
}
