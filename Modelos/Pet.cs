using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendLimpio.Models
{
    public class Pet
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string Species { get; set; } = "";

        public string Breed { get; set; } = "";

        public int Age { get; set; }

        public string Sex { get; set; } = "";

        [Column("UserId")]
        public Guid UserId { get; set; }

        
    }
}