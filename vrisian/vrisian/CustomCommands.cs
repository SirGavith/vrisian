using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace vrisian
{
    public abstract class CustomCommandParent
    {
        public abstract string Identifier { get; set; }
        public abstract string Description { get; set; }

        public abstract void RestoreDefaultKeyGesture();
        public abstract void Register();
        public abstract void Deregister();
    }


    public class CustomCommandCategory : CustomCommandParent
    {
        public override string Identifier { get; set; }
        public override string Description { get; set; }

        public List<CustomCommandParent> Commands { get; set; }

        public CustomCommandCategory(string identifier, string description, List<CustomCommandParent> commands)
        {
            Identifier = identifier;
            Description = description;
            Commands = commands;
        }


        public override void RestoreDefaultKeyGesture() => Commands.ForEach(C => C.RestoreDefaultKeyGesture());
        public override void Register() => Commands.ForEach(C => C.Register());
        public override void Deregister() => Commands.ForEach(C => C.Deregister());
    }

    public class CustomCommand : CustomCommandParent
    {
        public override string Identifier { get; set; }
        public override string Description { get; set; }

        public KeyGesture DefaultKeyGesture { get; }
        public Action Executed { get; set; }

        public KeyGesture KeyGesture { get; set; }

        public CustomCommand(string identifier, string description, KeyGesture defaultKeyGesture, Action executed)
        {
            Identifier = identifier;
            Description = description;
            DefaultKeyGesture = defaultKeyGesture;
            KeyGesture = DefaultKeyGesture;
            Executed = executed;
        }

        public override void RestoreDefaultKeyGesture() => KeyGesture = DefaultKeyGesture;
        public override void Register() => Utils.Window?.CommandBindings.Add(CommandBinding);
        public override void Deregister() => Utils.Window?.CommandBindings.Remove(CommandBinding);

        private CommandBinding CommandBinding
        {
            get
            {
                var R = new RoutedCommand();
                R.InputGestures.Add(KeyGesture);
                var Exec = Executed;
                return new CommandBinding(R, (object sender, ExecutedRoutedEventArgs e) => Exec());
            }
        }
    }
}
