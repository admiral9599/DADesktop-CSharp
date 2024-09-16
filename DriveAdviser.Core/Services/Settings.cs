using DriveAdviser.Core;
using Nini.Config;
using System;

namespace DriveAdviser.Services
{
    public enum Headers
    {
        User,
        Update,
        Drives,
        App,
        Computer
    }

    public enum Names
    {
        EmailAddress,
        Saved,
        Version,
        Debug,
        Build,
        Id
    }

    public enum BuildType
    {
        Public,
        Schrock
    }

    public class Settings
    {
      
        // private string settingsName = "DriveAdviser.ini";

        //private IConfigSource getConfig;
        //private IConfig setConfig;
        private  readonly IniConfigSource settings;
        

        public Settings()
        {
            try
            {
                settings = new IniConfigSource(Constants.IniFullPath);
                var cversion = GetSettingValueInt("Update", "Version");
                if (cversion < SettingsVersion)
                {
                    UpdateSettingsFile();
                    settings = new IniConfigSource(Constants.IniFullPath);
                }
            }
            catch (Exception)
            {
                //throw exception if file not found
                //Create Settings File 
                CreateDefaultSettingsFile();

                //reinstatiate the config so the autosave isn't null
                settings = new IniConfigSource(Constants.IniFullPath);
            }
            settings.AutoSave = true;
        }

        
        public int SettingsVersion { get; } = 11;

        private void UpdateSettingsFile()
        {
            IConfigSource mainSource = new IniConfigSource(Constants.IniFullPath);

            CreateDefaultSettingsFile();
            IConfigSource newSource = new IniConfigSource(Constants.IniFullPath);
            newSource.Merge(mainSource);
            newSource.Configs[Headers.Update.ToString()].Set(Names.Version.ToString(),SettingsVersion);
            newSource.Save();
        }

        private void CreateDefaultSettingsFile()
        {
            var settingsCreate = new IniConfigSource();

            var config = settingsCreate.AddConfig(Headers.User.ToString());
            config.Set(Names.EmailAddress.ToString(), "");
            config.Set(Names.Saved.ToString(), false);
            config = settingsCreate.AddConfig(Headers.Update.ToString());
            config.Set(Names.Version.ToString(), SettingsVersion);

            settingsCreate.AddConfig(Headers.Drives.ToString());
            config = settingsCreate.AddConfig(Headers.App.ToString());
            config.Set(Names.Debug.ToString(), false);
            config.Set(Names.Build.ToString(),BuildType.Public.ToString());
            config = settingsCreate.AddConfig(Headers.Computer.ToString());
            config.Set(Names.Saved.ToString(),false);
            config.Set(Names.Id.ToString(),string.Empty);
            settingsCreate.Save(Constants.IniFullPath);
        }

        public void UpdateAndCreateSetting(string settingHeader, string settingName, object settingValue)
        {
            settings.Configs[settingHeader].Set(settingName, settingValue);
        }

        public void UpdateAndCreateSetting(Headers settingHeader, Names settingName, object settingValue)
        {
            settings.Configs[settingHeader.ToString()].Set(settingName.ToString(), settingValue);
        }

        public void UpdateAndCreateSetting(Headers settingHeader, string settingName, object settingValue)
        {
            settings.Configs[settingHeader.ToString()].Set(settingName, settingValue);
        }

        public string GetSettingValueString(Headers settingHeader, Names settingName)
        {
            return settings.Configs[settingHeader.ToString()].GetString(settingName.ToString());
        }

        public static BuildType GetBuildType()
        {
            return  (BuildType)Enum.Parse(typeof(BuildType),new IniConfigSource(Constants.IniFullPath).Configs[Headers.App.ToString()].GetString(Names.Build.ToString()));
        }

        public string GetSettingValueString(Headers settingHeader, string settingName)
        {
            return settings.Configs[settingHeader.ToString()].GetString(settingName);
        }


        public int GetSettingValueInt(string settingHeader, string settingName)
        {
            return settings.Configs[settingHeader].GetInt(settingName);
        }

        public int GetSettingValueInt(Headers settingHeader, Names settingName)
        {
            return settings.Configs[settingHeader.ToString()].GetInt(settingName.ToString());
        }


        public bool GetSettingValueBool(Headers settingHeader, Names settingName)
        {
            return settings.Configs[settingHeader.ToString()].GetBoolean(settingName.ToString());
        }

        public bool GetSettingValueBool(string settingHeader, string settingName)
        {
            return settings.Configs[settingHeader].GetBoolean(settingName);
        }
    }
}