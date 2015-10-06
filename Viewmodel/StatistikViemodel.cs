using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Mathe1.Common;

namespace Mathe1.Viewmodel
{
    public class StatistikViemodel : ViewmodelBase
    {
        private readonly WpfUIDialogWindowService _dialogService;
        private readonly Lazy<DelegateCommand<StatistikItem>> _lazyDeleteStatistikEintragCommand; 
        private readonly Lazy<DelegateCommand<StatistikItem>> _lazyShowDetailsStatistikEintragCommand;
        private readonly ICollectionView _view; 

        public StatistikViemodel(WpfUIDialogWindowService dialogService)
        {
            Auswertung = new ObservableCollection<StatistikItem>(StatistikReader.Read());
            _view = CollectionViewSource.GetDefaultView(Auswertung);
            _view.SortDescriptions.Add(new SortDescription("Timestamp",ListSortDirection.Descending));

            _dialogService = dialogService;
            _lazyDeleteStatistikEintragCommand = new Lazy<DelegateCommand<StatistikItem>>(()=> new DelegateCommand<StatistikItem>(DeleteExecute, CanDeleteExecute));
            _lazyShowDetailsStatistikEintragCommand = new Lazy<DelegateCommand<StatistikItem>>(()=> new DelegateCommand<StatistikItem>(ShowDetailsCommandExecute, CanShowDetailsCommandExecute));
        }


        public ObservableCollection<StatistikItem> Auswertung { get; private set; }

        public void Add(StatistikItem auswertung)
        {
            Auswertung.Add(auswertung);
            _view.Refresh();
            StatistikWriter.Write(new Statistik(Auswertung.ToList()));
        }

        public ICommand DeleteStatistikEintragCommand { get { return _lazyDeleteStatistikEintragCommand.Value; } }

        private bool CanDeleteExecute(StatistikItem arg)
        {
            return arg != null;
        }

        private void DeleteExecute(StatistikItem obj)
        {
            if (!CanDeleteExecute(obj))
                return;

            Auswertung.Remove(obj);
            _view.Refresh();
            StatistikWriter.Write(new Statistik(Auswertung.ToList()));
        }

        public ICommand ShowDetailsCommand { get { return _lazyShowDetailsStatistikEintragCommand.Value; } }

        private bool CanShowDetailsCommandExecute(StatistikItem arg)
        {
            return arg != null;
        }

        private void ShowDetailsCommandExecute(StatistikItem obj)
        {
            if (!CanShowDetailsCommandExecute(obj))
                return;

            var vm = new StatistikAufgabenDetailsViewmodel(obj.Aufgaben);
            _dialogService.ShowDialog("Details", vm);
        }
    }
}
