using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerce517.Areas.Identity.Controllers
{
    [Area(SD.IdentityArea)]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            UpdatePersonalInfoVM updateduser = user.Adapt<UpdatePersonalInfoVM>();

            return View(updateduser);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateInfo(UpdatePersonalInfoVM personalInfoVM)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(e => e.Errors.Select(e => e.ErrorMessage));
                TempData["error-notification"] = string.Join(" , ", errors);
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(personalInfoVM.Id);

            if (user is null)
            {
                return RedirectToAction("NotFound", "Home", new { area = "identity" });
            }

            user.Email = personalInfoVM.Email;
            user.Name = personalInfoVM.Name;
            user.PhoneNumber = personalInfoVM.PhoneNumber;
            user.Street = personalInfoVM.Street;
            user.City = personalInfoVM.City;
            user.State = personalInfoVM.State;
            user.ZipCode = personalInfoVM.ZipCode;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> ChangePhoto(string Id, IFormFile Photo)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user is null)
            {
                return NotFound();
            }

            if (Photo is not null && Photo.Length > 0)
            {
                var oldpath = "";

                if (user.ProfileImage is not null)
                {
                    oldpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\profile", user.ProfileImage);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Photo.FileName);
                var pathName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\profile", fileName);
                using (var stream = System.IO.File.Create(pathName))
                {
                    Photo.CopyTo(stream);
                }
                user.ProfileImage = fileName;
                await _userManager.UpdateAsync(user);

                //remove old photo ==> using the old path

                if (System.IO.File.Exists(oldpath))
                {
                    System.IO.File.Delete(oldpath);
                }



                return RedirectToAction(nameof(Index));

            }
            TempData["error-notification"] = "Failed to change profile image";
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> ChangePassword(string Id, string CurrentPassword, string Password)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(e => e.Errors.Select(e => e.ErrorMessage));
                TempData["error-notification"] = string.Join(" , ", errors);
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                TempData["error-notification"] = "Error with send data";
                return RedirectToAction(nameof(Index));
            }

          var result = await _userManager.ChangePasswordAsync(user, CurrentPassword, Password);
            if (!result.Succeeded)
            {
                TempData["error-notification"] = "Error with change password";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.UpdateAsync(user);

            TempData["success-notification"] = "Password changed successfully";
            return RedirectToAction(nameof(Index));
        }

    }
}
