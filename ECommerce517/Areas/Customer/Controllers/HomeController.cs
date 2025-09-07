using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommerce517.Models;
using ECommerce517.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ECommerce517.Areas.Customer.Controllers;

[Area(SD.CustomerArea)]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private ApplicationDbContext _context = new();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(ProductFilterVM productFilterVM, int page = 1)
    {
        const double discount = 50;
        var products = _context.Products.Include(e => e.Category).AsQueryable();

        // Filter
        if (productFilterVM.ProductName is not null)
        {
            products = products.Where(e => e.Name.Contains(productFilterVM.ProductName));
            ViewBag.ProductName = productFilterVM.ProductName;
        }

        if (productFilterVM.MinPrice is not null)
        {
            products = products.Where(e => e.Price - e.Price * (e.Discount / 100) >= productFilterVM.MinPrice);
            ViewBag.MinPrice = productFilterVM.MinPrice;
        }

        if (productFilterVM.MaxPrice is not null)
        {
            products = products.Where(e => e.Price - e.Price * (e.Discount / 100) <= productFilterVM.MaxPrice);
            ViewBag.MaxPrice = productFilterVM.MaxPrice;
        }

        if (productFilterVM.CategoryId is not null)
        {
            products = products.Where(e => e.CategoryId == productFilterVM.CategoryId);
            ViewBag.CategoryId = productFilterVM.CategoryId;
        }

        if (productFilterVM.IsHot)
        {
            products = products.Where(e => e.Discount > discount);
            ViewBag.IsHot = productFilterVM.IsHot;
        }

        // Pagination
        double totalPages = Math.Ceiling(products.Count() / 8.0); // 3.1 => 4
        int currentPage = page;

        ViewBag.TotalPages = totalPages;
        ViewBag.CurrentPage = currentPage;

        products = products.Skip((page -1) * 8).Take(8);

        // Returned Data
        var categories = _context.Categories.ToList();
        ViewBag.Categories = categories;
        //ViewData["Categories"] = categories;

        return View(products.ToList());
    }

    public IActionResult Details([FromRoute] int id)
    {
        var product = _context.Products.Include(e => e.Category).FirstOrDefault(e => e.Id == id);

        if (product is null)
            return NotFound();

        // Update Traffic
        ++product.Traffic;
        _context.SaveChanges();

        // Related Products
        var relatedProducts = _context.Products.Include(e=>e.Category).Where(e => e.CategoryId == product.CategoryId && e.Id != product.Id).Skip(0).Take(4);

        // Top Traffic
        var topTraffic = _context.Products.Include(e => e.Category).OrderByDescending(e => e.Traffic).Where(e => e.Id != product.Id).Skip(0).Take(4);

        // Similar Products
        var similarProducts = _context.Products.Include(e => e.Category).Where(e => e.Name.Contains(product.Name) && e.Id != product.Id).Skip(0).Take(4);

        // Return Data
        ProductWithRelatedVM productWithRelatedVM = new()
        {
            Product = product,
            RelatedProducts = relatedProducts.ToList(),
            TopTraffic = topTraffic.ToList(),
            SimilarProducts = similarProducts.ToList()
        };

        return View(productWithRelatedVM);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult NotFound()
    {
        return View();
    }
    public ViewResult Welcome()
    {
        return View();
    }

    public ViewResult PersonalInfo()
    {
        string name = "Mohamed";
        int age = 27;
        double balance = 2000;

        List<string> skills = new()
        {
            "C",
            "C++",
            "C#"
        };

        PersonalInfoVM personalInfoVM = new PersonalInfoVM()
        {
            Name = name,
            Age = age,
            Balance = balance,
            Skills = skills
        };

        return View(model: personalInfoVM);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
