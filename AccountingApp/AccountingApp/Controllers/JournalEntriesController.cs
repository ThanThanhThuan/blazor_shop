using AccountingApp.Data;
using AccountingApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JournalEntriesController : ControllerBase
    {
        private readonly AccountingContext _context;

        public JournalEntriesController(AccountingContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<JournalEntry>> Post(JournalEntry entry)
        {
            // Double Entry Validation
            if (entry.Lines.Sum(x => x.Debit) != entry.Lines.Sum(x => x.Credit))
            {
                return BadRequest("Debits must equal Credits.");
            }

            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync();
            return Ok(entry);
        }

        [HttpGet("accounts")]
        public async Task<ActionResult<List<Account>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }

        // Add this method inside the controller
        [HttpGet("ledger/{accountId}")]
        public async Task<ActionResult<List<LedgerItem>>> GetLedger(int accountId)
        {
            var items = await _context.JournalEntryLines
                .Include(l => l.JournalEntry)
                .ThenInclude(je => je.Attachments) // <--- INCLUDE ATTACHMENTS
                .Where(l => l.AccountId == accountId)
                .OrderBy(l => l.JournalEntry.Date)
                .Select(l => new LedgerItem
                {
                    JournalEntryId = l.JournalEntryId,
                    Date = l.JournalEntry.Date,
                    Description = l.JournalEntry.Description,
                    Debit = l.Debit,
                    Credit = l.Credit,
                    // Map attachments
                    Attachments = l.JournalEntry.Attachments.Select(a => new AttachmentInfo
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        ContentType = a.ContentType
                    }).ToList()
                })
                .ToListAsync();

            return items;
        }

        // GET: api/JournalEntries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JournalEntry>> GetJournalEntry(int id)
        {
            var journalEntry = await _context.JournalEntries
                .Include(j => j.Lines)
                .Include(j => j.Attachments) // Include files so we can list them
                .FirstOrDefaultAsync(j => j.Id == id);

            if (journalEntry == null) return NotFound();

            return journalEntry;
        }

        // DELETE: api/JournalEntries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJournalEntry(int id)
        {
            var journalEntry = await _context.JournalEntries.FindAsync(id);
            if (journalEntry == null) return NotFound();

            // EF Core Cascade Delete will automatically delete the Lines and Attachments 
            // because of the required Foreign Key relationship.
            _context.JournalEntries.Remove(journalEntry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/JournalEntries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJournalEntry(int id, JournalEntry journalEntry)
        {
            if (id != journalEntry.Id) return BadRequest();

            // 1. Validate Double Entry Rule
            if (journalEntry.Lines.Sum(l => l.Debit) != journalEntry.Lines.Sum(l => l.Credit))
            {
                return BadRequest("Transaction is unbalanced.");
            }

            // 2. Load the existing entry from DB (including lines) to track changes
            var existingEntry = await _context.JournalEntries
                .Include(j => j.Lines)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (existingEntry == null) return NotFound();

            // 3. Update Header Info
            existingEntry.Date = journalEntry.Date;
            existingEntry.Description = journalEntry.Description;

            // 4. Update Lines (The tricky part)
            // 4a. Identify lines to delete (present in DB, but not in incoming Request)
            // We compare IDs.
            var requestLineIds = journalEntry.Lines.Select(l => l.Id).ToList();
            var linesToDelete = existingEntry.Lines.Where(l => !requestLineIds.Contains(l.Id)).ToList();

            foreach (var line in linesToDelete)
            {
                _context.JournalEntryLines.Remove(line);
            }

            // 4b. Identify lines to Add or Update
            foreach (var line in journalEntry.Lines)
            {
                var existingLine = existingEntry.Lines.FirstOrDefault(l => l.Id == line.Id);

                if (existingLine != null)
                {
                    // Update existing
                    existingLine.AccountId = line.AccountId;
                    existingLine.Debit = line.Debit;
                    existingLine.Credit = line.Credit;
                }
                else
                {
                    // Insert new (ensure ID is 0 so EF knows it's new)
                    line.Id = 0;
                    existingEntry.Lines.Add(line);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.JournalEntries.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }
        [HttpGet("general")]
        public async Task<ActionResult<List<LedgerItem>>> GetGeneralJournal([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var items = await _context.JournalEntryLines
                .Include(l => l.JournalEntry)
                .Include(l => l.Account) // We need the Account Name now
                .Where(l => l.JournalEntry.Date >= startDate && l.JournalEntry.Date <= endDate)
                .OrderBy(l => l.JournalEntry.Date)     // Order by Date first
                .ThenBy(l => l.JournalEntry.Id)        // Then by Entry ID (keeps lines together)
                .Select(l => new LedgerItem
                {
                    JournalEntryId = l.JournalEntryId,
                    Date = l.JournalEntry.Date,
                    Description = l.JournalEntry.Description,
                    AccountName = l.Account.Name, // Map the new property
                    Debit = l.Debit,
                    Credit = l.Credit
                    // We can omit attachments here to keep the query light, 
                    // or include them if you want to see them in this view too.
                })
                .ToListAsync();

            return items;
        }
    }

}