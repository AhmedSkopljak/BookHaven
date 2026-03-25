using BookHavenWebRazor_Temp.Data;
using BookHavenWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookHavenWebRazor_Temp.Pages.Categories;
[BindProperties]
public class Delete : PageModel
{
    private readonly ApplicationDbContext _db;
    
    public Category Category { get;set; }
    
    public Delete(ApplicationDbContext db)
    {
        _db = db;
    }
    public void OnGet(int? id)
    {
        if (id != null && id != 0)
        {
            Category =  _db.Categories.Find(id);
        }
    }

    public IActionResult OnPost()
    {
        _db.Categories.Remove(Category);
        _db.SaveChanges();
        TempData["success"] = "Category deleted successfully";
        return RedirectToPage("Index");
    }
}