using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
        public int ReleaseYear { get; set; }
        public string PosterImage { get; set; }
        public bool isDeleted{ get; set; }
        public string MovieLink { get; set; }
        [ForeignKey("User")]
        public int Id { get; set; }
        public User User { get; set; }
     
    }
}
