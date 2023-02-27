using FlashCards.Models;
using FlashCards.Models.ViewModels;
using FlashCards.Models.Quiz;

namespace FlashCards.Data.Services
{
    public interface ICardsService
    {
        Task<IEnumerable<CardCategory>> GetAllCardCategoriesAsync();
        Task<IEnumerable<CardCategory>> GetAllCardCategoriesWithSubjectsAsync();
        Task<IEnumerable<CardSubject>> GetAllCardSubjectsAsync();
        Task<string> GetAllCardSubjectsJsonAsync();
        Task<IEnumerable<CardSubject>> GetCardSubjectsForCategoryAsync(int categoryId);

        Task<SetsPageViewModel> GetAllCardSetsAsync(CardSetFiltersViewModel cardSetFilters, int currentPage, int setsPerPage, string? userId);
        Task<CategoryPageViewModel> GetAllCardSetsOfCategoryAsync(string categoryName, CardSetFiltersViewModel cardSetFilters, int currentPage, int setsPerPage, string? userId);
        Task<SubjectPageViewModel> GetAllCardSetsOfSubjectAsync(string categoryName, string subjectName, CardSetFiltersViewModel cardSetFilters, int currentPage, int setsPerPage, string? userId);

        Task<IEnumerable<QuestionWithAnswerViewModel>> GetCardSetPreviewAsync(int cardSetId, int count, string? userId);
        Task<CardSetViewModel> GetCardSetAsync(int id, string? userId);
        Task<IEnumerable<Card>> GetCardsForCardSetAsync(int id, string? userId);

        Task<int> CreateCardSetAsync(CreateCardSetViewModel model, string userId);
        Task<CreateCardSetViewModel> GetCardSetForEditAsync(int id);
        Task<int> EditCardSetAsync(CreateCardSetViewModel model);

        Task<CreateCardSetViewModel> GetCardSetForCopyAsync(int id, string userId);

        Task<bool> DeleteCardSet(int? id, string? userId);
        Task<bool> MakeCardSetPrivate(int? id, string? userId);

        Task AddCardSetAcccessAsync(int id, string userId);
        Task<FrontPageCardSetListsViewModel> GetFrontPageCardSetsAsync(string? userId);

        Task<CardSetQuizViewModel> GetCardSetQuizAsync(int id, string? userId);
    }
}
