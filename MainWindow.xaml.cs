using MahApps.Metro.Controls;
using System.Windows;

namespace Mathe1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewmodel _data = new MainWindowViewmodel();
        public MainWindow()
        {
            DataContext = _data;
            InitializeComponent();
            
        }


    }
}
