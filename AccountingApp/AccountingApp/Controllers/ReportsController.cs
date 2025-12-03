using AccountingApp.Data;
using AccountingApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly AccountingContext _context;

        public ReportsController(AccountingContext context)
        {
            _context = context;
        }

        [HttpGet("trial-balance")]
        public async Task<ActionResult<List<TrialBalanceItem>>> GetTrialBalance()
        {
            // 1. Get all accounts and calculate raw totals directly in SQL
            var rawData = await _context.Accounts
                .Select(a => new
                {
                    Name = a.Name,
                    Type = a.Type,
                    // Sum all debits and credits for this account
                    TotalDebit = a.JournalEntryLines.Sum(l => l.Debit),
                    TotalCredit = a.JournalEntryLines.Sum(l => l.Credit)
                })
                .ToListAsync();

            // 2. Process into Trial Balance format (Netting)
            var report = new List<TrialBalanceItem>();

            foreach (var item in rawData)
            {
                var net = item.TotalDebit - item.TotalCredit;

                // Skip accounts with zero balance
                if (net == 0) continue;

                var line = new TrialBalanceItem
                {
                    AccountName = item.Name,
                    AccountType = item.Type
                };

                if (net > 0)
                {
                    // Net Debit
                    line.DebitBalance = net;
                }
                else
                {
                    // Net Credit (store as positive number for display)
                    line.CreditBalance = Math.Abs(net);
                }

                report.Add(line);
            }

            return report;
        }
    }
}