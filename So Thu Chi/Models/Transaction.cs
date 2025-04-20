using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace So_Thu_Chi.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "please select category")]
        public int CategoryId { get; set; }

        //Add ? mark for validation ignore
        public Category? Category { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "please select amount")]
        public int Amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? CategoryWithIcon
        {
            get
            {
                return Category == null ? "" : Category.Icon + " " + Category.Title;
            }
        }
        
        [NotMapped]
        public string? FormattedAmount
        {
            get
            {
                return ((Category == null || Category.Title == "Expense") ? "- " : "+ ") + Amount.ToString("N0") + "đ";
            }
        }
    }
}
