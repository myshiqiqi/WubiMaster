using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WubiMaster.Models
{
    public partial class WubiCustomModel : CustomBaseModel
    {
        public string show_es { get; set; }
        public string zh_trad { get; set; }
        public string new_spelling { get; set; }
        public string new_hide_pinyin { get; set; }
        public string GB2312 { get; set; }
        public string single_char { get; set; }
        public string full_shape { get; set; }


        public WubiCustomModel()
        {
            FilePath = "wubi.custom.yaml";
            AttributeDict = new Dictionary<string, string>();

            show_es = "switches/@0/reset";
            zh_trad = "switches/@1/reset";
            new_spelling = "switches/@2/reset";
            new_hide_pinyin = "switches/@3/reset";
            GB2312 = "switches/@4/reset";
            single_char = "switches/@5/reset";
            full_shape = "switches/@6/reset";
        }
    }
}
