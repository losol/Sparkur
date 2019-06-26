using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sparkur.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class AdminController : Controller {

   			[Route("admin")]
			public IActionResult Index()
			{
				return View();
			}

			[Route("admin/maintenance")]
			public IActionResult Maintenance()
			{
				return View();
			}
	}
}