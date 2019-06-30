using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sparkur.Areas.Admin.Controllers
{
	[Authorize(Policy = "RequireAdministratorRole")]
	[Area("Admin")]
	public class AdminController : Controller {

			
   			[Route("admin"), HttpGet]
			public IActionResult Index()
			{
				return View();
			}

			[Route("admin/maintenance"), HttpGet]
			public IActionResult Maintenance()
			{
				return View();
			}
	}
}