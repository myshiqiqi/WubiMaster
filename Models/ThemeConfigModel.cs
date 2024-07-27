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

        /// <summary>
        /// 是否使用阴影效果
        /// </summary>
        [ObservableProperty]
        private bool themeUseShade;

        // 是否启用随机配色
        [ObservableProperty]
        private bool theme_RandomColor;

        // 是否启用跟随主题
        [ObservableProperty]
        private bool theme_FollowTheme;

        // 随机主题
        [ObservableProperty]
        private bool theme_RandomSkin;

        /// <summary>
        /// 文本字体
        /// </summary>
        [ObservableProperty]
        private string themeTextFont = "微软雅黑";

        /// <summary>
        /// 标签字体
        /// </summary>
        [ObservableProperty]
        private string themeLabelFont = "微软雅黑";

        /// <summary>
        /// 注解字体
        /// </summary>
        [ObservableProperty]
        private string themeCommentFont = "黑体字根";

        /// <summary>
        /// 文本字型
        /// </summary>
        [ObservableProperty]
        private string themeTextWeight = "常规";

        /// <summary>
        /// 标签字型
        /// </summary>
        [ObservableProperty]
        private string themeLabelWeight = "常规";

        /// <summary>
        /// 注解字型
        /// </summary>
        [ObservableProperty]
        private string themeCommentWeight = "常规";

        /// <summary>
        /// 文本字号
        /// </summary>
        [ObservableProperty]
        private string themeTextSize = "中号";

        /// <summary>
        /// 标签字号
        /// </summary>
        [ObservableProperty]
        private string themeLabelSize = "中号";

        /// <summary>
        /// 注解字号
        /// </summary>
        [ObservableProperty]
        private string themeCommentSize = "中号";
    }
}
