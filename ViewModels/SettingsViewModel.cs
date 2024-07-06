using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WubiMaster.Common;
using WubiMaster.Models;
using WubiMaster.Views.PopViews;

namespace WubiMaster.ViewModels
{
    public partial class SettingsViewModel : ObservableRecipient
    {
        #region ObservablePropertys

        [ObservableProperty]
        private bool autoStart;

        [ObservableProperty]
        private string backupPath;

        [ObservableProperty]
        private bool canUpdateRimeWubi;

        [ObservableProperty]
        private bool cobboxThemesEnable;

        [ObservableProperty]
        private SettingsConfigModel configModel;

        [ObservableProperty]
        private bool daemonIsRun;

        [ObservableProperty]
        private string defaultCikuFile;

        [ObservableProperty]
        private bool isFromLocal;

        [ObservableProperty]
        private bool isRandomThemes;

        [ObservableProperty]
        private int logBackIndex;

        [ObservableProperty]
        private ObservableCollection<LogBackModel> logBackList;

        [ObservableProperty]
        private int pluginIndex;

        [ObservableProperty]
        private ObservableCollection<string> pluginsList;

        [ObservableProperty]
        private string processFilePath;

        [ObservableProperty]
        private bool quickSpllType06;

        [ObservableProperty]
        private bool quickSpllType86;

        [ObservableProperty]
        private bool quickSpllType98;

        private RegistryHelper registryHelper;

        [ObservableProperty]
        private bool serviceIsRun;

        [ObservableProperty]
        private int shiciIndex;

        [ObservableProperty]
        private ObservableCollection<ShiciIntervalModel> shiciIntervalList;

        [ObservableProperty]
        private int themeIndex;

        [ObservableProperty]
        private List<ThemeModel> themeList;

        [ObservableProperty]
        private string userCikuFile;

        [ObservableProperty]
        private string userFilePath;

        [ObservableProperty]
        private bool winStateChecked;

        [ObservableProperty]
        private string wubiSchemaTip;

        #endregion ObservablePropertys

        private static readonly HttpClient client = new HttpClient();

        public SettingsViewModel()
        {
            ConfigModel = new SettingsConfigModel();
            registryHelper = new RegistryHelper();
            ThemeList = new List<ThemeModel>();
            ShiciIntervalList = new ObservableCollection<ShiciIntervalModel>();
            LogBackList = new ObservableCollection<LogBackModel>();
            PluginsList = new ObservableCollection<string>();
            PluginsList.Add("Logo");
            PluginsList.Add("时辰");

            WeakReferenceMessenger.Default.Register<string, string>(this, "ChangeQuickSpllType", ChangeQuickSpllType);

            InitThemes();
            InitShiciInterval();
            InitLogBackList();
            CheckService();
            LoadConfig();
        }

        public static void CopyDirectory(string srcPath, string destPath)
        {
            try
            {
                //获取目录下（不包含子目录）的文件和子目录
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
                foreach (FileSystemInfo i in fileinfo)
                {
                    //判断是否文件夹
                    if (i is DirectoryInfo)
                    {
                        if (!Directory.Exists(destPath + "\\" + i.Name))
                        {
                            //目标目录下不存在此文件夹即创建子文件夹
                            Directory.CreateDirectory(destPath + "\\" + i.Name);
                        }
                        //递归调用复制子文件夹
                        CopyDirectory(i.FullName, destPath + "\\" + i.Name);
                    }
                    else
                    {
                        //不是文件夹即复制文件，true表示可以覆盖同名文件
                        File.Copy(i.FullName, destPath + "\\" + i.Name, true);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 更新五笔方案文件或配置
        /// </summary>
        private async void UpdateWubiFilesAsync(bool online, string zip_name, string url = "", string save_path = "", string target_zip = "")
        {
            // 获取当前五笔版本
            string wubi_type = ConfigHelper.ReadConfigByString("running_schema", "none");
            if (wubi_type == "none") return;

            if (online)
            {
                await DownLoadWubiSchemaAsync(url, save_path);
            }
            else
            {
                save_path = target_zip;
            }

            await App.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, async () =>
            {
                try
                {
                    // 将下载好的方案zip解压
                    ZipHelper.DecompressZip(save_path, GlobalValues.UserPath);

                    // 将解压的方案文件复制到用户目录下，然后删除
                    string wubi_file = GlobalValues.UserPath + "\\" + zip_name;
                    if (Directory.Exists(wubi_file))
                        CopyDirectory(wubi_file, GlobalValues.UserPath);
                    else
                        throw new Exception($"找到不到路径：{wubi_file}");
                    Directory.Delete(wubi_file, true);

                    // 要点：更新操作时，将对应五笔版本的tables下的信息复制到用户目录下
                    string table_path = "";
                    if (wubi_type == "86") table_path = @$"{GlobalValues.UserPath}\tables\86";
                    else if (wubi_type == "98") table_path = @$"{GlobalValues.UserPath}\tables\98";
                    else table_path = @$"{GlobalValues.UserPath}\tables\06";
                    if (Directory.Exists(table_path))
                        CopyDirectory(table_path, GlobalValues.UserPath);
                    else
                        throw new Exception($"找到不到路径：{table_path}");

                    // 更新 wubi master 文本信息
                    string wubi_master_info = "";
                    wubi_master_info += "中书君标记文件，请勿删除！\n\r";
                    wubi_master_info += $"更新时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}\n\r";
                    wubi_master_info += $"五笔类型：{wubi_type}\n\r";
                    if (File.Exists(GlobalValues.UserPath + "\\" + GlobalValues.SchemaKey))
                        File.Delete(GlobalValues.UserPath + "\\" + GlobalValues.SchemaKey);
                    File.WriteAllText(GlobalValues.UserPath + "\\" + GlobalValues.SchemaKey, wubi_master_info);

                }
                catch (Exception)
                {
                    throw;
                }
            });
        }

        [RelayCommand]
        public async void ChangeWubiMode()
        {
            LodingView lodingView = new LodingView();

            try
            {
                // 判断当前环境是否已经安装rime
                if (string.IsNullOrEmpty(GlobalValues.UserPath) || string.IsNullOrEmpty(GlobalValues.ProcessPath))
                {
                    this.ShowMessage("未检测到 Rime 引擎的安装信息，请先安装 Rime 程序！", DialogType.Warring);
                    return;
                }

                // 判断当前环境是否已经初始化
                if (!File.Exists(GlobalValues.UserPath + GlobalValues.SchemaKey))
                {
                    this.ShowMessage("请先初始化五笔引擎！", DialogType.Warring);
                    return;
                }

                // 检测环境正常的情况下，对切换模式也起提示与说明
                string ask_info = "确定要切换吗？此过程有可能需要网络环境！";
                var ask = this.ShowAskMessage(ask_info);
                if (!(bool)ask)
                    return;

                // 停止服务
                ServiceHelper.KillService();
                await Task.Delay(100);

                // 异步加载 loading
                App.Current.Dispatcher.BeginInvoke(() => { lodingView.ShowPop(); });
                await Task.Delay(1500);

                if (ConfigModel.SettingsClassMode)
                {
                    // 切换为经典模式
                    if (File.Exists(GlobalValues.WubiZipPath))
                    {
                        UpdateWubiFilesAsync(false, GlobalValues.WubiFileName, target_zip: GlobalValues.WubiZipPath);
                    }
                    else
                    {
                        UpdateWubiFilesAsync(true, GlobalValues.WubiFileName, url: GlobalValues.RimeWubiZipUrl, save_path: GlobalValues.WubiZipPath);
                    }
                }
                else
                {
                    // 切换为音辅模式
                    if (File.Exists(GlobalValues.YinfuZipPath))
                    {
                        UpdateWubiFilesAsync(false, GlobalValues.YinfuFileName, target_zip: GlobalValues.YinfuZipPath);
                    }
                    else
                    {
                        UpdateWubiFilesAsync(true, GlobalValues.YinfuFileName, url: GlobalValues.YinfuZipUrl, save_path: GlobalValues.YinfuZipPath);
                    }
                }

                ConfigModel.SaveConfig();
                this.ShowMessage("切换成功", DialogType.Success);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                this.ShowMessage(ex.ToString(), DialogType.Error);
            }
            finally
            {
                // 启动服务
                UpdateWubiSchemaTip();
                lodingView.ClosePop();
                ServiceHelper.RunService();
                await Task.Delay(500);
                ServiceHelper.Deployer();
            }
        }

        [RelayCommand]
        public async void Backup()
        {
            try
            {
                if (string.IsNullOrEmpty(GlobalValues.UserPath) || !Directory.Exists(GlobalValues.UserPath))
                {
                    this.ShowMessage("找不到 Rime 程序用户目录！", DialogType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(BackupPath))
                    this.ShowMessage("请先设置备份目录");

                ServiceHelper.KillService();
                await Task.Delay(500);

                string zipName = $"backup_{DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss.fff")}.zip";
                ZipHelper.CompressDirectoryZip(GlobalValues.UserPath, BackupPath + $@"\{zipName}");

                ServiceHelper.RunService();

                this.ShowMessage($"备份成功：{zipName}", DialogType.Success);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                this.ShowMessage("备份失败：" + ex.Message);
            }
        }

        [RelayCommand]
        public void ChagedDaemonState()
        {
            if (DaemonIsRun)
                ServiceHelper.StartDaemon();
            else
                ServiceHelper.StopDaemon();

            ConfigHelper.WriteConfigByBool("daemon_state", DaemonIsRun);
        }

        [RelayCommand]
        public void ChangeLogBackDays(string days)
        {
            ConfigHelper.WriteConfigByString("log_back_days", days);
        }

        [RelayCommand]
        public void ChangePlugins(object obj)
        {
            if (obj == null) return;

            string plugName = obj.ToString();
            ConfigHelper.WriteConfigByString("plugin_name", plugName);
            WeakReferenceMessenger.Default.Send<string, string>(plugName, "ChangePluginControl");
        }

        [RelayCommand]
        public void ChangeShiciInterval(int index)
        {
            string interval = ShiciIntervalList.First(i => i.Id == index).Minutes.ToString();
            WeakReferenceMessenger.Default.Send<string, string>(interval, "ChangeShiciInterval");

            ConfigHelper.WriteConfigByString("shici_interval", interval);
        }

        [RelayCommand]
        public void ChangeTheme(string theme)
        {
            if (theme == null || theme.Length == 0) { return; }
            try
            {
                string pack = $"pack://application:,,,/WubiMaster;component/Themes/{theme}.xaml";
                ResourceDictionary themeResource = new ResourceDictionary();
                themeResource.Source = new Uri(pack);
                Application.Current.Resources.MergedDictionaries[0].Source = themeResource.Source;

                var themeModel = ThemeList.FirstOrDefault(t => t.Value == theme);
                ThemeIndex = ThemeList.IndexOf(themeModel);
                ConfigHelper.WriteConfigByString("theme_value", theme);

                WeakReferenceMessenger.Default.Send<string, string>("SmartSkinColor", "SmartSkinColor");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                this.ShowMessage(ex.Message, DialogType.Error);
            }
        }

        [RelayCommand]
        public void CheckService()
        {
            ServiceIsRun = ServiceHelper.FindService();
        }

        [RelayCommand]
        public async Task GetRimeWubiAsync()
        {
            // 先检测rime环境
            if (string.IsNullOrEmpty(GlobalValues.UserPath) || string.IsNullOrEmpty(GlobalValues.ProcessPath))
            {
                this.ShowMessage("未检测到 Rime 引擎的安装信息，请先安装 Rime 程序！", DialogType.Warring);
                return;
            }

            // 在配置前，先提示会将原有的方案覆盖
            bool? result = this.ShowAskMessage("请注意：本次操作将清除 Rime 用户目录下所有数据！", DialogType.Normal);
            if (result != true)
                return;

            if (isFromLocal)
            {
                // 从本地选择 rime-wubi.zip 文件
                var openFileDialog = new OpenFileDialog()
                {
                    Filter = "rime-wubi.zip (.zip)|*.zip"
                };

                var dialog_result = (bool)openFileDialog.ShowDialog();
                if (dialog_result)
                {
                    var rime_wubi_zip = openFileDialog.FileName;

                    // 将本地zip文件移动到指定目录
                    if (File.Exists(GlobalValues.WubiZipPath))
                        File.Delete(GlobalValues.WubiZipPath);
                    File.Copy(rime_wubi_zip, GlobalValues.WubiZipPath);
                }
                else
                {
                    return;
                }
            }

            // 停止服务
            ServiceHelper.KillService();
            await Task.Delay(100);

            // 异步加载 loading
            LodingView lodingView = new LodingView();
            App.Current.Dispatcher.BeginInvoke(() => { lodingView.ShowPop(); });
            await Task.Delay(1500);

            if (!isFromLocal)
            {
                // 删除从github下载的旧方案
                if (File.Exists(GlobalValues.WubiZipPath))
                    File.Delete(GlobalValues.WubiZipPath);

                // 从github下载方案
                var down_value = await DownLoadWubiSchemaAsync(GlobalValues.RimeWubiZipUrl, GlobalValues.WubiZipPath);
                if (!down_value)
                {
                    lodingView.ClosePop();
                    this.ShowMessage("网络情况不佳，无法从 Github 获取五笔方案资源", DialogType.Error);
                    return;
                }
            }

            await App.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, async () =>
            {
                try
                {
                    // 删除用户目录中的配置
                    if (Directory.Exists(GlobalValues.UserPath))
                    {
                        DirectoryInfo dir = new DirectoryInfo(GlobalValues.UserPath);
                        FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                        foreach (FileSystemInfo i in fileinfo)
                        {
                            if (i is DirectoryInfo)            //判断是否文件夹
                            {
                                try
                                {
                                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                                    subdir.Delete(true);          //删除子目录和文件
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                File.Delete(i.FullName);      //删除指定文件
                            }
                        }
                    }

                    // 安装字根字体
                    if (!FontHelper.CheckFont("黑体字根.ttf"))
                    {
                        string heiti_font = GlobalValues.HeitiFont;
                        FontHelper.InstallFont(heiti_font);
                    }

                    // 将下载好的方案zip解压
                    ZipHelper.DecompressZip(GlobalValues.WubiZipPath, GlobalValues.UserPath);

                    // 将解压的方案文件复制到用户目录下，然后删除
                    string wubi_file = GlobalValues.UserPath + "\\" + GlobalValues.WubiFileName;
                    if (Directory.Exists(wubi_file))
                        CopyDirectory(wubi_file, GlobalValues.UserPath);
                    Directory.Delete(wubi_file, true);

                    // 添加初始化成功的标记，标记为 wubi_master.txt
                    // 并在该文件中添加初始化信息
                    string wubi_master_str = "";
                    wubi_master_str += "中书君标记文件，请勿删除！\n\r";
                    wubi_master_str += $"更新时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}\n\r";
                    wubi_master_str += $"五笔类型：86\n\r";
                    File.WriteAllText(GlobalValues.UserPath + "\\" + GlobalValues.SchemaKey, wubi_master_str);

                    // 首页同步，默认五笔方案设置为86
                    WeakReferenceMessenger.Default.Send<string, string>("86", "ChangeShcemaState");

                    lodingView.ClosePop();
                    this.ShowMessage("初始化成功", DialogType.Success);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.Message, true);
                    WeakReferenceMessenger.Default.Send<string, string>("", "ChangeShcemaState");
                    this.ShowMessage($"初始化失败: {ex.Message}", DialogType.Error);
                    return;
                }
                finally
                {
                    // 启动服务
                    UpdateWubiSchemaTip();
                    lodingView.ClosePop();
                    ServiceHelper.RunService();
                    await Task.Delay(500);
                    CmdHelper.RunCmd(GlobalValues.ProcessPath, "WeaselDeployer.exe /deploy");
                }
            });
        }

        [RelayCommand]
        public void OpenLogPath()
        {
            string path = GlobalValues.AppDirectory + "Logs";
            if (!Directory.Exists(path))
            {
                this.ShowMessage("找不到日志目录！", DialogType.Error);
                return;
            }
            Process.Start("explorer.exe", path);
        }

        [RelayCommand]
        public void OpenProcessFilePath()
        {
            if (string.IsNullOrEmpty(GlobalValues.ProcessPath) || !Directory.Exists(GlobalValues.ProcessPath))
            {
                this.ShowMessage("找不到 Rime 程序安装目录！", DialogType.Error);
                return;
            }
            Process.Start("explorer.exe", GlobalValues.ProcessPath);

            //System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            //System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            //if (result == System.Windows.Forms.DialogResult.Cancel)
            //{
            //    return;
            //}
            //ProcessFilePath = dialog.SelectedPath;

            //ConfigHelper.WriteConfigByString("process_file_path", ProcessFilePath);
        }

        [RelayCommand]
        public void OpenUserFilePath()
        {
            if (string.IsNullOrEmpty(GlobalValues.UserPath) || !Directory.Exists(GlobalValues.UserPath))
            {
                this.ShowMessage("找不到 Rime 程序用户目录！", DialogType.Error);
                return;
            }
            Process.Start("explorer.exe", GlobalValues.UserPath);
            //System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            //System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            //if (result == System.Windows.Forms.DialogResult.Cancel)
            //{
            //    return;
            //}
            //UserFilePath = dialog.SelectedPath;

            //ConfigHelper.WriteConfigByString("user_file_path", UserFilePath);
        }

        [RelayCommand]
        public void QuickSpellChange()
        {
            try
            {
                if (QuickSpllType86)
                    ConfigHelper.WriteConfigByString("quick_search_type", "86");
                else if (QuickSpllType98)
                    ConfigHelper.WriteConfigByString("quick_search_type", "98");
                else if (QuickSpllType06)
                    ConfigHelper.WriteConfigByString("quick_search_type", "06");
                else
                    ConfigHelper.WriteConfigByString("quick_search_type", "86");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                this.ShowMessage(ex.Message, DialogType.Warring);
            }
        }

        [RelayCommand]
        public void RandomThemes()
        {
            if (IsRandomThemes)
            {
                CobboxThemesEnable = false;
                Random random = new Random();
                ThemeIndex = random.Next(0, ThemeList.Count);
            }
            else
            {
                CobboxThemesEnable = true;
            }
            ConfigHelper.WriteConfigByBool("is_random_themes", IsRandomThemes);
        }

        [RelayCommand]
        public void SetAutoStart()
        {
            var exeName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            if (AutoStart)
            {
                if (!StartAutomaticallyCreate(exeName))
                {
                    this.ShowMessage("设置程序开机自启失败！", DialogType.Error);
                    AutoStart = false;
                }
            }
            else
            {
                if (!StartAutomaticallyDel(exeName))
                {
                    this.ShowMessage("取消程序开机自启失败！", DialogType.Error);
                    AutoStart = true;
                }
            }

            ConfigHelper.WriteConfigByBool("auto_start", AutoStart);
        }

        [RelayCommand]
        public void SetBackupPath()
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            BackupPath = dialog.SelectedPath;

            ConfigHelper.WriteConfigByString("backup_path", BackupPath);
        }

        [RelayCommand]
        public void SetDefaultCikuFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = UserFilePath;
            openFileDialog.Filter = "Yaml|*.dict.yaml";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.ShowDialog();
            string fullPath = openFileDialog.FileName;
            DefaultCikuFile = Path.GetFileName(fullPath);

            ConfigHelper.WriteConfigByString("default_ciku_file", DefaultCikuFile);
            WeakReferenceMessenger.Default.Send<string, string>("20", "ReLoadCikuData"); // todo: 参数设置成显示条数吧
        }

        [RelayCommand]
        public void SetUserCikuFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = UserFilePath;
            openFileDialog.Filter = "Yaml|*.dict.yaml";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.ShowDialog();
            string fullPath = openFileDialog.FileName;
            UserCikuFile = Path.GetFileName(fullPath);

            ConfigHelper.WriteConfigByString("user_ciku_file", UserCikuFile);
        }

        [RelayCommand]
        public void SwicthService()
        {
            try
            {
                if (ServiceIsRun)
                    ServiceHelper.RunService();
                else
                    ServiceHelper.KillService();
            }
            catch (Exception ex)
            {
                this.ShowMessage(ex.Message, DialogType.Warring);
            }
        }

        [RelayCommand]
        public async Task UpdateRimeWubiAsync()
        {
            // 获取当前五笔版本
            string wubi_type = ConfigHelper.ReadConfigByString("running_schema", "none");
            if (wubi_type == "none") return;

            // 先检测rime环境
            if (string.IsNullOrEmpty(GlobalValues.UserPath) || string.IsNullOrEmpty(GlobalValues.ProcessPath))
            {
                this.ShowMessage("未检测到 Rime 引擎的安装信息，请先安装 Rime 程序！", DialogType.Warring);
                return;
            }

            // 在配置前，先提示会将原有的方案覆盖
            bool? result = this.ShowAskMessage("请知悉：本次操作将在不删除配置的情况下更新方案配置信息", DialogType.Normal);
            if (result != true)
                return;

            if (isFromLocal)
            {
                // 从本地选择 rime-wubi.zip 文件
                var openFileDialog = new OpenFileDialog()
                {
                    Filter = "rime-wubi.zip (.zip)|*.zip"
                };

                var dialog_result = (bool)openFileDialog.ShowDialog();
                if (dialog_result)
                {
                    var rime_wubi_zip = openFileDialog.FileName;

                    // 将本地zip文件移动到指定目录
                    if (File.Exists(GlobalValues.WubiZipPath))
                        File.Delete(GlobalValues.WubiZipPath);
                    File.Copy(rime_wubi_zip, GlobalValues.WubiZipPath);
                }
                else
                {
                    return;
                }
            }

            // 停止服务
            ServiceHelper.KillService();

            // 异步加载 loading
            LodingView lodingView = new LodingView();
            App.Current.Dispatcher.BeginInvoke(() => { lodingView.ShowPop(); });
            await Task.Delay(1500);

            if (!isFromLocal)
            {
                // 删除从github下载的旧方案
                if (File.Exists(GlobalValues.WubiZipPath))
                    File.Delete(GlobalValues.WubiZipPath);

                // 从github下载方案
                var down_value = await DownLoadWubiSchemaAsync(GlobalValues.RimeWubiZipUrl, GlobalValues.WubiZipPath);
                if (!down_value)
                {
                    lodingView.ClosePop();
                    this.ShowMessage("网络情况不佳，无法从 Github 获取五笔方案资源", DialogType.Error);
                    return;
                }
            }

            await App.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, async () =>
            {
                try
                {
                    // 将下载好的方案zip解压
                    ZipHelper.DecompressZip(GlobalValues.WubiZipPath, GlobalValues.UserPath);

                    // 将解压的方案文件复制到用户目录下，然后删除
                    string wubi_file = GlobalValues.UserPath + "\\" + GlobalValues.WubiFileName;
                    if (Directory.Exists(wubi_file))
                        CopyDirectory(wubi_file, GlobalValues.UserPath);
                    else
                        throw new Exception($"找到不到路径：{wubi_file}");
                    Directory.Delete(wubi_file, true);

                    // 要点：更新操作时，将对应五笔版本的tables下的信息复制到用户目录下
                    string table_path = "";
                    if (wubi_type == "86") table_path = @$"{GlobalValues.UserPath}\tables\86";
                    else if (wubi_type == "98") table_path = @$"{GlobalValues.UserPath}\tables\98";
                    else table_path = @$"{GlobalValues.UserPath}\tables\06";
                    if (Directory.Exists(table_path))
                        CopyDirectory(table_path, GlobalValues.UserPath);
                    else
                        throw new Exception($"找到不到路径：{table_path}");

                    // 更新 wubi master 文本信息
                    string wubi_master_info = "";
                    wubi_master_info += "中书君标记文件，请勿删除！\n\r";
                    wubi_master_info += $"更新时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}\n\r";
                    wubi_master_info += $"五笔类型：{wubi_type}\n\r";
                    if (File.Exists(GlobalValues.UserPath + "\\" + GlobalValues.SchemaKey))
                        File.Delete(GlobalValues.UserPath + "\\" + GlobalValues.SchemaKey);
                    File.WriteAllText(GlobalValues.UserPath + "\\" + GlobalValues.SchemaKey, wubi_master_info);

                    lodingView.ClosePop();
                    this.ShowMessage("更新成功", DialogType.Success);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.Message, true);
                    this.ShowMessage($"更新失败: {ex.Message}", DialogType.Error);
                    return;
                }
                finally
                {
                    lodingView.ClosePop();
                    ServiceHelper.RunService();
                    await Task.Delay(500);
                    CmdHelper.RunCmd(GlobalValues.ProcessPath, "WeaselDeployer.exe /deploy");
                }
            });
        }

        [RelayCommand]
        public void WinStateLayout(object obj)
        {
            WinStateChecked = bool.Parse(obj?.ToString());
            string layoutStr = "left";
            if (WinStateChecked) layoutStr = "right";
            else layoutStr = "left";
            WeakReferenceMessenger.Default.Send<string, string>(layoutStr, "ChangeWinStateLayout");
        }

        private void ChangeQuickSpllType(object recipient, string message)
        {
            string type = message;
            switch (type)
            {
                case "86":
                    QuickSpllType86 = true;
                    QuickSpllType98 = false;
                    QuickSpllType06 = false;
                    break;

                case "98":
                    QuickSpllType86 = false;
                    QuickSpllType98 = true;
                    QuickSpllType06 = false;
                    break;

                case "06":
                    QuickSpllType86 = false;
                    QuickSpllType98 = false;
                    QuickSpllType06 = true;
                    break;

                default:
                    QuickSpllType86 = true;
                    QuickSpllType98 = false;
                    QuickSpllType06 = false;
                    break;
            }
            QuickSpellChange();
        }

        /// <summary>
        /// 从github下载五笔方案
        /// </summary>
        private async Task<bool> DownLoadWubiSchemaAsync(string zipUrl, string savePath)
        {
            try
            {
                // 使用HttpClient下载ZIP文件
                HttpResponseMessage response = await client.GetAsync(zipUrl);
                response.EnsureSuccessStatusCode();

                // 获取ZIP文件的字节流
                byte[] contentBytes = await response.Content.ReadAsByteArrayAsync();

                // 将字节流写入到本地ZIP文件
                File.WriteAllBytes(savePath, contentBytes);

                return true;
            }
            catch (HttpRequestException ex)
            {
                LogHelper.Error("从 github 下载五笔方案异常：" + ex.ToString());
                return false;
            }
        }

        private void InitLogBackList()
        {
            int[] backDays = new int[] { 5, 15, 30, 100 };
            foreach (int day in backDays)
            {
                LogBackModel backModel = new LogBackModel();
                backModel.Value = day.ToString();
                backModel.Text = day.ToString() + "天";
                LogBackList.Add(backModel);
            }
        }

        private void InitShiciInterval()
        {
            ShiciIntervalList.Add(new ShiciIntervalModel() { Id = 0, Value = "5分钟", Minutes = 5 });
            ShiciIntervalList.Add(new ShiciIntervalModel() { Id = 1, Value = "25分钟", Minutes = 25 });
            ShiciIntervalList.Add(new ShiciIntervalModel() { Id = 2, Value = "1小时", Minutes = 60 });
            ShiciIntervalList.Add(new ShiciIntervalModel() { Id = 3, Value = "1天", Minutes = 1440 });
        }

        private void InitThemes()
        {
            try
            {
                ObservableCollection<ThemeModel> themeModels = new ObservableCollection<ThemeModel>();
                var sourceThemes = new ResourceDictionary();
                sourceThemes.Source = new Uri("pack://application:,,,/WubiMaster;component/Themes/ThemeNames.xaml");
                foreach (string name in sourceThemes.Keys)
                {
                    string path = sourceThemes[name].ToString();
                    ThemeModel themeModel = new ThemeModel();
                    themeModel.Name = name;
                    themeModel.Value = path;
                    themeModels?.Add(themeModel);
                }
                ThemeList = themeModels.OrderBy(t => t.Name).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
        }

        private void LoadConfig()
        {
            // 加载用户目录配置
            UserFilePath = GlobalValues.UserPath;

            // 加载程序目录配置
            ProcessFilePath = GlobalValues.ProcessPath;

            // 加载默认词库
            DefaultCikuFile = ConfigHelper.ReadConfigByString("default_ciku_file");

            // 加载扩展词库
            UserCikuFile = ConfigHelper.ReadConfigByString("user_ciku_file");

            // 加载今日诗词更换时间
            string interval = ConfigHelper.ReadConfigByString("shici_interval", "25");
            var _list = ShiciIntervalList.Select(i => i.Minutes).ToList();
            if (_list.IndexOf(int.Parse(interval)) == -1) { ShiciIndex = 1; interval = "25"; }
            else ShiciIndex = _list.IndexOf(int.Parse(interval));
            WeakReferenceMessenger.Default.Send<string, string>(interval, "ChangeShiciInterval");

            // 加载日志备份时长
            string logBackDays = ConfigHelper.ReadConfigByString("log_back_days", "30");
            LogBackIndex = LogBackList.Select(l => l.Value).ToList().IndexOf(logBackDays);

            // 加载主题配置
            IsRandomThemes = ConfigHelper.ReadConfigByBool("is_random_themes", false);
            if (IsRandomThemes)
            {
                CobboxThemesEnable = false;
                int index = new Random().Next(ThemeList.Count);
                ChangeTheme(ThemeList[index].Value);
            }
            else
            {
                CobboxThemesEnable = true;
                string themeValue = ConfigHelper.ReadConfigByString("theme_value");
                if (!string.IsNullOrEmpty(themeValue)) ChangeTheme(themeValue);
                else ChangeTheme("DefultBlueTheme");
            }

            // 加载首页快速查询字根版本类型
            string quickSpllType = ConfigHelper.ReadConfigByString("quick_search_type");
            if (quickSpllType == "86")
                QuickSpllType86 = true;
            else if (quickSpllType == "98")
                QuickSpllType98 = true;
            else if (quickSpllType == "06")
                QuickSpllType06 = true;
            else
                QuickSpllType86 = true;

            // 加载方案备份目录
            BackupPath = ConfigHelper.ReadConfigByString("backup_path");

            // 加载工作方案版本
            UpdateWubiSchemaTip();

            // 加载winsate布局位置
            string layoutStr = ConfigHelper.ReadConfigByString("win_state_layout");
            if (layoutStr == "right") WinStateChecked = true;
            else WinStateChecked = false; ;

            // 加载守护进程状态
            DaemonIsRun = ConfigHelper.ReadConfigByBool("daemon_state");
            ChagedDaemonState();

            // 加载插件名称
            string plugName = ConfigHelper.ReadConfigByString("plugin_name");
            PluginIndex = string.IsNullOrEmpty(plugName) ? 0 : PluginsList.IndexOf(plugName);

            // 加载是否自动启动
            AutoStart = ConfigHelper.ReadConfigByBool("auto_start");
        }

        #region 开机自启

        /// <summary>
        /// 开机自启创建
        /// </summary>
        /// <param name="exeName">程序名称</param>
        /// <returns></returns>
        public bool StartAutomaticallyCreate(string exeName)
        {
            try
            {
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + exeName + ".lnk");
                //设置快捷方式的目标所在的位置(源程序完整路径)
                shortcut.TargetPath = System.Windows.Forms.Application.ExecutablePath;
                //应用程序的工作目录
                //当用户没有指定一个具体的目录时，快捷方式的目标应用程序将使用该属性所指定的目录来装载或保存文件。
                shortcut.WorkingDirectory = System.Environment.CurrentDirectory;
                //目标应用程序窗口类型(1.Normal window普通窗口,3.Maximized最大化窗口,7.Minimized最小化)
                shortcut.WindowStyle = 1;
                //快捷方式的描述
                shortcut.Description = exeName + "_Ink";
                //设置快捷键(如果有必要的话.)
                //shortcut.Hotkey = "CTRL+ALT+D";
                shortcut.Save();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 开机自启删除
        /// </summary>
        /// <param name="exeName">程序名称</param>
        /// <returns></returns>
        public bool StartAutomaticallyDel(string exeName)
        {
            try
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + exeName + ".lnk");
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
            return false;
        }

        #endregion 开机自启

        private void UpdateWubiSchemaTip()
        {
            try
            {
                string wubi_master_key = GlobalValues.UserPath + GlobalValues.SchemaKey;
                bool hasKey = File.Exists(wubi_master_key);
                if (hasKey)
                {
                    WubiSchemaTip = "已初始化五笔引擎";
                    CanUpdateRimeWubi = true;
                }
                else
                {
                    WubiSchemaTip = "五笔引擎未初始化";
                    CanUpdateRimeWubi = false;
                    WeakReferenceMessenger.Default.Send<string, string>("", "ChangeShcemaState");
                }
            }
            catch (Exception ex)
            {
                WubiSchemaTip = "五笔引擎未初始化";
                WeakReferenceMessenger.Default.Send<string, string>("", "ChangeShcemaState");
                LogHelper.Error(ex.Message);
            }
        }
    }
}