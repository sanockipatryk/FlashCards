using FlashCards.Helpers;
using FlashCards.Models;
using FlashCards.Models.ViewModels;
using FlashCards.Models.Quiz;
using FlashCards.Models.Types.Enums;
using FlashCards.SSoT;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace FlashCards.Data.Services
{
    public class CardsService : ICardsService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CardsService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            return await _context.CardSubjects.Select(c => new CardSubject
            {
                Id = c.Id,
                Name = c.Name,
                CardCategoryId = c.CardCategoryId
            }).ToListAsync();
        }

        public async Task<string> GetAllCardSubjectsJsonAsync()
        {
            var cardSubjects = await GetAllCardSubjectsAsync();
            return JsonSerializer.Serialize(cardSubjects);
        }

        public async Task<IEnumerable<CardSubject>> GetCardSubjectsForCategoryAsync(int categoryId)
        {
            return await _context.CardSubjects.Where(c => c.CardCategoryId == categoryId).ToListAsync();
        }

        public async Task<SetsPageViewModel> GetAllCardSetsAsync(
            CardSetFiltersViewModel cardSetFilters,
            int currentPage,
            int setsPerPage,
            string? userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var isUserAdmin = false;
            if (user != null)
                isUserAdmin = await _userManager.IsInRoleAsync(user, DefaultAppValues.AdminRole);

            var cardSetsCount = await _context.CardSets
                .PublicOrUserIsOwner(userId, isUserAdmin)
                .NotDeleted()
                .ApplyFilters(cardSetFilters)
                .CountAsync();

            var numberOfPages = PaginationHelpers.GetTotalPages(cardSetsCount, setsPerPage);
            currentPage = PaginationHelpers.GetMinOrMaxPageIfOverBounds(currentPage, numberOfPages);

            var cardSets = await _context.CardSets
                .PublicOrUserIsOwner(userId, isUserAdmin)
                .NotDeleted()
                .ApplyFilters(cardSetFilters)
                .ApplySort(cardSetFilters.SortBy)
                .ApplyPagination(currentPage, setsPerPage, cardSetsCount)
                .SelectDefaultCardSetDataForView()
                .ToListAsync();

            var categories = await _context.CardCategories.ToListAsync();
            return new SetsPageViewModel
            {
                CardSetListData = new CardSetListViewModel
                {
                    CardSets = cardSets,
                },
                CardCategories = categories,
                Pagination = new PaginationViewModel
                {
                    CurrentPage = currentPage,
                    NumberOfPages = numberOfPages
                },
                Filters = cardSetFilters
            };
        }

        public async Task<CategoryPageViewModel> GetAllCardSetsOfCategoryAsync(
            string categoryName,
            CardSetFiltersViewModel cardSetFilters,
            int currentPage,
            int setsPerPage,
            string? userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var isUserAdmin = false;
            if (user != null)
                isUserAdmin = await _userManager.IsInRoleAsync(user, DefaultAppValues.AdminRole);

            var cardCategory = await _context.CardCategories.Include(c => c.CardSubjects)
            .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower());
            if (cardCategory != null)
            {
                var cardSubjectIds = cardCategory.CardSubjects.Select(cs => cs.Id).ToList();
                var cardSetsCount = await _context.CardSets
                    .Where(c => cardSubjectIds.Contains(c.CardSubjectId))
                    .PublicOrUserIsOwner(userId, isUserAdmin)
                    .NotDeleted()
                    .ApplyFilters(cardSetFilters)
                    .CountAsync();

                var numberOfPages = PaginationHelpers.GetTotalPages(cardSetsCount, setsPerPage);
                currentPage = PaginationHelpers.GetMinOrMaxPageIfOverBounds(currentPage, numberOfPages);
                var cardSets = await _context.CardSets
                    .Where(c => cardSubjectIds.Contains(c.CardSubjectId))
                    .PublicOrUserIsOwner(userId, isUserAdmin)
                    .NotDeleted()
                    .ApplyFilters(cardSetFilters)
                    .ApplySort(cardSetFilters.SortBy)
                    .ApplyPagination(currentPage, setsPerPage, cardSetsCount)
                    .Select(c => new CardSet
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        IsPublic = c.IsPublic,
                        UserId = c.UserId,
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
                    CardSetListData = new CardSetListViewModel
                    {
                        CardSets = cardSets,
                        CategoryName = cardCategory.Name,
                    },
                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = currentPage,
                        NumberOfPages = numberOfPages
                    },
                    Filters = cardSetFilters
                };
            }
            return new CategoryPageViewModel();
        }

        public async Task<SubjectPageViewModel> GetAllCardSetsOfSubjectAsync(
            string categoryName,
            string subjectName,
            CardSetFiltersViewModel cardSetFilters,
            int currentPage,
            int setsPerPage,
            string? userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var isUserAdmin = false;
            if (user != null)
                isUserAdmin = await _userManager.IsInRoleAsync(user, DefaultAppValues.AdminRole);

            var cardSubject = await _context.CardSubjects
                .Include(c => c.CardCategory)
                .FirstOrDefaultAsync(
                    c => c.Name.ToLower() == subjectName.ToLower() &&
                    c.CardCategory.Name.ToLower() == categoryName.ToLower()
                    );

            if (cardSubject != null)
            {
                var cardSetCount = await _context.CardSets
                    .ApplyFilters(cardSetFilters)
                    .PublicOrUserIsOwner(userId, isUserAdmin)
                    .NotDeleted()
                    .CountAsync(c => c.CardSubjectId == cardSubject.Id);

                var numberOfPages = PaginationHelpers.GetTotalPages(cardSetCount, setsPerPage);
                currentPage = PaginationHelpers.GetMinOrMaxPageIfOverBounds(currentPage, numberOfPages);

                var cardSets = await _context.CardSets
                    .Where(c => c.CardSubjectId == cardSubject.Id)
                    .PublicOrUserIsOwner(userId, isUserAdmin)
                    .NotDeleted()
                    .ApplyFilters(cardSetFilters)
                    .ApplySort(cardSetFilters.SortBy)
                    .ApplyPagination(currentPage, setsPerPage, cardSetCount)
                    .Select(c => new CardSet
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        IsPublic = c.IsPublic,
                        UserId = c.UserId,
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
                    CardSetListData = new CardSetListViewModel
                    {
                        CardSets = cardSets,
                        CategoryName = cardSubject.CardCategory.Name,
                    },
                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = currentPage,
                        NumberOfPages = numberOfPages
                    },
                    Filters = cardSetFilters
                };
            }
            return new SubjectPageViewModel();
        }


        public async Task<IEnumerable<QuestionWithAnswerViewModel>> GetCardSetPreviewAsync(int cardSetId, int count, string? userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var isUserAdmin = false;
            if (user != null)
                isUserAdmin = await _userManager.IsInRoleAsync(user, DefaultAppValues.AdminRole);

            var cardSet = await _context.CardSets
                .Where(c => c.Id == cardSetId)
                .PublicOrUserIsOwner(userId, isUserAdmin)
                .NotDeleted()
                .FirstOrDefaultAsync();

            if (cardSet != null)
            {
                var questionsWithAnswers = await _context.Cards
                .Where(c => c.CardSetId == cardSet.Id)
                .Skip(count)
                .Take(DefaultAppValues.PreviewCardCount)
                .Select(c => new QuestionWithAnswerViewModel
                {
                    Question = c.Question,
                    Answer = c.Answer
                })
                .ToListAsync();

                return questionsWithAnswers;
            }
            return null;
        }

        public async Task<CardSetViewModel> GetCardSetAsync(int id, string? userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var isUserAdmin = false;
            if (user != null)
                isUserAdmin = await _userManager.IsInRoleAsync(user, DefaultAppValues.AdminRole);

            var cardSet = await _context.CardSets
                .Where(c => c.Id == id)
                .PublicOrUserIsOwner(userId, isUserAdmin)
                .NotDeleted()
                .Select(c => new CardSet
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    DateUpdated = c.DateUpdated,
                    IsPublic = c.IsPublic,
                    UserId = c.UserId,
                    User = new ApplicationUser
                    {
                        Id = c.User.Id,
                        Nickname = c.User.Nickname,
                    },
                    CardSubject = new CardSubject
                    {
                        Id = c.CardSubject.Id,
                        Name = c.CardSubject.Name,
                        CardCategory = new CardCategory
                        {
                            Id = c.CardSubject.CardCategory.Id,
                            Name = c.CardSubject.CardCategory.Name,
                        }
                    },
                })
                .FirstOrDefaultAsync();

            if (cardSet != null)
            {
                await AddCardSetAcccessAsync(id, userId);
                var cardCount = await _context.Cards.CountAsync(c => c.CardSetId == cardSet.Id);
                return new CardSetViewModel
                {
                    Id = cardSet.Id,
                    Name = cardSet.Name,
                    Description = cardSet.Description,
                    DateUpdated = cardSet.DateUpdated,
                    IsPublic = cardSet.IsPublic,
                    User = cardSet.User,
                    CardSubject = cardSet.CardSubject,
                    CardCount = cardCount
                };
            }
            return new CardSetViewModel();
        }

        public async Task<IEnumerable<Card>> GetCardsForCardSetAsync(int id, string? userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var isUserAdmin = false;
            if (user != null)
                isUserAdmin = await _userManager.IsInRoleAsync(user, DefaultAppValues.AdminRole);

            var cardSet = await _context.CardSets
                .Where(c => c.Id == id)
                .PublicOrUserIsOwner(userId, isUserAdmin)
                .NotDeleted()
                .FirstOrDefaultAsync();
            if (cardSet != null)
            {
                var cards = await _context.Cards.Where(c => c.CardSetId == cardSet.Id)
                    .Select(c => new Card
                    {
                        Id = c.Id,
                        Question = c.Question,
                        Answer = c.Answer,
                    }).ToListAsync();

                return cards;
            }
            return null;
        }


        public async Task<int> CreateCardSetAsync(CreateCardSetViewModel model, string userId)
        {
            model.CardSet.DateCreated = DateTime.UtcNow;
            model.CardSet.DateUpdated = DateTime.UtcNow;
            model.CardSet.Id = 0; // setting id to 0, to remove Id from model when copying a cardset
            model.CardSet.UserId = userId;
            model.CardSet.IsDeleted = false;
            var cardSet = await _context.CardSets.AddAsync(model.CardSet);
            await _context.SaveChangesAsync();
            return cardSet.Entity.Id;
        }

        public async Task<CreateCardSetViewModel> GetCardSetForEditAsync(int id)
        {
            var cardSet = await _context.CardSets
            .SelectForEditOrCopy()
            .FirstOrDefaultAsync(c => c.Id == id);
            if (cardSet != null)
            {
                var cards = await _context.Cards.Where(c => c.CardSetId == cardSet.Id).ToListAsync();
                cardSet.Cards = cards;
                return new CreateCardSetViewModel
                {
                    CardSet = cardSet,
                    CardCategories = await GetAllCardCategoriesAsync(),
                    CardSubjectsListJson = await GetAllCardSubjectsJsonAsync(),
                    SelectedCardCategoryId = cardSet.CardSubject.CardCategoryId,
                    AddManyCards = DefaultAppValues.AddManyCards,
                    ActionType = CreateSetActionType.Edit
                };
            }
            return new CreateCardSetViewModel();
        }

        public async Task<int> EditCardSetAsync(CreateCardSetViewModel model)
        {
            var cardSetFromDb = await _context.CardSets.FirstOrDefaultAsync(c => c.Id == model.CardSet.Id);
            if (cardSetFromDb != null)
            {
                var cardsFromDb = await _context.Cards.Where(c => c.CardSetId == cardSetFromDb.Id).ToListAsync();
                _context.RemoveRange(cardsFromDb);

                cardSetFromDb.Name = model.CardSet.Name;
                cardSetFromDb.Description = model.CardSet.Description;
                cardSetFromDb.CardSubjectId = model.CardSet.CardSubjectId;
                cardSetFromDb.IsPublic = model.CardSet.IsPublic;
                cardSetFromDb.DateUpdated = DateTime.UtcNow;

                model.CardSet.Cards.ForEach(c =>
                {
                    c.CardSetId = cardSetFromDb.Id;
                });

                await _context.Cards.AddRangeAsync(model.CardSet.Cards);

                await _context.SaveChangesAsync();
                return cardSetFromDb.Id;
            }
            return 0;
        }

        public async Task<CreateCardSetViewModel> GetCardSetForCopyAsync(int id, string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var isUserAdmin = false;
            if (user != null)
                isUserAdmin = await _userManager.IsInRoleAsync(user, DefaultAppValues.AdminRole);

            var cardSet = await _context.CardSets
                .Where(c => c.Id == id)
                .PublicOrUserIsOwner(userId, isUserAdmin)
                .NotDeleted()
                .SelectForEditOrCopy()
                .FirstOrDefaultAsync();

            if (cardSet != null)
            {
                var cards = await _context.Cards.Where(c => c.CardSetId == cardSet.Id).ToListAsync();
                cardSet.Cards = cards;
                return new CreateCardSetViewModel
                {
                    CardSet = cardSet,
                    CardCategories = await GetAllCardCategoriesAsync(),
                    CardSubjectsListJson = await GetAllCardSubjectsJsonAsync(),
                    SelectedCardCategoryId = cardSet.CardSubject.CardCategoryId,
                    AddManyCards = DefaultAppValues.AddManyCards,
                    ActionType = CreateSetActionType.Copy
                };
            }
            return new CreateCardSetViewModel();
        }

        public async Task<bool> DeleteCardSet(int? id, string? userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var cardSet = await _context.CardSets
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();

            var isUserAdmin = await _userManager.IsInRoleAsync(user, DefaultAppValues.AdminRole);

            if (cardSet != null && (user.Id == cardSet.UserId || isUserAdmin))
            {
                cardSet.IsDeleted = true;
                if (!isUserAdmin)
                {
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            return false;
        }

        public async Task<bool> MakeCardSetPrivate(int? id, string? userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var cardSet = await _context.CardSets
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();

            var isUserAdmin = await _userManager.IsInRoleAsync(user, DefaultAppValues.AdminRole);

            if (cardSet != null && (user.Id == cardSet.UserId || isUserAdmin))
            {
                cardSet.IsPublic = false;
                _context.CardSets.Update(cardSet);
                if (!isUserAdmin)
                {
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            return false;
        }

        public async Task AddCardSetAcccessAsync(int id, string userId)
        {
            await _context.CardSetAccesses.AddAsync(new CardSetAccess
            {
                DateAccessed = DateTime.UtcNow,
                CardSetId = id,
                UserId = userId
            });
            await _context.SaveChangesAsync();
        }

        public async Task<FrontPageCardSetListsViewModel> GetFrontPageCardSetsAsync(string? userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var isUserAdmin = false;
            if (user != null)
                isUserAdmin = await _userManager.IsInRoleAsync(user, DefaultAppValues.AdminRole);

            List<CardSet>? recentlyAccessedCardSets = new List<CardSet>();
            List<CardSet>? recentlyEdittedUserCardSets = new List<CardSet>();

            if (userId != null)
            {
                var recentlyAccessedCardSetsAll = await _context.CardSetAccesses
                    .Include(c => c.CardSet)
                    .Where(c => c.UserId == userId && !c.CardSet.IsDeleted && (c.CardSet.IsPublic || c.CardSet.UserId == userId))
                    .OrderByDescending(c => c.DateAccessed)
                    .Take(100)
                    .ToListAsync();

                var recentlyAccessedCardSetIds = recentlyAccessedCardSetsAll
                    .Select(c => c.CardSetId)
                    .Distinct()
                    .Take(6)
                    .ToList();

                var recentlyAccessedCardSetsUnsorted = await _context.CardSets
                    .Where(c => recentlyAccessedCardSetIds.Contains(c.Id))
                    .PublicOrUserIsOwner(userId, isUserAdmin)
                    .NotDeleted()
                    .SelectDefaultCardSetDataForView()
                    .ToListAsync();

                var recentlyAccessedCardSetsSorted = new List<CardSet>();
                foreach (var setId in recentlyAccessedCardSetIds)
                {
                    recentlyAccessedCardSetsSorted
                        .Add(recentlyAccessedCardSetsUnsorted.Where(c => c.Id == setId).First());
                }

                recentlyAccessedCardSets = recentlyAccessedCardSetsSorted;

                recentlyEdittedUserCardSets = await _context.CardSets
                    .Where(c => c.UserId == userId)
                    .NotDeleted()
                    .OrderByDescending(c => c.DateUpdated)
                    .Take(6)
                    .SelectDefaultCardSetDataForView()
                    .ToListAsync();
            }

            var recentlyEdittedPublicCardSets = await _context.CardSets
                .Where(c => c.UserId != userId && c.IsPublic)
                .NotDeleted()
                .OrderByDescending(c => c.DateUpdated)
                .Take(6)
                .SelectDefaultCardSetDataForView()
                .ToListAsync();

            return new FrontPageCardSetListsViewModel
            {
                MostRecentAccessedCardSets = new CardSetListViewModel
                {
                    CardSets = recentlyAccessedCardSets,
                    ListName = "Sets recently accessed by you"
                },
                MostRecentPublicCardSets = new CardSetListViewModel
                {
                    CardSets = recentlyEdittedPublicCardSets,
                    ListName = "Recently editted public sets"
                },
                MostRecentUserCardSets = new CardSetListViewModel
                {
                    CardSets = recentlyEdittedUserCardSets,
                    ListName = "Your recently editted sets"
                },
            };
        }

        public async Task<CardSetQuizViewModel> GetCardSetQuizAsync(int id, string? userId)
        {
            var cardSet = await _context.CardSets
               .Include(c => c.Cards)
               .NotDeleted()
               .FirstOrDefaultAsync(c => c.Id == id);

            if (cardSet != null)
            {
                List<FourOptionsQuizQuestion> fourOptionsQuestions = new List<FourOptionsQuizQuestion>();
                List<TrueFalseQuizQuestion> trueFalseQuestions = new List<TrueFalseQuizQuestion>();
                List<BasicQuizQuestion> basicQuestions = new List<BasicQuizQuestion>();
                //shuffle order of cards
                var allCards = cardSet.Cards.ShuffleArray();
                if (allCards.Count() >= 4)
                {
                    Random rnd = new Random();
                    //During quiz, an asnwer is the question, and questions are possible answer to it
                    var allAnswers = new List<string>();
                    foreach (var card in allCards)
                    {
                        allAnswers.Add(card.Question);
                    }
                    int maximumRandomValue = (int)Enum.GetValues(typeof(QuizQuestionType)).Cast<QuizQuestionType>().Max() + 1;
                    foreach (var card in allCards)
                    {
                        var questionType = rnd.Next(maximumRandomValue);
                        switch ((QuizQuestionType)questionType)
                        {
                            case QuizQuestionType.FourOptionsQuestion:
                                allAnswers.Remove(card.Question);

                                var possibleAnswers = allAnswers.ShuffleArray().Take(3).ToList();
                                possibleAnswers.Add(card.Question);
                                possibleAnswers = possibleAnswers.ShuffleArray().ToList();

                                var quizQuestion = new FourOptionsQuizQuestion
                                {
                                    Question = card.Answer,
                                    CorrectAnswer = card.Question,
                                    PossibleAnswers = possibleAnswers
                                };
                                fourOptionsQuestions.Add(quizQuestion);
                                allAnswers.Add(card.Question);
                                break;
                            case QuizQuestionType.TrueFalseQuestion:
                                var trueFalseQuestion = new TrueFalseQuizQuestion
                                {
                                    Question = card.Answer
                                };
                                var trueOrFalse = rnd.Next(2);
                                if (trueOrFalse == 1)
                                {
                                    trueFalseQuestion.PossibleAnswer = card.Question;
                                    trueFalseQuestion.CorrectAnswer = true;

                                    trueFalseQuestions.Add(trueFalseQuestion);
                                }
                                else
                                {
                                    allAnswers.Remove(card.Question);

                                    trueFalseQuestion.PossibleAnswer = allAnswers.ShuffleArray().Take(1).FirstOrDefault();
                                    trueFalseQuestion.CorrectAnswer = false;

                                    allAnswers.Add(card.Question);
                                    trueFalseQuestions.Add(trueFalseQuestion);
                                }
                                break;
                            case QuizQuestionType.BasicTextQuestion:
                                var basicQuestion = new BasicQuizQuestion
                                {
                                    Question = card.Answer,
                                    CorrectAnswer = card.Question
                                };
                                basicQuestions.Add(basicQuestion);

                                break;
                        }
                    }
                    return new CardSetQuizViewModel
                    {
                        CardSet = cardSet,
                        FourOptionsQuestions = fourOptionsQuestions,
                        TrueFalseQuestions = trueFalseQuestions,
                        BasicQuizQuestions = basicQuestions
                    };
                }
                return new CardSetQuizViewModel();
            }
            return new CardSetQuizViewModel();
        }
    }
}
