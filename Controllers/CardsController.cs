using FlashCards.Data.Services;
using FlashCards.Models;
using FlashCards.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FlashCards.Controllers
{
	[Route("Cards")]
	public class CardsController : Controller
	{
		private readonly ICardsService _service;
		public CardsController(ICardsService service)
		{
			_service = service;
		}
		
		[HttpGet("{categoryName}")]
		public async Task<IActionResult> Category(string categoryName)
		{
			CategoryPageViewModel categoryPageViewModel = await _service.GetCardCategoryWithItsCardSets(categoryName);

			if(categoryPageViewModel.CardCategory != null)
            {
				return View(nameof(Category), categoryPageViewModel);
			}
			return RedirectToAction("Index", "Home");
		}

		[HttpGet("{categoryName}/{subjectName}")]
		public async Task<IActionResult> Subject(string categoryName, string subjectName)
		{
			SubjectPageViewModel subjectPageViewModel = await _service.GetCardSubjectWithItsCardSets(categoryName, subjectName);

			if (subjectPageViewModel.CardSubject != null)
			{
				return View(nameof(Subject), subjectPageViewModel);
			}
			return RedirectToAction("Index", "Home");
		}

		[Authorize]
		[HttpGet("CreateSet")]
		public async Task<IActionResult> CreateSet()
        {
			var createCardSetViewModel = new CreateCardSetViewModel
			{
				CardCategories = await _service.GetAllCardCategoriesAsync(),
				CardSet = new CardSet()
                {
					Cards = new List<Card>()
					{
						new Card(),
						new Card(),
						new Card()
					}
				},
				SelectedCardCategoryId = -1,
				AddManyCards = 1
				
			};
			return View(createCardSetViewModel);
        }

		[Authorize]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public async Task<IActionResult> CreateSetPOST(CreateCardSetViewModel model)
		{
            if (!ModelState.IsValid)
            {
				model.CardCategories = await _service.GetAllCardCategoriesAsync();
				return View(nameof(CreateSet), model);
            }
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			await _service.CreateCardSetAsync(model, userId);
			return RedirectToAction("Index", "Home");
		}

		[HttpPost("AddCardToModel")]
		public async Task<IActionResult> AddCardToModel(CreateCardSetViewModel model)
        {
            model.CardCategories = await _service.GetAllCardCategoriesAsync();
			for(int i = 0; i<model.AddManyCards; i++)
            {
				model.CardSet.Cards.Add(new Card());
            }
			model.AddManyCards = 1;
			ModelState.Clear(); //For clearing validation errors
			return View(nameof(CreateSet), model);
		}

		[HttpPost("DeleteCardFromModel/{cardIndex}")]
		public async Task<IActionResult> DeleteCardFromModel(CreateCardSetViewModel model, int cardIndex)
        {
			model.CardCategories = await _service.GetAllCardCategoriesAsync();
			model.CardSet.Cards.RemoveAt(cardIndex);
			model.AddManyCards = 1;
			ModelState.Clear(); //For clearing validation errors
			return View(nameof(CreateSet), model);
		}

		[HttpGet("GetSubjectsForCardCategory/{categoryId}")]
		public async Task<IActionResult> GetSubjectsForCardCategory(int categoryId)
        {
			var cardSubjects =  await _service.GetCardSubjectsForCategoryAsync(categoryId);

			return Json(new SelectList(cardSubjects, "Id", "Name"));
		}
	}
}
