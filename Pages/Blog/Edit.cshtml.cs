using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;

namespace razorweb.Pages_Blog
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IAuthorizationService _authorizationService;

        public EditModel(AppDbContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        [BindProperty]
        public Article Article { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.articles == null)
            {
                return NotFound();
            }

            var article = await _context.articles.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            Article = article;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            /**
                gắn Article vào _context
                => Article sẽ tương ứng vs 1 bài viết trên csdl
                sau đó thiết lập trạng thái của nó là bị sửa đổi để ef so sánh và cập nhật sửa đổi
            */

            try
            {
                // kiểm tra chính sách CanUpdateArticle
                var result = await _authorizationService.AuthorizeAsync(User, Article, "CanUpdateArticle");
                if (result.Succeeded)
                {
                    _context.Attach(Article).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Quá hạn sửa đổi bài viết");
                    return Page();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(Article.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ArticleExists(int id)
        {
            return (_context.articles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
