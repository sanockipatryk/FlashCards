namespace FlashCards.Helpers
{
    public static class PaginationHelpers
    {
        public static int GetMinOrMaxPageIfOverBounds(int currentPage, int totalPages)
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

        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> queryItem, int currentPage, int itemsPerPage, int allItemsCount) where T : class
        {
            if (allItemsCount > 0)
            {
                return queryItem
                    .Skip((currentPage - 1) * itemsPerPage)
                    .Take(itemsPerPage);
            }
            return queryItem;
        }
    }
}
