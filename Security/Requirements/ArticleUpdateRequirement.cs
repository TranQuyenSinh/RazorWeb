using Microsoft.AspNetCore.Authorization;

namespace App.Security.Requirement
{
    // Quá hạn ngày: 5/5/2023 thì không cho cập nhật
    public class ArticleUpdateRequirement : IAuthorizationRequirement
    {
        // thông tin kiểm tra user
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public ArticleUpdateRequirement(int year = 2023, int month = 5, int day = 5)
        {
            Year = year;
            Month = month;
            Day = day;
        }
    }
}