using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        public void Update(object obj)
        {
            //WubiCustom.SetAttribute(WubiCustom.max_code_length, "10000");  // 默认不限制编码长度

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
                            AttributeModel.EnableEncoder = false;  // 智能造词禁
                            WubiCustom.SetAttribute(WubiCustom.max_code_length, "4");  // 编码最长长度固定为4
                        }
                        break;
                    case "AutoClear":  // 空码自动清空编码开关
                        if (AttributeModel.AutoClear)
                        {
                            AttributeModel.EnableSentence = false;  // 连打模式禁
                            AttributeModel.EnableEncoder = false;  // 智能造词禁
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
                            AttributeModel.AutoSelect = false;  // 四码唯一自动上屏禁
                            AttributeModel.EnableSentence = true;  // 连打模式启用
                            AttributeModel.EnableUserDict = true;  // 自动调频启用
                            AttributeModel.AutoClear = false; // 空码时清空编码禁
                            WubiCustom.DelAttribute(WubiCustom.max_code_length);  // 取消编码长度限制
                        }
                        break;
                    default:
                        break;
                }

            }

            // emoji 表情开关
            WubiCustom.SetAttribute(WubiCustom.show_es, AttributeModel.ShowEs ? "1" : "0");
            // 繁简切换开关
            WubiCustom.SetAttribute(WubiCustom.zh_trad, AttributeModel.ZhTrad ? "1" : "0");
            // 字根拆分提示开关
            WubiCustom.SetAttribute(WubiCustom.new_spelling, AttributeModel.ShowSpelling ? "1" : "0");
            // 注音提示开关
            WubiCustom.SetAttribute(WubiCustom.new_hide_pinyin, AttributeModel.ShowPinyin ? "1" : "0");
            // 生僻字开关
            WubiCustom.SetAttribute(WubiCustom.GB2312, AttributeModel.IsGb2312 ? "1" : "0");
            // 单字流开关
            WubiCustom.SetAttribute(WubiCustom.single_char, AttributeModel.IsSingleChar ? "1" : "0");
            // 全角半角开关
            WubiCustom.SetAttribute(WubiCustom.full_shape, AttributeModel.IsFullShape ? "1" : "0");
            // 启用自动调频
            WubiCustom.SetAttribute(WubiCustom.enable_user_dict, AttributeModel.EnableUserDict.ToString().ToLower());
            // 启用逐码提示
            WubiCustom.SetAttribute(WubiCustom.enable_completion, AttributeModel.EnableCompletion.ToString().ToLower());
            // 启用连打模式，与四码唯一自动上屏互冲
            WubiCustom.SetAttribute(WubiCustom.enable_sentence, AttributeModel.EnableSentence.ToString().ToLower());
            // 启用四码唯一自动上屏，与连打模式互冲
            WubiCustom.SetAttribute(WubiCustom.auto_select, AttributeModel.AutoSelect.ToString().ToLower());
            // 空码时清空编码
            WubiCustom.SetAttribute(WubiCustom.auto_clear, AttributeModel.AutoClear ? "max_length" : "");
            // 智能造词
            WubiCustom.SetAttribute(WubiCustom.enable_encoder, AttributeModel.EnableEncoder.ToString().ToLower());
            /**----------------处理快捷键类指令----------------**/
            // 回车清空编码
            string key_str = "";
            string enter_clear_code_str = "\r\n    - {accept: Return, send: Escape, when: composing}\r\n    - {accept: Return, send: Escape, when: has_menu}";
            key_str += AttributeModel.EnterClearCode ? enter_clear_code_str :"";
            WubiCustom.SetAttribute(WubiCustom.enter_clear_code, key_str);
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
    }
}
