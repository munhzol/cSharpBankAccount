using System;
using System.ComponentModel.DataAnnotations;

namespace BankAccount.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionID {get; set;}

        [Required]
        // [Range(1000,double.MaxValue)]
        public double Amount {get; set;}

        public DateTime CreatedAt {get; set;} = DateTime.Now;
        // public DateTime UpdatedAt {get; set;} = DateTime.Now;

        // [ForeignKey("User")]
        public int UserID {get; set;}
        public User Owner {get; set;}
    }
}