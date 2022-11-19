using FlashCards.Models.Types.Enums;

namespace FlashCards.Models.ViewModels
{
    public class SendReportViewModel
    {
        public string? Description { get; set; }
        public int CardSetId { get; set; }
        public ReportCause ReportCause { get; set; }
    }
}