
using Newtonsoft.Json;
using NLog;
using PeopleListCore.Helpers;
using PeopleListCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace PeopleList.Core
{
    public class PeopleJSONReader : IReader
    {
        public Logger logger = LogManager.GetCurrentClassLogger();
        public async Task AddPeople(string path)
        {
            try
            {
                var text = "";
                using (var sr = new StreamReader(path, Encoding.UTF8))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        text += line;
                    }
                }
                var peoples = JsonConvert.DeserializeObject<List<People>>(text);
                foreach (var people in peoples)
                {
                    if (!await HelperConnect.FindEmail(people.Email)) {

                        await HelperConnect.AddPeople(people);
                    }
                }
            }catch(Exception e)
            {
                logger.Error("Wrong JSON format: " + e.Message);
            }
        }

        public void Create(string path)
        {
            var peoples = HelperConnect.GetPeoples();
            var json = JsonConvert.SerializeObject(peoples);
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.WriteLine(json);
            }
        }

    }
}