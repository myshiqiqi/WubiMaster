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

namespace WubiMaster.Controls
{

    public class ColorModel
    {
        public string Color { get; set; }
        public string Name { get; set; }
    }

    public class ColorRange
    {
        public Color Color1 { get; set; }
        public Color Color2 { get; set; }
    }

    public partial class ColorPickerControl : UserControl
    {
        public static readonly DependencyProperty CanvasColorProperty =
            DependencyProperty.Register("CanvasColor", typeof(Color), typeof(ColorPickerControl), new PropertyMetadata(Colors.Black));

        public static readonly DependencyProperty ColorValueProperty =
                    DependencyProperty.Register("ColorValue", typeof(double), typeof(ColorPickerControl), new PropertyMetadata(0.0));

        public static readonly DependencyProperty CurrentBrushProperty =
                    DependencyProperty.Register("CurrentBrush", typeof(Brush), typeof(ColorPickerControl), new PropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty CurrentColorStrProperty =
            DependencyProperty.Register("CurrentColorStr", typeof(string), typeof(ColorPickerControl), new PropertyMetadata("#000000"));

        public static readonly DependencyProperty CurrnetColorProperty =
                    DependencyProperty.Register("CurrnetColor", typeof(Color), typeof(ColorPickerControl), new PropertyMetadata(Colors.Black));

        public static readonly DependencyProperty DefaultColorsProperty =
            DependencyProperty.Register("DefaultColors", typeof(List<ColorModel>), typeof(ColorPickerControl), new PropertyMetadata(new List<ColorModel>()));

        public static readonly DependencyProperty OpcityValueProperty =
            DependencyProperty.Register("OpcityValue", typeof(double), typeof(ColorPickerControl), new PropertyMetadata(255.0));

        private bool is_tbox_foucsed;

        private readonly List<ColorRange> _colorRangeList = new List<ColorRange>()
        {
            new ColorRange
            {
                Color1 = Color.FromRgb(255, 0 ,0),
                Color2 = Color.FromRgb(255, 0, 255),
            },
            new ColorRange
            {
                Color1 = Color.FromRgb(255, 0 ,255),
                Color2 = Color.FromRgb(0, 0, 255),
            },
            new ColorRange
            {
                Color1 = Color.FromRgb(0, 0 ,255),
                Color2 = Color.FromRgb(0, 255, 255),
            },
            new ColorRange
            {
                Color1 = Color.FromRgb(0, 255 ,255),
                Color2 = Color.FromRgb(0, 255, 0),
            },
            new ColorRange
            {
                Color1 = Color.FromRgb(0, 255 ,0),
                Color2 = Color.FromRgb(255, 255, 0),
            },
            new ColorRange
            {
                Color1 = Color.FromRgb(255, 255 ,0),
                Color2 = Color.FromRgb(255, 0, 0),
            },
        };

        private bool is_mouse_down;

        public ColorPickerControl()
        {
            InitializeComponent();
            InitColors();
        }

        public Color CanvasColor
        {
            get { return (Color)GetValue(CanvasColorProperty); }
            set { SetValue(CanvasColorProperty, value); }
        }

        public double ColorValue
        {
            get { return (double)GetValue(ColorValueProperty); }
            set { SetValue(ColorValueProperty, value); }
        }

        public Brush CurrentBrush
        {
            get { return (Brush)GetValue(CurrentBrushProperty); }
            set { SetValue(CurrentBrushProperty, value); }
        }

        public string CurrentColorStr
        {
            get { return (string)GetValue(CurrentColorStrProperty); }
            set { SetValue(CurrentColorStrProperty, value); }
        }

        public Color CurrnetColor
        {
            get { return (Color)GetValue(CurrnetColorProperty); }
            set { SetValue(CurrnetColorProperty, value); }
        }

        public List<ColorModel> DefaultColors
        {
            get { return (List<ColorModel>)GetValue(DefaultColorsProperty); }
            set { SetValue(DefaultColorsProperty, value); }
        }

        public double OpcityValue
        {
            get { return (double)GetValue(OpcityValueProperty); }
            set { SetValue(OpcityValueProperty, value); }
        }

        private void ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            is_mouse_down = true;
            e.Handled = true;
        }

        private Color GetColorBySlider(double s_value)
        {
            Color new_color = Colors.Black;

            if (s_value <= 1)
            {
                var color_range = _colorRangeList[0];
                var color_b = (byte)(255.0 * s_value);
                new_color = Color.FromArgb(CurrnetColor.A, 255, 0, color_b);
            }
            else if (s_value > 1 && s_value <= 2)
            {
                var color_range = _colorRangeList[1];
                var color_b = (byte)(255 - (255.0 * (s_value - 1)));
                new_color = Color.FromArgb(CurrnetColor.A, color_b, 0, 255);
            }
            else if (s_value > 2 && s_value <= 3)
            {
                var color_range = _colorRangeList[2];
                var color_b = (byte)(255.0 * (s_value - 2));
                new_color = Color.FromArgb(CurrnetColor.A, 0, color_b, 255);
            }
            else if (s_value > 3 && s_value <= 4)
            {
                var color_range = _colorRangeList[3];
                var color_b = (byte)(255 - (255.0 * (s_value - 3)));
                new_color = Color.FromArgb(CurrnetColor.A, 0, 255, color_b);
            }
            else if (s_value > 4 && s_value <= 5)
            {
                var color_range = _colorRangeList[4];
                var color_b = (byte)(255.0 * (s_value - 4));
                new_color = Color.FromArgb(CurrnetColor.A, color_b, 255, 0);
            }
            else if (s_value > 5 && s_value <= 6)
            {
                var color_range = _colorRangeList[5];
                var color_b = (byte)(255 - (255.0 * (s_value - 5)));
                new_color = Color.FromArgb(CurrnetColor.A, 255, color_b, 0);
            }
            else
            {
                new_color = new_color;
            }

            return new_color;
        }

        // 获取移动时颜色值
        private Color GetMoveColor(Point m_point)
        {
            // 左右移动时
            var new_color = GetColorBySlider(ColorValue);
            var x_offtset_r = 255.0 - new_color.R;
            var x_offtset_g = 255.0 - new_color.G;
            var x_offtset_b = 255.0 - new_color.B;

            var max_widht = this.g.ActualWidth;
            var percent_x = m_point.X / max_widht;
            var x_new_r = 255.0 - x_offtset_r * percent_x;
            var x_new_g = 255.0 - x_offtset_g * percent_x;
            var x_new_b = 255.0 - x_offtset_b * percent_x;

            new_color = Color.FromArgb(new_color.A, (byte)x_new_r, (byte)x_new_g, (byte)x_new_b);

            // 上下移动时计算颜色深度
            var y_offtset_r = new_color.R;
            var y_offtset_g = new_color.G;
            var y_offtset_b = new_color.B;

            var max_height = this.g.ActualHeight;
            var percent_y = m_point.Y / max_height;
            var y_new_r = y_offtset_r - y_offtset_r * percent_y;
            var y_new_g = y_offtset_g - y_offtset_g * percent_y;
            var y_new_b = y_offtset_b - y_offtset_b * percent_y;

            new_color = Color.FromArgb(new_color.A, (byte)y_new_r, (byte)y_new_g, (byte)y_new_b);

            return new_color;
        }

        // 通过颜色计算 slider 位置
        private double GetValueByColor(Color color)
        {
            double new_value = ColorValue;

            IDictionary<string, byte> color_values = new Dictionary<string, byte>();
            color_values.Add("R", color.R);
            color_values.Add("G", color.G);
            color_values.Add("B", color.B);
            var max_name = color_values.MaxBy(d => d.Value).Key;
            var min_name = color_values.MinBy(d => d.Value).Key; ;

            if (max_name == "R" && min_name == "G")
            {
                // 通道0
                new_value = (color_values["B"] / 255.0);
            }
            else if (max_name == "B" && min_name == "G")
            {
                // 通道1
                new_value = 2 - (color_values["R"] / 255.0);
            }
            else if (max_name == "B" && min_name == "R")
            {
                // 通道2
                new_value = 2 + (color_values["G"] / 255.0);
            }
            else if (max_name == "G" && min_name == "R")
            {
                // 通道3
                new_value = 4 - (color_values["B"] / 255.0);
            }
            else if (max_name == "G" && min_name == "B")
            {
                // 通道4
                new_value = 4 + (color_values["R"] / 255.0);
            }
            else if (max_name == "R" && min_name == "B")
            {
                // 通道5
                new_value = 6 - (color_values["G"] / 255.0);
            }
            else
            {
                new_value = 0;
            }

            return new_value;
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            is_mouse_down = false;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (is_mouse_down && e.LeftButton == MouseButtonState.Pressed)
            {
                Grid g = sender as Grid;
                Point mouse_p = e.GetPosition(g);

                Canvas.SetLeft(ellipse, mouse_p.X - (ellipse.ActualWidth / 2));
                Canvas.SetTop(ellipse, mouse_p.Y - (ellipse.ActualHeight / 2));

                var new_color = GetMoveColor(mouse_p);
                Update(new_color, true);
            }
            else
            {
                is_mouse_down = false;
            }
        }

        private void InitColors()
        {
            DefaultColors = new List<ColorModel>();

            var colors = new List<string>();
            colors.Add("#f44336");
            colors.Add("#e91e63");
            colors.Add("#9c27b0");
            colors.Add("#673ab7");
            colors.Add("#3f51b5");
            colors.Add("#2196f3");
            colors.Add("#03a9f4");
            colors.Add("#00bcd4");
            colors.Add("#009688");
            colors.Add("#4caf50");
            colors.Add("#8bc34a");
            colors.Add("#cddc39");
            colors.Add("#ffeb3b");
            colors.Add("#ffc107");
            colors.Add("#ff9800");
            colors.Add("#ff5722");
            colors.Add("#795548");
            colors.Add("#9e9e9e");
            colors.Add("#607d8b");

            foreach (var c in colors)
            {
                DefaultColors.Add(new ColorModel() { Name = "", Color = c });
            }
        }

        // 回到颜色首位
        private void MoveColorFirst()
        {
            var x = this.g.ActualWidth - ellipse.Width;
            var y = 0;

            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            var new_color = (Color)ColorConverter.ConvertFromString(radio.Content.ToString());

            // 通过选中色逆向计算光谱位置
            ColorValue = GetValueByColor(new_color);

            // 计算颜色深度位置
            MoveColorFirst();

            // 更新颜色
            Update(new_color);
        }

        // 色谱颜色也生变化时
        private void slider_color_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (is_tbox_foucsed)
                return;

            var new_color = GetColorBySlider(e.NewValue);
            Update(new_color);

            Point p = new Point();
            p.X = Canvas.GetLeft(ellipse) + (ellipse.Width / 2);
            p.Y = Canvas.GetTop(ellipse) + (ellipse.Height / 2);
            var move_color = GetMoveColor(p);
            Update(move_color, true);
        }

        private void slider_opcity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (is_tbox_foucsed)
                return;
            var new_color_a = (byte)e.NewValue;
            var new_color = Color.FromArgb(new_color_a, CurrnetColor.R, CurrnetColor.G, CurrnetColor.B);

            Update(new_color, true);
        }

        // 颜色更新
        private void Update(Color new_color, bool is_deep = false, bool is_str = false)
        {
            CurrentBrush = new SolidColorBrush(new_color);
            CurrnetColor = new_color;
            if (!is_deep)
                CanvasColor = Color.FromRgb(new_color.R, new_color.G, new_color.B);
            if (!is_str)
                CurrentColorStr = new_color.ToString();
            OpcityValue = new_color.A;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!is_tbox_foucsed)
                return;

            try
            {
                TextBox text_box = sender as TextBox;
                var text = text_box.Text.Trim();
                if (!text.Contains("#"))
                {
                    return;
                }

                if (text.Length == 7 || text.Length == 9)
                {
                    var new_color = (Color)ColorConverter.ConvertFromString(text_box.Text.Trim());

                    // 通过选中色逆向计算光谱位置
                    ColorValue = GetValueByColor(new_color);

                    Update(new_color, is_str: true);
                }
            }
            catch (System.Exception ex)
            { }

        }

        private void color_picker_Loaded(object sender, RoutedEventArgs e)
        {
            MoveColorFirst();

            var new_color = GetColorBySlider(0);
            Update(new_color);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            is_tbox_foucsed = true;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            is_tbox_foucsed = false;
        }
    }
}
