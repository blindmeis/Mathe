using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using MahApps.Metro.Controls;

namespace Mathe1.Common
{
    /// <summary>
    /// Interaktionslogik für DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : MetroWindow
    {
        //Merken wenn Window geschlossen wurde, damit kein DialogResult mehr gesetzt wird
        private bool _isClosed = false;

        public DialogWindow()
        {
            InitializeComponent();
            this.DialogPresenter.DataContextChanged += DialogPresenterDataContextChanged;
            this.Closed += DialogWindowClosed;
        }

        void DialogWindowClosed(object sender, EventArgs e)
        {
            this._isClosed = true;
        }

        private void DialogPresenterDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var d = e.NewValue as IDialogResultVMHelper;

            if (d == null)
                return;

            d.RequestCloseDialog += new EventHandler<RequestCloseDialogEventArgs>(DialogResultTrueEvent).MakeWeak(eh => d.RequestCloseDialog -= eh); ;
        }

        private void DialogResultTrueEvent(object sender, RequestCloseDialogEventArgs eventargs)
        {
            //Wichtig damit für ein geschlossenes Window kein DialogResult mehr gesetzt wird
            //GC räumt Window irgendwann weg und durch MakeWeak fliegt es auch beim IDialogResultVMHelper raus
            if(_isClosed) return;

            this.DialogResult = eventargs.DialogResult;
        }
    }
}
