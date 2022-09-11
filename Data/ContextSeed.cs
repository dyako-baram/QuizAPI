using Microsoft.AspNetCore.Identity;
using QuizAPI.Models;
using System.Security.Claims;

namespace QuizAPI.Data
{
	public static class ContextSeed
	{
		public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
		{
			await roleManager.CreateAsync(new IdentityRole("Admin"));
			await roleManager.CreateAsync(new IdentityRole("User"));
		}
		public static async Task SeedAdminAsync(UserManager<QuizUser> userManager, RoleManager<IdentityRole> roleManager)
		{

			var defaultUser = new QuizUser
			{
				UserName = "admin",
				Email = "admin@gmail.com",
				EmailConfirmed = true,
				Age = new DateTime(1999,6,14).ToUniversalTime(),
				Country="Iraq"
			};
			if (userManager.Users.Select(x => new IdentityUser { Id = x.Id }).All(x => x.Id != defaultUser.Id))
			{
				var user = await userManager.FindByEmailAsync(defaultUser.Email);
				if (user == null)
				{
					await userManager.CreateAsync(defaultUser, "adminpassword");
					await userManager.AddToRoleAsync(defaultUser, "Admin");
					var userAdded = await userManager.FindByEmailAsync(defaultUser.Email);
					List<Claim> claims = new List<Claim>
					{
						new Claim("CanEdit","true"),
						new Claim("CanPost","true"),
						new Claim("CanDelete","true")
					};
					await userManager.AddClaimsAsync(userAdded, claims);
				}
			}
		}

	}
}
