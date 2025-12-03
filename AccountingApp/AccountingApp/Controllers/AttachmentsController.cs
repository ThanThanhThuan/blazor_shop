using AccountingApp.Data;
using AccountingApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : ControllerBase
    {
        private readonly AccountingContext _context;

        public AttachmentsController(AccountingContext context)
        {
            _context = context;
        }

        // GET: api/Attachments/5 (Download/View)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttachment(int id)
        {
            var file = await _context.Attachments.FindAsync(id);
            if (file == null) return NotFound();

            // Returns the file stream so the browser can render/download it
            return File(file.Data, file.ContentType, file.FileName);
        }

        // POST: api/Attachments/upload/10 (Upload for Entry ID 10)
        [HttpPost("upload/{entryId}")]
        public async Task<IActionResult> Upload(int entryId, List<IFormFile> files)
        {
            // Verify Entry exists
            var entry = await _context.JournalEntries.FindAsync(entryId);
            if (entry == null) return NotFound("Journal Entry not found");

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await formFile.CopyToAsync(memoryStream);

                    var attachment = new JournalEntryAttachment
                    {
                        JournalEntryId = entryId,
                        FileName = formFile.FileName,
                        ContentType = formFile.ContentType,
                        Data = memoryStream.ToArray()
                    };

                    _context.Attachments.Add(attachment);
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}