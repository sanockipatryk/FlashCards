using FlashCards.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlashCards.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CardCategory> CardCategories { get; set; }
        public DbSet<CardSubject> CardSubjects { get; set; }
        public DbSet<CardSet> CardSets { get; set; }
        public DbSet<Card> Cards { get; set; }
    }
}