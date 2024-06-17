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
		[ObservableProperty]
		private bool inLine;

		[ObservableProperty]
		private bool textVertical;

		[ObservableProperty]
		private bool isBanyueMode;

		[ObservableProperty]
		private bool horizontal;

		[ObservableProperty]
		private bool isTemplateAll;

    }
}
