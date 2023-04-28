using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace DeepDiveEmulator.ClassFileINI
{
    public class FileINI
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;



        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string inputSection, string inputKey, string inputValue, string inputPath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string inputSection, string inputKey, string inputDefault, StringBuilder inputRetVal, int inputSize, string inputPath);



        public FileINI(string inputPath = null)
        {
            Path = new FileInfo(inputPath ?? EXE + ".ini").FullName;
        }

        public string Read(string inputKey, string inputSection = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(inputSection ?? EXE, inputKey, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public bool ReadInt(string inputKey, out int outputValue, string inputSection = null)
        {
            outputValue = 0;
            string value = Read(inputKey, inputSection);
            if (int.TryParse(value, out int valueChecked) == true)
            {
                outputValue = valueChecked;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ReadUInt(string inputKey, out uint outputValue, string inputSection = null)
        {
            outputValue = 0;
            string value = Read(inputKey, inputSection);
            if (uint.TryParse(value, out uint valueChecked) == true)
            {
                outputValue = valueChecked;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ReadDouble(string inputKey, out double outputValue, string inputSection = null)
        {
            outputValue = 0;
            string value = Read(inputKey, inputSection);
            if (double.TryParse(value, out double valueChecked) == true)
            {
                outputValue = valueChecked;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ReadByte(string inputKey, out Byte outputValue, string inputSection = null)
        {
            outputValue = 0;
            string value = Read(inputKey, inputSection);
            if (Byte.TryParse(value, out Byte valueChecked) == true)
            {
                outputValue = valueChecked;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ReadBool(string inputKey, out bool outputValue, string inputSection = null)
        {
            outputValue = false;
            string value = Read(inputKey, inputSection);
            if (bool.TryParse(value, out bool valueChecked) == true)
            {
                outputValue = valueChecked;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Write(string inputKey, string inputValue, string inputSection = null)
        {
            WritePrivateProfileString(inputSection ?? EXE, inputKey, inputValue, Path);
        }

        /*
        public void KeyDelete(string inputKey, string inputSection = null)
        {
            Write(inputKey, null, inputSection ?? EXE);
        }
        */

        /*
        public bool KeyExists(string inputKey, string inputSection = null)
        {
            return Read(inputKey, inputSection).Length > 0;
        }
        */


        /*
        public void SectionDelete(string inputSection = null)
        {
            Write(null, null, inputSection ?? EXE);
        }
        */
    }
}
