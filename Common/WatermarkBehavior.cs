using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Mathe1.Common
{
    public class TextBoxWatermarkBehavior : System.Windows.Interactivity.Behavior<TextBox>
    {
        internal TextBlockAdorner MyAdorner { get; private set; }
        private WeakPropertyChangeNotifier _notifier;

        #region DependencyProperty's

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.RegisterAttached("Label", typeof(string), typeof(TextBoxWatermarkBehavior), new FrameworkPropertyMetadata(WatermarkTextChanged));

        private static void WatermarkTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var b = d as TextBoxWatermarkBehavior;
            if(b==null)
                return;

            b.AssociatedObject.TryRemoveAdorners<TextBlockAdorner>();
            b.MyAdorner = new TextBlockAdorner(b.AssociatedObject, e.NewValue.ToString(), b.LabelStyle);
            b.UpdateAdorner();
        }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelStyleProperty =
            DependencyProperty.RegisterAttached("LabelStyle", typeof(Style), typeof(TextBoxWatermarkBehavior));

        public Style LabelStyle
        {
            get { return (Style)GetValue(LabelStyleProperty); }
            set { SetValue(LabelStyleProperty, value); }
        }

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += this.AssociatedObjectLoaded;
            this.AssociatedObject.TextChanged += this.AssociatedObjectTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Loaded -= this.AssociatedObjectLoaded;
            this.AssociatedObject.TextChanged -= this.AssociatedObjectTextChanged;

            this._notifier = null;
        }

        private void AssociatedObjectTextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateAdorner();
        }

        private void AssociatedObjectLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.MyAdorner = new TextBlockAdorner(this.AssociatedObject, this.Label, this.LabelStyle);

            this.UpdateAdorner();

            //AddValueChanged for IsFocused in a weak manner
            this._notifier = new WeakPropertyChangeNotifier(this.AssociatedObject, UIElement.IsFocusedProperty);
            this._notifier.ValueChanged += new EventHandler(this.UpdateAdorner);
        }

        private void UpdateAdorner(object sender, EventArgs e)
        {
            this.UpdateAdorner();
        }


        private void UpdateAdorner()
        {
            if (!String.IsNullOrEmpty(this.AssociatedObject.Text) || this.AssociatedObject.IsFocused)
            {
                // Hide the Watermark Label if the adorner layer is visible
                this.AssociatedObject.TryRemoveAdorners<TextBlockAdorner>();
            }
            else
            {
                // Show the Watermark Label if the adorner layer is visible
                this.AssociatedObject.TryAddAdorner<TextBlockAdorner>(MyAdorner);
            }
        }
    }

    public class TextBlockAdorner : System.Windows.Documents.Adorner
    {
        private readonly TextBlock _child;
        readonly VisualCollection _visualChildren;

        public TextBlockAdorner(UIElement adornedElement, string label, Style labelStyle)
            : base(adornedElement)
        {
            _child = new TextBlock { Style = labelStyle, Text = label };
            _visualChildren = new VisualCollection(this);
            _visualChildren.Add(_child);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (_child == null)
                return base.MeasureOverride(constraint);

            _child.Width = AdornedElement.RenderSize.Width;
            _child.Height = AdornedElement.RenderSize.Height;

            _child.Measure(AdornedElement.RenderSize);
            return _child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_child == null)
                return base.ArrangeOverride(finalSize);

            _child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override int VisualChildrenCount { get { return _visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return _visualChildren[index]; }
    }

    public static class AdornerExtensions
    {
        public static void TryRemoveAdorners<T>(this UIElement elem) where T : System.Windows.Documents.Adorner
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(elem);
            if (adornerLayer != null)
            {
                adornerLayer.RemoveAdorners<T>(elem);
            }
        }

        public static void RemoveAdorners<T>(this AdornerLayer adr, UIElement elem)
            where T : System.Windows.Documents.Adorner
        {
            System.Windows.Documents.Adorner[] adorners = adr.GetAdorners(elem);

            if (adorners == null) return;

            for (int i = adorners.Length - 1; i >= 0; i--)
            {
                if (adorners[i] is T)
                    adr.Remove(adorners[i]);
            }
        }

        public static void TryAddAdorner<T>(this UIElement elem, System.Windows.Documents.Adorner adorner)
            where T : System.Windows.Documents.Adorner
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(elem);
            if (adornerLayer != null && !adornerLayer.ContainsAdorner<T>(elem))
            {
                adornerLayer.Add(adorner);
            }
        }

        public static bool ContainsAdorner<T>(this AdornerLayer adr, UIElement elem)
            where T : System.Windows.Documents.Adorner
        {
            System.Windows.Documents.Adorner[] adorners = adr.GetAdorners(elem);

            if (adorners == null) return false;

            for (int i = adorners.Length - 1; i >= 0; i--)
            {
                if (adorners[i] is T)
                    return true;
            }
            return false;
        }

        public static void RemoveAllAdorners(this AdornerLayer adr, UIElement elem)
        {
            System.Windows.Documents.Adorner[] adorners = adr.GetAdorners(elem);

            if (adorners == null) return;

            foreach (var toRemove in adorners)
                adr.Remove(toRemove);
        }
    }
}
