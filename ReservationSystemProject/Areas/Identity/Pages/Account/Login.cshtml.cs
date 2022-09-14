using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ReservationSystemProject.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            _logger.LogInformation("User clicked on the Login link.");
            _logger.LogInformation("Entering Login page with OnGetAsync() method.");
            if (!string.IsNullOrEmpty(ErrorMessage))
            {

                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            _logger.LogInformation("User clicked on the Log in button."); 
            _logger.LogInformation("Entering Login page with OnPostAsync() method.");
            returnUrl = returnUrl ?? Url.Content("~/");
            Trace.Listeners.Add(new TextWriterTraceListener("IdentityDebug.log"));
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                Debug.WriteLine("Checking user validity");
                Debug.Assert(result.Succeeded is true, "User is invalid");
                _logger.LogInformation("Checking the User login information in the database.");
                if (result.Succeeded)
                {
                    Debug.WriteLine("User is valid");
                    _logger.LogInformation("User logged in.");
                    _logger.LogInformation("Login success!");

                    if (returnUrl.ToString() != "/")
                    {
                        Debug.WriteLine("Redirecting to existing target destination");
                        Trace.Close();
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        _logger.LogInformation("Exit Login page. Attempting to RedirectUser().");
                        Debug.WriteLine("Redirecting user to relevant area");
                        Trace.Close();
                        return RedirectToAction("RedirectUser", "Home", new { area = "" });
                    }

                }
                if (result.RequiresTwoFactor)
                {
                    Trace.Close();
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    Trace.Close();
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    //wrong password
                    _logger.LogError("Error in the Login page. Login failed!");
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    Debug.WriteLine("Login error displayed");
                    Trace.Close();
                    return Page();
                }
            }
            Debug.WriteLine("Page model invalid");
            Trace.Close();
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
