using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc; 
using AutoGen.Data.Models; 
using AutoGen.Data.RepoInterfaces; 
 
namespace AutoGen.Controllers; 
 
[Authorize] 
public class HarmReductionController : Controller 
{ 
	// PMT Landmark
	public async Task<IActionResult> HarmReductionUpdate()
	{
		return View();
	}

	public async Task<IActionResult> HarmReductionRead()
	{
		return View();
	}

	public async Task<IActionResult> HarmReductionCreate()
	{
		return View();
	}
 
	 
} 
