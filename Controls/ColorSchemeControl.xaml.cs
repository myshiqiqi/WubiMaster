﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WubiMaster.Common;
using WubiMaster.Models;

namespace WubiMaster.Controls
{
    public partial class ColorSchemeControl : UserControl
    {
        public static readonly DependencyProperty AlignTypeProperty =
            DependencyProperty.Register("AlignType", typeof(string), typeof(ColorSchemeControl), new PropertyMetadata("center"));

        public static readonly DependencyProperty AntialiasModeProperty =
            DependencyProperty.Register("AntialiasMode", typeof(string), typeof(ColorSchemeControl), new PropertyMetadata("default"));

        public static readonly DependencyProperty AsciiTipFollowCursorProperty =
            DependencyProperty.Register("AsciiTipFollowCursor", typeof(bool), typeof(ColorSchemeControl));

        public static readonly DependencyProperty AuthorProperty =
                                    DependencyProperty.Register("Author", typeof(string), typeof(ColorSchemeControl));

        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register("BackColor", typeof(Brush), typeof(ColorSchemeControl), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register("BorderColor", typeof(Brush), typeof(ColorSchemeControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty BorderPaddingProperty =
            DependencyProperty.Register("BorderPadding", typeof(Thickness), typeof(ColorSchemeControl));

        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(double), typeof(ColorSchemeControl), new PropertyMetadata(2.0));

        public static readonly DependencyProperty CandidateAbbreviateLengthProperty =
            DependencyProperty.Register("CandidateAbbreviateLength", typeof(string), typeof(ColorSchemeControl));

        public static readonly DependencyProperty CandidateBackColorProperty =
                                    DependencyProperty.Register("CandidateBackColor", typeof(Brush), typeof(ColorSchemeControl));

        public static readonly DependencyProperty CandidateBorderColorProperty =
            DependencyProperty.Register("CandidateBorderColor", typeof(Brush), typeof(ColorSchemeControl));

        public static readonly DependencyProperty CandidateBorderCornerProperty =
            DependencyProperty.Register("CandidateBorderCorner", typeof(CornerRadius), typeof(ColorSchemeControl), new PropertyMetadata(new CornerRadius(0)));

        public static readonly DependencyProperty CandidateBorderMarginProperty =
            DependencyProperty.Register("CandidateBorderMargin", typeof(Thickness), typeof(ColorSchemeControl), new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty CandidateMarginProperty =
                            DependencyProperty.Register("CandidateMargin", typeof(Thickness), typeof(ColorSchemeControl));

        public static readonly DependencyProperty CandidateShadowColorProperty =
                            DependencyProperty.Register("CandidateShadowColor", typeof(Color), typeof(ColorSchemeControl));

        public static readonly DependencyProperty CandidateSpacingProperty =
            DependencyProperty.Register("CandidateSpacing", typeof(double), typeof(ColorSchemeControl));

        public static readonly DependencyProperty CandidateTextColorProperty =
                                    DependencyProperty.Register("CandidateTextColor", typeof(Brush), typeof(ColorSchemeControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty ClickToCaptureProperty =
            DependencyProperty.Register("ClickToCapture", typeof(bool), typeof(ColorSchemeControl));

        public static readonly DependencyProperty ColorFormatProperty =
                                            DependencyProperty.Register("ColorFormat", typeof(string), typeof(ColorSchemeControl));

        public static readonly DependencyProperty ColorMaxWidthProperty =
            DependencyProperty.Register("ColorMaxWidth", typeof(double), typeof(ColorSchemeControl));

        public static readonly DependencyProperty ColorMinHeightProperty =
            DependencyProperty.Register("ColorMinHeight", typeof(double), typeof(ColorSchemeControl));

        public static readonly DependencyProperty ColorMinWidthProperty =
            DependencyProperty.Register("ColorMinWidth", typeof(double), typeof(ColorSchemeControl));

        public static readonly DependencyProperty ColorModelProperty =
            DependencyProperty.Register("ColorModel", typeof(ColorSchemeModel), typeof(ColorSchemeControl), new PropertyMetadata(new PropertyChangedCallback(OnColorChanged)));

        public static readonly DependencyProperty ColorNameProperty =
                                                    DependencyProperty.Register("ColorName", typeof(string), typeof(ColorSchemeControl));

        public static readonly DependencyProperty ColorSchemeProperty =
            DependencyProperty.Register("ColorScheme", typeof(string), typeof(ColorSchemeControl));

        public static readonly DependencyProperty ColorThemeDarkProperty =
            DependencyProperty.Register("ColorThemeDark", typeof(string), typeof(ColorSchemeControl));

        public static readonly DependencyProperty CommentFontFaceProperty =
            DependencyProperty.Register("CommentFontFace", typeof(FontFamily), typeof(ColorSchemeControl), new PropertyMetadata(new FontFamily("黑体字根")));

        public static readonly DependencyProperty CommentFontPointProperty =
            DependencyProperty.Register("CommentFontPoint", typeof(double), typeof(ColorSchemeControl), new PropertyMetadata(12.0));

        public static readonly DependencyProperty CommentFontStyleProperty =
            DependencyProperty.Register("CommentFontStyle", typeof(FontStyle), typeof(ColorSchemeControl), new PropertyMetadata(FontStyles.Normal));

        public static readonly DependencyProperty CommentFontWeightProperty =
            DependencyProperty.Register("CommentFontWeight", typeof(FontWeight), typeof(ColorSchemeControl), new PropertyMetadata(FontWeights.Normal));

        public static readonly DependencyProperty CommentTextColorProperty =
                                                            DependencyProperty.Register("CommentTextColor", typeof(Brush), typeof(ColorSchemeControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty ContentMarginProperty =
            DependencyProperty.Register("ContentMargin", typeof(Thickness), typeof(ColorSchemeControl), new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty CornerRadiusProperty =
                    DependencyProperty.Register("CornerRadius", typeof(double), typeof(ColorSchemeControl), new PropertyMetadata(5.0));

        public static readonly DependencyProperty DisplayTrayIconProperty =
            DependencyProperty.Register("DisplayTrayIcon", typeof(bool), typeof(ColorSchemeControl));

        public static readonly DependencyProperty EnhancedPositionProperty =
            DependencyProperty.Register("EnhancedPosition", typeof(bool), typeof(ColorSchemeControl));

        public static readonly DependencyProperty FontFaceProperty =
            DependencyProperty.Register("FontFace", typeof(FontFamily), typeof(ColorSchemeControl), new PropertyMetadata(new FontFamily("Microsoft YaHei")));

        public static readonly DependencyProperty FontFaceStyleProperty =
            DependencyProperty.Register("FontFaceStyle", typeof(FontStyle), typeof(ColorSchemeControl), new PropertyMetadata(FontStyles.Normal));

        public static readonly DependencyProperty FontFaceWeightProperty =
                    DependencyProperty.Register("FontFaceWeight", typeof(FontWeight), typeof(ColorSchemeControl), new PropertyMetadata(FontWeights.Normal));

        public static readonly DependencyProperty FontPointProperty =
                    DependencyProperty.Register("FontPoint", typeof(double), typeof(ColorSchemeControl), new PropertyMetadata(12.0));

        public static readonly DependencyProperty FullScreenProperty =
            DependencyProperty.Register("FullScreen", typeof(bool), typeof(ColorSchemeControl));

        public static readonly DependencyProperty HiliteBorderCornerHProperty =
            DependencyProperty.Register("HiliteBorderCornerH", typeof(CornerRadius), typeof(ColorSchemeControl), new PropertyMetadata(new CornerRadius(0)));

        public static readonly DependencyProperty HiliteBorderCornerTVProperty =
            DependencyProperty.Register("HiliteBorderCornerTV", typeof(CornerRadius), typeof(ColorSchemeControl), new PropertyMetadata(new CornerRadius(0)));

        public static readonly DependencyProperty HiliteBorderCornerVProperty =
            DependencyProperty.Register("HiliteBorderCornerV", typeof(CornerRadius), typeof(ColorSchemeControl), new PropertyMetadata(new CornerRadius(10)));

        public static readonly DependencyProperty HiliteBorderMarginProperty =
            DependencyProperty.Register("HiliteBorderMargin", typeof(Thickness), typeof(ColorSchemeControl), new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty HilitedBackColorProperty =
                                                                                            DependencyProperty.Register("HilitedBackColor", typeof(Brush), typeof(ColorSchemeControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly DependencyProperty HilitedCandidateBackColorProperty =
            DependencyProperty.Register("HilitedCandidateBackColor", typeof(Brush), typeof(ColorSchemeControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty HilitedCandidateBorderColorProperty =
            DependencyProperty.Register("HilitedCandidateBorderColor", typeof(Brush), typeof(ColorSchemeControl));

        public static readonly DependencyProperty HilitedCandidateShadowColorProperty =
                    DependencyProperty.Register("HilitedCandidateShadowColor", typeof(Color), typeof(ColorSchemeControl));

        public static readonly DependencyProperty HilitedCandidateTextColorProperty =
                            DependencyProperty.Register("HilitedCandidateTextColor", typeof(Brush), typeof(ColorSchemeControl), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty HilitedCommentTextColorProperty =
            DependencyProperty.Register("HilitedCommentTextColor", typeof(Brush), typeof(ColorSchemeControl), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty HilitedLabelColorProperty =
                    DependencyProperty.Register("HilitedLabelColor", typeof(Brush), typeof(ColorSchemeControl), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty HilitedMarkColorProperty =
            DependencyProperty.Register("HilitedMarkColor", typeof(Brush), typeof(ColorSchemeControl));

        public static readonly DependencyProperty HilitedShadowColorProperty =
                                    DependencyProperty.Register("HilitedShadowColor", typeof(Color), typeof(ColorSchemeControl));

        public static readonly DependencyProperty HilitedTextColorProperty =
                            DependencyProperty.Register("HilitedTextColor", typeof(Brush), typeof(ColorSchemeControl), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty HilitePaddingProperty =
            DependencyProperty.Register("HilitePadding", typeof(double), typeof(ColorSchemeControl));

        public static readonly DependencyProperty HiliteSpacingProperty =
            DependencyProperty.Register("HiliteSpacing", typeof(double), typeof(ColorSchemeControl));

        public static readonly DependencyProperty HiSpacingMarginProperty =
            DependencyProperty.Register("HiSpacingMargin", typeof(Thickness), typeof(ColorSchemeControl));

        public static readonly DependencyProperty HorizontalProperty =
            DependencyProperty.Register("Horizontal", typeof(bool), typeof(ColorSchemeControl));

        public static readonly DependencyProperty InlinePreeditProperty =
            DependencyProperty.Register("InlinePreedit", typeof(bool), typeof(ColorSchemeControl));

        public static readonly DependencyProperty IsBanYueModeProperty =
            DependencyProperty.Register("IsBanYueMode", typeof(bool), typeof(ColorSchemeControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsShowSpellingProperty =
            DependencyProperty.Register("IsShowSpelling", typeof(bool), typeof(ColorSchemeControl), new PropertyMetadata(true));

        public static readonly DependencyProperty Label_1Property =
                    DependencyProperty.Register("Label_1", typeof(string), typeof(ColorSchemeControl), new PropertyMetadata("㊀"));

        public static readonly DependencyProperty Label_2Property =
            DependencyProperty.Register("Label_2", typeof(string), typeof(ColorSchemeControl), new PropertyMetadata("㊁"));

        public static readonly DependencyProperty Label_3Property =
            DependencyProperty.Register("Label_3", typeof(string), typeof(ColorSchemeControl), new PropertyMetadata("㊂"));

        public static readonly DependencyProperty LabelColorProperty =
                                                                                            DependencyProperty.Register("LabelColor", typeof(Brush), typeof(ColorSchemeControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty LabelFontFaceProperty =
            DependencyProperty.Register("LabelFontFace", typeof(FontFamily), typeof(ColorSchemeControl), new PropertyMetadata(new FontFamily("Microsoft YaHei")));

        public static readonly DependencyProperty LabelFontPointProperty =
            DependencyProperty.Register("LabelFontPoint", typeof(double), typeof(ColorSchemeControl), new PropertyMetadata(12.0));

        public static readonly DependencyProperty LabelFontStyleProperty =
            DependencyProperty.Register("LabelFontStyle", typeof(FontStyle), typeof(ColorSchemeControl), new PropertyMetadata(FontStyles.Normal));

        public static readonly DependencyProperty LabelFontWeightProperty =
            DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(ColorSchemeControl), new PropertyMetadata(FontWeights.Normal));

        public static readonly DependencyProperty LabelFormatProperty =
                            DependencyProperty.Register("LabelFormat", typeof(string), typeof(ColorSchemeControl));

        public static readonly DependencyProperty MarginXProperty =
            DependencyProperty.Register("MarginX", typeof(double), typeof(ColorSchemeControl));

        public static readonly DependencyProperty MarginYProperty =
            DependencyProperty.Register("MarginY", typeof(double), typeof(ColorSchemeControl));

        public static readonly DependencyProperty MarkTextProperty =
            DependencyProperty.Register("MarkText", typeof(string), typeof(ColorSchemeControl));

        public static readonly DependencyProperty MarkTextVisibleProperty =
            DependencyProperty.Register("MarkTextVisible", typeof(Visibility), typeof(ColorSchemeControl), new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty MaxHeightColorMaxHeightProperty =
                    DependencyProperty.Register("ColorMaxHeight", typeof(double), typeof(ColorSchemeControl));

        public static readonly DependencyProperty NextPageColorProperty =
                                                                    DependencyProperty.Register("NextPageColor", typeof(string), typeof(ColorSchemeControl), new PropertyMetadata("#00000000"));

        public static readonly DependencyProperty PagingOnScrollProperty =
            DependencyProperty.Register("PagingOnScroll", typeof(bool), typeof(ColorSchemeControl));

        public static readonly DependencyProperty PreeditTypeProperty =
            DependencyProperty.Register("PreeditType", typeof(string), typeof(ColorSchemeControl));

        public static readonly DependencyProperty PrevpageColorProperty =
                            DependencyProperty.Register("PrevpageColor", typeof(string), typeof(ColorSchemeControl), new PropertyMetadata("#00000000"));

        public static readonly DependencyProperty RoundCornerProperty =
            DependencyProperty.Register("RoundCorner", typeof(double), typeof(ColorSchemeControl), new PropertyMetadata(5.0));

        public static readonly DependencyProperty ShadowColorProperty =
                                    DependencyProperty.Register("ShadowColor", typeof(Color), typeof(ColorSchemeControl));

        public static readonly DependencyProperty ShadowOffsetXProperty =
            DependencyProperty.Register("ShadowOffsetX", typeof(double), typeof(ColorSchemeControl));

        public static readonly DependencyProperty ShadowOffsetYProperty =
            DependencyProperty.Register("ShadowOffsetY", typeof(double), typeof(ColorSchemeControl));

        public static readonly DependencyProperty ShadowRadiusProperty =
            DependencyProperty.Register("ShadowRadius", typeof(double), typeof(ColorSchemeControl));

        public static readonly DependencyProperty SpacingMarginProperty =
            DependencyProperty.Register("SpacingMargin", typeof(Thickness), typeof(ColorSchemeControl));

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(ColorSchemeControl));

        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register("TextColor", typeof(Brush), typeof(ColorSchemeControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty TextVerticalProperty =
            DependencyProperty.Register("TextVertical", typeof(bool), typeof(ColorSchemeControl), new PropertyMetadata(false));

        public static readonly DependencyProperty UnInlinePreeditProperty =
            DependencyProperty.Register("UnInlinePreedit", typeof(bool), typeof(ColorSchemeControl));

        public static readonly DependencyProperty VerticalAutoReverseProperty =
            DependencyProperty.Register("VerticalAutoReverse", typeof(bool), typeof(ColorSchemeControl));

        public static readonly DependencyProperty VerticalProperty =
            DependencyProperty.Register("Vertical", typeof(bool), typeof(ColorSchemeControl), new PropertyMetadata(false));

        public static readonly DependencyProperty VerticalTextLeftToRightProperty =
            DependencyProperty.Register("VerticalTextLeftToRight", typeof(bool), typeof(ColorSchemeControl));

        public static readonly DependencyProperty VerticalTextProperty =
            DependencyProperty.Register("VerticalText", typeof(bool), typeof(ColorSchemeControl));

        public static readonly DependencyProperty VerticalTextWithWrapProperty =
            DependencyProperty.Register("VerticalTextWithWrap", typeof(bool), typeof(ColorSchemeControl));

        public ColorSchemeControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 标签、候选文字、注解文字之间的相对对齐方式 (top ; center ; bottom)
        /// </summary>
        public string AlignType
        {
            get { return (string)GetValue(AlignTypeProperty); }
            set { SetValue(AlignTypeProperty, value); }
        }

        /// <summary>
        /// antialias_mode (default；force_dword；cleartype；grayscale；aliased)
        /// </summary>
        public string AntialiasMode
        {
            get { return (string)GetValue(AntialiasModeProperty); }
            set { SetValue(AntialiasModeProperty, value); }
        }

        /// <summary>
        /// 切换 ASCII 模式时，提示跟随鼠标，而非输入光标
        /// </summary>
        public bool AsciiTipFollowCursor
        {
            get { return (bool)GetValue(AsciiTipFollowCursorProperty); }
            set { SetValue(AsciiTipFollowCursorProperty, value); }
        }

        /// <summary>
        /// 配色作者名称
        /// </summary>
        public string Author
        {
            get { return (string)GetValue(AuthorProperty); }
            set { SetValue(AuthorProperty, value); }
        }

        /// <summary>
        /// 候选窗背景色
        /// </summary>
        public Brush BackColor
        {
            get { return (Brush)GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }

        /// <summary>
        /// 候选窗边框颜色
        /// </summary>
        public Brush BorderColor
        {
            get { return (Brush)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        /// <summary>
        /// 边框与主体间的内边距
        /// </summary>
        public Thickness BorderPadding
        {
            get { return (Thickness)GetValue(BorderPaddingProperty); }
            set { SetValue(BorderPaddingProperty, value); }
        }

        /// <summary>
        /// 边框宽度
        /// </summary>
        public double BorderWidth
        {
            get { return (double)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }

        /// <summary>
        /// 候选项略写，超过此数字则用省略号代替。设置为 0 则不启用此功能
        /// </summary>
        public string CandidateAbbreviateLength
        {
            get { return (string)GetValue(CandidateAbbreviateLengthProperty); }
            set { SetValue(CandidateAbbreviateLengthProperty, value); }
        }

        /// <summary>
        /// 非高亮候选背景颜色
        /// </summary>
        public Brush CandidateBackColor
        {
            get { return (Brush)GetValue(CandidateBackColorProperty); }
            set { SetValue(CandidateBackColorProperty, value); }
        }

        /// <summary>
        /// 非高亮候选的边框颜色
        /// </summary>
        public Brush CandidateBorderColor
        {
            get { return (Brush)GetValue(CandidateBorderColorProperty); }
            set { SetValue(CandidateBorderColorProperty, value); }
        }

        // 编码区边框圆角度
        public CornerRadius CandidateBorderCorner
        {
            get { return (CornerRadius)GetValue(CandidateBorderCornerProperty); }
            set { SetValue(CandidateBorderCornerProperty, value); }
        }

        // 编码区边框边距
        public Thickness CandidateBorderMargin
        {
            get { return (Thickness)GetValue(CandidateBorderMarginProperty); }
            set { SetValue(CandidateBorderMarginProperty, value); }
        }

        /// <summary>
        /// 候选项间的距离
        /// </summary>
        public Thickness CandidateMargin
        {
            get { return (Thickness)GetValue(CandidateMarginProperty); }
            set { SetValue(CandidateMarginProperty, value); }
        }

        /// <summary>
        /// 非高亮候选背景块阴影颜色
        /// </summary>
        public Color CandidateShadowColor
        {
            get { return (Color)GetValue(CandidateShadowColorProperty); }
            set { SetValue(CandidateShadowColorProperty, value); }
        }

        /// <summary>
        /// 候选项之间的间距
        /// </summary>
        public double CandidateSpacing
        {
            get { return (double)GetValue(CandidateSpacingProperty); }
            set { SetValue(CandidateSpacingProperty, value); }
        }

        /// <summary>
        /// 非高亮候选文字颜色
        /// </summary>
        public Brush CandidateTextColor
        {
            get { return (Brush)GetValue(CandidateTextColorProperty); }
            set { SetValue(CandidateTextColorProperty, value); }
        }

        /// <summary>
        /// 鼠标点击候选项，创建截图
        /// </summary>
        public bool ClickToCapture
        {
            get { return (bool)GetValue(ClickToCaptureProperty); }
            set { SetValue(ClickToCaptureProperty, value); }
        }

        /// <summary>
        /// 颜色格式：argb；rgba；abgr（默认）
        /// </summary>
        public string ColorFormat
        {
            get { return (string)GetValue(ColorFormatProperty); }
            set { SetValue(ColorFormatProperty, value); }
        }

        /// <summary>
        /// 候选框最大高度，0 不启用此功能
        /// </summary>
        public double ColorMaxHeight
        {
            get { return (double)GetValue(MaxHeightColorMaxHeightProperty); }
            set { SetValue(MaxHeightColorMaxHeightProperty, value); }
        }

        /// <summary>
        /// 候选框最大宽度，0 不启用此功能
        /// </summary>
        public double ColorMaxWidth
        {
            get { return (double)GetValue(ColorMaxWidthProperty); }
            set { SetValue(ColorMaxWidthProperty, value); }
        }

        /// <summary>
        /// 候选框最小高度
        /// </summary>
        public double ColorMinHeight
        {
            get { return (double)GetValue(ColorMinHeightProperty); }
            set { SetValue(ColorMinHeightProperty, value); }
        }

        /// <summary>
        /// 候选框最小宽度
        /// </summary>
        public double ColorMinWidth
        {
            get { return (double)GetValue(ColorMinWidthProperty); }
            set { SetValue(ColorMinWidthProperty, value); }
        }

        /// <summary>
        /// 外观Model
        /// </summary>
        public ColorSchemeModel ColorModel
        {
            get { return (ColorSchemeModel)GetValue(ColorModelProperty); }
            set { SetValue(ColorModelProperty, value); }
        }

        /// <summary>
        /// 方案设置中显示的配色名称
        /// </summary>
        public string ColorName
        {
            get { return (string)GetValue(ColorNameProperty); }
            set { SetValue(ColorNameProperty, value); }
        }

        /// <summary>
        /// 配色方案
        /// </summary>
        public string ColorScheme
        {
            get { return (string)GetValue(ColorSchemeProperty); }
            set { SetValue(ColorSchemeProperty, value); }
        }

        /// <summary>
        /// 设置系统为深色模式时的配色方案
        /// </summary>
        public string ColorThemeDark
        {
            get { return (string)GetValue(ColorThemeDarkProperty); }
            set { SetValue(ColorThemeDarkProperty, value); }
        }

        /// <summary>
        ///  注释字体
        /// </summary>
        public FontFamily CommentFontFace
        {
            get { return (FontFamily)GetValue(CommentFontFaceProperty); }
            set { SetValue(CommentFontFaceProperty, value); }
        }

        /// <summary>
        /// 注释字号
        /// </summary>
        public double CommentFontPoint
        {
            get { return (double)GetValue(CommentFontPointProperty); }
            set { SetValue(CommentFontPointProperty, value); }
        }

        /// <summary>
        /// 注解字型
        /// </summary>
        public FontStyle CommentFontStyle
        {
            get { return (FontStyle)GetValue(CommentFontStyleProperty); }
            set { SetValue(CommentFontStyleProperty, value); }
        }

        /// <summary>
        /// 注解字重
        /// </summary>
        public FontWeight CommentFontWeight
        {
            get { return (FontWeight)GetValue(CommentFontWeightProperty); }
            set { SetValue(CommentFontWeightProperty, value); }
        }

        /// <summary>
        /// 注释文字颜色
        /// </summary>
        public Brush CommentTextColor
        {
            get { return (Brush)GetValue(CommentTextColorProperty); }
            set { SetValue(CommentTextColorProperty, value); }
        }

        /// <summary>
        /// 主体元素与面板间的外边距
        /// </summary>
        public Thickness ContentMargin
        {
            get { return (Thickness)GetValue(ContentMarginProperty); }
            set { SetValue(ContentMarginProperty, value); }
        }

        /// <summary>
        /// 候选窗口圆角半径
        /// </summary>
        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// 托盘显示独立于语言栏的额外图标
        /// </summary>
        public bool DisplayTrayIcon
        {
            get { return (bool)GetValue(DisplayTrayIconProperty); }
            set { SetValue(DisplayTrayIconProperty, value); }
        }

        /// <summary>
        /// 无法定位候选框时，在窗口左上角显示候选框
        /// </summary>
        public bool EnhancedPosition
        {
            get { return (bool)GetValue(EnhancedPositionProperty); }
            set { SetValue(EnhancedPositionProperty, value); }
        }

        /// <summary>
        /// 全局字体
        /// </summary>
        public FontFamily FontFace
        {
            get { return (FontFamily)GetValue(FontFaceProperty); }
            set { SetValue(FontFaceProperty, value); }
        }

        /// <summary>
        /// 全局字体样式
        /// </summary>
        public FontStyle FontFaceStyle
        {
            get { return (FontStyle)GetValue(FontFaceStyleProperty); }
            set { SetValue(FontFaceStyleProperty, value); }
        }

        /// <summary>
        /// 全局字体粗细
        /// </summary>
        public FontWeight FontFaceWeight
        {
            get { return (FontWeight)GetValue(FontFaceWeightProperty); }
            set { SetValue(FontFaceWeightProperty, value); }
        }

        /// <summary>
        /// 全局字号
        /// </summary>
        public double FontPoint
        {
            get { return (double)GetValue(FontPointProperty); }
            set { SetValue(FontPointProperty, value); }
        }

        /// <summary>
        /// 是否全屏模式
        /// </summary>
        public bool FullScreen
        {
            get { return (bool)GetValue(FullScreenProperty); }
            set { SetValue(FullScreenProperty, value); }
        }

        /// <summary>
        /// 高亮背影圆角值-横向布局
        /// </summary>
        public CornerRadius HiliteBorderCornerH
        {
            get { return (CornerRadius)GetValue(HiliteBorderCornerHProperty); }
            set { SetValue(HiliteBorderCornerHProperty, value); }
        }


        /// <summary>
        /// 横排显示中间项的圆角度
        /// </summary>
        public CornerRadius CenterBorderCorner
        {
            get { return (CornerRadius)GetValue(CenterBorderCornerProperty); }
            set { SetValue(CenterBorderCornerProperty, value); }
        }

        public static readonly DependencyProperty CenterBorderCornerProperty =
            DependencyProperty.Register("CenterBorderCorner", typeof(CornerRadius), typeof(ColorSchemeControl), new PropertyMetadata(new CornerRadius(0)));


        /// <summary>
        /// 横向皮肤最后选项的圆角度
        /// </summary>
        public CornerRadius LastBorderCornerH
        {
            get { return (CornerRadius)GetValue(LastBorderCornerHProperty); }
            set { SetValue(LastBorderCornerHProperty, value); }
        }

        public static readonly DependencyProperty LastBorderCornerHProperty =
            DependencyProperty.Register("LastBorderCornerH", typeof(CornerRadius), typeof(ColorSchemeControl), new PropertyMetadata(new CornerRadius(0)));


        /// <summary>
        /// 文本纵向皮肤最后选项的圆角度
        /// </summary>
        public CornerRadius LastBorderCornerTV
        {
            get { return (CornerRadius)GetValue(LastBorderCornerTVProperty); }
            set { SetValue(LastBorderCornerTVProperty, value); }
        }

        public static readonly DependencyProperty LastBorderCornerTVProperty =
            DependencyProperty.Register("LastBorderCornerTV", typeof(CornerRadius), typeof(ColorSchemeControl), new PropertyMetadata(new CornerRadius(0)));


        /// <summary>
        /// 垂直皮肤最后选项的圆角度
        /// </summary>
        public CornerRadius LastBorderCornerV
        {
            get { return (CornerRadius)GetValue(LastBorderCornerVProperty); }
            set { SetValue(LastBorderCornerVProperty, value); }
        }

        public static readonly DependencyProperty LastBorderCornerVProperty =
            DependencyProperty.Register("LastBorderCornerV", typeof(CornerRadius), typeof(ColorSchemeControl), new PropertyMetadata(new CornerRadius(0)));



        /// <summary>
        /// 高亮背景圆角值-文本纵向布局
        /// </summary>
        public CornerRadius HiliteBorderCornerTV
        {
            get { return (CornerRadius)GetValue(HiliteBorderCornerTVProperty); }
            set { SetValue(HiliteBorderCornerTVProperty, value); }
        }

        /// <summary>
        /// 高亮背影圆角值-纵向布局
        /// </summary>
        public CornerRadius HiliteBorderCornerV
        {
            get { return (CornerRadius)GetValue(HiliteBorderCornerVProperty); }
            set { SetValue(HiliteBorderCornerVProperty, value); }
        }

        // 高亮背景与边框间的外边距
        public Thickness HiliteBorderMargin
        {
            get { return (Thickness)GetValue(HiliteBorderMarginProperty); }
            set { SetValue(HiliteBorderMarginProperty, value); }
        }

        /// <summary>
        /// 编码背景颜色
        /// </summary>
        public Brush HilitedBackColor
        {
            get { return (Brush)GetValue(HilitedBackColorProperty); }
            set { SetValue(HilitedBackColorProperty, value); }
        }

        /// <summary>
        /// 高亮候选背景颜色
        /// </summary>
        public Brush HilitedCandidateBackColor
        {
            get { return (Brush)GetValue(HilitedCandidateBackColorProperty); }
            set { SetValue(HilitedCandidateBackColorProperty, value); }
        }

        /// <summary>
        /// 高亮候选边框颜色
        /// </summary>
        public Brush HilitedCandidateBorderColor
        {
            get { return (Brush)GetValue(HilitedCandidateBorderColorProperty); }
            set { SetValue(HilitedCandidateBorderColorProperty, value); }
        }

        /// <summary>
        /// 高亮候选背景块阴影颜色
        /// </summary>
        public Color HilitedCandidateShadowColor
        {
            get { return (Color)GetValue(HilitedCandidateShadowColorProperty); }
            set { SetValue(HilitedCandidateShadowColorProperty, value); }
        }

        /// <summary>
        /// 高亮候选文字颜色
        /// </summary>
        public Brush HilitedCandidateTextColor
        {
            get { return (Brush)GetValue(HilitedCandidateTextColorProperty); }
            set { SetValue(HilitedCandidateTextColorProperty, value); }
        }

        /// <summary>
        /// 高亮候选的注释颜色
        /// </summary>
        public Brush HilitedCommentTextColor
        {
            get { return (Brush)GetValue(HilitedCommentTextColorProperty); }
            set { SetValue(HilitedCommentTextColorProperty, value); }
        }

        /// <summary>
        /// 高亮候选的标签颜色
        /// </summary>
        public Brush HilitedLabelColor
        {
            get { return (Brush)GetValue(HilitedLabelColorProperty); }
            set { SetValue(HilitedLabelColorProperty, value); }
        }

        /// <summary>
        /// 高亮候选前的标记颜色
        /// </summary>
        public Brush HilitedMarkColor
        {
            get { return (Brush)GetValue(HilitedMarkColorProperty); }
            set { SetValue(HilitedMarkColorProperty, value); }
        }

        /// <summary>
        /// 编码背景块阴影颜色
        /// </summary>
        public Color HilitedShadowColor
        {
            get { return (Color)GetValue(HilitedShadowColorProperty); }
            set { SetValue(HilitedShadowColorProperty, value); }
        }

        /// <summary>
        /// 编码文字颜色
        /// </summary>
        public Brush HilitedTextColor
        {
            get { return (Brush)GetValue(HilitedTextColorProperty); }
            set { SetValue(HilitedTextColorProperty, value); }
        }

        /// <summary>
        /// 高亮区域和内部文字的间距，影响高亮区域大小
        /// </summary>
        public double HilitePadding
        {
            get { return (double)GetValue(HilitePaddingProperty); }
            set { SetValue(HilitePaddingProperty, value); }
        }

        /// <summary>
        /// 候选项和相应标签的间距，候选项与注解文字之间的距离
        /// </summary>
        public double HiliteSpacing
        {
            get { return (double)GetValue(HiliteSpacingProperty); }
            set { SetValue(HiliteSpacingProperty, value); }
        }

        // 文字与标签及注解之间的距离
        public Thickness HiSpacingMargin
        {
            get { return (Thickness)GetValue(HiSpacingMarginProperty); }
            set { SetValue(HiSpacingMarginProperty, value); }
        }

        /// <summary>
        /// 是否横向布局
        /// </summary>
        public bool Horizontal
        {
            get { return (bool)GetValue(HorizontalProperty); }
            set { SetValue(HorizontalProperty, value); }
        }

        /// <summary>
        /// 是否在行内显示预编辑区
        /// </summary>
        public bool InlinePreedit
        {
            get { return (bool)GetValue(InlinePreeditProperty); }
            set { SetValue(InlinePreeditProperty, value); }
        }

        /// <summary>
        /// 是否是启用天圆地方模式
        /// 也叫半月模式
        /// </summary>
        public bool IsBanYueMode
        {
            get { return (bool)GetValue(IsBanYueModeProperty); }
            set { SetValue(IsBanYueModeProperty, value); }
        }

        // 是否显示字根拆分提示
        public bool IsShowSpelling
        {
            get { return (bool)GetValue(IsShowSpellingProperty); }
            set { SetValue(IsShowSpellingProperty, value); }
        }

        public string Label_1
        {
            get { return (string)GetValue(Label_1Property); }
            set { SetValue(Label_1Property, value); }
        }

        public string Label_2
        {
            get { return (string)GetValue(Label_2Property); }
            set { SetValue(Label_2Property, value); }
        }

        public string Label_3
        {
            get { return (string)GetValue(Label_3Property); }
            set { SetValue(Label_3Property, value); }
        }

        /// <summary>
        /// 标签文字颜色
        /// </summary>
        public Brush LabelColor
        {
            get { return (Brush)GetValue(LabelColorProperty); }
            set { SetValue(LabelColorProperty, value); }
        }

        /// <summary>
        /// 标签字体
        /// </summary>
        public FontFamily LabelFontFace
        {
            get { return (FontFamily)GetValue(LabelFontFaceProperty); }
            set { SetValue(LabelFontFaceProperty, value); }
        }

        /// <summary>
        /// 标签字号
        /// </summary>
        public double LabelFontPoint
        {
            get { return (double)GetValue(LabelFontPointProperty); }
            set { SetValue(LabelFontPointProperty, value); }
        }

        /// <summary>
        /// 标签字型
        /// </summary>
        public FontStyle LabelFontStyle
        {
            get { return (FontStyle)GetValue(LabelFontStyleProperty); }
            set { SetValue(LabelFontStyleProperty, value); }
        }

        /// <summary>
        /// 标签字重
        /// </summary>
        public FontWeight LabelFontWeight
        {
            get { return (FontWeight)GetValue(LabelFontWeightProperty); }
            set { SetValue(LabelFontWeightProperty, value); }
        }

        /// <summary>
        /// 标签字符号
        /// </summary>
        public string LabelFormat
        {
            get { return (string)GetValue(LabelFormatProperty); }
            set { SetValue(LabelFormatProperty, value); }
        }

        /// <summary>
        /// 主体元素和候选框的左右、上下边距，为负值时，不显示候选框
        /// </summary>
        public double MarginX
        {
            get { return (double)GetValue(MarginXProperty); }
            set { SetValue(MarginXProperty, value); }
        }

        /// <summary>
        /// 主体元素和候选框的左右、上下边距，为负值时，不显示候选框
        /// </summary>
        public double MarginY
        {
            get { return (double)GetValue(MarginYProperty); }
            set { SetValue(MarginYProperty, value); }
        }

        /// <summary>
        /// 候选项前的标记符号
        /// </summary>
        public string MarkText
        {
            get { return (string)GetValue(MarkTextProperty); }
            set { SetValue(MarkTextProperty, value); }
        }

        /// <summary>
        /// 标记符号隐藏与显示
        /// </summary>
        public Visibility MarkTextVisible
        {
            get { return (Visibility)GetValue(MarkTextVisibleProperty); }
            set { SetValue(MarkTextVisibleProperty, value); }
        }

        /// <summary>
        /// 翻页箭头颜色：下一页；不设置则不显示箭头
        /// inline_preedit: false
        /// </summary>
        public string NextPageColor
        {
            get { return (string)GetValue(NextPageColorProperty); }
            set { SetValue(NextPageColorProperty, value); }
        }

        /// <summary>
        /// 在候选窗口上滑动滚轮的行为：true（翻页）；false（选中下一个候选）
        /// </summary>
        public bool PagingOnScroll
        {
            get { return (bool)GetValue(PagingOnScrollProperty); }
            set { SetValue(PagingOnScrollProperty, value); }
        }

        /// <summary>
        /// 预编辑区显示内容 composition（编码）；preview（高亮候选）；preview_all（全部候选）
        /// </summary>
        public string PreeditType
        {
            get { return (string)GetValue(PreeditTypeProperty); }
            set { SetValue(PreeditTypeProperty, value); }
        }

        /// <summary>
        /// 翻页箭头颜色：上一页；不设置则不显示箭头
        /// inline_preedit: false
        /// </summary>
        public string PrevpageColor
        {
            get { return (string)GetValue(PrevpageColorProperty); }
            set { SetValue(PrevpageColorProperty, value); }
        }

        /// <summary>
        /// 候选背景色块圆角半径，别名 hilited_corner_radius
        /// </summary>
        public double RoundCorner
        {
            get { return (double)GetValue(RoundCornerProperty); }
            set { SetValue(RoundCornerProperty, value); }
        }

        /// <summary>
        /// 候选窗阴影色
        /// </summary>
        public Color ShadowColor
        {
            get { return (Color)GetValue(ShadowColorProperty); }
            set { SetValue(ShadowColorProperty, value); }
        }

        /// <summary>
        /// 阴影绘制的偏离距离
        /// </summary>
        public double ShadowOffsetX
        {
            get { return (double)GetValue(ShadowOffsetXProperty); }
            set { SetValue(ShadowOffsetXProperty, value); }
        }

        /// <summary>
        /// 阴影绘制的偏离距离
        /// </summary>
        public double ShadowOffsetY
        {
            get { return (double)GetValue(ShadowOffsetYProperty); }
            set { SetValue(ShadowOffsetYProperty, value); }
        }

        /// <summary>
        /// 阴影区域半径，为 0 不显示阴影；需要同时在配色方案中指定非透明的阴影颜色
        /// </summary>
        public double ShadowRadius
        {
            get { return (double)GetValue(ShadowRadiusProperty); }
            set { SetValue(ShadowRadiusProperty, value); }
        }

        /// <summary>
        /// inline_preedit 为 false 时，编码区域和候选区域的间距
        /// </summary>
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        /// <summary>
        /// 编码区与候选项的距离
        /// </summary>
        public Thickness SpacingMargin
        {
            get { return (Thickness)GetValue(SpacingMarginProperty); }
            set { SetValue(SpacingMarginProperty, value); }
        }

        /// <summary>
        /// 默认文字颜色
        /// </summary>
        public Brush TextColor
        {
            get { return (Brush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        // 文本纵向显示模式
        public bool TextVertical
        {
            get { return (bool)GetValue(TextVerticalProperty); }
            set { SetValue(TextVerticalProperty, value); }
        }

        /// <summary>
        /// 是否取消在行内显示预编辑区
        /// </summary>
        public bool UnInlinePreedit
        {
            get { return (bool)GetValue(UnInlinePreeditProperty); }
            set { SetValue(UnInlinePreeditProperty, value); }
        }

        public bool Vertical
        {
            get { return (bool)GetValue(VerticalProperty); }
            set { SetValue(VerticalProperty, value); }
        }

        /// <summary>
        /// 文本竖排模式下，候选窗口位于光标上方时倒序排序
        /// </summary>
        public bool VerticalAutoReverse
        {
            get { return (bool)GetValue(VerticalAutoReverseProperty); }
            set { SetValue(VerticalAutoReverseProperty, value); }
        }

        /// <summary>
        /// 是否启用竖排文本
        /// </summary>
        public bool VerticalText
        {
            get { return (bool)GetValue(VerticalTextProperty); }
            set { SetValue(VerticalTextProperty, value); }
        }

        /// <summary>
        /// 竖排方向是否从左到右
        /// </summary>
        public bool VerticalTextLeftToRight
        {
            get { return (bool)GetValue(VerticalTextLeftToRightProperty); }
            set { SetValue(VerticalTextLeftToRightProperty, value); }
        }

        /// <summary>
        /// 文本竖排模式下是否自动换行
        /// </summary>
        public bool VerticalTextWithWrap
        {
            get { return (bool)GetValue(VerticalTextWithWrapProperty); }
            set { SetValue(VerticalTextWithWrapProperty, value); }
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;

            ColorSchemeControl c = (ColorSchemeControl)d;
            c.ColorModel = e.NewValue as ColorSchemeModel;

            ColorStyle styleModel = c.ColorModel.Style;
            ColorScheme schemeModel = c.ColorModel.UsedColor;
            string color_format = schemeModel.color_format ??= "abgr";

            // 公共
            c.TextColor = c.BrushConvter(schemeModel.text_color, colorFormat: color_format);
            c.FontPoint = double.Parse(styleModel.font_point);
            c.LabelFontPoint = string.IsNullOrEmpty(styleModel.label_font_point) ? c.FontPoint : double.Parse(styleModel.label_font_point);
            c.LabelColor = c.BrushConvter(schemeModel.label_color, Colors.Gray.ToString(), colorFormat: color_format);
            c.CommentFontPoint = string.IsNullOrEmpty(styleModel.comment_font_point) ? c.FontPoint : double.Parse(styleModel.comment_font_point);
            c.LabelFontFace = string.IsNullOrEmpty(styleModel.label_font_face) ? c.FontFace : new FontFamily(styleModel.label_font_face);
            c.CommentFontFace = string.IsNullOrEmpty(styleModel.comment_font_face) ? c.FontFace : new FontFamily(styleModel.comment_font_face);
            c.CommentTextColor = c.BrushConvter(schemeModel.comment_text_color, Colors.Gray.ToString(), colorFormat: color_format);

            #region 候选文本字体处理

            string[] font_info = new string[] { "微软雅黑", "", "" };
            var font_strs = styleModel.font_face.Split(":");
            for (int i = 0; i < font_strs.Length; i++)
                font_info[i] = font_strs[i];

            c.FontFace = new FontFamily(font_info[0]);

            if (font_info[1] == "bold")
            {
                c.FontFaceWeight = FontWeights.Bold;
                c.FontFaceStyle = FontStyles.Normal;
            }
            else if (font_info[1] == "italic")
            {
                c.FontFaceWeight = FontWeights.Normal;
                c.FontFaceStyle = FontStyles.Italic;
            }
            else
            {
                c.FontFaceWeight = FontWeights.Normal;
                c.FontFaceStyle = FontStyles.Normal;
            }

            if (font_info[2] == "italic")
                c.FontFaceStyle = FontStyles.Italic;

            c.FontPoint = double.Parse(styleModel.font_point);

            #endregion 候选文本字体处理

            #region 标签字体处理

            string[] label_font_info = new string[] { "微软雅黑", "", "" };
            var label_font_strs = styleModel.label_font_face.Split(":");
            for (int i = 0; i < label_font_strs.Length; i++)
                label_font_info[i] = label_font_strs[i];

            c.LabelFontFace = new FontFamily(label_font_info[0]);

            if (label_font_info[1] == "bold")
            {
                c.LabelFontWeight = FontWeights.Bold;
                c.LabelFontStyle = FontStyles.Normal;
            }
            else if (label_font_info[1] == "italic")
            {
                c.LabelFontWeight = FontWeights.Normal;
                c.LabelFontStyle = FontStyles.Italic;
            }
            else
            {
                c.LabelFontWeight = FontWeights.Normal;
                c.LabelFontStyle = FontStyles.Normal;
            }

            if (label_font_info[2] == "italic")
                c.LabelFontStyle = FontStyles.Italic;

            c.LabelFontPoint = double.Parse(styleModel.label_font_point);

            #endregion 标签字体处理

            #region 注解字体处理

            string[] comment_font_info = new string[] { "微软雅黑", "", "" };
            var comment_font_strs = styleModel.comment_font_face.Split(":");
            for (int i = 0; i < comment_font_strs.Length; i++)
                comment_font_info[i] = comment_font_strs[i];

            c.CommentFontFace = new FontFamily(comment_font_info[0]);

            if (comment_font_info[1] == "bold")
            {
                c.CommentFontWeight = FontWeights.Bold;
                c.CommentFontStyle = FontStyles.Normal;
            }
            else if (comment_font_info[1] == "italic")
            {
                c.CommentFontWeight = FontWeights.Normal;
                c.CommentFontStyle = FontStyles.Italic;
            }
            else
            {
                c.CommentFontWeight = FontWeights.Normal;
                c.CommentFontStyle = FontStyles.Normal;
            }

            if (comment_font_info[2] == "italic")
                c.CommentFontStyle = FontStyles.Italic;

            c.CommentFontPoint = double.Parse(styleModel.comment_font_point);

            #endregion 注解字体处理

            // 边框/候选窗口
            c.BackColor = c.BrushConvter(schemeModel.back_color, colorFormat: color_format);
            c.BorderColor = c.BrushConvter(schemeModel.border_color, colorFormat: color_format);
            c.BorderWidth = double.Parse(styleModel.layout.border_width);
            c.CornerRadius = double.Parse(styleModel.layout.corner_radius);
            c.ShadowColor = c.ColorConvter(schemeModel.shadow_color, colorFormat: color_format);

            // 编码区
            c.HilitedTextColor = c.BrushConvter(schemeModel.hilited_text_color, schemeModel.text_color, colorFormat: color_format);
            c.HilitedBackColor = c.BrushConvter(schemeModel.hilited_back_color, schemeModel.back_color, colorFormat: color_format);
            c.HilitedShadowColor = c.ColorConvter(schemeModel.hilited_shadow_color, colorFormat: color_format);

            // 高亮候选
            c.HilitedCandidateBackColor = c.BrushConvter(schemeModel.hilited_candidate_back_color, schemeModel.back_color, colorFormat: color_format);
            c.HilitedCandidateTextColor = c.BrushConvter(schemeModel.hilited_candidate_text_color, c.HilitedTextColor.ToString(), colorFormat: color_format);
            c.HilitedCandidateBorderColor = c.BrushConvter(schemeModel.hilited_candidate_border_color, schemeModel.hilited_candidate_back_color, colorFormat: color_format);
            c.RoundCorner = double.Parse(styleModel.layout.round_corner);
            c.HilitedLabelColor = c.BrushConvter(schemeModel.hilited_label_color, Colors.Gray.ToString(), colorFormat: color_format);
            c.MarkText = styleModel.mark_text;
            c.HilitedMarkColor = c.BrushConvter(schemeModel.hilited_mark_color, schemeModel.text_color, colorFormat: color_format);
            if (!string.IsNullOrEmpty(schemeModel.hilited_mark_color))
                c.MarkTextVisible = Visibility.Visible;
            c.HilitedCandidateShadowColor = c.ColorConvter(schemeModel.hilited_candidate_shadow_color, colorFormat: color_format);
            c.HilitedCommentTextColor = c.BrushConvter(schemeModel.hilited_comment_text_color, Colors.Gray.ToString(), colorFormat: color_format);

            // 非高亮区
            c.CandidateTextColor = c.BrushConvter(schemeModel.candidate_text_color, schemeModel.text_color, colorFormat: color_format);
            c.CandidateBackColor = c.BrushConvter(schemeModel.candidate_back_color, schemeModel.back_color, colorFormat: color_format);
            c.CandidateShadowColor = c.ColorConvter(schemeModel.candidate_shadow_color, colorFormat: color_format);
            c.CandidateBorderColor = c.BrushConvter(schemeModel.candidate_border_color, schemeModel.back_color, colorFormat: color_format);

            // 布局
            c.HilitePadding = double.Parse(styleModel.layout.hilite_padding) - c.BorderWidth;
            c.HiliteSpacing = double.Parse(styleModel.layout.hilite_spacing);
            c.HiSpacingMargin = new Thickness(c.HiliteSpacing, 0, c.HiliteSpacing, 0);
            // margin x y
            c.MarginX = double.Parse(styleModel.layout.margin_x) - double.Parse(styleModel.layout.hilite_padding);
            c.MarginY = double.Parse(styleModel.layout.margin_y) - double.Parse(styleModel.layout.hilite_padding);
            c.MarginX = c.MarginX <= 0 ? 0 : c.MarginX;
            c.MarginY = c.MarginY <= 0 ? 0 : c.MarginY;
            c.ContentMargin = new Thickness(c.MarginX, c.MarginY, c.MarginX, c.MarginY);
            c.Spacing = double.Parse(styleModel.layout.spacing) - (c.BorderWidth * 2);
            c.SpacingMargin = new Thickness(0, 0, 0, c.Spacing);
            c.CandidateSpacing = double.Parse(styleModel.layout.candidate_spacing) - (c.BorderWidth * 4);
            c.CandidateMargin = new Thickness(0, 0, 0, c.CandidateSpacing);
            c.InlinePreedit = bool.Parse(styleModel.inline_preedit);
            c.UnInlinePreedit = !c.InlinePreedit;
            c.TextVertical = bool.Parse(styleModel.vertical_text ?? "false");
            // # 如果方本采用纵书模式，则横向布局和纵向布局都将无效
            if (c.TextVertical)
            {
                c.Vertical = false;
                c.Horizontal = false;
            }
            else
            {
                c.Horizontal = bool.Parse(styleModel.horizontal ?? "false");
                c.Vertical = !c.Horizontal;
            }

            #region 天圆地方模式处理
            //CenterBorderCorner
            /**「天圓地方」佈局：由 margin 與 hilite_padding 確定, 當margin <= hilite_padding時生效**/
            var candidate_corner = c.RoundCorner >= 2 ? c.RoundCorner - 2 : 0;
            if (c.InlinePreedit)
            {
                // 当编码在行内时
                if (double.Parse(styleModel.layout.margin_x) <= double.Parse(styleModel.layout.hilite_padding))
                {
                    // # 天圆地方模式
                    c.IsBanYueMode = true;
                    c.HiliteBorderCornerTV = new CornerRadius(c.RoundCorner, 0, 0, c.RoundCorner);
                    c.HiliteBorderCornerV = new CornerRadius(candidate_corner, candidate_corner, 0, 0);
                    c.HiliteBorderCornerH = new CornerRadius(candidate_corner, 0, 0, candidate_corner);
                    c.LastBorderCornerV = new CornerRadius(0, 0, c.RoundCorner, c.RoundCorner);
                    c.LastBorderCornerH = new CornerRadius(0, c.RoundCorner, c.RoundCorner, 0);
                    c.LastBorderCornerTV = new CornerRadius(0, c.RoundCorner, c.RoundCorner, 0);
                    c.CenterBorderCorner = new CornerRadius(0);
                    c.HiliteBorderMargin = new Thickness(-2);
                    c.BorderPadding = new Thickness(0);
                }
                else
                {
                    // # 非天圆地方模式
                    c.IsBanYueMode = false;
                    c.HiliteBorderCornerTV = new CornerRadius(c.RoundCorner);
                    c.HiliteBorderCornerV = new CornerRadius(candidate_corner);
                    c.LastBorderCornerV = new CornerRadius(c.RoundCorner);
                    c.HiliteBorderCornerH = new CornerRadius(candidate_corner);
                    c.LastBorderCornerH = new CornerRadius(c.RoundCorner);
                    c.LastBorderCornerTV = new CornerRadius(c.RoundCorner);
                    c.CenterBorderCorner = new CornerRadius(c.RoundCorner);
                    c.HiliteBorderMargin = new Thickness(0);
                    c.BorderPadding = new Thickness(2);
                }
            }
            else // 当编码在高亮区时
            {
                if (double.Parse(styleModel.layout.margin_x) <= double.Parse(styleModel.layout.hilite_padding))
                {
                    // 判断是天圆地方模式
                    c.IsBanYueMode = true;
                    c.HiliteBorderCornerTV = new CornerRadius(0);
                    c.HiliteBorderCornerV = new CornerRadius(0);
                    c.HiliteBorderCornerH = new CornerRadius(0, 0, 0, candidate_corner);
                    c.LastBorderCornerV = new CornerRadius(0, 0, c.RoundCorner, c.RoundCorner);
                    c.LastBorderCornerH = new CornerRadius(0, c.RoundCorner, c.RoundCorner, 0);
                    c.LastBorderCornerTV = new CornerRadius(0, c.RoundCorner, c.RoundCorner, 0);
                    c.CenterBorderCorner = new CornerRadius(0);
                    c.HiliteBorderMargin = new Thickness(-2);
                    c.CandidateBorderMargin = new Thickness(-2, -2, 0, 2);
                    c.CandidateBorderCorner = new CornerRadius(candidate_corner, 0, 0, 0);
                    c.BorderPadding = new Thickness(0);
                }
                else
                {
                    // 判断为普通模式
                    c.IsBanYueMode = false;
                    c.HiliteBorderCornerTV = new CornerRadius(c.RoundCorner);
                    c.HiliteBorderCornerV = new CornerRadius(candidate_corner);
                    c.LastBorderCornerV = new CornerRadius(c.RoundCorner);
                    c.HiliteBorderCornerH = new CornerRadius(candidate_corner);
                    c.LastBorderCornerH = new CornerRadius(c.RoundCorner);
                    c.LastBorderCornerTV = new CornerRadius(c.RoundCorner);
                    c.CenterBorderCorner = new CornerRadius(c.RoundCorner);
                    c.HiliteBorderMargin = new Thickness(0);
                    c.CandidateBorderMargin = new Thickness(0, 0, 0, 2);  // 编码区的外边距
                    c.CandidateBorderCorner = new CornerRadius(candidate_corner);  // 编码区的圆角值
                    c.BorderPadding = new Thickness(2);
                }
            }
            #endregion

            #region 阴影效果处理
            //if (!c.ColorModel.OtherProperty.IsUseShade)
            //    c.ColorModel.Style.layout.shadow_radius = "0";
            //else
            //    c.ColorModel.Style.layout.shadow_radius = "8";
            #endregion


            // 处理其它属性，比如序号之类
            // 序号处理
            string label_str = c.ColorModel.OtherProperty.LabelStr;
            string[] label_array = label_str.Replace("[", "").Replace("]", "").Replace(" ", "").Split(",");
            c.Label_1 = label_array[0];
            c.Label_2 = label_array[1];
            c.Label_3 = label_array[2];
            // 序号后缀
            string suffix_str = c.ColorModel.OtherProperty.LabelSuffix;
            suffix_str = suffix_str == "无" ? "" : suffix_str;
            suffix_str = suffix_str == "空格" ? " " : suffix_str;
            c.Label_1 += suffix_str;
            c.Label_2 += suffix_str;
            c.Label_3 += suffix_str;
            // mark 标记
            //string mark_str = c.ColorModel.OtherProperty.MarkText;
            //mark_str = mark_str == "默认" ? "" : mark_str;
            //mark_str = mark_str == "无" ? "*" : mark_str;
            //if (mark_str == "*")
            //{
            //    c.MarkText = "";
            //    //c.ColorModel.UsedColor.hilited_mark_color = "";
            //}
            //else
            //{
            //    c.MarkText = mark_str;
            //}
            // 拆分字根提示是否显示
            c.IsShowSpelling = c.ColorModel.OtherProperty.ShowSpelling;
        }

        private Brush BrushConvter(string colorTxt, string defaultColor = "0x00000000", string colorFormat = "abgr")
        {
            Color targetColor = ColorConvter(colorTxt, defaultColor, colorFormat);
            SolidColorBrush targetBrush = new SolidColorBrush(targetColor);
            return targetBrush;
        }

        private Color ColorConvter(string colorTxt, string defaultColor = "0x00000000", string colorFormat = "abgr")
        {
            var color_str = ColorConverterHelper.ConverterFromRime(colorTxt, defaultColor, colorFormat);
            var color = (Color)ColorConverter.ConvertFromString(color_str);
            return color;
        }
    }
}