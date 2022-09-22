using FlashCards.Data.Services;
using FlashCards.Models;
using FlashCards.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FlashCards.Controllers
{
	[Route("Sets")]
	public class CardsController : Controller
	{
		private readonly ICardsService _service;
		public CardsController(ICardsService service)
		{
			_service = service;
		}

		//[Authorize]
		//[HttpGet("{userNickName}")]
		//public async Task<IActionResult> UserSets(string userNickName)
		//{
		//	return RedirectToAction(nameof(UserSets), new {userNickName = userNickName, page = 1 });
		//}

		//[Authorize]
		//[HttpGet("{requestedUserNickName}/page/{page:int?}")]
		//public async Task<IActionResult> UserSets(string requestedUserNickName, int page = 1, int cardsPerPage = 12)
		//{
		//	string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		//	SetsPageViewModel setsPageViewModel = await _service.GetAllUserCardSetsAsync(requestedUserNickName, userId, page, cardsPerPage);

		//	if (setsPageViewModel.CardSets != null)
		//	{
		//		return View(nameof(Sets), setsPageViewModel);
		//	}
		//	return RedirectToAction("Index", "Home");
		//}

		//[Authorize]
		//[HttpGet("MySets/{categoryName}/page/{page:int?}")]
		//public async Task<IActionResult> MySetsCategory(string categoryName, int page = 1, int cardsPerPage = 12)
		//{
		//	string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		//	SetsPageViewModel setsPageViewModel = await _service.GetAllOwnerCardSetsAsync(userId, page, cardsPerPage);

		//	if (setsPageViewModel.CardSets != null)
		//	{
		//		return View(nameof(Sets), setsPageViewModel);
		//	}
		//	return RedirectToAction("Index", "Home");
		//}

		//[Authorize]
		//[HttpGet("MySets/{categoryName}/{subjectName}/page/{page:int?}")]
		//public async Task<IActionResult> MySetsSubject(string categoryName, string subjectName, int page = 1, int cardsPerPage = 12)
		//{
		//	string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		//	SetsPageViewModel setsPageViewModel = await _service.GetAllOwnerCardSetsAsync(userId, page, cardsPerPage);

		//	if (setsPageViewModel.CardSets != null)
		//	{
		//		return View(nameof(Sets), setsPageViewModel);
		//	}
		//	return RedirectToAction("Index", "Home");
		//}

		


		[HttpGet("page/{page:int?}")]
		public async Task<IActionResult> Sets(string categoryName, int page = 1, int cardsPerPage = 12, string? name = null, string? numberOfCards = null, string? author = null, string? sort = null)
		{
			SetsPageViewModel setsPageViewModel = await _service.GetAllCardSetsAsync(page, cardsPerPage, name, numberOfCards, author, sort);

				return View(nameof(Sets), setsPageViewModel);

			//return RedirectToAction("Index", "Home");
		}

		[HttpGet("{categoryName}")]
		public IActionResult Category(string categoryName, string subjectName)
		{
			return RedirectToAction(nameof(Category), new { categoryName = categoryName, page = 1 });
		}

		[HttpGet("{categoryName}/page/{page:int?}")]
		public async Task<IActionResult> Category(string categoryName, int page = 1, int cardsPerPage = 12, string? name = null, string? numberOfCards = null, string? author = null, string? sort = null)
		{
			CategoryPageViewModel categoryPageViewModel = await _service.GetPublicCardCategoryWithItsCardSetsAsync(categoryName, page, cardsPerPage, name, numberOfCards, author, sort);

			if(categoryPageViewModel.CardCategory != null)
            {
				return View(nameof(Category), categoryPageViewModel);
			}
			return RedirectToAction("Index", "Home");
		}

		[HttpGet("{categoryName}/{subjectName}")]
		public IActionResult Subject(string categoryName, string subjectName)
		{
			return RedirectToAction(nameof(Subject), new {categoryName = categoryName, subjectName = subjectName, page = 1});
		}

		[HttpGet("{categoryName}/{subjectName}/page/{page:int?}")]
		public async Task<IActionResult> Subject(string categoryName, string subjectName, int page = 1, int cardsPerPage = 12, string? name = null, string? numberOfCards = null, string? author = null, string? sort = null)
		{
			SubjectPageViewModel subjectPageViewModel = await _service.GetCardSubjectWithItsCardSetsAsync(categoryName, subjectName, page, cardsPerPage, name, numberOfCards, author, sort);

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

		[HttpGet("GetCardSetPreview/{cardSetId}/{count}")]
		public async Task<IActionResult> GetCardSetPreview(int cardSetId, int count)
		{
			var cardSetPreview = await _service.GetCardSetPreviewAsync(cardSetId, count);
			if(cardSetPreview != null)
			{
				return PartialView("_PreviewCardSetInnerModal", cardSetPreview);
			}
			return NotFound();
		}


	}
}
