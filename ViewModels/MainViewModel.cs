﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WubiMaster.Common;
using WubiMaster.Controls;
using WubiMaster.Views;
using static WubiMaster.Common.RegistryHelper;

namespace WubiMaster.ViewModels
{
    public partial class MainViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private string backIconText;

        [ObservableProperty]
        private object currentView;

        [ObservableProperty]
        private Visibility maskLayerVisable = Visibility.Collapsed;

        [ObservableProperty]
        private string pageTitle;

        [ObservableProperty]
        private UserControl pluginControl;

        private RegistryHelper registryHelper;

        [ObservableProperty]
        private bool showTestView;

        [ObservableProperty]
        private HorizontalAlignment winStateLayout = HorizontalAlignment.Left;

        public MainViewModel()
        {
            IsActive = true;
            pageDict = new Dictionary<string, object>();
            registryHelper = new RegistryHelper();

            //Register<string, string>：消息类型、token类型
            WeakReferenceMessenger.Default.Register<string, string>(this, "ShowMaskLayer", ShowMaskLayer);
            WeakReferenceMessenger.Default.Register<string, string>(this, "ChangeWinStateLayout", ChangeWinStateLayout);
            WeakReferenceMessenger.Default.Register<string, string>(this, "ChangePluginControl", ChangePluginControl);

            TestViewShow();
            GetRimeRegistryKey();
            ReadUserPathRegistry();
            ReadProcessPathRegistry();
            ReadServerRegistry();
            LaodAllSpellingDataAsync();
            SetBackIconText();
            LoadConfig();
        }

        private Dictionary<string, object> pageDict { get; set; }

        [RelayCommand]
        public void ChangePage(object pageName)
        {
            if (pageName == null) return;

            string pName = pageName.ToString();
            PageTitle = pName;
            if (pageDict.ContainsKey(pName))
                CurrentView = pageDict[pName];
            else
            {
                switch (pageName.ToString())
                {
                    case "Home":
                        HomeView homeView = new HomeView();
                        pageDict[pName] = homeView;
                        CurrentView = homeView;
                        break;

                    case "Attribute":
                        AttributeView attributeView = new AttributeView();
                        pageDict[pName] = attributeView;
                        CurrentView = attributeView;
                        break;

                    case "Etymon":
                        EtymonView etymonView = new EtymonView();
                        pageDict[pName] = etymonView;
                        CurrentView = etymonView;
                        break;

                    case "Lexicon":
                        LexiconView lexiconView = new LexiconView();
                        pageDict[pName] = lexiconView;
                        CurrentView = lexiconView;
                        break;

                    case "EtymonKey":
                        EtymonKeyView etymonKeyView = new EtymonKeyView();
                        pageDict[pName] = etymonKeyView;
                        CurrentView = etymonKeyView;
                        break;

                    case "Themes":
                        ThemeView themeView = new ThemeView();
                        pageDict[pName] = themeView;
                        CurrentView = themeView;
                        break;

                    case "Settings":
                        SettingsView settingsView = new SettingsView();
                        pageDict[pName] = settingsView;
                        CurrentView = settingsView;
                        break;

                    case "About":
                        AboutView aboutView = new AboutView();
                        pageDict[pName] = aboutView;
                        CurrentView = aboutView;
                        break;

                    default:
                        TestView testView = new TestView();
                        pageDict[pName] = testView;
                        CurrentView = testView;
                        break;
                }
            }
        }

        [RelayCommand]
        public void CloseWindow(object obj)
        {
            App.Current.MainWindow.Close();
        }

        [RelayCommand]
        public void Deploy()
        {
            App.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(GlobalValues.ProcessPath))
                    {
                        this.ShowMessage("请先安装 Rime 引擎包", DialogType.Warring);
                        return;
                    }

                    try
                    {
                        bool isRun = ServiceHelper.FindService();
                        if (!isRun)
                        {
                            this.ShowMessage("算法服务未启动，无法执行部署操作", DialogType.Warring);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.ShowMessage(ex.Message, DialogType.Warring);
                    }

                    CmdHelper.RunCmd(GlobalValues.ProcessPath, "WeaselDeployer.exe /deploy");
                    this.ShowMessage("部署成功", DialogType.Success);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.Message);
                    this.ShowMessage("部署失败", DialogType.Fail);
                }
            });
        }

        [RelayCommand]
        public async void LoadedNav(object navParent)
        {
            StackPanel panel = (StackPanel)navParent;
            foreach (NavButton navBtn in panel.Children)
            {
                string pName = navBtn.NavName;
                if (pageDict.ContainsKey(pName)) continue;

                switch (pName)
                {
                    case "Home":
                        HomeView homeView = new HomeView();
                        pageDict[pName] = homeView;
                        break;

                    case "Attribute":
                        AttributeView attributeView = new AttributeView();
                        pageDict[pName] = attributeView;
                        break;

                    case "Etymon":
                        EtymonView etymonView = new EtymonView();
                        pageDict[pName] = etymonView;
                        break;

                    case "Lexicon":
                        LexiconView lexiconView = new LexiconView();
                        pageDict[pName] = lexiconView;
                        break;

                    case "Themes":
                        ThemeView themeView = new ThemeView();
                        pageDict[pName] = themeView;
                        break;

                    case "EtymonKey":
                        EtymonKeyView etymonKeyView = new EtymonKeyView();
                        pageDict[pName] = etymonKeyView;
                        break;

                    case "Settings":
                        SettingsView settingsView = new SettingsView();
                        pageDict[pName] = settingsView;
                        break;

                    case "About":
                        AboutView aboutView = new AboutView();
                        pageDict[pName] = aboutView;
                        break;

                    default:
                        TestView testView = new TestView();
                        pageDict[pName] = testView;
                        break;
                }
            }
        }

        [RelayCommand]
        public async void LoadedWindow()
        {
            await Task.Delay(500);
            ChangePage("Home");
        }

        [RelayCommand]
        public void MaxWindow(object obj)
        {
            Border main_border = obj as Border;

            if (App.IsMaximized)
            {
                main_border.Width = 1000;
                App.Current.MainWindow.WindowState = WindowState.Normal;
                App.Current.MainWindow.Width = 1000;
                App.Current.MainWindow.Height = 700;

                App.IsMaximized = false;
            }
            else
            {
                var width_ratio = SystemParameters.PrimaryScreenWidth / SystemParameters.PrimaryScreenHeight;
                main_border.Width = 700 * width_ratio;

                App.Current.MainWindow.WindowState = WindowState.Maximized;

                App.IsMaximized = true;
            }
        }

        [RelayCommand]
        public void MinWindow(object obj)
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private static void SetDefultTheme()
        {
            string defultPack = "Pack://application:,,,/WubiMaster;component/Themes/DefultBlueTheme.xaml";
            Application.Current.Resources.MergedDictionaries[0].Source = new Uri(defultPack);
        }

        private void ChangePluginControl(object recipient, string message)
        {
            string pluginName = message;
            LoadPluginControl(pluginName);
        }

        private void ChangeWinStateLayout(object recipient, string message)
        {
            var layoutStr = message.ToString();
            if (layoutStr == "right")
                WinStateLayout = HorizontalAlignment.Right;
            else
                WinStateLayout = HorizontalAlignment.Left;

            ConfigHelper.WriteConfigByString("win_state_layout", WinStateLayout == HorizontalAlignment.Right ? "right" : "left");
        }

        private void GetRimeRegistryKey()
        {
            try
            {
                if (registryHelper.IsExist(KeyType.HKEY_LOCAL_MACHINE, @"WOW6432Node\Rime\Weasel"))
                {
                    GlobalValues.RimeKey = @"WOW6432Node\Rime\Weasel";
                }
                else if (registryHelper.IsExist(KeyType.HKEY_LOCAL_MACHINE, @"Rime\Weasel"))
                {
                    GlobalValues.RimeKey = @"Rime\Weasel";
                }

                if (string.IsNullOrEmpty(GlobalValues.RimeKey)) throw new NullReferenceException("无法找到Rime程序安装目录");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
        }

        private async void LaodAllSpellingDataAsync()
        {
            await Task.Run(() =>
            {
                SpellingWorker.LoadAllSpellingData();
            });
        }

        private void LoadConfig()
        {
            var layoutStr = ConfigHelper.ReadConfigByString("win_state_layout");
            if (layoutStr == "right") WinStateLayout = HorizontalAlignment.Right;
            else WinStateLayout = HorizontalAlignment.Left;

            // 加载logo插件
            string pluginName = ConfigHelper.ReadConfigByString("plugin_name");
            LoadPluginControl(pluginName);
        }

        private void LoadPluginControl(string pName = "Logo")
        {
            switch (pName)
            {
                case "Logo":
                    var logo = new LogoControl();
                    PluginControl = logo;
                    break;

                case "时辰":
                    var shichen = new ShichenControl();
                    PluginControl = shichen;
                    break;

                default:
                    var defaultPlugin = new LogoControl();
                    PluginControl = defaultPlugin;
                    break;
            }
        }

        private void ReadProcessPathRegistry()
        {
            try
            {
                if (string.IsNullOrEmpty(GlobalValues.RimeKey)) return;

                string pPath = registryHelper.GetValue(KeyType.HKEY_LOCAL_MACHINE, GlobalValues.RimeKey, "WeaselRoot");
                if (string.IsNullOrEmpty(pPath)) throw new NullReferenceException("ProcessFilePath: 无法找到 Rime 的注册表信息");
                GlobalValues.ProcessPath = pPath;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
        }

        private void ReadServerRegistry()
        {
            try
            {
                if (string.IsNullOrEmpty(GlobalValues.RimeKey)) return;

                string sName = registryHelper.GetValue(KeyType.HKEY_LOCAL_MACHINE, GlobalValues.RimeKey, "ServerExecutable");
                if (string.IsNullOrEmpty(sName)) throw new NullReferenceException("无法找到 Rime 的注册表信息");
                GlobalValues.ServerName = sName;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
        }

        private void ReadUserPathRegistry()
        {
            try
            {
                if (!registryHelper.IsExist(KeyType.HKEY_CURRENT_USER, @"Rime\Weasel")) return;

                string uPath = registryHelper.GetValue(KeyType.HKEY_CURRENT_USER, @"Rime\Weasel", "RimeUserDir");
                if (string.IsNullOrEmpty(uPath))
                {
                    uPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Rime";
                    if (!Directory.Exists(uPath)) throw new NullReferenceException("UserPath: 无法找到 Rime 的注册表信息");
                }

                GlobalValues.UserPath = uPath;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
        }

        private void SetBackIconText()
        {
            try
            {
                List<string> icon_texts = new List<string>();
                var backIconDict = new ResourceDictionary();
                backIconDict.Source = new Uri("pack://application:,,,/WubiMaster;component/Resource/BackIconText.xaml");
                foreach (string name in backIconDict.Keys)
                {
                    string text = backIconDict[name].ToString();
                    icon_texts.Add(text);
                }
                Random rd = new Random();
                int index = rd.Next(icon_texts.Count);
                BackIconText = icon_texts[index];
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
        }

        private void ShowMaskLayer(object recipient, string message)
        {
            bool isShow = bool.Parse(message);

            if (isShow)
                MaskLayerVisable = Visibility.Visible;
            else
                MaskLayerVisable = Visibility.Collapsed;
        }

        private void TestViewShow()
        {
#if DEBUG
            ShowTestView = true;
#else
            ShowTestView = false;
#endif
        }
    }
}