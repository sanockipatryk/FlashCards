using FlashCards.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashCards.Helpers
{
    public static class PaginationHelpers
    {
        public static int GetMinOrMaxPage(int currentPage, int totalPages)
        {
            if (currentPage > totalPages)
            {
                return totalPages;
            }
            if (currentPage < 1)
            {
                return 1;
            }
            else
            {
                return currentPage;
            }
        }
        public static int GetTotalPages(int itemsCount, int itemsPerPage)
        {
            return (int)Math.Ceiling((double)itemsCount / (double)itemsPerPage);
        }

        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> set, int currentPage, int cardsPerPage, int cardSetsCount) where T : class
        {
            if (cardSetsCount > 0)
            {
                return set
                    .Skip((currentPage - 1) * cardsPerPage)
                    .Take(cardsPerPage);
            }
            return set;
        }
    }
}
