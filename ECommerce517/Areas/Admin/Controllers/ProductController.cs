using ECommerce517.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;

namespace ECommerce517.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    [Authorize(Roles = $"{SD.SuperAdminRole},{SD.AdminRole}")]
    public class ProductController : Controller
    {
        ApplicationDbContext _context = new();

        IProductRepository _ProductRepositry;//= new ProductRepositry();
        IRepository<Category> _CategoryRepositry ;//= new Repository<Category>();
        IRepository<Brand> _BrandRepositry ;//= new Repository<Brand>();

        public ProductController(IProductRepository ProductRepositry, IRepository<Category> CategoryRepositry, IRepository<Brand> BrandRepositry)
        {
            _ProductRepositry = ProductRepositry;
            _CategoryRepositry = CategoryRepositry;
            _BrandRepositry = BrandRepositry;

        }
        public async Task<IActionResult> Index()
        {

            //var products = _context.Products.Include(e => e.Category)/*.OrderBy(e=>e.Quantity)*/;
            var products = await _ProductRepositry.GetAsync(includes: [e => e.Category]);

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _CategoryRepositry.GetAsync();

            var brands = await _BrandRepositry.GetAsync();

            CategoryWithBrandVM categoryWithBrandVM = new()
            {
                Categories = categories,
                Brands = brands
            };

            return View(categoryWithBrandVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile MainImg)
        {
            if (MainImg is not null && MainImg.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(MainImg.FileName);
                // 0924fdsfs-d429-fskdf-jsd230-423.png

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                // Save Img in wwwroot
                using (var stream = System.IO.File.Create(filePath))
                {
                    MainImg.CopyTo(stream);
                }

                // Sava img name in DB
                product.MainImg = fileName;

                // Save in DB
                await _ProductRepositry.CreateAsync(product);
                await _ProductRepositry.CommitAsync();

                return RedirectToAction(nameof(Index));
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _ProductRepositry.GetOneAsync(e => e.Id == id);

            if (product is null)
                return RedirectToAction(SD.NotFoundPage, SD.HomeController);


            var categories = await _CategoryRepositry.GetAsync();

            var brands = await _BrandRepositry.GetAsync();


            CategoryWithBrandVM categoryWithBrandVM = new()
            {
                Categories = categories,
                Brands = brands,
                Product = product
            };

            return View(categoryWithBrandVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile? MainImg)
        {
            //var productInDB = _context.Products.AsNoTracking().FirstOrDefault(e => e.Id == product.Id);
            var productInDB = await _ProductRepositry.GetOneAsync(e => e.Id == product.Id, tracked: false);

            if (productInDB is null)
                return BadRequest();

            if (MainImg is not null && MainImg.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(MainImg.FileName);
                // 0924fdsfs-d429-fskdf-jsd230-423.png

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                // Save Img in wwwroot
                using (var stream = System.IO.File.Create(filePath))
                {
                    MainImg.CopyTo(stream);
                }

                // Delete old img from wwwroot
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", productInDB.MainImg);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Update img name in DB
                product.MainImg = fileName;
            }
            else
            {
                product.MainImg = productInDB.MainImg;
            }

            // Update in DB
            _ProductRepositry.Update(product);
            await _ProductRepositry.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(e => e.Id == id);

            if (product is null)
                return RedirectToAction(SD.NotFoundPage, SD.HomeController);

            // Delete old img from wwwroot
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", product.MainImg);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            // Remove in DB
            _ProductRepositry.Delete(product);
            await _ProductRepositry.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
