﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Mathe1.Common
{
    /// <summary>
    /// see: http://agsmith.wordpress.com/2008/04/07/propertydescriptor-addvaluechanged-alternative/
    /// das gibt ein Memoryleak:
    /// var textProp = DependencyPropertyDescriptor.FromProperty(TextBox.TextProperty, typeof(TextBox));
    ///        if (textProp != null)
    ///        {
    ///            textProp.AddValueChanged(AssociatedObject, (s, args) => this.UpdateText());
    ///        }
    /// so wird es weak:
    /// //AddValueChanged for IsFocused in a weak manner
    ///     this.notifier = new WeakPropertyChangeNotifier(this.AssociatedObject, UIElement.IsFocusedProperty);
    ///     this.notifier.ValueChanged += new EventHandler(this.UpdateAdorner);
    /// </summary>
    public class WeakPropertyChangeNotifier : DependencyObject, IDisposable
    {
        #region Member Variables
        private readonly WeakReference _propertySource;
        #endregion // Member Variables

        #region Constructor
        public WeakPropertyChangeNotifier(DependencyObject propertySource, string path)
            : this(propertySource, new PropertyPath(path))
        {
        }
        public WeakPropertyChangeNotifier(DependencyObject propertySource, DependencyProperty property)
            : this(propertySource, new PropertyPath(property))
        {
        }
        public WeakPropertyChangeNotifier(DependencyObject propertySource, PropertyPath property)
        {
            if (null == propertySource)
                throw new ArgumentNullException("propertySource");
            if (null == property)
                throw new ArgumentNullException("property");

            this._propertySource = new WeakReference(propertySource);

            Binding binding = new Binding();
            binding.Path = property;
            binding.Mode = BindingMode.OneWay;
            binding.Source = propertySource;
            BindingOperations.SetBinding(this, ValueProperty, binding);
        }
        #endregion // Constructor

        #region PropertySource
        public DependencyObject PropertySource
        {
            get
            {
                try
                {
                    // note, it is possible that accessing the target property
                    // will result in an exception so i’ve wrapped this check
                    // in a try catch
                    return this._propertySource.IsAlive
                    ? this._propertySource.Target as DependencyObject
                    : null;
                }
                catch
                {
                    return null;
                }
            }
        }
        #endregion // PropertySource

        #region Value
        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
        typeof(object), typeof(WeakPropertyChangeNotifier), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WeakPropertyChangeNotifier notifier = (WeakPropertyChangeNotifier)d;
            if (null != notifier.ValueChanged)
                notifier.ValueChanged(notifier, EventArgs.Empty);
        }

        /// <summary>
        /// Returns/sets the value of the property
        /// </summary>
        /// <seealso cref="ValueProperty"/>
        [Description("Returns/sets the value of the property")]
        [Category("Behavior")]
        [Bindable(true)]
        public object Value
        {
            get
            {
                return (object)this.GetValue(ValueProperty);
            }
            set
            {
                this.SetValue(ValueProperty, value);
            }
        }
        #endregion //Value

        #region Events
        public event EventHandler ValueChanged;
        #endregion // Events

        #region IDisposable Members
        public void Dispose()
        {
            BindingOperations.ClearBinding(this, ValueProperty);
        }
        #endregion
    }
}
