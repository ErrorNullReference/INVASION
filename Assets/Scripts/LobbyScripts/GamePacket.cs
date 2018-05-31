using System;
using System.Collections.Generic;
/// <summary>
/// Class that manages a packet data. Static members should be thread safe, instance members are not thread safe. Ideally GamePackets should be used with a limited set amout of different MaxCapacity values with UsePools active, which will allow for easy pooling with as little allocations as possible. GamePackets that have finished their use should be disposed through DisposePacket
/// </summary>
public class GamePacket
{
    private static readonly object locker = new object();

    /// <summary>
    /// Determines whenever pools should be used. Pools are divided through MaxCapacity
    /// </summary>
    public static bool UsePools { get; private set; }

    private static Dictionary<int, Queue<GamePacket>> packetsPools;

    static GamePacket()
    {
        UsePools = true;
        packetsPools = new Dictionary<int, Queue<GamePacket>>();
    }
    /// <summary>
    /// Gets an initializated packet instance
    /// </summary>
    /// <param name="maxCapacity">max packet capacity</param>
    /// <returns>initializated instance</returns>
    public static GamePacket CreatePacket(int maxCapacity)
    {
        GamePacket res = null;

        if (maxCapacity <= 0)
            throw new ArgumentException("Capacity cannot be equal or less than 0", "maxCapacity");

        lock (locker)
            if (UsePools)
            {
                if (packetsPools.ContainsKey(maxCapacity))
                {
                    Queue<GamePacket> list = packetsPools[maxCapacity];
                    if (list.Count > 0)
                        res = list.Dequeue();
                }
            }

        if (res == null)
            res = new GamePacket(maxCapacity);

        res.Reset();

        return res;
    }
    /// <summary>
    /// Gets an initializated packet instance. MaxCapacity will be equal to dala.Length
    /// </summary>
    /// <param name="data">buffer to use as the internal buffer of the packet. MaxCapacity == data.Length</param>
    /// <returns>initializated instance</returns>
    public static GamePacket CreatePacket(byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException("data", "buffer data must not be null");
        if (data.Length <= 0)
            throw new ArgumentException("buffer must have at least Length > 0", "data");

        GamePacket res = new GamePacket(data);

        res.Reset();

        return res;
    }
    /// <summary>
    /// Performs a clear in all internal pools, releasing all resources
    /// </summary>
    public static void ClearInternalPools()
    {
        lock (locker)
            packetsPools.Clear();
    }
    /// <summary>
    /// Setter for the UsePools variable. Pools are divided through MaxCapacity
    /// </summary>
    /// <param name="usePools">Determines whenever pools should be used. Pools are divided through MaxCapacity</param>
    public static void SetUsePools(bool usePools)
    {
        lock (locker)
            UsePools = usePools;
    }


    /// <summary>
    /// Amount of bytes that has been written in the packet. This may not return a correct value if already written data in the packet has been overwritten or if methods other than the given Write methods in this class has been used to modify the internal buffer, and it must be reset manually. GamePackets created through CreatePacket will be already resetted. This value has no impact in the internal logic of GamePacket and should be used as a way to store the current amount of written byte data
    /// </summary>
    public int CurrentLength { get; set; }
    /// <summary>
    /// Current seek position in the packet, used for all Read/Write operations
    /// </summary>
    public int CurrentSeek { get; set; }
    /// <summary>
    /// Determines whenever the current instance has been disposed through DestroyPacket. Disposed packets should not be used
    /// </summary>
    public bool IsDisposed { get; private set; }
    /// <summary>
    /// Max capacity of the packet. Write/Read operations that go beyond this limit will throw exceptions
    /// </summary>
    public int MaxCapacity { get { return Data.Length; } }
    /// <summary>
    /// Internal buffer used by the packet Changes done to the buffer directly will not automatically modify the other variables
    /// </summary>
    public byte[] Data { get; private set; }

    /// <summary>
    /// Operation that disposes the current packet. if UsePools is true the disposed instance will be pooled, id UsePools is false it does nothing
    /// </summary>
    public void DisposePacket()
    {
        if (!IsDisposed)
        {
            Queue<GamePacket> list;

            lock (locker)
            {
                if (UsePools)
                {
                    if (!packetsPools.ContainsKey(MaxCapacity))
                    {
                        list = new Queue<GamePacket>();
                        packetsPools.Add(MaxCapacity, list);
                    }
                    else
                        list = packetsPools[MaxCapacity];

                    list.Enqueue(this);
                }
            }

            IsDisposed = true;
        }
    }
    /// <summary>
    /// Copies the internal buffer of the given gamepacket to the current instance. Elements copied are equal to the minimum of the 2 MaxCapacity.
    /// </summary>
    /// <param name="instanceDataOffset">offset of the current instance internal buffer</param>
    /// <param name="toCopy">gamepacket to copy from</param>
    /// <param name="toCopyOffset">offset of the given packet</param>
    public void Copy(int instanceDataOffset, GamePacket toCopy, int toCopyOffset)
    {
        int v1 = MaxCapacity - instanceDataOffset;
        int v2 = toCopy.MaxCapacity - toCopyOffset;
        int l = v1 > v2 ? v1 : v2;

        WriteByteData(toCopy, toCopyOffset, instanceDataOffset, l);
    }
    /// <summary>
    /// Reads the internal buffer into the given array
    /// </summary>
    /// <param name="buffer">buffer on which the internal buffer elements will be written</param>
    /// <param name="bufferOffset">buffer offset</param>
    /// <param name="dataOffset">internal buffer offset</param>
    /// <param name="lengthToRead">number of bytes to read</param>
    public void ReadByteData(byte[] buffer, int bufferOffset, int dataOffset, int lengthToRead)
    {
        CurrentSeek = dataOffset;
        ReadByteData(buffer, bufferOffset, lengthToRead);
    }
    /// <summary>
    /// Reads the internal buffer into the given array
    /// </summary>
    /// <param name="buffer">buffer on which the internal buffer elements will be written. Buffer Seek will not be moved</param>
    /// <param name="bufferOffset">buffer offset</param>
    /// <param name="dataOffset">internal buffer offset</param>
    /// <param name="lengthToRead">number of bytes to read</param>
    public void ReadByteData(GamePacket buffer, int bufferOffset, int dataOffset, int lengthToRead)
    {
        CurrentSeek = dataOffset;
        ReadByteData(buffer, bufferOffset, lengthToRead);
    }
    /// <summary>
    /// Reads the internal buffer into the given array
    /// </summary>
    /// <param name="buffer">buffer on which the internal buffer elements will be written. Buffer Seek will not be moved</param>
    /// <param name="bufferOffset">buffer offset</param>
    /// <param name="lengthToRead">number of bytes to read</param>
    public void ReadByteData(GamePacket buffer, int bufferOffset, int lengthToRead)
    {
        ReadByteData(buffer.Data, bufferOffset, lengthToRead);
    }
    /// <summary>
    /// Reads the internal buffer into the given array
    /// </summary>
    /// <param name="buffer">buffer on which the internal buffer elements will be written</param>
    /// <param name="bufferOffset">buffer offset</param>
    /// <param name="lengthToRead">number of bytes to read</param>
    public void ReadByteData(byte[] buffer, int bufferOffset, int lengthToRead)
    {
        Utils.Write(Data, CurrentSeek, buffer, bufferOffset, lengthToRead);
        CurrentSeek += lengthToRead;
    }
    /// <summary>
    /// Writes the given byte array into the internal buffer
    /// </summary>
    /// <param name="buffer">buffer from which elements will be written on the internal buffer</param>
    /// <param name="bufferOffset">buffer offset</param>
    /// <param name="lengthToWrite">number of bytes to write</param>
    public void WriteByteData(byte[] buffer, int bufferOffset, int lengthToWrite)
    {
        Utils.Write(buffer, bufferOffset, Data, CurrentSeek, lengthToWrite);
        CurrentLength += lengthToWrite;
        CurrentSeek += lengthToWrite;
    }
    /// <summary>
    /// Writes the given byte array into the internal buffer
    /// </summary>
    /// <param name="buffer">buffer from which elements will be written on the internal buffer</param>
    /// <param name="bufferOffset">buffer offset</param>
    /// <param name="dataOffset">internal buffer offset</param>
    /// <param name="lengthToWrite">number of bytes to write</param>
    public void WriteByteData(byte[] buffer, int bufferOffset, int dataOffset, int lengthToWrite)
    {
        CurrentSeek = dataOffset;
        WriteByteData(buffer, bufferOffset, lengthToWrite);
    }
    /// <summary>
    /// Writes the given byte array into the internal buffer
    /// </summary>
    /// <param name="buffer">buffer from which elements will be written on the internal buffer. Buffer Seek will not be moved</param>
    /// <param name="bufferOffset">buffer offset</param>
    /// <param name="dataOffset">internal buffer offset</param>
    /// <param name="lengthToWrite">number of bytes to write</param>
    public void WriteByteData(GamePacket buffer, int bufferOffset, int dataOffset, int lengthToWrite)
    {
        CurrentSeek = dataOffset;
        WriteByteData(buffer, bufferOffset, lengthToWrite);
    }
    /// <summary>
    /// Writes the given byte array into the internal buffer
    /// </summary>
    /// <param name="buffer">buffer from which elements will be written on the internal buffer. Buffer Seek will not be moved</param>
    /// <param name="bufferOffset">buffer offset</param>
    /// <param name="lengthToWrite">number of bytes to write</param>
    public void WriteByteData(GamePacket buffer, int bufferOffset, int lengthToWrite)
    {
        WriteByteData(buffer.Data, bufferOffset, lengthToWrite);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="values">values to write</param>
    public void Write(char[] values)
    {
        int n = Utils.Write(Data, CurrentSeek, values);
        CurrentSeek += n;
        CurrentLength += n;
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <param name="values">values to write</param>
    public void Write(int offset, char[] values)
    {
        CurrentSeek = offset;
        Write(values);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    public void Write(string value)
    {
        int n = Utils.Write(Data, CurrentSeek, value);

        CurrentSeek += n;
        CurrentLength += n;
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    /// <param name="offset">move the seek at this offset</param>
    public void Write(string value, int offset)
    {
        CurrentSeek = offset;
        Write(value);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    public void Write(char value)
    {
        Utils.Write(Data, CurrentSeek, value);
        CurrentSeek += sizeof(char);
        CurrentLength += sizeof(char);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    /// <param name="offset">move the seek at this offset</param>
    public void Write(char value, int offset)
    {
        CurrentSeek = offset;
        Write(value);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    public void Write(float value)
    {
        Utils.Write(Data, CurrentSeek, value);
        CurrentSeek += sizeof(float);
        CurrentLength += sizeof(float);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    /// <param name="offset">move the seek at this offset</param>
    public void Write(float value, int offset)
    {
        CurrentSeek = offset;
        Write(value);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    public void Write(double value)
    {
        Utils.Write(Data, CurrentSeek, value);
        CurrentSeek += sizeof(double);
        CurrentLength += sizeof(double);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    /// <param name="offset">move the seek at this offset</param>
    public void Write(double value, int offset)
    {
        CurrentSeek = offset;
        Write(value);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    public void Write(bool value)
    {
        Utils.Write(Data, CurrentSeek, value);
        CurrentSeek += sizeof(bool);
        CurrentLength += sizeof(bool);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    /// <param name="offset">move the seek at this offset</param>
    public void Write(bool value, int offset)
    {
        CurrentSeek = offset;
        Write(value);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    public void Write(short value)
    {
        Utils.Write(Data, CurrentSeek, value);
        CurrentSeek += sizeof(short);
        CurrentLength += sizeof(short);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    /// <param name="offset">move the seek at this offset</param>
    public void Write(short value, int offset)
    {
        CurrentSeek = offset;
        Write(value);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    public void Write(ushort value)
    {
        Utils.Write(Data, CurrentSeek, value);
        CurrentSeek += sizeof(ushort);
        CurrentLength += sizeof(ushort);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    /// <param name="offset">move the seek at this offset</param>
    public void Write(ushort value, int offset)
    {
        CurrentSeek = offset;
        Write(value);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    public void Write(byte value)
    {
        Utils.Write(Data, CurrentSeek, value);
        CurrentSeek += sizeof(byte);
        CurrentLength += sizeof(byte);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    /// <param name="offset">move the seek at this offset</param>
    public void Write(byte value, int offset)
    {
        CurrentSeek = offset;
        Write(value);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    public void Write(int value)
    {
        Utils.Write(Data, CurrentSeek, value);
        CurrentSeek += sizeof(int);
        CurrentLength += sizeof(int);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    /// <param name="offset">move the seek at this offset</param>
    public void Write(int value, int offset)
    {
        CurrentSeek = offset;
        Write(value);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    public void Write(uint value)
    {
        Utils.Write(Data, CurrentSeek, value);
        CurrentSeek += sizeof(uint);
        CurrentLength += sizeof(uint);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    /// <param name="offset">move the seek at this offset</param>
    public void Write(uint value, int offset)
    {
        CurrentSeek = offset;
        Write(value);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    public void Write(long value)
    {
        Utils.Write(Data, CurrentSeek, value);
        CurrentSeek += sizeof(long);
        CurrentLength += sizeof(long);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    /// <param name="offset">move the seek at this offset</param>
    public void Write(long value, int offset)
    {
        CurrentSeek = offset;
        Write(value);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    public void Write(ulong value)
    {
        Utils.Write(Data, CurrentSeek, value);
        CurrentSeek += sizeof(ulong);
        CurrentLength += sizeof(ulong);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    /// <param name="offset">move the seek at this offset</param>
    public void Write(ulong value, int offset)
    {
        CurrentSeek = offset;
        Write(value);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    public void Write(sbyte value)
    {
        Utils.Write(Data, CurrentSeek, value);
        CurrentSeek += sizeof(sbyte);
        CurrentLength += sizeof(sbyte);
    }
    /// <summary>
    /// Writes the value in the packet
    /// </summary>
    /// <param name="value">value to write</param>
    /// <param name="offset">move the seek at this offset</param>
    public void Write(sbyte value, int offset)
    {
        CurrentSeek = offset;
        Write(value);
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <param name="out_chars">output char array</param>
    /// <param name="out_charsOffset">output char array offset from which to start writing</param>
    /// <returns>total number of char elements</returns>
    public int ReadChars(char[] out_chars, int out_charsOffset)
    {
        int charC;
        int byteC;
        Utils.ReadChars(Data, CurrentSeek, out_chars, out_charsOffset, out byteC, out charC);
        CurrentSeek += byteC;
        return charC;
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <param name="out_chars">output char array</param>
    /// <param name="out_charsOffset">output char array offset from which to start writing</param>
    /// <returns>total number of char elements</returns>
    public int ReadChars(int offset, char[] out_chars, int out_charsOffset)
    {
        CurrentSeek = offset;
        return ReadChars(out_chars, out_charsOffset);
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <returns>value</returns>
    public char ReadChar()
    {
        char res = Utils.ReadChar(Data, CurrentSeek);
        CurrentSeek += sizeof(char);
        return res;
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <returns>value</returns>
    public char ReadChar(int offset)
    {
        CurrentSeek = offset;
        return ReadChar();
    }
    /// <summary>
    /// Reads a value from the packet (Causes char array allocation)
    /// </summary>
    /// <returns>value</returns>
    public string ReadString()
    {
        int n;

        string s = Utils.ReadString(Data, CurrentSeek, out n);

        CurrentSeek += n;

        return s;
    }
    /// <summary>
    /// Reads a value from the packet (Causes char array allocation)
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <returns>value</returns>
    public string ReadString(int offset)
    {
        CurrentSeek = offset;
        return ReadString();
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <returns>value</returns>
    public float ReadFloat()
    {
        float res = Utils.ReadSingle(Data, CurrentSeek);
        CurrentSeek += sizeof(float);
        return res;
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <returns>value</returns>
    public float ReadFloat(int offset)
    {
        CurrentSeek = offset;
        return ReadFloat();
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <returns>value</returns>
    public double ReadDouble()
    {
        double res = Utils.ReadDouble(Data, CurrentSeek);
        CurrentSeek += sizeof(double);
        return res;
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <returns>value</returns>
    public double ReadDouble(int offset)
    {
        CurrentSeek = offset;
        return ReadDouble();
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <returns>value</returns>
    public short ReadShort()
    {
        short res = Utils.ReadInt16(Data, CurrentSeek);
        CurrentSeek += sizeof(short);
        return res;
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <returns>value</returns>
    public short ReadShort(int offset)
    {
        CurrentSeek = offset;
        return ReadShort();
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <returns>value</returns>
    public ushort ReadUShort()
    {
        ushort res = Utils.ReadUInt16(Data, CurrentSeek);
        CurrentSeek += sizeof(ushort);
        return res;
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <returns>value</returns>
    public ushort ReadUShort(int offset)
    {
        CurrentSeek = offset;
        return ReadUShort();
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <returns>value</returns>
    public int ReadInt()
    {
        int res = Utils.ReadInt32(Data, CurrentSeek);
        CurrentSeek += sizeof(int);
        return res;
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <returns>value</returns>
    public int ReadInt(int offset)
    {
        CurrentSeek = offset;
        return ReadInt();
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <returns>value</returns>
    public uint ReadUInt()
    {
        uint res = Utils.ReadUInt32(Data, CurrentSeek);
        CurrentSeek += sizeof(uint);
        return res;
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <returns>value</returns>
    public uint ReadUInt(int offset)
    {
        CurrentSeek = offset;
        return ReadUInt();
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <returns>value</returns>
    public long ReadLong()
    {
        long res = Utils.ReadInt64(Data, CurrentSeek);
        CurrentSeek += sizeof(long);
        return res;
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <returns>value</returns>
    public long ReadLong(int offset)
    {
        CurrentSeek = offset;
        return ReadLong();
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <returns>value</returns>
    public ulong ReadULong()
    {
        ulong res = Utils.ReadUInt64(Data, CurrentSeek);
        CurrentSeek += sizeof(ulong);
        return res;
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <returns>value</returns>
    public ulong ReadULong(int offset)
    {
        CurrentSeek = offset;
        return ReadULong();
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <returns>value</returns>
    public bool ReadBool()
    {
        bool res = Utils.ReadBoolean(Data, CurrentSeek);
        CurrentSeek += sizeof(bool);
        return res;
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <returns>value</returns>
    public bool ReadBool(int offset)
    {
        CurrentSeek = offset;
        return ReadBool();
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <returns>value</returns>
    public byte ReadByte()
    {
        byte res = Data[CurrentSeek];
        CurrentSeek += sizeof(byte);
        return res;
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <returns>value</returns>
    public byte ReadByte(int offset)
    {
        CurrentSeek = offset;
        return ReadByte();
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <returns>value</returns>
    public sbyte ReadSByte()
    {
        sbyte res = Utils.ReadSByte(Data, CurrentSeek);
        CurrentSeek += sizeof(sbyte);
        return res;
    }
    /// <summary>
    /// Reads a value from the packet
    /// </summary>
    /// <param name="offset">move the seek at this offset</param>
    /// <returns>value</returns>
    public sbyte ReadSByte(int offset)
    {
        CurrentSeek = offset;
        return ReadSByte();
    }
    /// <summary>
    /// Sets current length and seek to 0
    /// </summary>
    public void ResetSeekLength()
    {
        CurrentLength = 0;
        CurrentSeek = 0;
    }
    /// <summary>
    /// Creates new packet instance with the given capacity
    /// </summary>
    /// <param name="maxCapacity">packet capacity</param>
    protected GamePacket(int maxCapacity)
    {
        Data = new byte[maxCapacity];
    }
    /// <summary>
    /// Creates new instance with the given buffer as the internal buffer. MaxCapacity == bufferToUse.Length
    /// </summary>
    /// <param name="bufferToUse">buffer to use as the internalBuffer</param>
    protected GamePacket(byte[] bufferToUse)
    {
        Data = bufferToUse;
    }
    /// <summary>
    /// Resets internal values to default
    /// </summary>
    protected void Reset()
    {
        ResetSeekLength();
        IsDisposed = false;
    }
}