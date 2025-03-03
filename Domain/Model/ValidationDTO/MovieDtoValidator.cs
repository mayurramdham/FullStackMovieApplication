using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.ValidationDTO
{
    public class MovieDtoValidator:AbstractValidator<MovieDto>
    {
        public MovieDtoValidator()
        {
            RuleFor(x => x.MovieTitle).NotEmpty().WithMessage("Movie title must not be empty.");
            RuleFor(x => x.ReleaseYear).Must(HaveFourDigits).GreaterThan(1900).WithMessage("Release year must be greater than 1900.");
            RuleFor(x => x.PosterImage).NotNull().WithMessage("Poster image must not be null.")
                      .WithMessage("Poster image must be a valid image file.");
        }
        private bool HaveFourDigits(int year) { return year >= 1000 && year <= 9999; }

    }
    }

