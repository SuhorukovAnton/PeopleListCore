

using System.IO;

namespace PeopleList.Core
{
    public class ReaderFactory
    {
        private PeopleJSONReader peopleJSON { get; set; }
        private PeopleXmlReader peopleXml { get; set; }

        public ReaderFactory(PeopleJSONReader peopleJSON, PeopleXmlReader peopleXml)
        {
            this.peopleJSON = peopleJSON;
            this.peopleXml = peopleXml;
        }
        internal IReader GetFactory(string format)
        {
            switch (format)
            {
                case "xml":
                    return peopleXml;
                case "json":
                    return peopleJSON;
            }
            throw new FileFormatException();
        }
    }
}