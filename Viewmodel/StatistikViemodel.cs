using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Mathe1.Common;

namespace Mathe1.Viewmodel
{
    public class StatistikViemodel : ViewmodelBase
    {
        private readonly Lazy<DelegateCommand<StatistikItem>> _lazyDeleteStatistikEintragCommand; 
        public StatistikViemodel()
        {
            Auswertung = new ObservableCollection<StatistikItem>(StatistikReader.Read());
            _lazyDeleteStatistikEintragCommand = new Lazy<DelegateCommand<StatistikItem>>(()=> new DelegateCommand<StatistikItem>(DeleteExecute, CanDeleteExecute));
        }

        public ObservableCollection<StatistikItem> Auswertung { get; private set; }

        public void Add(StatistikItem auswertung)
        {
            Auswertung.Add(auswertung);
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
            StatistikWriter.Write(new Statistik(Auswertung.ToList()));
        }
    }
}
