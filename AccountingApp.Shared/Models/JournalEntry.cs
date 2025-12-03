namespace AccountingApp.Shared.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public List<JournalEntryLine> Lines { get; set; } = new();
        // Add this
        public List<JournalEntryAttachment> Attachments { get; set; } = new();
    }
}