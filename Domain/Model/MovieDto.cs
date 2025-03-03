using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string MovieTitle { get; set; }
        public string MovieLink { get; set; }
   
        public int ReleaseYear { get; set; }
        //[ApiModelProperty(Required = true, Type = "file", Format = "binary", Description = "Accepted file types: .jpg, .jpeg, .png")]
        [SwaggerSchema(Format = "binary", Description = "Accepted file types: .jpg, .jpeg, .png")]
        public IFormFile PosterImage { get; set; }
    }
}
