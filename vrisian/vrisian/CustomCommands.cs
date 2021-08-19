using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace vrisian
{
    //public static class CustomCommands
    //{
    //    //public static readonly RoutedUICommand OpenFolderBrowserDialog = new RoutedUICommand();

        
    //}

    public class CustomCommand
    {
        public CustomCommand(Key key, ModifierKeys modifierKeys, ExecutedRoutedEventHandler execute)
        {
            new CustomCommand(key, modifierKeys, execute, (object sender, CanExecuteRoutedEventArgs e) => { e.CanExecute = true; });
        }

        public CustomCommand(Key key, ModifierKeys modifierKeys, ExecutedRoutedEventHandler execute, CanExecuteRoutedEventHandler canExecute)
        {
            new CustomCommand(new KeyGesture(key, modifierKeys), execute, canExecute);
        }

        public CustomCommand(MouseAction mouse, ModifierKeys modifierKeys, ExecutedRoutedEventHandler execute, CanExecuteRoutedEventHandler canExecute)
        {
            new CustomCommand(new MouseGesture(mouse, modifierKeys), execute, canExecute);
        }

        public CustomCommand(InputGesture I, ExecutedRoutedEventHandler execute, CanExecuteRoutedEventHandler canExecute)
        {
            var r = new RoutedCommand();
            r.InputGestures.Add(I);
            new CustomCommand(new CommandBinding(r, execute, canExecute));
        }

        public CustomCommand(CommandBinding C) => Utils.Window.CommandBindings.Add(C);
    }

    //struct CustomCommand
    //{
    //    public CustomCommand(RoutedCommand r, ExecutedRoutedEventHandler executed, CanExecuteRoutedEventHandler canExecute)
    //    {
    //        Command = r;
    //        Execute = executed;
    //        CanExecute = canExecute;
    //        //CommandType = commandType;
    //    }

    //    public RoutedCommand Command { get; set; }
    //    public ExecutedRoutedEventHandler Execute { get; set; }
    //    public CanExecuteRoutedEventHandler CanExecute { get; set; }
    //    //public Commands CommandType { get; set; }
    //}
}
