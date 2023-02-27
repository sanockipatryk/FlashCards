using System.ComponentModel.DataAnnotations;

namespace FlashCards.Models.Types.Enums
{
    public enum CreateSetActionType
    {
        Create = 1,
        Edit = 2,
        Copy = 3
    };

    public enum SetsSortBy
    {
        [Display(Name = "Most recent")]
        Newest = 1,
        [Display(Name = "Least recent")]
        Oldest = 2,
        [Display(Name = "Name A-Z")]
        AZ = 3,
        [Display(Name = "Name Z-A")]
        ZA = 4,
    }

    public enum SetsNumberOfCards
    {
        [Display(Name = "1 - 19")]
        OneToTwenty = 1,
        [Display(Name = "20 - 39")]
        TwentyToForty = 2,
        [Display(Name = "40 - 50")]
        FortyAndMore = 3,
    }

    public enum ReportsSortBy
    {
        Newest = 1,
        Oldest = 2,
        Cause = 3
    }

    public enum ReportsShow
    {
        Unevaluated = 1,
        Evaluated = 2,
        All = 3,
    }

    public enum ReportCause
    {
        [Display(Name = "Set is empty")]
        SetEmpty = 1,
        [Display(Name = "Wrong category or subject")]
        WrongCategoryOrSubject = 2,
        [Display(Name = "Inappropriate content")]
        InappropriateContent = 3,
        [Display(Name = "Innacurate content")]
        InnacurateContent = 4
    }

    public enum ReportResponse
    {
        [Display(Name = "Don't take any action")]
        NoAction = 0,
        [Display(Name = "Make set private and send a notification")]
        HideSet = 1,
        [Display(Name = "Delete set and send a notification")]
        DeleteSet = 2,
        [Display(Name = "Send a notification")]
        SendMessage = 3
    }

    public enum QuizQuestionType
    {
        FourOptionsQuestion = 0,
        TrueFalseQuestion = 1,
        BasicTextQuestion = 2
    }
}
