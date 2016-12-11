using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    public static class Utility
    {
        public static List<string> SeparateString(string text, int maximumWidth)
        {
            List<string> separated = new List<string>();

            int index = 0;
            while (true)
            {
                if (text.Length <= maximumWidth)
                {
                    separated.Add(text);
                    break;
                }
                else
                {
                    string substring = text.Substring(index, maximumWidth);
                    separated.Add(substring.Trim());
                    text = text.Substring(maximumWidth);
                }
            }

            return separated;
        }
    }


    /* 
    * Instead of saving the output to the console whenever we draw something,
    * I searched for a way of reading the console output. It seems there is
    * no way to do this in a way that would be cross-platform, but considering
    * this application is only meant to be ran on Windows, I've found a 
    * function that could potentially be useful 
    * https://msdn.microsoft.com/en-us/library/windows/desktop/ms684965(v=vs.85).aspx
    *
    * It seems someone already implemented this function from kernel32.dll 
    * in .NET so it can read from the buffer easily in a string list
    *
    * http://stackoverflow.com/a/12366307/1526058 
    */
    public class ConsoleReader
    {
        public static IEnumerable<string> ReadFromBuffer(short x, short y, short width, short height)
        {
            IntPtr buffer = Marshal.AllocHGlobal(width * height * Marshal.SizeOf(typeof(CHAR_INFO)));
            if (buffer == null)
                throw new OutOfMemoryException();

            try
            {
                COORD coord = new COORD();
                SMALL_RECT rc = new SMALL_RECT();
                rc.Left = x;
                rc.Top = y;
                rc.Right = (short)(x + width - 1);
                rc.Bottom = (short)(y + height - 1);

                COORD size = new COORD();
                size.X = width;
                size.Y = height;

                const int STD_OUTPUT_HANDLE = -11;
                if (!ReadConsoleOutput(GetStdHandle(STD_OUTPUT_HANDLE), buffer, size, coord, ref rc))
                {
                    // 'Not enough storage is available to process this command' may be raised for buffer size > 64K (see ReadConsoleOutput doc.)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                IntPtr ptr = buffer;
                for (int h = 0; h < height; h++)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int w = 0; w < width; w++)
                    {
                        CHAR_INFO ci = (CHAR_INFO)Marshal.PtrToStructure(ptr, typeof(CHAR_INFO));
                        char[] chars = Console.OutputEncoding.GetChars(ci.charData);
                        sb.Append(chars[0]);
                        ptr += Marshal.SizeOf(typeof(CHAR_INFO));
                    }
                    yield return sb.ToString();
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CHAR_INFO
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] charData;
            public short attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD dwSize;
            public COORD dwCursorPosition;
            public short wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadConsoleOutput(IntPtr hConsoleOutput, IntPtr lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, ref SMALL_RECT lpReadRegion);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
    }
}
