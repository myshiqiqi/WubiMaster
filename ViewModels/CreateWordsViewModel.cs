namespace WubiMaster.ViewModels;

public partial class CreateWordsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<string> cikuList;

    [ObservableProperty]
    private ObservableCollection<string> containsWords;

    [ObservableProperty]
    private string newCode;

    [ObservableProperty]
    private string newWord;

    public CreateWordsViewModel()
    {
    }

    private void CreateNewCode()
    {
    }

    private void GetCiKuList()
    {
    }

    private void GetContainsWords()
    {
    }

    [RelayCommand]
    public void AddNewWord()
    {
    }

    [RelayCommand]
    public void AddNextNewWord()
    {
    }

    [RelayCommand]
    public void CloseWindow(object obj)
    {
    }
}