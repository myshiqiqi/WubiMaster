using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WubiMaster.Common;

namespace WubiMaster.Models
{
    public partial class AttributeModel : BaseModel
    {
        // 表情开关
        [ObservableProperty]
        private bool showEs;

        // 简繁开关
        [ObservableProperty]
        private bool zhTrad;

        // 字根提示开关
        [ObservableProperty]
        private bool showSpelling;

        // 注音提示开关
        [ObservableProperty]
        private bool showPinyin;

        // 生僻字开关
        [ObservableProperty]
        private bool isGb2312;

        // 单字模式开关
        [ObservableProperty]
        private bool isSingleChar;

        // 全角半角开关
        [ObservableProperty]
        private bool isFullShape;

        // 纯五笔方案开关
        [ObservableProperty]
        private bool isWubi = true;

        // 五笔拼音方案开关
        [ObservableProperty]
        private bool isWubiPinyin;

        // 纯拼音方案开关
        [ObservableProperty]
        private bool isPinyin;

        // 86码表切换开关
        [ObservableProperty]
        private bool isDict86 = true;

        // 98码表切换开关
        [ObservableProperty]
        private bool isDict98;

        // 新世纪码表切换开关
        [ObservableProperty]
        private bool isDict06;
    }
}
