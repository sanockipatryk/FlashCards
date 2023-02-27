
using FlashCards.Data.Services;
using FlashCards.Helpers;
using FlashCards.Models;
using FlashCards.Models.ViewModels;
using FlashCards.Models.Types.Enums;
using FlashCards.SSoT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashCards.Controllers
{
	[Route("[controller]")]
	public class ReportsController : Controller
	{
		private readonly IReportsService _reportsService;
		private readonly ICardsService _cardsService;
		private readonly INotificationsService _notificationService;

		public ReportsController(
			IReportsService reportsService,
			ICardsService cardsService,
			INotificationsService notificationService)
		{
			_reportsService = reportsService;
			_cardsService = cardsService;
			_notificationService = notificationService;
		}

		[Authorize(Policy = "RequireAdminRole")]
		[HttpGet("page/{page:int?}")]
		public async Task<IActionResult> Index(
			CardSetReportsFilterViewModel reportsFilters,
			int page = DefaultAppValues.Page,
			int reportsPerPage = DefaultAppValues.ReportsPerPage
			)
		{
			var allReports = await _reportsService.GetReports(reportsFilters, page, reportsPerPage);
			return View(allReports);
		}

		[Authorize(Policy = "RequireAdminRole")]
		[HttpGet("Details/{id:int}")]
		public async Task<IActionResult> Details(int id)
		{
			var reportToEvaluate = await _reportsService.GetReport(id);
			return View(reportToEvaluate);
		}

		[Authorize(Policy = "RequireAdminRole")]
		[ValidateAntiForgeryToken]
		[ActionName("EvaluateReport")]
		[HttpPost("EvaluateReport")]
		public async Task<IActionResult> EvaluateReport(CardSetReport report)
		{
			if (ModelState.GetFieldValidationState(nameof(report.EvaluationReasoning)) == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid &&
				(report.ReportResponse == ReportResponse.NoAction || ModelState.GetFieldValidationState(nameof(report.MessageToReportedSetOwner)) == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid))
			{
				string adminId = UserManagerExtensions.GetUserId(User);
				var evaluatedReport = await _reportsService.EvaluateReport(report, adminId);
				if (evaluatedReport.IsEvaluated == true)
				{

					// creating this message here to use it both in ReportResponse.NoAction case and default case
					var messageForUserWhoReported = UserNotificationsHelpers.GenerateMessageForUserWhoReportedASet
								(
									evaluatedReport.ReportCause,
									(ReportResponse)evaluatedReport.ReportResponse,
									evaluatedReport.User.Nickname,
									evaluatedReport.CardSet.Name
								);

					switch (evaluatedReport.ReportResponse)
					{
						case ReportResponse.NoAction:
							var sendNotificationToReportingUser = await _notificationService
									.SendNotification(evaluatedReport.UserId, DefaultAppValues.YourReportWasEvaluatedTitle, messageForUserWhoReported);

							if (sendNotificationToReportingUser)
							{
								await _reportsService.ConfirmEvaluation();
							}
							else
							{
								_reportsService.RollbackEvaluation();
							}
							break;
						case ReportResponse.HideSet:
							var hideSetResult = await _cardsService.MakeCardSetPrivate(evaluatedReport.CardSetId, adminId);
							if (hideSetResult)
							{
								goto default;
							}
							else
							{
								_reportsService.RollbackEvaluation();
								break;
							}
						case ReportResponse.DeleteSet:

							var deleteSetResult = await _cardsService.DeleteCardSet(evaluatedReport.CardSetId, adminId);
							if (deleteSetResult)
							{
								goto default;
							}
							else
							{
								_reportsService.RollbackEvaluation();
								break;
							}

						case ReportResponse.SendMessage:
						default:
							var messageForOwnerOfReportedSet = UserNotificationsHelpers.GenerateMessageForOwnerOfReportedSet
								(
									evaluatedReport.ReportCause,
									(ReportResponse)evaluatedReport.ReportResponse,
									evaluatedReport.CardSet.User.Nickname,
									evaluatedReport.CardSet.Name,
									evaluatedReport.MessageToReportedSetOwner
								);

							var notificationForReportedUser = await _notificationService
								.SendNotification(evaluatedReport.UserId, DefaultAppValues.YourReportWasEvaluatedTitle, messageForUserWhoReported);

							var notificationForOwnerOfReportedSet = await _notificationService
								.SendNotification(evaluatedReport.CardSet.UserId, DefaultAppValues.YourSetWasReportedTitle, messageForOwnerOfReportedSet);

							if (notificationForReportedUser && notificationForOwnerOfReportedSet)
							{
								await _reportsService.ConfirmEvaluation();
							}
							else
							{
								_reportsService.RollbackEvaluation();
							}
							return RedirectToAction(nameof(Details), new { id = evaluatedReport.Id });
					}
					return RedirectToAction(nameof(Details), new { id = evaluatedReport.Id });
				}
				return View(nameof(Details), report);
			}
			return View(nameof(Details), report);
		}

		[Authorize]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public async Task<IActionResult> SendReport([FromBody] SendReportViewModel report)
		{
			if (ModelState.IsValid)
			{
				string userId = UserManagerExtensions.GetUserId(User);
				var reportSent = await _reportsService.SendReport(report, userId);
				if (reportSent)
				{
					return Ok();
				}
				return BadRequest();
			}
			return BadRequest();
		}
	}
}