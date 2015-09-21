using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mathe1.Common
{
    public interface IModule
    {
        string Displayname { get; }

        int Schwierigkeit { get; set; }

        ObservableCollection<IAufgabe> Aufgaben { get; }

        ICommand NeueAufgabenCommand { get; }

        ICommand PrüfenCommand { get; }
    }
}
