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
        private int colorIndex;

        [ObservableProperty]
        private List<ColorsModel> colorsList;

        [ObservableProperty]
        private ColorSchemeModel currentColor;

        private ColorsModel default_color;

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
            WeakReferenceMessenger.Default.Register<string, string>(this, "ChangeColorScheme", ChangeColorScheme);
            WeakReferenceMessenger.Default.Register<string, string>(this, "ChangeAutoColor", ChangeAutoColor);

            LoadColorShemes();
            LoadConfig();
        }

        [RelayCommand]
        public void ChangeHorizontal(object obj)
        {
            var tempColor = CurrentColor;
            CurrentColor = null;
            CurrentColor = tempColor;
        }

        [RelayCommand]
        public void ChangeTheme(object obj)
        {
            if (obj == null) return;

            try
            {
                var cModel = ColorsList.First(c => c.description.color_name == obj.ToString());
                if (cModel == null) throw new NullReferenceException($"找不到皮肤对象: {obj.ToString()}");

                ColorIndex = ColorsList.IndexOf(cModel);

                ColorSchemeModel _colorModel = new ColorSchemeModel();
                _colorModel.Style = cModel.style;
                _colorModel.UsedColor = cModel.preset_color_schemes.FirstOrDefault().Value;

                CurrentColor = _colorModel;
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
            try
            {
                if (CurrentColor == null)
                    LoadCurrentColor();

                if (AutoColor)
                {
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

                    string shemeName = ColorsList[ColorIndex].description.color_name;
                    ChangeTheme(shemeName);
                    CurrentColor = CurrentColor;
                }
                else
                {
                    LoadColorShemes();
                    LoadCurrentColor();
                }

                ConfigHelper.WriteConfigByBool("auto_color", AutoColor);
                // 如果包含 custom 文件，先将其删除掉
                if (File.Exists(GlobalValues.UserPath + @"\weasel.custom.yaml"))
                {
                    File.Delete(GlobalValues.UserPath + @"\weasel.custom.yaml");
                }

                // 将 colors 文件下的主题数据写入到 custom 外观文件中去
                WriteWeaselCustonDetails();

                string colorScheme = ColorsList[ColorIndex].description.color_name;
                ConfigHelper.WriteConfigByString("color_scheme", colorScheme);
                ServiceHelper.Deployer();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                this.ShowMessage("设置主题过程发生错误，请查看日志", DialogType.Error);
            }
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
            WriteWeaselCustonDetails();

            string colorScheme = ColorsList[ColorIndex].description.color_name;
            ConfigHelper.WriteConfigByString("color_scheme", colorScheme);

            this.ShowMessage("应用成功，部署生效", DialogType.Success);
        }

        [RelayCommand]
        public void SetRandomColor()
        {
            ConfigHelper.WriteConfigByBool("random_color", RandomColor);
        }

        [RelayCommand]
        public void ViewLoaded()
        {
            LoadCurrentColor();
        }

        private Brush BrushConvter(string colorTxt, string defaultColor = "0x000000", string colorFormat = "abgr")
        {
            Color targetColor = ColorConvter(colorTxt, defaultColor, colorFormat);
            SolidColorBrush targetBrush = new SolidColorBrush(targetColor);
            return targetBrush;
        }

        private void ChangeAutoColor(object recipient, string message)
        {
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

                ColorsList = new List<ColorsModel>();
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
                        ColorsList.Add(cModel);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error($"外观文件[{files[i].Name}]格式异常，未能正确加载\n" + ex.ToString());
                        continue;
                    }
                }
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
            ChangeTheme(shemeName);
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

        private void WriteWeaselCustonDetails()
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
    }
}