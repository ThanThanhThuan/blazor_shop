using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AccountingApp.Shared.Models
{
    public class Account
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        // --- ADD THIS PROPERTY ---
        [JsonIgnore] // Prevents "Cycle detected" errors when sending Accounts to UI
        public List<JournalEntryLine> JournalEntryLines { get; set; } = new();
        // -------------------------
    }
}