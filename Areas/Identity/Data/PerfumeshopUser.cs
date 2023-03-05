using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Perfumeshop.Areas.Identity.Data;

// Add profile data for application users by adding properties to the CandleShopUser class
public class PerfumeshopUser : IdentityUser
{
    [PersonalData]
    public int user_ID  { get; set; }
}

