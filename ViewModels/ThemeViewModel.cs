using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WubiMaster.Common;
using WubiMaster.Models;

namespace WubiMaster.ViewModels
{
    public partial class ThemeViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private ThemeConfigModel configModel;

        [ObservableProperty]
        private ColorCandidateModel candidateModel;  // 候选项模型类

        [ObservableProperty]
        private int colorIndex = -1;

        [ObservableProperty]
        private List<ColorsModel> colorsList;

        [ObservableProperty]
        private ColorTemplateModel colorTemplate;

        [ObservableProperty]
        private ColorSchemeModel currentSkin;

        private ColorsModel default_color;

        private DefaultCustomModel DefaultCustomDetails;
        private bool is_loaded = false;

        [ObservableProperty]
        private WeaselCustomModel weaselCustomDetails;

        private string weaselCustomPath = "";

        [ObservableProperty]
        private WeaselModel weaselDetails;

        private string weaselPath = "";

        public ThemeViewModel()
        {
            ConfigModel = new ThemeConfigModel();
            ColorsList = new List<ColorsModel>();
            ColorTemplate = new ColorTemplateModel();
            CandidateModel = new ColorCandidateModel();
            DefaultCustomDetails = new DefaultCustomModel();

            WeakReferenceMessenger.Default.Register<string, string>(this, "ChangeColorScheme", ChangeColorScheme);
            WeakReferenceMessenger.Default.Register<string, string>(this, "SmartSkinColor", SmartSkinColor);

            LoadColorShemes();
            LoadConfig();
        }

        /// <summary>
        /// 选择候选项样式
        /// </summary> C1andidateChange
        /// <param name="obj"></param>
        [RelayCommand]
        public void UpdateSkinCandidate(object obj)
        {
            try
            {
                // 首先需要将新值更新到model中
                CandidateModel.Change();

                // 候选序号样式
                string candidate_str = CandidateModel.LabelDict.Values.ToList()[CandidateModel.LabelIndex];
                DefaultCustomDetails.SetAttribute(DefaultCustomDetails.SelectLabels, candidate_str);
                CurrentSkin.OtherProperty.LabelStr = candidate_str;

                // 候选个数设定
                string candidate_count = CandidateModel.NumList[CandidateModel.NumIndex];
                DefaultCustomDetails.SetAttribute(DefaultCustomDetails.PageSize, candidate_count);

                // 候选序号后缀（标签符）
                string suffix_str = CandidateModel.LabelSuffixList[CandidateModel.LabelSuffixIndex];
                CurrentSkin.OtherProperty.LabelSuffix = suffix_str;
                suffix_str = suffix_str == "无" ? "" : suffix_str;
                suffix_str = suffix_str == "空格" ? " " : suffix_str;
                CurrentSkin.Style.label_format = "%s" + suffix_str;

                // mark 符
                //string mark_str = CandidateModel.MarkTextList[CandidateModel.MarkTextIndex];
                //CurrentSkin.OtherProperty.MarkText = mark_str;
                //CurrentSkin.Style.mark_text = mark_str;

                // 是否显示拆分提示
                CurrentSkin.OtherProperty.ShowSpelling = ConfigModel.ThemeShowSpell;

                DefaultCustomDetails.Write();
                UpdateCurrentSkin(null);

                CandidateModel.SaveConfig();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                this.ShowMessage("设置失败，请查看日志定位问题。");
            }
        }

        [RelayCommand]
        public void ChangeSkin(object obj)
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
                _colorModel.OtherProperty.ShowSpelling = ConfigModel.ThemeShowSpell;
                //if (!ConfigModel.ThemeUseShade)
                //    _colorModel.Style.layout.shadow_radius = "0";
                CurrentSkin = _colorModel;

                if (ColorTemplate.IsTemplateAll)
                    SetColorFromTemp();  // 当模板应用于单个文件时，将当前主题的样式同步到模板对象中
                else
                    SetTempFromColor();  // 当模板应用于全部文件时，从模板对象中修改当前主题模板样式

                UpdateCurrentSkin(null);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                this.ShowMessage("选中的样式在外观文件中不存在！", DialogType.Error);
            }
        }

        /// <summary>
        /// 删除主题
        /// </summary>
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

                string color_name = CurrentSkin.Style.color_scheme;
                ColorSchemeModel csModel = new ColorSchemeModel();
                csModel.Style = default_color.style;
                csModel.UsedColor = default_color.preset_color_schemes.FirstOrDefault().Value;
                CurrentSkin = csModel;

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

        /// <summary>
        /// 皮肤跟随主题
        /// </summary>
        [RelayCommand]
        public void ColorFromTheme()
        {
            // 开关互冲
            if (ConfigModel.Theme_FollowTheme)
                ConfigModel.Theme_RandomColor = false;

            App.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    LoadCurrentSkin();

                    if (ConfigModel.Theme_FollowTheme)
                    {
                        ChangeSkin(ColorsList[ColorIndex].description.color_name);

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

                        //CurrentSkin.UsedColor.color_format = "argb";
                        CurrentSkin.UsedColor.text_color = ColorToStr(text_color.ToString());
                        CurrentSkin.UsedColor.comment_text_color = ColorToStr(comment_text_color.ToString());
                        CurrentSkin.UsedColor.label_color = ColorToStr(label_color.ToString());
                        CurrentSkin.UsedColor.back_color = ColorToStr(back_color.ToString());
                        CurrentSkin.UsedColor.shadow_color = ColorToStr(shadow_color.ToString());
                        CurrentSkin.UsedColor.border_color = ColorToStr(border_color.ToString());
                        CurrentSkin.UsedColor.hilited_text_color = ColorToStr(hilited_text_color.ToString());
                        CurrentSkin.UsedColor.hilited_back_color = ColorToStr(hilited_back_color.ToString());
                        CurrentSkin.UsedColor.hilited_candidate_text_color = ColorToStr(hilited_candidate_text_color.ToString());
                        CurrentSkin.UsedColor.hilited_candidate_back_color = ColorToStr(hilited_candidate_back_color.ToString());
                        CurrentSkin.UsedColor.hilited_label_color = ColorToStr(hilited_label_color.ToString());
                        CurrentSkin.UsedColor.hilited_comment_text_color = ColorToStr(hilited_comment_text_color.ToString());
                        CurrentSkin.UsedColor.candidate_text_color = ColorToStr(candidate_text_color.ToString());
                        CurrentSkin.UsedColor.candidate_back_color = ColorToStr(candidate_back_color.ToString());

                        UpdateCurrentSkin(null);
                    }

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

        /// <summary>
        /// 随机配色
        /// </summary>
        [RelayCommand]
        public void RandomColor()
        {
            // 开关互冲
            if (ConfigModel.Theme_RandomColor)
                ConfigModel.Theme_FollowTheme = false;
            else
                return;

            List<ThemeModel> themeList = new List<ThemeModel>();
            var sourceThemes = new ResourceDictionary();
            sourceThemes.Source = new Uri("pack://application:,,,/WubiMaster;component/Themes/ThemeNames.xaml");
            foreach (string name in sourceThemes.Keys)
            {
                string path = sourceThemes[name].ToString();
                ThemeModel themeModel = new ThemeModel();
                themeModel.Name = name;
                themeModel.Value = path;
                themeList?.Add(themeModel);
            }
            themeList = themeList.OrderBy(t => t.Name).ToList();
            var usedTheme = themeList[new Random().Next(themeList.Count)];

            var theme_resource = new ResourceDictionary();
            theme_resource.Source = new Uri($"pack://application:,,,/WubiMaster;component/Themes/{usedTheme.Value}.xaml");
            var test = (SolidColorBrush)theme_resource["text-100"];

            App.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    LoadCurrentSkin();

                    if (ConfigModel.Theme_RandomColor)
                    {
                        //ChangeSkin(ColorsList[ColorIndex].description.color_name);

                        SolidColorBrush text_color = (SolidColorBrush)theme_resource["text-100"];
                        SolidColorBrush comment_text_color = (SolidColorBrush)theme_resource["text-200"];
                        SolidColorBrush label_color = (SolidColorBrush)theme_resource["text-200"];
                        SolidColorBrush back_color = (SolidColorBrush)theme_resource["bg-100"];
                        SolidColorBrush shadow_color = (SolidColorBrush)theme_resource["primary-300"];
                        SolidColorBrush border_color = (SolidColorBrush)theme_resource["primary-200"];
                        SolidColorBrush hilited_text_color = (SolidColorBrush)theme_resource["accent-200"];
                        SolidColorBrush hilited_back_color = (SolidColorBrush)theme_resource["accent-100"];
                        SolidColorBrush hilited_candidate_text_color = (SolidColorBrush)theme_resource["bg-100"];
                        SolidColorBrush hilited_candidate_back_color = (SolidColorBrush)theme_resource["primary-100"];
                        SolidColorBrush hilited_label_color = (SolidColorBrush)theme_resource["bg-200"];
                        SolidColorBrush hilited_comment_text_color = (SolidColorBrush)theme_resource["bg-200"];
                        SolidColorBrush candidate_text_color = (SolidColorBrush)theme_resource["text-100"];
                        SolidColorBrush candidate_back_color = (SolidColorBrush)theme_resource["bg-100"];

                        /*首选项颜色随机匹配
                         *结果一：
                         *    背景色：bg-100
                         *    边框色：随机主色或随机背景色
                         *    首选背景色：随机主色
                         *    首选文字色：bg-100
                         *    编码背景色：随机高亮色
                         *    编码文字色：另一个高亮色
                         *    其他文字色：默认
                         *    
                         *结果二：
                         *    背景色：bg-100
                         *    边框色：随机高亮或随机背景色
                         *    首选背景色：随机高亮色
                         *    首选文字色：另一个高亮色
                         *    编码背景色：随机主色
                         *    编码文字色：bg-100
                         *    其他文字色：默认
                         *结果三：
                         *    背景色：随机主色或随机高亮色
                         *    边框色：随机主色或随机高亮色
                         *    首选背景色：bg-100
                         *    首选文字色：txt-100
                         *    编码背景色：bg-300
                         *    编码文字色：随机主色或随机高亮色
                         *    其他文字色：bg-200
                         *结果四：
                         *    背景色：随机背景色
                         *    边框色：随机主色或随机高亮色
                         *    首选背景色：同背景色
                         *    首选文字色：随机主色
                         *    首选label: 高亮色1
                         *    道选注解：高亮色2
                         *    编码背景色：背景色
                         *    编码文字色：text-100
                         *    其他文字色：默认
                         */

                        //----首选项颜色随机匹配----------//
                        // 结果一：
                        //     背景色：bg-100
                        //     边框色：随机主色

                        //     首选背景色：主色随机，首选字体：背景色3，其他字体：字体色1，边框色：主色随机，编码背景：高亮色随机，编码字体色：纯白色
                        // 结果二：高亮色1作背景 + 高亮色2作文字色
                        // 结果三：以上任意一个颜色 + 纯白色字体
                        // 结果四：首选项没有颜色，字体是主色系
                        List<string> primary_colors = new List<string>() { "primary-100", "primary-200", "primary-300" };
                        List<string> accent_colors = new List<string>() { "accent-100", "accent-200" };
                        List<string> bg_colors = new List<string>() { "bg-100", "bg-200", "bg-300" };
                        var hilited_candidate_random = new Random();
                        int hilited_candidate_index =  hilited_candidate_random.Next(4);
                        if (hilited_candidate_index == 0)
                        {
                            // 背景色
                            back_color = (SolidColorBrush)theme_resource["bg-100"];
                            // 边框色
                            var bor_list = primary_colors.Concat(bg_colors).ToList();
                            border_color = (SolidColorBrush)theme_resource[bor_list[new Random().Next(bor_list.Count)]];
                            // 首选背景色
                            hilited_candidate_back_color = (SolidColorBrush)theme_resource[primary_colors[new Random().Next(primary_colors.Count)]];
                            // 首选文字色
                            hilited_candidate_text_color = (SolidColorBrush)theme_resource["bg-100"];
                            hilited_label_color = hilited_candidate_text_color;
                            hilited_comment_text_color = hilited_candidate_text_color;
                            // 编码区颜色
                            int accent_incex = new Random().Next(accent_colors.Count);
                            hilited_text_color = (SolidColorBrush)theme_resource[accent_colors[accent_incex]];
                            accent_colors.RemoveAt(accent_incex);
                            hilited_back_color = (SolidColorBrush)theme_resource[accent_colors[new Random().Next(accent_colors.Count)]];
                        }
                        else if (hilited_candidate_index == 1)
                        {
                            // 背景色
                            back_color = (SolidColorBrush)theme_resource["bg-100"];
                            // 边框色
                            var bor_list = accent_colors.Concat(bg_colors).ToList();
                            border_color = (SolidColorBrush)theme_resource[bor_list[new Random().Next(bor_list.Count)]];
                            // 首选背景色
                            int accent_index = new Random().Next(accent_colors.Count);
                            hilited_candidate_back_color = (SolidColorBrush)theme_resource[accent_colors[accent_index]];
                            // 首选文字色
                            accent_colors.RemoveAt(accent_index);
                            hilited_candidate_text_color = (SolidColorBrush)theme_resource[accent_colors[new Random().Next(accent_colors.Count)]];
                            hilited_label_color = hilited_candidate_text_color;
                            hilited_comment_text_color = hilited_candidate_text_color;
                            // 编码区颜色
                            hilited_text_color = (SolidColorBrush)theme_resource[primary_colors[new Random().Next(primary_colors.Count)]];
                            hilited_back_color = (SolidColorBrush)theme_resource[accent_colors[new Random().Next(accent_colors.Count)]];
                        }
                        else if (hilited_candidate_index == 2)
                        {
                            var bg_list = primary_colors.Concat(accent_colors).ToList();
                            // 背景色
                            back_color = (SolidColorBrush)theme_resource[bg_list[new Random().Next(bg_list.Count)]];
                            border_color = (SolidColorBrush)theme_resource[bg_list[new Random().Next(bg_list.Count)]];
                            // 首选背景色
                            hilited_candidate_back_color = (SolidColorBrush)theme_resource[bg_colors[new Random().Next(bg_colors.Count)]];
                            // 首选文字色
                            hilited_candidate_text_color = (SolidColorBrush)theme_resource["text-100"];
                            hilited_label_color = hilited_candidate_text_color;
                            hilited_comment_text_color = hilited_candidate_text_color;
                            // 编码区颜色
                            hilited_text_color = (SolidColorBrush)theme_resource["text-200"];
                            hilited_back_color = (SolidColorBrush)theme_resource["bg-300"];
                            // 其它
                            label_color = (SolidColorBrush)theme_resource["bg-200"];
                            candidate_text_color = (SolidColorBrush)theme_resource["bg-200"];
                            comment_text_color = (SolidColorBrush)theme_resource["bg-200"];
                        }
                        else if (hilited_candidate_index==3)
                        {
                            // 背景色
                            back_color = (SolidColorBrush)theme_resource["bg-100"];
                            // 边框色
                            var bor_list = primary_colors.Concat(bg_colors).ToList();
                            border_color = (SolidColorBrush)theme_resource[bor_list[new Random().Next(bor_list.Count)]];
                            // 首选背景色
                            hilited_candidate_back_color = back_color;
                            // 首选文字色
                            hilited_candidate_text_color = (SolidColorBrush)theme_resource["primary-100"];
                            int accent_index = new Random().Next(accent_colors.Count);
                            hilited_label_color = (SolidColorBrush)theme_resource[accent_colors[accent_index]];
                            accent_colors.RemoveAt(accent_index);
                            hilited_comment_text_color = (SolidColorBrush)theme_resource[accent_colors[new Random().Next(accent_colors.Count)]];
                            // 编码区颜色
                            hilited_text_color = (SolidColorBrush)theme_resource["text-100"];
                            hilited_back_color = back_color;
                        }
                        //else
                        //{
                        //    hilited_candidate_back_color = (SolidColorBrush)theme_resource["bg-100"];
                        //    hilited_candidate_text_color = (SolidColorBrush)theme_resource["primary-100"];
                        //}

                        //CurrentSkin.UsedColor.color_format = "argb";
                        CurrentSkin.UsedColor.text_color = ColorToStr(text_color.ToString());
                        CurrentSkin.UsedColor.comment_text_color = ColorToStr(comment_text_color.ToString());
                        CurrentSkin.UsedColor.label_color = ColorToStr(label_color.ToString());
                        CurrentSkin.UsedColor.back_color = ColorToStr(back_color.ToString());
                        CurrentSkin.UsedColor.shadow_color = ColorToStr(shadow_color.ToString());
                        CurrentSkin.UsedColor.border_color = ColorToStr(border_color.ToString());
                        CurrentSkin.UsedColor.hilited_text_color = ColorToStr(hilited_text_color.ToString());
                        CurrentSkin.UsedColor.hilited_back_color = ColorToStr(hilited_back_color.ToString());
                        CurrentSkin.UsedColor.hilited_candidate_text_color = ColorToStr(hilited_candidate_text_color.ToString());
                        CurrentSkin.UsedColor.hilited_candidate_back_color = ColorToStr(hilited_candidate_back_color.ToString());
                        CurrentSkin.UsedColor.hilited_label_color = ColorToStr(hilited_label_color.ToString());
                        CurrentSkin.UsedColor.hilited_comment_text_color = ColorToStr(hilited_comment_text_color.ToString());
                        CurrentSkin.UsedColor.candidate_text_color = ColorToStr(candidate_text_color.ToString());
                        CurrentSkin.UsedColor.candidate_back_color = ColorToStr(candidate_back_color.ToString());

                        UpdateCurrentSkin(null);
                    }

                    // 如果包含 custom 文件，先将其删除掉
                    if (File.Exists(GlobalValues.UserPath + @"\weasel.custom.yaml"))
                    {
                        File.Delete(GlobalValues.UserPath + @"\weasel.custom.yaml");
                    }

                    // 将 colors 文件下的主题数据写入到 custom 外观文件中去
                    SaveWeaselCustom();

                    string colorScheme = ColorsList[ColorIndex].description.color_name;
                    ConfigHelper.WriteConfigByString("color_scheme", colorScheme);
                    //ServiceHelper.Deployer();
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
            if (ConfigModel.Theme_RandomSkin)
            {
                Random rd = new Random();
                int index = rd.Next(ColorsList.Count);
                var rdModel = ColorsList[index];

                ChangeSkin(rdModel.style.color_scheme);
                ServiceHelper.Deployer();
            }
        }

        // 更新当前显示的皮肤
        public void UpdateCurrentSkin(object obj)
        {
            // 将config中的配置更新到当前皮肤中
            CurrentSkin.OtherProperty.IsUseShade = ConfigModel.ThemeUseShade;  // 是否使用阴影效果

            var tempColor = CurrentSkin;
            CurrentSkin = null;
            CurrentSkin = tempColor;

            ConfigModel.SaveConfig();
        }

        [RelayCommand]
        public void UpdateTemplate(object obj)
        {
            CurrentSkin.Style.inline_preedit = ColorTemplate.InLine.ToString();
            CurrentSkin.Style.vertical_text = ColorTemplate.TextVertical.ToString();
            if (ColorTemplate.IsBanyueMode)
            {
                CurrentSkin.Style.layout.margin_x = "0";
                CurrentSkin.Style.layout.margin_y = "0";
            }
            else
            {
                double lay_hilite_padding = double.Parse(CurrentSkin.Style.layout.hilite_padding);
                CurrentSkin.Style.layout.margin_x = (lay_hilite_padding + 3).ToString();
                CurrentSkin.Style.layout.margin_y = (lay_hilite_padding + 3).ToString();
            }
            CurrentSkin.Style.horizontal = ColorTemplate.Horizontal.ToString();

            UpdateCurrentSkin(null);

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
                LoadCurrentSkin();

            is_loaded = true;
        }

        // todo: 将变量color更换为skin
        // 更新皮肤配色
        [RelayCommand]
        public void UpdateSkinColor()
        {
            ConfigModel.SaveConfig();
            UpdateCurrentSkin(null);
        }

        private Brush BrushConvter(string colorTxt, string defaultColor = "0x000000", string colorFormat = "abgr")
        {
            Color targetColor = ColorConvter(colorTxt, defaultColor, colorFormat);
            SolidColorBrush targetBrush = new SolidColorBrush(targetColor);
            return targetBrush;
        }

        // 切换智能换肤
        private void SmartSkinColor(object recipient, string message)
        {
            if (ConfigModel.Theme_RandomSkin)
                SetRandomColor();
            if (ConfigModel.Theme_FollowTheme)
                ColorFromTheme();
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
            if (!Directory.Exists(GlobalValues.UserPath + "\\colors"))
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

            // 加载是否是将模板应用于全部皮肤
            ColorTemplate.IsTemplateAll = ConfigHelper.ReadConfigByBool("is_template_all");
            LoadTemplate();
        }

        /// <summary>
        /// 加载当前使用中的皮肤
        /// </summary>
        private void LoadCurrentSkin()
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
            ChangeSkin(shemeName);
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
                CurrentSkin = _colorModel;
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
                WeaselCustomDetails.patch.style = CurrentSkin.Style;
                string name = ColorsList[ColorIndex].description.color_name;
                WeaselCustomDetails.patch.preset_color_schemes.Add(name, CurrentSkin.UsedColor);

                YamlHelper.WriteYaml(WeaselCustomDetails, weaselCustomPath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
        }

        private void SetColorFromTemp()
        {
            CurrentSkin.Style.inline_preedit = ColorTemplate.InLine.ToString();
            CurrentSkin.Style.vertical_text = ColorTemplate.TextVertical.ToString();
            if (ColorTemplate.IsBanyueMode)
            {
                CurrentSkin.Style.layout.margin_x = "0";
                CurrentSkin.Style.layout.margin_y = "0";
            }
            else
            {
                double lay_hilite_padding = double.Parse(CurrentSkin.Style.layout.hilite_padding);
                CurrentSkin.Style.layout.margin_x = (lay_hilite_padding + 3).ToString();
                CurrentSkin.Style.layout.margin_y = (lay_hilite_padding + 3).ToString();
            }
            CurrentSkin.Style.horizontal = ColorTemplate.Horizontal.ToString();
        }

        private void SetTempFromColor()
        {
            double lay_margin = double.Parse(CurrentSkin.Style.layout.margin_x);
            double lay_hilite_padding = double.Parse(CurrentSkin.Style.layout.hilite_padding);

            ColorTemplate.InLine = bool.Parse(CurrentSkin.Style.inline_preedit);
            ColorTemplate.TextVertical = bool.Parse(CurrentSkin.Style.vertical_text);
            ColorTemplate.IsBanyueMode = lay_margin <= lay_hilite_padding;
            ColorTemplate.Horizontal = bool.Parse(CurrentSkin.Style.horizontal);
        }
    }
}