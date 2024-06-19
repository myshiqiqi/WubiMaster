using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        private WubiCustomModel WubiCustom;

        public AttributeViewModel()
        {
            AttributeModel = new AttributeModel();
            WubiCustom = new WubiCustomModel();
        }

        [RelayCommand]
        public void Update(object obj)
        {
            WubiCustom.SetAttribute(WubiCustom.show_es, AttributeModel.ShowEs ? "1" : "0");
            WubiCustom.SetAttribute(WubiCustom.zh_trad, AttributeModel.ZhTrad ? "1" : "0");
            WubiCustom.SetAttribute(WubiCustom.new_spelling, AttributeModel.ShowSpelling ? "1" : "0");
            WubiCustom.SetAttribute(WubiCustom.GB2312, AttributeModel.IsGb2312 ? "1" : "0");
            WubiCustom.SetAttribute(WubiCustom.single_char, AttributeModel.IsSingleChar ? "1" : "0");
            WubiCustom.SetAttribute(WubiCustom.full_shape, AttributeModel.IsFullShape ? "1" : "0");

            WubiCustom.Write();
        }
    }
}
