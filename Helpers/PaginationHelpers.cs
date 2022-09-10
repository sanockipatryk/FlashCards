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
    }
}
