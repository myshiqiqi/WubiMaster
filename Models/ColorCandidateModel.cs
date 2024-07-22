using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using WubiMaster.Common;

namespace WubiMaster.Models
{
    /// <summary>
    /// 皮肤候选项模型类
    /// </summary>
    public partial class ColorCandidateModel : ObservableRecipient
    {
        // 序号类型列表
        [ObservableProperty]
        private Dictionary<string, string> labelDict;

        // 选择的序号index
        [ObservableProperty]
        private int labelIndex;

        // 选中的后缀index
        [ObservableProperty]
        private int labelSuffixIndex;

        // 序号后缀（分隔符）
        [ObservableProperty]
        private List<string> labelSuffixList;

        // 首选项标记符index
        [ObservableProperty]
        private int markTextIndex;

        // mark符集合
        [ObservableProperty]
        private List<string> markTextList;

        // 候选数值Index
        [ObservableProperty]
        private int numIndex;

        // 候选数列表
        [ObservableProperty]
        private List<string> numList;

        [ObservableProperty]
        private List<string> localFonts;

        public ColorCandidateModel()
        {
            // 初始化序号列表
            InitLabelDict();
            // 初始化序号候选数列表
            InitLabelCount();
            // 初始化后缀集合
            InitSuffixList();
            // 初始化 Mark 符集合
            InitMarkTextList();
            // 加载本地字体
            GetLocalFonts();
            // 加载配置数据
            LoadConfig();
        }

        private void GetLocalFonts()
        {
            LocalFonts = new List<string>();
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                LocalFonts.Add(fontFamily.ToString());
            }
        }

        public void Change()
        {
            NumList.Clear();
            var label_str = LabelDict.Values.ToList()[LabelIndex];
            var label_length = label_str.Replace("[", "").Replace("]", "").Replace(",", "").Replace(" ", "").Trim().Length;
            int min_count = 3;
            int max_count = label_length > 10 ? 10 : label_length;
            for (int i = min_count; i <= max_count; i++)
            {
                NumList.Add(i.ToString());
            }

            NumIndex = NumIndex >= NumList.Count ? NumList.Count - 1 : NumIndex;
        }

        // 将必要的数据保存到配置文件中去
        public void SaveConfig()
        {
            ConfigHelper.WriteConfigByInt("candidate_num_index", NumIndex);
            ConfigHelper.WriteConfigByInt("candidate_label_index", LabelIndex);
            ConfigHelper.WriteConfigByInt("candidate_label_suffix_index", LabelSuffixIndex);
            ConfigHelper.WriteConfigByInt("candidate_mark_text_index", MarkTextIndex);
        }

        private void AddLabel(string label_strs)
        {
            string[] label_array = label_strs.Replace(" ", "").Split(",");
            LabelDict.Add($"{label_array[0]}{label_array[1]}{label_array[2]}{label_array[3]}...{label_array[label_array.Length - 1]}", $"[ {label_strs} ]");
        }

        private void InitLabelCount()
        {
            NumList ??= new List<string>();
            var label_str = LabelDict.Values.ToList()[LabelIndex];
            var label_length = label_str.Replace("[", "").Replace("]", "").Replace(",", "").Replace(" ", "").Trim().Length;
            int min_count = 3;
            int max_count = label_length > 10 ? 10 : label_length;
            for (int i = min_count; i <= max_count; i++)
            {
                NumList.Add(i.ToString());
            }

            NumIndex = 2; // 默认是第三位选项值，即5个候选项值
        }

        private void InitLabelDict()
        {
            LabelDict = new Dictionary<string, string>();
            AddLabel("㊀, ㊁, ㊂, ㊃, ㊄, ㊅, ㊆, ㊇, ㊈, ㊉");
            AddLabel("〡, 〢, 〣, 〤, 〥, 〦, 〧, 〨, 〩, 〸, 〹, 〺");
            AddLabel("甲, 乙, 丙, 丁, 戊, 己, 庚, 辛, 壬, 癸");
            AddLabel("子, 丑, 寅, 卯, 辰, 巳, 午, 未, 申, 酉, 戌, 亥");
            AddLabel("Ⅰ, Ⅱ, Ⅲ, Ⅳ, Ⅴ, Ⅵ, Ⅶ, Ⅷ, Ⅸ, Ⅹ, Ⅺ, Ⅻ");
            AddLabel("1, 2, 3, 4, 5, 6, 7, 8, 9, 10");
            AddLabel("⑴, ⑵, ⑶, ⑷, ⑸, ⑹, ⑺, ⑻, ⑼, ⑽");
            AddLabel("☰, ☱, ☲, ☳, ☴, ☵, ☶, ☷");
            AddLabel("♈, ♉, ♊, ♋, ♌, ♍, ♎, ♏, ♐, ♑, ♒, ♓");
            AddLabel("♔, ♕, ♖, ♗, ♘, ♙, ♚, ♛, ♜, ♝, ♞, ♟");
            AddLabel("♠, ♥, ♣, ♦, ♤, ♡, ♧, ♢");
            AddLabel("⚀, ⚁, ⚂, ⚃, ⚄, ⚅");
            AddLabel("❶, ❷, ❸, ❹, ❺, ❻, ❼, ❽, ❾, ❿");
            AddLabel("①, ②, ③, ④, ⑤, ⑥, ⑦, ⑧, ⑨, ⑩");
            AddLabel("𝄞, ♩, ♪, ♫, ♬, ♭, ♮, ♯");
            AddLabel("🀀, 🀁, 🀂, 🀃, 🀄, 🀅, 🀆");
            AddLabel("🀇, 🀈, 🀉, 🀊, 🀋, 🀌, 🀍, 🀎, 🀏");
            AddLabel("🀐, 🀑, 🀒, 🀓, 🀔, 🀕, 🀖, 🀗, 🀘");
            AddLabel("🀙, 🀚, 🀛, 🀜, 🀝, 🀞, 🀟, 🀠, 🀡");
            AddLabel("🀢, 🀣, 🀤, 🀥, 🀦, 🀧, 🀨, 🀩, 🀪, 🀫");
        }

        private void InitMarkTextList()
        {
            MarkTextList = new List<string>();
            string[] mark_texts = "默认, 无, ★, ☆, ⛤, ⛥, ⛦, ⛧, ✡, ❋, ❊, ❉, ❈, ❇, ❆, ❅, ❄, ❃, ❂, ❁, ❀, ✿, ✾, ✽, ✼, ✻, ✺, ✹, ✸, ✷, ✶, ✵, ✴, ✳, ✲, ✱, ✰, ✯, ✮, ✭, ✬, ✫, ✪, ✩, ✧, ✦, ✥, ✤, ✣, ✢".Replace(" ", "").Split(",");
            for (int i = 0; i < mark_texts.Length; i++)
            {
                MarkTextList.Add(mark_texts[i]);
            }
        }

        private void InitSuffixList()
        {
            LabelSuffixList = new List<string>();
            string[] suffix_strs = "无,.,空格,-,|,■,□,→,↣,➼,➤,~,:,#,*,+,●".Split(",");
            for (int i = 0; i < suffix_strs.Length; i++)
            {
                LabelSuffixList.Add(suffix_strs[i]);
            }
        }

        /// <summary>
        /// 从配置文件中加载数据
        /// </summary>
        private void LoadConfig()
        {
            NumIndex = ConfigHelper.ReadConfigByInt("candidate_num_index", 2);
            LabelIndex = ConfigHelper.ReadConfigByInt("candidate_label_index", 0);
            LabelSuffixIndex = ConfigHelper.ReadConfigByInt("candidate_label_suffix_index", 0);
            MarkTextIndex = ConfigHelper.ReadConfigByInt("candidate_mark_text_index", 0);
        }
    }
}