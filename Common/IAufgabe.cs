using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mathe1.Viewmodel;

namespace Mathe1.Common
{
    public interface IAufgabe
    {
        int? Operator1 { get; set; }

        Operationen Operation { get; set; }

        int? Operator2 { get; set; }

        int? Result { get; set; }

        int? LockResult { get; set; }

        void ValidateResult();

        bool? ObSuccess { get; }

        bool ObLocked { get; }

        bool ObFalsch { get; }

        ObservableCollection<Versuche> VersucheCounter { get; }

        int MaxVersuche { get; }


        /// <summary>
        /// 0 = Einfach
        /// 1 = Normal
        /// 2 = Schwer
        /// </summary>
        int Schwierigkeit { get; }
    }
       
}
