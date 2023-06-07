using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace DeepDiveEmulator.Classes
{
    public class FileINI
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string inSection, string inKey, string inDefault, StringBuilder inRetVal, int inSize, string inPath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string inSection, string inKey, string inValue, string inPath);

        public FileINI(string inPath = null)
        {
            Path = new FileInfo(inPath ?? EXE + ".ini").FullName;
        }

        public bool ReadKeyString(string inKey, out string outValue, string inSection = null)
        {
            StringBuilder stringBuilder = new StringBuilder(4095);
            GetPrivateProfileString(inSection ?? EXE, inKey, "", stringBuilder, 4095, Path);
            //
            outValue = stringBuilder.ToString();
            if (outValue.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ReadKeyByte(string inKey, out Byte outValue, string inSection = null)
        {
            outValue = 0;
            if (ReadKeyString(inKey, out string value, inSection) == true && Byte.TryParse(value, out outValue) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ReadKeyInt(string inKey, out int outValue, string inSection = null)
        {
            outValue = 0;
            if (ReadKeyString(inKey, out string value, inSection) == true && int.TryParse(value, out outValue) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ReadKeyUInt(string inKey, out uint outValue, string inSection = null)
        {
            outValue = 0;
            if (ReadKeyString(inKey, out string value, inSection) == true && uint.TryParse(value, out outValue) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ReadKeyDouble(string inKey, out double outValue, string inSection = null)
        {
            outValue = 0;
            if (ReadKeyString(inKey, out string value, inSection) == true && double.TryParse(value, out outValue) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ReadKeyBool(string inKey, out bool outValue, string inSection = null)
        {
            outValue = false;
            if (ReadKeyString(inKey, out string value, inSection) == true && bool.TryParse(value, out outValue) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void WriteKey(string inKey, string inValue, string inSection = null)
        {
            WritePrivateProfileString(inSection ?? EXE, inKey, inValue, Path);
        }
        public void DeleteKey(string inKey, string inSection = null)
        {
            WriteKey(inKey, null, inSection ?? EXE);
        }
        public void DeleteSection(string inSection = null)
        {
            WriteKey(null, null, inSection ?? EXE);
        }
    }
}
