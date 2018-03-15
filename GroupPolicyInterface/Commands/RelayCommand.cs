using System;
using System.Windows.Input;

/* RelayCommand lets the ViewModel class know that a button
 * has been clicked or that an event has happened.
 */

namespace GroupPolicyInterface.Commands
{
    class RelayCommand : ICommand
    {
        private Action _action;

        //This is called an inline function
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayCommand(Action action)
        {
            _action = action;
        }

        //Will always return true. This is to allow the Command to run.
        //If set to false it will never run.
        public bool CanExecute(object parameter)
        {
            return true;
        }

        //Tells our private field to actually run the code which we
        //will  inject into the RelayCommand via the Constructor
        public void Execute(object parameter)
        {
            _action();
        }
    }
}