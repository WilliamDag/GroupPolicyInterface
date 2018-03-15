﻿using System;
using System.Windows.Input;
using GroupPolicyInterface.Commands;
using GroupPolicyInterface.Models;
using GroupPolicyInterface.ViewModels;
using GroupPolicyInterface.Views;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Windows;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace GroupPolicyInterface.ViewModels
{
    class ReadPolicyViewModel : BaseViewModel
    {
        private ObservableCollection<GroupPolicy> _gpoList = new ObservableCollection<GroupPolicy>();
        public ObservableCollection<GroupPolicy> gpoList
        {
            get { return _gpoList; }
            set { _gpoList = value; }
        }

        public ICommand ReadButtonCommand { get; set; }
        public string textReadButton { get; set; }

        public ReadPolicyViewModel()
        {
            textReadButton = "Read Policies";

            ReadButtonCommand = new RelayCommand(ReadButtonClick);
            

            /*
            // Create an instance of HKEY_CURRENT_USER registry key
            RegistryKey masterKey = Registry.CurrentUser;
            // Open the Internet Explorer registry sub key under the HKEY_CURRENT_USER parent key (read only)
            RegistryKey iexplorerKey = masterKey.OpenSubKey("SOFTWARE", true);
            RegistryKey subKey = iexplorerKey.CreateSubKey(@"Policies\Microsoft\Internet Explorer\Restrictions");
            subKey.SetValue("NoBrowserSaveAs", 1, RegistryValueKind.DWord);
            Console.WriteLine(subKey.GetValue("NoBrowserSaveAs"));
            subKey.Close();
            iexplorerKey.Close();
            masterKey.Close();
            */
        }




        private void ReadButtonClick()
        {
            GroupPolicy groupPolicy = new GroupPolicy();
        }
    }
}


/*
private static void UpdateGPO()
{
    try
    {
        Process proc = new Process();
        ProcessStartInfo procStartInfo = new ProcessStartInfo(@"cmd.exe", "/c" + "gpupdate/force");
        procStartInfo.RedirectStandardOutput = true;
        procStartInfo.UseShellExecute = false;
        procStartInfo.CreateNoWindow = true;
        procStartInfo.LoadUserProfile = true;
        proc.StartInfo = procStartInfo;
        proc.Start();
        proc.WaitForExit();
    }
    catch (Exception ex)
    {

    }
}
*/