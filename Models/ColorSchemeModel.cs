using CommunityToolkit.Mvvm.ComponentModel;
using System.Security.Cryptography.Xml;

namespace WubiMaster.Models
{
    public partial class ColorSchemeModel : ObservableRecipient
    {
        [ObservableProperty]
        private ColorStyle style;

        [ObservableProperty]
        private ColorScheme usedColor;

        [ObservableProperty]
        private OtherPropertyModel otherProperty;

        public ColorSchemeModel()
        {
            Style = new ColorStyle();
            UsedColor = new ColorScheme();
            OtherProperty = new OtherPropertyModel();

            // 初始化一些默认值
            OtherProperty.LabelStr = "[ ㊀, ㊁, ㊂, ㊃, ㊄, ㊅, ㊆, ㊇, ㊈, ㊉ ]";
            OtherProperty.LabelSuffix = "";
            OtherProperty.MarkText = "";
        }
    }

    /// <summary>
    /// 针对于Rime皮肤的额外属性补充类
    /// </summary>
    public class OtherPropertyModel
    {
        /// <summary>
        /// 候选项序号样式
        /// </summary>
        public string LabelStr { get; set; }

        /// <summary>
        /// 候选项序号后缀（标签符）
        /// </summary>
        public string LabelSuffix { get; set; }

        /// <summary>
        /// 马克符标记
        /// </summary>
        public string MarkText { get; set; }

        /// <summary>
        /// 是否显示字根拆分提示
        /// </summary>
        public bool ShowSpelling { get; set; }
    }
}