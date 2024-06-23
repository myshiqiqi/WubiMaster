using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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

        // 启用用户词典自动调频
        [ObservableProperty]
        private bool enableUserDict;

        // 启用逐码提示
        [ObservableProperty]
        private bool enableCompletion;

        // 启用连打模式
        // 需要取消最长编码
        [ObservableProperty]
        private bool enableSentence;

        // 最长编码
        [ObservableProperty]
        private int maxCodeLength;

        // 四码唯一自动上屏
        // 需要设置最长编码为4
        // 该功能与连打模式相冲
        [ObservableProperty]
        private bool autoSelect;

        // 启用回车清空编码
        [ObservableProperty]
        private bool enterClearCode;

        // 启用空码时清除编码
        [ObservableProperty]
        private bool autoClear;

        // 第五码将首选上屏(顶字上屏)
        [ObservableProperty]
        private bool autoTopWord;

        // 启用智能造词
        [ObservableProperty]
        private bool enableEncoder;

        // 启用分号模式，这里表示分号单引号设置为二三候选
        [ObservableProperty]
        private bool enableSemicolon;
    }
}
