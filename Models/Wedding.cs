using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId{get; set;}


        [Required(ErrorMessage="Wedder One is required!")]
        [MinLength(2, ErrorMessage="Wedder names must be at least 2 characters long")]
        public string Wedder1{get; set;}


        [Required(ErrorMessage="Wedder One can't marry themselves!")]
        [MinLength(2, ErrorMessage="Wedder names must be at least 2 characters long")]
        public string Wedder2{get; set;}


        [Required(ErrorMessage="A wedding date is required...")]
        public DateTime Date{get; set;}

        [Required(ErrorMessage="Wedding address is required")]
        public string Address{get; set;}


        [Required]
        public int UserId {get; set;}

        public User Creator {get; set;}


        public List<Association> Wedders{get; set;}


        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}