using Microsoft.Win32;

namespace GroupPolicyInterface.Models
{
    class GroupPolicy
    {
        public string _name { get; set; }
        public string _shortDescription { get; set; }
        public string _longDescription { get; set; }
        public string _regPath { get; set; }
        public string _keyName { get; set; }
        public bool? _state { get; set; }
        public string _keyValue { get; set; }

        public GroupPolicy(string name, string shortDescription, string longDescription, string regPath, string keyName)
        {
            _name = name;
            _shortDescription = shortDescription;
            _longDescription = longDescription;
            _regPath = regPath;
            _keyName = keyName.Substring(0, keyName.Length -1);
            // Create an instance of HKEY_CURRENT_USER registry key
            RegistryKey masterKey = Registry.CurrentUser;
            // Open the Software registry sub key under the HKEY_CURRENT_USER parent key (read only)
            RegistryKey softwareKey = masterKey.OpenSubKey("SOFTWARE", true);
            RegistryKey sub = softwareKey.CreateSubKey(_regPath);
            
            if (sub.GetValue(_keyName) != null)
            {
                if (sub.GetValue(_keyName).ToString() == "0")
                {
                    _state = false;
                }
                else if (sub.GetValue(_keyName).ToString() == "1")
                {
                    _state = true;
                }
            }
            else
                _state = null;
            sub.Close();
            softwareKey.Close();
            masterKey.Close();
        }
    }
}