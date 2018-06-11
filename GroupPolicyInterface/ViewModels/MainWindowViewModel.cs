using System.Windows.Input;
using GroupPolicyInterface.Commands;
using GroupPolicyInterface.Views;
using System.Windows.Controls;

/* This Class is what is going to hold all the
 * C# code that the MainWindow.xaml will need to run.
 */

namespace GroupPolicyInterface.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        //ICommand for the View button
        public ICommand ReadPoliciesButtonCommand { get; set; }
        //Text for View button
        public string textReadButton { get; set; }
        //UserControl to change windows
        public UserControl ContentControlBinding { get; set; }

        public MainWindowViewModel()
        {
            //The usercontrol sets the current view to the ReadPolicyView - to display the GPO list
            ContentControlBinding = new ReadPolicyView();
        }
    }
}
