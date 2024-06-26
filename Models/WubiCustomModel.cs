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
        public string switches { get; set; }
        //public string show_es { get; set; }
        //public string zh_trad { get; set; }
        //public string new_spelling { get; set; }
        //public string new_hide_pinyin { get; set; }
        //public string GB2312 { get; set; }
        //public string single_char { get; set; }
        //public string full_shape { get; set; }
        public string enable_user_dict { get; set; }
        public string enable_completion { get; set; }
        public string enable_sentence { get; set; }
        public string max_code_length { get; set; }
        public string auto_select { get; set; }
        public string enter_clear_code { get; set; }
        public string auto_clear { get; set; }
        public string enable_encoder { get; set; }
        public string enable_semicolon { get; set; }
        public string tab_clear_code { get; set; }
        public string encode_commit_history { get; set; }
        public string delimiter { get; set; }

        public WubiCustomModel()
        {
            FilePath = "wubi.custom.yaml";
            AttributeDict = new Dictionary<string, string>();

            switches = "switches/+";
            //show_es = "switches/@0/reset";
            //zh_trad = "switches/@1/reset";
            //new_spelling = "switches/@2/reset";
            //new_hide_pinyin = "switches/@3/reset";
            //GB2312 = "switches/@4/reset";
            //single_char = "switches/@5/reset";
            //full_shape = "switches/@6/reset";
            enable_user_dict = "translator/enable_user_dict";
            enable_completion = "translator/enable_completion";
            enable_sentence = "translator/enable_sentence";
            max_code_length = "speller/max_code_length";
            auto_select = "speller/auto_select";
            enter_clear_code = "key_binder/bindings/+";
            auto_clear = "speller/auto_clear";
            enable_encoder = "translator/enable_encoder";
            enable_semicolon = "key_binder/bindings/+";
            tab_clear_code = "key_binder/bindings/+";
            encode_commit_history = "translator/encode_commit_history";
            delimiter = "speller/delimiter";
        }
    }
}
