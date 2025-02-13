using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using dit230680c_AS.Data;
using dit230680c_AS.Models;

public class PasswordHistoryValidator<TUser> : IPasswordValidator<TUser> where TUser : IdentityUser
{
    private readonly AppDbContext _context;

    public PasswordHistoryValidator(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
    {
        var passwordHistory = await _context.PasswordHistories
            .Where(ph => ph.UserId == user.Id)
            .OrderByDescending(ph => ph.CreatedAt)
            .Take(2) // Last 2 passwords
            .ToListAsync();

        foreach (var pastPassword in passwordHistory)
        {
            if (manager.PasswordHasher.VerifyHashedPassword(user, pastPassword.HashedPassword, password) == PasswordVerificationResult.Success)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "You cannot reuse your last 2 passwords."
                });
            }
        }

        return IdentityResult.Success;
    }
}
