using System;

namespace C64
{
    public class Endian
    {


        public static ushort Convert(ushort littleEndian)
        {
            ushort BigEndian = (ushort)(littleEndian >> 8);
            return BigEndian |= (ushort)(littleEndian << 8);

        }

        public static uint Convert(uint littleEndian)
        {
            uint BigEndian = 0;
            for (int i = 0; i < 4; i++)
            {
                BigEndian |= (uint)(littleEndian & 0x000000ff);
                if (i < 3)
                {
                    BigEndian <<= 8;
                    littleEndian >>= 8;
                }
            }
            return BigEndian;
        }


        public static ulong Convert(ulong littleEndian)
        {
            ulong BigEndian = 0;
            for (int i = 0; i < sizeof(ulong); i++)
            {
                BigEndian |= (ulong)(littleEndian & 0x00000000000000ff);
                if (i < sizeof(ulong) - 1)
                {
                    BigEndian <<= 8;
                    littleEndian >>= 8;
                }
            }
            return BigEndian;
        }

        public static short Convert(short littleEndian)
        {
            short BigEndian = (short)(littleEndian >> 8);
            return BigEndian |= (short)(littleEndian << 8);

        }

        public static int Convert(int littleEndian)
        {
            int BigEndian = 0;
            for (int i = 0; i < 4; i++)
            {
                BigEndian |= (littleEndian & 0x000000ff);
                if (i < 3)
                {
                    BigEndian <<= 8;
                    littleEndian >>= 8;
                }
            }
            return BigEndian;
        }

        public static long Convert(long littleEndian)
        {
            long BigEndian = 0;
            for (int i = 0; i < sizeof(long); i++)
            {
                BigEndian |= (long)(littleEndian & 0x00000000000000ff);
                if (i < sizeof(long) - 1)
                {
                    BigEndian <<= 8;
                    littleEndian >>= 8;
                }
            }
            return BigEndian;
        }


    }

    public class BigEndian
    {
        public byte[] Buffer;
        int position = 0;

        public void CreateBuffer(int length)
        {
            Buffer = new byte[length];
        }

        public void WriteByte(int index, byte b)
        {
            Buffer[index++] = b;
            position = index;
        }
        public void WriteByte(byte b)
        {
            Buffer[position++] = b;
        }

        public void WriteWord(int index, short s)
        {
            Buffer[index++] = (byte)(s >> 8);
            Buffer[index++] = (byte)(s & 0xff);
            position = index;
        }
        public void WriteWord(short s)
        {
            Buffer[position++] = (byte)(s >> 8);
            Buffer[position++] = (byte)(s & 0xff);
        }

        public void WriteWord(int index, byte[] array, short s)
        {
            array[index++] = (byte)(s >> 8);
            array[index++] = (byte)(s & 0xff);
        }

        public void WriteWord(int index, sbyte[] array, short s)
        {
            array[index++] = (sbyte)(s >> 8);
            array[index++] = (sbyte)(s & 0xff);
        }

        public void WriteUWord(int index, ushort s)
        {
            Buffer[index++] = (byte)(s >> 8);
            Buffer[index++] = (byte)(s & 0xff);
            position = index;
        }
        public void WriteUWord(ushort s)
        {
            Buffer[position++] = (byte)(s >> 8);
            Buffer[position++] = (byte)(s & 0xff);
        }
        public void WriteUWord(int index, byte[] array, ushort s)
        {
            array[index++] = (byte)(s >> 8);
            array[index] = (byte)(s & 0xff);
        }


        public void WriteLong(int index, int l)
        {
            for (int i = 3; i >= 0; i--)
            {
                Buffer[index + (3 - i)] = (byte)(l >> i * 8);
            }
            position = index + sizeof(int);
        }
        public void WriteLong(int l)
        {
            for (int i = 3; i >= 0; i--)
            {
                Buffer[position + (3 - i)] = (byte)(l >> i * 8);
            }
            position += sizeof(int);
        }
        public void WriteULong(int index, uint l)
        {
            for (int i = 3; i >= 0; i--)
            {
                Buffer[index + (3 - i)] = (byte)(l >> i * 8);
            }
            position = index + sizeof(uint);
        }
        public void WriteULong(uint l)
        {
            for (int i = 3; i >= 0; i--)
            {
                Buffer[position + (3 - i)] = (byte)(l >> i * 8);
            }
            position += sizeof(uint);
        }
        public void WriteULong(int index, byte[] array, uint l)
        {
            for (int i = 3; i >= 0; i--)
            {
                array[index + (3 - i)] = (byte)(l >> i * 8);
            }
        }

        public byte ReadByte(int index)
        {
            byte b = Buffer[index++];
            position = index;
            return b;

        }
        public byte ReadByte()
        {
            byte b = Buffer[position++];
            return b;

        }

        public byte ReadByte(int index, byte[] array)
        { return array[index]; }

        public short ReadWord(int index)
        {
            short s = (short)(Buffer[index++] << 8);
            if (index < Buffer.Length)
                s |= (short)(Buffer[index++] & 0xff);

            position = index;
            return s;
        }

        public short ReadWord()
        {
            short s = (short)(Buffer[position++] << 8);

            if (position < Buffer.Length)
                s |= (short)(Buffer[position++] & 0xff);

            return s;
        }

        public short ReadWord(int index, byte[] array)
        {
            short s = (short)(array[index++] << 8);
            if (index < array.Length)
                return s |= (short)(array[index] & 0xff);
            else
                return s;
        }

        public ushort ReadUWord(int index)
        {
            ushort s = (ushort)(Buffer[index++] << 8);

            if (index < Buffer.Length)
                s |= (ushort)(Buffer[index++] & 0xff);

            position = index;

            return s;
        }
        public ushort ReadUWord()
        {
            ushort s = (ushort)(Buffer[position++] << 8);
            s |= (ushort)(Buffer[position++] & 0xff);

            return s;
        }
        public ushort ReadUWord(int index, byte[] array)
        {
            ushort s = (ushort)(array[index++] << 8);
            return s |= (ushort)(array[index] & 0xff);
        }

        public int ReadLong(int index)
        {
            int s = 0;

            for (int i = 0; i < 4; i++)
                s |= Buffer[index + i] << (3 - i) * 8;

            position = index + sizeof(int);
            return s;
        }
        public int ReadLong()
        {
            int s = 0;

            for (int i = 0; i < 4; i++)
                if (position + i < Buffer.Length)
                    s |= Buffer[position + i] << (3 - i) * 8;

            position += sizeof(int);
            return s;
        }
        public int ReadLong(int index, byte[] array)
        {
            int s = 0;

            for (int i = 0; i < 4; i++)
                if (index + i < array.Length)
                    s |= array[index + i] << (3 - i) * 8;

            return s;
        }

        public uint ReadULong(int index)
        {
            uint s = 0;

            for (int i = 0; i < 4; i++)
                if (index + i < Buffer.Length)
                    s |= (uint)Buffer[index + i] << (3 - i) * 8;

            position = index + sizeof(uint);
            return s;
        }
        public uint ReadULong()
        {
            uint s = 0;

            for (int i = 0; i < 4; i++)
                if (position + i < Buffer.Length)
                    s |= (uint)Buffer[position + i] << (3 - i) * 8;

            position += sizeof(uint);
            return s;
        }
        public uint ReadULong(int index, byte[] array)
        {
            uint s = 0;

            for (int i = 0; i < 4; i++)
                if (index + i < array.Length)
                    s |= (uint)array[index + i] << (3 - i) * 8;

            return s;
        }

        public unsafe uint ReadULong(int index, byte* array, int arrayLength)
        {
            uint s = 0;

            for (int i = 0; i < 4; i++)
                if (index + i < arrayLength)
                    s |= (uint)array[index + i] << (3 - i) * 8;

            return s;
        }

        public string ReadString(int index, int length)
        {
            string str = "";

            for (int i = 0; i < length; i++)
                str += (char)ReadByte(index + i);

            position = index;
            return str;
        }

        public void CopyWords(ushort[] buffer, int length)
        {
            for (int i = 0; i != length; i++)
                WriteUWord(i << 1, buffer[i]);

            position = 0;
        }

        public void CopyWords(ushort[] srcArray, byte[] destArray, int length)
        {
            for (int i = 0; i != length; i++)
                WriteUWord(i << 1, destArray, srcArray[i]);
        }

        public void CopyULongs(UInt32[] buffer, int length)
        {
            for (int i = 0; i != length; i++)
                WriteULong(i << 2, buffer[i]);

            position = 0;
        }

        public void CopyULongs(UInt32[] buffer, byte[] array, int length)
        {
            for (int i = 0; i != length; i++)
                WriteULong(i << 2, array, buffer[i]);
        }

        public ushort[] ReadWordsAt(int index, ushort length)
        {
            ushort[] buffer = new ushort[length];

            for (int i = 0; i != length; i++)
                buffer[i] = ReadUWord(index + i * 2);

            return buffer;
        }

        public ushort[] ReadWordsAt(ushort length)
        {
            ushort[] buffer = new ushort[length];

            for (int i = 0; i != length; i++)
                buffer[i] = ReadUWord();

            return buffer;
        }

        public byte[] ReadBytesAt(int index, ushort length)
        {
            byte[] buffer = new byte[length];

            for (int i = 0; i != length; i++)
                buffer[i] = ReadByte(index + i);

            return buffer;
        }
    }
}
