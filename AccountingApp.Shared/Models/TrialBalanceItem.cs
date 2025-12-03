namespace AccountingApp.Shared.Models
{
    public class TrialBalanceItem
    {
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;

        // In a Trial Balance, an account appears in ONLY one column (Debit OR Credit)
        public decimal DebitBalance { get; set; }
        public decimal CreditBalance { get; set; }
    }
}