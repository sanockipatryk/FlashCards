using FlashCards.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashCards.Data.Services
{
	public class CardsService : ICardsService
	{
		private readonly ApplicationDbContext _context;
		public CardsService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<CardCategory>> GetAllCardCategoriesAsync()
		{
			return await _context.CardCategories.ToListAsync();
		}

		public async Task<IEnumerable<CardCategory>> GetAllCardCategoriesWithSubjectsAsync()
		{
			return await _context.CardCategories.Include(c => c.CardSubjects).ToListAsync();
		}

		public async Task<IEnumerable<CardSubject>> GetAllCardSubjectsAsync()
		{
			return await _context.CardSubjects.ToListAsync();
		}

		public async Task<IEnumerable<CardSubject>> GetCardSubjectsForCategoryAsync(int categoryId)
		{
			return await _context.CardSubjects.Where(c => c.CardCategoryId == categoryId).ToListAsync();
		}

		public async Task<IEnumerable<CardSet>> GetAllPublicCardSetsAsync()
		{
			return await _context.CardSets.Where(c => c.IsPublic).ToListAsync();
		}

		public async Task<IEnumerable<CardSet>> GetAllPublicCardSetsForSubjectAsync(int subjectId)
		{
			return await _context.CardSets.Where(c => c.IsPublic && c.CardSubjectId == subjectId).ToListAsync();
		}

		public async Task<IEnumerable<CardSet>> GetAllUserCardSetsAsync(string userId)
		{
			return await _context.CardSets.Where(c => c.UserId == userId).ToListAsync();
		}
	}
}
