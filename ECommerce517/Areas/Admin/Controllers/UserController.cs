using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerce517.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    [Authorize(Roles = $"{SD.SuperAdminRole},{SD.AdminRole}")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public IActionResult Index()
        {
            var users = _userManager.Users;
            return View(users.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(AdminUserCreate adminUserCreate)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(e => e.Errors.Select(e => e.ErrorMessage));

                TempData["error-notification"] = string.Join(", ",errors);
                return View(adminUserCreate);
            }

            ApplicationUser user =new ApplicationUser()
            {
                Name=adminUserCreate.Name,
                UserName=adminUserCreate.UserName,
                Email=adminUserCreate.Email,
                EmailConfirmed=adminUserCreate.ConfirmEmail
                
            };

            var result = await _userManager.CreateAsync(user,adminUserCreate.Password);

            // addRole

            if (!result.Succeeded)
            {
               var createErrors= result.Errors.Select(e => e.Description);
                TempData["error-notification"] = string.Join(", ", createErrors);
                return View(adminUserCreate);
            }


            //chcek for superAdmin=> 

            if((User.IsInRole(SD.AdminArea) || User.IsInRole(SD.CompanyRole)) && adminUserCreate.UserRole==SD.SuperAdminRole)
            {
                TempData["error-notification"] = "Only super admin can set Super admin ";

                return View(adminUserCreate);
            }

             var roleResult=   await _userManager.AddToRoleAsync(user,adminUserCreate.UserRole);




            if (!roleResult.Succeeded)
            {
                TempData["error-notification"] = "Failed To assign Role! ";
                return View(adminUserCreate);

            }

            

            TempData["success-notification"] = "User Created Successfully !";
            return RedirectToAction(nameof(Index));
        }
    }
}
