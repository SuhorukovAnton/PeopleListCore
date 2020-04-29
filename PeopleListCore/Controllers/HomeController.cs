
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using NLog;
using PeopleList.Core;
using PeopleListCore.Helpers;
using PeopleListCore.Models;

namespace PeopleListCore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private Logger logger;
        private HelperWorkWithData helperWork;
        private ReaderFactory readerFactory;
        private readonly IConfiguration config;
        private readonly IStringLocalizer<Resource> Resource;
        private PeopleManager manager;
        private IWebHostEnvironment appEnvironment;
        public HomeController(IConfiguration config, IStringLocalizer<Resource> Resource, IWebHostEnvironment appEnvironment, PeopleManager manager, ReaderFactory readerFactory, HelperWorkWithData helperWork)
        {
            this.helperWork = helperWork;
            logger = LogManager.GetCurrentClassLogger();
            this.appEnvironment = appEnvironment;
            this.config = config;
            this.Resource = Resource;
            this.manager = manager;
            this.readerFactory = readerFactory;
        }
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Auth", "Account");
            }
            await WriteLangsToViewBag();

            if (!User.IsInRole("SuperAdmin"))
            {
                ViewData["hidden"] = true;
                if (User.IsInRole("Admin"))
                {
                    ViewData["canAdd"] = true;
                }
            }
            else
            {
                ViewData["canAdd"] = true;
            }
            return View(manager.GetPeoples());
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult> OpenAdd()
        {
           await WriteLangsToViewBag();
            return View("~/Views/Home/Add.cshtml");
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult> Add(FormAdd formAdd)
        {
            var isFind = await manager.FindEmail(formAdd.Email);

            if (isFind)
            {
                ModelState.AddModelError("Email", Resource["EmailIsBusy"]);
            }else if (ModelState.IsValid)
            {
                formAdd.Password = helperWork.GetHash(formAdd.Password);
                await manager.AddPeople(formAdd);
                return RedirectToAction("Index", "Home");
            }
            await WriteLangsToViewBag();
            return View(formAdd);
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> Remove(int id)
        {
            await manager.RemovePeople(id);
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Read(int id)
        {
           await WriteLangsToViewBag();
            ViewData["canEdit"] = (id == int.Parse(User.Identity.Name) || User.IsInRole("SuperAdmin"));
            return View("~/Views/Home/People.cshtml", await GetFormEdit(id));
        }

        public async Task<ActionResult> Edit(int id, FormEdit formEdit)
        {
            ViewData["canEdit"] = id == int.Parse(User.Identity.Name) || User.IsInRole("SuperAdmin");
            var peopleTmp = await manager.GetPeople(id);
            ViewData["Img"] = peopleTmp.Img;
            var isFind = await manager.FindEmail(formEdit.Email) && peopleTmp.Email != formEdit.Email;

            if (isFind)
            {
                ModelState.AddModelError("Email", Resource["EmailIsBusy"]);
            }
            else if (ModelState.IsValid)
            {
                await manager.EditPeople(formEdit);
                ViewData["Message"] = Resource["SaveIsSuccessfully"];
            }
            await WriteLangsToViewBag();
            return View("~/Views/Home/People.cshtml", formEdit);
        }

        public async Task<ActionResult> LoadImg(int id, IFormFile img)
        {
            if (img != null)
            {
                
                await manager.AddImg(id, await helperWork.Save(img, id));

                ViewData["canEdit"] = true;
                await WriteLangsToViewBag();
                return View("~/Views/Home/People.cshtml", await GetFormEdit(id));
            }

            return RedirectToAction("Index", "Home");
            
        }
        
        public async Task<ActionResult> Load(IFormFile file)
        {
            if (file != null)
            {
                var path =await helperWork.Save(file);
                var reader = readerFactory.GetFactory(Path.GetExtension(file.FileName).Substring(1));
                await reader.AddPeople(appEnvironment.WebRootPath + "/files/download/" + path);
            }

            return RedirectToAction("Index", "Home");
        }

        private ActionResult GetView()
        {
            ViewData["hiddenAdd"] = !User.IsInRole("Admin");
            return View("~/Views/Home/Index.cshtml");
        }

        public ActionResult List()
        {
            if (!User.IsInRole("SuperAdmin"))
            {
                ViewData["hidden"] = true;
            }
            return View(manager.GetPeoples());
        }

        private async Task<FormEdit> GetFormEdit(int id)
        {
            var people = await manager.GetPeople(id);
            ViewData["Img"] = people.Img;

            return new FormEdit
            {
                Id = id,
                Name = people.Name,
                Surname = people.Surname,
                Birthday = people.Birthday,
                Email = people.Email
            };
        }
        
        [AllowAnonymous]
        [HttpPost]
        public IActionResult SetLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return RedirectToAction("Index");
        }

        public FileResult UnloadPeoples(string format)
        {
            var reader = readerFactory.GetFactory(format);
            reader.Create(appEnvironment.WebRootPath + "/files/" + "peoples." + format);
            string file_path = Path.Combine(appEnvironment.ContentRootPath, "wwwroot/files/peoples." + format);
            string file_type = "application/" + format;
            string file_name = "peoples." + format;
            return PhysicalFile(file_path, file_type, file_name);
        }

        public async Task WriteLangsToViewBag()
        {
            var people = await manager.GetPeople(int.Parse(User.Identity.Name));
            ViewData["Email"] = people.Email;
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