using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations; // Added for DataAnnotations

namespace Modulos.Client.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            var client = _httpClientFactory.CreateClient("ModulosAPI");
            var content = new StringContent(JsonSerializer.Serialize(new { email, password }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Auth/login", content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result?.User != null && result.Success)
                {
                    await SignInUser(result, rememberMe);

                    if (result.User.MustChangePassword)
                    {
                        return RedirectToAction("ChangePassword");
                    }

                    return RedirectToAction("Profile");
                }
            }

            ModelState.AddModelError("", "Credenciales inválidas.");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userJson = HttpContext.Session.GetString("UserData");
            if (string.IsNullOrEmpty(userJson)) return RedirectToAction("Login");

            var user = JsonSerializer.Deserialize<UserDto>(userJson);
            return View(user);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var userJson = HttpContext.Session.GetString("UserData");
            if (string.IsNullOrEmpty(userJson)) return RedirectToAction("Login");

            var user = JsonSerializer.Deserialize<UserDto>(userJson);
            
            // Map string gender back to numeric string for the select value
            if (user != null)
            {
                user.Genero = user.Genero switch
                {
                    "Masculino" => "0",
                    "Femenino" => "1",
                    "Otro" => "2",
                    "PrefieroNoDecirlo" => "3",
                    "Prefiero no decirlo" => "3",
                    _ => "3"
                };
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(UserDto model)
        {
            var client = _httpClientFactory.CreateClient("ModulosAPI");
            var token = HttpContext.Session.GetString("JWTToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Map UserDto back to what the API expects (UpdateProfileRequest)
            var updateRequest = new
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Genero = int.Parse(model.Genero), // Assuming the API expects the int value of the enum
                PhoneNumber = model.PhoneNumber,
                Address = model.Address
            };

            var content = new StringContent(JsonSerializer.Serialize(updateRequest), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("Auth/profile", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result != null && result.Success)
                {
                    // Update session data
                    HttpContext.Session.SetString("UserData", JsonSerializer.Serialize(result.User));
                    return RedirectToAction("Profile");
                }
            }

            ModelState.AddModelError("", "Error al actualizar el perfil.");
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Las contraseñas no coinciden.");
                return View();
            }

            var client = _httpClientFactory.CreateClient("ModulosAPI");
            var token = HttpContext.Session.GetString("JWTToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(new { currentPassword, newPassword, confirmPassword }), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("Auth/change-password", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Error al cambiar la contraseña.");
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("ModulosAPI");
            var token = HttpContext.Session.GetString("JWTToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("Auth/register", content);

            // Intentar deserializar la respuesta siempre para obtener el mensaje informativo
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, options);

            if (response.IsSuccessStatusCode && authResponse?.Success == true)
            {
                ViewBag.Message = authResponse.Message ?? "Usuario registrado correctamente.";
                return View();
            }

            if (authResponse != null && !string.IsNullOrEmpty(authResponse.Message))
            {
                // Mapear errores comunes a campos específicos
                if (authResponse.Message.Contains("Password", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Password", authResponse.Message);
                }
                else if (authResponse.Message.Contains("Email", StringComparison.OrdinalIgnoreCase) || authResponse.Message.Contains("ya está registrado", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Email", authResponse.Message);
                }
                else if (authResponse.Message.Contains("DNI", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("DNI", authResponse.Message);
                }
                else
                {
                    ModelState.AddModelError("", authResponse.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", $"Error inesperado del servidor: {response.StatusCode}");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Users()
        {
            var client = _httpClientFactory.CreateClient("ModulosAPI");
            var token = HttpContext.Session.GetString("JWTToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("Auth/users");
            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
                return View(users);
            }

            return View(new List<UserDto>());
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UserProfile(string id)
        {
            var client = _httpClientFactory.CreateClient("ModulosAPI");
            var token = HttpContext.Session.GetString("JWTToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"Auth/user/{id}");
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();
                ViewBag.IsAdminViewingOther = true;
                return View("Profile", user);
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var client = _httpClientFactory.CreateClient("ModulosAPI");
            var token = HttpContext.Session.GetString("JWTToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"Auth/reset-password/{id}", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                TempData["SuccessMessage"] = result?.Message;
            }
            else
            {
                TempData["ErrorMessage"] = "Error al reiniciar la contraseña.";
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private async Task SignInUser(AuthResponse auth, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, auth.User?.FullName ?? "Unknown"),
                new Claim(ClaimTypes.Email, auth.User?.Email ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, auth.User?.Id ?? string.Empty),
                new Claim("JWTToken", auth.Token ?? string.Empty)
            };

            if (auth.User?.Roles != null)
            {
                foreach (var role in auth.User.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            // Store some data in session for easy access
            if (auth.Token != null) HttpContext.Session.SetString("JWTToken", auth.Token);
            if (auth.User != null) HttpContext.Session.SetString("UserData", JsonSerializer.Serialize(auth.User));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : null
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public UserDto? User { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Genero { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool MustChangePassword { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(50, ErrorMessage = "Los nombres no pueden exceder los 50 caracteres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(50, ErrorMessage = "Los apellidos no pueden exceder los 50 caracteres")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener exactamente 8 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El DNI debe contener solo números")]
        public string DNI { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El género es obligatorio")]
        public int Genero { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Direccion { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Role { get; set; } = string.Empty;
    }
}
