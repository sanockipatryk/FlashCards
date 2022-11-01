using FlashCards.Models;
using FlashCards.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FlashCards.Helpers
{
    public static class CardSetFiltersHelpers
    {
        public static IQueryable<CardSet> PublicOrUserIsOwner(this IQueryable<CardSet> set, string? userId) => set.Where(c => c.IsPublic || userId != null && c.UserId == userId);

        public static bool FilterCardsCount(int cardsCount, string numberOfCards)
        {
            switch (numberOfCards)
            {
                case "lessThanTwenty": return cardsCount > 0 && cardsCount < 20;
                case "twentyToForty": return cardsCount >= 20 && cardsCount < 40;
                case "moreThanForty": return cardsCount >= 40;
                default:
                    return false;
            }
        }

        public static IQueryable<CardSet> ApplyFilters(this IQueryable<CardSet> set, string? name, string? numberOfCards, string? author)
        {
            if (name != null)
            {
                set = set.Where(c => name == null || c.Name.ToLower().Contains(name.ToLower()));
            }
            if (author != null)
            {
                set = set.Include(c => c.User)
                    .Where(c => author == null || c.User.Nickname.ToLower().Contains(author.ToLower()));
            }
            if (numberOfCards != null)
            {
                set = set.Include(c => c.Cards)
                    .Where(c => numberOfCards == null || numberOfCards.Equals("lessThanTwenty") ? c.Cards.Count > 0 && c.Cards.Count < 20 :
                        numberOfCards.Equals("twentyToForty") ? c.Cards.Count >= 20 && c.Cards.Count < 40 :
                        numberOfCards.Equals("moreThanForty") ? c.Cards.Count >= 40 : false);
            }
            return set;
        }

        public static IQueryable<CardSet> ApplySort(this IQueryable<CardSet> set, string? sort)
        {
            if (sort != null)
            {
                if (sort.Equals("oldest"))
                {
                    return set.OrderBy(c => c.DateUpdated);
                }
                if (sort.Equals("az"))
                {
                    return set.OrderBy(c => c.Name);
                }
                if (sort.Equals("za"))
                {
                    return set.OrderByDescending(c => c.Name);
                }
                return set.OrderByDescending(c => c.DateUpdated);
            }
            return set.OrderByDescending(c => c.DateUpdated);
        }
    }
}
