using FlashCards.Data.Services;
using FlashCards.Models;
using FlashCards.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
		//public async Task<IActionResult> Index()
		//{
		//	return View();
		//}
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

		[HttpGet("CreateSet")]
		public async Task<IActionResult> CreateSet()
        {
			var createCardSetViewModel = new CreateCardSetViewModel
			{
				CardCategories = await _service.GetAllCardCategoriesAsync(),
				CardSet = new CardSet(),
				SelectedCardCategoryId = -1
			};
			return View(createCardSetViewModel);
        }

		[HttpPost]
		public async Task<IActionResult> CreateSetPOST(CreateCardSetViewModel model)
		{
            if (!ModelState.IsValid)
            {
				model.CardCategories = await _service.GetAllCardCategoriesAsync();
				return View(nameof(CreateSet), model);
            }
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
