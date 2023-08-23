using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Path = System.IO.Path;

namespace DeepDiveEmulator.Classes
{
    public class FileINI
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string inSection, string inKey, string inValue, string inPath);

        string PathFile;
        Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>(StringComparer.InvariantCultureIgnoreCase);

        public FileINI(string file)
        {
            PathFile = file;

            if (File.Exists(file) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                File.Create(file).Close();
            }
        }

        public void ReadFile()
        {
            var txt = File.ReadAllText(PathFile);
            Dictionary<string, string> currentSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            data[""] = currentSection;

            foreach (var line in txt.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()))
            {
                if (line.StartsWith(";"))
                {
                    continue;
                }
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                    data[line.Substring(1, line.LastIndexOf("]") - 1)] = currentSection;
                    continue;
                }
                var idx = line.IndexOf("=");
                if (idx == -1)
                {
                    currentSection[line] = "";
                }
                else
                {
                    currentSection[line.Substring(0, idx)] = line.Substring(idx + 1);
                }
            }
        }
        public string? ReadKeyString(string inKey, string inSection)
        {
            string? outValue = null;
            ReadFile();
            if (data.ContainsKey(inSection) == true && data[inSection].ContainsKey(inKey) == true)
            {
                outValue = data[inSection][inKey];
            }
            return outValue;
        }
        public Byte? ReadKeyByte(string inKey, string inSection)
        {
            Byte? outValue = null;
            ReadFile();
            string? valueString = ReadKeyString(inKey, inSection);
            if (valueString != null && Byte.TryParse(valueString, out Byte valueByte) == true)
            {
                outValue = valueByte;
            }
            return outValue;
        }
        public int? ReadKeyInt(string inKey, string inSection)
        {
            int? outValue = null;
            ReadFile();
            string? valueString = ReadKeyString(inKey, inSection);
            if (valueString != null && int.TryParse(valueString, out int valueInt) == true)
            {
                outValue = valueInt;
            }
            return outValue;
        }
        public uint? ReadKeyUInt(string inKey, string inSection)
        {
            uint? outValue = null;
            ReadFile();
            string? valueString = ReadKeyString(inKey, inSection);
            if (valueString != null && uint.TryParse(valueString, out uint valueUInt) == true)
            {
                outValue = valueUInt;
            }
            return outValue;
        }
        public double? ReadKeyDouble(string inKey, string inSection)
        {
            double? outValue = null;
            ReadFile();
            string? valueString = ReadKeyString(inKey, inSection);
            if (valueString != null && double.TryParse(valueString, out double valueDouble) == true)
            {
                outValue = valueDouble;
            }
            return outValue;
        }
        public bool? ReadKeyBool(string inKey, string inSection)
        {
            bool? outValue = null;
            ReadFile();
            string? valueString = ReadKeyString(inKey, inSection);
            if (valueString != null && bool.TryParse(valueString, out bool valueBool) == true)
            {
                outValue = valueBool;
            }
            return outValue;
        }
        public void WriteKey(string inKey, string inSection, string inValue)
        {
            WritePrivateProfileString(inSection, inKey, inValue, PathFile);
        }
        public void DeleteKey(string inKey, string inSection = null)
        {
            WriteKey(inKey, inSection, null);
        }
        public void DeleteSection(string inSection)
        {
            WriteKey(null, inSection, null);
        }
    }
}
