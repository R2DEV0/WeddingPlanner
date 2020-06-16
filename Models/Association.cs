using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace WeddingPlanner.Models
{
    public class Association
    {
        [Key]
        public int AssociationId{get; set;}

        [Required]
        public int UserId{get; set;}

        public User User{get; set;}

        [Required]
        public int WeddingId{get; set;}

        public Wedding Wedding{get; set;}
    }
}