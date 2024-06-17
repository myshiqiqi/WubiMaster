using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace WubiMaster.Models
{
    /// <summary>
    /// 皮肤候选项模型类
    /// </summary>
    public partial class ColorCandidateModel : ObservableRecipient
    {
        [ObservableProperty]
        private int candidateCountIndex;  // 候选数值Index

        [ObservableProperty]
        private List<string> candidateCountList;  // 候选数列表

        [ObservableProperty]
        private Dictionary<string, string> labelDict;  // 序号类型列表

        [ObservableProperty]
        private int labelIndex;  // 选择的序号index

        public ColorCandidateModel()
        {
            // 初始化序号列表
            InitLabelDict();
            // 初始化序号候选数列表
            InitLabelCount();
        }

        private void AddLabel(string label_strs)
        {
            LabelDict.Add(label_strs.Replace(" ", "").Substring(0, 9) + "...", $"[ {label_strs} ]");
        }

        private void InitLabelCount()
        {
            CandidateCountList ??= new List<string>();
            var label_str = LabelDict.Values.ToList()[LabelIndex];
            var label_length = label_str.Replace("[", "").Replace("]", "").Replace(",", "").Replace(" ", "").Trim().Length;
            int min_count = 3;
            int max_count = label_length > 10 ? 10 : label_length;
            for (int i = min_count; i <= max_count; i++)
            {
                CandidateCountList.Add(i.ToString());
            }

            CandidateCountIndex = 2; // 默认是第三位选项值，即5个候选项值
        }

        private void InitLabelDict()
        {
            LabelDict = new Dictionary<string, string>();
            AddLabel("㊀, ㊁, ㊂, ㊃, ㊄, ㊅, ㊆, ㊇, ㊈, ㊉");
            AddLabel("〡, 〢, 〣, 〤, 〥, 〦, 〧, 〨, 〩, 〸, 〹, 〺");
            AddLabel("甲, 乙, 丙, 丁, 戊, 己, 庚, 辛, 壬, 癸");
            AddLabel("子, 丑, 寅, 卯, 辰, 巳, 午, 未, 申, 酉, 戌, 亥");
            AddLabel("Ⅰ, Ⅱ, Ⅲ, Ⅳ, Ⅴ, Ⅵ, Ⅶ, Ⅷ, Ⅸ, Ⅹ, Ⅺ, Ⅻ, Ⅼ, Ⅽ, Ⅾ, Ⅿ");
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
        }

        public void Update()
        {
            CandidateCountList.Clear();
            var label_str = LabelDict.Values.ToList()[LabelIndex];
            var label_length = label_str.Replace("[", "").Replace("]", "").Replace(",", "").Replace(" ", "").Trim().Length;
            int min_count = 3;
            int max_count = label_length > 10 ? 10 : label_length;
            for (int i = min_count; i <= max_count; i++)
            {
                CandidateCountList.Add(i.ToString());
            }

            CandidateCountIndex = CandidateCountIndex >= CandidateCountList.Count ? CandidateCountList.Count -1 : CandidateCountIndex;
        }
    }
}