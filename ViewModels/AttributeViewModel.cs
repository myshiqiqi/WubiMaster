using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WubiMaster.Models;

namespace WubiMaster.ViewModels
{
    public partial class AttributeViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private AttributeConfigModel configModel;

        private UserCustomModel UserCustom;

        private WubiCustomModel WubiCustom;

        public AttributeViewModel()
        {
            ConfigModel = new AttributeConfigModel();
            WubiCustom = new WubiCustomModel();
            UserCustom = new UserCustomModel();
        }

        [RelayCommand]
        public void ChangeDict()
        {
            string dict_type = "";
            if (ConfigModel.IsDict86)
                dict_type = "86";
            else if (ConfigModel.IsDict98)
                dict_type = "98";
            else
                dict_type = "06";

            ConfigModel.SaveConfig();
            WeakReferenceMessenger.Default.Send<string, string>(dict_type, "ChangeWubiDict");
        }

        [RelayCommand]
        public void ChangeSchema(object obj)
        {
            string schema = "";
            if (ConfigModel.IsWubiPinyin)
                schema = "wubi_pinyin";
            else if (ConfigModel.IsWubi)
                schema = "wubi";
            else
                schema = "luna_pinyin_simp";

            UserCustom.SetSchema(schema);
            UserCustom.Write();
            ConfigModel.SaveConfig();
        }

        [RelayCommand]
        public void Update(object obj)
        {
            // 处理互冲的开关项
            if (obj != null)
            {
                string arg = obj.ToString();
                switch (arg)
                {
                    case "ShowSpelling":  // 拆分提示开关
                        if (!ConfigModel.ShowSpelling) ConfigModel.ShowPinyin = true;
                        break;

                    case "ShowPinyin":  // 注音提示开关
                        if (!ConfigModel.ShowPinyin) ConfigModel.ShowSpelling = true;
                        break;

                    case "EnableUserDict":  // 动态调频开关
                        if (!ConfigModel.EnableUserDict)
                        {
                            ConfigModel.EnableEncoder = false;  // 自造词禁
                            ConfigModel.DisUserDictPatt = false;  // 关闭一二三简禁用调频
                        }
                        break;

                    case "EnableSentence":  // 连打模式开关
                        if (ConfigModel.EnableSentence)
                        {
                            ConfigModel.AutoSelect = false;  // 四码唯一自动上屏禁
                            ConfigModel.AutoClear = false; // 空码时清空编码禁
                            ConfigModel.AutoTopWord = false;  // 第五码将首选上屏禁
                            WubiCustom.DelAttribute(WubiCustom.max_code_length);  // 取消编码长度限制
                        }
                        break;

                    case "AutoSelect":  // 四码唯一自动上屏开关
                        if (ConfigModel.AutoSelect)
                        {
                            ConfigModel.EnableSentence = false;  // 连打模式禁
                            ConfigModel.AutoTopWord = false;  // 第五码将首选上屏禁
                            WubiCustom.SetAttribute(WubiCustom.max_code_length, "4");  // 编码最长长度固定为4
                        }
                        break;

                    case "AutoClear":  // 空码自动清空编码开关
                        if (ConfigModel.AutoClear)
                        {
                            ConfigModel.EnableSentence = false;  // 连打模式禁
                            WubiCustom.SetAttribute(WubiCustom.max_code_length, "4");  // 编码最长长度固定为4
                        }
                        break;

                    case "AutoTopWord":  // 第五码将首选上屏开关
                        if (ConfigModel.AutoTopWord)
                        {
                            ConfigModel.EnableSentence = false;  // 连打模式禁
                            ConfigModel.AutoSelect = false;  // 四码唯一自动上屏禁
                            WubiCustom.SetAttribute(WubiCustom.max_code_length, "4");  // 编码最长长度固定为4
                        }
                        break;

                    case "EnableEncoder":  // 智能造词开关
                        if (ConfigModel.EnableEncoder)
                        {
                            ConfigModel.EnableUserDict = true;  // 动态调频启用（使用用字典功能）
                        }
                        break;

                    default:
                        break;
                }
            }

            /**----------------菜单类开关----------------**/
            string switch_str = "";
            // emoji 表情开关
            switch_str += "\r\n    - {name: show_es, reset: " + (ConfigModel.ShowEs ? "1" : "0") + "}";
            WubiCustom.SetAttribute(WubiCustom.switches, switch_str);
            // 繁简切换开关
            switch_str += "\r\n    - {name: zh_trad, reset: " + (ConfigModel.ZhTrad ? "1" : "0") + "}";
            WubiCustom.SetAttribute(WubiCustom.switches, switch_str);
            // 字根拆分提示开关
            switch_str += "\r\n    - {name: new_spelling, reset: " + (ConfigModel.ShowSpelling ? "1" : "0") + "}";
            WubiCustom.SetAttribute(WubiCustom.switches, switch_str);
            // 注音提示开关
            switch_str += "\r\n    - {name: new_hide_pinyin, reset: " + (ConfigModel.ShowPinyin ? "1" : "0") + "}";
            WubiCustom.SetAttribute(WubiCustom.switches, switch_str);
            // 生僻字开关
            switch_str += "\r\n    - {name: GB2312, reset: " + (ConfigModel.IsGb2312 ? "1" : "0") + "}";
            WubiCustom.SetAttribute(WubiCustom.switches, switch_str);
            // 单字流开关
            switch_str += "\r\n    - {name: single_char, reset: " + (ConfigModel.IsSingleChar ? "1" : "0") + "}";
            WubiCustom.SetAttribute(WubiCustom.switches, switch_str);
            // 全角半角开关
            switch_str += "\r\n    - {name: full_shape, reset: " + (ConfigModel.IsFullShape ? "1" : "0") + "}";
            WubiCustom.SetAttribute(WubiCustom.switches, switch_str);
            /**----------------菜单类开关----------------**/
            // 启用自动调频
            WubiCustom.SetAttribute(WubiCustom.enable_user_dict, ConfigModel.EnableUserDict.ToString().ToLower());
            // 一二三简禁用调频
            string dis_user_dict_patt_str = "\r\n    - \"^[a-y]{1,3}$\"";
            WubiCustom.SetAttribute(WubiCustom.dis_user_dict_patt, ConfigModel.DisUserDictPatt ? dis_user_dict_patt_str : "");
            // 启用逐码提示
            WubiCustom.SetAttribute(WubiCustom.enable_completion, ConfigModel.EnableCompletion.ToString().ToLower());
            // 启用连打模式，与四码唯一自动上屏互冲，需要添加分词符以帮助长句模式
            WubiCustom.SetAttribute(WubiCustom.enable_sentence, ConfigModel.EnableSentence.ToString().ToLower());
            WubiCustom.SetAttribute(WubiCustom.delimiter, ConfigModel.EnableSentence ? "\" , . `\"" : "");
            // 启用四码唯一自动上屏，与连打模式互冲
            WubiCustom.SetAttribute(WubiCustom.auto_select, ConfigModel.AutoSelect.ToString().ToLower());
            // 空码时清空编码
            WubiCustom.SetAttribute(WubiCustom.auto_clear, ConfigModel.AutoClear ? "max_length" : "");
            // 智能造词
            WubiCustom.SetAttribute(WubiCustom.enable_encoder, ConfigModel.EnableEncoder.ToString().ToLower());
            // 对已上屏词自动成词
            ConfigModel.EncodeCommitHistory = ConfigModel.EnableEncoder;
            WubiCustom.SetAttribute(WubiCustom.encode_commit_history, ConfigModel.EncodeCommitHistory.ToString().ToLower());
            /**----------------处理快捷键类指令----------------**/
            string key_str = "";
            // 回车清空编码
            string enter_clear_code_str = "\r\n    - {accept: Return, send: Escape, when: composing}\r\n    - {accept: Return, send: Escape, when: has_menu}";
            key_str += ConfigModel.EnterClearCode ? enter_clear_code_str : "";
            WubiCustom.SetAttribute(WubiCustom.enter_clear_code, key_str);
            // Tab 键清空编码
            string tab_clear_code_str = "\r\n    - {accept: Tab, send: Escape, when: composing}\r\n    - {accept: Tab, send: Escape, when: has_menu}";
            key_str += ConfigModel.TabClearCode ? tab_clear_code_str : "";
            WubiCustom.SetAttribute(WubiCustom.tab_clear_code, key_str);
            // 分号单引号二三候选
            string enable_semicolon_str = "\r\n    - {accept: semicolon, send: 2, when: has_menu}\r\n    - {accept: apostrophe, send: 3, when: has_menu}";
            key_str += ConfigModel.EnableSemicolon ? enable_semicolon_str : "";
            WubiCustom.SetAttribute(WubiCustom.enable_semicolon, key_str);
            /**----------------处理快捷键类指令----------------**/
            // z键万能键
            string z_omnipotent_str = "\r\n    - derive/^(...).$/$1z/\r\n    - derive/^(..).(.*)$/$1z$2/\r\n    - derive/^(.).(.*)$/$1z$2/\r\n    - derive/^.(.*)$/z$1/";
            WubiCustom.SetAttribute(WubiCustom.z_omnipotent, ConfigModel.ZOmnipotent ? z_omnipotent_str : "");
            // 编码提示键名
            string show_key_name_str = "\r\n    - 'xform/^([a-z]*)$/$1\\t〈\\U$1\\E〉/'\r\n    - \"xlit|ABCDEFGHIJKLMNOPQRSTUVWXY|工子又大月土王目水日口田山已火之金白木禾立女人幺言|\"";
            WubiCustom.SetAttribute(WubiCustom.show_key_name, ConfigModel.ShowKeyName ? show_key_name_str : "");

            // 处理最长编码，如果没有引用，则不限制长度
            if (!ConfigModel.AutoSelect && !ConfigModel.AutoClear && !ConfigModel.AutoTopWord)
                WubiCustom.DelAttribute(WubiCustom.max_code_length);  // 取消编码长度限制

            // 将配置写入 custom
            WubiCustom.Write();
            // 将属性值保存到配置文件
            ConfigModel.SaveConfig();
        }
    }
}