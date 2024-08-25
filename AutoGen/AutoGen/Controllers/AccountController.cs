using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc; 
using AutoGen.Data.Models; 
using AutoGen.Data.RepoInterfaces; 
 
namespace AutoGen.Controllers; 
 
[Authorize] 
public class AccountController : Controller 
{ 
	// PMT Landmark
	public async Task<IActionResult> Register()
	{
		return View();
	}

	public async Task<IActionResult> RecoverPassword()
	{
		return View();
	}

	public async Task<IActionResult> Login()
	{
		return View();
	}
 
	 
} 
