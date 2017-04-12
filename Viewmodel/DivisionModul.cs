using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mathe1.Common;
using Mathe1.Properties;

namespace Mathe1.Viewmodel
{
    class DivisionModul : ModulBase
    {
        public DivisionModul() : base(Operationen.Division,Settings.Default.GradDivision)
        {
        }

        public override string Displayname
        {
            get { return "Division (:)"; }
        }

        protected override void SetSchwierigkeitToSettings(int value)
        {
            Settings.Default.GradDivision = value;
        }
    }
}
