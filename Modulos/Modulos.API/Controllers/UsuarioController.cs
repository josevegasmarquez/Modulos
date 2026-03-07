using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Modulos.API.Models;
using Modulos.API.ViewModels;

namespace Modulos.API.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public UsuarioController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var model = new UserProfileViewModel
            {
                Nombres = user.Nombres,
                Apellidos = user.Apellidos,
                Email = user.Email,
                Telefono = user.Telefono,
                Direccion = user.Direccion,
                FechaNacimiento = user.FechaNacimiento,
                Genero = user.Genero,
                FechaRegistro = user.FechaRegistro
            };
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var model = new UserProfileViewModel 
            { 
                Id = user.Id,
                Nombres = user.Nombres,
                Apellidos = user.Apellidos,
                FechaNacimiento = user.FechaNacimiento,
                Email = user.Email,
                Genero = user.Genero,
                Telefono = user.Telefono,
                Direccion = user.Direccion
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
               return View(model);
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var emailChange = user.Email != model.Email;
            user.Nombres = model.Nombres;
            user.Apellidos = model.Apellidos;
            user.Telefono = model.Telefono;
            user.Direccion = model.Direccion;
            user.FechaNacimiento = model.FechaNacimiento;
            user.Genero = model.Genero;
            if (emailChange)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("Email", "El email ya está en uso por otro usuario.");
                    return View(model);
                }
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    AddErrors(setEmailResult);
                    return View(model);
                }
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.Email);
                if (!setUserNameResult.Succeeded)
                {
                    AddErrors(setUserNameResult);
                    return View(model);
                }
            }
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if (emailChange)
                {
                    await _signInManager.RefreshSignInAsync(user);
                }
                TempData["SuccessMessage"] = "Perfil actualizado exitosamente.";
                return RedirectToAction(nameof(Perfil));
            }
            AddErrors(result);
            return View(model);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
