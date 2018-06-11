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
    /* This Class is what is going to hold all the
     * C# code that the ReadPolicyView.xaml will need to run.
     * This will also hold all variables and methods that provide functionality
     * to the main interface of this application. This does not include object variables for a specific GPO.
     * These can be found in the GroupPolicy model class.
     */
    class ReadPolicyViewModel : BaseViewModel
    {
        //Create a private ObservableCollection of GroupPolicy objects. This will be used for our list in the interface.
        private ObservableCollection<GroupPolicy> gpoList;
        //Use accessors to return the private list.
        public ObservableCollection<GroupPolicy> _gpoList
        {
            get { return gpoList; }
            set { gpoList = value; }
        }
        //Constants that will be used to represent the index value from the ReadAllLines method below.
        private const int name = 0;
        private const int shortDescription = 1;
        private const int longDescription = 2;
        private const int regPath = 3;
        private const int keyName = 4;
        private const int keyValueKind = 5;
        private const int enabledValue = 6;
        private const int disabledValue = 7;

        //ICommands that we bind to the UI buttons in the View class.
        public ICommand SavePoliciesButtonCommand { get; set; }
        public string textSaveButton { get; set; }
        public ICommand EnableAllPoliciesButtonCommand { get; set; }
        public string textEnableAllButton { get; set; }
        public ICommand DisableAllPoliciesButtonCommand { get; set; }
        public string textDisableAllButton { get; set; }
        public ICommand ResetAllPoliciesButtonCommand { get; set; }
        public string textResetAllButton { get; set; }

        //String for CSV file in the applications bin folder.
        public string filename = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), @"gpoList.csv");

        //This method sets the initial values of strings displayed on UI buttons, and specifies the methods to be used in each RelayCommand.
        //This method is run automatically as it is the default constructor.
        public ReadPolicyViewModel()
        {
            textSaveButton = "Save Policies";
            SavePoliciesButtonCommand = new RelayCommand(SavePoliciesButtonClick);
            textEnableAllButton = "Enable All";
            EnableAllPoliciesButtonCommand = new RelayCommand(EnableAllPoliciesButtonClick);
            textDisableAllButton = "Disable All";
            DisableAllPoliciesButtonCommand = new RelayCommand(DisableAllPoliciesButtonClick);
            textResetAllButton = "Reset All";
            ResetAllPoliciesButtonCommand = new RelayCommand(ResetAllPoliciesButtonClick);

            //Initial run of the UpdatePolicies funtion - this returns the initial list of GPOs.
            UpdatePolicies();
        }

        //The UpdatePolicies method returns the list of GPOs by first compiling a list from the CSV,
        //and then comparing the state values to the registry.
        public void UpdatePolicies()
        {
            //Returns the GroupPolicies - requires the filename & path of the CSV file.
            IEnumerable<GroupPolicy> ReadCSV(string fileName)
            {
                //Array of all lines from CSV
                string[] lines = File.ReadAllLines(fileName);

                //Using the lines from the array, split them into each variable
                return lines.Select(lineToSplit =>
                {
                    //Create types to group the GPOs by, checking if the GPO name contains a "[" E.g. [Internet Zone]
                    string[] data = lineToSplit.Split(';');
                    string type = "";
                    string nameOnly = data[name].TrimStart('"');
                    if (data[name].Contains("["))
                    {
                        string fullName = data[name].TrimStart('"');
                        int typeStartIndex = fullName.IndexOf("[");
                        int typeEndIndex = fullName.IndexOf("]");
                        type = fullName.Substring(typeStartIndex + 1, typeEndIndex - 1);
                        nameOnly = fullName.Substring(typeEndIndex + 1);
                    }
                    //Return each GroupPolicy object, passing through the constants declared earlier.
                    return new GroupPolicy(type, nameOnly, data[shortDescription], data[longDescription],
                        data[regPath], data[keyName], data[keyValueKind], data[enabledValue], data[disabledValue]);
                });
            }
            //Set the ObservableCollection _gpoList to the IEnumerable of GroupPolicy, passing through the filename variable.
            _gpoList = new ObservableCollection<GroupPolicy>(ReadCSV(filename));
        }

        //Method to reset all group policies to "Not Configured"
        public void ResetAllPoliciesButtonClick()
        {
            if (MessageBox.Show("Are you sure you want to Reset All?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Create an instance of HKEY_CURRENT_USER registry key
                RegistryKey masterKey = Registry.CurrentUser;
                // Open the Software registry sub key under the HKEY_CURRENT_USER parent key (read only)
                RegistryKey softwareKey = masterKey.OpenSubKey("SOFTWARE", true);
                RegistryValueKind regKeyValueKind;
                foreach (var item in _gpoList)
                {
                    RegistryKey sub = softwareKey.CreateSubKey(item._regPath);
                    try
                    {
                        if (item._keyValueKind == "String")
                        {
                            regKeyValueKind = RegistryValueKind.String;
                        }
                        else
                            regKeyValueKind = RegistryValueKind.DWord;

                        if(item._state == "Enabled" || item._state== "Disabled")
                        {
                            item._state = "Not Configured";
                            
                        }
                        item._keyValue = null;
                        sub.SetValue(item._keyName, item._keyValue, regKeyValueKind);
                        sub.Close();
                    }
                    catch (ArgumentException e) { }
                }

                softwareKey.Close();
                masterKey.Close();
                onChanged(nameof(_gpoList));
                MessageBox.Show("Press Save Policies to save.");
            }

        }

        //Method to Enable all group policies
        public void EnableAllPoliciesButtonClick()
        {
            if (MessageBox.Show("Are you sure you want to Enable All?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Create an instance of HKEY_CURRENT_USER registry key
                RegistryKey masterKey = Registry.CurrentUser;
                // Open the Software registry sub key under the HKEY_CURRENT_USER parent key (read only)
                RegistryKey softwareKey = masterKey.OpenSubKey("SOFTWARE", true);
                RegistryValueKind regKeyValueKind;
                foreach (var item in _gpoList)
                {
                    RegistryKey sub = softwareKey.CreateSubKey(item._regPath);
                    try
                    {
                        if (item._keyValueKind == "String")
                        {
                            regKeyValueKind = RegistryValueKind.String;
                        }
                        else
                            regKeyValueKind = RegistryValueKind.DWord;
                        if (item._state == "Disabled" || item._state == "Not Configured")
                        {
                            item._state = "Enabled";
                        }
                        item._keyValue = item._enabledValue;

                        sub.SetValue(item._keyName, item._keyValue, regKeyValueKind);
                        sub.Close();
                    }
                    catch (ArgumentException e) { }
                }

                softwareKey.Close();
                masterKey.Close();
                onChanged(nameof(_gpoList));
                MessageBox.Show("Press Save Policies to save.");
            }

        }

        //Method to Disable all group policies
        public void DisableAllPoliciesButtonClick()
        {
            if (MessageBox.Show("Are you sure you want to Disable All?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Create an instance of HKEY_CURRENT_USER registry key
                RegistryKey masterKey = Registry.CurrentUser;
                // Open the Software registry sub key under the HKEY_CURRENT_USER parent key (read only)
                RegistryKey softwareKey = masterKey.OpenSubKey("SOFTWARE", true);
                RegistryValueKind regKeyValueKind;
                foreach (var item in _gpoList)
                {
                    RegistryKey sub = softwareKey.CreateSubKey(item._regPath);
                    try
                    {
                        if (item._keyValueKind == "String")
                        {
                            regKeyValueKind = RegistryValueKind.String;
                        }
                        else
                            regKeyValueKind = RegistryValueKind.DWord;
                        if (item._state == "Enabled" || item._state == "Not Configured")
                        {
                            item._state = "Disabled";
                        }
                        item._keyValue = item._disabledValue;

                        sub.SetValue(item._keyName, item._keyValue, regKeyValueKind);
                        sub.Close();
                    }
                    catch (ArgumentException e) { }
                }

                softwareKey.Close();
                masterKey.Close();
                onChanged(nameof(_gpoList));
                MessageBox.Show("Press Save Policies to save.");
            }
        }

        //Method to Save all group policies to the state specified in the combobox value on the UI view class
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
                {
                    regKeyValueKind = RegistryValueKind.DWord;
                }
                    
                if (item._state == "Disabled")
                {
                    try
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
                    catch(ArgumentException e) { }
                    
                    
                }
                if (item._state == "Enabled")
                {
                    try
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
                    catch (ArgumentException e) { }
                    
                }
                if (item._state == "Not Configured" || item._state == null)
                {
                    if (item._multipleKeys != null)
                    {
                        string[] multipleKeys = item._multipleKeys.Split(',');
                        foreach (string keyname in multipleKeys)
                        {
                            item._keyName = keyname;
                            try
                            {
                                if (sub.GetValue(item._keyName) != null)
                                {
                                    sub.DeleteValue(item._keyName);
                                }
                            }
                            catch (ArgumentException e)
                            {
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            if (sub.GetValue(item._keyName) != null)
                            {
                                sub.DeleteValue(item._keyName);
                            }
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
            UpdatePolicies();
        }
    }
}