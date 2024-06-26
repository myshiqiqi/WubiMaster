using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WubiMaster.Models
{
    public partial class ThemeConfigModel : ConfigBaseModel
    {
        /**
         * 备注：
         * config model 用于配置不同页面或不同功能的属性值
         * 为了防止属性名称重复，这里采用在属性名称前添加关键词作为区分
         * 例如：本类是用于配置皮肤外观属性值信息的，因此属性的前缀都添加了 theme 前缀
         **/

        /// <summary>
        /// 是否显示拆分，仅供展示用
        /// 默认是显示的
        /// </summary>
        [ObservableProperty]
        private bool themeShowSpell = true;
    }
}
