using FlashCards.Models;
using FlashCards.Models.Types.Enums;
using FlashCards.Models.ViewModels;

namespace FlashCards.Helpers
{
    public static class CardSetReportsFiltersHelpers
    {
        public static IQueryable<CardSetReport> ApplyFilters(this IQueryable<CardSetReport> query, CardSetReportsFilterViewModel reportsFilters)
        {
            if (reportsFilters.CardSetId != null)
            {
                query = query.Where(q => q.CardSetId == reportsFilters.CardSetId);
            }

            if (reportsFilters.Cause != null)
            {
                query = query.Where(q => q.ReportCause == reportsFilters.Cause);
            }

            if (reportsFilters.Show != null)
            {
                switch (reportsFilters.Show)
                {
                    case ReportsShow.Evaluated:
                        return query.Where(q => q.IsEvaluated);
                    case ReportsShow.Unevaluated:
                        return query.Where(q => !q.IsEvaluated);
                    case ReportsShow.All:
                    default:
                        return query;
                }
            }
            return query;
        }

        public static IQueryable<CardSetReport> ApplySort(this IQueryable<CardSetReport> query, ReportsSortBy? sortBy)
        {
            if (sortBy != null)
            {
                switch (sortBy)
                {
                    case ReportsSortBy.Oldest:
                        return query.OrderBy(q => q.DateReported);
                    case ReportsSortBy.Cause:
                        return query.OrderByDescending(q => q.ReportCause);
                    case ReportsSortBy.Newest:
                    default:
                        return query.OrderByDescending(q => q.DateReported);
                }
            }
            return query.OrderByDescending(q => q.DateReported);
        }
    }
}
