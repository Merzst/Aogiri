using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;

namespace Aogiri.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public IndexModel(ApplicationDbContext db) { _db = db; }

    public List<Advertisement> Ads           { get; set; } = new();
    public List<Category>      Categories    { get; set; } = new();
    public List<Subcategory>   Subcategories { get; set; } = new();
    public List<Location>      Locations     { get; set; } = new();

    public string?  Query         { get; set; }
    public int?     CategoryId    { get; set; }
    public int?     SubcategoryId { get; set; }
    public string?  City          { get; set; }
    public decimal? MinPrice      { get; set; }
    public decimal? MaxPrice      { get; set; }
    public string   Sort          { get; set; } = "newest";
    public string?  Condition     { get; set; }
    public string?  DealType      { get; set; }
    public int      Page          { get; set; } = 1;
    public int      TotalPages    { get; set; }
    public int      TotalCount    { get; set; }
    private const int PageSize = 12;

    public async Task OnGetAsync(
        string? q, int? categoryId, int? subcategoryId,
        string? city, decimal? minPrice, decimal? maxPrice,
        string sort = "newest", int page = 1,
        string? condition = null, string? dealType = null)
    {
        Query         = q;
        CategoryId    = categoryId;
        SubcategoryId = subcategoryId;
        City          = city;
        MinPrice      = minPrice;
        MaxPrice      = maxPrice;
        Sort          = sort;
        Page          = page;
        Condition     = condition;
        DealType      = dealType;

        Categories    = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        Subcategories = await _db.Subcategories
            .Where(s => categoryId == null || s.CategoryID == categoryId)
            .OrderBy(s => s.Name).ToListAsync();
        Locations     = await _db.Locations.OrderBy(l => l.City).ToListAsync();

        var query = _db.Advertisements
            .Include(a => a.Category)
            .Include(a => a.Subcategory)
            .Include(a => a.Location)
            .Include(a => a.User)
            .Where(a => a.Status == "Active");

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(a => a.Title.Contains(q) || (a.Description != null && a.Description.Contains(q)));

        if (categoryId.HasValue)    query = query.Where(a => a.CategoryID == categoryId);
        if (subcategoryId.HasValue) query = query.Where(a => a.SubcategoryID == subcategoryId);
        if (!string.IsNullOrEmpty(city)) query = query.Where(a => a.Location.City == city);
        if (minPrice.HasValue)      query = query.Where(a => a.Price >= minPrice);
        if (maxPrice.HasValue)      query = query.Where(a => a.Price <= maxPrice);
        if (!string.IsNullOrEmpty(condition))  query = query.Where(a => a.Condition == condition);
        if (!string.IsNullOrEmpty(dealType))   query = query.Where(a => a.DealType  == dealType);

        query = sort switch
        {
            "price_asc"  => query.OrderBy(a => a.Price),
            "price_desc" => query.OrderByDescending(a => a.Price),
            "popular"    => query.OrderByDescending(a => a.ViewCount),
            _            => query.OrderByDescending(a => a.PublishedDate)
        };

        TotalCount = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
        Ads        = await query.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();
    }
}
