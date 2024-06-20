using System;
using System.Collections.Generic;
using System.IO;
using WubiMaster.Common;

namespace WubiMaster.Models
{
    public abstract class CustomBaseModel
    {
        public Dictionary<string, string> AttributeDict { get; set; }
        public string FilePath { get; set; }

        public void DelAttribute(string key)
        {
            AttributeDict ??= new Dictionary<string, string>();
            if (AttributeDict.ContainsKey(key))
            {
                AttributeDict.Remove(key);
            }
        }

        public void SetAttribute(string key, string value)
        {
            AttributeDict ??= new Dictionary<string, string>();
            AttributeDict[key] = value;
        }

        public void Write()
        {
            try
            {
                string file_path = GlobalValues.UserPath + $"\\{FilePath}";

                string txt = "";
                txt += "# rime settings\n";
                txt += "# encoding: utf-8\n";
                txt += "# author: 空山明月\n";
                txt += "# modify: 中书君\n";
                txt += $"# time: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}\n";
                txt += "\npatch:\n";

                foreach (var k in AttributeDict.Keys)
                {
                    txt += $"  {k}: {AttributeDict[k]}\n";
                }

                if (File.Exists(file_path))
                    File.Delete(file_path);
                File.WriteAllText(file_path, txt);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}