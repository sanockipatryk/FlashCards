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

        public async Task<SetsPageViewModel> GetAllCardSetsAsync(int currentPage, int cardsPerPage, string? name, string? numberOfCards, string? author, string? sort)
        {
            var cardSetsCount = _context.CardSets.Where(c => c.IsPublic)
                    .ApplyFilters(name, numberOfCards, author)
                    .Count();


            var numberOfPages = PaginationHelpers.GetTotalPages(cardSetsCount, cardsPerPage);
            currentPage = PaginationHelpers.GetMinOrMaxPage(currentPage, numberOfPages);
            var cardSets = await _context.CardSets
                .Where(c => c.IsPublic)
                .ApplyFilters(name, numberOfCards, author)
                .ApplySort(sort)
                .ApplyPagination(currentPage, cardsPerPage, cardSetsCount)
                .Select(c => new CardSet
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    User = new ApplicationUser
                    {
                        Nickname = c.User.Nickname
                    },
                    CardSubject = new CardSubject
                    {
                        Name = c.CardSubject.Name,
                        CardCategory = new CardCategory
                        {
                            Name = c.CardSubject.CardCategory.Name
                        }
                    },
                })
                .ToListAsync();

            var categories = await _context.CardCategories.ToListAsync();
            return new SetsPageViewModel
            {
                CardSets = cardSets,
                CardCategories = categories,
                Pagination = new PaginationViewModel
                {
                    CurrentPage = currentPage,
                    NumberOfPages = numberOfPages
                },
                Filters = new CardSetFiltersViewModel
                {
                    Name = name,
                    NumberOfCards = numberOfCards,
                    Author = author,
                    SortBy = sort
                }
            };
        }

        public async Task<CategoryPageViewModel> GetPublicCardCategoryWithItsCardSetsAsync(string categoryName, int currentPage, int cardsPerPage, string? name, string? numberOfCards, string? author, string? sort)
        {
            var cardCategory = await _context.CardCategories.Include(c => c.CardSubjects)
            .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower());
            if (cardCategory != null)
            {
                var cardSubjectIds = cardCategory.CardSubjects.Select(cs => cs.Id).ToList();
                var cardSetsCount = _context.CardSets.Where(c => c.IsPublic)
                    .ApplyFilters(name, numberOfCards, author)
                    .Count(c => cardSubjectIds.Contains(c.CardSubjectId) && c.IsPublic);
                var numberOfPages = PaginationHelpers.GetTotalPages(cardSetsCount, cardsPerPage);
                currentPage = PaginationHelpers.GetMinOrMaxPage(currentPage, numberOfPages);
                var cardSets = await _context.CardSets.
                    Where(c => cardSubjectIds.Contains(c.CardSubjectId) && c.IsPublic)
                    .ApplyFilters(name, numberOfCards, author)
                    .ApplySort(sort)
                    .ApplyPagination(currentPage, cardsPerPage, cardSetsCount)
                    .Select(c => new CardSet
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        User = new ApplicationUser
                        {
                            Nickname = c.User.Nickname
                        },
                        CardSubject = new CardSubject
                        {
                            Name = c.CardSubject.Name,
                        },
                    })
                    .ToListAsync();

                var otherCategories = await _context.CardCategories.Where(c => c.Id != cardCategory.Id).ToListAsync();

                return new CategoryPageViewModel
                {
                    CardCategory = cardCategory,
                    OtherCategories = otherCategories,
                    CardSets = cardSets,
                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = currentPage,
                        NumberOfPages = numberOfPages
                    },
                    Filters = new CardSetFiltersViewModel
                    {
                        Name = name,
                        NumberOfCards = numberOfCards,
                        Author = author,
                        SortBy = sort
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

        public async Task<SubjectPageViewModel> GetCardSubjectWithItsCardSetsAsync(string categoryName, string subjectName, int currentPage, int cardsPerPage, string? name, string? numberOfCards, string? author, string? sort)
        {
            var cardSubject = await _context.CardSubjects
                .Include(c => c.CardCategory)
                .FirstOrDefaultAsync(c => c.Name.ToLower() == subjectName.ToLower() && c.CardCategory.Name.ToLower() == categoryName.ToLower());

            if (cardSubject != null)
            {
                var cardSetCount = _context.CardSets
                    .ApplyFilters(name, numberOfCards, author)
                    .Count(c => c.CardSubjectId == cardSubject.Id && c.IsPublic);

                var numberOfPages = PaginationHelpers.GetTotalPages(cardSetCount, cardsPerPage);
                currentPage = PaginationHelpers.GetMinOrMaxPage(currentPage, numberOfPages);

                cardSubject.CardSets = await _context.CardSets
                    .Where(c => c.CardSubjectId == cardSubject.Id && c.IsPublic)
                    .ApplyFilters(name, numberOfCards, author)
                    .ApplySort(sort)
                    .ApplyPagination(currentPage, cardsPerPage, cardSetCount)
                    .Select(c => new CardSet
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        User = new ApplicationUser
                        {
                            Nickname = c.User.Nickname
                        }
                    })
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
                    },
                    Filters = new CardSetFiltersViewModel
                    {
                        Name = name,
                        NumberOfCards = numberOfCards,
                        Author = author,
                        SortBy = sort
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

        //Fix later
        public async Task<SetsPageViewModel> GetAllOwnerCardSetsAsync(string userId, int currentPage, int cardsPerPage)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                var cardSetCount = _context.CardSets.Count(c => c.UserId == user.Id);
                if (cardSetCount > 0)
                {
                    var numberOfPages = PaginationHelpers.GetTotalPages(cardSetCount, cardsPerPage);
                    currentPage = PaginationHelpers.GetMinOrMaxPage(currentPage, numberOfPages);
                    var cardSets = await _context.CardSets
                        .Where(c => c.UserId == user.Id)
                        .ApplyPagination(currentPage, cardsPerPage, cardSetCount)
                        .Include(c => c.User)
                        .Include(c => c.CardSubject)
                        .Include(c => c.CardSubject.CardCategory)
                        .Select(c => new CardSet
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Description = c.Description,
                            User = new ApplicationUser
                            {
                                Nickname = c.User.Nickname
                            },
                            CardSubject = new CardSubject
                            {
                                Name = c.CardSubject.Name,
                                CardCategory = new CardCategory
                                {
                                    Name = c.CardSubject.CardCategory.Name
                                }
                            },
                        })
                        .ToListAsync();

                    var categories = await _context.CardCategories.ToListAsync();
                    return new SetsPageViewModel
                    {
                        CardSets = cardSets,
                        CardCategories = categories,
                        Pagination = new PaginationViewModel
                        {
                            CurrentPage = currentPage,
                            NumberOfPages = numberOfPages
                        }
                    };
                }
            }
            return new SetsPageViewModel();
        }

        //Fix later
        public async Task<IEnumerable<CardSet>> GetAllUserCardSetsAsync(string requestedUserNickName, int currentPage, int cardsPerPage)
        {

            var requestedUser = await _context.Users.FirstOrDefaultAsync(u => u.Nickname.ToLower() == requestedUserNickName.ToLower());
            if (requestedUser != null)
            {
                var cardSetCount = _context.CardSets.Count(c => c.UserId == requestedUser.Id && c.IsPublic);
                if (cardSetCount > 0)
                {
                    var numberOfPages = PaginationHelpers.GetTotalPages(cardSetCount, cardsPerPage);
                    currentPage = PaginationHelpers.GetMinOrMaxPage(currentPage, numberOfPages);
                    var cardSets = await _context.CardSets
                        .Where(c => c.UserId == requestedUser.Id && c.IsPublic)
                        .ApplyPagination(currentPage, cardsPerPage, cardSetCount)
                        .Include(c => c.CardSubject)
                        .Include(c => c.CardSubject.CardCategory)
                        .Select(c => new CardSet
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Description = c.Description,
                            CardSubject = new CardSubject
                            {
                                Name = c.CardSubject.Name,
                                CardCategory = new CardCategory
                                {
                                    Name = c.CardSubject.CardCategory.Name
                                }
                            },
                        })
                        .ToListAsync();
                    return cardSets;
                }

                return new List<CardSet>();
            }

            return new List<CardSet>();
        }

        public async Task<IEnumerable<QuestionWithAnswerViewModel>> GetCardSetPreviewAsync(int cardSetId, int count)
        {
            var cardSet = await _context.CardSets.Include(c => c.Cards).FirstOrDefaultAsync(c => c.Id == cardSetId && c.IsPublic);
            if (cardSet != null)
            {
                var questionsWithAnswers = new List<QuestionWithAnswerViewModel>();
                cardSet.Cards.Skip(count).Take(10).ToList()
                    .ForEach(c => questionsWithAnswers.Add(new QuestionWithAnswerViewModel { Question = c.Question, Answer = c.Answer }));

                return questionsWithAnswers;
            }
            return null;
        }

        public async Task CreateCardSetAsync(CreateCardSetViewModel model, string userId)
        {
            model.CardSet.DateCreated = DateTime.UtcNow;
            model.CardSet.DateUpdated = DateTime.UtcNow;
            foreach (var card in model.CardSet.Cards)
            {
                card.DateCreated = DateTime.UtcNow;
                card.DateUpdated = DateTime.UtcNow;
            }
            model.CardSet.UserId = userId;
            await _context.CardSets.AddAsync(model.CardSet);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserTheOwner(string userId, string requestedUserNickName)
        {
            if (userId != null && requestedUserNickName != null)
            {
                var loggedInUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (loggedInUser != null)
                {
                    if (loggedInUser.Nickname.ToLower() == requestedUserNickName.ToLower() || requestedUserNickName.ToLower() == "me")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            return false;
        }
    }
}
