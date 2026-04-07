using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;

public static class SeedData
{
    public static async Task CriarAdmin(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        string email = "admin@teste.com"; // depois colocar via env
        string senha = "admin123";

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
        }

        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new Usuario
            {
                UserName = email,
                Email = email,
                Nome = "Administrador",
                Cpf = "00000000000",
                DataNascimento = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, senha);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(" | ", result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, "Admin"))
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}