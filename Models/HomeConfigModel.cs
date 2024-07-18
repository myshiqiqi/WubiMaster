using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WubiMaster.Models
{
    public partial class HomeConfigModel : ConfigBaseModel
    {
        /// <summary>
        /// 诗词背景显示青花女
        /// </summary>
        [ObservableProperty]
        private bool homeQinghuaBack;

        [ObservableProperty]
        private bool homeVectorBack = true;
    }
}
