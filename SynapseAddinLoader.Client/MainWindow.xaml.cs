using System.Windows;

namespace SynapseAddinLoader.Client
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(string windowTitle)
        {
            InitializeComponent();
            Title = windowTitle;
            DataContext = new MainWindowViewModel();
        }
    }
}
