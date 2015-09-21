using System;

namespace Mathe1.Common
{
    public class RequestCloseDialogEventArgs : EventArgs
    {
        public RequestCloseDialogEventArgs(bool dialogresult)
        {
            this.DialogResult = dialogresult;
        }

        public bool DialogResult
        {
            get; set;
        }
    }

    public interface IDialogResultVMHelper
    {
        event EventHandler<RequestCloseDialogEventArgs> RequestCloseDialog;
    }
}
