using FlashCards.Models.Types.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCards.Models
{
    public class CardSetReport
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        [DisplayName("Report cause")]
        public ReportCause ReportCause { get; set; }

        [DisplayName("Reporting user")]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [DisplayName("Set Id")]
        [ForeignKey(nameof(CardSet))]
        public int? CardSetId { get; set; }
        public CardSet? CardSet { get; set; }

        [DisplayName("Date reported")]
        public DateTime DateReported { get; set; }
        public bool IsEvaluated { get; set; }
        public DateTime? DateEvaluated { get; set; }
        [ForeignKey(nameof(EvaluatedBy))]
        public string? EvaluatingAdminId { get; set; }
        public ApplicationUser? EvaluatedBy { get; set; }
        public ReportResponse? ReportResponse { get; set; }
        [MinLength(20, ErrorMessage = "Please enter a reasoning with minimum 20 characters.")]
        public string? EvaluationReasoning { get; set; }

        [MinLength(20, ErrorMessage = "Please enter a explanation with minimum 20 characters.")]
        public string? MessageToReportedSetOwner { get; set; }

    }
}
