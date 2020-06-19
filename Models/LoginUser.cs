using System.ComponentModel.DataAnnotations;

namespace BankAccount.Models
{
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        [Display(Name="Email")]
        public string LoginUserEmail {get; set;}

        [Required]
        [DataType(DataType.Password)]
        [Display(Name="Password")]
        public string LoginUserPassword {get; set;}

    }
}