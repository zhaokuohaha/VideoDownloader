using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using VideoDownloader.ViewModels;

namespace VideoDownloader
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;

        public App()
        {
            Ioc.Default.ConfigureServices(ConfigureServices());
           InitializeComponent();
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            RegisterViewModels(services);
            return services.BuildServiceProvider();
        }

        private static void RegisterViewModels(ServiceCollection services)
        {
            services.AddTransient<MainWindowViewModel>();
        }
    }

}
