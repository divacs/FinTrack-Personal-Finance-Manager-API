using FinTrack.API.DTOs.Budget;
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
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetRepository _repository;

        public BudgetsController(IBudgetRepository repository)
        {
            _repository = repository;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

        [HttpGet]
        // Gets all budgets for the authenticated user
        public async Task<IActionResult> GetUserBudgets()
        {
            var userId = GetUserId();
            var budgets = await _repository.GetUserBudgetsAsync(userId);

            var result = budgets.Select(b => new GetBudgetDto
            {
                Id = b.Id,
                CategoryId = b.CategoryId,
                CategoryName = b.Category.Name,
                LimitAmount = b.LimitAmount,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                CurrentSpending = b.CurrentSpending
            });

            return Ok(result);
        }

        [HttpPost]
        // Creates a new budget
        public async Task<IActionResult> Create([FromBody] CreateBudgetDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var budget = new Budget
            {
                UserId = GetUserId(),
                CategoryId = dto.CategoryId,
                LimitAmount = dto.LimitAmount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            var created = await _repository.AddAsync(budget);

            return CreatedAtAction(nameof(GetUserBudgets), new { id = created.Id }, dto);
        }

        [HttpPut("{id}")]
        // Updates an existing budget by its ID
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBudgetDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var budget = new Budget
            {
                Id = id,
                UserId = GetUserId(),
                CategoryId = dto.CategoryId,
                LimitAmount = dto.LimitAmount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            var updated = await _repository.UpdateAsync(budget);
            if (updated == null) return NotFound();

            return Ok(dto);
        }

        [HttpDelete("{id}")]
        // Deletes a budget by its ID
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id, GetUserId());
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
