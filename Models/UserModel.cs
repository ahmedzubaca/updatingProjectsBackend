using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UpdatingProjects.Models
{
    public class UserModel
    {

        [Key]
        public int Id { get; set; } 
        
        [Column(TypeName ="nvarchar (30)")]
        public string Name { get; set; }
       
        [Column(TypeName ="nvarchar (100)")]
        
        [JsonIgnore]       
        public string Password { get; set; }        
    }
}
