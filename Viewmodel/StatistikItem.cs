using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Mathe1.Common;

namespace Mathe1.Viewmodel
{
    [Serializable]
    public class Statistik : List<StatistikItem>
    {
        public Statistik()
        {
            
        }

        public Statistik(IList<StatistikItem> items):base(items)
        {
            
        }
    }

    [Serializable]
    public class StatistikItem: ViewmodelBase
    {
        public Operationen AufgabenTyp { get; set; }
        public Dictionary<int, AuswertungAufgabe> Ergebnisse { get; set; }
        public DateTime Timestamp { get; set; }

        public StatistikItem(Operationen aufgabenTyp, Dictionary<int, AuswertungAufgabe> ergebnisse)
        {
            Timestamp = DateTime.Now;
            AufgabenTyp = aufgabenTyp;
            Ergebnisse = ergebnisse;
        }
    }

    [Serializable]
    public class AuswertungAufgabe
    {
        public int GeschaffteAufgaben { get; set; }
        public int MaxAufgaben { get; set; }

        public AuswertungAufgabe(int geschaffteAufgaben, int maxAufgaben)
        {
            GeschaffteAufgaben = geschaffteAufgaben;
            MaxAufgaben = maxAufgaben;
        }
    }

    public class StatistikReader
    {
        public static Statistik Read()
        {
            var filename = "Statistik.math";

            var fi = new FileInfo(filename);

            if(!fi.Exists)
                return new Statistik();

            IFormatter formatter = new BinaryFormatter();
            using (var stream = new FileStream(filename,FileMode.Open,FileAccess.Read))
            {
                if(stream.Length==0)
                    return new Statistik();

                stream.Seek(0, SeekOrigin.Begin);
                return (Statistik)formatter.Deserialize(stream);
            }
        }
    }
    public class StatistikWriter
    {
        public static void Write(Statistik auswertung)
        {
            var filename = "Statistik.math";
            
            IFormatter formatter = new BinaryFormatter();
            var stream = new MemoryStream();

            using (var fileStream = new FileStream(filename,FileMode.Create))
            {
                formatter.Serialize(stream, auswertung);
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
        }
    }

    public class StatistikEventArgs : EventArgs
    {
        public StatistikItem Auswertung { get; private set; }

        public StatistikEventArgs(StatistikItem auswertung)
        {
            Auswertung = auswertung;
        }
    }

    public class StatistikErgebnisConverter : IMultiValueConverter
    {
       

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var auswertung = values[1] as AuswertungAufgabe;

            if (auswertung == null)
                return Binding.DoNothing;

            int key;//vllt für später ma
            if (!int.TryParse(values[0].ToString(), out key))
                return Binding.DoNothing;

            var quote = (double)auswertung.GeschaffteAufgaben / (double)auswertung.MaxAufgaben;

            if (quote >= 0.9)
                return StatistikAuswertungRank.SehrGut;

            if (quote >= 0.5)
                return StatistikAuswertungRank.Ok;

            return StatistikAuswertungRank.Naja;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum StatistikAuswertungRank
    {
        SehrGut,
        Ok,
        Naja
    }
}
