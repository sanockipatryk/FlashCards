using FlashCards.Data.Services;
using FlashCards.Models.Types.Enums;
using FlashCards.Helpers;
using FlashCards.Models;
using FlashCards.Models.ViewModels;
using FlashCards.SSoT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FlashCards.Controllers
{
    [Route("Sets")]
    public class CardsController : Controller
    {
        private readonly ICardsService _cardsService;

        public CardsController(ICardsService cardsService)
        {
            _cardsService = cardsService;
        }

        //GET: View single set
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Set(int id)
        {
            string? userId = UserManagerExtensions.GetUserId(User);

            var cardSet = await _cardsService.GetCardSetAsync(id, userId);
            if (cardSet.Name != null)
            {
                return View(cardSet);

            }
            return View("Error", new ErrorViewModel
            {
                ErrorStatus = "Bad Request",
                ErrorMessage = "Requested resource could not be accessed."
            });
        }

        //GET: View quiz for set; Quiz generated for Sets with at least 4 or more cards

        [Authorize]
        [HttpGet("Quiz/{id:int}")]
        public async Task<IActionResult> Quiz(int id)
        {
            string? userId = UserManagerExtensions.GetUserId(User);

            var cardSetQuiz = await _cardsService.GetCardSetQuizAsync(id, userId);
            if (cardSetQuiz.CardSet != null)
            {
                return View(cardSetQuiz);

            }
            return View("Error", new ErrorViewModel
            {
                ErrorStatus = "Bad Request",
                ErrorMessage = "This quiz could not be accessed."
            });
        }

        //GET: Browse sets -> redirect to action with page parameter

        [HttpGet]
        public IActionResult Sets()
        {
            return RedirectToAction(nameof(Sets), new { page = DefaultAppValues.Page });
        }

        //GET: Browse all sets with page parameters and filters

        [HttpGet("page/{page:int?}")]
        public async Task<IActionResult> Sets(
            CardSetFiltersViewModel cardSetFilters,
            int page = DefaultAppValues.Page,
            int setsPerPage = DefaultAppValues.SetsPerPage
            )
        {
            string? userId = UserManagerExtensions.GetUserId(User);

            SetsPageViewModel setsPageViewModel = await _cardsService.GetAllCardSetsAsync(cardSetFilters, page, setsPerPage, userId);

            return View(nameof(Sets), setsPageViewModel);
        }

        //GET: Browse sets from a category -> redirect to action with page parameter

        [HttpGet("{categoryName}")]
        public IActionResult Category(string categoryName)
        {
            return RedirectToAction(nameof(Category), new { categoryName = categoryName, page = DefaultAppValues.Page });
        }

        //GET: Browse all sets from a category with page parameters and filters

        [HttpGet("{categoryName}/page/{page:int?}")]
        public async Task<IActionResult> Category(
            string categoryName,
            CardSetFiltersViewModel cardSetFilters,
            int page = DefaultAppValues.Page,
            int setsPerPage = DefaultAppValues.SetsPerPage)
        {
            string? userId = UserManagerExtensions.GetUserId(User);

            CategoryPageViewModel categoryPageViewModel =
                await _cardsService.GetAllCardSetsOfCategoryAsync(
                    categoryName, cardSetFilters, page, setsPerPage, userId);

            if (categoryPageViewModel.CardCategory != null)
            {
                categoryPageViewModel.CardSetListData.ActionName = nameof(Category);
                return View(nameof(Category), categoryPageViewModel);
            }

            return View("Error", new ErrorViewModel
            {
                ErrorStatus = "Wrong category",
                ErrorMessage = "Requested category does not exist."
            });
        }

        //GET: Browse sets from a subject -> redirect to action with page parameter

        [HttpGet("{categoryName}/{subjectName}")]
        public IActionResult Subject(string categoryName, string subjectName)
        {
            return RedirectToAction(nameof(Subject),
                new { categoryName = categoryName, subjectName = subjectName, page = DefaultAppValues.Page });
        }

        //GET: Browse all sets from a subject with page parameters and filters

        [HttpGet("{categoryName}/{subjectName}/page/{page:int?}")]
        public async Task<IActionResult> Subject(
            string categoryName,
            string subjectName,
            CardSetFiltersViewModel cardSetFilters,
            int page = DefaultAppValues.Page,
            int setsPerPage = DefaultAppValues.SetsPerPage
            )
        {
            string? userId = UserManagerExtensions.GetUserId(User);

            SubjectPageViewModel subjectPageViewModel =
                await _cardsService.GetAllCardSetsOfSubjectAsync(
                    categoryName, subjectName, cardSetFilters, page, setsPerPage, userId);

            if (subjectPageViewModel.CardSubject != null)
            {
                subjectPageViewModel.CardSetListData.ActionName = nameof(Subject);
                return View(nameof(Subject), subjectPageViewModel);
            }

            return View("Error", new ErrorViewModel
            {
                ErrorStatus = "Wrong category or subject",
                ErrorMessage = "Requested category or subject does not exist."
            });
        }

        //GET: Get model for creating new set

        [Authorize]
        [HttpGet("CreateSet")]
        public async Task<IActionResult> CreateSet()
        {

            // Temp data used for storing current model when adding/removing cards from the model - 
            // this way, the complex model can be passed to another action, without changing the URL of user

            var model = TempData.Get<CreateCardSetViewModel>("CreateSetModel");
            if (model != null)
            {
                TempData.Remove("CreateSetModel");
                return View(model);
            }

            var createCardSetViewModel = new CreateCardSetViewModel
            {
                CardCategories = await _cardsService.GetAllCardCategoriesAsync(),
                CardSubjectsListJson = await _cardsService.GetAllCardSubjectsJsonAsync(),
                CardSet = new CardSet()
                {
                    IsPublic = true,
                    Cards = new List<Card>()
                    {
                        new Card(),
                        new Card(),
                        new Card()
                    }
                },
                SelectedCardCategoryId = DefaultAppValues.SelectedCardCategoryId,
                AddManyCards = DefaultAppValues.AddManyCards,
                ActionType = CreateSetActionType.Create
                // Model is used also for editing and copying set - based on ActionType field
            };
            return View(createCardSetViewModel);
        }

        //POST: Creating new set

        [Authorize]
        [ValidateAntiForgeryToken]
        [ActionName("CreateSet")]
        [HttpPost("CreateSet")]
        public async Task<IActionResult> CreateSetPOST(CreateCardSetViewModel model)
        {
            if (!ModelState.IsValid)
            {

                model.CardCategories = await _cardsService.GetAllCardCategoriesAsync();
                model.CardSubjectsListJson = await _cardsService.GetAllCardSubjectsJsonAsync();
                return View(nameof(CreateSet), model);
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cardSetId = await _cardsService.CreateCardSetAsync(model, userId);
            return RedirectToAction(nameof(Set), new { id = cardSetId });
        }

        //GET: Get model for editing a set

        [Authorize("MustBeCardSetOwner")]
        [HttpGet("EditSet/{id:int}")]
        public async Task<IActionResult> EditSet(int id)
        {
            if (!ModelState.IsValid)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorStatus = "Bad Request",
                    ErrorMessage = "Requested resource could not be accessed."
                });
            }

            var model = TempData.Get<CreateCardSetViewModel>("CreateSetModel");
            if (model != null)
            {
                TempData.Remove("CreateSetModel");
                return View(nameof(CreateSet), model);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cardSetViewModel = await _cardsService.GetCardSetForEditAsync(id);

            return View(nameof(CreateSet), cardSetViewModel);
        }

        //POST: Editing a set
        [Authorize]
        [ValidateAntiForgeryToken]
        [ActionName("EditSet")]
        [HttpPost("EditSet/{id}")]
        public async Task<IActionResult> EditSetPOST(CreateCardSetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CardCategories = await _cardsService.GetAllCardCategoriesAsync();
                model.CardSubjectsListJson = await _cardsService.GetAllCardSubjectsJsonAsync();
                return View(nameof(CreateSet), model);
            }
            var cardSetId = await _cardsService.EditCardSetAsync(model);
            if (cardSetId != 0)
            {
                return RedirectToAction(nameof(Set), new { id = cardSetId });
            }

            return View("Error", new ErrorViewModel
            {
                ErrorStatus = "Something went wrong",
                ErrorMessage = "Editing set was not successful."
            });

        }

        //GET: Get model for copying a set

        [Authorize]
        [HttpGet("CopySet/{id:int}")]
        public async Task<IActionResult> CopySet(int id)
        {
            if (!ModelState.IsValid)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorStatus = "Bad Request",
                    ErrorMessage = "Requested resource could not be accessed."
                });
            }

            var model = TempData.Get<CreateCardSetViewModel>("CreateSetModel");
            if (model != null)
            {
                TempData.Remove("CreateSetModel");
                return View(nameof(CreateSet), model);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cardSetForCopy = await _cardsService.GetCardSetForCopyAsync(id, userId);

            return View(nameof(CreateSet), cardSetForCopy);
        }

        //POST: Copying a set

        [Authorize]
        [ValidateAntiForgeryToken]
        [ActionName("CopySet")]
        [HttpPost("CopySet/{id}")]
        public async Task<IActionResult> CopySetPOST(CreateCardSetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CardCategories = await _cardsService.GetAllCardCategoriesAsync();
                model.CardSubjectsListJson = await _cardsService.GetAllCardSubjectsJsonAsync();
                return View(nameof(CreateSet), model);
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cardSetId = await _cardsService.CreateCardSetAsync(model, userId);
            return RedirectToAction(nameof(Set), new { id = cardSetId });
        }

        [Authorize("MustBeCardSetOwner")]
        [ValidateAntiForgeryToken]
        [HttpPost("DeleteSet/{id:int}")]
        public async Task<IActionResult> DeleteSet(int id)
        {
            if (!ModelState.IsValid)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorStatus = "Bad Request",
                    ErrorMessage = "Requested resource could not be accessed."
                });
            }

            string? userId = UserManagerExtensions.GetUserId(User);

            var result = await _cardsService.DeleteCardSet(id, userId);

            if (result)
            {
                return RedirectToAction(nameof(Sets), new { page = DefaultAppValues.Page });
            }

            return View("Error", new ErrorViewModel
            {
                ErrorStatus = "Bad Request",
                ErrorMessage = "Requested resource could not be accessed."
            });

        }

        //POST: Adding one or more empty cards to model - depends on model.AddManyCards value

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("AddCardToModel")]
        public async Task<IActionResult> AddCardToModel(CreateCardSetViewModel model)
        {
            model.CardCategories = await _cardsService.GetAllCardCategoriesAsync();
            model.CardSubjectsListJson = await _cardsService.GetAllCardSubjectsJsonAsync();
            if (model.CardSet.Cards == null)
            {
                model.CardSet.Cards = new List<Card>();
            }
            for (int i = 0; i < model.AddManyCards; i++)
            {
                model.CardSet.Cards.Add(new Card());
            }
            model.AddManyCards = DefaultAppValues.AddManyCards;

            TempData.Put("CreateSetModel", model); // storing complex model in tempdata with the use of json

            switch (model.ActionType)
            {
                case CreateSetActionType.Edit:
                    return RedirectToAction(nameof(EditSet), new { id = model.CardSet.Id });
                case CreateSetActionType.Copy:
                    return RedirectToAction(nameof(CopySet), new { id = model.CardSet.Id });
                default:
                    return RedirectToAction(nameof(CreateSet));
                    // redirecting to acton instead of returning View(nameof(CreateSet)) to keep the same URL
            }
        }

        //POST: Removing a card from model; cant remove when there is only one card left

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("DeleteCardFromModel/{cardIndex}")]
        public async Task<IActionResult> DeleteCardFromModel(CreateCardSetViewModel model, int cardIndex)
        {
            model.CardCategories = await _cardsService.GetAllCardCategoriesAsync();
            model.CardSubjectsListJson = await _cardsService.GetAllCardSubjectsJsonAsync();
            if (model.CardSet.Cards.Count > 1)
            {
                model.CardSet.Cards.RemoveAt(cardIndex);
            }
            model.AddManyCards = DefaultAppValues.AddManyCards;
            TempData.Put("CreateSetModel", model); // storing model in tempdata with the use of json

            switch (model.ActionType)
            {
                case CreateSetActionType.Edit:
                    return RedirectToAction(nameof(EditSet), new { id = model.CardSet.Id });
                case CreateSetActionType.Copy:
                    return RedirectToAction(nameof(CopySet), new { id = model.CardSet.Id });
                default:
                    return RedirectToAction(nameof(CreateSet));
                    // redirecting to acton instead of returning View(nameof(CreateSet)) to keep the same URL
            }
        }

        //GET: Getting SelectList of all subjects available for specified flashcard category

        [HttpGet("GetSubjectsForCardCategory/{categoryId}")]
        public async Task<IActionResult> GetSubjectsForCardCategory(int categoryId)
        {
            var cardSubjects = await _cardsService.GetCardSubjectsForCategoryAsync(categoryId);

            return Json(new SelectList(cardSubjects, "Id", "Name"));
        }

        //GET: Getting list of questions+answers to preview for user before commiting to entering the set page

        [HttpGet("GetCardSetPreview/{cardSetId}/{count}")]
        public async Task<IActionResult> GetCardSetPreview(int cardSetId, int count)
        {
            string? userId = UserManagerExtensions.GetUserId(User);

            var cardSetPreview = await _cardsService.GetCardSetPreviewAsync(cardSetId, count, userId);
            if (cardSetPreview != null)
            {
                return PartialView("_PreviewCardSetInnerModal", cardSetPreview);
            }
            return NotFound();
        }

        //GET: Getting list of cards in set


        [HttpGet("GetCardsForCardSet/{id}")]
        public async Task<IActionResult> GetCardsForCardSet(int id)
        {
            string? userId = UserManagerExtensions.GetUserId(User);

            var cards = await _cardsService.GetCardsForCardSetAsync(id, userId);
            if (cards.Count() > 0)
            {
                return Json(cards);

            }
            return Json(null);
        }

        [HttpGet("GetAllCardSubjects")]
        public async Task<IActionResult> GetAllCardSubjects()
        {
            var cardSubjects = await _cardsService.GetAllCardSubjectsAsync();
            return Json(cardSubjects);
        }

    }
}
