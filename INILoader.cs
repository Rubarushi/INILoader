using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace INILoader
{
    public class INILoader
    {
        private Dictionary<string, List<KeyValuePair<string, object>>> KeyPairSet = new Dictionary<string, List<KeyValuePair<string, object>>>();

        private string m_fileName = string.Empty;
        private string m_title = string.Empty;

        private void CheckTitle()
        {
            if (m_title == string.Empty)
                throw new Exception("INILoader::m_title is Empty");
        }
        
        public INILoader()
        {

        }

        public void SetTitle(string title)
        {
            m_title = title;
        }

        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        public void SaveInt(string title, string keyName, int value)
        {
            WritePrivateProfileString(title, keyName, $" {value}", m_fileName);
        }

        public void SaveBool(string title, string keyName, bool value)
        {
            WritePrivateProfileString(title, keyName, $" {value}", m_fileName);
        }

        public void SaveFloat(string title, string keyName, float value)
        {
            WritePrivateProfileString(title, keyName, $" {value}", m_fileName);
        }

        public int LoadInt(string keyName, int defaultValue)
        {
            CheckTitle();
            List<KeyValuePair<string, object>> value = KeyPairSet[m_title];
            foreach(var i in value)
            {
                if (i.Key.Equals(keyName))
                    return Convert.ToInt32(i.Value);
            }
            return defaultValue;
        }

        public float LoadFloat(string keyName, float defaultValue)
        {
            CheckTitle();
            List<KeyValuePair<string, object>> value = KeyPairSet[m_title];
            foreach (var i in value)
            {
                if (i.Key.Equals(keyName))
                    return Convert.ToSingle(i.Value);
            }
            return defaultValue;
        }

        public void ReloadFIle(string fileName, string title = "")
        {
            m_title = title;
            LoadFile();
        }

        public string LoadString(string keyName, string defaultValue)
        {
            CheckTitle();
            List<KeyValuePair<string, object>> value = KeyPairSet[m_title];
            foreach (var i in value)
            {
                if (i.Key.Equals(keyName))
                    return i.Value.ToString();
            }
            return defaultValue;
        }

        public bool LoadBool(string keyName, bool defaultValue)
        {
            CheckTitle();
            List<KeyValuePair<string, object>> value = KeyPairSet[m_title];
            foreach (var i in value)
            {
                if (i.Key.Equals(keyName))
                {
                    if(i.Value.ToString().Equals("1") || i.Value.ToString().ToLower().Equals("true"))
                    {
                        return true;
                    }
                    if(i.Value.ToString().Equals("0") || i.Value.ToString().ToLower().Equals("false"))
                    {
                        return false;
                    }
                }
            }
            return defaultValue;
        }

        public void SetFIleName(string fileName)
        {
            m_fileName = fileName;
            LoadFile();
        }

        public void SetFIleName(string path, string fileName)
        {
            m_fileName = $"{path}\\{fileName}";
            LoadFile();
        }

        public INILoader(string fileName)
        {
            m_fileName = Directory.GetCurrentDirectory() + "\\" + fileName;
            LoadFile();
        }

        public INILoader(string path, string fileName)
        {
            m_fileName = $"{path}\\{fileName}";
            LoadFile();
        }

        private void LoadFile()
        {
            string currentKey = string.Empty;
            if(!File.Exists(m_fileName))
            {
                throw new Exception($"File({m_fileName}) is not exists");
            }
            using (StreamReader br = new StreamReader(File.Open(m_fileName, FileMode.Open)))
            {
                while(!br.EndOfStream)
                {
                    string line = br.ReadLine();

                    if(line != null)
                    {
                        line = line.Trim();

                        if (line.StartsWith(";"))
                        {
                            continue;
                        }

                        if(line.StartsWith("[")) // Key
                        {
                            if(line.Contains(";"))
                            {
                                line = line.Trim().Substring(0, line.IndexOf(";"));
                            }

                            if(!line.EndsWith("]"))
                            {
                                throw new Exception($"Missing \"]\" ({line}])");
                            }

                            currentKey = line.Trim().Substring(1, line.Length - 2);//Load Key First

                            string temp = br.ReadLine().Replace(" ", "");

                            if (temp != null && temp.Contains("="))//Key And Value Exists
                            {
                                if(temp.IndexOf(";") != -1)
                                {
                                    temp = temp.Substring(0, temp.IndexOf(";"));
                                }
                                int equalIndex = temp.IndexOf("=");
                                string keyofvalue = temp.Substring(0, equalIndex).Trim();
                                object valueofvalue = temp.Substring(equalIndex + 1).Trim();
                                KeyValuePair<string, object> pair = new KeyValuePair<string, object>(keyofvalue, valueofvalue);
                                if(!KeyPairSet.ContainsKey(currentKey))
                                {
                                    KeyPairSet.Add(currentKey, new List<KeyValuePair<string, object>>());
                                }
                                KeyPairSet[currentKey].Add(pair);
                            }
                        }
                        else if(line.Contains("=") && currentKey != string.Empty)
                        {
                            if (line.IndexOf(";") != -1)
                            {
                                line = line.Substring(0, line.IndexOf(";"));
                            }
                            int equalIndex = line.IndexOf("=");
                            string keyofvalue = line.Substring(0, equalIndex).Trim();
                            object valueofvalue = line.Substring(equalIndex + 1).Trim();

                            KeyValuePair<string, object> pair = new KeyValuePair<string, object>(keyofvalue, valueofvalue);
                            if (!KeyPairSet.ContainsKey(currentKey))
                            {
                                KeyPairSet.Add(currentKey, new List<KeyValuePair<string, object>>());
                            }
                            KeyPairSet[currentKey].Add(pair);
                        }
                    }
                }
            }
        }
    }
}
