using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace vrisian
{
    public class CustomCommand
    {
        private CommandBinding C;

        public CustomCommand(Key key, ModifierKeys modifierKeys, ExecutedRoutedEventHandler execute, CanExecuteRoutedEventHandler canExecute = null)
        {
            Register(new KeyGesture(key, modifierKeys), execute, canExecute);
        }

        public CustomCommand(MouseAction mouse, ModifierKeys modifierKeys, ExecutedRoutedEventHandler execute, CanExecuteRoutedEventHandler canExecute = null)
        {
            Register(new MouseGesture(mouse, modifierKeys), execute, canExecute);
        }

        public void Register(InputGesture I, ExecutedRoutedEventHandler execute, CanExecuteRoutedEventHandler canExecute = null)
        {
            if (canExecute == null)
            {
                canExecute = (object sender, CanExecuteRoutedEventArgs e) => { e.CanExecute = true; };
            }

            var r = new RoutedCommand();
            r.InputGestures.Add(I);
            C = new CommandBinding(r, execute, canExecute);
            Register();
        }

        public void Register() => Utils.Window?.CommandBindings.Add(C);

        public void Remove() => Utils.Window?.CommandBindings.Remove(C);
    }
}
