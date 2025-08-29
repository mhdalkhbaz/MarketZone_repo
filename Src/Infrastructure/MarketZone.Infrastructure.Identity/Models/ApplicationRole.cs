using System;
using Microsoft.AspNetCore.Identity;

namespace MarketZone.Infrastructure.Identity.Models
{
    public class ApplicationRole(string name) : IdentityRole<Guid>(name)
    {
    }
}
