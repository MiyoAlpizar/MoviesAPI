using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Validations
{
    public class FileSizeValidation : ValidationAttribute
    {
        private readonly int maxSize;

        public FileSizeValidation(int MaxSize)
        {
            maxSize = MaxSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            if (value is IFormFile formFile)
            {
                if (formFile.Length > maxSize * 1024 * 1024)
                {
                    return new ValidationResult($"Image size must not be graater than {maxSize} mb");
                }
            }
            return ValidationResult.Success;
        }

    }
}
