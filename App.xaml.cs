using System;
using System.Collections.Generic;
using System.Windows;
using Mathe1.Common;
using Mathe1.Properties;
using Microsoft.Shell;

namespace Mathe1
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Helper.SetOnStartUpSettings();

            base.OnStartup(e);

            this.Exit += (sender, args) =>
            {
                Settings.Default.Save();
            };

            this.DispatcherUnhandledException += (sender, args) => MessageBox.Show(args.Exception.Message);
        }

        private const string Unique = "MatheNurEinmalStarten!";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }


        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // Bring window to foreground
            if (this.MainWindow.WindowState == WindowState.Minimized)
            {
                this.MainWindow.WindowState = WindowState.Normal;
            }

            this.MainWindow.Activate();

            return true;
        }
    }
}
