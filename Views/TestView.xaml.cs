/**
 * 写给开发者的一封信：
 * 
 * 很高兴你发现了藏在这里的信，这是莫大的缘分，也是历史性的一刻！
 * 中书君是我的第一个开源软件项目。
 * 这个软件没有什么架构而言，就是最简单的MVVM结构。
 * 界面采用了纯手工原生UI元素，这不是一件容易的事情，设计UI消耗了巨大的精力。
 * 有时候半夜还偷偷在次卧室里Debug代码，一是因为白天要上班挣糊口费，另一方面是因为怕吵到家人。
 * 做这个项目的念头是从新冠第二年中旬时有的，那时失业在家，焦虑之余还怀着一份梦想。
 * 回想起来，写下第一句“Hello World”时，正好是十年前。
 * 而今驻足自省，发现年轻时的豪言壮语成了一句句空洞的回音，平凡的我依旧平凡。
 * 原来，我也像大多数人一样，平庸，还被生活折腾得蓬头垢面。
 * 但我是个爱笑的人，失业的时候仰头大笑，生病的时候边咳嗽边笑，职场失意时淡然一笑，看到所爱之人时会心一笑。
 * 也许你也是一个程序员也许不是，也许你年轻也许不年轻，也许你有所热爱也许有所痛恨。
 * 不论如何，都想对你说一声：朋友，你还好吗？
 * 愿你所走之路，皆是康庄大道；所遇之人，皆是良人；愿你勇气不减，活成自己想成的模样！
 * 
 * 空山明月 2024年8月3日
 * **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WubiMaster.Views
{
    /// <summary>
    /// TestView.xaml 的交互逻辑
    /// </summary>
    public partial class TestView : UserControl
    {
        public TestView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string txtSource = this.tbMultiLine1.Text;
            if (txtSource == "") return;

            string[] txtSources = txtSource.Split("\n");
            Dictionary<string, string> colorDict = new Dictionary<string, string>();
            foreach (string s in txtSources)
            {
                var _str = s.Trim();
                if (_str == "") continue;
                string[] _strs = _str.Replace("--", "").Replace("#", "").Replace(";", "").Split(":");
                colorDict.Add(_strs[0], _strs[1]);
            }



            //--primary - 100:#0085ff;
            //--primary - 200:#69b4ff;
            //--primary - 300:#e0ffff;
            //--accent - 100:#006fff;
            //--accent - 200:#e1ffff;
            //--text - 100:#FFFFFF;
            //--text - 200:#9e9e9e;
            //--bg - 100:#1E1E1E;
            //--bg - 200:#2d2d2d;
            //--bg - 300:#454545;


            //back_color: 0xFFFFFF
            //border_color: 0xCCCCCC
            //label_color: 0x5C5C5C
            //text_color: 0x8F3F00
            //hilited_text_color: 0xF5F5F5
            //hilited_back_color: 0xF39621
            //hilited_candidate_text_color: 0xFFFFFF
            //hilited_candidate_back_color: 0xB5513F
            //hilited_label_color: 0xF5F5F5
            //hilited_comment_text_color: 0xF5F5F5
            //candidate_text_color: 0x333333
            //comment_text_color: 0x5C5C5C

            Dictionary<string, string> themeDict = new Dictionary<string, string>();
            themeDict.Add("back_color", ColorConvert(colorDict["bg-100"]));
            themeDict.Add("border_color", ColorConvert(colorDict["bg-300"]));
            themeDict.Add("label_color", ColorConvert(colorDict["text-200"]));
            themeDict.Add("text_color", ColorConvert(colorDict["accent-200"]));
            themeDict.Add("hilited_text_color", ColorConvert(colorDict["bg-200"]));
            themeDict.Add("hilited_back_color", ColorConvert(colorDict["accent-100"]));
            themeDict.Add("hilited_candidate_text_color", ColorConvert(colorDict["bg-100"]));
            themeDict.Add("hilited_candidate_back_color", ColorConvert(colorDict["primary-100"]));
            themeDict.Add("hilited_label_color", ColorConvert(colorDict["bg-200"]));
            themeDict.Add("hilited_comment_text_color", ColorConvert(colorDict["bg-200"]));
            themeDict.Add("candidate_text_color", ColorConvert(colorDict["text-100"]));
            themeDict.Add("comment_text_color", ColorConvert(colorDict["text-200"]));

            string themeStrs = "";
            foreach (var k in themeDict.Keys)
            {
                themeStrs += $"{k}: {themeDict[k]}\n";
            }
            this.tbMultiLine2.Text = themeStrs;
            Console.WriteLine();
        }

        private string ColorConvert(string sourceColor)
        {
            var colorValus = sourceColor.ToCharArray();
            string color = $"{colorValus[4]}{colorValus[5]}{colorValus[2]}{colorValus[3]}{colorValus[0]}{colorValus[1]}";
            return "0x" + color.ToUpper();
        }
    }
}
