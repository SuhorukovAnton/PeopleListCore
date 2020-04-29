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
        private Logger logger { get; set; }
        private PeopleManager manager;
        public PeopleXmlReader(PeopleManager manager)
        {
            this.manager = manager;
            logger = LogManager.GetCurrentClassLogger();
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
                        if (!await manager.FindEmail(people.Email))
                        {
                            await manager.AddPeople(people);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Wrong format XML:" + e.Message);
            }
        }

        public void Create(string path)
        {
            var peoples = manager.GetPeoples();
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