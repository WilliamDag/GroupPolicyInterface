using System.ComponentModel;

/*
 * This BaseViewModel class acts as a base class for all
 * future viewmodels which will all inherit from this.
 * The INotifyPropertyChanged and the OnChanged method sends
 * a signal to the UI whenever a property of the viewmodel
 * has changed and updates the UI with the new value.
 */

namespace GroupPolicyInterface.ViewModels
{
    class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void onChanged(string propertyChanged)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyChanged));
        }
    }
}