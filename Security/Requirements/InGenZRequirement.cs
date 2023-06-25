using Microsoft.AspNetCore.Authorization;

namespace App.Security.Requirement
{   
    // 1997 => 2012
    public class InGenZRequirement :IAuthorizationRequirement
    {
        // thông tin kiểm tra user
        public int FromYear { get; set; }
        public int ToYear { get; set; }

        public InGenZRequirement(int fromYear = 1997, int toYear = 2012) {
            FromYear = fromYear;
            ToYear = toYear;
        }
    }
}