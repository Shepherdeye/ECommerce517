using ECommerce517.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce517.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    [Authorize(Roles = $"{SD.SuperAdminRole},{SD.AdminRole}")]
    public class CategoryController : Controller
    {
        //private ApplicationDbContext _context = new();
        private IRepository<Category> _categoryRepository ;


        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAsync();

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if(!ModelState.IsValid)
            {
                //var errors = ModelState.Values.SelectMany(e => e.Errors);
                //TempData["error-notification"] = String.Join(", ", errors.Select(e=>e.ErrorMessage));

                return View(category);
            }

            await _categoryRepository.CreateAsync(category);
            await _categoryRepository.CommitAsync();

            TempData["success-notification"] = "Add Category Successfully";
            Response.Cookies.Append("success-notification", "Add Category Successfully", new()
            {
                Secure = true,
                Expires = DateTime.Now.AddDays(1)
            });

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetOneAsync(e => e.Id == id);

            if (category is null)
                return RedirectToAction(SD.NotFoundPage, SD.HomeController);

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if(!ModelState.IsValid)
            {
                return View(category);
            }

            _categoryRepository.Update(category);
            await _categoryRepository.CommitAsync();

            TempData["success-notification"] = "Update Category Successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetOneAsync(e => e.Id == id);

            if(category is null)
                return RedirectToAction(SD.NotFoundPage, SD.HomeController);

            _categoryRepository.Delete(category);
            await _categoryRepository.CommitAsync();

            TempData["success-notification"] = "Delete Category Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
