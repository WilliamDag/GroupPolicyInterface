using Microsoft.Win32;
using System;
using System.Windows;

namespace GroupPolicyInterface.Models
{
    class GroupPolicy
    {
        //Group Policy properties
        public string _type { get; set; }
        public string _name { get; set; }
        public string _shortDescription { get; set; }
        public string _longDescription { get; set; }
        //Path in the registry
        public string _regPath { get; set; }
        //Name of the registry key
        public string _keyName { get; set; }
        //State of the key - enabled / disabled / not configured
        public string _state { get; set; }
        //The key value - Can be anything but usually 1 or 0
        public string _keyValue { get; set; }
        //The value kind - string / Dword
        public string _keyValueKind { get; set; }
        //The value that means the GPO is enabled
        public string _enabledValue { get; set; }
        //The value that means the GPO is disabled
        public string _disabledValue { get; set; }
        public string _multipleKeys { get; set; }

        //Default constructor
        public GroupPolicy(string type, string name, string shortDescription, string longDescription, string regPath, string keyName, string keyValueKind, string enabledValue, string disabledValue)
        {
            //Set the object values to those passed through the constructor - this is specified in the UpdatePolicies method in the ReadPolicyViewModel
            _type = type;
            _name = name;
            _shortDescription = shortDescription;
            _longDescription = longDescription;
            _regPath = regPath;
            //If the keyname constains a ',' then it has multiple Registry keys for one group policy
            if(keyName.Contains(","))
            {
                _multipleKeys = keyName;
                //Create an array for the key names if there are multiple
                string[] multipleKeys = keyName.Split(',');
                //For each  of the key names, set the object value
                foreach (string keyname in multipleKeys)
                {
                    _keyName = keyname;
                }
            }
            //If not multiple, then just set the object value
            else
            {
                _keyName = keyName;
            }
            //Specify what value kind the key is, E.g. String, DWord
            _keyValueKind = keyValueKind;
            //Specify the enabled value to be compared in the registry - Most are '1' for enabled, but not all.
            _enabledValue = enabledValue;
            //Specify the disabled value to be compared in the registry - Most are '0' for enabled, but not all.
            _disabledValue = disabledValue.Substring(0, disabledValue.Length - 1);
            // Create an instance of HKEY_CURRENT_USER registry key
            RegistryKey masterKey = Registry.CurrentUser;
            // Open the Software registry sub key under the HKEY_CURRENT_USER parent key (read only)
            RegistryKey softwareKey = masterKey.OpenSubKey("SOFTWARE", true);
            RegistryKey sub = softwareKey.CreateSubKey(_regPath);

            //If the registry key value exists (is not null) then check if the value is equal to the disable value or enabled value
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
            //If the registry key value is null then the GPO is not configured
            else
                _state = "Not Configured";
            //Close the registry keys
            sub.Close();
            softwareKey.Close();
            masterKey.Close();
        }
    }
}