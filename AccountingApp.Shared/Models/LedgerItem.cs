namespace AccountingApp.Shared.Models
{
    public class LedgerItem
    {
        public int JournalEntryId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        // --- ADD THIS ---
        public string AccountName { get; set; } = string.Empty;
        // ----------------
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        // Add this simple list just for display purposes
        public List<AttachmentInfo> Attachments { get; set; } = new();
    }

    public class AttachmentInfo
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}