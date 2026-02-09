using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace hrms.Model
{
    public enum ExpenseStatus
    {
        PENDING,
        APPROVED,
        REJECTED
    }
    public class Expense : BaseEntity
    {
        public int Id { get; set; }
        public int TravelId { get; set; }
        public Travel Travel { get; set; }
        public int TravelerId { get; set; }
        public User Traveler { get; set; }
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public ExpenseCategory Category { get; set; }
        public ExpenseStatus Status { get; set; }
        public string Remarks { get; set; }

        public List<ExpenseProof> Proofs { get; set; } = new List<ExpenseProof>();
    }
}
