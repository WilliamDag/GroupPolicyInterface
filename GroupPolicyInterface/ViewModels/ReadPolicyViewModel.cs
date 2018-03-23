using System;
using System.Windows.Input;
using GroupPolicyInterface.Commands;
using GroupPolicyInterface.Models;
using Microsoft.Win32;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace GroupPolicyInterface.ViewModels
{
    class ReadPolicyViewModel : BaseViewModel
    {
        private ObservableCollection<GroupPolicy> gpoList;
        public ObservableCollection<GroupPolicy> _gpoList
        {
            get { return gpoList; }
            set { gpoList = value; }
        }

        public ICommand SavePoliciesButtonCommand { get; set; }
        public string textSaveButton { get; set; }
        
        public string filename = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), @"gpoList.csv");

        public ReadPolicyViewModel()
        {
            textSaveButton = "Save Policies";
            SavePoliciesButtonCommand = new RelayCommand(SavePoliciesButtonClick);

            IEnumerable<GroupPolicy> ReadCSV(string fileName)
            {
                string[] lines = File.ReadAllLines(fileName);
                
                return lines.Select(lineToSplit =>
                {
                    string[] data = lineToSplit.Split(';');
                    return new GroupPolicy(data[0], data[1], data[2], data[3], data[4]);
                });
            }
            _gpoList = new ObservableCollection<GroupPolicy>(ReadCSV(filename));
        }

        public void SavePoliciesButtonClick()
        {
            onChanged(nameof(_gpoList));
            // Create an instance of HKEY_CURRENT_USER registry key
            RegistryKey masterKey = Registry.CurrentUser;
            // Open the Software registry sub key under the HKEY_CURRENT_USER parent key (read only)
            RegistryKey softwareKey = masterKey.OpenSubKey("SOFTWARE", true);
            foreach (var item in _gpoList)
            {
                RegistryKey sub = softwareKey.CreateSubKey(item._regPath);
                if (item._state == false)
                {
                    item._keyValue = "0";
                    sub.SetValue(item._keyName, item._keyValue, RegistryValueKind.DWord);
                    Console.WriteLine(item._name.ToString());
                    Console.WriteLine(item._keyName.ToString());
                }
                if (item._state == true)
                {
                    item._keyValue = "1";
                    sub.SetValue(item._keyName, item._keyValue, RegistryValueKind.DWord);
                    Console.WriteLine(item._name.ToString());
                    Console.WriteLine(item._keyName.ToString());
                }
                if (item._state == null)
                {
                    sub.DeleteValue(item._keyName);
                    Console.WriteLine(item._name.ToString());
                    Console.WriteLine(item._keyName.ToString());
                }
                sub.Close();
            }
            softwareKey.Close();
            masterKey.Close();
        }
    }
}