using FlashCards.Helpers;
using FlashCards.Models;
using FlashCards.Models.ViewModels;
using FlashCards.Models.Types.Enums;
using Microsoft.EntityFrameworkCore;
using FlashCards.SSoT;

namespace FlashCards.Data.Services
{
    public class ReportsService : IReportsService
    {
        private readonly ApplicationDbContext _context;
        public ReportsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CardSetReportsListViewModel> GetReports(CardSetReportsFilterViewModel reportsFilters, int page, int reportsPerPage)
        {
            var reportsCount = await _context.CardSetReports.ApplyFilters(reportsFilters).CountAsync();

            var numberOfPages = PaginationHelpers.GetTotalPages(reportsCount, reportsPerPage);

            var allReports = await _context.CardSetReports
                .ApplyFilters(reportsFilters)
                .ApplySort(reportsFilters.SortBy)
                .ApplyPagination(page, reportsPerPage, reportsCount)
                .Select(r => new CardSetReport
                {
                    Id = r.Id,
                    ReportCause = r.ReportCause,
                    Description = r.Description,
                    CardSetId = r.CardSetId,
                    DateReported = r.DateReported,
                    IsEvaluated = r.IsEvaluated,
                    ReportResponse = r.ReportResponse,
                    MessageToReportedSetOwner = r.MessageToReportedSetOwner,
                    DateEvaluated = r.DateEvaluated,
                    User = new ApplicationUser
                    {
                        Email = r.User.Email,
                        Nickname = r.User.Nickname
                    },
                    EvaluatedBy = new ApplicationUser
                    {
                        Nickname = r.EvaluatedBy.Nickname
                    }
                })
                .ToListAsync();

            return new CardSetReportsListViewModel
            {
                Reports = allReports,
                Filters = reportsFilters,
                Pagination = new PaginationViewModel
                {
                    CurrentPage = page,
                    NumberOfPages = numberOfPages
                }
            };
        }

        public async Task<CardSetReport> GetReport(int id)
        {
            var report = await _context.CardSetReports
                .Select(r => new CardSetReport
                {
                    Id = r.Id,
                    ReportCause = r.ReportCause,
                    Description = r.Description,
                    CardSetId = r.CardSetId,
                    CardSet = new CardSet
                    {
                        IsPublic = r.CardSet.IsPublic,
                        IsDeleted = r.CardSet.IsDeleted
                    },
                    DateReported = r.DateReported,
                    IsEvaluated = r.IsEvaluated,
                    ReportResponse = r.ReportResponse,
                    EvaluationReasoning = r.EvaluationReasoning,
                    MessageToReportedSetOwner = r.MessageToReportedSetOwner,
                    DateEvaluated = r.DateEvaluated,
                    User = new ApplicationUser
                    {
                        Email = r.User.Email,
                        Nickname = r.User.Nickname
                    },
                    EvaluatedBy = new ApplicationUser
                    {
                        Email = r.User.Email,
                        Nickname = r.User.Nickname
                    },
                })
                .FirstOrDefaultAsync(r => r.Id == id);

            return report;
        }

        public async Task<int> GetCountOfUnevaluatedReports()
        {
            return await _context.CardSetReports.CountAsync(r => r.IsEvaluated == false);
        }

        public async Task<bool> SendReport(SendReportViewModel report, string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                var cardSetReport = new CardSetReport
                {
                    CardSetId = report.CardSetId,
                    ReportCause = report.ReportCause,
                    Description = report.Description.Length > 0 ? report.Description : null,
                    DateReported = DateTime.UtcNow,
                    UserId = userId,
                    IsEvaluated = false
                };
                var result = await _context.CardSetReports.AddAsync(cardSetReport);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        //int reportId, string adminId
        public async Task<CardSetReport> EvaluateReport(CardSetReport report, string adminId)
        {
            var reportFromDb = await _context.CardSetReports
                .Where(r => r.Id == report.Id)
                .Include(r => r.User)
                .Include(r => r.CardSet).ThenInclude(r => r.User)
                .FirstOrDefaultAsync();
            if (reportFromDb != null)
            {
                reportFromDb.DateEvaluated = DateTime.Now;
                reportFromDb.EvaluatingAdminId = adminId;
                reportFromDb.ReportResponse = report.ReportResponse;
                reportFromDb.MessageToReportedSetOwner = report.MessageToReportedSetOwner;
                reportFromDb.EvaluationReasoning = report.EvaluationReasoning;
                reportFromDb.IsEvaluated = true;
                _context.CardSetReports.Update(reportFromDb);
                return reportFromDb;
            }
            return new CardSetReport();
        }
        public async Task ConfirmEvaluation()
        {
            await _context.SaveChangesAsync();
        }
        public void RollbackEvaluation()
        {
            _context.ChangeTracker.Clear();
        }
    }
}