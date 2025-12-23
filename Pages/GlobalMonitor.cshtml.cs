using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RealTimeRequestLogger.Pages
{
    public class GlobalMonitorModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Title"] = "全局请求监听";
        }
    }
}
