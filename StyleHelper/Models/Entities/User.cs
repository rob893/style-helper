using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using RHerber.Common.Models;

namespace StyleHelper.Models.Entities;

public class User : IdentityUser<int>, IIdentifiable<int>
{
    [MaxLength(255)]
    public string? FirstName { get; set; }

    [MaxLength(255)]
    public string? LastName { get; set; }

    public DateTimeOffset Created { get; set; }

    public List<RefreshToken> RefreshTokens { get; set; } = new();

    public List<UserRole> UserRoles { get; set; } = new();

    public List<LinkedAccount> LinkedAccounts { get; set; } = new();
}
