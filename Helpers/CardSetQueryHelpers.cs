using FlashCards.Models;

namespace FlashCards.Helpers
{
    public static class CardSetQueryHelpers
    {
        public static IQueryable<CardSet> PublicOrUserIsOwner(this IQueryable<CardSet> set, string? userId, bool isUserAdmin) => set.Where(c => ((c.IsPublic || userId != null && c.UserId == userId) || isUserAdmin));
        public static IQueryable<CardSet> NotDeleted(this IQueryable<CardSet> set) => set.Where(c => !c.IsDeleted);

        public static IQueryable<CardSet> SelectForEditOrCopy(this IQueryable<CardSet> set)
        {
            return set.Select(c => new CardSet
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
            });
        }

        public static IQueryable<CardSet> SelectDefaultCardSetDataForView(this IQueryable<CardSet> set)
        {
            return set.Select(c => new CardSet
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
            });
        }
    }
}