using FlashCards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlashCards.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.CardCategories.Any())
                { }
                else
                {
                    string[] cardCategories = new string[]
                    {
                    "Languages", "Maths", "Science", "Human Sciences", "Arts", "Other"
                    };
                    string[] cardSubjects = new string[]
                    {
                    "English", "French", "German", "Spanish",
                    "Algebra", "Arithmetic", "Geometry", "Calculus",
                    "Biology", "Chemistry", "Physics", "Medicine",
                    "Psychology", "Business", "Economics", "Law",
                    "History", "Music", "Philosophy", "Literature",
                    "Hobby", "Sports", "Skills", "Other"
                    };
                    int row = 0;
                    foreach (var cardCategory in cardCategories)
                    {
                        var newCategory = context.CardCategories.Add(new CardCategory() { Name = cardCategory });
                        context.SaveChanges();

                        for (int i = 0; i < 4; i++)
                        {
                            context.CardSubjects.Add(new CardSubject()
                            {
                                Name = cardSubjects[row * 4 + i],
                                CardCategoryId = newCategory.Entity.Id
                            });
                        }
                        row++;
                    }

                    context.SaveChanges();
                }
                using (var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>())
                {
                    var user = new ApplicationUser
                    {
                        Email = "test@test.com",
                        Nickname = "test-user",
                        UserName = "test@test.com",
                        NormalizedEmail = "TEST@TEST.COM",
                        NormalizedUserName = "TEST@TEST.COM",
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString("D")
                    };
                    if (!context.Users.Any(u => u.UserName == user.Email))
                    {
                        var password = new PasswordHasher<ApplicationUser>();
                        var hashed = password.HashPassword(user, "secret");
                        user.PasswordHash = hashed;

                        var userStore = new UserStore<ApplicationUser>(context);
                        var result = userStore.CreateAsync(user);

                        context.SaveChangesAsync();
                    }
                }

                if (!context.Cards.Any())
                {
                    int i = 0;
                    var cardCategories = context.CardCategories.Include(c => c.CardSubjects).ToList();
                    var user = context.Users.FirstOrDefault(u => u.Email == "test@test.com");
                    foreach (var category in cardCategories)
                    {
                        foreach (var subject in category.CardSubjects)
                        {
                            for (int j = 0; j < 30; j++)
                            {
                                var cardSet = new CardSet()
                                {
                                    Name = $"Card set {j + i * 30}",
                                    Description = $"Description for Card set {j + i * 30}",
                                    DateCreated = DateTime.UtcNow,
                                    DateUpdated = DateTime.UtcNow,
                                    CardSubjectId = subject.Id,
                                    UserId = user.Id,
                                    IsPublic = true,
                                    Cards = new List<Card>()
                                };
                                context.CardSets.Add(cardSet);

                                context.SaveChanges();
                                for (int k = 0; k < 12; k++)
                                {
                                    cardSet.Cards.Add(new Card()
                                    {
                                        Question = $"Question {j + i * 30}/{k}",
                                        Answer = $"Answer {j + i * 30}/{k}",
                                        CardSetId = cardSet.Id
                                    });
                                }
                                context.AddRange(cardSet.Cards);
                            }
                            i++;
                        }
                        context.SaveChanges();
                    }
                }
            }

        }
    }
}
