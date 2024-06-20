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
    public class DefaultCustomModel : CustomBaseModel
    {
        public string PageSize { get; set; }
        public string SelectLabels { get; set; }

        public DefaultCustomModel()
        {
            FilePath = "default.custom.yaml";
            AttributeDict = new Dictionary<string, string>();

            PageSize = @"menu/page_size";
            SelectLabels = @"menu/alternative_select_labels";
        }
    }
}
