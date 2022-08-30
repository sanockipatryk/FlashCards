using FlashCards.Models;
using FlashCards.Models.ViewModels;

namespace FlashCards.Data.Services
{
	public interface ICardsService
	{
		Task<IEnumerable<CardCategory>> GetAllCardCategoriesAsync();
		Task<IEnumerable<CardCategory>> GetAllCardCategoriesWithSubjectsAsync();
		Task<CategoryPageViewModel> GetCardCategoryWithItsCardSets(string categoryName);
		Task<IEnumerable<CardSubject>> GetAllCardSubjectsAsync();
		Task<SubjectPageViewModel> GetCardSubjectWithItsCardSets(string categoryName, string subjectName);
		Task<IEnumerable<CardSubject>> GetCardSubjectsForCategoryAsync(int categoryId);
		Task<IEnumerable<CardSet>> GetAllPublicCardSetsAsync();
		Task<IEnumerable<CardSet>> GetAllUserCardSetsAsync(string userId);
		Task<IEnumerable<CardSet>> GetAllPublicCardSetsForSubjectAsync(int subjectId);

	}
}
