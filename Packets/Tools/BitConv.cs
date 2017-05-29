using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2NG.Tools
{
    public class ByteConverter
    {
        public static int GetByteOffset(byte[] bytes, int num, int offset, int length, int def)
        {
            if (offset < 0) offset = 0;
            if (length < 0 || length > bytes.Length - offset) length = bytes.Length - offset;
            if (length < 0 || num < 0) return -1;

            for (int i = 0; i < length; i++)
            {
                if (bytes[i + offset] == num)
                {
                    return i;
                }
            }
            return def;
        }
        public static int GetBytePosition(byte[] bytes, int num, int start)
        {
            return GetBytePosition(bytes, num, start, bytes.Length, bytes.Length);
        }
        public static int GetBytePosition(byte[] bytes, int num, int start, int end, int def)
        {
            if (start < 0 || end < 0 || end > bytes.Length || num < 0)
                throw new ArgumentException();

            for (int i = start; i < end - 1; i++)
            {
                if (bytes[i] == num)
                {
                    return i;
                }
            }
            return def;
        }
        public static string GetString(byte[] bytes, int offset, int length)
        {
            if (offset < 0) offset = 0;
            if (length < 0 || length > bytes.Length - offset) length = bytes.Length - offset;
            if (length < 0) return null;

            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = (char)bytes[i + offset];
                if (char.GetUnicodeCategory(chars[i]) == System.Globalization.UnicodeCategory.Control)
                    chars[i] = '.';
            }
            return new String(chars);
        }
        public static string GetString(byte[] bytes, int offset, int length, int terminator)
        {
            length = GetByteOffset(bytes, terminator, offset, length, length);
            if (length == -1)
                return null;
            return GetString(bytes, offset, length);
        }
        public static string GetNullString(byte[] bytes, int offset)
        {
            return GetString(bytes, offset, -1, 0);
        }


    }
}
