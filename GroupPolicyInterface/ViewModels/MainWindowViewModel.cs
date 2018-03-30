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
        public ICommand ReadPoliciesButtonCommand { get; set; }
        public string textReadButton { get; set; }
        public UserControl ContentControlBinding { get; set; }

        public MainWindowViewModel()
        {
            ContentControlBinding = new ReadPolicyView();
        }
    }
}
