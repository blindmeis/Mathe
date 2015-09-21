using Mathe1.Common;
using Mathe1.Properties;

namespace Mathe1.Viewmodel
{
    class AdditionModul : ModulBase
    {
        public AdditionModul() : base(Operationen.Addition,Settings.Default.GradAddition)
        {
        }

        public override string Displayname
        {
            get { return "Addition (+)"; }
        }

        protected override void SetSchwierigkeitToSettings(int value)
        {
            Settings.Default.GradAddition = value;
        }
    }
}
