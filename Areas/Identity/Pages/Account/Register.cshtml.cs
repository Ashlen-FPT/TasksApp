using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using TasksApp.Data;
using TasksApp.Models;

namespace TasksApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public RegisterModel(
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           ILogger<RegisterModel> logger,
           IEmailSender emailSender, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Organization")]
            public string Categories { get; set; }

            public string Role { get; set; }

            public IEnumerable<SelectListItem> RoleList { get; set; }
            public IEnumerable<SelectListItem> EntityList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            Input = new InputModel
            {
                RoleList = _roleManager.Roles.Where(u => u.Name != SD.Role_Operator).Select(x => x.Name).Select(i => new SelectListItem
                {

                    Text = i,
                    Value = i

                }),

                EntityList =_context.BEs.Select(x=>x.Categories).Select(y=> new SelectListItem
                {
                    Text=y,
                    Value=y
                })
            };
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Email = Input.Email,
                    Role = Input.Role,
                    Categories = Input.Categories
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");


                    if (!await _roleManager.RoleExistsAsync(SD.Role_Supervisor))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Supervisor));

                    }

                    if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));

                    }

                    if (!await _roleManager.RoleExistsAsync(SD.Role_Operator))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Operator));

                    }



                    //await _userManager.AddToRoleAsync(user, SD.Role_Supervisor);

                    if (user.Role == null)
                     {
                         await _userManager.AddToRoleAsync(user, SD.Role_Operator);
                     }
                     else
                     {

                         await _userManager.AddToRoleAsync(user, user.Role);
                     }
 

                  /*  var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
*/
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if (user.Role == null)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "User");
                        }
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
