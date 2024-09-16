using DriveAdviser.Core.Services;
using DriveAdviser.Services;
using Microsoft.Win32.TaskScheduler;
using System;
using System.IO;
using System.Reflection;

namespace DriveAdviser.Helpers
{
    public static class TaskScheduler
    {
        public static Task GetTask(string nameOfTask)
        {
            var task = new TaskService();

            return task.GetTask(nameOfTask);
        }

        public static void CreateTask()
        {
            try
            {
                var fileName = Assembly.GetCallingAssembly().Location;
                var workingDir = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
                var username = Environment.UserDomainName + "\\" + Environment.UserName;
                var task = new TaskService();
                var t = task.GetTask("Drive Adviser");
                
                if (t != null) return;

                var td = TaskService.Instance.NewTask();
                td.Principal.RunLevel = TaskRunLevel.Highest;
                td.RegistrationInfo.Author = "Schrock Innovations";
                td.RegistrationInfo.Date = new DateTime();

                var logon = new LogonTrigger
                {
                    Delay = new TimeSpan(0, 0, 0, 30)
                };

                td.Triggers.Add(logon);
                td.Actions.Add(fileName, "/tray", workingDir);


                TaskService.Instance.RootFolder.RegisterTaskDefinition("Drive Adviser", td);
            }
            catch (Exception e)
            {
                var api = new DriveAdviserApi();
                var settings = new Settings();

                api.LogException(e.Message, e.StackTrace, e.InnerException?.Message, e.InnerException?.StackTrace, settings.GetSettingValueString(Headers.Computer, Names.Id));

            }
        }
    }
}