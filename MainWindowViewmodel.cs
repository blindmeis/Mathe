using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Mathe1.Common;
using Mathe1.Viewmodel;

namespace Mathe1
{
    class MainWindowViewmodel : ViewmodelBase
    {
        private Lazy<DelegateCommand> _lazyStatistikCommand;
        private WpfUIDialogWindowService _dialogService;

        public MainWindowViewmodel()
        {
            Titel = ConfigurationManager.AppSettings["Titel"];   
            Module = new ObservableCollection<IModule>();

            var add = new AdditionModul();
            add.StatistikEvent += (sender, args) => Auswertung.Add(args.Auswertung);

            var sub = new SubtraktionModul();
            sub.StatistikEvent += (sender, args) => Auswertung.Add(args.Auswertung);

            Module.Add(add);
            Module.Add(sub);

            _dialogService = new WpfUIDialogWindowService();
            Auswertung = new StatistikViemodel(_dialogService);

            _lazyStatistikCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(StatistikCommandExecute, CanStatistikCommandExecute));

        }

        public string Titel { get; private set; }

        public ObservableCollection<IModule> Module { get; private set; }

        public StatistikViemodel Auswertung { get; private set; }

        public ICommand StatistikCommand { get { return _lazyStatistikCommand.Value; } }

        private bool CanStatistikCommandExecute()
        {
            return true;
        }

        private void StatistikCommandExecute()
        {
            if(!CanStatistikCommandExecute())
                return;

            _dialogService.ShowDialog("Auswertung", Auswertung, Properties.Settings.Default, "HeightStatistik", "WidthStatistik", 400, 600, 800, 700);
        }

    }
}
