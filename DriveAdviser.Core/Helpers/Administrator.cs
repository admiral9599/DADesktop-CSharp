﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

namespace DriveAdviser.Core.Helpers
{
    public class Administrator
    {
        public static bool IsUserWindowsAdministrator()
        {
            var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                return true;
            return false;
        }

        public static void RestartAsAdminAndExit(string arguments = "")
        {
            var psi = new ProcessStartInfo
            {
                Arguments = arguments,
                //ErrorDialog = true,
                //ErrorDialogParentHandle = Utils.GetMainAppForm().Handle,
                FileName = Assembly.GetCallingAssembly().Location,
                Verb = "runas"
            };
            try
            {
                Process.Start(psi);
            }


            catch (Win32Exception ex)
            {
                Console.WriteLine(ex.NativeErrorCode == 1223 ? "Why" : "Cool");
            }
        }

        public static bool IsProcessOpen(string name)
        {
            //here we're going to get a list of all running processes on
            //the computer
            var openProcess = 0;
            var openProcesses = Process.GetProcesses();
            foreach (var clsProcess in openProcesses)
                if (clsProcess.ProcessName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    openProcess++;
            //now we're going to see if any of the running processes
            //match the currently running processes. Be sure to not
            //add the .exe to the name you provide, i.e: NOTEPAD,
            //not NOTEPAD.EXE or false is always returned even if
            //notepad is running.
            //Remember, if you have the process running more than once, 
            //say IE open 4 times the loop thr way it is now will close all 4,
            //if you want it to just close the first one it finds
            //then add a return; after the Kill
            if (openProcess > 1)
                return true;

            return false;
        }

        //otherwise we return a false
    }
}