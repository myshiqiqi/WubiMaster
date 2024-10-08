﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WubiMaster.Common;
using WubiMaster.Models;

namespace WubiMaster.Controls
{
    public partial class ShiciControl : UserControl
    {
        public static readonly DependencyProperty IconTextProperty =
            DependencyProperty.Register("IconText", typeof(string), typeof(ShiciControl), new PropertyMetadata("&#xeb2c;"));

        public static readonly DependencyProperty ShiciAuthorProperty =
                    DependencyProperty.Register("ShiciAuthor", typeof(string), typeof(ShiciControl));

        public static readonly DependencyProperty ShiciImageProperty =
                            DependencyProperty.Register("ShiciImage", typeof(ImageSource), typeof(ShiciControl));

        public static readonly DependencyProperty ShiciIntervalProperty =
            DependencyProperty.Register("ShiciInterval", typeof(int), typeof(ShiciControl), new PropertyMetadata(25, new PropertyChangedCallback(OnShiciIntervalChanged)));

        public static readonly DependencyProperty ShiciTextsProperty =
            DependencyProperty.Register("ShiciTexts", typeof(List<string>), typeof(ShiciControl));

        public static readonly DependencyProperty ShiciTitleProperty =
                            DependencyProperty.Register("ShiciTitle", typeof(string), typeof(ShiciControl));

        public static readonly DependencyProperty ShowQinghuaBackProperty =
            DependencyProperty.Register("ShowQinghuaBack", typeof(bool), typeof(ShiciControl), new PropertyMetadata(false));

        public static readonly DependencyProperty ShowVectorBackProperty =
            DependencyProperty.Register("ShowVectorBack", typeof(bool), typeof(ShiciControl), new PropertyMetadata(true));

        public static readonly DependencyProperty Tag1Property =
                                    DependencyProperty.Register("Tag1", typeof(string), typeof(ShiciControl));

        public static readonly DependencyProperty Tag2Property =
            DependencyProperty.Register("Tag2", typeof(string), typeof(ShiciControl));

        public static readonly DependencyProperty Tag3Property =
            DependencyProperty.Register("Tag3", typeof(string), typeof(ShiciControl));

        private List<ShiciContentModel> defaultShiciList;

        private bool IsFirstOpen = true;

        public ShiciControl()
        {
            Icons = new List<string>();
            ShiciTimer = new DispatcherTimer();
            InitializeComponent();
            InitDefaultShici();
            InitImages();
            InitTimer();

            ShiciImage = ChangeImage();
            InitIconText();
            ChangeIcon();
            GetJinrishiciAsync();
        }

        public List<ShiciContentModel> DefaultShiciList
        {
            get { return defaultShiciList; }
            set { defaultShiciList = value; }
        }

        public List<string> Icons { get; set; }

        public string IconText
        {
            get { return (string)GetValue(IconTextProperty); }
            set { SetValue(IconTextProperty, value); }
        }

        public List<ImageSource> Images { get; set; }

        public string ShiciAuthor
        {
            get { return (string)GetValue(ShiciAuthorProperty); }
            set { SetValue(ShiciAuthorProperty, value); }
        }

        public ImageSource ShiciImage
        {
            get { return (ImageSource)GetValue(ShiciImageProperty); }
            set { SetValue(ShiciImageProperty, value); }
        }

        public int ShiciInterval
        {
            get { return (int)GetValue(ShiciIntervalProperty); }
            set { SetValue(ShiciIntervalProperty, value); }
        }

        public List<string> ShiciTexts
        {
            get { return (List<string>)GetValue(ShiciTextsProperty); }
            set { SetValue(ShiciTextsProperty, value); }
        }

        public DispatcherTimer ShiciTimer { get; set; }

        public string ShiciTitle
        {
            get { return (string)GetValue(ShiciTitleProperty); }
            set { SetValue(ShiciTitleProperty, value); }
        }

        public bool ShowQinghuaBack
        {
            get { return (bool)GetValue(ShowQinghuaBackProperty); }
            set { SetValue(ShowQinghuaBackProperty, value); }
        }

        public bool ShowVectorBack
        {
            get { return (bool)GetValue(ShowVectorBackProperty); }
            set { SetValue(ShowVectorBackProperty, value); }
        }

        public string Tag1
        {
            get { return (string)GetValue(Tag1Property); }
            set { SetValue(Tag1Property, value); }
        }

        public string Tag2
        {
            get { return (string)GetValue(Tag2Property); }
            set { SetValue(Tag2Property, value); }
        }

        public string Tag3
        {
            get { return (string)GetValue(Tag3Property); }
            set { SetValue(Tag3Property, value); }
        }

        public string Token { get; set; }

        private HttpRequestHelper httpRequestHelper { get; set; }

        public void ChangeIcon()
        {
            if (Icons == null)
                return;

            Random rd = new Random();
            int index = rd.Next(Icons.Count);
            IconText = Icons[index];
        }

        private static void OnShiciIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShiciControl shiciControl = (ShiciControl)d;
            int newValue = (int)e.NewValue;
            if (shiciControl.ShiciTimer == null) return;
            shiciControl.ShiciTimer.Stop();
            shiciControl.ShiciTimer.Interval = TimeSpan.FromMinutes(newValue);

            shiciControl.ShiciImage = shiciControl.ChangeImage();
            shiciControl.ChangeIcon();
            shiciControl.GetJinrishiciAsync();

            shiciControl.ShiciTimer.Start();
        }

        private ImageSource ChangeImage()
        {
            Random random = new Random();
            int index = random.Next(0, Images.Count);
            return Images[index];
        }

        private async Task GetJinrishiciAsync()
        {
            try
            {
                DateTime todayTime = DateTime.Now;// Convert.ToDateTime("2023/11/28");
                ShiciType type = ShiciType.Defualt;
                string tag = "";

                string jieriToday = NongliHelper.GetChinaHoliday(todayTime);
                string jieqiLast = NongliHelper.GetSolarTermLast(todayTime);
                string jieqiHouLast = NongliHelper.GetJieqiHou(todayTime);
                string jijieToday = NongliHelper.GetJijie(todayTime);
                string monthToday = NongliHelper.GetMonth(todayTime);
                string monthName = NongliHelper.GetJiejieMonth(todayTime);
                string dayToday = NongliHelper.GetDay(todayTime);
                tag = monthName;

                if (!string.IsNullOrEmpty(jieriToday))
                {
                    tag = jieriToday;
                    switch (jieriToday)
                    {
                        case "春节":
                            type = ShiciType.Chunjie;
                            break;

                        case "元宵节":
                            type = ShiciType.Yuanxiaojie;
                            break;

                        case "寒食节":
                            type = ShiciType.Hanshijie;
                            break;

                        case "清明节":
                            type = ShiciType.Qingmingjie;
                            break;

                        case "端午节":
                            type = ShiciType.Duanwujie;
                            break;

                        case "七夕节":
                            type = ShiciType.Qixijie;
                            break;

                        case "中秋节":
                            type = ShiciType.Zhongqiujie;
                            break;

                        case "重阳节":
                            type = ShiciType.Chongyangjie;
                            break;

                        default:
                            type = ShiciType.Defualt;
                            break;
                    }
                }
                else
                {
                    Random random = new Random();
                    int value = random.Next(0, 5);
                    if (value % 2 == 0)
                    {
                        type = ShiciType.Defualt;
                    }
                    else
                    {
                        switch (jijieToday)
                        {
                            case "春":
                                type = ShiciType.Chun;
                                break;

                            case "夏":
                                type = ShiciType.Xia;
                                break;

                            case "秋":
                                type = ShiciType.Qiu;
                                break;

                            case "冬":
                                type = ShiciType.Dong;
                                break;

                            default:
                                type = ShiciType.Defualt;
                                break;
                        }
                    }
                }

                Tag1 = jieqiLast;
                Tag2 = jieqiHouLast;
                Tag3 = tag;

                ShiciContentModel model = ShiciHelper.GetShiciByType(type);

                // 给中书君诗句添加出现的随机概率
                if (type == ShiciType.Defualt)
                {
                    Random defaultRD = new Random();
                    var rdValue = defaultRD.Next(0, 5);
                    if (rdValue == 0)
                    {
                        model = DefaultShiciList[^1];
                    }
                }

                ShiciTitle = model.origin.Split(new char[] { '·', '/' })[0].Trim();
                ShiciAuthor = model.author;
                ShiciTexts = ShiciToArray(model.content);

                // 保证最长只有三句
                for (int i = 0; i < ShiciTexts.Count - 3; i++)
                    ShiciTexts.RemoveAt(0);
            }
            catch (Exception ex)
            {
                Random random = new Random();
                int index = random.Next(0, DefaultShiciList.Count);
                ShiciContentModel model = DefaultShiciList[index];

                ShiciTitle = model.origin;
                ShiciAuthor = model.author;
                ShiciTexts = ShiciToArray(model.content);
                LogHelper.Warn(ex.Message);
            }
        }

        private void InitDefaultShici()
        {
            DefaultShiciList = new List<ShiciContentModel>();

            ShiciContentModel model1 = new ShiciContentModel();
            model1.content = "明月几时有，把酒问青天。";
            model1.origin = "水调歌头";
            model1.author = "苏轼";
            DefaultShiciList.Add(model1);

            ShiciContentModel model2 = new ShiciContentModel();
            model2.content = "天生我材必有用，千金散尽还复来。";
            model2.origin = "将进酒";
            model2.author = "李白";
            DefaultShiciList.Add(model2);

            ShiciContentModel model3 = new ShiciContentModel();
            model3.content = "流水落花春去也，天上人间。";
            model3.origin = "浪淘沙";
            model3.author = "李煜";
            DefaultShiciList.Add(model3);

            ShiciContentModel model4 = new ShiciContentModel();
            model4.content = "会当凌绝顶，一览众山小。";
            model4.origin = "望岳";
            model4.author = "杜甫";
            DefaultShiciList.Add(model4);

            ShiciContentModel model5 = new ShiciContentModel();
            model5.content = "采菊东篱下，悠然见南山。";
            model5.origin = "饮酒";
            model5.author = "陶渊明";
            DefaultShiciList.Add(model5);

            ShiciContentModel model6 = new ShiciContentModel();
            model6.content = "大漠孤烟直，长河落日圆。";
            model6.origin = "使至塞上";
            model6.author = "王维";
            DefaultShiciList.Add(model6);

            ShiciContentModel model7 = new ShiciContentModel();
            model7.content = "晩来天欲雪，能饮一杯无。";
            model7.origin = "赠刘十九";
            model7.author = "白居易";
            DefaultShiciList.Add(model7);

            ShiciContentModel model8 = new ShiciContentModel();
            model8.content = "天下英雄谁敌手？曹刘。生子当如孙仲谋。";
            model8.origin = "南乡子";
            model8.author = "辛弃疾";
            DefaultShiciList.Add(model8);

            ShiciContentModel model9 = new ShiciContentModel();
            model9.content = "青春都一饷。忍把浮名，换了浅斟低唱！";
            model9.origin = "鹤冲天";
            model9.author = "柳永";
            DefaultShiciList.Add(model9);

            ShiciContentModel model10 = new ShiciContentModel();
            model10.content = "便作春江都是泪，流不尽，许多愁。";
            model10.origin = "江城子";
            model10.author = "秦观";
            DefaultShiciList.Add(model10);

            ShiciContentModel model11 = new ShiciContentModel();
            model11.content = "多谢中书君，伴我此幽栖";
            model11.origin = "自笑";
            model11.author = "苏轼";
            DefaultShiciList.Add(model11);
        }

        private void InitIconText()
        {
            try
            {
                List<string> icon_texts = new List<string>();
                var backIconDict = new ResourceDictionary();
                backIconDict.Source = new Uri("pack://application:,,,/WubiMaster;component/Resource/ShiciIconText.xaml");
                foreach (string name in backIconDict.Keys)
                {
                    string text = backIconDict[name].ToString();
                    icon_texts.Add(text);
                }
                Icons = icon_texts;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
        }

        private void InitImages()
        {
            Images = new List<ImageSource>();
            Images.Add(new BitmapImage(new Uri("../Images/JinriShici/侧脸.png", UriKind.Relative)));
            Images.Add(new BitmapImage(new Uri("../Images/JinriShici/侧身微笑.png", UriKind.Relative)));
            Images.Add(new BitmapImage(new Uri("../Images/JinriShici/低头.png", UriKind.Relative)));
            Images.Add(new BitmapImage(new Uri("../Images/JinriShici/低头的人.png", UriKind.Relative)));
            Images.Add(new BitmapImage(new Uri("../Images/JinriShici/翘二郎腿的女人.png", UriKind.Relative)));
            Images.Add(new BitmapImage(new Uri("../Images/JinriShici/青花瓷少女.png", UriKind.Relative)));
            Images.Add(new BitmapImage(new Uri("../Images/JinriShici/青花瓷衣服.png", UriKind.Relative)));
            Images.Add(new BitmapImage(new Uri("../Images/JinriShici/手托头.png", UriKind.Relative)));
            Images.Add(new BitmapImage(new Uri("../Images/JinriShici/思考.png", UriKind.Relative)));
            Images.Add(new BitmapImage(new Uri("../Images/JinriShici/长发女人.png", UriKind.Relative)));
            Images.Add(new BitmapImage(new Uri("../Images/JinriShici/正面.png", UriKind.Relative)));
            Images.Add(new BitmapImage(new Uri("../Images/JinriShici/坐椅子的女人.png", UriKind.Relative)));
            Images.Add(new BitmapImage(new Uri("../Images/JinriShici/坐着的女人.png", UriKind.Relative)));
        }

        private void InitTimer()
        {
            ShiciTimer ??= new DispatcherTimer();
            ShiciTimer.Interval = TimeSpan.FromMinutes(ShiciInterval > 0 ? ShiciInterval : 25);
            ShiciTimer.Tick += ShiciTimer_Tick;
            ShiciTimer.Start();
        }

        private void ShiciTimer_Tick(object? sender, EventArgs e)
        {
            ShiciImage = ChangeImage();
            ChangeIcon();
            GetJinrishiciAsync();
        }

        private List<string> ShiciToArray(string text)
        {
            List<string> strList = new List<string>();

            try
            {
                var strs = text.Split(new char[] { ',', '.', ';', '/', '?', '!', '，', '。', '；', '、', '？', '！' });
                for (int i = strs.Length - 1; i >= 0; i--)
                {
                    if (!string.IsNullOrEmpty(strs[i].Trim()))
                    {
                        strList.Add(strs[i].Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }

            return strList;
        }
    }
}