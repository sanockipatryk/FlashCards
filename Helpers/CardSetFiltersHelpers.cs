using FlashCards.Models;
using FlashCards.Models.Types.Enums;
using FlashCards.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FlashCards.Helpers
{
    public static class CardSetFiltersHelpers
    {
        public static IQueryable<CardSet> ApplyFilters(this IQueryable<CardSet> query, CardSetFiltersViewModel cardSetFilters)
        {
            if (cardSetFilters.Name != null)
            {
                query = query.Where(c => cardSetFilters.Name == null || c.Name.ToLower().Contains(cardSetFilters.Name.ToLower()));
            }
            if (cardSetFilters.Author != null)
            {
                query = query.Include(c => c.User)
                    .Where(c => cardSetFilters.Author == null || c.User.Nickname.ToLower().Contains(cardSetFilters.Author.ToLower()));
            }
            if (cardSetFilters.NumberOfCards != null)
            {
                query = query.Include(c => c.Cards);
                switch (cardSetFilters.NumberOfCards)
                {
                    case SetsNumberOfCards.OneToTwenty:
                        query = query.Where(c => c.Cards.Count > 0 && c.Cards.Count < 20);
                        break;
                    case SetsNumberOfCards.TwentyToForty:
                        query = query.Where(c => c.Cards.Count >= 20 && c.Cards.Count < 40);
                        break;
                    case SetsNumberOfCards.FortyAndMore:
                        query = query.Where(c => c.Cards.Count >= 40);
                        break;
                }
            }
            return query;
        }

        public static IQueryable<CardSet> ApplySort(this IQueryable<CardSet> query, SetsSortBy? sortBy)
        {
            switch (sortBy)
            {
                case SetsSortBy.Oldest:
                    return query.OrderBy(c => c.DateUpdated);
                case SetsSortBy.AZ:
                    return query.OrderBy(c => c.Name);
                case SetsSortBy.ZA:
                    return query.OrderByDescending(c => c.Name);
                case SetsSortBy.Newest:
                default:
                    return query.OrderByDescending(c => c.DateUpdated);
            }
        }
    }
}
