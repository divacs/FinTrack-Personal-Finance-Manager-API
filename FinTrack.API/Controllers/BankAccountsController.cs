using FinTrack.API.DTOs.BankAccount;
using FinTrack.Application.Interfaces;
using FinTrack.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // svi endpointi zahtevaju prijavu
    public class BankAccountsController : ControllerBase
    {
        private readonly IBankAccountRepository _repository;

        public BankAccountsController(IBankAccountRepository repository)
        {
            _repository = repository;
        }

        // methods to get user id and check roles
        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        private bool IsAdmin() => User.IsInRole("Admin");
        private bool IsManager() => User.IsInRole("Manager");

        // GET /api/bankaccounts
        // All roles can access their own accounts, Admin and Manager can access all accounts
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            bool allUsers = IsAdmin() || IsManager();
            var accounts = await _repository.GetAllAsync(userId, allUsers);

            var result = accounts.Select(a => new GetBankAccountDto
            {
                Id = a.Id,
                BankName = a.BankName,
                AccountNumber = a.AccountNumber,
                Balance = a.Balance
            });

            return Ok(result);
        }

        // GET /api/bankaccounts/{id}
        // All roles can access their own accounts, Admin and Manager can access any account
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = GetUserId();
            bool allUsers = IsAdmin() || IsManager();
            var account = await _repository.GetByIdAsync(id, userId, allUsers);

            if (account == null) return NotFound();

            var result = new GetBankAccountDto
            {
                Id = account.Id,
                BankName = account.BankName,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance
            };

            return Ok(result);
        }

        // POST /api/bankaccounts
        // All roles can create accounts for themselves
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> Create([FromBody] CreateBankAccountDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var entity = new BankAccount
            {
                BankName = dto.BankName,
                AccountNumber = dto.AccountNumber,
                Balance = dto.Balance,
                UserId = userId
            };

            var created = await _repository.AddAsync(entity);

            var result = new GetBankAccountDto
            {
                Id = created.Id,
                BankName = created.BankName,
                AccountNumber = created.AccountNumber,
                Balance = created.Balance
            };

            return CreatedAtAction(nameof(Get), new { id = created.Id }, result);
        }

        // PUT /api/bankaccounts/{id}
        // All roles can update their own accounts, Admin and Manager can update any account
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBankAccountDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var entity = new BankAccount
            {
                Id = id,
                UserId = userId,
                BankName = dto.BankName,
                AccountNumber = dto.AccountNumber,
                Balance = dto.Balance
            };

            // Admin and Manager can update any account
            if (IsAdmin() || IsManager()) entity.UserId = null!;

            var updated = await _repository.UpdateAsync(entity);
            if (updated == null) return NotFound();

            var result = new GetBankAccountDto
            {
                Id = updated.Id,
                BankName = updated.BankName,
                AccountNumber = updated.AccountNumber,
                Balance = updated.Balance
            };

            return Ok(result);
        }

        // DELETE /api/bankaccounts/{id}
        // All roles can delete their own accounts, Admin and Manager can delete any account
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            bool allUsers = IsAdmin() || IsManager();

            var deleted = await _repository.DeleteAsync(id, userId, allUsers);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
