using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WubiMaster.Models
{
	/// <summary>
	/// 皮肤布局模板模型类
	/// </summary>
    public partial class ColorTemplateModel : ObservableRecipient
    {
		/// <summary>
		/// 是否将编码内嵌到行内
		/// </summary>
		[ObservableProperty]
		private bool inLine;

		/// <summary>
		/// 是否采用文本垂直模式
		/// </summary>
		[ObservableProperty]
		private bool textVertical;

		/// <summary>
		/// 是否切换到半月模式，即天圆地方模式
		/// </summary>
		[ObservableProperty]
		private bool isBanyueMode;

		/// <summary>
		/// 是否横各显示外观
		/// </summary>
		[ObservableProperty]
		private bool horizontal;

		/// <summary>
		/// 是否将模板应用到所有皮肤
		/// </summary>
		[ObservableProperty]
		private bool isTemplateAll;

		/// <summary>
		/// 文本垂直模式时，候选项是否自左向右排列
		/// </summary>
		[ObservableProperty]
		private bool isTextLeftToRight;

    }
}
