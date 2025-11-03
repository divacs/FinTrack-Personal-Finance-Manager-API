using FinTrack.API.DTOs.Transaction;
using FinTrack.Application.Interfaces;
using FinTrack.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _repository;

        public TransactionsController(ITransactionRepository repository)
        {
            _repository = repository;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        private bool IsAdmin() => User.IsInRole("Admin");
        private bool IsManager() => User.IsInRole("Manager");

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,User")]
        // All roles can access their own transactions, Admin and Manager can access all transactions
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            bool allUsers = IsAdmin() || IsManager();
            var transactions = await _repository.GetAllAsync(userId, allUsers);

            var result = transactions.Select(t => new GetTransactionDto
            {
                Id = t.Id,
                BankAccountId = t.BankAccountId,
                Amount = t.Amount,
                Currency = t.Currency,
                Date = t.Date,
                Description = t.Description,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name,
                Type = t.Type
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,User")]
        // All roles can access their own transactions, Admin and Manager can access any transaction
        public async Task<IActionResult> Get(int id)
        {
            var userId = GetUserId();
            bool allUsers = IsAdmin() || IsManager();
            var transaction = await _repository.GetByIdAsync(id, userId, allUsers);

            if (transaction == null) return NotFound();

            var result = new GetTransactionDto
            {
                Id = transaction.Id,
                BankAccountId = transaction.BankAccountId,
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                Date = transaction.Date,
                Description = transaction.Description,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category?.Name,
                Type = transaction.Type
            };

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,User")]
        // All roles can create transactions for themselves
        public async Task<IActionResult> Create([FromBody] CreateTransactionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new Transaction
            {
                BankAccountId = dto.BankAccountId,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Date = dto.Date,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Type = dto.Type
            };

            var created = await _repository.AddAsync(entity);

            var result = new GetTransactionDto
            {
                Id = created.Id,
                BankAccountId = created.BankAccountId,
                Amount = created.Amount,
                Currency = created.Currency,
                Date = created.Date,
                Description = created.Description,
                CategoryId = created.CategoryId,
                CategoryName = created.Category?.Name,
                Type = created.Type
            };

            return CreatedAtAction(nameof(Get), new { id = created.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,User")]
        // All roles can update their own transactions, Admin and Manager can update any transaction
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTransactionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new Transaction
            {
                Id = id,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Date = dto.Date,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Type = dto.Type
            };

            var updated = await _repository.UpdateAsync(entity);
            if (updated == null) return NotFound();

            var result = new GetTransactionDto
            {
                Id = updated.Id,
                BankAccountId = updated.BankAccountId,
                Amount = updated.Amount,
                Currency = updated.Currency,
                Date = updated.Date,
                Description = updated.Description,
                CategoryId = updated.CategoryId,
                CategoryName = updated.Category?.Name,
                Type = updated.Type
            };

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager,User")]
        // All roles can delete their own transactions, Admin and Manager can delete any transaction
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            bool allUsers = IsAdmin() || IsManager();

            var deleted = await _repository.DeleteAsync(id, userId, allUsers);
            if (!deleted) return NotFound();

            return NoContent();
        }

        [HttpPost("report")]
        [Authorize(Roles = "Admin,Manager,User")]
        // All roles can generate reports for their own transactions, Admin and Manager can generate reports for all transactions
        public async Task<IActionResult> Report([FromBody] TransactionReportDto dto)
        {
            var userId = GetUserId();
            bool allUsers = IsAdmin() || IsManager();

            var transactions = await _repository.GetReportAsync(userId, allUsers, dto.FromDate, dto.ToDate, dto.CategoryId, dto.BankAccountId);

            var result = transactions.Select(t => new GetTransactionDto
            {
                Id = t.Id,
                BankAccountId = t.BankAccountId,
                Amount = t.Amount,
                Currency = t.Currency,
                Date = t.Date,
                Description = t.Description,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name,
                Type = t.Type
            });

            return Ok(result);
        }
    }
}
