using UnityEngine;
using System.Diagnostics;
using System.IO;
using System;
using System.Text;
//using VOCASY;//
//using VOCASY.Utility;

public class TestTime : MonoBehaviour
{
    Stopwatch watch = new Stopwatch();
    public double AverageFirst;
    public double AverageSecond;
    public int False;
    public int True;
    double d1;
    double d2;
    uint c1;
    uint c2;

    uint cycles = 1000000;

    void Update()
    {
        int n = 200000;
        GamePacket first = GamePacket.CreatePacket(n);
        GamePacket second = GamePacket.CreatePacket(n);


        watch.Reset();
        watch.Start();
        for (int z = 0; z < cycles; z++)
        {

        }
        watch.Stop();



        long time = watch.ElapsedMilliseconds;
        d1 += time;
        c1++;




        watch.Reset();
        watch.Start();
        for (int z = 0; z < cycles; z++)
        {

        }
        watch.Stop();



        long time2 = watch.ElapsedMilliseconds;
        d2 += time2;
        c2++;

        if (time > time2)
        {
            False++;
            //UnityEngine.Debug.LogFormat("First mode elapsed time = {0} , second mode elapsed time = {1} . Is first faster than second ? {2}", time, time2, time <= time2);

        }
        else
            True++;

        AverageFirst = d1 / c1;
        AverageSecond = d2 / c2;

        first.DisposePacket();
        second.DisposePacket();
    }

    private class GamePacketStream
    {
        private BinaryReader reader;
        private BinaryWriter writer;
        private MemoryStream stream;

        public GamePacketStream(byte[] data, int length)
        {
            stream = new MemoryStream(data, 0, length);
            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);
        }

        public GamePacketStream()
        {
            stream = new MemoryStream();
            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);
        }

        public byte[] ToByteArray()
        {
            return stream.ToArray();
        }

        public float GetFloat()
        {
            return reader.ReadSingle();
        }

        public uint GetUInt32()
        {
            return reader.ReadUInt32();
        }

        public byte GetByte()
        {
            return reader.ReadByte();
        }

        public void WriteFloat(float f)
        {
            writer.Write(f);
        }

        public void WriteUInt32(uint i)
        {
            writer.Write(i);
        }

        public void WriteByte(byte b)
        {
            writer.Write(b);
        }

        public float GetFloat(int offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
            return GetFloat();
        }

        public uint GetUInt32(int offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
            return GetUInt32();
        }

        public byte GetByte(int offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
            return GetByte();
        }

        public void WriteFloat(float f, int offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
            WriteFloat(f);
        }

        public void WriteUInt32(uint i, int offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
            WriteUInt32(i);
        }

        public void WriteByte(byte b, int offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
            WriteByte(b);
        }

        public int GetLenght()
        {
            return (int)stream.Length;
        }
    }

    private class GamePacketStringStream
    {
        private byte[] data;
        Encoding enc;

        public GamePacketStringStream(string s)
        {
            enc = Encoding.UTF8;
            data = enc.GetBytes(s);
        }

        public string Read()
        {
            return enc.GetString(data);
        }

        public void Write(string s)
        {
            enc.GetBytes(s, 0, enc.GetByteCount(s), data, 0);
        }
    }
}