using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace YUR.SDK.Unity.Utilities.Config
{
    internal class YUR_ConfigurationManager
    {
        public static bool SaveConfig<T>(T classToSave, string filePath)
        {
            string fileText = YUR_Conversions.ConvertObjectToString<T>(classToSave);
            File.WriteAllText(filePath, fileText);
            
            return true;
        }

        public static bool LoadConfig<T>(T @object, string filePath)
        {
            string fileText = File.ReadAllText(filePath);
            @object = YUR_Conversions.ConvertStringToObject<T>(fileText);
            return true;
        }
    }

    public class Editor
    {


        public string FilePath { get; }
        public PrintSettings pSettings = new PrintSettings();
        public event Action<Editor> ConfigChangedEvent;

        private readonly FileSystemWatcher _configWatcher;


        private bool _saving;

        public Editor(string filePath)
        {
            FilePath = filePath;
            if (File.Exists(FilePath))
            {
                Load();
                var text = File.ReadAllText(FilePath);
                Save();
            }
            else
            {
                Save();
            }

            _configWatcher = new FileSystemWatcher(Environment.CurrentDirectory)
            {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "YUR.cfg",
                EnableRaisingEvents = true
            };
            _configWatcher.Changed += _configWatcher_Changed; ;
        }

        private void _configWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (_saving)
            {
                _saving = false;
                return;
            }

            Load();

            ConfigChangedEvent?.Invoke(this);
        }

        public void Save()
        {

            _saving = true;
            Utilities.Config.YUR_ConfigurationManager.SaveConfig(this, FilePath);
            _saving = false;
            //ConfigSerializer.SaveConfig(this, FilePath);
        }

        public void Load()
        {
            Utilities.Config.YUR_ConfigurationManager.LoadConfig(this, FilePath);
        }


        public class PrintSettings
        {
            public bool Debugging = false;
            public bool Error_Reporting = true;

        }
    }
}

