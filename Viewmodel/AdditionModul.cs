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
    class AdditionModul : ModulBase
    {
        public AdditionModul() : base(Operationen.Addition,Properties.Settings.Default.GradAddition)
        {
        }

        public override string Displayname
        {
            get { return "Addition (+)"; }
        }

        protected override void SetSchwierigkeitToSettings(int value)
        {
            Mathe1.Properties.Settings.Default.GradAddition = value;
        }
    }
}
