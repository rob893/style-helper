using Microsoft.AspNetCore.Identity;
using RHerber.Common.Models;

namespace StyleHelper.Models.Entities;

public class Role : IdentityRole<int>, IIdentifiable<int>
{
    public List<UserRole> UserRoles { get; set; } = new();
}
