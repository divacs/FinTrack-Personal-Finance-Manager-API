using FinTrack.API.DTOs.Category;
using FinTrack.Application.Interfaces;
using FinTrack.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repository;

        public CategoriesController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        // GET /api/categories
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,User")]
        // Get all categories - accessible to all roles
        public async Task<IActionResult> GetAll()
        {
            var categories = await _repository.GetAllAsync();
            var result = categories.Select(c => new GetCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ColorHex = c.ColorHex
            });

            return Ok(result);
        }

        // POST /api/categories
        [HttpPost]
        [Authorize(Roles = "Admin")]
        // Create a new category - Admin only
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var category = new Category
            {
                Name = dto.Name,
                ColorHex = dto.ColorHex
            };

            var created = await _repository.AddAsync(category);

            var result = new GetCategoryDto
            {
                Id = created.Id,
                Name = created.Name,
                ColorHex = created.ColorHex
            };

            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, result);
        }

        // PUT /api/categories/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        // Update an existing category - Admin only
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var category = new Category
            {
                Id = id,
                Name = dto.Name,
                ColorHex = dto.ColorHex
            };

            var updated = await _repository.UpdateAsync(category);
            if (updated == null) return NotFound();

            var result = new GetCategoryDto
            {
                Id = updated.Id,
                Name = updated.Name,
                ColorHex = updated.ColorHex
            };

            return Ok(result);
        }

        // DELETE /api/categories/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        // Delete a category - Admin only
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
