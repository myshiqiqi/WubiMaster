using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WubiMaster.Models;

namespace WubiMaster.ViewModels
{
    public partial class AttributeViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private AttributeModel attributeModel;

        private UserCustomModel UserCustom;

        private WubiCustomModel WubiCustom;

        public AttributeViewModel()
        {
            AttributeModel = new AttributeModel();
            WubiCustom = new WubiCustomModel();
            UserCustom = new UserCustomModel();
        }

        [RelayCommand]
        public void ChangeDict()
        {
            string dict_type = "";
            if (AttributeModel.IsDict86)
                dict_type = "86";
            else if (AttributeModel.IsDict98)
                dict_type = "98";
            else
                dict_type = "06";

            AttributeModel.SaveConfig();
            WeakReferenceMessenger.Default.Send<string, string>(dict_type, "ChangeWubiDict");
        }

        [RelayCommand]
        public void ChangeSchema(object obj)
        {
            string schema = "";
            if (AttributeModel.IsWubiPinyin)
                schema = "wubi_pinyin";
            else if (AttributeModel.IsWubi)
                schema = "wubi";
            else
                schema = "luna_pinyin_simp";

            UserCustom.SetSchema(schema);
            UserCustom.Write();
            AttributeModel.SaveConfig();
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
                        if (!AttributeModel.ShowSpelling) AttributeModel.ShowPinyin = true;
                        break;

                    case "ShowPinyin":  // 注音提示开关
                        if (!AttributeModel.ShowPinyin) AttributeModel.ShowSpelling = true;
                        break;

                    case "EnableUserDict":  // 动态调频开关
                        if (!AttributeModel.EnableUserDict)
                        {
                            AttributeModel.EnableEncoder = false;  // 自造词禁
                        }
                        break;

                    case "EnableSentence":  // 连打模式开关
                        if (AttributeModel.EnableSentence)
                        {
                            AttributeModel.AutoSelect = false;  // 四码唯一自动上屏禁
                            AttributeModel.AutoClear = false; // 空码时清空编码禁
                            AttributeModel.AutoTopWord = false;  // 第五码将首选上屏禁
                            WubiCustom.DelAttribute(WubiCustom.max_code_length);  // 取消编码长度限制
                        }
                        break;

                    case "AutoSelect":  // 四码唯一自动上屏开关
                        if (AttributeModel.AutoSelect)
                        {
                            AttributeModel.EnableSentence = false;  // 连打模式禁
                            AttributeModel.AutoTopWord = false;  // 第五码将首选上屏禁
                            WubiCustom.SetAttribute(WubiCustom.max_code_length, "4");  // 编码最长长度固定为4
                        }
                        break;

                    case "AutoClear":  // 空码自动清空编码开关
                        if (AttributeModel.AutoClear)
                        {
                            AttributeModel.EnableSentence = false;  // 连打模式禁
                            WubiCustom.SetAttribute(WubiCustom.max_code_length, "4");  // 编码最长长度固定为4
                        }
                        break;

                    case "AutoTopWord":  // 第五码将首选上屏开关
                        if (AttributeModel.AutoTopWord)
                        {
                            AttributeModel.EnableSentence = false;  // 连打模式禁
                            AttributeModel.AutoSelect = false;  // 四码唯一自动上屏禁
                            WubiCustom.SetAttribute(WubiCustom.max_code_length, "4");  // 编码最长长度固定为4
                        }
                        break;

                    case "EnableEncoder":  // 智能造词开关
                        if (AttributeModel.EnableEncoder)
                        {
                            AttributeModel.EnableUserDict = true;  // 动态调频启用（使用用字典功能）
                        }
                        break;

                    default:
                        break;
                }
            }

            /**----------------菜单类开关----------------**/
            string switch_str = "";
            // emoji 表情开关
            switch_str += "\r\n    - {name: show_es, reset: " + (AttributeModel.ShowEs ? "1" : "0") + "}";
            WubiCustom.SetAttribute(WubiCustom.switches, switch_str);
            // 繁简切换开关
            switch_str += "\r\n    - {name: zh_trad, reset: " + (AttributeModel.ZhTrad ? "1" : "0") + "}";
            WubiCustom.SetAttribute(WubiCustom.switches, switch_str);
            // 字根拆分提示开关
            switch_str += "\r\n    - {name: new_spelling, reset: " + (AttributeModel.ShowSpelling ? "1" : "0") + "}";
            WubiCustom.SetAttribute(WubiCustom.switches, switch_str);
            // 注音提示开关
            switch_str += "\r\n    - {name: new_hide_pinyin, reset: " + (AttributeModel.ShowPinyin ? "1" : "0") + "}";
            WubiCustom.SetAttribute(WubiCustom.switches, switch_str);
            // 生僻字开关
            switch_str += "\r\n    - {name: GB2312, reset: " + (AttributeModel.IsGb2312 ? "1" : "0") + "}";
            WubiCustom.SetAttribute(WubiCustom.switches, switch_str);
            // 单字流开关
            switch_str += "\r\n    - {name: single_char, reset: " + (AttributeModel.IsSingleChar ? "1" : "0") + "}";
            WubiCustom.SetAttribute(WubiCustom.switches, switch_str);
            // 全角半角开关
            switch_str += "\r\n    - {name: full_shape, reset: " + (AttributeModel.IsFullShape ? "1" : "0") + "}";
            WubiCustom.SetAttribute(WubiCustom.switches, switch_str);
            /**----------------菜单类开关----------------**/
            // 启用自动调频
            WubiCustom.SetAttribute(WubiCustom.enable_user_dict, AttributeModel.EnableUserDict.ToString().ToLower());
            // 启用逐码提示
            WubiCustom.SetAttribute(WubiCustom.enable_completion, AttributeModel.EnableCompletion.ToString().ToLower());
            // 启用连打模式，与四码唯一自动上屏互冲，需要添加分词符以帮助长句模式
            WubiCustom.SetAttribute(WubiCustom.enable_sentence, AttributeModel.EnableSentence.ToString().ToLower());
            WubiCustom.SetAttribute(WubiCustom.delimiter, AttributeModel.EnableSentence ? "\" , . `\"" : "");
            // 启用四码唯一自动上屏，与连打模式互冲
            WubiCustom.SetAttribute(WubiCustom.auto_select, AttributeModel.AutoSelect.ToString().ToLower());
            // 空码时清空编码
            WubiCustom.SetAttribute(WubiCustom.auto_clear, AttributeModel.AutoClear ? "max_length" : "");
            // 智能造词
            WubiCustom.SetAttribute(WubiCustom.enable_encoder, AttributeModel.EnableEncoder.ToString().ToLower());
            // 对已上屏词自动成词
            AttributeModel.EncodeCommitHistory = AttributeModel.EnableEncoder;
            WubiCustom.SetAttribute(WubiCustom.encode_commit_history, AttributeModel.EncodeCommitHistory.ToString().ToLower());
            /**----------------处理快捷键类指令----------------**/
            string key_str = "";
            // 回车清空编码
            string enter_clear_code_str = "\r\n    - {accept: Return, send: Escape, when: composing}\r\n    - {accept: Return, send: Escape, when: has_menu}";
            key_str += AttributeModel.EnterClearCode ? enter_clear_code_str : "";
            WubiCustom.SetAttribute(WubiCustom.enter_clear_code, key_str);
            // Tab 键清空编码
            string tab_clear_code_str = "\r\n    - {accept: Tab, send: Escape, when: composing}\r\n    - {accept: Tab, send: Escape, when: has_menu}";
            key_str += AttributeModel.TabClearCode ? tab_clear_code_str : "";
            WubiCustom.SetAttribute(WubiCustom.tab_clear_code, key_str);
            // 分号单引号二三候选
            string enable_semicolon_str = "\r\n    - {accept: semicolon, send: 2, when: has_menu}\r\n    - {accept: apostrophe, send: 3, when: has_menu}";
            key_str += AttributeModel.EnableSemicolon ? enable_semicolon_str : "";
            WubiCustom.SetAttribute(WubiCustom.enable_semicolon, key_str);
            /**----------------处理快捷键类指令----------------**/

            // 处理最长编码，如果没有引用，则不限制长度
            if (!AttributeModel.AutoSelect && !AttributeModel.AutoClear && !AttributeModel.AutoTopWord)
                WubiCustom.DelAttribute(WubiCustom.max_code_length);  // 取消编码长度限制

            // 将配置写入 custom
            WubiCustom.Write();
            // 将属性值保存到配置文件
            AttributeModel.SaveConfig();
        }
    }
}