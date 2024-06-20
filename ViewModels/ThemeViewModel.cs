using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using WubiMaster.Common;
using WubiMaster.Models;

namespace WubiMaster.ViewModels
{
    public partial class ThemeViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private bool autoColor;

        [ObservableProperty]
        private ColorCandidateModel candidateModel;  // 候选项模型类

        [ObservableProperty]
        private int colorIndex = -1;

        [ObservableProperty]
        private List<ColorsModel> colorsList;

        [ObservableProperty]
        private ColorTemplateModel colorTemplate;

        [ObservableProperty]
        private ColorSchemeModel currentColor;

        private ColorsModel default_color;

        private DefaultCustomModel DefaultCustomDetails;
        private bool is_loaded = false;

        [ObservableProperty]
        private bool randomColor;

        [ObservableProperty]
        private WeaselCustomModel weaselCustomDetails;

        private string weaselCustomPath = "";

        [ObservableProperty]
        private WeaselModel weaselDetails;

        private string weaselPath = "";

        public ThemeViewModel()
        {
            ColorsList = new List<ColorsModel>();
            ColorTemplate = new ColorTemplateModel();
            CandidateModel = new ColorCandidateModel();
            DefaultCustomDetails = new DefaultCustomModel();

            WeakReferenceMessenger.Default.Register<string, string>(this, "ChangeColorScheme", ChangeColorScheme);
            WeakReferenceMessenger.Default.Register<string, string>(this, "ChangeAutoColor", ChangeAutoColor);

            LoadColorShemes();
            LoadConfig();
        }

        /// <summary>
        /// 选择候选项样式
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        public void CandidateChange(object obj)
        {
            try
            {
                // 首先需要将新值更新到model中
                CandidateModel.Change();

                // 候选序号样式
                string candidate_str = CandidateModel.LabelDict.Values.ToList()[CandidateModel.LabelIndex];
                DefaultCustomDetails.SetAttribute(DefaultCustomDetails.SelectLabels, candidate_str);
                CurrentColor.OtherProperty.LabelStr = candidate_str;

                // 候选个数设定
                string candidate_count = CandidateModel.NumList[CandidateModel.NumIndex];
                DefaultCustomDetails.SetAttribute(DefaultCustomDetails.PageSize, candidate_count);

                // 候选序号后缀（标签符）
                string suffix_str = CandidateModel.LabelSuffixList[CandidateModel.LabelSuffixIndex];
                CurrentColor.OtherProperty.LabelSuffix = suffix_str;
                suffix_str = suffix_str == "无" ? "" : suffix_str;
                suffix_str = suffix_str == "空格" ? " " : suffix_str;
                CurrentColor.Style.label_format = "%s" + suffix_str;

                // mark 符
                //string mark_str = CandidateModel.MarkTextList[CandidateModel.MarkTextIndex];
                //CurrentColor.OtherProperty.MarkText = mark_str;
                //CurrentColor.Style.mark_text = mark_str;

                DefaultCustomDetails.Write();
                UpdateCurrentColor(null);

                CandidateModel.SaveConfig();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                this.ShowMessage("设置失败，请查看日志定位问题。");
            }
        }

        [RelayCommand]
        public void ChangeColor(object obj)
        {
            if (obj == null) return;

            try
            {
                var cModel = ColorsList.First(c => c.description.color_name == obj.ToString());
                if (cModel == null) throw new NullReferenceException($"找不到皮肤对象: {obj.ToString()}");

                ColorIndex = ColorsList.IndexOf(cModel);
                // 利用反射创建对象的副本
                // 避免在修改了临时的皮肤外观时，导致列表中的皮肤对象值也发生变化
                ColorSchemeModel _colorModel = new ColorSchemeModel();
                _colorModel.Style = CopyOut.TransReflection<ColorStyle, ColorStyle>(cModel.style);//cModel.style;
                _colorModel.UsedColor = CopyOut.TransReflection<ColorScheme, ColorScheme>(cModel.preset_color_schemes.FirstOrDefault().Value);//cModel.preset_color_schemes.FirstOrDefault().ConfigValue;
                // 加载其它项，如序号标签之类
                _colorModel.OtherProperty.LabelStr = CandidateModel.LabelDict.Values.ToList()[CandidateModel.LabelIndex];
                _colorModel.OtherProperty.LabelSuffix = CandidateModel.LabelSuffixList[CandidateModel.LabelSuffixIndex];
                //_colorModel.OtherProperty.MarkText = CandidateModel.MarkTextList[CandidateModel.MarkTextIndex];

                CurrentColor = _colorModel;

                if (ColorTemplate.IsTemplateAll)
                    SetColorFromTemp();  // 当模板应用于单个文件时，将当前主题的样式同步到模板对象中
                else
                    SetTempFromColor();  // 当模板应用于全部文件时，从模板对象中修改当前主题模板样式

                UpdateCurrentColor(null);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                this.ShowMessage("选中的样式在外观文件中不存在！", DialogType.Error);
            }
        }

        [RelayCommand]
        public void DeleteColor()
        {
            try
            {
                var ask = this.ShowAskMessage("被删除的主题无法恢复，确定要执行删除操作吗？");
                if (!((bool)ask))
                {
                    return;
                }

                string color_name = CurrentColor.Style.color_scheme;
                ColorSchemeModel csModel = new ColorSchemeModel();
                csModel.Style = default_color.style;
                csModel.UsedColor = default_color.preset_color_schemes.FirstOrDefault().Value;
                CurrentColor = csModel;

                string yaml_name = @$"{GlobalValues.UserPath}\colors\{color_name}.yaml";
                if (File.Exists(yaml_name))
                {
                    File.Delete(yaml_name);
                }
                else
                {
                    this.ShowMessage("找不到要删除的就题文件！", DialogType.Warring);
                    return;
                }

                LoadColorShemes();
                ColorIndex = 0;
                this.ShowMessage("主题删除成功！", DialogType.Success);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                this.ShowMessage("主题删除失败！", DialogType.Fail);
            }
        }

        [RelayCommand]
        public void SetAutoColor()
        {
            App.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    LoadCurrentColor();

                    if (AutoColor)
                    {
                        ChangeColor(ColorsList[ColorIndex].description.color_name);

                        SolidColorBrush text_color = (SolidColorBrush)App.Current.FindResource("text-100");
                        SolidColorBrush comment_text_color = (SolidColorBrush)App.Current.FindResource("text-200");
                        SolidColorBrush label_color = (SolidColorBrush)App.Current.FindResource("text-200");
                        SolidColorBrush back_color = (SolidColorBrush)App.Current.FindResource("bg-100");
                        SolidColorBrush shadow_color = (SolidColorBrush)App.Current.FindResource("primary-300");
                        SolidColorBrush border_color = (SolidColorBrush)App.Current.FindResource("primary-200");
                        SolidColorBrush hilited_text_color = (SolidColorBrush)App.Current.FindResource("accent-200");
                        SolidColorBrush hilited_back_color = (SolidColorBrush)App.Current.FindResource("accent-100");
                        SolidColorBrush hilited_candidate_text_color = (SolidColorBrush)App.Current.FindResource("bg-100");
                        SolidColorBrush hilited_candidate_back_color = (SolidColorBrush)App.Current.FindResource("primary-100");
                        SolidColorBrush hilited_label_color = (SolidColorBrush)App.Current.FindResource("bg-200");
                        SolidColorBrush hilited_comment_text_color = (SolidColorBrush)App.Current.FindResource("bg-200");
                        SolidColorBrush candidate_text_color = (SolidColorBrush)App.Current.FindResource("text-100");
                        SolidColorBrush candidate_back_color = (SolidColorBrush)App.Current.FindResource("bg-100");

                        //CurrentColor.UsedColor.color_format = "argb";
                        CurrentColor.UsedColor.text_color = ColorToStr(text_color.ToString());
                        CurrentColor.UsedColor.comment_text_color = ColorToStr(comment_text_color.ToString());
                        CurrentColor.UsedColor.label_color = ColorToStr(label_color.ToString());
                        CurrentColor.UsedColor.back_color = ColorToStr(back_color.ToString());
                        CurrentColor.UsedColor.shadow_color = ColorToStr(shadow_color.ToString());
                        CurrentColor.UsedColor.border_color = ColorToStr(border_color.ToString());
                        CurrentColor.UsedColor.hilited_text_color = ColorToStr(hilited_text_color.ToString());
                        CurrentColor.UsedColor.hilited_back_color = ColorToStr(hilited_back_color.ToString());
                        CurrentColor.UsedColor.hilited_candidate_text_color = ColorToStr(hilited_candidate_text_color.ToString());
                        CurrentColor.UsedColor.hilited_candidate_back_color = ColorToStr(hilited_candidate_back_color.ToString());
                        CurrentColor.UsedColor.hilited_label_color = ColorToStr(hilited_label_color.ToString());
                        CurrentColor.UsedColor.hilited_comment_text_color = ColorToStr(hilited_comment_text_color.ToString());
                        CurrentColor.UsedColor.candidate_text_color = ColorToStr(candidate_text_color.ToString());
                        CurrentColor.UsedColor.candidate_back_color = ColorToStr(candidate_back_color.ToString());

                        UpdateCurrentColor(null);
                    }

                    ConfigHelper.WriteConfigByBool("auto_color", AutoColor);
                    // 如果包含 custom 文件，先将其删除掉
                    if (File.Exists(GlobalValues.UserPath + @"\weasel.custom.yaml"))
                    {
                        File.Delete(GlobalValues.UserPath + @"\weasel.custom.yaml");
                    }

                    // 将 colors 文件下的主题数据写入到 custom 外观文件中去
                    SaveWeaselCustom();

                    string colorScheme = ColorsList[ColorIndex].description.color_name;
                    ConfigHelper.WriteConfigByString("color_scheme", colorScheme);
                    ServiceHelper.Deployer();
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.ToString());
                    this.ShowMessage("设置主题过程发生错误，请查看日志", DialogType.Error);
                }
            });
        }

        [RelayCommand]
        public void SetColor()
        {
            // 如果包含 custom 文件，先将其删除掉
            if (File.Exists(GlobalValues.UserPath + @"\weasel.custom.yaml"))
            {
                File.Delete(GlobalValues.UserPath + @"\weasel.custom.yaml");
            }

            // 将 colors 文件下的主题数据写入到 custom 外观文件中去
            SaveWeaselCustom();

            string colorScheme = ColorsList[ColorIndex].description.color_name;
            ConfigHelper.WriteConfigByString("color_scheme", colorScheme);

            this.ShowMessage("应用成功，部署生效", DialogType.Success);
        }

        [RelayCommand]
        public void SetRandomColor()
        {
            if (RandomColor)
            {
                Random rd = new Random();
                int index = rd.Next(ColorsList.Count);
                var rdModel = ColorsList[index];

                ChangeColor(rdModel.style.color_scheme);
                ServiceHelper.Deployer();
            }

            ConfigHelper.WriteConfigByBool("random_color", RandomColor);
        }

        [RelayCommand]
        public void UpdateCurrentColor(object obj)
        {
            var tempColor = CurrentColor;
            CurrentColor = null;
            CurrentColor = tempColor;
        }

        [RelayCommand]
        public void UpdateTemplate(object obj)
        {
            CurrentColor.Style.inline_preedit = ColorTemplate.InLine.ToString();
            CurrentColor.Style.vertical_text = ColorTemplate.TextVertical.ToString();
            if (ColorTemplate.IsBanyueMode)
            {
                CurrentColor.Style.layout.margin_x = "0";
                CurrentColor.Style.layout.margin_y = "0";
            }
            else
            {
                double lay_hilite_padding = double.Parse(CurrentColor.Style.layout.hilite_padding);
                CurrentColor.Style.layout.margin_x = (lay_hilite_padding + 3).ToString();
                CurrentColor.Style.layout.margin_y = (lay_hilite_padding + 3).ToString();
            }
            CurrentColor.Style.horizontal = ColorTemplate.Horizontal.ToString();

            UpdateCurrentColor(null);

            ConfigHelper.WriteConfigByBool("is_template_all", ColorTemplate.IsTemplateAll);
            ConfigHelper.WriteConfigByBool("inline_preedit", ColorTemplate.InLine);
            ConfigHelper.WriteConfigByBool("vertical_text", ColorTemplate.TextVertical);
            ConfigHelper.WriteConfigByBool("horizontal", ColorTemplate.Horizontal);
            ConfigHelper.WriteConfigByBool("is_banyue_mode", ColorTemplate.IsBanyueMode);
        }

        [RelayCommand]
        public void ViewLoaded()
        {
            if (is_loaded) return;

            if (ColorIndex == -1)
                LoadCurrentColor();

            is_loaded = true;
        }

        private Brush BrushConvter(string colorTxt, string defaultColor = "0x000000", string colorFormat = "abgr")
        {
            Color targetColor = ColorConvter(colorTxt, defaultColor, colorFormat);
            SolidColorBrush targetBrush = new SolidColorBrush(targetColor);
            return targetBrush;
        }

        private void ChangeAutoColor(object recipient, string message)
        {
            if (RandomColor)
                SetRandomColor();
            if (AutoColor)
                SetAutoColor();
        }

        private void ChangeColorScheme(object recipient, string message)
        {
            LoadColorShemes();
            LoadCustomColor();
        }

        private Color ColorConvter(string colorTxt, string defaultColor = "0x000000", string colorFormat = "abgr")
        {
            try
            {
                if (string.IsNullOrEmpty(colorTxt))
                    colorTxt = defaultColor;

                string colorStr = "";
                Color targetColor = Colors.Black;
                string _color = colorTxt.Substring(2, colorTxt.Length - 2);
                if (_color.Length <= 6) _color = "FF" + _color;
                var _cArray = _color.ToArray();

                switch (colorFormat)
                {
                    case "argb":
                        colorStr = "#" + _cArray.ToString();
                        targetColor = (Color)ColorConverter.ConvertFromString(colorStr);
                        break;

                    case "rgba":
                        colorStr = "#" + $"{_cArray[6]}{_cArray[7]}{_cArray[0]}{_cArray[1]}{_cArray[2]}{_cArray[3]}{_cArray[4]}{_cArray[5]}";
                        targetColor = (Color)ColorConverter.ConvertFromString(colorStr);
                        break;

                    default:
                        // 默认是 abgr
                        colorStr = "#" + $"{_cArray[0]}{_cArray[1]}{_cArray[6]}{_cArray[7]}{_cArray[4]}{_cArray[5]}{_cArray[2]}{_cArray[3]}";
                        targetColor = (Color)ColorConverter.ConvertFromString(colorStr);
                        break;
                }

                return targetColor;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                return Colors.Black;
            }
        }

        private string ColorToStr(string color_str, string color_format = "argb", string result_format = "abgr")
        {
            string color_result = "";
            color_str = color_str.Contains("#") ? color_str.Substring(1, color_str.Length - 1) : color_str;
            var color_array = color_str.ToArray().Select(c => c.ToString()).ToList();

            switch (result_format)
            {
                case "argb":
                    break;

                case "agbr":
                    break;

                default:  // 默认是返回 abgr 格式
                    color_result = $"{color_array[0]}{color_array[1]}{color_array[6]}{color_array[7]}{color_array[4]}{color_array[5]}{color_array[2]}{color_array[3]}";
                    break;
            }
            color_result = $"0x{color_result}";

            return color_result;
        }

        private void LoadColorShemes()
        {
            if (!File.Exists(GlobalValues.UserPath + "\\" + GlobalValues.SchemaKey))
                return;

            if (string.IsNullOrEmpty(GlobalValues.UserPath)) return;
            weaselPath = @$"{GlobalValues.UserPath}\weasel.yaml";
            weaselCustomPath = @$"{GlobalValues.UserPath}\weasel.custom.yaml";
            //string colorThemesPath = @$"{GlobalValues.UserPath}\color_themes.yaml";
            string colorsDirectory = @$"{GlobalValues.UserPath}\colors";

            // 加载colors文件夹下的所有外观文件
            try
            {
                if (!Directory.Exists(colorsDirectory))
                    throw new NullReferenceException("can't find colors directory");

                DirectoryInfo dInfo = new DirectoryInfo(colorsDirectory);
                FileInfo[] files = dInfo.GetFiles();

                var _colorsList = new List<ColorsModel>();
                for (int i = 0; i < files.Length; i++)
                {
                    try
                    {
                        FileInfo file = files[i];
                        string colorTxt = File.ReadAllText(file.FullName);
                        ColorsModel cModel = YamlHelper.Deserizlize<ColorsModel>(colorTxt);
                        if (cModel.description.color_name == "default")
                        {
                            default_color = cModel;
                            continue;
                        }
                        _colorsList.Add(cModel);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error($"外观文件[{files[i].Name}]格式异常，未能正确加载\n" + ex.ToString());
                        continue;
                    }
                }

                ColorsList = _colorsList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                this.ShowMessage("加载皮肤文件出错！", DialogType.Error);
            }
        }

        private void LoadConfig()
        {
            // 加载rime外观跟随主题
            AutoColor = ConfigHelper.ReadConfigByBool("auto_color");

            // 加载rime外观随机更换
            RandomColor = ConfigHelper.ReadConfigByBool("random_color");

            // 加载是否是将模板应用于全部皮肤
            ColorTemplate.IsTemplateAll = ConfigHelper.ReadConfigByBool("is_template_all");
            LoadTemplate();
        }

        /// <summary>
        /// 加载当前使用中的皮肤
        /// </summary>
        private void LoadCurrentColor()
        {
            // 首先要从 custom 文件中去加载
            // 如果无法从 custom 文件中正确地加载，则从 weasel 文件中去加载
            // 并将加载到的皮肤信息写入新的 custom 文件中去

            weaselPath = @$"{GlobalValues.UserPath}\weasel.yaml";
            weaselCustomPath = @$"{GlobalValues.UserPath}\weasel.custom.yaml";

            // 加载 weasel 中的外观
            try
            {
                string weaselTxt = File.ReadAllText(weaselPath);
                WeaselDetails = YamlHelper.Deserizlize<WeaselModel>(weaselTxt);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                this.ShowMessage("无法加载当前皮肤文件信息，详情请查看日志", DialogType.Error);
                return;
            }

            // 加载 weasel.custom 中的外观
            try
            {
                if (!File.Exists(weaselCustomPath))
                    throw new Exception("can't find weasel.custom file");

                string weaselCustomTxt = File.ReadAllText(weaselCustomPath);
                WeaselCustomDetails = YamlHelper.Deserizlize<WeaselCustomModel>(weaselCustomTxt);
                if (WeaselCustomDetails == null)
                    throw new Exception("weasel custom details is null");

                ConfigHelper.WriteConfigByString("color_scheme", WeaselCustomDetails.patch.style.color_scheme);

                // 将从 custom 中加载到的信息同步到 current color 对象中
                ColorSchemeModel csModel = new ColorSchemeModel();
                csModel.Style = WeaselCustomDetails.patch.style;
                csModel.UsedColor = WeaselCustomDetails.patch.preset_color_schemes.Values.First();
            }
            catch (Exception ex)
            {
                WeaselCustomDetails = new WeaselCustomModel();
                WeaselCustomDetails.patch = new CustomPatch();
                WeaselCustomDetails.patch.style = WeaselDetails.style;
                WeaselCustomDetails.patch.preset_color_schemes = WeaselDetails.preset_color_schemes;

                // 将从 custom 中加载到的信息同步到 current color 对象中
                ColorSchemeModel csModel = new ColorSchemeModel();
                csModel.Style = WeaselCustomDetails.patch.style;
                csModel.UsedColor = WeaselCustomDetails.patch.preset_color_schemes.Values.First();
            }

            // 加载当前使用的主题
            string shemeName = WeaselCustomDetails.patch.style.color_scheme;
            ChangeColor(shemeName);
        }

        private void LoadCustomColor()
        {
            return;
            if (!File.Exists(GlobalValues.UserPath + "\\" + GlobalValues.SchemaKey))
                return;

            try
            {
                string shemeName = WeaselCustomDetails.patch.style.color_scheme;
                ColorSchemeModel _colorModel = new ColorSchemeModel();
                _colorModel.Style = WeaselCustomDetails.patch.style;
                _colorModel.UsedColor = WeaselCustomDetails.patch.preset_color_schemes[shemeName];
                CurrentColor = _colorModel;
                ColorIndex = ColorsList.Select(c => c.description.color_name).ToList().IndexOf(shemeName);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                this.ShowAskMessage("加载 weasel.custom 异常");
            }
        }

        private void LoadTemplate()
        {
            if (!ColorTemplate.IsTemplateAll)
                return;
            ColorTemplate.InLine = ConfigHelper.ReadConfigByBool("inline_preedit");
            ColorTemplate.TextVertical = ConfigHelper.ReadConfigByBool("vertical_text");
            ColorTemplate.Horizontal = ConfigHelper.ReadConfigByBool("horizontal");
            ColorTemplate.IsBanyueMode = ConfigHelper.ReadConfigByBool("is_banyue_mode");
        }

        private void SaveWeaselCustom()
        {
            try
            {
                WeaselCustomDetails = new WeaselCustomModel();
                WeaselCustomDetails.patch = new CustomPatch();
                WeaselCustomDetails.patch.preset_color_schemes = new Dictionary<string, ColorScheme>();
                WeaselCustomDetails.patch.style = CurrentColor.Style;
                string name = ColorsList[ColorIndex].description.color_name;
                WeaselCustomDetails.patch.preset_color_schemes.Add(name, CurrentColor.UsedColor);

                YamlHelper.WriteYaml(WeaselCustomDetails, weaselCustomPath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
        }

        private void SetColorFromTemp()
        {
            CurrentColor.Style.inline_preedit = ColorTemplate.InLine.ToString();
            CurrentColor.Style.vertical_text = ColorTemplate.TextVertical.ToString();
            if (ColorTemplate.IsBanyueMode)
            {
                CurrentColor.Style.layout.margin_x = "0";
                CurrentColor.Style.layout.margin_y = "0";
            }
            else
            {
                double lay_hilite_padding = double.Parse(CurrentColor.Style.layout.hilite_padding);
                CurrentColor.Style.layout.margin_x = (lay_hilite_padding + 3).ToString();
                CurrentColor.Style.layout.margin_y = (lay_hilite_padding + 3).ToString();
            }
            CurrentColor.Style.horizontal = ColorTemplate.Horizontal.ToString();
        }

        private void SetTempFromColor()
        {
            double lay_margin = double.Parse(CurrentColor.Style.layout.margin_x);
            double lay_hilite_padding = double.Parse(CurrentColor.Style.layout.hilite_padding);

            ColorTemplate.InLine = bool.Parse(CurrentColor.Style.inline_preedit);
            ColorTemplate.TextVertical = bool.Parse(CurrentColor.Style.vertical_text);
            ColorTemplate.IsBanyueMode = lay_margin <= lay_hilite_padding;
            ColorTemplate.Horizontal = bool.Parse(CurrentColor.Style.horizontal);
        }
    }
}