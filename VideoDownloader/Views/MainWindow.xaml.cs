using CommunityToolkit.Mvvm.DependencyInjection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoDownloader.ViewModels;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace VideoDownloader.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            Ioc.Default.GetRequiredService<IContentDialogService>().SetContentPresenter(RootContentDialog);
            Ioc.Default.GetRequiredService<ISnackbarService>().SetSnackbarPresenter(SnackbarPresenter);

            DataContext = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        }

        public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;
    }
}