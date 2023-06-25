using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Models;

namespace App.Security.Requirement
{
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        private readonly ILogger<AppAuthorizationHandler> _logger;
        private readonly UserManager<AppUser> _userManager;

        public AppAuthorizationHandler(ILogger<AppAuthorizationHandler> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            // context.PendingRequirements => Requirement chưa kiểm tra
            // context.User 
            // context.Resource => mặc định là HttpContext
            var requirements = context.PendingRequirements.ToList();

            foreach (var requiment in requirements)
            {
                if (requiment is InGenZRequirement)
                {
                    // code kiểm tra user đảm bảo requirement InGenZRequirement
                    if (IsGenZ(context.User, (InGenZRequirement)requiment))
                    {
                        context.Succeed(requiment);
                    }
                    // context.Succeed(requiment);
                }

                if (requiment is ArticleUpdateRequirement)
                {
                    if (CanUpdateArticle(context.User, context.Resource, (ArticleUpdateRequirement)requiment))
                    {
                        context.Succeed(requiment);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private bool CanUpdateArticle(ClaimsPrincipal user, object value, ArticleUpdateRequirement requiment)
        {
            if (user.IsInRole("Admin"))
            {
                _logger.LogInformation("Admin cập nhật Article");
                return true;
            }

            var article = value as Article;
            var dateCanUpdate = new DateTime(requiment.Year, requiment.Month, requiment.Day);
            if (article.Created < dateCanUpdate)
            {
                _logger.LogInformation("Quá hạn ngày sửa đổi");
                return false;
            }
            return true;
        }

        private bool IsGenZ(ClaimsPrincipal user, InGenZRequirement requiment)
        {
            // lấy user từ db
            var appuserTask = _userManager.GetUserAsync(user);
            Task.WaitAll(appuserTask);
            var appUser = appuserTask.Result;

            if (appUser.BirthDate == null)
            {
                _logger.LogInformation($"{appUser.UserName} không có ngày sinh, không thõa mãn GenZRequirement");
                return false;
            }

            int? year = appUser.BirthDate.Value.Year;

            var success = (year >= requiment.FromYear && year <= requiment.ToYear);
            if (success)
            {
                _logger.LogInformation($"{appUser.UserName} thõa mãn GenZRequirement");
            }
            else
            {
                _logger.LogInformation($"{appUser.UserName} không thõa mãn GenZRequirement");
            }
            return success;
        }
    }
}