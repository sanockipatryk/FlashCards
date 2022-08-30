using FlashCards.Models;
using FlashCards.Models.ViewModels;
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

        public async Task<CategoryPageViewModel> GetCardCategoryWithItsCardSets(string categoryName)
        {
            var cardCategory = await _context.CardCategories.Include(c => c.CardSubjects)
            .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower());
            if (cardCategory != null)
            {
                var cardSets = await _context.CardSets.Include(cs => cs.CardSubject)
                    .Where(cs => cs.CardSubject.CardCategoryId == cardCategory.Id && (cs.IsPublic || cs.UserId == "test"))
                    .ToListAsync();

                return new CategoryPageViewModel
                {
                    CardCategory = cardCategory,
                    CardSets = cardSets
                };
            }

            return new CategoryPageViewModel();
        }

        public async Task<IEnumerable<CardSubject>> GetAllCardSubjectsAsync()
        {
            return await _context.CardSubjects.ToListAsync();
        }

        public async Task<IEnumerable<CardSubject>> GetCardSubjectsForCategoryAsync(int categoryId)
        {
            return await _context.CardSubjects.Where(c => c.CardCategoryId == categoryId).ToListAsync();
        }

        public async Task<SubjectPageViewModel> GetCardSubjectWithItsCardSets(string categoryName, string subjectName)
        {
            var cardSubject = await _context.CardSubjects.Include(c => c.CardCategory)
            .FirstOrDefaultAsync(c => c.Name.ToLower() == subjectName.ToLower() && c.CardCategory.Name.ToLower() == categoryName.ToLower());
            if (cardSubject != null)
            {
                cardSubject.CardSets = await _context.CardSets.Where(cs => cs.CardSubjectId == cardSubject.Id && cs.IsPublic).ToListAsync();
                var otherSubjects = await _context.CardSubjects.Where(c => c.Id != cardSubject.Id && c.CardCategoryId == cardSubject.CardCategoryId).ToListAsync();

                return new SubjectPageViewModel
                {
                    CardSubject = cardSubject,
                    OtherSubjects = otherSubjects
                };
            }

            return new SubjectPageViewModel();
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
