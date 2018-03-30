using Microsoft.Win32;
using System;
using System.Windows;

namespace GroupPolicyInterface.Models
{
    class GroupPolicy
    {
        public string _name { get; set; }
        public string _shortDescription { get; set; }
        public string _longDescription { get; set; }
        public string _regPath { get; set; }
        public string _keyName { get; set; }
        public string _state { get; set; }
        public string _keyValue { get; set; }
        public string _keyValueKind { get; set; }
        public string _enabledValue { get; set; }
        public string _disabledValue { get; set; }
        public string _multipleKeys { get; set; }

        public GroupPolicy(string name, string shortDescription, string longDescription, string regPath, string keyName, string keyValueKind, string enabledValue, string disabledValue)
        {
            _name = name;
            _shortDescription = shortDescription;
            _longDescription = longDescription;
            _regPath = regPath;
            if(keyName.Contains(","))
            {
                _multipleKeys = keyName;
                string[] multipleKeys = keyName.Split(',');
                foreach (string keyname in multipleKeys)
                {
                    _keyName = keyname;
                }
            }
            else
            {
                _keyName = keyName;
            }
            _keyValueKind = keyValueKind;
            _enabledValue = enabledValue;
            _disabledValue = disabledValue.Substring(0, disabledValue.Length - 1);
            // Create an instance of HKEY_CURRENT_USER registry key
            RegistryKey masterKey = Registry.CurrentUser;
            // Open the Software registry sub key under the HKEY_CURRENT_USER parent key (read only)
            RegistryKey softwareKey = masterKey.OpenSubKey("SOFTWARE", true);
            RegistryKey sub = softwareKey.CreateSubKey(_regPath);

            if (sub.GetValue(_keyName) != null)
            {
                if (sub.GetValue(_keyName).ToString() == _disabledValue)
                {
                    _state = "Disabled";
                }
                else if (sub.GetValue(_keyName).ToString() == _enabledValue)
                {
                    _state = "Enabled";
                }
            }
            else
                _state = "Not Configured";
            sub.Close();
            softwareKey.Close();
            masterKey.Close();
        }
    }
}