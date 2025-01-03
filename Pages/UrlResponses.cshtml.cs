using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RealTimeRequestLogger.Data;
using RealTimeRequestLogger.Models;

namespace RealTimeRequestLogger.Pages;

public class UrlResponsesModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public UrlResponsesModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<UrlResponse> UrlResponses { get; set; } = new();

    [BindProperty]
    public UrlResponse NewUrlResponse { get; set; } = new();

    public async Task OnGetAsync()
    {
        UrlResponses = await _context.UrlResponses.OrderByDescending(u => u.UpdatedAt).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // 验证JSON格式
        try
        {
            System.Text.Json.JsonDocument.Parse(NewUrlResponse.JsonResponse);
        }
        catch (System.Text.Json.JsonException)
        {
            ModelState.AddModelError("NewUrlResponse.JsonResponse", "请输入有效的JSON格式");
            return Page();
        }

        NewUrlResponse.CreatedAt = DateTime.UtcNow;
        NewUrlResponse.UpdatedAt = DateTime.UtcNow;

        _context.UrlResponses.Add(NewUrlResponse);
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var urlResponse = await _context.UrlResponses.FindAsync(id);

        if (urlResponse != null)
        {
            _context.UrlResponses.Remove(urlResponse);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAsync([FromBody] UpdateUrlResponseModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var urlResponse = await _context.UrlResponses.FindAsync(model.Id);

        if (urlResponse == null)
        {
            return NotFound();
        }

        // 验证JSON格式
        try
        {
            System.Text.Json.JsonDocument.Parse(model.JsonResponse);
        }
        catch (System.Text.Json.JsonException)
        {
            return BadRequest("无效的JSON格式");
        }

        urlResponse.Path = model.Path;
        urlResponse.JsonResponse = model.JsonResponse;
        urlResponse.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return new JsonResult(new { success = true });
    }

    public class UpdateUrlResponseModel
    {
        public int Id { get; set; }
        public string Path { get; set; } = string.Empty;
        public string JsonResponse { get; set; } = string.Empty;
    }
}
