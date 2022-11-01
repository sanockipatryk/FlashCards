using FlashCards.Models;

namespace FlashCards.Data.Services
{
    public interface IUsersService
    {
        Task<UserSetsData> GetUserSetsDataAsync(string userId);
    }
}
