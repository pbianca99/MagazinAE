using MagazinAE.Models.VMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace MagazinAE.Controllers
{
    [Route("[Controller]")]
    public class UserController : Controller
    {
        private readonly MagazinAEContext context;

        public UserController(MagazinAEContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var list = context.Users.Select(p => new UserVM().UserToUserVM(p)).ToList();
            return View(list);
        }

        [HttpGet]
        [Route("New")]
        public IActionResult New()
        {
            var user = new UserVM();
            return View(user);
        }

        [HttpPost]
        [Route("New")]
        public IActionResult Create(UserVM dto)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "There were some errors in your form");
                return View("New", dto);
            }

            if(dto.Password != dto.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Password and Confirmed Password doesn't match");
                return View("New", dto);
            }

            dto.Password = Base64.Base64Encode(dto.ConfirmPassword);

            context.Users.Add(UserVM.VMUserToUser(dto));
            context.SaveChanges();

            return View("Index", context.Users.Select(p => new UserVM().UserToUserVM(p)).ToList());
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var user = context.Users.FirstOrDefault(p => p.Id == id);

            if (user == null)
                return View("Index", context.Users.Select(p => new UserVM().UserToUserVM(p)).ToList());
            else
                return View(new UserVM().UserToUserVM(user));
        }

        [HttpPost]
        [Route("Edit/{id}")]
        public IActionResult Edit(int id, UserVM dto)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "There were some errors in your form");
                return View("Edit", dto);
            }


            var user = context.Users.FirstOrDefault(p => p.Id == id);
            if (user == null)
                return View("Index", context.Users.Select(p => new UserVM().UserToUserVM(p)).ToList());

            if (string.IsNullOrWhiteSpace(dto.ConfirmPassword) || dto.Password != dto.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Password and Confirmed Password doesn't match");
                return View("Edit", dto);
            }

            dto.Password = Base64.Base64Encode(dto.ConfirmPassword);

            user.Username = dto.Username;
            user.Password = dto.Password;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
           
            context.Users.Update(user);
            context.SaveChanges();

            //return View("Index", context.Users.Select(p => new UserVM().UserToUserVM(p)).ToList());
            return RedirectToAction("Edit", new { id = id });
        }


        [HttpDelete]
        [Route("Delete/{id}")]
        public JsonResult Delete(int id)
        {
            var user = context.Users.FirstOrDefault(p => p.Id == id);
            if (user == null)
                return Json(new { success = true, message = "Already Deleted" });


            context.Users.Remove(user);
            context.SaveChanges();

            return Json(new { success = true, message = "Delete success" });
        }
    }
}
