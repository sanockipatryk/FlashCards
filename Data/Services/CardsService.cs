﻿using FlashCards.Helpers;
using FlashCards.Models;
using FlashCards.Models.ViewModels;
using FlashCards.SSoT;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;

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

        public async Task<SetsPageViewModel> GetAllCardSetsAsync(
            int currentPage,
            int cardsPerPage,
            string? name,
            string? numberOfCards,
            string? author,
            string? sort,
            string? userId)
        {
            var cardSetsCount = _context.CardSets
                .PublicOrUserIsOwner(userId)
                .ApplyFilters(name, numberOfCards, author)
                .Count();

            var numberOfPages = PaginationHelpers.GetTotalPages(cardSetsCount, cardsPerPage);
            currentPage = PaginationHelpers.GetMinOrMaxPageIfOverBounds(currentPage, numberOfPages);

            var cardSets = await _context.CardSets
                .PublicOrUserIsOwner(userId)
                .ApplyFilters(name, numberOfCards, author)
                .ApplySort(sort)
                .ApplyPagination(currentPage, cardsPerPage, cardSetsCount)
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
                Filters = new CardSetFiltersViewModel
                {
                    Name = name,
                    NumberOfCards = numberOfCards,
                    Author = author,
                    SortBy = sort
                }
            };
        }

        public async Task<CategoryPageViewModel> GetAllCardSetsOfCategoryAsync(
            string categoryName,
            int currentPage,
            int cardsPerPage,
            string? name,
            string? numberOfCards,
            string? author,
            string? sort,
            string? userId)
        {
            var cardCategory = await _context.CardCategories.Include(c => c.CardSubjects)
            .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower());
            if (cardCategory != null)
            {
                var cardSubjectIds = cardCategory.CardSubjects.Select(cs => cs.Id).ToList();
                var cardSetsCount = _context.CardSets
                    .Where(c => cardSubjectIds.Contains(c.CardSubjectId))
                    .PublicOrUserIsOwner(userId)
                    .ApplyFilters(name, numberOfCards, author)
                    .Count();

                var numberOfPages = PaginationHelpers.GetTotalPages(cardSetsCount, cardsPerPage);
                currentPage = PaginationHelpers.GetMinOrMaxPageIfOverBounds(currentPage, numberOfPages);
                var cardSets = await _context.CardSets
                    .Where(c => cardSubjectIds.Contains(c.CardSubjectId))
                    .PublicOrUserIsOwner(userId)
                    .ApplyFilters(name, numberOfCards, author)
                    .ApplySort(sort)
                    .ApplyPagination(currentPage, cardsPerPage, cardSetsCount)
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

        public async Task<SubjectPageViewModel> GetAllCardSetsOfSubjectAsync(
            string categoryName,
            string subjectName,
            int currentPage,
            int cardsPerPage,
            string? name,
            string? numberOfCards,
            string? author,
            string? sort,
            string? userId)
        {
            var cardSubject = await _context.CardSubjects
                .Include(c => c.CardCategory)
                .FirstOrDefaultAsync(c => c.Name.ToLower() == subjectName.ToLower() && c.CardCategory.Name.ToLower() == categoryName.ToLower());

            if (cardSubject != null)
            {
                var cardSetCount = _context.CardSets
                    .ApplyFilters(name, numberOfCards, author)
                    .PublicOrUserIsOwner(userId)
                    .Count(c => c.CardSubjectId == cardSubject.Id);

                var numberOfPages = PaginationHelpers.GetTotalPages(cardSetCount, cardsPerPage);
                currentPage = PaginationHelpers.GetMinOrMaxPageIfOverBounds(currentPage, numberOfPages);

                var cardSets = await _context.CardSets
                    .Where(c => c.CardSubjectId == cardSubject.Id)
                    .PublicOrUserIsOwner(userId)
                    .ApplyFilters(name, numberOfCards, author)
                    .ApplySort(sort)
                    .ApplyPagination(currentPage, cardsPerPage, cardSetCount)
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


        public async Task<IEnumerable<QuestionWithAnswerViewModel>> GetCardSetPreviewAsync(int cardSetId, int count, string? userId)
        {
            var cardSet = await _context.CardSets.Include(c => c.Cards)
                .Where(c => c.Id == cardSetId)
                .PublicOrUserIsOwner(userId)
                .FirstOrDefaultAsync();
            if (cardSet != null)
            {
                var questionsWithAnswers = new List<QuestionWithAnswerViewModel>();
                cardSet.Cards.Skip(count).Take(DefaultAppValues.PreviewCardCount).ToList()
                    .ForEach(c => questionsWithAnswers.Add(new QuestionWithAnswerViewModel { Question = c.Question, Answer = c.Answer }));

                return questionsWithAnswers;
            }
            return null;
        }

        public async Task<CardSetViewModel> GetCardSetAsync(int id, string? userId)
        {
            var cardSet = await _context.CardSets
                .Where(c => c.Id == id)
                .PublicOrUserIsOwner(userId)
                .Select(c => new CardSet
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    DateUpdated = c.DateUpdated,
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
                var cardCount = _context.Cards.Count(c => c.CardSetId == cardSet.Id);
                return new CardSetViewModel
                {
                    Id = cardSet.Id,
                    Name = cardSet.Name,
                    Description = cardSet.Description,
                    DateUpdated = cardSet.DateUpdated,
                    User = cardSet.User,
                    CardSubject = cardSet.CardSubject,
                    CardCount = cardCount
                };
            }
            return new CardSetViewModel();
        }

        public async Task<IEnumerable<Card>> GetCardsForCardSetAsync(int id, string? userId)
        {
            var cardSet = await _context.CardSets
                .Where(c => c.Id == id)
                .PublicOrUserIsOwner(userId)
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
            var cardSet = await _context.CardSets.AddAsync(model.CardSet);
            await _context.SaveChangesAsync();
            return cardSet.Entity.Id;
        }

        public async Task<CreateCardSetViewModel> GetCardSetForEditAsync(int id)
        {
            var cardSet = await _context.CardSets.Select(c => new CardSet
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CardSubjectId = c.CardSubjectId,
                CardSubject = new CardSubject
                {
                    Id = c.CardSubject.Id,
                    CardCategoryId = c.CardSubject.CardCategoryId
                },
                DateUpdated = c.DateUpdated,
                IsPublic = c.IsPublic,
                UserId = c.UserId,

            }).FirstOrDefaultAsync(c => c.Id == id);
            if (cardSet != null)
            {
                var cards = await _context.Cards.Where(c => c.CardSetId == cardSet.Id).ToListAsync();
                cardSet.Cards = cards;
                return new CreateCardSetViewModel
                {
                    CardSet = cardSet,
                    CardCategories = await GetAllCardCategoriesAsync(),
                    SelectedCardCategoryId = cardSet.CardSubject.CardCategoryId,
                    AddManyCards = DefaultAppValues.AddManyCards,
                    ActionType = Enums.CreateSetActionType.Edit
                };
            }
            return new CreateCardSetViewModel();
        }

        public async Task EditCardSetAsync(CreateCardSetViewModel model)
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

                model.CardSet.Cards.ForEach(c =>
                {
                    c.CardSetId = cardSetFromDb.Id;
                });

                await _context.Cards.AddRangeAsync(model.CardSet.Cards);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<CreateCardSetViewModel> GetCardSetForCopyAsync(int id, string userId)
        {
            var cardSet = await _context.CardSets
                .Where(c => c.Id == id)
                .PublicOrUserIsOwner(userId)
                .Select(c => new CardSet
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CardSubjectId = c.CardSubjectId,
                    CardSubject = new CardSubject
                    {
                        Id = c.CardSubject.Id,
                        CardCategoryId = c.CardSubject.CardCategoryId
                    },
                    DateUpdated = c.DateUpdated,
                    IsPublic = c.IsPublic,
                    UserId = c.UserId,
                })
                .FirstOrDefaultAsync();

            if (cardSet != null)
            {
                var cards = await _context.Cards.Where(c => c.CardSetId == cardSet.Id).ToListAsync();
                cardSet.Cards = cards;
                return new CreateCardSetViewModel
                {
                    CardSet = cardSet,
                    CardCategories = await GetAllCardCategoriesAsync(),
                    SelectedCardCategoryId = cardSet.CardSubject.CardCategoryId,
                    AddManyCards = DefaultAppValues.AddManyCards,
                    ActionType = Enums.CreateSetActionType.Copy
                };
            }
            return new CreateCardSetViewModel();
        }

        public async Task<bool> DeleteCardSet(int id, string? userId)
        {
            var cardSet = await _context.CardSets
            .Where(c => c.Id == id && userId != null && c.UserId == userId)
            .FirstOrDefaultAsync();

            if (cardSet != null)
            {
                _context.CardSets.Remove(cardSet);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
