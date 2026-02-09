using hrms.Model;

namespace hrms.Dto.Request.Expense
{
    public class ExpenseCreateDto
    {
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public string? Remarks { get; set; }
        public List<IFormFile> Proofs { get; set; }
    }
}
