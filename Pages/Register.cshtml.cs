using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WordGuessGame.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "กรุณากรอกชื่อผู้ใช้")]
            [StringLength(100, ErrorMessage = "ชื่อผู้ใช้ต้องมีความยาวระหว่าง 3 ถึง 100 ตัวอักษร", MinimumLength = 3)]
            [Display(Name = "ชื่อผู้ใช้")]
            public string Username { get; set; }

            [Required(ErrorMessage = "กรุณากรอกอีเมล")]
            [EmailAddress(ErrorMessage = "รูปแบบอีเมลไม่ถูกต้อง")]
            [Display(Name = "อีเมล")]
            public string Email { get; set; }

            [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
            [StringLength(100, ErrorMessage = "รหัสผ่านต้องมีความยาวอย่างน้อย 6 ตัวอักษร", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "รหัสผ่าน")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "ยืนยันรหัสผ่าน")]
            [Compare("Password", ErrorMessage = "รหัสผ่านและการยืนยันรหัสผ่านไม่ตรงกัน")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Username, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Auto sign-in the user after successful registration
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User signed in automatically after registration.");

                    // Redirect to home page or return URL
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
