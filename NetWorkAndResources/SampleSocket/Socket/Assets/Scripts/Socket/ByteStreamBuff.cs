using System;
using System.IO;
using System.Text;

/// <summary>
/// 二进制数据流
/// </summary>
public class ByteStreamBuff
{
    private MemoryStream stream = null;
    private BinaryWriter writer = null;
    private BinaryReader reader = null;

    public ByteStreamBuff()
    {
        stream = new MemoryStream();
        writer = new BinaryWriter(stream);
    }

    public ByteStreamBuff(byte[] _data)
    {
        stream = new MemoryStream(_data);
        reader = new BinaryReader(stream);
    }

    public void Write_Byte(byte _data)
    {
        writer.Write(_data);
    }
    public void Write_Bytes(byte[] _data)
    {
        writer.Write(_data.Length);
        writer.Write(_data);
    }
    public void Write_Int(int _data)
    {
        writer.Write(_data);
    }
    public void Write_uInt(uint _data)
    {
        writer.Write(_data);
    }
    public void Write_Short(short _data)
    {
        writer.Write(_data);
    }
    public void Write_uShort(ushort _data)
    {
        writer.Write(_data);
    }
    public void Write_Long(long _data)
    {
        writer.Write(_data);
    }
    public void Write_uLong(ulong _data)
    {
        writer.Write(_data);
    }
    public void Write_Bool(bool _data)
    {
        writer.Write(_data);
    }
    public void Write_Float(float _data)
    {
        byte[] temp = flip(BitConverter.GetBytes(_data));
        writer.Write(temp.Length);
        writer.Write(BitConverter.ToSingle(temp, 0));
    }
    public void Write_Double(double _data)
    {
        byte[] temp = flip(BitConverter.GetBytes(_data));
        writer.Write(temp.Length);
        writer.Write(BitConverter.ToDouble(temp, 0));
    }
    public void Write_UTF8String(string _data)
    {
        byte[] temp = Encoding.UTF8.GetBytes(_data);
        writer.Write(temp.Length);
        writer.Write(temp);
    }
    public void Write_UniCodeString(string _data)
    {
        byte[] temp = Encoding.Unicode.GetBytes(_data);
        writer.Write(temp.Length);
        writer.Write(temp);
    }

    public byte Read_Byte()
    {
        return reader.ReadByte();
    }
    public byte[] Read_Bytes()
    {
        int len = Read_Int();
        return reader.ReadBytes(len);
    }
    public int Read_Int()
    {
        return reader.ReadInt32();
    }
    public uint Read_uInt()
    {
        return reader.ReadUInt32();
    }
    public short Read_Short()
    {
        return reader.ReadInt16();
    }
    public ushort Read_uShort()
    {
        return reader.ReadUInt16();
    }
    public long Read_Long()
    {
        return reader.ReadInt64();
    }
    public ulong Read_uLong()
    {
        return reader.ReadUInt64();
    }
    public bool Read_Bool()
    {
        return reader.ReadBoolean();
    }
    public float Read_Float()
    {
        return BitConverter.ToSingle(flip(Read_Bytes()), 0);
    }
    public double Read_Double()
    {
        return BitConverter.ToDouble(flip(Read_Bytes()), 0);
    }
    public string Read_UTF8String()
    {
        return Encoding.UTF8.GetString(Read_Bytes());
    }
    public string Read_UniCodeString()
    {
        return Encoding.Unicode.GetString(Read_Bytes());
    }

    private byte[] flip(byte[] _data)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(_data);
        return _data;
    }
    public byte[] ToArray()
    {
        stream.Flush();
        return stream.ToArray();
    }

    public void Close()
    {
        if (stream != null)
        {
            stream.Close();
            stream = null;
        }
        if (writer != null)
        {
            writer.Close();
            writer = null;
        }
        if (reader != null)
        {
            reader.Close();
            reader = null;
        }
    }
}