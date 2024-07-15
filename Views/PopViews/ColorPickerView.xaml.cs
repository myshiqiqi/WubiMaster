using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WubiMaster.Views.PopViews
{
    /// <summary>
    /// ColorPickerView.xaml 的交互逻辑
    /// </summary>
    public partial class ColorPickerView : Window
    {
        public static readonly DependencyProperty CurrentBrushProperty =
            DependencyProperty.Register("CurrentBrush", typeof(Brush), typeof(ColorPickerView));

        public static readonly DependencyProperty FirstColorProperty =
                    DependencyProperty.Register("FirstColor", typeof(string), typeof(ColorPickerView), new PropertyMetadata("#000000", OnFirstColorChanged));

        public ColorPickerView()
        {
            InitializeComponent();
        }

        public Brush CurrentBrush
        {
            get { return (Brush)GetValue(CurrentBrushProperty); }
            set { SetValue(CurrentBrushProperty, value); }
        }

        /// <summary>
        /// 用于初始化颜色值
        /// 特意使用了 string 类型，方便 color 类型和 brush 类型的转换
        /// </summary>
        public string FirstColor
        {
            get { return (string)GetValue(FirstColorProperty); }
            set { SetValue(FirstColorProperty, value); }
        }

        private static void OnFirstColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerView view = (ColorPickerView)d;
            var new_c_str = e.NewValue.ToString();
            var new_color = (Color)ColorConverter.ConvertFromString(new_c_str);

            view.CurrentBrush = new SolidColorBrush(new_color);
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}