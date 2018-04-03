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
        private const int name = 0;
        private const int shortDescription = 1;
        private const int longDescription = 2;
        private const int regPath = 3;
        private const int keyName = 4;
        private const int keyValueKind = 5;
        private const int enabledValue = 6;
        private const int disabledValue = 7;

        public ICommand SavePoliciesButtonCommand { get; set; }
        public string textSaveButton { get; set; }
        public ICommand EnableAllPoliciesButtonCommand { get; set; }
        public string textEnableAllButton { get; set; }
        public ICommand DisableAllPoliciesButtonCommand { get; set; }
        public string textDisableAllButton { get; set; }
        public string filename = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), @"gpoList.csv");

        public ReadPolicyViewModel()
        {
            textSaveButton = "Save Policies";
            SavePoliciesButtonCommand = new RelayCommand(SavePoliciesButtonClick);
            textEnableAllButton = "Enable All";
            EnableAllPoliciesButtonCommand = new RelayCommand(EnableAllPoliciesButtonClick);
            textDisableAllButton = "Disable All";

            IEnumerable<GroupPolicy> ReadCSV(string fileName)
            {
                string[] lines = File.ReadAllLines(fileName);

                return lines.Select(lineToSplit =>
                {
                    string[] data = lineToSplit.Split(';');
                    string type = "";
                    string nameOnly = data[name];
                    if (data[name].Contains("["))
                    {
                        string fullName = data[name].TrimStart('"');
                        int typeStartIndex = fullName.IndexOf("[");
                        int typeEndIndex = fullName.IndexOf("]");
                        type = fullName.Substring(typeStartIndex+1, typeEndIndex-1);
                        nameOnly = fullName.Substring(typeEndIndex+1);
                    }
                    return new GroupPolicy(type, nameOnly, data[shortDescription], data[longDescription],
                        data[regPath], data[keyName], data[keyValueKind], data[enabledValue], data[disabledValue]);
                });
            }
            _gpoList = new ObservableCollection<GroupPolicy>(ReadCSV(filename));
        }

        public void EnableAllPoliciesButtonClick()
        {
            onChanged(nameof(_gpoList));
            // Create an instance of HKEY_CURRENT_USER registry key
            RegistryKey masterKey = Registry.CurrentUser;
            // Open the Software registry sub key under the HKEY_CURRENT_USER parent key (read only)
            RegistryKey softwareKey = masterKey.OpenSubKey("SOFTWARE", true);
            RegistryValueKind regKeyValueKind;
            foreach (var item in _gpoList)
            {
                RegistryKey sub = softwareKey.CreateSubKey(item._regPath);
                if (item._keyValueKind == "String")
                {
                    regKeyValueKind = RegistryValueKind.String;
                }
                else
                    regKeyValueKind = RegistryValueKind.DWord;
                item._keyValue = item._enabledValue;
                sub.SetValue(item._keyName, item._keyValue, regKeyValueKind);
                sub.Close();
            }
            softwareKey.Close();
            masterKey.Close();
        }
        public void DisableAllPoliciesButtonClick()
        {
            onChanged(nameof(_gpoList));
            // Create an instance of HKEY_CURRENT_USER registry key
            RegistryKey masterKey = Registry.CurrentUser;
            // Open the Software registry sub key under the HKEY_CURRENT_USER parent key (read only)
            RegistryKey softwareKey = masterKey.OpenSubKey("SOFTWARE", true);
            RegistryValueKind regKeyValueKind;
            foreach (var item in _gpoList)
            {
                item._state = "Disabled";
                RegistryKey sub = softwareKey.CreateSubKey(item._regPath);
                if (item._keyValueKind == "String")
                {
                    regKeyValueKind = RegistryValueKind.String;
                }
                else
                    regKeyValueKind = RegistryValueKind.DWord;
                
                item._keyValue = item._disabledValue;
                sub.SetValue(item._keyName, item._keyValue, regKeyValueKind);
                sub.Close();
            }
            softwareKey.Close();
            masterKey.Close();
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
                RegistryKey sub = softwareKey.CreateSubKey(item._regPath);
                if (item._keyValueKind == "String")
                {
                    regKeyValueKind = RegistryValueKind.String;
                }
                else
                    regKeyValueKind = RegistryValueKind.DWord;
                if (item._state == "Disabled")
                {
                    if (item._multipleKeys != null)
                    {
                        string[] multipleKeys = item._multipleKeys.Split(',');
                        foreach (string keyname in multipleKeys)
                        {
                            item._keyName = keyname;
                            item._keyValue = item._disabledValue;
                            sub.SetValue(item._keyName, item._keyValue, regKeyValueKind);
                        }
                    }
                    else
                    {
                        item._keyValue = item._disabledValue;
                        sub.SetValue(item._keyName, item._keyValue, regKeyValueKind);
                    }
                }
                if (item._state == "Enabled")
                {
                    if (item._multipleKeys != null)
                    {
                        string[] multipleKeys = item._multipleKeys.Split(',');
                        foreach (string keyname in multipleKeys)
                        {
                            item._keyName = keyname;
                            item._keyValue = item._enabledValue;
                            sub.SetValue(item._keyName, item._keyValue, regKeyValueKind);
                        }
                    }
                    else
                    {
                        item._keyValue = item._enabledValue;
                        sub.SetValue(item._keyName, item._keyValue, regKeyValueKind);
                    }
                }
                if (item._state == "Not Configured" || item._state == null)
                {
                    if (item._multipleKeys != null)
                    {
                        string[] multipleKeys = item._multipleKeys.Split(',');
                        foreach (string keyname in multipleKeys)
                        {
                            item._keyName = keyname;
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
                    }
                    else
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
                }
                sub.Close();
            }
            softwareKey.Close();
            masterKey.Close();
        }
    }
}