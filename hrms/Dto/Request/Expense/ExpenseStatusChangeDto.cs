using hrms.Model;

namespace hrms.Dto.Request.Expense
{
    public class ExpenseStatusChangeDto
    {
        public string Status { get; set; }
        public string? Remarks { get; set; }
    }
}
