using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Mathe1.Common
{
    public class FocusBorderAdornerBehavior : AttachableForStyleBehavior<FrameworkElement, FocusBorderAdornerBehavior>
    {
        private BorderAdorner _adorner;

        public FocusBorderAdornerBehavior()
        {
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObjectOnLoaded;
            AssociatedObject.PreviewGotKeyboardFocus += AssociatedObject_GotKeyboardFocus;
            AssociatedObject.PreviewLostKeyboardFocus += AssociatedObjectOnLostKeyboardFocus;
            AssociatedObject.LostFocus += AssociatedObjectLostFocus;
        }

        private void AssociatedObjectLostFocus(object sender, RoutedEventArgs e)
        {
            if (_adorner != null)
                _adorner.Visibility = Visibility.Collapsed;
        }

        private void AssociatedObjectOnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs keyboardFocusChangedEventArgs)
        {
            if (_adorner != null)
                _adorner.Visibility = Visibility.Collapsed;

        }

        void AssociatedObject_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            CreateAdorner();

            if (_adorner != null)
                _adorner.Visibility = Visibility.Visible;
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            CreateAdorner();
        }

        private void CreateAdorner()
        {
            if (_adorner != null)
                return;

            if (AssociatedObject == null)
                return;

            var adornerLayer = AdornerLayer.GetAdornerLayer(this.AssociatedObject);
            if (adornerLayer == null)
            {
                return;
            }

            _adorner = new BorderAdorner(this.AssociatedObject);
            _adorner.Visibility = Visibility.Collapsed;

            adornerLayer.Add(_adorner);
        }
    }

    /// <summary>
    /// use <Setter Property="behaviours:MyXYZBehaviour.IsEnabledForStyle" Value="True"/>
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <typeparam name="TBehavior"></typeparam>
    public class AttachableForStyleBehavior<TComponent, TBehavior> : Behavior<TComponent>
        where TComponent : System.Windows.DependencyObject
        where TBehavior : AttachableForStyleBehavior<TComponent, TBehavior>, new()
    {
        public static DependencyProperty IsEnabledForStyleProperty =
            DependencyProperty.RegisterAttached("IsEnabledForStyle", typeof(bool),
            typeof(AttachableForStyleBehavior<TComponent, TBehavior>), new FrameworkPropertyMetadata(false, OnIsEnabledForStyleChanged));

        public bool IsEnabledForStyle
        {
            get { return (bool)GetValue(IsEnabledForStyleProperty); }
            set { SetValue(IsEnabledForStyleProperty, value); }
        }

        private static void OnIsEnabledForStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = d as UIElement;

            if (uie != null)
            {
                var behColl = Interaction.GetBehaviors(uie);
                var existingBehavior = behColl.FirstOrDefault(b => b.GetType() ==
                      typeof(TBehavior)) as TBehavior;

                if ((bool)e.NewValue == false && existingBehavior != null)
                {
                    behColl.Remove(existingBehavior);
                }

                else if ((bool)e.NewValue == true && existingBehavior == null)
                {
                    behColl.Add(new TBehavior());
                }
            }
        }
    }

    class BorderAdorner : System.Windows.Documents.Adorner
    {
        private readonly Border _child;
        readonly VisualCollection _visualChildren;

        public BorderAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _child = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(2) };
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

}
