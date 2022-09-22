using FlashCards.Models;
using FlashCards.Models.ViewModels;

namespace FlashCards.Data.Services
{
	public interface ICardsService
	{
		Task<IEnumerable<CardCategory>> GetAllCardCategoriesAsync();
		Task<IEnumerable<CardCategory>> GetAllCardCategoriesWithSubjectsAsync();
		Task<SetsPageViewModel> GetAllCardSetsAsync(int currentPage, int cardsPerPage, string? name, string? numberOfCards, string? author, string? sort);
		Task<CategoryPageViewModel> GetPublicCardCategoryWithItsCardSetsAsync(string categoryName, int currentPage, int cardsPerPage, string? name, string? numberOfCards, string? author, string? sort);
		Task<IEnumerable<CardSubject>> GetAllCardSubjectsAsync();
		Task<SubjectPageViewModel> GetCardSubjectWithItsCardSetsAsync(string categoryName, string subjectName, int currentPage, int cardsPerPage, string? name, string? numberOfCards, string? author, string? sort);
		Task<IEnumerable<CardSubject>> GetCardSubjectsForCategoryAsync(int categoryId);
		Task<IEnumerable<CardSet>> GetAllPublicCardSetsAsync();
		Task<SetsPageViewModel> GetAllOwnerCardSetsAsync(string userId, int currentPage, int cardsPerPage);
		Task<IEnumerable<CardSet>> GetAllUserCardSetsAsync(string requestedUserNickName, int currentPage, int cardsPerPage);
		Task<IEnumerable<CardSet>> GetAllPublicCardSetsForSubjectAsync(int subjectId);
		Task<IEnumerable<QuestionWithAnswerViewModel>> GetCardSetPreviewAsync(int cardSetId, int count);

		Task CreateCardSetAsync(CreateCardSetViewModel model, string userId);

		Task<bool> IsUserTheOwner(string userId, string requestedUserNickName);

	}
}
