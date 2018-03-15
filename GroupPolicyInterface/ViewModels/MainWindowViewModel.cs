using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupPolicyInterface.Models;
using GroupPolicyInterface.Commands;
using GroupPolicyInterface.ViewModels;
using GroupPolicyInterface.Views;
using System.Windows.Controls;
using System.IO;

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
            textReadButton = "Read Policies";
            ReadPoliciesButtonCommand = new RelayCommand(ReadPoliciesButtonClick);

            GroupPolicy groupPolicy = new GroupPolicy();
            string binPath = Path.GetDirectoryName(Directory.GetCurrentDirectory());
            string filename = Path.Combine(binPath, @"gpoList.csv");
            StreamReader sr = new StreamReader(filename);

            string line = string.Empty;

            while ((line = sr.ReadLine()) != null)
            {
                line = line.Replace("\"", "");
                string[] stringsArr = line.Split(',');
                groupPolicy._name = stringsArr[0];
                groupPolicy._description = stringsArr[1];
            }
            sr.Close();
        }

        private void ReadPoliciesButtonClick()
        {
            ContentControlBinding = new ReadPolicyView();
            onChanged(nameof(ContentControlBinding));
        }
    }
}
