namespace WubiMaster.ViewModels;

public partial class PopViewModel : ObservableObject
{
    [ObservableProperty]
    private MessageBoxControl messageBox;

    [RelayCommand]
    public void CancelPop(object obj)
    {
        if (obj == null) return;
        obj.Cancel(obj);
    }

    [RelayCommand]
    public void Close(object obj)
    {
        //this.Cancel(obj);

        if (obj == null) return;
        WeakReferenceMessenger.Default.Send<string, string>("false", "ShowMaskLayer");

        try
        {
            Window w = (Window)obj;
            w.Close();
        }
        catch (Exception ex)
        {
            LogHelper.Warn("提示窗口未正常关闭");
            LogHelper.Error(ex.Message);
        }
    }

    [RelayCommand]
    public void ConfirmPop(object obj)
    {
        if (obj == null) return;
        obj.Confirm(obj);
    }
}