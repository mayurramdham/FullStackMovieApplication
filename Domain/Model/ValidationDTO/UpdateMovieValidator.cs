using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.ValidationDTO
{
    public class UpdateMovieValidator:AbstractValidator<UpdateMovieDto>
    {
        public UpdateMovieValidator()
        {
            RuleFor(x => x.MovieTitle).NotEmpty().WithMessage("Movie title must not be empty.");
            RuleFor(x => x.ReleaseYear).Must(HaveFourDigits).GreaterThan(1900).WithMessage("Release year must be greater than 1900.");
            //RuleFor(x => x.PosterImage).Must(BeAValidImageType).WithMessage("posterImage not selected as  of type \".jpg\", \".jpeg\", \".png\", \".gif\"");
            // RuleFor(x => x.PosterImage).Must(BeUnderMaxSize).WithMessage("posterImage not greater than of size 5MB");
       

        }
        private bool HaveFourDigits(int year) { return year >= 1000 && year <= 9999; }
        private bool BeAValidImageType(IFormFile PosterImage)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(PosterImage.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        // Custom validation for file size (max size of 5 MB)
        private bool BeUnderMaxSize(IFormFile PosterImage)
        {
            const int maxFileSizeInBytes = 5 * 1024 * 1024; // 5MB
            return PosterImage.Length <= maxFileSizeInBytes;
        }

    }
}

