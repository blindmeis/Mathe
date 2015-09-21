using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mathe1.Common
{
    [Serializable]
    public class ViewmodelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        [field: NonSerialized]
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly object _lock = new object();
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        private string _focusToBindingPath;

        public ViewmodelBase()
        {
            this.DisableOnPropertyChanged = false;
        }

        public bool DisableOnPropertyChanged { get; set; }

        public string FocusToBindingPath
        {
            get { return _focusToBindingPath; }
            set
            {
                //immer on Propertychanged...
                _focusToBindingPath = value;
                OnPropertyChanged();
            }
        }

        public virtual IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                lock (_lock)
                {
                    if (_errors.ContainsKey(propertyName) && (_errors[propertyName] != null) && _errors[propertyName].Count > 0)
                        return _errors[propertyName].ToList();
                    else
                        return null;
                }
            }
            else
                lock (_lock)
                {
                    return _errors.SelectMany(err => err.Value.ToList());
                }
        }

        public virtual bool IsAllValid()
        {
            Validate();
            return !HasErrors;
        }

        public bool HasErrors
        {
            get
            {
                lock (_lock)
                {
                    return _errors.Any(propErrors => propErrors.Value != null && propErrors.Value.Count > 0);
                }
            }
        }

        protected virtual void ValidateProperty([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return;

            lock (_lock)
            {
                var wert = this.GetType().GetProperty(propertyName).GetValue(this, null);
                var validationContext = new ValidationContext(this, null, null);
                validationContext.MemberName = propertyName;
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateProperty(wert, validationContext, validationResults);

                //clear previous _errors from tested property  
                if (_errors.ContainsKey(propertyName))
                    _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
                HandleValidationResults(validationResults);
            }
        }

        public void Validate()
        {
            lock (_lock)
            {
                var validationContext = new ValidationContext(this, null, null);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                //clear all previous _errors  
                var propNames = _errors.Keys.ToList();
                _errors.Clear();
                propNames.ForEach(OnErrorsChanged);
                HandleValidationResults(validationResults);
            }
        }

        private void HandleValidationResults(List<ValidationResult> validationResults)
        {
            //Group validation results by property names  
            var resultsByPropNames = from res in validationResults
                                     from mname in res.MemberNames
                                     group res by mname into g
                                     select g;
            //add _errors to dictionary and inform binding engine about _errors  
            foreach (var prop in resultsByPropNames)
            {
                var messages = prop.Select(r => r.ErrorMessage).ToList();
                _errors.Add(prop.Key, messages);
                OnErrorsChanged(prop.Key);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (DisableOnPropertyChanged)
            {
                return;
            }

            ValidateProperty(propertyName);

            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }

            OnPropertyChanged(propertyInfo.Name);
        }

        protected virtual void OnErrorsChanged(string propertyName)
        {
            var handler = ErrorsChanged;
            if (handler != null) handler(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
