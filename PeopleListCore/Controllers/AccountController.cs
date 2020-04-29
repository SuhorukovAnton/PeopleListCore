using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PeopleListCore.Models;
using PeopleListCore.Helpers;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace PeopleListCore.Controllers
{
    

    public class AccountController : Controller
    {
        private readonly IConfiguration config;
        private readonly IStringLocalizer<Resource> Resource;
        private HelperWorkWithData helperWork;
        private PeopleManager manager;
        public AccountController(IConfiguration config, IStringLocalizer<Resource> Resource, PeopleManager manager, HelperWorkWithData helperWork)
        {
            this.helperWork = helperWork;
            this.manager = manager;
            this.config = config;
            this.Resource = Resource;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Auth(FormAuth form)
        {
            if (ModelState.IsValid)
            {
                var people = await manager.FindUser(form.Email, form.Password);
                if (people != null)
                {
                    await Authenticate(people.id.ToString(), people.Role); 

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", Resource["UserIsNotFound"]);
                }
            }
            WriteLangsToViewBag();
            ViewData["Auth"] = true;
            return View(form);
        }
        [HttpGet]
        public ActionResult Auth()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["Auth"] = true;
            WriteLangsToViewBag();
            return View();
               
        }


        private async Task Authenticate(string userId, Roles role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userId),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role.ToString())
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<ActionResult> Logoff()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        private void WriteLangsToViewBag()
        {

            List<string> langs = new List<string>(config["Langs"].Split(','));
            List<string> nameLangs = new List<string>();
            langs.ForEach(elem =>
            {
                nameLangs.Add(helperWork.FirstUpper(new CultureInfo(elem).NativeName));
            });
            ViewBag.langs = langs;
            ViewBag.langsFullName = nameLangs;
        }

    }
}
