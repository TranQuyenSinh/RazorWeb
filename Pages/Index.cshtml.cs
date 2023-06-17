using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace razorweb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly MyBlogContext myBlogContext;
    public IndexModel(ILogger<IndexModel> logger, MyBlogContext _myBlogContext)
    {
        _logger = logger;
        myBlogContext = _myBlogContext;
    }

    public void OnGet()
    {
        var posts = (from a in myBlogContext.articles
                     orderby a.Created descending
                     select a).ToList();
        ViewData["posts"] = posts;
    }
}
