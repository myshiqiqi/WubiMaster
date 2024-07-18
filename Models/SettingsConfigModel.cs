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

        /// <summary>
        /// 是否开机自动启动
        /// </summary>
        [ObservableProperty]
        private bool settingsAutoStart;

        /// <summary>
        /// 方案备份目录
        /// </summary>
        [ObservableProperty]
        private string settingsBackupPath;

        /// <summary>
        /// 诗词背景显示矢量图
        /// </summary>
        [ObservableProperty]
        private bool settingsVectorBack = true;

        /// <summary>
        /// 诗词背景显示青花女
        /// </summary>
        [ObservableProperty]
        private bool settingsQinghuaBack;
    }
}
