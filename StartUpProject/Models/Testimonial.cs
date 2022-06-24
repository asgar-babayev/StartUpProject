using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StartUpProject.Models
{
    public class Testimonial
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        public string UserName { get; set; }
        public string CompanyName { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
