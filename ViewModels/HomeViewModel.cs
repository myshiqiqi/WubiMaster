﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WubiMaster.Common;
using WubiMaster.Models;
using WubiMaster.Views.PopViews;

namespace WubiMaster.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private HomeConfigModel configModel;

        [ObservableProperty]
        private bool schema06State;

        [ObservableProperty]
        private bool schema86State;

        [ObservableProperty]
        private bool schema98State;

        [ObservableProperty]
        private int shiciInterval = 25;

        [ObservableProperty]
        private string spellingKeytText = "钱";

        [ObservableProperty]
        private string spellingText;

        [ObservableProperty]
        private string spellTextShow06;

        [ObservableProperty]
        private string spellTextShow86;

        [ObservableProperty]
        private string spellTextShow98;

        public HomeViewModel()
        {
            ConfigModel = new HomeConfigModel();

            WeakReferenceMessenger.Default.Register<string, string>(this, "ChangeShiciInterval", ChangeShiciInterval);
            WeakReferenceMessenger.Default.Register<string, string>(this, "ChangeShcemaState", ChangeShcemaState);
            WeakReferenceMessenger.Default.Register<string, string>(this, "ChangeWubiDict", ChangeWubiDict);
            WeakReferenceMessenger.Default.Register<string, string>(this, "ChangeShiciBackType", ChangeShiciBackType);

            LoadSpellTextShow();
            GetTheKeyTextAsync();
            LoadConfig();
        }

        private void ChangeShiciBackType(object recipient, string message)
        {
            string type = message.ToString();
            if (type == "0")
            {
                ConfigModel.HomeVectorBack = true;
                ConfigModel.HomeQinghuaBack = false;
            }
            else
            {
                ConfigModel.HomeVectorBack = false;
                ConfigModel.HomeQinghuaBack = true;
            }
            ConfigModel.SaveConfig();
        }

        // 切换五笔码表
        private void ChangeWubiDict(object recipient, string message)
        {
            string type = message;
            string tableFiles = @$"{GlobalValues.UserPath}\tables\86";
            if (type == "06")
                tableFiles = @$"{GlobalValues.UserPath}\tables\06";
            else if (type == "98")
                tableFiles = @$"{GlobalValues.UserPath}\tables\98";
            else
                tableFiles = @$"{GlobalValues.UserPath}\tables\86";

            try
            {
                DirectoryInfo mabiaoDir = new DirectoryInfo(tableFiles);
                FileSystemInfo[] mabiaoInfo = mabiaoDir.GetFileSystemInfos();
                foreach (FileSystemInfo info in mabiaoInfo)
                {
                    if (info is not DirectoryInfo)
                        File.Copy(info.FullName, GlobalValues.UserPath + @$"\{info.Name}", true);
                }

                UdateShcemaState(type);
                ConfigHelper.WriteConfigByString("running_schema", type);
                WeakReferenceMessenger.Default.Send<string, string>(type, "ChangeQuickSpllType");
                WeakReferenceMessenger.Default.Send<string, string>("", "ChangeColorScheme");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message, true);
            }
        }

        [RelayCommand]
        public void ViewLoaded()
        {
            ShowWelcomeViewAsync();
        }

        private async void ShowWelcomeViewAsync()
        {
            bool is_first_login = ConfigHelper.ReadConfigByBool("is_first_login", true);
            if (!is_first_login) return;
            //await Task.Delay(3 * 1000);

            await App.Current.Dispatcher.BeginInvoke(() =>
            {
                ShowPopWindow(new WelcomeView());

                ConfigHelper.WriteConfigByBool("is_first_login", false);
            });

        }

        private static void ShowPopWindow(Window popView)
        {
            //Window mainWindow = App.Current.MainWindow;

            //WeakReferenceMessenger.Default.Send<string, string>("true", "ShowMaskLayer");
            //popView.Owner = mainWindow;
            //popView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popView.ShowPop();
        }

        [RelayCommand]
        public void ShowDonationView(object obj)
        {
            ShowPopWindow(new DonationView());
        }

        [RelayCommand]
        public async void ChangeWbTable(object obj)
        {
            if (obj == null) return;
            string type = obj.ToString();
            string tableFiles = @$"{GlobalValues.UserPath}\tables\86";
            if (type == "06")
                tableFiles = @$"{GlobalValues.UserPath}\tables\06";
            else if (type == "98")
                tableFiles = @$"{GlobalValues.UserPath}\tables\98";
            else
                tableFiles = @$"{GlobalValues.UserPath}\tables\86";

            try
            {
                UdateShcemaState("null");

                // 先检测rime环境
                if (string.IsNullOrEmpty(GlobalValues.UserPath) || string.IsNullOrEmpty(GlobalValues.ProcessPath))
                {
                    this.ShowMessage("未检测到 Rime 引擎的安装信息，请先安装 Rime 程序！", DialogType.Warring);
                    return;
                }

                // 检测rime是否已初始化
                if (!File.Exists(GlobalValues.UserPath + GlobalValues.SchemaKey))
                {
                    this.ShowMessage("请先到设置中心初始化 Rime 引擎");
                    return;
                }

                // 在配置前，先提示会将原有的方案覆盖
                bool? result = this.ShowAskMessage($"确认将码表切换为 {type} 版本吗？", DialogType.Normal);
                if (result != true)
                    return;

                // 停止服务
                ServiceHelper.KillService();
                await Task.Delay(1000);

                // 将对应的码表解压到目录中，并覆盖同名文件
                //ZipHelper.DecompressZip(tableFiles, GlobalValues.UserPath, true);

                // 将对应的五笔码表复制到用户目录
                DirectoryInfo mabiaoDir = new DirectoryInfo(tableFiles);
                FileSystemInfo[] mabiaoInfo = mabiaoDir.GetFileSystemInfos();
                foreach (FileSystemInfo info in mabiaoInfo)
                {
                    if (info is not DirectoryInfo)
                        File.Copy(info.FullName, GlobalValues.UserPath + @$"\{info.Name}", true);
                }

                await Task.Delay(500);

                ConfigHelper.WriteConfigByString("running_schema", type);
                WeakReferenceMessenger.Default.Send<string, string>(type, "ChangeQuickSpllType");

                this.ShowMessage("配置成功，记得重新部署哦😀");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message, true);
                this.ShowMessage($"配置失败: {ex.Message}", DialogType.Error);
                return;
            }
            finally
            {
                // 启动服务
                ServiceHelper.RunService();
                string runningSchema = ConfigHelper.ReadConfigByString("running_schema");
                UdateShcemaState(runningSchema);
                WeakReferenceMessenger.Default.Send<string, string>("", "ChangeColorScheme");
            }
        }

        [RelayCommand]
        public void CopyInfo(object obj)
        {
            Clipboard.SetDataObject(obj);
            this.ShowMessage("已复制到剪贴板");
        }

        [RelayCommand]
        public void ToWebPage(object obj)
        {
            try
            {
                string url = obj?.ToString();
                Process.Start("explorer.exe", url);
            }
            catch (Exception ex)
            {
                this.ShowMessage("无法打开网址", DialogType.Error);
                LogHelper.Error(ex.Message);
            }
        }

        [RelayCommand]
        public void ZingenSearch(object obj)
        {
            if (obj == null)
            {
                SpellingText = "";
                return;
            }

            try
            {
                string keyWord = obj.ToString().Trim();
                string type = ConfigHelper.ReadConfigByString("quick_search_type");
                var result = SpellingWorker.ZingenSearch(keyWord, type);
                if (result == null)
                {
                    SpellingText = "";
                    return;
                }
                else
                {
                    SpellingText = result.Spelling + "・" + result.Code;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
        }

        private void ChangeShcemaState(object recipient, string message)
        {
            string type = message;
            ConfigHelper.WriteConfigByString("running_schema", type);
            UdateShcemaState(type);
        }

        private void ChangeShiciInterval(object recipient, string message)
        {
            try
            {
                int newInterval = int.Parse(message);

                if (newInterval < 5)
                {
                    ShiciInterval = 25;
                    return;
                }
                ShiciInterval = newInterval;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
        }

        private async void GetTheKeyTextAsync()
        {
            ConcurrentDictionary<string, string[]> keyTextDict = new ConcurrentDictionary<string, string[]>();
            List<string> keyTextList = new List<string>();

            await Task.Run(() =>
            {
                SpellingWorker.SpellingQueue86.AsParallel().ForAll(s =>
                {
                    if (s.GBType != "GBK")
                        keyTextDict.TryAdd(s.Text, new string[] { s.Code, "", "" });
                });

                SpellingWorker.SpellingQueue98.AsParallel().ForAll(s =>
                {
                    if (keyTextDict.ContainsKey(s.Text))
                        keyTextDict[s.Text][1] = s.Code;
                });

                SpellingWorker.SpellingQueue06.AsParallel().ForAll(s =>
                {
                    if (keyTextDict.ContainsKey(s.Text))
                        keyTextDict[s.Text][2] = s.Code;
                });

                Parallel.ForEach(keyTextDict.Keys, k =>
                {
                    var value = keyTextDict[k];
                    if ((value[0] != value[1]) && (value[0] != value[2]) && (value[1] != value[2]))
                        keyTextList.Add(k);
                });
            });

            Random random = new Random();
            int _keyindex = random.Next(0, keyTextList.Count);
            SpellingKeytText = keyTextList[_keyindex];
            LoadSpellTextShow();
        }

        private void LoadConfig()
        {
            string runningSchema = ConfigHelper.ReadConfigByString("running_schema");
            UdateShcemaState(runningSchema);
        }

        private void LoadSpellTextShow()
        {
            try
            {
                string keyWord = SpellingKeytText;
                var result86 = SpellingWorker.ZingenSearch(keyWord, "86");
                SpellTextShow86 = result86.Spelling + "・" + result86.Code;
                var result98 = SpellingWorker.ZingenSearch(keyWord, "98");
                SpellTextShow98 = result98.Spelling + "・" + result98.Code;
                var result06 = SpellingWorker.ZingenSearch(keyWord, "06");
                SpellTextShow06 = result06.Spelling + "・" + result06.Code;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                this.ShowMessage("无法加载字根版本示例", DialogType.Error);
            }
        }

        private void UdateShcemaState(string type)
        {
            switch (type)
            {
                case "86":
                    Schema86State = true;
                    Schema98State = false;
                    Schema06State = false;
                    break;

                case "98":
                    Schema86State = false;
                    Schema98State = true;
                    Schema06State = false;
                    break;

                case "06":
                    Schema86State = false;
                    Schema98State = false;
                    Schema06State = true;
                    break;

                default:
                    Schema86State = false;
                    Schema98State = false;
                    Schema06State = false;
                    break;
            }
        }
    }
}