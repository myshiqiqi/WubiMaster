using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WubiMaster.Common;
using WubiMaster.Models;
using WubiMaster.Views.PopViews;

namespace WubiMaster.ViewModels
{
    public partial class ThemeViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private ColorCandidateModel candidateModel;

        [ObservableProperty]
        private int colorIndex = -1;

        // 候选项模型类
        [ObservableProperty]
        private List<ColorsModel> colorsList;

        [ObservableProperty]
        private ThemeConfigModel configModel;

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
            //ColorTemplate = new ColorTemplateModel();
            CandidateModel = new ColorCandidateModel();
            DefaultCustomDetails = new DefaultCustomModel();

            WeakReferenceMessenger.Default.Register<string, string>(this, "ChangeColorScheme", ChangeColorScheme);
            WeakReferenceMessenger.Default.Register<string, string>(this, "SmartSkinColor", SmartSkinColor);

            LoadColorShemes();
            LoadConfig();
        }

        /// <summary>
        /// AI 配色
        /// </summary>
        [RelayCommand]
        public void AIColor()
        {
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
                    List<string> primary_colors = new List<string>() { "primary-100", "primary-200", "primary-300" };
                    List<string> accent_colors = new List<string>() { "accent-100", "accent-200" };
                    List<string> bg_colors = new List<string>() { "bg-100", "bg-200", "bg-300" };
                    var hilited_candidate_random = new Random();
                    int hilited_candidate_index = hilited_candidate_random.Next(5);
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
                        candidate_back_color = back_color;
                    }
                    else if (hilited_candidate_index == 3)
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
                    else if (hilited_candidate_index == 4)
                    {
                        // 背景色
                        back_color = Brushes.Transparent;
                        // 边框色
                        border_color = Brushes.Transparent;
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

                    CurrentSkin.UsedColor.text_color = ColorConverterHelper.ConverterToRime(text_color.ToString());
                    CurrentSkin.UsedColor.comment_text_color = ColorConverterHelper.ConverterToRime(comment_text_color.ToString());
                    CurrentSkin.UsedColor.label_color = ColorConverterHelper.ConverterToRime(label_color.ToString());
                    CurrentSkin.UsedColor.back_color = ColorConverterHelper.ConverterToRime(back_color.ToString());
                    CurrentSkin.UsedColor.shadow_color = ColorConverterHelper.ConverterToRime(shadow_color.ToString());
                    CurrentSkin.UsedColor.border_color = ColorConverterHelper.ConverterToRime(border_color.ToString());
                    CurrentSkin.UsedColor.hilited_text_color = ColorConverterHelper.ConverterToRime(hilited_text_color.ToString());
                    CurrentSkin.UsedColor.hilited_back_color = ColorConverterHelper.ConverterToRime(hilited_back_color.ToString());
                    CurrentSkin.UsedColor.hilited_candidate_text_color = ColorConverterHelper.ConverterToRime(hilited_candidate_text_color.ToString());
                    CurrentSkin.UsedColor.hilited_candidate_back_color = ColorConverterHelper.ConverterToRime(hilited_candidate_back_color.ToString());
                    CurrentSkin.UsedColor.hilited_label_color = ColorConverterHelper.ConverterToRime(hilited_label_color.ToString());
                    CurrentSkin.UsedColor.hilited_comment_text_color = ColorConverterHelper.ConverterToRime(hilited_comment_text_color.ToString());
                    CurrentSkin.UsedColor.candidate_text_color = ColorConverterHelper.ConverterToRime(candidate_text_color.ToString());
                    CurrentSkin.UsedColor.candidate_back_color = ColorConverterHelper.ConverterToRime(candidate_back_color.ToString());

                    UpdateCurrentSkin(null);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.ToString());
                    this.ShowMessage("设置主题过程发生错误，请查看日志", DialogType.Error);
                }
            });
        }

        /// <summary>
        /// 切换皮肤
        /// </summary>
        /// <param name="obj"></param>
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

                if (ConfigModel.IsTemplateAll)
                    SetColorFromTemp();  // 当模板应用于单个文件时，将当前主题的样式同步到模板对象中
                else
                    SetTempFromColor();  // 当模板应用于全部文件时，从模板对象中修改当前主题模板样式

                // 从配置是加载字体信息
                GetCadidateFont();
                GetCadidateFontSize();

                UpdateTemplate(null);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                this.ShowMessage("选中的样式在外观文件中不存在！", DialogType.Error);
            }
        }

        /// <summary>
        /// 皮肤跟随主题
        /// </summary>
        [RelayCommand]
        public void ColorFromTheme()
        {
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
                        CurrentSkin.UsedColor.text_color = ColorConverterHelper.ConverterToRime(text_color.ToString());
                        CurrentSkin.UsedColor.comment_text_color = ColorConverterHelper.ConverterToRime(comment_text_color.ToString());
                        CurrentSkin.UsedColor.label_color = ColorConverterHelper.ConverterToRime(label_color.ToString());
                        CurrentSkin.UsedColor.back_color = ColorConverterHelper.ConverterToRime(back_color.ToString());
                        CurrentSkin.UsedColor.shadow_color = ColorConverterHelper.ConverterToRime(shadow_color.ToString());
                        CurrentSkin.UsedColor.border_color = ColorConverterHelper.ConverterToRime(border_color.ToString());
                        CurrentSkin.UsedColor.hilited_text_color = ColorConverterHelper.ConverterToRime(hilited_text_color.ToString());
                        CurrentSkin.UsedColor.hilited_back_color = ColorConverterHelper.ConverterToRime(hilited_back_color.ToString());
                        CurrentSkin.UsedColor.hilited_candidate_text_color = ColorConverterHelper.ConverterToRime(hilited_candidate_text_color.ToString());
                        CurrentSkin.UsedColor.hilited_candidate_back_color = ColorConverterHelper.ConverterToRime(hilited_candidate_back_color.ToString());
                        CurrentSkin.UsedColor.hilited_label_color = ColorConverterHelper.ConverterToRime(hilited_label_color.ToString());
                        CurrentSkin.UsedColor.hilited_comment_text_color = ColorConverterHelper.ConverterToRime(hilited_comment_text_color.ToString());
                        CurrentSkin.UsedColor.candidate_text_color = ColorConverterHelper.ConverterToRime(candidate_text_color.ToString());
                        CurrentSkin.UsedColor.candidate_back_color = ColorConverterHelper.ConverterToRime(candidate_back_color.ToString());

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
        /// 新建皮肤
        /// </summary>
        [RelayCommand]
        public void CreateNewSkin(object obj)
        {
            try
            {
                var new_skin_count = ColorsList.Where(c => c.description.color_name.Contains("Skin-")).Count() + 1;
                var new_skin_name = "Skin-" + new_skin_count;

                ColorSchemeModel csModel = new ColorSchemeModel();
                csModel.Style = default_color.style;
                csModel.UsedColor = default_color.preset_color_schemes.FirstOrDefault().Value;
                CurrentSkin = csModel;
                CurrentSkin.Style.color_scheme = new_skin_name;

                SaveCurrentSkin(new_skin_name);
                SaveWeaselCustom();
                ReLoadColorShemes();

                UpdateSkinCandidate(null);
            }
            catch (Exception ex)
            {
                this.ShowMessage("创建新皮肤失败，详情请查看日志", DialogType.Error);
                LogHelper.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 删除皮肤
        /// </summary>
        [RelayCommand]
        public void DeleteSkin()
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

                this.ShowMessage("主题删除成功！", DialogType.Success);

                // 删除完成后, 默认选中第一个皮肤外观
                LoadColorShemes();
                ColorIndex = 0;
                SaveWeaselCustom();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                this.ShowMessage("主题删除失败！", DialogType.Fail);
            }
        }

        /// <summary>
        /// 导出皮肤
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        public void ExportSkin(object obj)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "导出";
                sfd.FileName = "weasel.custom.yaml";
                sfd.DefaultExt = ".yaml";
                sfd.Filter = "Text documents|*.yaml";
                sfd.InitialDirectory = "";

                if (sfd.ShowDialog() == true)
                {
                    var targetPath = sfd.FileName;
                    SaveWeaselCustom(targetPath);
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage("保存失败，详细信息请查看日志", DialogType.Error);
                LogHelper.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 保存皮肤到 colors 目录下
        /// </summary>
        [RelayCommand]
        public void SaveSkin(object obj)
        {
            try
            {
                var skin_name = CurrentSkin.UsedColor.name;
                SaveCurrentSkin(skin_name);
                ReLoadColorShemes();

                this.ShowMessage($"皮肤【{skin_name}】保存成功", DialogType.Success);
            }
            catch (Exception ex)
            {
                this.ShowMessage("保存失败，详情请查看日志", DialogType.Error);
                LogHelper.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 将当前皮肤样式保存到weasel.custom.yaml中
        /// </summary>
        [RelayCommand]
        public void SetColor()
        {
            // 将 colors 文件下的主题数据写入到 custom 外观文件中去
            SaveWeaselCustom();

            string colorScheme = ColorsList[ColorIndex].description.color_name;
            ConfigHelper.WriteConfigByString("color_scheme", colorScheme);
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

        [RelayCommand]
        public void SetSkinColor(object obj)
        {
            // 建立一个颜色转换的临时函数
            var get_brush = (string colorstr) =>
            {
                var color_str = ColorConverterHelper.ConverterFromRime(colorstr);
                var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color_str));
                return brush;
            };

            // 根据参数类型识别颜色对象
            if (obj == null) return;
            string color_name = obj.ToString();
            Brush brush = null;
            switch (color_name)
            {
                case "back_color":
                    brush = get_brush(CurrentSkin.UsedColor.back_color);
                    break;

                case "border_color":
                    brush = get_brush(CurrentSkin.UsedColor.border_color);
                    break;

                case "hilited_candidate_back_color":
                    brush = get_brush(CurrentSkin.UsedColor.hilited_candidate_back_color);
                    break;

                case "hilited_candidate_border_color":
                    brush = get_brush(CurrentSkin.UsedColor.hilited_candidate_border_color);
                    break;

                case "hilited_candidate_text_color":
                    brush = get_brush(CurrentSkin.UsedColor.hilited_candidate_text_color);
                    break;

                case "hilited_label_color":
                    brush = get_brush(CurrentSkin.UsedColor.hilited_label_color);
                    break;

                case "hilited_comment_text_color":
                    brush = get_brush(CurrentSkin.UsedColor.hilited_comment_text_color);
                    break;

                case "candidate_back_color":
                    brush = get_brush(CurrentSkin.UsedColor.candidate_back_color);
                    break;

                case "candidate_text_color":
                    brush = get_brush(CurrentSkin.UsedColor.candidate_text_color);
                    break;

                case "label_color":
                    brush = get_brush(CurrentSkin.UsedColor.label_color);
                    break;

                case "comment_text_color":
                    brush = get_brush(CurrentSkin.UsedColor.comment_text_color);
                    break;

                case "hilited_back_color":
                    brush = get_brush(CurrentSkin.UsedColor.hilited_back_color);
                    break;

                case "text_color":
                    brush = get_brush(CurrentSkin.UsedColor.text_color);
                    break;

                default:
                    break;
            }

            // 打开窗口
            ColorPickerView cpv = new ColorPickerView();
            cpv.FirstColor = brush.ToString();
            cpv.ShowPop();

            // 颜色赋值，并通知更新
            switch (color_name)
            {
                case "back_color":
                    CurrentSkin.UsedColor.back_color = ColorConverterHelper.ConverterToRime(cpv.CurrentBrush.ToString());
                    break;

                case "border_color":
                    CurrentSkin.UsedColor.border_color = ColorConverterHelper.ConverterToRime(cpv.CurrentBrush.ToString());
                    break;

                case "hilited_candidate_back_color":
                    CurrentSkin.UsedColor.hilited_candidate_back_color = ColorConverterHelper.ConverterToRime(cpv.CurrentBrush.ToString());
                    break;

                case "hilited_candidate_border_color":
                    CurrentSkin.UsedColor.hilited_candidate_border_color = ColorConverterHelper.ConverterToRime(cpv.CurrentBrush.ToString());
                    break;

                case "hilited_candidate_text_color":
                    CurrentSkin.UsedColor.hilited_candidate_text_color = ColorConverterHelper.ConverterToRime(cpv.CurrentBrush.ToString());
                    break;

                case "hilited_label_color":
                    CurrentSkin.UsedColor.hilited_label_color = ColorConverterHelper.ConverterToRime(cpv.CurrentBrush.ToString());
                    break;

                case "hilited_comment_text_color":
                    CurrentSkin.UsedColor.hilited_comment_text_color = ColorConverterHelper.ConverterToRime(cpv.CurrentBrush.ToString());
                    break;

                case "candidate_back_color":
                    CurrentSkin.UsedColor.candidate_back_color = ColorConverterHelper.ConverterToRime(cpv.CurrentBrush.ToString());
                    break;

                case "candidate_text_color":
                    CurrentSkin.UsedColor.candidate_text_color = ColorConverterHelper.ConverterToRime(cpv.CurrentBrush.ToString());
                    break;

                case "label_color":
                    CurrentSkin.UsedColor.label_color = ColorConverterHelper.ConverterToRime(cpv.CurrentBrush.ToString());
                    break;

                case "comment_text_color":
                    CurrentSkin.UsedColor.comment_text_color = ColorConverterHelper.ConverterToRime(cpv.CurrentBrush.ToString());
                    break;

                case "hilited_back_color":
                    CurrentSkin.UsedColor.hilited_back_color = ColorConverterHelper.ConverterToRime(cpv.CurrentBrush.ToString());
                    break;

                case "text_color":
                    CurrentSkin.UsedColor.text_color = ColorConverterHelper.ConverterToRime(cpv.CurrentBrush.ToString());
                    break;

                default:
                    break;
            }
            UpdateCurrentSkin(null);
        }

        /// <summary>
        /// 皮肤重命名
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        public void SkinRename(object obj)
        {
            try
            {
                if (obj == null) return;
                var new_skin_name = obj.ToString();
                if (new_skin_name.Length <= 0)
                {
                    this.ShowMessage("请指定一个有效的名称！", DialogType.Warring);
                    return;
                }
                var all_names = ColorsList.Select(c => c.description.display_name).ToList();
                if (all_names.Contains(new_skin_name))
                {
                    this.ShowMessage("皮肤名称不可重复！", DialogType.Warring);
                    return;
                }

                SaveCurrentSkin(new_skin_name);
                SaveWeaselCustom();
                ReLoadColorShemes();

                UpdateSkinCandidate(null);

                this.ShowMessage("重命名成功", DialogType.Success);
            }
            catch (Exception ex)
            {
                this.ShowMessage("重命名失败，详情请查看日志", DialogType.Error);
                LogHelper.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 更新当前皮肤信息
        /// </summary>
        /// <param name="obj"></param>
        public void UpdateCurrentSkin(object obj)
        {
            // 将config中的配置更新到当前皮肤中
            CurrentSkin.OtherProperty.IsUseShade = ConfigModel.ThemeUseShade;  // 是否使用阴影效果

            var tempColor = CurrentSkin;
            CurrentSkin = null;
            CurrentSkin = tempColor;

            ConfigModel.SaveConfig();

            SetColor();
        }

        /// <summary>
        /// 设置或更改候选项字型
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        public void UpdateFont(object obj)
        {
            GetCadidateFont();
            ConfigModel.SaveConfig();
            UpdateCurrentSkin(null);
        }

        /// <summary>
        /// 设置或更改候选项字号
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        public void UpdateFontSize(object obj)
        {
            GetCadidateFontSize();

            ConfigModel.SaveConfig();
            UpdateCurrentSkin(null);
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

        // todo: 将变量color更换为skin
        // 更新皮肤配色
        [RelayCommand]
        public void UpdateSkinColor()
        {
            ConfigModel.SaveConfig();
            UpdateCurrentSkin(null);
        }

        /// <summary>
        /// 更新模板
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        public void UpdateTemplate(object obj)
        {
            try
            {
                CurrentSkin.Style.inline_preedit = ConfigModel.InLine.ToString();
                CurrentSkin.Style.vertical_text = ConfigModel.TextVertical.ToString();
                if (ConfigModel.IsBanyueMode)
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
                CurrentSkin.Style.horizontal = ConfigModel.Horizontal.ToString();
                CurrentSkin.Style.vertical_text_left_to_right = ConfigModel.IsTextLeftToRight.ToString();
                if (ConfigModel.IsOrthogonal)
                {
                    CurrentSkin.Style.layout.corner_radius = "0";
                    CurrentSkin.Style.layout.round_corner = "0";
                }
                else
                {
                    CurrentSkin.Style.layout.corner_radius = "10";
                    CurrentSkin.Style.layout.round_corner = "10";
                }
                if (ConfigModel.IsShowBorder)
                {
                    CurrentSkin.Style.layout.border_width = "2";
                }
                else
                {
                    CurrentSkin.Style.layout.border_width = "0";
                }

                UpdateCurrentSkin(null);
                ConfigModel.SaveConfig();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
        }

        [RelayCommand]
        public void ViewLoaded()
        {
            if (is_loaded) return;

            if (ColorIndex == -1)
                LoadCurrentSkin();

            is_loaded = true;
        }

        private Brush BrushConvter(string colorTxt, string defaultColor = "0x000000", string colorFormat = "abgr")
        {
            Color targetColor = ColorConvter(colorTxt, defaultColor, colorFormat);
            SolidColorBrush targetBrush = new SolidColorBrush(targetColor);
            return targetBrush;
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

        // 从配置获取字体信息
        private void GetCadidateFont()
        {
            string font_type = "";
            if (ConfigModel.ThemeTextWeight == "加粗")
                font_type = ":bold";
            else if (ConfigModel.ThemeTextWeight == "倾斜")
                font_type = ":italic";
            else if (ConfigModel.ThemeTextWeight == "加粗 倾斜")
                font_type = ":bold:italic";
            else
                font_type = "";
            CurrentSkin.Style.font_face = ConfigModel.ThemeTextFont + font_type;

            string label_type = "";
            if (ConfigModel.ThemeLabelWeight == "加粗")
                label_type = ":bold";
            else if (ConfigModel.ThemeLabelWeight == "倾斜")
                label_type = ":italic";
            else if (ConfigModel.ThemeLabelWeight == "加粗 倾斜")
                label_type = ":bold:italic";
            else
                label_type = "";
            CurrentSkin.Style.label_font_face = ConfigModel.ThemeLabelFont + label_type;

            string comment_type = "";
            if (ConfigModel.ThemeCommentWeight == "加粗")
                comment_type = ":bold";
            else if (ConfigModel.ThemeCommentWeight == "倾斜")
                comment_type = ":italic";
            else if (ConfigModel.ThemeCommentWeight == "加粗 倾斜")
                comment_type = ":bold:italic";
            else
                comment_type = "";
            CurrentSkin.Style.comment_font_face = ConfigModel.ThemeCommentFont + comment_type;
        }

        // 从配置里获取字号信息
        private void GetCadidateFontSize()
        {
            double font_size = 14;
            if (ConfigModel.ThemeTextSize == "小号")
                font_size = 12;
            else if (ConfigModel.ThemeTextSize == "中号")
                font_size = 15;
            else
                font_size = 18;
            CurrentSkin.Style.font_point = font_size.ToString();

            double label_size = 14;
            if (ConfigModel.ThemeLabelSize == "小号")
                label_size = 12;
            else if (ConfigModel.ThemeLabelSize == "中号")
                label_size = 15;
            else
                label_size = 18;
            CurrentSkin.Style.label_font_point = label_size.ToString();

            double comment_size = 14;
            if (ConfigModel.ThemeCommentSize == "小号")
                comment_size = 12;
            else if (ConfigModel.ThemeCommentSize == "中号")
                comment_size = 15;
            else
                comment_size = 18;
            CurrentSkin.Style.comment_font_point = comment_size.ToString();
        }

        // 从 colors 文件夹下加载皮肤集
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
            ConfigModel.IsTemplateAll = ConfigHelper.ReadConfigByBool("is_template_all");
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
            if (!ConfigModel.IsTemplateAll)
                return;
            ConfigModel.InLine = ConfigHelper.ReadConfigByBool("inline_preedit");
            ConfigModel.TextVertical = ConfigHelper.ReadConfigByBool("vertical_text");
            ConfigModel.Horizontal = ConfigHelper.ReadConfigByBool("horizontal");
            ConfigModel.IsBanyueMode = ConfigHelper.ReadConfigByBool("is_banyue_mode");
        }

        // 重新加载皮肤集
        private void ReLoadColorShemes()
        {
            LoadColorShemes();
            ColorIndex = ColorsList.Select(c => c.description.color_name).ToList().IndexOf(CurrentSkin.Style.color_scheme);
        }

        /// <summary>
        /// 将当前皮肤只在到color目录下
        /// </summary>
        /// <param name="skinName">名称</param>
        private void SaveCurrentSkin(string skinName = "")
        {
            skinName = string.IsNullOrEmpty(skinName) ? CurrentSkin.Style.color_scheme : skinName;

            CurrentSkin.Style.color_scheme_dark = "";
            CurrentSkin.UsedColor.author = "中书君";
            CurrentSkin.UsedColor.name = skinName;

            ColorsModel new_skin_model = new ColorsModel();
            new_skin_model.description.color_name = CurrentSkin.Style.color_scheme;
            new_skin_model.description.display_name = skinName;
            new_skin_model.description.create_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            new_skin_model.description.update_time = new_skin_model.description.create_time;

            new_skin_model.style = CurrentSkin.Style;
            new_skin_model.preset_color_schemes.Add(CurrentSkin.Style.color_scheme, CurrentSkin.UsedColor);

            var save_path = GlobalValues.UserPath + $"\\colors\\{CurrentSkin.Style.color_scheme}.yaml";
            YamlHelper.WriteYaml(new_skin_model, save_path);
        }

        /// <summary>
        /// 向 weasel.custom.yaml 写入数据
        /// </summary>
        private void SaveWeaselCustom(string targetPath = "")
        {
            try
            {
                targetPath = string.IsNullOrEmpty(targetPath) ? weaselCustomPath : targetPath;
                if (File.Exists(targetPath))
                    File.Delete(targetPath);

                WeaselCustomDetails = new WeaselCustomModel();
                WeaselCustomDetails.patch = new CustomPatch();
                WeaselCustomDetails.patch.preset_color_schemes = new Dictionary<string, ColorScheme>();
                WeaselCustomDetails.patch.style = CurrentSkin.Style;
                string name = ColorsList[ColorIndex].description.color_name;
                WeaselCustomDetails.patch.preset_color_schemes.Add(name, CurrentSkin.UsedColor);

                YamlHelper.WriteYaml(WeaselCustomDetails, targetPath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
        }

        private void SetColorFromTemp()
        {
            CurrentSkin.Style.inline_preedit = ConfigModel.InLine.ToString();
            CurrentSkin.Style.vertical_text = ConfigModel.TextVertical.ToString();
            if (ConfigModel.IsBanyueMode)
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
            CurrentSkin.Style.horizontal = ConfigModel.Horizontal.ToString();
        }

        private void SetTempFromColor()
        {
            double lay_margin = double.Parse(CurrentSkin.Style.layout.margin_x);
            double lay_hilite_padding = double.Parse(CurrentSkin.Style.layout.hilite_padding);

            ConfigModel.InLine = bool.Parse(CurrentSkin.Style.inline_preedit);
            ConfigModel.TextVertical = bool.Parse(CurrentSkin.Style.vertical_text);
            ConfigModel.IsBanyueMode = lay_margin <= lay_hilite_padding;
            ConfigModel.Horizontal = bool.Parse(CurrentSkin.Style.horizontal);
        }

        // 切换智能换肤
        private void SmartSkinColor(object recipient, string message)
        {
            if (ConfigModel.Theme_RandomSkin)
                SetRandomColor();
            if (ConfigModel.Theme_FollowTheme)
                ColorFromTheme();
        }
    }
}