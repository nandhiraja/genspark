using System.ComponentModel.DataAnnotations;

namespace BankingAPI.Models.DTOs
{
    public class SearchRequest
    {
        public int StartPageNo {get;set;}=0;
        public int ItemsPrePage {get;set;}=10;   // assume 10 is min
        public DateTime? FromDate {get;set;} = null;
        public DateTime? ToDate {get;set;} =null;
        public string CurrentUserAccountNo  {get;set;} =string.Empty;
        public string? TransferAccountNo {get; set;} = null;
        public string? SortBy {get;set;} ="";
        public bool SortByDesc {get;set;}= true;
        public Dictionary<string,int> AmountRange = new Dictionary<string, int>{{"from",0},{"to",0}};

    }
}