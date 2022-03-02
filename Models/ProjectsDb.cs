using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace UpdatingProjects.Models
{
    [Table("ProjectsDb")]
    public partial class ProjectsDb
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string ProjectCategory { get; set; }
        [StringLength(100)]
        public string ProjectTitle { get; set; }
        [StringLength(30)]
        public string ImageCategory { get; set; }
        [StringLength(100)]
        public string ImageTitle { get; set; }
        [StringLength(300)]
        public string Url { get; set; }
        
        [NotMapped]
        public IFormFile ImageFile { get; set; }

        [NotMapped]
        public string ImageName { get; set; }
    }
}
