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
        private List<State> states;

        public List<State> _states
        {
            get { return states; }
        }

        public ICommand SavePoliciesButtonCommand { get; set; }
        public string textSaveButton { get; set; }

        public string filename = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), @"gpoList.csv");

        public ReadPolicyViewModel()
        {
            states = State.GetAvailableStates();
            textSaveButton = "Save Policies";
            SavePoliciesButtonCommand = new RelayCommand(SavePoliciesButtonClick);

            IEnumerable<GroupPolicy> ReadCSV(string fileName)
            {
                string[] lines = File.ReadAllLines(fileName);

                return lines.Select(lineToSplit =>
                {
                    string[] data = lineToSplit.Split(';');
                    return new GroupPolicy(data[0].TrimStart('"'), data[1], data[2], data[3], data[4], data[5], data[6]);
                });
            }
            _gpoList = new ObservableCollection<GroupPolicy>(ReadCSV(filename));
        }

        public void SavePoliciesButtonClick()
        {
            onChanged(nameof(_gpoList));
            onChanged(nameof(_states));
            // Create an instance of HKEY_CURRENT_USER registry key
            RegistryKey masterKey = Registry.CurrentUser;
            // Open the Software registry sub key under the HKEY_CURRENT_USER parent key (read only)
            RegistryKey softwareKey = masterKey.OpenSubKey("SOFTWARE", true);
            foreach (var item in _gpoList)
            {
                RegistryKey sub = softwareKey.CreateSubKey(item._regPath);
                if (item._state == "Disabled")
                {
                    item._keyValue = item._disabledValue;
                    sub.SetValue(item._keyName, item._keyValue, RegistryValueKind.DWord);
                }
                if (item._state == "Enabled")
                {
                    item._keyValue = item._enabledValue;
                    sub.SetValue(item._keyName, item._keyValue, RegistryValueKind.DWord);
                }
                if (item._state == "Not Configured")
                {
                    try
                    {
                        sub.DeleteValue(item._keyName);
                    }
                    catch (ArgumentException e)
                    {
                    }
                }
                sub.Close();
            }
            softwareKey.Close();
            masterKey.Close();
        }
    }
}