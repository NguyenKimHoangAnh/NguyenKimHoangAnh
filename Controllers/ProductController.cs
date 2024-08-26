using NguyenKimHoangAnh.Context;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

public class ProductController : Controller
{
    private ASPEntities _context = new ASPEntities();

    public ActionResult Index()
    {
        var products = _context.Products.ToList();
        ViewBag.ItemCount = products.Count;
        return View(products);
    }



    public ActionResult ProductDetail(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return HttpNotFound();
        }
        return View(product);
    }

    public async Task<ActionResult> ProductsByCategory(int categoryId)
    {
        var products = await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();

        ViewBag.CategoryId = categoryId;
        return View(products);
    }
}
