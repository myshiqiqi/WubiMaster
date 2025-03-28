namespace WubiMaster.ViewModels;

public partial class LexiconViewModel : ObservableObject
{
    private List<CikuModel> CikuAllList;

    [ObservableProperty]
    private ObservableCollection<CikuModel> cikuList;

    [ObservableProperty]
    private bool pageControlEnable;

    [ObservableProperty]
    private int pageControlIndex;

    [ObservableProperty]
    private int pageCount;

    [ObservableProperty]
    private int pageNumber = 1;

    [ObservableProperty]
    private int totalPageCount;

    public LexiconViewModel()
    {
        CikuAllList = new List<CikuModel>();
        WeakReferenceMessenger.Default.Register<string, string>(this, "ReLoadCikuData", ReLoadCikuData);

        LoadCiKus();
        LoadCikuData();
        InitPageControl();
    }

    /// <summary>
    /// 加载词库下载选项
    /// </summary>
    public void LoadCiKus()
    {
        // 默认词库路径
        var default_ciku_path = GlobalValues.UserPath + @"\wubi.dict.yaml";
        // 扩展词库路径
        var extend_ciku_path = GlobalValues.UserPath + @"\wubi.extended.dict.yaml";


        if (!File.Exists(extend_ciku_path))
        {
            this.ShowMessage($"找不到【wubi.extended.dict.yaml】扩展词库文件", DialogType.Error);
            return;
        }

        // # 读取词库名称列表
        var cikuNames = new List<(string, string)>();
        cikuNames.Add(("默认词库", default_ciku_path));
        cikuNames.Add(("扩展词库", extend_ciku_path));
        cikuNames.Add(("", ""));
    }

    private void InitPageControl()
    {
        try
        {
            if (CikuAllList.Count <= 0) return;

            PageControlIndex = ConfigHelper.ReadConfigByInt("page_control_index", 1);
            PageNumber = 1;
            PageCount = ConfigHelper.ReadConfigByInt("page_count", 20);
            TotalPageCount = (CikuAllList.Count / PageCount) + ((CikuAllList.Count % PageCount) == 0 ? 0 : 1);
            PageControlEnable = TotalPageCount > 0 ? true : false;

            ObservableCollection<CikuModel> temp_list = new ObservableCollection<CikuModel>();
            for (int i = 0; i < PageCount; i++)
            {
                CikuModel c = CikuAllList[i];
                temp_list.Add(c);
            }

            CikuList = temp_list;
        }
        catch (Exception ex)
        {
            this.ShowMessage($"无法加载词库信息，请检查配置信息是否正确！", DialogType.Warring);
            LogHelper.Error(ex.Message);
        }
    }

    private void LoadCikuData()
    {
        string defaultCikuFile = ConfigHelper.ReadConfigByString("default_ciku_file");
        if (string.IsNullOrEmpty(defaultCikuFile))
            return;

        App.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                string defaultCikuPath = ConfigHelper.ReadConfigByString("user_file_path") + "\\" + defaultCikuFile;
                if (!File.Exists(defaultCikuPath)) return;

                // 按行读，找到 yaml 的结尾符 ...
                string yamlText = "";
                int yamlEndLine = -1;
                ConcurrentQueue<CikuModel> wubiDictList = new ConcurrentQueue<CikuModel>();

                string[] wubiDictStrs = File.ReadAllLines(defaultCikuPath);

                for (int i = 0; i < wubiDictStrs.Length; i++)
                {
                    yamlEndLine = i + 1;
                    if (wubiDictStrs[i] == "...")
                        break;
                    yamlText += wubiDictStrs[i] + "\r\n";
                }

                Parallel.For(yamlEndLine, wubiDictStrs.Length, (j) =>
                {
                    string dataStr = wubiDictStrs[j];
                    string[] dataInfoArrsy = dataStr.Split("\t").Where(d => d != "").ToArray();

                    string[] values = new string[] { "", "", "", "", "默认" };
                    for (int i = 0; i < dataInfoArrsy.Length; i++)
                        values[i] = dataInfoArrsy[i];
                    CikuModel model = new CikuModel();
                    model.Id = j - yamlEndLine;
                    model.Text = values[0];
                    model.Code = values[1];
                    model.Weight = values[2];
                    model.Stem = values[3];
                    model.Group = values[4];

                    wubiDictList.Enqueue(model);
                });

                var _list = wubiDictList.OrderBy(d => d.Id).ToList();
                CikuAllList = _list;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
        });
    }

    private void ReLoadCikuData(object recipient, string message)
    {
        CikuList?.Clear();
        LoadCikuData();
        InitPageControl();
    }

    [RelayCommand]
    public void ClosePopView(object obj)
    {
        Window popView = obj as Window;

        WeakReferenceMessenger.Default.Send<string, string>("true", "ShowMaskLayer");

        popView?.ClosePop();
    }

    [RelayCommand]
    public void CreateWords(object obj)
    {
        Window mainWindow = App.Current.MainWindow;

        WeakReferenceMessenger.Default.Send<string, string>("true", "ShowMaskLayer");

        CreateWordsView createWordsView = new CreateWordsView();
        createWordsView.Owner = mainWindow;
        createWordsView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        createWordsView.ShowPop();
    }

    [RelayCommand]
    public void PageControlSelectionChanged(object obj)
    {
        ComboBoxItem item = (ComboBoxItem)obj;
        var content = item.Content;
        if (content != null)
        {
            PageCount = int.Parse(content.ToString());

            ConfigHelper.WriteConfigByInt("page_control_index", PageControlIndex);
            ConfigHelper.WriteConfigByInt("page_count", PageCount);

            CikuList?.Clear();
            LoadCikuData();
            InitPageControl();
        }
    }

    [RelayCommand]
    public void ToFirstPage(object obj)
    {
        App.Current.Dispatcher.BeginInvoke(() =>
        {
            PageNumber = 0;
            var _list = CikuAllList.AsParallel().Where(c => c.Id >= PageCount * PageNumber && c.Id < PageCount * (PageNumber + 1)).ToList();
            ObservableCollection<CikuModel> temp_list = new ObservableCollection<CikuModel>();
            foreach (var item in _list)
                temp_list.Add(item);

            PageNumber++;
            CikuList = temp_list;
        });
    }

    [RelayCommand]
    public void ToLastPage(object obj)
    {
        App.Current.Dispatcher.BeginInvoke(() =>
        {
            PageNumber = TotalPageCount;
            var _list = CikuAllList.AsParallel().Where(c => c.Id >= PageCount * (PageNumber - 1) && c.Id < PageCount * PageNumber).ToList();
            ObservableCollection<CikuModel> temp_list = new ObservableCollection<CikuModel>();
            foreach (var item in _list)
                temp_list.Add(item);

            CikuList = temp_list;
        });
    }

    [RelayCommand]
    public void ToNextPage(object obj)
    {
        App.Current.Dispatcher.BeginInvoke(() =>
        {
            if (PageNumber == TotalPageCount) { this.ShowMessage("没有更多内容了"); return; }
            var _list = CikuAllList.AsParallel().Where(c => c.Id >= PageCount * PageNumber && c.Id < PageCount * (PageNumber + 1)).ToList();
            ObservableCollection<CikuModel> temp_list = new ObservableCollection<CikuModel>();
            foreach (var item in _list)
                temp_list.Add(item);

            CikuList = temp_list;
            PageNumber++;
        });
    }

    [RelayCommand]
    public void ToPreviousPage(object obj)
    {
        App.Current.Dispatcher.BeginInvoke(() =>
        {
            if (PageNumber == 1) { this.ShowMessage("没有更多内容了"); return; }
            PageNumber--;
            var _list = CikuAllList.AsParallel().Where(c => c.Id >= PageCount * (PageNumber - 1) && c.Id < PageCount * PageNumber).ToList();
            ObservableCollection<CikuModel> temp_list = new ObservableCollection<CikuModel>();
            foreach (var item in _list)
                temp_list.Add(item);

            CikuList = temp_list;
        });
    }

    [RelayCommand]
    private void AddCustomWords(object? obj)
    {
        MessageBox.Show("hello");
    }
}