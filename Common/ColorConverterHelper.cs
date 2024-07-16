using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WubiMaster.Common
{
    public class ColorConverterHelper
    {
        public static string ConverterFromRime(string colorTxt, string defaultColor = "0x00000000", string colorFormat = "abgr")
        {
            var ConverColorText = (string colorTxt) =>
            {
                if (colorTxt.Contains("0x"))
                    colorTxt = colorTxt.Substring(2, colorTxt.Length - 2);
                else if (colorTxt.Contains("#"))
                    colorTxt = colorTxt.Substring(1, colorTxt.Length - 1);
                return colorTxt;
            };

            string colorStr = "";
            Color targetColor = Colors.Black;
            char[] _cArray = null;

            try
            {
                if (string.IsNullOrEmpty(colorTxt))
                    colorTxt = defaultColor;

                switch (colorFormat)
                {
                    case "argb":
                        colorTxt = ConverColorText(colorTxt);
                        if (colorTxt.Length <= 6) colorTxt = "FF" + colorTxt;
                        _cArray = colorTxt.ToArray();
                        colorStr = "#" + $"{_cArray[0]}{_cArray[1]}{_cArray[2]}{_cArray[3]}{_cArray[4]}{_cArray[5]}{_cArray[6]}{_cArray[7]}";
                        targetColor = (Color)ColorConverter.ConvertFromString(colorStr);
                        break;

                    case "rgba":
                        colorTxt = ConverColorText(colorTxt);
                        if (colorTxt.Length <= 6) colorTxt = colorTxt + "FF";
                        _cArray = colorTxt.ToArray();
                        colorStr = "#" + $"{_cArray[6]}{_cArray[7]}{_cArray[0]}{_cArray[1]}{_cArray[2]}{_cArray[3]}{_cArray[4]}{_cArray[5]}";
                        targetColor = (Color)ColorConverter.ConvertFromString(colorStr);
                        break;

                    default:
                        // 默认是 abgr
                        colorTxt = ConverColorText(colorTxt);
                        if (colorTxt.Length <= 6) colorTxt = "FF" + colorTxt;
                        _cArray = colorTxt.ToArray();
                        colorStr = "#" + $"{_cArray[0]}{_cArray[1]}{_cArray[6]}{_cArray[7]}{_cArray[4]}{_cArray[5]}{_cArray[2]}{_cArray[3]}";
                        targetColor = (Color)ColorConverter.ConvertFromString(colorStr);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }

            return targetColor.ToString();
        }

        public static string ConverterToRime(string color_str, string color_format = "argb", string result_format = "abgr")
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
    }
}
