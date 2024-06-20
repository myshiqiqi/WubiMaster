using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WubiMaster.Common;

namespace WubiMaster.Models
{
    public partial class AttributeModel : BaseModel
    {
        [ObservableProperty]
        private bool showEs;

        [ObservableProperty]
        private bool zhTrad;

        [ObservableProperty]
        private bool showSpelling = true;

        [ObservableProperty]
        private bool showPinyin;

        [ObservableProperty]
        private bool isGb2312;

        [ObservableProperty]
        private bool isSingleChar;

        [ObservableProperty]
        private bool isFullShape;

        public AttributeModel()
        {
            LoadConfig();
        }
    }
}
