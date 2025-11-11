using Alamana.Data.Entities;
using Alamana.Service.Authentication.Dtos;
using Alamana.Service.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Alamana.Service.Email;
using Microsoft.EntityFrameworkCore;
using Alamana.Data.Context;
using Alamana.Service.ConfirmationEmail;

namespace Alamana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {


        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly IJwtTokenService _jwtTokenService;
        //private readonly IEmailSender _emailSender;

        //public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IJwtTokenService jwtTokenService, IEmailSender emailSender)
        //{
        //    _userManager = userManager;
        //    _signInManager = signInManager;
        //    _roleManager = roleManager;
        //    _jwtTokenService = jwtTokenService;
        //    _emailSender = emailSender;
        //}












        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailSender _emailSender;
        private readonly AlamanaBbContext _dbContext;
        private readonly IConfirmationService _confirmationService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IJwtTokenService jwtTokenService, IEmailSender emailSender, AlamanaBbContext dbContext, IConfirmationService confirmationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtTokenService = jwtTokenService;
            _emailSender = emailSender;
            _dbContext = dbContext;
            _confirmationService = confirmationService;
        }












        //[HttpPost("userRegister")]
        //public async Task<IActionResult> UserRegister(registerDto registerDto)
        //{
        //    var email = registerDto.Email;
        //    if (email.Contains(' '))
        //    {
        //        return BadRequest(new {message = "لا ينبغي أن يحتوي تنسيق البريد الإلكتروني على أي مسافات." });
        //    }
        //    // تحقق من وجود @ في البريد الإلكتروني
        //    if (!email.Contains('@'))
        //    {
        //        return BadRequest(new { message = "صيغة البريد الإلكتروني غير صالحة." });
        //    }

        //    // استخراج الجزء الذي بعد الـ "@" للتأكد أنه مكتوب بالكامل بحروف صغيرة
        //    var domain = email.Split('@').Last();  // النطاق بعد "@"

        //    if (domain != domain.ToLower())  // إذا كان النطاق يحتوي على أحرف كبيرة
        //    {
        //        return BadRequest(new { message = "تنسيق البريد الإلكتروني غير صحيح. يجب أن يكون اسم النطاق بأحرف صغيرة." });
        //    }

        //    // التأكد من أن النطاق هو "@gmail.com" فقط
        //    if (!email.EndsWith("@gmail.com"))
        //    {
        //        return BadRequest(new { message = "يُسمح فقط بحسابات Gmail." });
        //    }

        //    var user = new ApplicationUser
        //    {
        //        UserName = registerDto.Email.Split('@')[0],
        //        Email = registerDto.Email,
        //        FullName = registerDto.FullName
        //    };

        //    var result = await _userManager.CreateAsync(user, registerDto.Password);

        //    if (result.Succeeded)
        //    {
        //        // تعيين دور (اختياري)
        //        var role = "User";
        //        if (!await _roleManager.RoleExistsAsync(role))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole(role));
        //        }
        //        await _userManager.AddToRoleAsync(user, role);

        //        // إنشاء التوكن
        //        var token = _jwtTokenService.GenerateJwtToken(user);

        //        return Ok(new
        //        {
        //            Message = "تم التسجيل بنجاح",
        //            Token = token,
        //            UserId = user.Id,
        //            FullName = user.FullName,
        //            Email = user.Email
        //        });    
        //    }
        //    if (result.Errors.Any(e => e.Code == "DuplicateUserName" || e.Code == "DuplicateEmail"))
        //    {
        //        return BadRequest(new { message = "هذا البريد الإلكتروني موجود بالفعل." });
        //    }
        //    //else if (result.Errors.Any(e => e.Code == "PasswordTooWeak"))
        //    //{
        //    //    return BadRequest("Password is too weak.");
        //    //}
        //    else if (result.Errors.Any(e => e.Code == "InvalidEmail"))
        //    {
        //        return BadRequest(new { message = "صيغة البريد الإلكتروني غير صالحة." });
        //    }
        //    else if (result.Errors.Any(e => e.Description.Contains("Password")))
        //    {
        //        return BadRequest(new { message = "(يجب أن تحتوي كلمة المرور على رقم واحد على الأقل (0 - 9" });
        //    }
        //    else
        //    {
        //        return BadRequest(new { Message = "خطا في تسجيل الدخول", Errors = result.Errors.Select(e => e.Description)});
        //    }

        //}










        [HttpPost("userRegister")]
        public async Task<IActionResult> UserRegister(registerDto dto)
        {
            //var email = dto.Email?.Trim();
            //if (string.IsNullOrWhiteSpace(email))
            //    return BadRequest(new { message = "Email is required.", messageAr = "البريد الإلكتروني مطلوب." });

            var email = dto.Email;
            if (email.Contains(' '))
            {
                return BadRequest(new { message = "لا ينبغي أن يحتوي تنسيق البريد الإلكتروني على أي مسافات." });
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
            // تحققاتك المختصرة هنا...

            var user = new ApplicationUser
            {
                UserName = email.Split('@')[0],
                Email = email,
                FullName = dto.FullName,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                // نفس معالجة الأخطاء اللي عندك...
                return BadRequest(new { message = "Registration error.", errors = result.Errors.Select(e => e.Description) });
            }

            // إضافة الدور
            const string role = "User";
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
            await _userManager.AddToRoleAsync(user, role);

            // ابعتي إيميل تأكيد للتسجيل
            await _confirmationService.SendOrRenewConfirmationAsync(user.Id, user.Email!, "register");

            return Ok(new
            {
                success = true,
                needEmailConfirmation = true,
                message = "Registered successfully. Please confirm your email from the link sent to your inbox.",
                messageAr = "تم التسجيل بنجاح. من فضلك أكّدي بريدك الإلكتروني من الرسالة المرسلة."
            });
        }







        //[Authorize]
        //[HttpGet("getUserId")]
        //public async Task<IActionResult> GetUserId()
        //{
        //    // الحصول على التوكن من الهيدر
        //    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        //    if (string.IsNullOrEmpty(token))
        //    {
        //        return Unauthorized(new { Message = "Token is missing" });
        //    }

        //    // فك تشفير التوكن لاستخراج الـ Claims
        //    var handler = new JwtSecurityTokenHandler();
        //    var jwtToken = handler.ReadJwtToken(token);

        //    // استخراج الـ email من التوكن
        //    var email = jwtToken?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return Unauthorized(new { Message = "Email not found in token" });
        //    }

        //    // البحث عن المستخدم باستخدام الـ email
        //    var user = await _userManager.FindByEmailAsync(email);

        //    if (user == null)
        //    {
        //        return Unauthorized(new { Message = "User not found" });
        //    }

        //    return Ok(new { UserId = user.Id });
        //}





        ////[Authorize]
        //[HttpGet("getUserDetails/{userId}")]
        //public async Task<IActionResult> GetUserDetails(string userId)
        //{
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return BadRequest(new { Message = "User ID is required" });
        //    }

        //    // البحث عن المستخدم باستخدام الـ userId
        //    var user = await _userManager.FindByIdAsync(userId);

        //    if (user == null)
        //    {
        //        return NotFound(new { Message = "User not found" });
        //    }

        //    // إرجاع بيانات المستخدم
        //    return Ok(new
        //    {
        //        Id = user.Id,
        //        Name = user.UserName, // أو `FullName` حسب قاعدة بياناتك
        //        Email = user.Email
        //    });
        //}












        //[Authorize]
        //[HttpGet("getFullName")]
        //public async Task<IActionResult> GetFullName()
        //{
        //    // الحصول على التوكن من الهيدر
        //    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        //    if (string.IsNullOrEmpty(token))
        //    {
        //        return Unauthorized(new { Message = "Token is missing" });
        //    }

        //    // فك تشفير التوكن لاستخراج الـ Claims
        //    var handler = new JwtSecurityTokenHandler();
        //    var jwtToken = handler.ReadJwtToken(token);

        //    // استخراج الـ FullName من التوكن
        //    var fullName = jwtToken?.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;

        //    if (string.IsNullOrEmpty(fullName))
        //    {
        //        return Unauthorized(new { Message = "FullName not found in token" });
        //    }

        //    return Ok(new { FullName = fullName });
        //}






        //[HttpPost("login")]
        //public async Task<IActionResult> Login(loginDto loginDto)
        //{
        //    var user = await _userManager.FindByEmailAsync(loginDto.Email);
        //    if (user == null)
        //    {
        //        return Unauthorized(new { Message = "البريد الإلكتروني أو كلمة المرور غير صالحة" });
        //    }

        //    var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);

        //    if (result.Succeeded)
        //    {
        //        // إنشاء التوكن
        //        var token = _jwtTokenService.GenerateJwtToken(user);

        //        return Ok(new
        //        {
        //            Message = "تم تسجيل الدخول بنجاح",
        //            Token = token,
        //            UserId = user.Id,
        //            FullName = user.FullName,
        //            Email = user.Email
        //        });
        //    }

        //    return Unauthorized(new { Message = "البريد الإلكتروني أو كلمة المرور غير صالحة" });
        //}









        //[Authorize]
        //[HttpGet("secure-data")]
        //public IActionResult GetSecureData()
        //{
        //    return Ok(new { message = "This is secured data" });
        //}










        //[HttpPost("changePassword")]
        //public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        //{
        //    var user = await _userManager.FindByEmailAsync(changePasswordDto.Email);
        //    if (user == null)
        //    {
        //        return BadRequest(new { Message = "User not found." });
        //    }

        //    // التحقق من كلمة المرور القديمة باستخدام CheckPasswordSignInAsync
        //    var passwordValid = await _signInManager.CheckPasswordSignInAsync(user, changePasswordDto.OldPassword, lockoutOnFailure: false);

        //    if (!passwordValid.Succeeded)
        //    {
        //        return BadRequest(new { Message = "Old password is incorrect." });
        //    }

        //    // إذا كانت كلمة المرور القديمة صحيحة، نتابع مع تغيير كلمة المرور الجديدة
        //    var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

        //    if (result.Succeeded)
        //    {
        //        return Ok(new { Message = "Password changed successfully." });
        //    }

        //    return BadRequest(new { Message = "Failed to change password", Errors = result.Errors.Select(e => e.Description) });
        //}












        //// مسار ForgotPassword
        //[HttpPost("forgotPassword")]
        //public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        //{
        //    var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
        //    if (user == null)
        //    {
        //        return BadRequest("User not found.");
        //    }

        //    // توليد الرمز (Token)
        //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        //    // توليد الرابط الذي يحتوي على الـ Token
        //    //var resetLink = $"{Request.Scheme}://{Request.Host}/Auth/ResetPassword?token={token}&email={user.Email}";
        //    //var frontendUrl = "https://elsaeid-tea.netlify.app"; // رابط الفرونت إند
        //    var frontendUrl = "http://localhost:4200"; // رابط الفرونت إند
        //    //var resetLink = $"{frontendUrl}/resetPassword?token={token}&email={user.Email}";
        //    var resetLink = $"{frontendUrl}/resetPassword?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";


        //    // إرسال البريد الإلكتروني
        //    await _emailSender.SendEmailAsync(user.Email, "Password Reset", $"Click on this link to reset your password: {resetLink}");

        //    return Ok(new { Message = "Password reset link has been sent." });
        //}















        //// مسار ResetPassword
        //[HttpPost("resetPassword")]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        //{
        //    var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        //    if (user == null)
        //    {
        //        return BadRequest("User not found.");
        //    }

        //    // إعادة تعيين كلمة المرور باستخدام الرمز
        //    var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
        //    if (result.Succeeded)
        //    {
        //        return Ok(new { Message = "Password reset successfully." });
        //    }

        //    return BadRequest("Failed to reset password.");
        //}
































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
            var email = jwtToken?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { Message = "Email not found in token" });
            }

            // البحث عن المستخدم باستخدام الـ email
            var user = await _userManager.FindByIdAsync(email);

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
                Email = user.Email,
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






        //[HttpPost("login")]
        //public async Task<IActionResult> Login(loginDto loginDto)
        //{
        //    var user = await _userManager.FindByEmailAsync(loginDto.Email);
        //    if (user == null)
        //    {
        //        return Unauthorized(new {
        //            message = "Invalid email or password.",
        //            messageAr = "البريد الإلكتروني أو كلمة المرور غير صحيحة."
        //        });
        //    }

        //    var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);

        //    if (result.Succeeded)
        //    {
        //        // إنشاء التوكن
        //        var token = _jwtTokenService.GenerateJwtToken(user, "User");

        //        return Ok(new
        //        {
        //            Message = "Login Successfully",
        //            Token = token,
        //            UserId = user.Id,
        //            FullName = user.FullName,
        //            Email = user.Email
        //        });
        //    }

        //    return Unauthorized(new {
        //        message = "Invalid email or password.",
        //        messageAr = "البريد الإلكتروني أو كلمة المرور غير صحيحة."
        //    });
        //}




        [HttpPost("SendConfirmationEmail")]
        public async Task<IActionResult> SendConfirmationEmail(loginDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return NotFound(new { message = "User not found.", message_ar = "المستخدم غير موجود." });

            await _confirmationService.SendOrRenewConfirmationAsync(user.Id, user.Email!, "login");
            return Ok(new { success = true, message = "Confirmation email sent." });
        }





        [HttpGet("confirmEmail/{confirmationId}")]
        public async Task<IActionResult> ConfirmEmail(Guid confirmationId)
        {
            var confirmation = await _dbContext.EmailConfirmationRequests
                .FirstOrDefaultAsync(c => c.Id == confirmationId);

            if (confirmation == null)
                return NotFound(new { message = "Confirmation request not found.", message_ar = "طلب التأكيد غير موجود." });

            if (confirmation.IsConfirmed != null)
                return BadRequest(new { message = "Confirmation link has already been used.", message_ar = "تم استخدام رابط التأكيد بالفعل." });

            if (DateTime.UtcNow > confirmation.ExpiresAt)
                return BadRequest(new { message = "Confirmation link has expired.", message_ar = "رابط التأكيد منتهي الصلاحية." });

            var user = await _userManager.FindByIdAsync(confirmation.UserId);
            if (user == null)
                return NotFound(new { message = "User not found.", message_ar = "المستخدم غير موجود." });

            user.EmailConfirmed = true;
            confirmation.IsConfirmed = true;

            await _userManager.UpdateAsync(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Email confirmed successfully.", message_ar = "تم تأكيد البريد الإلكتروني بنجاح." });
        }







        [HttpGet("rejectEmail/{confirmationId}")]
        public async Task<IActionResult> RejectEmail(Guid confirmationId)
        {
            var confirmation = await _dbContext.EmailConfirmationRequests
                .FirstOrDefaultAsync(c => c.Id == confirmationId);

            if (confirmation == null)
                return NotFound(new { message = "Confirmation request not found.", message_ar = "طلب التأكيد غير موجود." });

            // صلاحية بسيطة (ساعة + دقيقتين زي ما تحبي)
            if (DateTime.UtcNow > confirmation.ExpiresAt.AddMinutes(2))
            {
                _dbContext.EmailConfirmationRequests.Remove(confirmation);
                await _dbContext.SaveChangesAsync();
                return BadRequest(new { message = "Confirmation link is invalid or expired.", message_ar = "رابط التأكيد غير صالح أو منتهي الصلاحية." });
            }

            // علّميه كـ rejected وامسحيه لو حابة (الاتنين صح)
            confirmation.IsConfirmed = false;
            _dbContext.EmailConfirmationRequests.Remove(confirmation);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "The process was ignored based on your selection.", message_ar = "تم تجاهل العملية بناءً على اختيارك." });
        }






        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound(new { message = "User not found." });

            if (user.EmailConfirmed)
                return BadRequest(new { message = "Email already confirmed." });

            // منع السبام: لو فيه طلب لسه صالح، بلاش تبعتي جديد
            var hasActive = await _dbContext.EmailConfirmationRequests
                .AnyAsync(x => x.UserId == user.Id && x.IsConfirmed == null && x.ExpiresAt > DateTime.UtcNow);
            if (hasActive)
                return Ok(new { message = "A confirmation link is already active. Please check your inbox." });

            await _confirmationService.SendOrRenewConfirmationAsync(user.Id, user.Email!, "register");
            return Ok(new { message = "Confirmation email re-sent." });
        }












        [HttpPost("login")]
        public async Task<IActionResult> Login(loginDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Unauthorized(new { message = "Invalid Email Or Password.", messageAr = "البريد الالكتروني او كلمه المرور غير صحيحه" });


            var passOk = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!passOk)
                return Unauthorized(new { message = "Invalid Email Or Password.", messageAr = "البريد الالكتروني او كلمه المرور غير صحيحه" });

            if (passOk)
            {
                var isUser = await _userManager.IsInRoleAsync(user, "User");
                if (!isUser)
                {
                    return Unauthorized(new { Message = "You do not have permission to access as an user." });
                }
            }

            if (!user.EmailConfirmed)
            {
                // ابعتي/جددي لينك التأكيد (كل محاولة Login هتبعت له)
                await _confirmationService.SendOrRenewConfirmationAsync(user.Id, user.Email!, "login");

                return Ok(new
                {
                    success = false,
                    needEmailConfirmation = true,
                    message = "Your account is not verified. A confirmation message has been sent to your email.",
                    messageAr = "حسابك غير مؤكد. تم إرسال رسالة تأكيد إلى بريدك الإلكتروني."
                });
            }

            // لو Confirmed → اصدر JWT وادخليه
            //var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "User";
            var token = _jwtTokenService.GenerateJwtToken(user, "User");

            return Ok(new
            {
                success = true,
                token,
                userId = user.Id,
                fullName = user.FullName,
                email = user.Email
            });
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
                return BadRequest(new
                {
                    message = "User not found.",
                    messageAr = "المستخدم غير موجود."
                });
            }

            // التحقق من كلمة المرور القديمة باستخدام CheckPasswordSignInAsync
            var passwordValid = await _signInManager.CheckPasswordSignInAsync(user, changePasswordDto.OldPassword, lockoutOnFailure: false);

            if (!passwordValid.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Old password is incorrect.",
                    messageAr = "كلمة المرور القديمة غير صحيحة."
                });
            }

            // إذا كانت كلمة المرور القديمة صحيحة، نتابع مع تغيير كلمة المرور الجديدة
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    message = "Password changed successfully.",
                    messageAr = "تم تغيير كلمة المرور بنجاح."
                });
            }

            return BadRequest(new
            {
                message = "Failed to change password.",
                messageAr = "فشل في تغيير كلمة المرور.",
                errors = result.Errors.Select(e => e.Description)
            });
        }






        // مسار ForgotPassword
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return BadRequest(new
                {
                    message = "User not found.",
                    messageAr = "المستخدم غير موجود."
                });
            }

            // توليد الرمز (Token)
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // توليد الرابط الذي يحتوي على الـ Token
            //var resetLink = $"{Request.Scheme}://{Request.Host}/Auth/ResetPassword?token={token}&email={user.Email}";
            //var frontendUrl = "https://elsaeid-tea.netlify.app"; // رابط الفرونت إند
            var frontendUrl = "https://bubblehope.com"; // رابط الفرونت إند
            //var resetLink = $"{frontendUrl}/resetPassword?token={token}&email={user.Email}";
            var resetLink = $"{frontendUrl}/auth/resetPassword?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";


            // إرسال البريد الإلكتروني
            await _emailSender.SendEmailAsync(user.Email, "Password Reset", $"Click on this link to reset your password: {resetLink}");

            return Ok(new
            {
                message = "Password reset link has been sent.",
                messageAr = "تم إرسال رابط إعادة تعيين كلمة المرور."
            });
        }








        //// مسار ResetPassword
        //[HttpPost("resetPassword")]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        //{
        //    var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        //    if (user == null)
        //    {
        //        return BadRequest("User not found.");
        //    }

        //    // إعادة تعيين كلمة المرور باستخدام الرمز
        //    var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
        //    if (result.Succeeded)
        //    {
        //        return Ok(new { Message = "Password reset successfully." });
        //    }

        //    return BadRequest("Failed to reset password.");
        //}



        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return BadRequest(new
                {
                    message = "User not found.",
                    messageAr = "المستخدم غير موجود."
                });
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    message = "Password has been reset successfully.",
                    messageAr = "تم إعادة تعيين كلمة المرور بنجاح."
                });
            }

            // ✅ تحليل الأخطاء لعرض رسائل واضحة مثل التسجيل
            if (result.Errors.Any(e => e.Description.Contains("Password")))
            {
                return BadRequest(new
                {
                    message = "Invalid password. It must contain at least one uppercase letter, one number, and one special character.",
                    messageAr = "كلمة المرور غير صالحة. يجب أن تحتوي على حرف كبير واحد على الأقل، ورقم، وحرف خاص."
                });
            }

            else if (result.Errors.Any(e => e.Code == "InvalidToken"))
            {
                return BadRequest(new
                {
                    message = "The reset token is invalid or has expired.",
                    messageAr = "رمز إعادة التعيين غير صالح أو انتهت صلاحيته."
                });
            }

            return BadRequest(new
            {
                message = "Failed to reset the password.",
                messageAr = "فشل في إعادة تعيين كلمة المرور.",
                errors = result.Errors.Select(e => e.Description)
            });
        }









    }
}
