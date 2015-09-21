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
    abstract class ModulBase : ViewmodelBase, IModule
    {
        public event EventHandler<StatistikEventArgs> StatistikEvent;

        private readonly Operationen _typ;
        private readonly Lazy<DelegateCommand> _lazyNeueAufgabenCommand; 
        private readonly Lazy<DelegateCommand> _lazyPrüfenCommand;
        private int _schwierigkeit;

        protected ModulBase(Operationen typ, int schwierigkeit)
        {
            _typ = typ;
            _lazyNeueAufgabenCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(NeueAufgabenCommandExecute, CanNeueAufgabenCommandExecute));
            _lazyPrüfenCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(PrüfenCommandExecute, CanPrüfenCommandExecute));

            Aufgaben = new ObservableCollection<IAufgabe>();
            Schwierigkeit = schwierigkeit;

            CreateAufgaben();
        }

        public abstract string Displayname { get; }

        public ObservableCollection<IAufgabe> Aufgaben { get; private set; }

        public int Schwierigkeit
        {
            get { return _schwierigkeit; }
            set
            {
                if(_schwierigkeit==value)
                    return;

                _schwierigkeit = value;

                OnPropertyChanged();

                SetSchwierigkeitToSettings(value);
                CreateAufgaben();
            }
        }

        protected abstract void SetSchwierigkeitToSettings(int value);

        public ICommand NeueAufgabenCommand { get { return _lazyNeueAufgabenCommand.Value; } }


        private bool CanNeueAufgabenCommandExecute()
        {
            return Aufgaben.All(x => x.ObLocked);
        }

        private void NeueAufgabenCommandExecute()
        {
            if (!CanNeueAufgabenCommandExecute())
                return;

            CreateAufgaben();
        }

        public ICommand PrüfenCommand { get { return _lazyPrüfenCommand.Value; } }

        private bool CanPrüfenCommandExecute()
        {
            return !Aufgaben.All(x => x.ObLocked) && Aufgaben.All(x => x.Operator1.HasValue && x.Operator2.HasValue && x.Result.HasValue);
        }

        private void PrüfenCommandExecute()
        {
            if (!CanPrüfenCommandExecute())
                return;

            foreach (var aufgabe in Aufgaben)
            {
                aufgabe.ValidateResult();
            }

            if (Aufgaben.All(x => x.ObLocked))
            {
                OnStatistikEvent(new StatistikItem(Aufgaben));
            }
        }

        protected void CreateAufgaben()
        {
            Aufgaben.Clear();

            for (int i = 0; i < 10; i++)
            {
                var aufgabe = new AufgabeViewmodel(_typ, Schwierigkeit);

                if (Aufgaben.Any(x =>x.Operator1 == aufgabe.Operator1 && x.Operator2 == aufgabe.Operator2 &&x.Result == aufgabe.Result))
                {
                    i--;
                    continue;
                }

                Aufgaben.Add(aufgabe);
            }
        }

        protected virtual void OnStatistikEvent(StatistikItem auswertung)
        {
            var handler = StatistikEvent;
            if (handler != null) handler(this, new StatistikEventArgs(auswertung));
        }
    }
}
