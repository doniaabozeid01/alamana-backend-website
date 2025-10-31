using Alamana.Data.Entities;
using Alamana.Service.Authentication.Dtos;
using Alamana.Service.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Alamana.Service.Email;

namespace Alamana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IJwtTokenService jwtTokenService, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtTokenService = jwtTokenService;
            _emailSender = emailSender;
        }

        [HttpPost("userRegister")]
        public async Task<IActionResult> UserRegister(registerDto registerDto)
        {
            var email = registerDto.Email;
            if (email.Contains(' '))
            {
                return BadRequest(new {message = "لا ينبغي أن يحتوي تنسيق البريد الإلكتروني على أي مسافات." });
                }
            // تحقق من وجود @ في البريد الإلكتروني
            if (!email.Contains('@'))
            {
                return BadRequest(new { message = "صيغة البريد الإلكتروني غير صالحة." });
            }

            // استخراج الجزء الذي بعد الـ "@" للتأكد أنه مكتوب بالكامل بحروف صغيرة
            var domain = email.Split('@').Last();  // النطاق بعد "@"

            if (domain != domain.ToLower())  // إذا كان النطاق يحتوي على أحرف كبيرة
            {
                return BadRequest(new { message = "تنسيق البريد الإلكتروني غير صحيح. يجب أن يكون اسم النطاق بأحرف صغيرة." });
            }

            // التأكد من أن النطاق هو "@gmail.com" فقط
            if (!email.EndsWith("@gmail.com"))
            {
                return BadRequest(new { message = "يُسمح فقط بحسابات Gmail." });
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Email.Split('@')[0],
                Email = registerDto.Email,
                FullName = registerDto.FullName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                // تعيين دور (اختياري)
                var role = "User";
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                await _userManager.AddToRoleAsync(user, role);

                // إنشاء التوكن
                var token = _jwtTokenService.GenerateJwtToken(user);

                return Ok(new
                {
                    Message = "تم التسجيل بنجاح",
                    Token = token,
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email
                });    
            }
            if (result.Errors.Any(e => e.Code == "DuplicateUserName" || e.Code == "DuplicateEmail"))
            {
                return BadRequest(new { message = "هذا البريد الإلكتروني موجود بالفعل." });
            }
            //else if (result.Errors.Any(e => e.Code == "PasswordTooWeak"))
            //{
            //    return BadRequest("Password is too weak.");
            //}
            else if (result.Errors.Any(e => e.Code == "InvalidEmail"))
            {
                return BadRequest(new { message = "صيغة البريد الإلكتروني غير صالحة." });
            }
            else if (result.Errors.Any(e => e.Description.Contains("Password")))
            {
                return BadRequest(new { message = "(يجب أن تحتوي كلمة المرور على رقم واحد على الأقل (0 - 9" });
            }
            else
            {
                return BadRequest(new { Message = "خطا في تسجيل الدخول", Errors = result.Errors.Select(e => e.Description)});
            }

        }

             
        [Authorize]
        [HttpGet("getUserId")]
        public async Task<IActionResult> GetUserId()
        {
            // الحصول على التوكن من الهيدر
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { Message = "Token is missing" });
            }

            // فك تشفير التوكن لاستخراج الـ Claims
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // استخراج الـ email من التوكن
            var email = jwtToken?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { Message = "Email not found in token" });
            }

            // البحث عن المستخدم باستخدام الـ email
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Unauthorized(new { Message = "User not found" });
            }

            return Ok(new { UserId = user.Id });
        }





        //[Authorize]
        [HttpGet("getUserDetails/{userId}")]
        public async Task<IActionResult> GetUserDetails(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Message = "User ID is required" });
            }

            // البحث عن المستخدم باستخدام الـ userId
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            // إرجاع بيانات المستخدم
            return Ok(new
            {
                Id = user.Id,
                Name = user.UserName, // أو `FullName` حسب قاعدة بياناتك
                Email = user.Email
            });
        }












        [Authorize]
        [HttpGet("getFullName")]
        public async Task<IActionResult> GetFullName()
        {
            // الحصول على التوكن من الهيدر
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { Message = "Token is missing" });
            }

            // فك تشفير التوكن لاستخراج الـ Claims
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // استخراج الـ FullName من التوكن
            var fullName = jwtToken?.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;

            if (string.IsNullOrEmpty(fullName))
            {
                return Unauthorized(new { Message = "FullName not found in token" });
            }

            return Ok(new { FullName = fullName });
        }






        [HttpPost("login")]
        public async Task<IActionResult> Login(loginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "البريد الإلكتروني أو كلمة المرور غير صالحة" });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // إنشاء التوكن
                var token = _jwtTokenService.GenerateJwtToken(user);

                return Ok(new
                {
                    Message = "تم تسجيل الدخول بنجاح",
                    Token = token,
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email
                });
            }

            return Unauthorized(new { Message = "البريد الإلكتروني أو كلمة المرور غير صالحة" });
        }









        [Authorize]
        [HttpGet("secure-data")]
        public IActionResult GetSecureData()
        {
            return Ok(new { message = "This is secured data" });
        }










        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(changePasswordDto.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found." });
            }

            // التحقق من كلمة المرور القديمة باستخدام CheckPasswordSignInAsync
            var passwordValid = await _signInManager.CheckPasswordSignInAsync(user, changePasswordDto.OldPassword, lockoutOnFailure: false);

            if (!passwordValid.Succeeded)
            {
                return BadRequest(new { Message = "Old password is incorrect." });
            }

            // إذا كانت كلمة المرور القديمة صحيحة، نتابع مع تغيير كلمة المرور الجديدة
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Password changed successfully." });
            }

            return BadRequest(new { Message = "Failed to change password", Errors = result.Errors.Select(e => e.Description) });
        }












        // مسار ForgotPassword
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // توليد الرمز (Token)
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // توليد الرابط الذي يحتوي على الـ Token
            //var resetLink = $"{Request.Scheme}://{Request.Host}/Auth/ResetPassword?token={token}&email={user.Email}";
            //var frontendUrl = "https://elsaeid-tea.netlify.app"; // رابط الفرونت إند
            var frontendUrl = "http://localhost:4200"; // رابط الفرونت إند
            //var resetLink = $"{frontendUrl}/resetPassword?token={token}&email={user.Email}";
            var resetLink = $"{frontendUrl}/resetPassword?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";


            // إرسال البريد الإلكتروني
            await _emailSender.SendEmailAsync(user.Email, "Password Reset", $"Click on this link to reset your password: {resetLink}");

            return Ok(new { Message = "Password reset link has been sent." });
        }















        // مسار ResetPassword
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // إعادة تعيين كلمة المرور باستخدام الرمز
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Password reset successfully." });
            }

            return BadRequest("Failed to reset password.");
        }





    }
}
