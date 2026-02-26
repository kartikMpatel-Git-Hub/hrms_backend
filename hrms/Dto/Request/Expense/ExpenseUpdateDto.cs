namespace hrms.dto.request.Expense
{
    public class ExpenseUpdateDto
    {
        public decimal? Amount { get; set; }
        public int? CategoryId { get; set; }
        public string? Remarks { get; set; }
        public string? Details { get; set; }
        public DateTime ExpenseDate { get; set; }
    }
}