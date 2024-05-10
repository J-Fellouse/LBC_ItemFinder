using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemFinder_WPF.Backend
{
    public class SaveBlacklistHandler
    {
        string _jsonSaveFilePath;
        List<string> _blacklist;

        public SaveBlacklistHandler(string jsonSaveFilePath, List<string> blacklist = null)
        {
            _jsonSaveFilePath= jsonSaveFilePath;
            if(blacklist != null)
            {
                _blacklist = blacklist.ConvertAll(word => word.ToLower());
            }
        }

        public bool isConfigurationFile()
        {
            return File.Exists(_jsonSaveFilePath);
        }

        public void saveBlacklistToFile()
        {
            string jsonTextData = JsonConvert.SerializeObject(_blacklist);
            File.WriteAllText(_jsonSaveFilePath, jsonTextData);
        }

        public List<string> getEntries()
        {
            List<string> data = new List<string>();
            if(isConfigurationFile())
            {
                data = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_jsonSaveFilePath));
            }

            return data;
        }

        public void handleNewSaveBlacklist()
        {
            saveBlacklistToFile();
        }
    }
}
