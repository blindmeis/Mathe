using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mathe1.Common;

namespace Mathe1.Viewmodel
{
    [Serializable]
    public class AufgabeViewmodel : ViewmodelBase, IAufgabe
    {
        private static readonly Random Zufall = new Random();
        private static readonly int AdditionLeichtZahlenraum;
        private static readonly int AdditionNormalZahlenraum;
        private static readonly int AdditionSchwerZahlenraum;

        private static readonly int SubtraktionLeichtZahlenraum;
        private static readonly int SubtraktionNormalZahlenraum;
        private static readonly int SubtraktionSchwerZahlenraum;

        private static readonly int MultiplikationLeichtZahlenraum;
        private static readonly int MultiplikationNormalZahlenraum;
        private static readonly int MultiplikationSchwerZahlenraum;

        private static readonly int DivisionLeichtZahlenraum;
        private static readonly int DivisionNormalZahlenraum;
        private static readonly int DivisionSchwerZahlenraum;

        private int? _operator1;
        private int? _operator2;
        private int? _result;
        private bool? _obSuccess;
        private bool _obOperator1Unbekannt;
        private bool _obOperator2Unbekannt;
        private bool _obResultUnbekannt;
        private int? _lockResult;
        private bool _obLocked;
        private string _lastValidatedOperator1;
        private string _lastValidatedOperator2;
        private string _lastValidatedResult;

        static AufgabeViewmodel()
        {
           AdditionLeichtZahlenraum = ConfigurationManager.AppSettings.AllKeys.Contains("AdditionLeichtZahlenraum") ? Convert.ToInt32(ConfigurationManager.AppSettings["AdditionLeichtZahlenraum"]): 10;
            AdditionNormalZahlenraum = ConfigurationManager.AppSettings.AllKeys.Contains("AdditionNormalZahlenraum") ? Convert.ToInt32(ConfigurationManager.AppSettings["AdditionNormalZahlenraum"]): 100;
            AdditionSchwerZahlenraum = ConfigurationManager.AppSettings.AllKeys.Contains("AdditionSchwerZahlenraum") ? Convert.ToInt32(ConfigurationManager.AppSettings["AdditionSchwerZahlenraum"]): 1000;

            SubtraktionLeichtZahlenraum = ConfigurationManager.AppSettings.AllKeys.Contains("SubtraktionLeichtZahlenraum") ? Convert.ToInt32(ConfigurationManager.AppSettings["SubtraktionLeichtZahlenraum"]): 20;
            SubtraktionNormalZahlenraum = ConfigurationManager.AppSettings.AllKeys.Contains("SubtraktionNormalZahlenraum") ? Convert.ToInt32(ConfigurationManager.AppSettings["SubtraktionNormalZahlenraum"]): 100;
            SubtraktionSchwerZahlenraum = ConfigurationManager.AppSettings.AllKeys.Contains("SubtraktionSchwerZahlenraum") ? Convert.ToInt32(ConfigurationManager.AppSettings["SubtraktionSchwerZahlenraum"]): 1000;

            MultiplikationLeichtZahlenraum = ConfigurationManager.AppSettings.AllKeys.Contains("MultiplikationLeichtZahlenraum") ? Convert.ToInt32(ConfigurationManager.AppSettings["MultiplikationLeichtZahlenraum"]): 10;
            MultiplikationNormalZahlenraum = ConfigurationManager.AppSettings.AllKeys.Contains("MultiplikationNormalZahlenraum") ? Convert.ToInt32(ConfigurationManager.AppSettings["MultiplikationNormalZahlenraum"]): 15;
            MultiplikationSchwerZahlenraum = ConfigurationManager.AppSettings.AllKeys.Contains("MultiplikationSchwerZahlenraum") ? Convert.ToInt32(ConfigurationManager.AppSettings["MultiplikationSchwerZahlenraum"]): 20;

            DivisionLeichtZahlenraum = ConfigurationManager.AppSettings.AllKeys.Contains("DivisionLeichtZahlenraum") ? Convert.ToInt32(ConfigurationManager.AppSettings["DivisionLeichtZahlenraum"]): 10;
            DivisionNormalZahlenraum = ConfigurationManager.AppSettings.AllKeys.Contains("DivisionNormalZahlenraum") ? Convert.ToInt32(ConfigurationManager.AppSettings["DivisionNormalZahlenraum"]): 15;
            DivisionSchwerZahlenraum = ConfigurationManager.AppSettings.AllKeys.Contains("DivisionSchwerZahlenraum") ? Convert.ToInt32(ConfigurationManager.AppSettings["DivisionSchwerZahlenraum"]): 20;

         
        }

        public AufgabeViewmodel(Operationen operation, int schwierigkeit)
        {
            Operation = operation;

            Schwierigkeit = schwierigkeit;

            Zahlenraum = GetZahlenraumFromSchwierigkeit(schwierigkeit);
            
            Operator1 = Zufall.Next(Zahlenraum);

            CreateOperator2();

            VerschiebeUnbekannte(schwierigkeit);

            MaxVersuche = GetMaxVersuche(schwierigkeit);

            VersucheCounter = new ObservableCollection<Versuche>();

            for (var i = 0; i < MaxVersuche; i++)
            {
                VersucheCounter.Add(new Versuche());
            }
          
        }

        public string LastValidatedOperator1
        {
            get { return _lastValidatedOperator1; }
            set
            {
                if(_lastValidatedOperator1==value)
                    return;
                _lastValidatedOperator1 = value;
                OnPropertyChanged();
            }
        }

        public string LastValidatedOperator2
        {
            get { return _lastValidatedOperator2; }
            set
            {
                if (_lastValidatedOperator2 == value)
                    return;
                _lastValidatedOperator2 = value;
                OnPropertyChanged();
            }
        }

        public string LastValidatedResult
        {
            get { return _lastValidatedResult; }
            set
            {
                if(_lastValidatedResult==value)
                    return;
                _lastValidatedResult = value;
                OnPropertyChanged();
            }
        }

        private void VerschiebeUnbekannte(int schwierigkeit)
        {
            switch (schwierigkeit)
            {
                case 0: //nix machen
                    ObResultUnbekannt = true;
                    return;
                case 1: //Operator 2 zu 33% als unbekannt
                    if (Zufall.Next(100) <= 33)
                    {
                        ObOperator2Unbekannt = true;
                        Result = CheckResult;
                        Operator2 = null;
                    }
                    else
                    {
                        ObResultUnbekannt = true;
                    }
                    return;
                case 2: //alle zu 33% als unbekannt
                    if (Zufall.Next(100) <= 66)
                    {
                        if (Zufall.Next(100) <= 50)
                        {
                            ObOperator1Unbekannt = true;
                            Result = CheckResult;
                            Operator1 = null;
                        }
                        else
                        {
                            ObOperator2Unbekannt = true;
                            Result = CheckResult;
                            Operator2 = null;
                        }
                    }
                    else
                    {
                        ObResultUnbekannt = true;
                    }
                    return;
            }
        }

        private int GetZahlenraumFromSchwierigkeit(int schwierigkeit)
        {
            switch (Operation)
            {
                case Operationen.Addition:
                    return schwierigkeit == 0 ? AdditionLeichtZahlenraum : (schwierigkeit == 1 ? AdditionNormalZahlenraum : AdditionSchwerZahlenraum);
                case Operationen.Subtraktion:
                    return schwierigkeit == 0 ? SubtraktionLeichtZahlenraum : (schwierigkeit == 1 ? SubtraktionNormalZahlenraum : SubtraktionSchwerZahlenraum);
                case Operationen.Multiplikation:
                    return schwierigkeit == 0 ? MultiplikationLeichtZahlenraum : (schwierigkeit == 1 ? MultiplikationNormalZahlenraum : MultiplikationSchwerZahlenraum);
                case Operationen.Division:
                    return schwierigkeit == 0 ? DivisionLeichtZahlenraum : (schwierigkeit == 1 ? DivisionNormalZahlenraum : DivisionSchwerZahlenraum); ;
            }

            return 0;
        }

        private int GetMaxVersuche(int schwierigkeit)
        {
            switch (schwierigkeit)
            {
                case 0:
                    return 1;
                case 1:
                    return 2;
                case 2:
                    return 3;
            }

            return 0;
        }

        private void CreateOperator2()
        {
            switch (Operation)
            {
                case Operationen.Addition:
                    while (Operator2==null)
                    {
                        Operator2 = Zufall.Next(Zahlenraum);
                        if (CheckResult == null || CheckResult > Zahlenraum * 2)
                            Operator2 = null;
                    }
                    break;
                case Operationen.Subtraktion:
                    while (Operator2 == null)
                    {
                        Operator2 = Zufall.Next(Zahlenraum);
                        if (CheckResult == null || CheckResult < 0)
                            Operator2 = null;
                    }
                    break;
                case Operationen.Multiplikation:
                    while (Operator2 == null)
                    {
                        Operator2 = Zufall.Next(Zahlenraum);
                        if (CheckResult == null)
                            Operator2 = null;
                    }
                    break;
                case Operationen.Division:
                    while (Operator2 == null)
                    {
                        Operator2 = Zufall.Next(Zahlenraum);
                        if (Operator2 == 0|| CheckResult == null)
                            Operator2 = null;
                    }
                    Operator1 = Operator1*Operator2;
                    break;
            }
        }

        public int? LockResult
        {
            get { return _lockResult; }
            set
            {
                if (_lockResult == value)
                    return;
                _lockResult = value;
                OnPropertyChanged();
            }
        }

        public void ValidateResult()
        {
            if (CheckResult == null || Result == null)
            {
                ObSuccess = false;
                return;
            }

            ObSuccess = CheckResult == Result;

            //Set LastValidated Values
            LastValidatedOperator1 = Operator1?.ToString() ?? "";
            LastValidatedOperator2 = Operator2?.ToString() ?? "";
            LastValidatedResult = Result.Value.ToString();

            HandleLockCounterStuff();
        }

        private int Zahlenraum { get; set; }

        public int? Operator1
        {
            get { return _operator1; }
            set
            {
                if (_operator1 == value)
                    return;
                _operator1 = value;
                OnPropertyChanged();
            }
        }

        public Operationen Operation { get; set; }
        
        public int Schwierigkeit { get; set; }

        public int? Operator2
        {
            get { return _operator2; }
            set
            {
                if (_operator2 == value)
                    return;
                _operator2 = value;
                OnPropertyChanged();
            }
        }

        public int? Result
        {
            get { return _result; }
            set
            {
                if (_result == value)
                    return;
                _result = value;
                OnPropertyChanged();
            }
        }

        public int? CheckResult
        {
            get
            {
                switch (Operation)
                {
                    case Operationen.Addition:
                        if (Operator1.HasValue && Operator2.HasValue)
                            return Operator1 + Operator2;
                        break;
                    case Operationen.Subtraktion:
                        if (Operator1.HasValue && Operator2.HasValue)
                            return Operator1 - Operator2;
                        break;
                    case Operationen.Multiplikation:
                        if (Operator1.HasValue && Operator2.HasValue)
                            return Operator1 * Operator2;
                        break;
                    case Operationen.Division:
                        if (Operator1.HasValue && Operator2.HasValue)
                            return Operator1 / Operator2;
                        break;
                }

                return null;
            }
        }

        public bool? ObSuccess
        {
            get { return _obSuccess; }
            private set
            {
                if (_obSuccess == value)
                    return;

                _obSuccess = value;
                OnPropertyChanged();
            }
        }

        public bool ObLocked
        {
            get { return _obLocked; }
            set
            {
                if(_obLocked==value)
                    return;
                _obLocked = value;
                OnPropertyChanged();
            }
        }

        public bool ObFalsch { get { return !ObSuccess.GetValueOrDefault(true); } }

        public ObservableCollection<Versuche> VersucheCounter { get; private set; }

        public int MaxVersuche { get; private set; }

        public bool ObOperator1Unbekannt
        {
            get { return _obOperator1Unbekannt; }
            set
            {
                if (_obOperator1Unbekannt == value)
                    return;
                _obOperator1Unbekannt = value;
                OnPropertyChanged();
            }
        }

        public bool ObOperator2Unbekannt
        {
            get { return _obOperator2Unbekannt; }
            set
            {
                if (_obOperator2Unbekannt == value)
                    return;
                _obOperator2Unbekannt = value;
                OnPropertyChanged();
            }
        }

        public bool ObResultUnbekannt
        {
            get { return _obResultUnbekannt; }
            set
            {
                if(_obResultUnbekannt == value)
                    return;
                _obResultUnbekannt = value;
                OnPropertyChanged();
            }
        }

        private void HandleLockCounterStuff()
        {
            if (ObLocked)
                return;

            if(VersucheCounter.Count>0)
                VersucheCounter.RemoveAt(0);

            if (VersucheCounter.Count == 0 || ObSuccess.GetValueOrDefault(ObFalsch))
                ObLocked = true;

            if (ObLocked)
            {
                switch (Operation)
                {
                    case Operationen.Addition:
                        if (ObOperator1Unbekannt)
                            LockResult = Result - Operator2;
                        if (ObOperator2Unbekannt)
                            LockResult = Result - Operator1;
                        if (ObResultUnbekannt)
                            LockResult = Operator1 + Operator2;
                        break;
                    case Operationen.Subtraktion:
                        if (ObOperator1Unbekannt)
                            LockResult = Result + Operator2;
                        if (ObOperator2Unbekannt)
                            LockResult = Operator1 - Result;
                        if (ObResultUnbekannt)
                            LockResult = Operator1 - Operator2;
                        break;
                    case Operationen.Multiplikation:
                        if (ObOperator1Unbekannt)
                            LockResult = Result / Operator2;
                        if (ObOperator2Unbekannt)
                            LockResult = Result / Operator1;
                        if (ObResultUnbekannt)
                            LockResult = Operator1 * Operator2;
                        break;
                    case Operationen.Division:
                        if (ObOperator1Unbekannt)
                            LockResult = Result * Operator2;
                        if (ObOperator2Unbekannt)
                            LockResult = Operator1 / Result;
                        if (ObResultUnbekannt)
                            LockResult = Operator1 / Operator2;
                        break;
                }
                OnPropertyChanged("ObFalsch");

                Debug.WriteLine(string.Format("Op1: {0} Op2: {1} Result: {2} --> {3} | {4}  <-- ObSuccess: {5}", Operator1, Operator2,Result,ObLocked, ObFalsch, ObSuccess));
            }
            else
            {
                if (!ObSuccess.GetValueOrDefault(false))
                {
                    if (ObOperator1Unbekannt)
                        Operator1 = null;
                    if (ObOperator2Unbekannt)
                        Operator2 = null;
                    if (ObResultUnbekannt)
                        Result = null;
                }
            }
        }
    }
}
