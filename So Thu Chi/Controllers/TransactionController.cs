using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using So_Thu_Chi.Models;
using Syncfusion.EJ2.Schedule;

namespace So_Thu_Chi.Controllers;

public class TransactionController : Controller
{
    private readonly ApplicationDbContext _context;

    public TransactionController(ApplicationDbContext context)
    {
        _context = context;
    }
 
    // Get
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Transactions.Include(t => t.Category);
        return View(await applicationDbContext.ToListAsync());
    }

    //Get Transaction/AddOrEdit
    public IActionResult AddOrEdit(int id = 0)
    {
        PopulateCategory();
        return id == 0 ? View(new Transaction()) : View( _context.Transactions.Find(id));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOrEdit(
        [Bind("TransactionId, CategoryId,Amount, Note, Date")]
        Transaction transaction)
    {
        if (ModelState.IsValid)
        {
            if (transaction.TransactionId == 0)
            {
                _context.Transactions.Add(transaction);
            }
            else
            {
                _context.Update(transaction);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopulateCategory();
        return View(transaction);
    }

    // POST: Transactions/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Transactions == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
        }

        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction != null)
        {
            _context.Transactions.Remove(transaction);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    
    [NonAction]
    public void PopulateCategory()
    {
        var categoryCollection = _context.Categories.ToList();
        Category defaultCategory = new Category() { CategoryId = 0, Title = "Choose" };
        categoryCollection.Insert(0, defaultCategory);
        ViewBag.Categories = categoryCollection;
    }
}