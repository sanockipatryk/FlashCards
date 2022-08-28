using FlashCards.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlashCards.Controllers
{
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
	}
}
