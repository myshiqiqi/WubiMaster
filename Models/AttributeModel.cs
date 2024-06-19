using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WubiMaster.Models
{
    public partial class AttributeModel: ObservableRecipient
    {
        [ObservableProperty]
        private bool showEs;

        [ObservableProperty]
        private bool zhTrad;

        [ObservableProperty]
        private bool showSpelling;

        [ObservableProperty]
        private bool isGb2312;

        [ObservableProperty]
        private bool isSingleChar;

        [ObservableProperty]
        private bool isFullShape;

        public void SaveConfig()
        {
            
        }
    }
}
