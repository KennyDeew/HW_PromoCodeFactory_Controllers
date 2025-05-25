using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Request
{
    public class CreateEmployeeRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }
}
