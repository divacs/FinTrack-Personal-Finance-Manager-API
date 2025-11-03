using FinTrack.Application.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        // CRUD operations for Category entity

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        // Get category by id
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }
        // Add new category
        public async Task<Category> AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }
        // Update existing category
        public async Task<Category?> UpdateAsync(Category category)
        {
            var existing = await _context.Categories.FindAsync(category.Id);
            if (existing == null) return null;

            existing.Name = category.Name;
            existing.ColorHex = category.ColorHex;

            await _context.SaveChangesAsync();
            return existing;
        }
        // Delete category by id
        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
