namespace SpottedZebra.UnitySizeExplorer.WPF.Views
{
    using System.Windows;
    using System.Windows.Input;

    public class TreeViewItemKeyboardShortcut
    {
        public static readonly DependencyProperty InputBindingsProperty =
            DependencyProperty.RegisterAttached(
                "InputBindings", 
                typeof(InputBindingCollection), 
                typeof(TreeViewItemKeyboardShortcut),
                new FrameworkPropertyMetadata(
                    new InputBindingCollection(),
                    (sender, e) =>
                    {
                        var element = sender as UIElement;
                        if (element == null)
                        {
                            return;
                        }

                        element.InputBindings.Clear();
                        element.InputBindings.AddRange((InputBindingCollection)e.NewValue);
                    }));

        public static InputBindingCollection GetInputBindings(UIElement element)
        {
            return (InputBindingCollection)element.GetValue(InputBindingsProperty);
        }

        public static void SetInputBindings(UIElement element, InputBindingCollection inputBindings)
        {
            element.SetValue(InputBindingsProperty, inputBindings);
        }
    }
}
