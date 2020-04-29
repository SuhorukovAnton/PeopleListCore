
using System;
using System.ComponentModel.DataAnnotations;

namespace PeopleListCore.Models
{
    public class FormAuth
    {
        [Required(ErrorMessage = "EmailRequired")]
        [DataType(DataType.EmailAddress)]
        [ValidEmail( ErrorMessage = "EmailValid")]
        public string Email { get; set; }

        [Required( ErrorMessage = "PasswordRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class FormAdd
    {
        [Required( ErrorMessage = "NameRequired")]
        public string Name { set; get; }

        [Required( ErrorMessage = "SurnameRequired")]
        public string Surname { set; get; }

        [Required( ErrorMessage = "EmailRequired")]
        [DataType(DataType.EmailAddress)]
        [ValidEmail( ErrorMessage = "EmailValid")]
       
        public string Email { set; get; }

        [Required( ErrorMessage = "BirthdayRequired")]
        [DataType(DataType.Date)]
        [ValidBirthday( ErrorMessage = "BirthdayValid")]
        public DateTime Birthday { set; get; }

        [Required( ErrorMessage = "PasswordRequired")]
        [DataType(DataType.Password)]
        [MinLength(8,  ErrorMessage = "PasswordMin")]
        [MaxLength(20,  ErrorMessage = "PasswordMax")]
        [ValidPassword( ErrorMessage = "PasswordValid")]
        public string Password { set; get; }


    }
    public class FormEdit
    {
        public int Id { get; set; }

        [Required( ErrorMessage = "NameRequired")]
        public string Name { set; get; }

        [Required( ErrorMessage = "SurnameRequired")]
        public string Surname { set; get; }

        [Required( ErrorMessage = "EmailRequired")]
        [DataType(DataType.EmailAddress)]
        [ValidEmail( ErrorMessage = "EmailValid")]
        public string Email { set; get; }

        [Required( ErrorMessage = "BirthdayRequired")]
        [DataType(DataType.Date)]
        [ValidBirthday( ErrorMessage = "BirthdayValid")]
        public DateTime Birthday { set; get; }

    }
    
    
}
