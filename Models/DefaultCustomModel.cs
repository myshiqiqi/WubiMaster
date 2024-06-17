using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WubiMaster.Common;

namespace WubiMaster.Models
{
    /// <summary>
    /// default.custom.yaml 映射类
    /// custom 类应当以补丁格式填写
    /// </summary>
    public class DefaultCustomModel
    {
        public string PageSize { get; set; }
        public string SelectLabels { get; set; }
        public Dictionary<string, string> DetailDict = new Dictionary<string, string>();

        public DefaultCustomModel()
        {
            PageSize = @"menu/page_size";
            SelectLabels = @"menu/alternative_select_labels";
        }

        private void InitDetailDict()
        {
            DetailDict.Add(PageSize, "");
        }

        public void SetVlaue(string key, string value)
        {
            DetailDict[key] = value;
        }

        public void Write()
        {
            try
            {
                string file_path = GlobalValues.UserPath + "\\default.custom.yaml";

                string txt = "";
                txt += "# Rime settings\n";
                txt += "# encoding: utf-8\n";
                txt += "# author: 空山明月\n";
                txt += "# modify: 中书君\n";
                txt += $"# time: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}\n";
                txt += "\npatch:\n";

                foreach (var k in DetailDict.Keys)
                {
                    txt += $"  {k}: {DetailDict[k]}\n";
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
