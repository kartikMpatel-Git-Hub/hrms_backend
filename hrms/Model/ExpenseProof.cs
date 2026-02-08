namespace hrms.Model
{
    public class ExpenseProof
    {
        public int Id { get; set; }
        public int ExpenseId { get; set; }
        public Expense Expense { get; set; }
        public string ProofDocumentUrl { get; set; }
        public string DocumentType { get; set; }
        public string Remakrs { get; set; }

    }
}
