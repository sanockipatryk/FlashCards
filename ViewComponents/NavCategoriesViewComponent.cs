using FlashCards.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlashCards.ViewComponents
{
	public class NavCategoriesViewComponent : ViewComponent
	{
		private readonly ICardsService _service;
		public NavCategoriesViewComponent(ICardsService service)
		{
			_service = service;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var categories = await _service.GetAllCardCategoriesWithSubjectsAsync();
			return View(categories);
		}
	}
}
