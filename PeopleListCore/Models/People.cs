
using Newtonsoft.Json;
using System.Xml;
using System.ComponentModel.DataAnnotations;
using System;

namespace PeopleListCore.Models
{
    public class People
    {
        [JsonIgnore]
        public int id { set; get; }
        [Required(ErrorMessage = "NameRequired")]
        public string Name { set; get; }
        [Required(ErrorMessageResourceName = "SurnameRequired")]
        public string Surname { set; get; }
        [Required( ErrorMessageResourceName = "EmailRequired")]
        [DataType(DataType.EmailAddress)]
        [ValidEmail( ErrorMessageResourceName = "EmailValid")]
        public string Email { set; get; }
        [Required( ErrorMessageResourceName = "BirthdayRequired")]
        [DataType(DataType.Date)]
        [ValidBirthday( ErrorMessageResourceName = "BirthdayValid")]
        public DateTime Birthday { set; get; }
        [JsonIgnore]
        public string Password { set; get; }
        [JsonIgnore]
        public string Img { set; get; }
        public Roles Role { set; get; }

        public void ReadXml(XmlReader r)
        {
            r.ReadStartElement();
            Name = r.ReadElementContentAsString(nameof(Name), "");
            Surname = r.ReadElementContentAsString(nameof(Surname), "");
            Email = r.ReadElementContentAsString(nameof(Email), "");
            Birthday = Convert.ToDateTime(r.ReadElementContentAsString(nameof(Birthday), ""));
            Role = (Roles)int.Parse(r.ReadElementContentAsString(nameof(Role), ""));
            r.ReadEndElement();
        }

        public void WriteXml(XmlWriter w)
        {
            w.WriteElementString("Name", Name);
            w.WriteElementString("Surname", Surname);
            w.WriteElementString("Email", Email);
            w.WriteElementString("Birthday", Birthday.ToString());
            w.WriteElementString("Role", ((int)Role).ToString());
        }
    }
}
