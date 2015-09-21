using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Mathe1.Common
{
    public class WpfUIDialogWindowService
    {
        #region Implementation of IUIDialogWindowService

        /// <summary>
        ///  Zeigt ein Dialog an.
        /// </summary>
        /// <param name="titel">Titel für den Dialog</param>
        /// <param name="datacontext">DataContext für den Dialog</param>
        /// <returns>true wenn DialogResult=true, ansonsten false</returns>
        public bool? ShowDialog(string titel, object datacontext)
        {
            return DoShowDialog(titel, datacontext, null, "", "", 0, 0);
        }

        /// <summary>
        ///  Zeigt ein Dialog an.
        /// </summary>
        /// <param name="titel">Titel für den Dialog</param>
        /// <param name="datacontext">DataContext für den Dialog</param>
        /// <param name="minHeigth">Minimum Height</param>
        /// <param name="minWidth">Minimum Width</param>
        /// <param name="maxHeigth">Maximum Height</param>
        /// <param name="maxWidth">Maximum Width</param>
        /// <param name="showInTaskbar"></param>
        /// <param name="icon"></param>
        /// <returns>true wenn DialogResult=true, ansonsten false</returns>
        public bool? ShowDialog(string titel, object datacontext,
            double minHeigth = 0, double minWidth = 0, double maxHeigth = double.PositiveInfinity, double maxWidth = double.PositiveInfinity,
            bool showInTaskbar = false, ImageSource icon = null)
        {
            return DoShowDialog(titel, datacontext, null, "", "", minHeigth, minWidth, maxHeigth, maxWidth, showInTaskbar, icon);
        }

        /// <summary>
        /// Zeigt ein Dialog an
        /// </summary>
        /// <param name="titel">Titel für den Dialog<</param>
        /// <param name="datacontext">DataContext für den Dialog</param>
        /// <param name="settings">ApplicationsSetting für Height and Width</param>
        /// <param name="pathHeigthSetting">Name für Height Setting</param>
        /// <param name="pathWidthSetting">Name für Width Setting</param>
        /// <param name="minHeigth">Minimum Height</param>
        /// <param name="minWidth">Minimum Width</param>
        /// <param name="maxHeigth">Maximum Height</param>
        /// <param name="maxWidth">Maximum Width</param>
        /// <param name="showInTaskbar"></param>
        /// <param name="icon"></param>
        /// <returns>true wenn DialogResult=true, ansonsten false</returns>
        public bool? ShowDialog(string titel, object datacontext,
            ApplicationSettingsBase settings, string pathHeigthSetting, string pathWidthSetting,
            double minHeigth, double minWidth, double maxHeigth = double.PositiveInfinity, double maxWidth = double.PositiveInfinity,
            bool showInTaskbar = false, ImageSource icon = null)
        {
            return DoShowDialog(titel, datacontext, settings, pathHeigthSetting, pathWidthSetting, minHeigth, minWidth,
                maxHeigth, maxWidth, showInTaskbar, icon);
        }

        #endregion

        private bool? DoShowDialog(string titel, object datacontext,
            ApplicationSettingsBase settings, string pathHeigthSetting, string pathWidthSetting,
            double minHeigth, double minWidth, double maxHeigth = double.PositiveInfinity,
            double maxWidth = double.PositiveInfinity,
            bool showInTaskbar = false, ImageSource icon = null)
        {
            var win = new DialogWindow { Title = titel, DataContext = datacontext };

            win.Owner = Application.Current.MainWindow;
            win.ShowInTaskbar = showInTaskbar;
            win.MinHeight = minHeigth;
            win.MinWidth = minWidth;
            win.MaxHeight = maxHeigth;
            win.MaxWidth = maxWidth;

            if (icon != null)
            {
                win.Icon = icon;
            }
            else
            {
                var mainIcon = Application.Current.MainWindow.Icon;
                if (mainIcon != null)
                    win.Icon = mainIcon;
            }

            try
            {
                if (settings != null)
                {
                    win.SizeToContent = SizeToContent.Manual;

                    var height = settings[pathHeigthSetting];
                    var width = settings[pathWidthSetting];

                    BindingOperations.SetBinding(win, FrameworkElement.HeightProperty, new Binding(pathHeigthSetting) { Source = settings, Mode = BindingMode.TwoWay });
                    BindingOperations.SetBinding(win, FrameworkElement.WidthProperty, new Binding(pathWidthSetting) { Source = settings, Mode = BindingMode.TwoWay });

                    win.Height = (double)height;
                    win.Width = (double)width;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                win.SizeToContent = SizeToContent.WidthAndHeight;
            }

            return win.ShowDialog();
        }
    }
}
