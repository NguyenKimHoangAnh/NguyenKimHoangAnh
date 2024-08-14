using NguyenKimHoangAnh.Context;
using Microsoft.Ajax.Utilities;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

public class CategoriesController : Controller
{
    private ASPEntities1 _context = new ASPEntities1();
    public ActionResult Index()
    {
        var categories = _context.Categories.ToList();
        return View(categories);
    }

    public ActionResult Index1()
    {
        var listCat = _context.Products
            .Select(p => p.Category)
            .Distinct()
            .ToList();
        return View(listCat);
    }

    public ActionResult Details(int id)
    {
        var category = _context.Categories.Find(id);
        if (category == null)
        {
            return HttpNotFound();
        }
        return View(category);
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("Categories1");
        }
        return View(category);
    }

    public ActionResult Edit(int id)
    {
        var category = _context.Categories.Find(id);
        if (category == null)
        {
            return HttpNotFound();
        }
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Entry(category).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("Categories1");
        }
        return View(category);
    }

    public ActionResult Delete(int id)
    {
        var category = _context.Categories.Find(id);
        if (category == null)
        {
            return HttpNotFound();
        }
        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        var category = _context.Categories.Find(id);
        _context.Categories.Remove(category);
        _context.SaveChanges();
        return RedirectToAction("Categories1");
    }

    public ActionResult ProductsByCategory(int categoryId)
    {
        var products = _context.Products
            .Where(p => p.CategoryId == categoryId)
            .ToList();

        if (!products.Any())
        {
            return HttpNotFound("No products found for this category.");
        }

        ViewBag.CategoryId = categoryId; // Optional: if you want to use it in the view
        return View(products);
    }

}
