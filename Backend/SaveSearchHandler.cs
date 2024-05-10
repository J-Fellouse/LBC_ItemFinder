using ItemFinder_WPF.Backend.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ItemFinder_WPF.Backend
{
    public class SaveSearchHandler
    {
        private string _jsonSaveFilePath;
        private Parameters _parameters;

        public SaveSearchHandler(string jsonSaveFilePath, Parameters parameters = null)
        {
            _jsonSaveFilePath = jsonSaveFilePath;
            _parameters = parameters;
        }

        public void createFile()
        {
            List<Parameters> parametersList = new List<Parameters>();
            parametersList.Add(_parameters);
            saveParametersToFile(parametersList);
        }

        public bool isConfigurationFile()
        {
            return File.Exists(_jsonSaveFilePath);
        }

        public void saveParametersToFile(List<Parameters> data)
        {
            string jsonTextData = JsonConvert.SerializeObject(data);
            File.WriteAllText(_jsonSaveFilePath, jsonTextData);
        }

        public void replaceEntry(string uuid)
        {
            List<Parameters> data = getEntries();
            int index = data.IndexOf(data.Where(param => param.UUID == uuid).ToList()[0]);

            if (index != -1)
            {
                data[index] = _parameters;
            }

            saveParametersToFile(data);
        }

        public void removeEntry(string uuid)
        {
            List<Parameters> data = getEntries();
            int index = data.IndexOf(data.Where(param => param.UUID == uuid).ToList()[0]);
            
            if(index != -1)
            {
                data.RemoveAt(index);
            }

            saveParametersToFile(data);
        }

        public void addEntry()
        {
            List<Parameters> data = getEntries();
            data.Add(_parameters);

            saveParametersToFile(data);
        }

        public List<Parameters> getEntries()
        {
            List<Parameters> data = JsonConvert.DeserializeObject<List<Parameters>>(File.ReadAllText(_jsonSaveFilePath));
            return data;
        }

        public void handleNewSaveSearch()
        {
            if(isConfigurationFile())
            {
                addEntry();
            }
            else
            {
                createFile();
            }
        }
    }
}
