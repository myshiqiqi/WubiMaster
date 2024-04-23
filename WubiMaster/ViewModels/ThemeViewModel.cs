﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
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
        private int colorIndex;

        [ObservableProperty]
        private ColorSchemeModel currentColor;

        [ObservableProperty]
        private WeaselCustomModel weaselCustomDetails;

        private string weaselCustomPath = "";

        [ObservableProperty]
        private ColorModel weaselDetails;

        private string weaselPath = "";

        public ThemeViewModel()
        {
            WeakReferenceMessenger.Default.Register<string, string>(this, "ChangeColorScheme", ChangeColorScheme);

            LoadRimeThemeDetails();
            LoadConfig();
        }

        [RelayCommand]
        public void ChangeTheme(object obj)
        {
            if (obj == null) return;

            try
            {
                ColorSchemeModel _colorModel = new ColorSchemeModel();
                _colorModel.Style = WeaselCustomDetails.patch.style;
                _colorModel.UsedColor = weaselDetails.preset_color_schemes[obj.ToString()];
                if (_colorModel.UsedColor == null) throw new NullReferenceException($"找不到皮肤对象: {obj.ToString()}");
                CurrentColor = _colorModel;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                //this.ShowMessage("无法查看该皮肤样式！");
            }
        }

        [RelayCommand]
        public void DeleteColor()
        {
        }

        [RelayCommand]
        public void SetColor()
        {
            if (WeaselCustomDetails == null)
            {
                this.ShowMessage("请先配置码表版本！", DialogType.Warring);
                return;
            }
            WriteWeaselCustonDetails();

            string colorScheme = WeaselDetails.preset_color_schemes.Keys.ToList()[ColorIndex];
            ConfigHelper.WriteConfigByString("color_scheme", colorScheme);

            this.ShowMessage("应用成功，部署生效", DialogType.Success);
        }

        private Brush BrushConvter(string colorTxt, string defaultColor = "0x000000", string colorFormat = "abgr")
        {
            Color targetColor = ColorConvter(colorTxt, defaultColor, colorFormat);
            SolidColorBrush targetBrush = new SolidColorBrush(targetColor);
            return targetBrush;
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

        private void ChangeColorScheme(object recipient, string message)
        {
            LoadRimeThemeDetails();
            string colorName = WeaselCustomDetails.patch.style.color_scheme;
            int index = WeaselDetails.preset_color_schemes.Keys.ToList().IndexOf(colorName);
            ColorIndex = index;
            ChangeTheme(colorName);
        }

        private void LoadConfig()
        {
            if (WeaselDetails != null)
            {
                if (WeaselCustomDetails == null)
                {
                    ColorIndex = 0;
                    string shemeName = WeaselDetails.preset_color_schemes.Keys.ToList()[ColorIndex];
                    ChangeTheme(shemeName);
                }
                else
                {
                    string shemeName = WeaselCustomDetails.patch.style.color_scheme;
                    ChangeTheme(shemeName);
                    ColorIndex = WeaselDetails.preset_color_schemes.Keys.ToList().IndexOf(shemeName); ;
                }
            }
        }

        private void LoadRimeThemeDetails()
        {
            if (string.IsNullOrEmpty(GlobalValues.UserPath)) return;
            weaselPath = @$"{GlobalValues.UserPath}\weasel.yaml";
            weaselCustomPath = @$"{GlobalValues.UserPath}\weasel.custom.yaml";

            try
            {
                string weaselTxt = File.ReadAllText(weaselPath);
                WeaselDetails = YamlHelper.Deserizlize<ColorModel>(weaselTxt);

                if (File.Exists(weaselCustomPath))
                {
                    string weaselCustomTxt = File.ReadAllText(weaselCustomPath);
                    WeaselCustomDetails = YamlHelper.Deserizlize<WeaselCustomModel>(weaselCustomTxt);
                    ConfigHelper.WriteConfigByString("color_scheme", WeaselCustomDetails.patch.style.color_scheme);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
        }

        private void WriteWeaselCustonDetails()
        {
            try
            {
                WeaselCustomDetails.patch.style.color_scheme = WeaselDetails.preset_color_schemes.Keys.ToList()[ColorIndex];
                YamlHelper.WriteYaml(WeaselCustomDetails, weaselCustomPath);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
        }
    }
}