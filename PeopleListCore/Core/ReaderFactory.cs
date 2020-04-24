

using System.IO;

namespace PeopleList.Core
{
    public class ReaderFactory
    {
        private PeopleJSONReader JSONReader { get; set; }
        private PeopleXmlReader XmlReader { get; set; }

        internal IReader GetFactory(string format)
        {
            switch (format)
            {
                case "xml":
                    return XmlReader ?? (XmlReader = new PeopleXmlReader());
                case "json":
                    return JSONReader ?? (JSONReader = new PeopleJSONReader());
            }
            throw new FileFormatException();
        }
    }
}