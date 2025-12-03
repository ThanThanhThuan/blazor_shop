using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AccountingApp.Shared.Models
{
    public class JournalEntryLine
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        // --- ADD THIS ---
        [System.Text.Json.Serialization.JsonIgnore] // Prevents cycles during serialization
        public JournalEntry? JournalEntry { get; set; }
        // ----------------
        public int AccountId { get; set; }
        // Note: We avoid circular references in JSON by not including the 'Account' object here deeply or by using DTOs, 
        // but for this simple example, we will keep it simple.
        // --- ADD THIS PROPERTY ---
        [JsonIgnore] // Prevents "Cycle detected" errors
        public Account? Account { get; set; }
        // -------------------------
        [Column(TypeName = "decimal(18,2)")] public decimal Debit { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Credit { get; set; }
    }
}