using BookHaven.DataAccess.Data;
using BookHaven.DataAccess.Repository.IRepository;
using BookHaven.Models;

namespace BookHaven.DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
    
    public void Update(Product obj)
    {
        var objFromDb = _db.Products.FirstOrDefault(u=>u.Id == obj.Id);
        if (objFromDb != null)
        {
            objFromDb.Title = obj.Title;
            objFromDb.Description = obj.Description;
            objFromDb.Price = obj.Price;
            objFromDb.ISBN = obj.ISBN;
            objFromDb.Author = obj.Author;
            objFromDb.CategoryId = obj.CategoryId;
            objFromDb.Price50 = obj.Price50;
            objFromDb.Price100 = obj.Price100;
            if (obj.ImageUrl != null)
            {
                objFromDb.ImageUrl = obj.ImageUrl;
            }
        }
    }
}