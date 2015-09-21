using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mathe1.Common;

namespace Mathe1.Viewmodel
{
    class SubtraktionModul: ModulBase
    {
        public SubtraktionModul() : base(Operationen.Subtraktion, Properties.Settings.Default.GradSubtraktion)
        {
        }

        public override string Displayname
        {
            get { return "Subtraktion (-)"; }
        }

        protected override void SetSchwierigkeitToSettings(int value)
        {
            Mathe1.Properties.Settings.Default.GradSubtraktion = value;
        }
    }
}
