using System;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class UpdateMovieDto
    {
        public int MovieId { get; set; }

        public string? MovieTitle { get; set; }

        public string? MovieLink { get; set; }

        public int ReleaseYear { get; set; }

        public IFormFile? PosterImage { get; set; }
    }
}
