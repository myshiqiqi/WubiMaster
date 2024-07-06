using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WubiMaster.Models
{
    public partial class SettingsConfigModel : ConfigBaseModel
    {
        /// <summary>
        /// 经典模式
        /// </summary>
        [ObservableProperty]
        private bool settingsClassMode = true;

        /// <summary>
        /// 音辅模式
        /// </summary>
        [ObservableProperty]
        private bool settingsYinfuMode;
    }
}
