using FlashCards.Models;

namespace FlashCards.Data.Services
{
	public interface ICardsService
	{
		Task<IEnumerable<CardCategory>> GetAllCardCategoriesAsync();
		Task<IEnumerable<CardCategory>> GetAllCardCategoriesWithSubjectsAsync();
		Task<IEnumerable<CardSubject>> GetAllCardSubjectsAsync();
		Task<IEnumerable<CardSubject>> GetCardSubjectsForCategoryAsync(int categoryId);
		Task<IEnumerable<CardSet>> GetAllPublicCardSetsAsync();
		Task<IEnumerable<CardSet>> GetAllUserCardSetsAsync(string userId);
		Task<IEnumerable<CardSet>> GetAllPublicCardSetsForSubjectAsync(int subjectId);

	}
}
