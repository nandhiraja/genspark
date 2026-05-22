using System.ComponentModel.DataAnnotations;

namespace BankingAPI.Models.DTOs
{
    public class CreateAccountRequest
    {
        public float Balance { get; set; }
        public string AccountType { get; set; } = "Savings";
        public int CustomerId { get; set; }

    }
}
