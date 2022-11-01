using FlashCards.Models;
using FlashCards.Models.ViewModels;

namespace FlashCards.Data.Services
{
    public interface ICardsService
    {
        Task<IEnumerable<CardCategory>> GetAllCardCategoriesAsync();
        Task<IEnumerable<CardCategory>> GetAllCardCategoriesWithSubjectsAsync();
        Task<IEnumerable<CardSubject>> GetAllCardSubjectsAsync();
        Task<IEnumerable<CardSubject>> GetCardSubjectsForCategoryAsync(int categoryId);
        
        Task<SetsPageViewModel> GetAllCardSetsAsync(int currentPage, int cardsPerPage, string? name, string? numberOfCards, string? author, string? sort, string? userId);
        Task<CategoryPageViewModel> GetAllCardSetsOfCategoryAsync(string categoryName, int currentPage, int cardsPerPage, string? name, string? numberOfCards, string? author, string? sort, string? userId);
        Task<SubjectPageViewModel> GetAllCardSetsOfSubjectAsync(string categoryName, string subjectName, int currentPage, int cardsPerPage, string? name, string? numberOfCards, string? author, string? sort, string? userId);

        Task<IEnumerable<QuestionWithAnswerViewModel>> GetCardSetPreviewAsync(int cardSetId, int count, string? userId);
        Task<CardSetViewModel> GetCardSetAsync(int id, string? userId);
        Task<IEnumerable<Card>> GetCardsForCardSetAsync(int id, string? userId);

        Task<int> CreateCardSetAsync(CreateCardSetViewModel model, string userId);
        Task<CreateCardSetViewModel> GetCardSetForEditAsync(int id);
        Task EditCardSetAsync(CreateCardSetViewModel model);

        Task<CreateCardSetViewModel> GetCardSetForCopyAsync(int id, string userId);

        Task<bool> DeleteCardSet(int id, string? userId);
    }
}
