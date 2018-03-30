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
using System.Windows;

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
                    return new GroupPolicy(data[0].TrimStart('"'), data[1], data[2], data[3], data[4], data[5], data[6], data[7]);
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
            RegistryValueKind regKeyValueKind;
            foreach (var item in _gpoList)
            {
                if (item._keyValueKind == "String")
                {
                    regKeyValueKind = RegistryValueKind.String;
                }
                else
                    regKeyValueKind = RegistryValueKind.DWord;
                RegistryKey sub = softwareKey.CreateSubKey(item._regPath);
                if (item._state == "Disabled")
                {                    
                    item._keyValue = item._disabledValue;
                    sub.SetValue(item._keyName, item._keyValue, regKeyValueKind);
                }
                if (item._state == "Enabled")
                {
                    item._keyValue = item._enabledValue;
                    sub.SetValue(item._keyName, item._keyValue, regKeyValueKind);
                }
                if (item._state == "Not Configured" || item._state == null)
                {
                    item._keyValue = item._disabledValue;
                    sub.SetValue(item._keyName, 0, regKeyValueKind);
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