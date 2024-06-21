using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            // 更新菜单选项状态值
            WubiCustom.SetAttribute(WubiCustom.show_es, AttributeModel.ShowEs ? "1" : "0");
            WubiCustom.SetAttribute(WubiCustom.zh_trad, AttributeModel.ZhTrad ? "1" : "0");
            WubiCustom.SetAttribute(WubiCustom.new_spelling, AttributeModel.ShowSpelling ? "1" : "0");
            WubiCustom.SetAttribute(WubiCustom.new_hide_pinyin, AttributeModel.ShowPinyin ? "1" : "0");
            WubiCustom.SetAttribute(WubiCustom.GB2312, AttributeModel.IsGb2312 ? "1" : "0");
            WubiCustom.SetAttribute(WubiCustom.single_char, AttributeModel.IsSingleChar ? "1" : "0");
            WubiCustom.SetAttribute(WubiCustom.full_shape, AttributeModel.IsFullShape ? "1" : "0");

            // custom 写入 & 属性值保存到配置文件中
            WubiCustom.Write();
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
                schema = "pinyin";

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
