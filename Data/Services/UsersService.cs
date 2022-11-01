using FlashCards.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashCards.Data.Services
{
    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext _context;

        public UsersService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserSetsData> GetUserSetsDataAsync(string userId) {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if(user != null) {
                var cardSets = await _context.CardSets
                    .Where(c => c.UserId == user.Id).Select(c => new CardSet {
                    IsPublic = c.IsPublic,
                })
                .ToListAsync();
                var cards = await _context.Cards
                    .Where(c => c.CardSet.UserId == user.Id)
                    .Select(c => new Card {
                        CardSet = new CardSet {
                            UserId = c.CardSet.UserId
                        }
                    })
                    .ToListAsync();
            
            return new UserSetsData {
                TotalSetCount = cardSets.Count(),
                PublicSetCount = cardSets.Count(c => c.IsPublic),
                PrivateSetCount = cardSets.Count(c => !c.IsPublic),
                TotalCardsCount = cards.Count(),
            };
            }
            return new UserSetsData();
        }
        
    }
}
