using FlashCards.Helpers;
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

        public async Task<CategoryPageViewModel> GetCardCategoryWithItsCardSets(string categoryName, int currentPage, int cardsPerPage)
        {
            var cardCategory = await _context.CardCategories.Include(c => c.CardSubjects)
            .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower());
            if (cardCategory != null)
            {
                var cardSubjectIds = cardCategory.CardSubjects.Select(cs => cs.Id).ToList();
                var cardSetsCount = _context.CardSets.Count(c => cardSubjectIds.Contains(c.CardSubjectId) && c.IsPublic);
                var numberOfPages = PaginationHelpers.GetTotalPages(cardSetsCount, cardsPerPage);
                currentPage = PaginationHelpers.GetMinOrMaxPage(currentPage, numberOfPages);
                var cardSets = await _context.CardSets.
                    Where(c => cardSubjectIds.Contains(c.CardSubjectId) && c.IsPublic)
                    .Skip((currentPage - 1) * cardsPerPage)
                    .Take(cardsPerPage)
                    .ToListAsync();
                
                return new CategoryPageViewModel
                {
                    CardCategory = cardCategory,
                    CardSets = cardSets,
                    Pagination = new PaginationViewModel
					{
                        CurrentPage = currentPage,
                        NumberOfPages = numberOfPages
                    }
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

        public async Task<SubjectPageViewModel> GetCardSubjectWithItsCardSets(string categoryName, string subjectName, int currentPage, int cardsPerPage)
        {
            var cardSubject = await _context.CardSubjects
                .Include(c => c.CardCategory)
                .FirstOrDefaultAsync(c => c.Name.ToLower() == subjectName.ToLower() && c.CardCategory.Name.ToLower() == categoryName.ToLower());

            if (cardSubject != null)
            {
                var cardSetCount = _context.CardSets.Count(c => c.CardSubjectId == cardSubject.Id && c.IsPublic);
                var numberOfPages = PaginationHelpers.GetTotalPages(cardSetCount, cardsPerPage);
                currentPage = PaginationHelpers.GetMinOrMaxPage(currentPage, numberOfPages);

                cardSubject.CardSets = await _context.CardSets
                    .Where(cs => cs.CardSubjectId == cardSubject.Id && cs.IsPublic)
                    .Skip((currentPage - 1) * cardsPerPage)
                    .Take(cardsPerPage)
                    .ToListAsync();

                var otherSubjects = await _context.CardSubjects
                    .Where(c => c.Id != cardSubject.Id && c.CardCategoryId == cardSubject.CardCategoryId)
                    .ToListAsync();

                return new SubjectPageViewModel
                {
                    CardSubject = cardSubject,
                    OtherSubjects = otherSubjects,
                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = currentPage,
                        NumberOfPages = numberOfPages
                    }
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

        public async Task CreateCardSetAsync(CreateCardSetViewModel model, string userId)
        {
            model.CardSet.DateCreated = DateTime.UtcNow;
            model.CardSet.DateUpdated = DateTime.UtcNow;
            foreach(var card in model.CardSet.Cards)
            {
                card.DateCreated = DateTime.UtcNow;
                card.DateUpdated = DateTime.UtcNow;
            }
            model.CardSet.UserId = userId;
            await _context.CardSets.AddAsync(model.CardSet);
            await _context.SaveChangesAsync();
        }
    }
}
