using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mathe1.Common;
using Mathe1.Properties;

namespace Mathe1.Viewmodel
{
    class MultiplikationModul : ModulBase
    {
        public MultiplikationModul() : base(Operationen.Multiplikation,Settings.Default.GradMultiplikation)
        {
        }

        public override string Displayname
        {
            get { return "Multiplikation (x)"; }
        }

        protected override void SetSchwierigkeitToSettings(int value)
        {
            Settings.Default.GradMultiplikation = value;
        }
    }
}

