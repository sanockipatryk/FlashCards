using System.Security.Claims;
using FlashCards.Data.Services;
using FlashCards.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FlashCards.ViewComponents
{
    public class UnevaluatedReportCounterViewComponent : ViewComponent
    {
        private readonly IReportsService _service;
        public UnevaluatedReportCounterViewComponent(IReportsService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var unevaluatedReportsCount = await _service.GetCountOfUnevaluatedReports();
            return View(unevaluatedReportsCount);
        }
    }
}