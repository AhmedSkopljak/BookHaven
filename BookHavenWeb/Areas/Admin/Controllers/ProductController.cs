using BookHaven.DataAccess.Repository.IRepository;
using BookHaven.Models;
using BookHaven.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookHavenWeb.Areas.Admin.Controllers;


[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    
    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
        return View(objProductList);
    }
    public IActionResult Upsert(int? id)
    {
        ProductVM productVm = new()
        {
            CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            Product = new Product()
        };
        if (id == null || id == 0)
        {
            //create
            return View(productVm);
        }
        else
        {
            //update
            productVm.Product = _unitOfWork.Product.Get(u => u.Id == id);
            return View(productVm);
        }
    }

    [HttpPost]
    public IActionResult Upsert(ProductVM productVm, IFormFile? file)
    {
        string wwwRootPath = _webHostEnvironment.WebRootPath;
        if (file != null)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string productPath =  Path.Combine(wwwRootPath, @"images\product");

            using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            
            productVm.Product.ImageUrl = @"images\product\"+fileName;
        }
        if (ModelState.IsValid)
        {
            _unitOfWork.Product.Add(productVm.Product);
            _unitOfWork.Save();
            TempData["success"] = "Product created successfully";
            return RedirectToAction("Index");
        }else
        {
            productVm.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(productVm); 
        }
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
        if (productFromDb == null)
        {
            return NotFound();
        }
        return View(productFromDb);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int? id)
    {
        Product? obj=_unitOfWork.Product.Get(u => u.Id == id);
        if (obj == null)
        {
            return NotFound();
        }
        _unitOfWork.Product.Remove(obj);
        _unitOfWork.Save();
        TempData["success"] = "Product deleted successfully";
        return RedirectToAction("Index");
    }
}