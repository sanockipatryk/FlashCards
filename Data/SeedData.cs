using FlashCards.Models;
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
                {
                    return;
                }

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
                    
                    for(int i = 0; i<4; i++)
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
        }
    }
}
