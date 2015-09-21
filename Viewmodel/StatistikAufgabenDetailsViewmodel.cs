using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mathe1.Common;

namespace Mathe1.Viewmodel
{
    class StatistikAufgabenDetailsViewmodel: ViewmodelBase
    {
        public IList<IAufgabe> Aufgaben { get; set; }

        public StatistikAufgabenDetailsViewmodel(IList<IAufgabe> aufgaben)
        {
            Aufgaben = aufgaben;
        }
    }
}
