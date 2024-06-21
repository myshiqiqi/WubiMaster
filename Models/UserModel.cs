using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WubiMaster.Models
{
    public partial class UserModel : BaseModel
    {
        [ObservableProperty]
        private bool isWubi = true;

        [ObservableProperty]
        private bool isWubiPinyin;

        [ObservableProperty]
        private bool isPinyin;
    }
}
