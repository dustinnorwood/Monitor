using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonadicCSharp;

namespace Monitor
{
    public static class Utility
    {
        public static int[] BitwiseHammingNumbers = new int[] { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4 };
        public static int GetByteHammingNumber(byte b) { return BitwiseHammingNumbers[b & 0xf] + BitwiseHammingNumbers[(b >> 4) & 0xf]; }

        public static int GetTickCount() { return System.Environment.TickCount; }
        public static int GetElapsedTime(int pastTicks) { return (int)(0xFFFFFFFF - pastTicks + System.Environment.TickCount); }
        public static int GetElapsedTime(int ticks, int pastTicks) { return (int)(0xFFFFFFFF - pastTicks + ticks); }

        public static byte DEC(char c)
        {
            return (byte)((c) > '9' ? c - 'A' + 10 : (c) - '0');
        }

        public static char HEX(byte n)
        {
            return (char)((n) > 9 ? (char)(n) - 10 + 'A' : (n) + '0');
        }

        public static byte BYTE(char c1, char c2)
        {
            return (byte)((DEC(c1) << 4) | DEC(c2));
        }

        public static string HEXBYTE(byte n)
        {
            return HEX((byte)(n >> 4)).ToString() + HEX((byte)(n & 0xf)).ToString();
        }

        public static Maybe<T> When<T>(this T obj, Predicate<T> pred, int timeoutMilliseconds)
        {
            int ticks = Utility.GetTickCount();
            while (Utility.GetElapsedTime(ticks) < timeoutMilliseconds)
            {
                if (pred(obj))
                    return new Maybe<T>(obj);
                System.Threading.Thread.Sleep(10);
            }
            return new Maybe<T>();
        }

        public static T When<T>(this T obj, Predicate<T> pred)
        {
            while (!pred(obj))
                System.Threading.Thread.Sleep(10);
            return obj;
        }

        public static int HashString(string s, int radix, int shift)
        {
            int hash = 0;
            for (int i = 0; i < s.Length; i++)
            {
                hash <<= shift;
                hash += (int)s[i];
                hash %= radix;
            }
            return hash;
        }

        public static string ToBase62(int num)
        {
            string base62 = "";
            do
            {
                int n = num % 62;
                base62 = ((char)((n > 35 ? n - 35 + 'a' : (n > 9 ? n - 10 + 'A' : n + '0')))).ToString() + base62;
                num /= 62;
            }
            while (num != 0);

            return base62;
        }

        public static bool TryParse(string s, out int i)
        {
            i = 0;
            try
            {
                if (string.IsNullOrEmpty(s))
                    return false;
                foreach (char c in s)
                {
                    if (c < '0' || c > '9')
                        return false;
                }
                i = int.Parse(s);
                return true;
            }
            catch
            {
                i = 0;
                return false;
            }
        }

        public static bool TryParse(string s, System.Globalization.NumberStyles style, out int i)
        {
            i = 0;
            try
            {
                if (string.IsNullOrEmpty(s))
                    return false;
                i = int.Parse(s, style);
                return true;
            }
            catch
            {
                i = 0;
                return false;
            }
        }

        public static bool TryParseL(string s, out long i)
        {
            i = 0;
            try
            {
                if (string.IsNullOrEmpty(s))
                    return false;
                foreach (char c in s)
                {
                    if (c < '0' || c > '9')
                        return false;
                }
                i = long.Parse(s);
                return true;
            }
            catch
            {
                i = 0;
                return false;
            }
        }

        public static bool TryParseL(string s, System.Globalization.NumberStyles style, out long i)
        {
            i = 0;
            try
            {
                if (string.IsNullOrEmpty(s))
                    return false;
                i = long.Parse(s, style);
                return true;
            }
            catch
            {
                i = 0;
                return false;
            }
        }

        public static int ParseZero(string s)
        {
            int i;
            if (TryParse(s, out i))
                return i;
            else return 0;
        }

        public static int ParseZero(string s, System.Globalization.NumberStyles style)
        {
            int i;
            if (TryParse(s, style, out i))
                return i;
            else return 0;
        }

        public static Maybe<int> MaybeParse(string s)
        {
            int i;
            if (TryParse(s, out i))
                return new Maybe<int>(i);
            else return new Maybe<int>();
        }

        public static Maybe<int> MaybeParse(string s, System.Globalization.NumberStyles style)
        {
            int i;
            if (TryParse(s, style, out i))
                return new Maybe<int>(i);
            else return new Maybe<int>();
        }
    }
}
