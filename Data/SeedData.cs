using FlashCards.Models;
using FlashCards.SSoT;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlashCards.Data
{
    public static class SeedData
    {
        public static void SeedDbData(IServiceProvider serviceProvider)
        {
            SeedRoles(serviceProvider);
            SeedUsers(serviceProvider);
            SeedCardsData(serviceProvider);
        }

        public static void SeedRoles(IServiceProvider serviceProvider)
        {
            using (var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>())
            {
                if (roleManager.RoleExistsAsync(DefaultAppValues.AdminRole).Result)
                { }
                else
                {
                    roleManager.CreateAsync(new IdentityRole(DefaultAppValues.AdminRole)).Wait();
                    roleManager.CreateAsync(new IdentityRole(DefaultAppValues.UserRole)).Wait();
                }
            }
        }
        private static void SeedUsers(IServiceProvider serviceProvider)
        {
            using (var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                var user = new ApplicationUser
                {
                    Email = "user@user.com",
                    Nickname = "UserOne",
                    UserName = "user@user.com",
                };
                if (userManager.FindByEmailAsync(user.Email).Result != null)
                { }
                else
                {
                    var result = userManager.CreateAsync(user, "!Q1w2e3r4").Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, DefaultAppValues.UserRole).Wait();
                    }
                }

                var user2 = new ApplicationUser
                {
                    Email = "user2@user.com",
                    Nickname = "FlashCards",
                    UserName = "user2@user.com",
                };
                if (userManager.FindByEmailAsync(user2.Email).Result != null)
                { }
                else
                {
                    var result = userManager.CreateAsync(user2, "!Q1w2e3r4").Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user2, DefaultAppValues.UserRole).Wait();
                    }
                }

                var admin = new ApplicationUser
                {
                    Email = "admin@admin.com",
                    Nickname = "Admin1",
                    UserName = "admin@admin.com",
                };
                if (userManager.FindByEmailAsync(admin.Email).Result != null)
                {
                }
                else
                {
                    var result = userManager.CreateAsync(admin, "!Q1w2e3r4").Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(admin, DefaultAppValues.AdminRole).Wait();
                    }
                }
            }
        }
        private static void SeedCardsData(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.CardCategories.AnyAsync().Result)
                { }
                else
                {
                    (string, string)[] cardCategories = new (string, string)[]
                    {
                        ("Languages", "🧏‍♀️"),
                        ("Maths", "🧮"),
                        ("Science", "🧪"),
                        ("Human Sciences", "🌍"),
                        ("Arts", "🎨"),
                        ("Other", "❓")
                    };

                    string[][] cardSubjects = new string[][]
                    {
                    new string [] {"English", "Croatian", "Czech", "Danish", "Dutch", "Estonian", "Finnish",
                     "French", "German", "Greek", "Hungarian", "Irish", "Italian", "Latvian", "Lithuanian",
                     "Maltese", "Polish", "Portuguese", "Romanian", "Slovak", "Slovenian", "Spanish", "Swedish"},
                    new string [] {"Algebra", "Arithmetic", "Geometry", "Calculus", "Statistics", "Probability", "Algorithms and Complexity"},
                    new string [] {"Biology", "Chemistry", "Physics", "Medicine", "Astronomy", "Astrophysics", "Computer Science", "Artificial Intelligence" },
                    new string [] {"Psychology", "Business", "Economics", "Law", "History", "Education", "Criminology", "Health", "Archeology"},
                    new string [] {"Painting", "Music", "Philosophy", "Literature", "Photography"},
                    new string [] {"Hobby", "Sports", "Skills", "Other"}
                    };
                    int index = 0;
                    foreach (var cardCategory in cardCategories)
                    {
                        var newCategory = context.CardCategories.AddAsync(new CardCategory() { Name = cardCategory.Item1, Icon = cardCategory.Item2 }).Result;
                        context.SaveChangesAsync().Wait();

                        for (int i = 0; i < cardSubjects[index].Length; i++)
                        {
                            context.CardSubjects.AddAsync(new CardSubject()
                            {
                                Name = cardSubjects[index][i],
                                CardCategoryId = newCategory.Entity.Id
                            });
                        }
                        index++;
                    }

                    context.SaveChangesAsync().Wait();
                }

                if (!context.Cards.AnyAsync().Result)
                {
                    int i = 0;
                    var cardCategories = context.CardCategories.Include(c => c.CardSubjects).ToListAsync().Result;
                    var user = context.Users.FirstOrDefaultAsync(u => u.Email == "user2@user.com").Result;
                    foreach (var category in cardCategories)
                    {
                        foreach (var subject in category.CardSubjects)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                var cardSet = new CardSet()
                                {
                                    Name = $"Set {j + i * 2 + 1}",
                                    Description = $"Description for Set {j + i * 2 + 1}",
                                    DateCreated = DateTime.UtcNow,
                                    DateUpdated = DateTime.UtcNow,
                                    CardSubjectId = subject.Id,
                                    UserId = user.Id,
                                    IsPublic = true,
                                    Cards = new List<Card>()
                                };
                                context.CardSets.AddAsync(cardSet);

                                context.SaveChangesAsync().Wait();
                                for (int k = 0; k < 12; k++)
                                {
                                    cardSet.Cards.Add(new Card()
                                    {
                                        Question = $"Question {j + i * 2 + 1}/{k}",
                                        Answer = $"Answer {j + i * 2 + 1}/{k}",
                                        CardSetId = cardSet.Id
                                    });
                                }
                                context.AddRangeAsync(cardSet.Cards).Wait();
                                context.SaveChangesAsync().Wait();
                            }
                            i++;
                        }
                        context.SaveChangesAsync().Wait();
                    }
                }
            }

        }
    }
}
