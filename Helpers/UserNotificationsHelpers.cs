using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashCards.Models;
using FlashCards.Models.Types.Enums;
using FlashCards.SSoT;

namespace FlashCards.Helpers
{
    public static class UserNotificationsHelpers
    {
        public static IQueryable<UserNotification> IfLoadingMore(this IQueryable<UserNotification> query, UserNotification? lastNotification)
        {
            if (lastNotification != null)
            {
                return query.Where(q => q.DateSent < lastNotification.DateSent);
            }
            return query;
        }


        public static string GenerateMessageForUserWhoReportedASet(
            ReportCause cause,
            ReportResponse response,
            string nickName,
            string cardSetName)
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine($"Dear {nickName},");
            str.AppendLine("Thank you for submiting a report.");
            str.AppendLine("Your actions allow us at Flashcards to maintain high standards for public sets created by our community.");
            str.AppendLine("");
            str.AppendLine($"You reported the set: {cardSetName} for the reason: {cause.GetDisplayName()}.");
            str.AppendLine("");
            str.Append("After reviewing the set, an administrator decided that ");
            switch (response)
            {
                case ReportResponse.NoAction:
                    str.Append("the set did meet our community standards, and required no action.");
                    break;
                case ReportResponse.HideSet:
                    str.Append("the set did not meet our community standards. Due to that, the set was hidden, and the user was notified of his wrongdoing. ");
                    break;
                case ReportResponse.DeleteSet:
                    str.Append("the set did not meet our community standards. Due to that, the set was fully removed, and the user was notified of his wrongdoing. ");
                    break;
                case ReportResponse.SendMessage:
                    str.Append("the user will be notified of his wrongdoing and instructed to fix his set content.");
                    break;
            }
            str.AppendLine("\n");
            str.AppendLine("Once again thank you for taking your time to submit a report.");
            str.AppendLine();
            str.AppendLine("Best regards,");
            str.AppendLine("FlashCards Team.");

            return str.ToString();
        }

        public static string GenerateMessageForOwnerOfReportedSet(
            ReportCause cause,
            ReportResponse response,
            string nickName,
            string cardSetName,
            string? messageToUser)
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine($"Dear {nickName},");
            str.AppendLine($"Your set: {cardSetName} was recently reported for the reason: {cause.GetDisplayName()}.");
            str.AppendLine("");
            str.Append("After reviewing the set, an administrator decided that ");

            switch (response)
            {
                case ReportResponse.HideSet:
                    str.AppendLine("the set did not meet our community standards. Due to that, the set was hidden from public access.");
                    break;
                case ReportResponse.DeleteSet:
                    str.AppendLine("the set did not meet our community standards. Due to that, the set was fully removed.");
                    break;
                case ReportResponse.SendMessage:
                    str.AppendLine("the set does not meet all of our community standards. Please review it and make changes in order to correct it.");
                    break;
            }
            if (messageToUser != null && messageToUser.Length > 0)
            {
                str.AppendLine("The administrator provided an explanation: ");
                str.AppendLine();
                str.AppendLine(messageToUser);
                str.AppendLine();
            }
            str.AppendLine("We hope you will continue to be a part of our community.");
            str.AppendLine();
            str.AppendLine("Best regards,");
            str.AppendLine("FlashCards Team.");

            return str.ToString();
        }

    }
}