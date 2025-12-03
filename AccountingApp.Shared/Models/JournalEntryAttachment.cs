namespace AccountingApp.Shared.Models
{
    public class JournalEntryAttachment
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty; // e.g., "image/png", "application/pdf"

        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}