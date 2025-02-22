namespace WubiMaster.ViewModels;

public class ViewModelLocator
{
    public ViewModelLocator()
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<MainViewModel>()
                .AddSingleton<AttributeViewModel>()
                .AddSingleton<CreateWordsViewModel>()
                .AddSingleton<EtymonKeyViewModel>()
                .AddSingleton<EtymonViewModel>()
                .AddSingleton<HomeViewModel>()
                .AddSingleton<LexiconViewModel>()
                .AddSingleton<MyColorsViewModel>()
                .AddSingleton<PopViewModel>()
                .AddSingleton<SettingsViewModel>()
                .AddSingleton<ThemeViewModel>()
                .BuildServiceProvider());
    }

    public MainViewModel? MainViewModel => Ioc.Default.GetService<MainViewModel>();
    public AttributeViewModel? AttributeViewModel => Ioc.Default.GetService<AttributeViewModel>();
    public CreateWordsViewModel? CreateWordsViewModel => Ioc.Default.GetService<CreateWordsViewModel>();
    public EtymonKeyViewModel? EtymonKeyViewModel => Ioc.Default.GetService<EtymonKeyViewModel>();
    public EtymonViewModel? EtymonViewModel => Ioc.Default.GetService<EtymonViewModel>();
    public HomeViewModel? HomeViewModel => Ioc.Default.GetService<HomeViewModel>();
    public LexiconViewModel? LexiconViewModel => Ioc.Default.GetService<LexiconViewModel>();
    public MyColorsViewModel? MyColorsViewModel => Ioc.Default.GetService<MyColorsViewModel>();
    public PopViewModel? PopViewModel => Ioc.Default.GetService<PopViewModel>();
    public SettingsViewModel? SettingsViewModel => Ioc.Default.GetService<SettingsViewModel>();
    public ThemeViewModel? ThemeViewModel => Ioc.Default.GetService<ThemeViewModel>();
}
