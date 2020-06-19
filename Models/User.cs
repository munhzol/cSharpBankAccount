using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccount.Models
{
    public class User
    {
        [Key]
        public int UserID {get; set;}

        [Required]
        [MinLength(4)]
        public string FirstName {get; set;}

        [Required]
        [MinLength(4)]
        public string LastName {get; set;}

        [Required]
        [EmailAddress]
        public string Email {get; set;}

        [Required]
        [DataType(DataType.Password)]
        public string Password {get; set;}

        [Required]
        [DataType(DataType.Password)]
        [NotMapped]
        [Compare("Password")]
        public string Confirm {get;set;}

        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpdatedAt {get; set;} = DateTime.Now;

        public List<Transaction> Transactions {get; set;}

    }
}