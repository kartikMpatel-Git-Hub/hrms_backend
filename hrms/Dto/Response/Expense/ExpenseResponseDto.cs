using hrms.Dto.Response.Expense.Category;
using hrms.Dto.Response.Expense.Proof;
using hrms.Model;

namespace hrms.Dto.Response.Expense
{
    public class ExpenseResponseDto
    {
        public int Id { get; set; }
        public int TravelId { get; set; }
        public int TravelerId { get; set; }
        public decimal Amount { get; set; }
        public ExpenseCategoryResponseDto Category { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string Details { get; set; }
        public DateTime ExpenseDate { get; set; }
        public List<ExpenseProofResponseDto> proofs { get; set; } = new List<ExpenseProofResponseDto>();
    }
}
