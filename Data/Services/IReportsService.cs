using FlashCards.Models;
using FlashCards.Models.ViewModels;
using FlashCards.Models.Types.Enums;

namespace FlashCards.Data.Services
{
    public interface IReportsService
    {
        Task<CardSetReportsListViewModel> GetReports(CardSetReportsFilterViewModel reportsFilters, int page, int reportsPerPage);
        Task<CardSetReport> GetReport(int id);
        Task<int> GetCountOfUnevaluatedReports();
        Task<bool> SendReport(SendReportViewModel report, string userId);
        Task<CardSetReport> EvaluateReport(CardSetReport report, string adminId);

        Task ConfirmEvaluation();
        void RollbackEvaluation();
    }
}