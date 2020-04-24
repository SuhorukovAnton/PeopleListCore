using System;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using NLog;
using PeopleListCore.Helpers;
using PeopleListCore.Models;

namespace PeopleList.Core
{
    public class PeopleXmlReader : IReader
    {
        public Logger Logger { get; set; }
        public PeopleXmlReader()
        {
           Logger = LogManager.GetCurrentClassLogger();
        }

        public async Task AddPeople(string path)
        {
            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true
            };

            try
            {
                using (var reader = XmlReader.Create(path, settings))
                {
                    reader.ReadToFollowing("People");
                    while (reader.Name == "People")
                    {
                        var people = new People();
                        people.ReadXml(reader);
                        if (!await HelperConnect.FindEmail(people.Email))
                        {
                            await HelperConnect.AddPeople(people);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Wrong format XML:" + e.Message);
            }
        }

        public void Create(string path)
        {
            var peoples = HelperConnect.GetPeoples();
            var settings = new XmlWriterSettings
            {
                Indent = true
            };
            using (var writer = XmlWriter.Create(path, settings))
            {
                writer.WriteStartElement("Peoples");
                peoples.ForEach((people) =>
                {
                    writer.WriteStartElement("People");
                    people.WriteXml(writer);
                    writer.WriteEndElement();
                });
                writer.WriteEndElement();
            }
        }

    }
}