using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Validations
{
    public class FileTypeValidation : ValidationAttribute
    {
        private readonly string[] validTypes;

        public FileTypeValidation(string[] validTypes)
        {
            this.validTypes = validTypes;
        }

        public FileTypeValidation(GroupFileType groupFileType)
        {
            if (groupFileType == GroupFileType.Imagen)
            {
                validTypes = new string[] { "image/jpeg","image/png","image/gif" };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is IFormFile formFile)
            {
                if (!validTypes.Contains(formFile.ContentType))
                {
                    return new ValidationResult($"File type must be one of next {string.Join(", ", validTypes)}");
                }
            }
            return ValidationResult.Success;
        }

    }
}
