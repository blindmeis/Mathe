using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Mathe1.Common;
using Mathe1.Viewmodel;

namespace Mathe1
{
    class MainWindowViewmodel : ViewmodelBase
    {
        private Lazy<DelegateCommand> _lazyStatistikCommand;
        private WpfUIDialogWindowService _dialogService;

        public MainWindowViewmodel()
        {
            Titel = ConfigurationManager.AppSettings["Titel"];   
            Module = new ObservableCollection<IModule>();

            var add = new AdditionModul();
            add.StatistikEvent += (sender, args) => Auswertung.Add(args.Auswertung);

            var sub = new SubtraktionModul();
            sub.StatistikEvent += (sender, args) => Auswertung.Add(args.Auswertung);

            Module.Add(add);
            Module.Add(sub);

            Auswertung = new StatistikViemodel();

            _lazyStatistikCommand = new Lazy<DelegateCommand>(() => new DelegateCommand(StatistikCommandExecute, CanStatistikCommandExecute));
            _dialogService = new WpfUIDialogWindowService();
#if DEBUG
            //var tmp = new ResourceDictionary(){Source = new Uri(@"..\..\Resources\Icons.xaml",UriKind.RelativeOrAbsolute)};

            //foreach (var key in tmp.Keys)
            //{
            //    var canvas = (Canvas)tmp[key];

            //   CreateSaveBitmap(canvas, @"..\..\..\..\Images\" + key + ".png");
            //}
#endif
        }

        public string Titel { get; private set; }

        public ObservableCollection<IModule> Module { get; private set; }

        public StatistikViemodel Auswertung { get; private set; }

        public ICommand StatistikCommand { get { return _lazyStatistikCommand.Value; } }

        private bool CanStatistikCommandExecute()
        {
            return true;
        }

        private void StatistikCommandExecute()
        {
            if(!CanStatistikCommandExecute())
                return;

            _dialogService.ShowDialog("Auswertung", Auswertung, Properties.Settings.Default, "HeightStatistik", "WidthStatistik", 400, 600, 800, 600);
        }

        private void CreateSaveBitmap(Canvas canvas, string filename)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
             (int)canvas.Width, (int)canvas.Height,
             96d, 96d, PixelFormats.Pbgra32);
            // needed otherwise the image output is black
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));

            renderBitmap.Render(canvas);

            //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream file = File.Create(filename))
            {
                encoder.Save(file);
            }
        }

    }
}
