using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using PeopleListCore.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PeopleListCore.Models
{

    public class ValidPassword : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var pass = value as string;
            if (pass == null)
            {
                return new ValidationResult(GetErrorMessage(validationContext));
            }
            var isLower = false;
            var isUpper = false;
            var isDigit = false;
            var isPunctuation = false;
            var isWhiteSpace = false;
            foreach (var c in pass)
            {
                isLower = char.IsLetter(c) && char.IsLower(c) || isLower;
                isUpper = char.IsLetter(c) && char.IsUpper(c) || isUpper;
                isDigit = char.IsDigit(c) || isDigit;
                isPunctuation = char.IsPunctuation(c) || isPunctuation;
                isWhiteSpace = char.IsWhiteSpace(c) || isWhiteSpace;
            }
           
            if (isLower && isUpper && isDigit && isPunctuation && !isWhiteSpace)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(GetErrorMessage(validationContext));
            }
        }
        private string GetErrorMessage(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                return "Invalid CPF";
            }

            ErrorMessageTranslationService errorTranslation = validationContext.GetService(typeof(ErrorMessageTranslationService)) as ErrorMessageTranslationService;
            return errorTranslation.GetLocalizedError(ErrorMessage);
        }
    }

    public class ValidBirthday : ValidationAttribute
    {
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                 return new ValidationResult(GetErrorMessage(validationContext)); 
            }
            try
            {
                var convertedDate = Convert.ToDateTime(value);
                var nowDate = DateTime.Now;
                if (convertedDate < nowDate)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(GetErrorMessage(validationContext));
                }
            }
            catch
            {
                 return new ValidationResult(GetErrorMessage(validationContext)); 
            }
        }
        private string GetErrorMessage(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                return "Invalid CPF";
            }

            ErrorMessageTranslationService errorTranslation = validationContext.GetService(typeof(ErrorMessageTranslationService)) as ErrorMessageTranslationService;
            return errorTranslation.GetLocalizedError(ErrorMessage);
        }
    }

    public class ValidEmail : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;
            if (email == null)
            {
                return new ValidationResult(GetErrorMessage(validationContext));
            }
            var pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";
            var isMatch = Regex.Match(email.ToLower(), pattern, RegexOptions.IgnoreCase);
            if (isMatch.Success)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(GetErrorMessage(validationContext));
            }
        }
        private string GetErrorMessage(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                return "Invalid CPF";
            }

            ErrorMessageTranslationService errorTranslation = validationContext.GetService(typeof(ErrorMessageTranslationService)) as ErrorMessageTranslationService;
            return errorTranslation.GetLocalizedError(ErrorMessage);
        }
    }

    public class NoFindEmail : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;
            if (email == null)
            {
                return new ValidationResult(GetErrorMessage(validationContext));
            }
            else if (!HelperConnect.FindEmail(email).Result)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(GetErrorMessage(validationContext));
            }
        }
        private string GetErrorMessage(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                return "Invalid CPF";
            }

            ErrorMessageTranslationService errorTranslation = validationContext.GetService(typeof(ErrorMessageTranslationService)) as ErrorMessageTranslationService;
            return errorTranslation.GetLocalizedError(ErrorMessage);
        }
    }

    public class ErrorMessageTranslationService
    {
        private readonly IStringLocalizer<Resource> _sharedLocalizer;
        public ErrorMessageTranslationService(IStringLocalizer<Resource> sharedLocalizer)
        {
            _sharedLocalizer = sharedLocalizer;
        }

        public string GetLocalizedError(string errorKey)
        {
            return _sharedLocalizer[errorKey];
        }
    }

}
