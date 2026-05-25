namespace BankingAPI.Models.DTOs
{
    public class SearchResponse
    {
        public List<Transaction> Accounts {get;set;} = new List<Transaction>();
    } 
}