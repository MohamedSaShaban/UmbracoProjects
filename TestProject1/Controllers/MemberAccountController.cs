using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Cms.Web.Common.Security;
using TestProject1.Models;


namespace TestProject1.Controllers
{
    public class MemberAccountController : SurfaceController
    {
        private readonly IMemberManager _memberManager;
        private readonly IMemberService _memberService;
        private readonly IMemberSignInManager _signInManager;

        public MemberAccountController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            IMemberManager memberManager,
            IMemberService memberService
,
            IMemberSignInManager signInManager) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _memberManager = memberManager;
            _memberService = memberService;
            _signInManager = signInManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string Name, string Email, string Username, string Password, bool HasBankAccount)
        {
            var existingMember = await _memberManager.FindByNameAsync(Username);
            if (existingMember != null)
            {
                TempData["RegisterMessage"] = "⚠️ اسم المستخدم موجود بالفعل.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var newMember = new MemberIdentityUser
            {
                UserName = Username,
                Email = Email,
                Name = Name
            };

            var result = await _memberManager.CreateAsync(newMember, Password);

            if (result.Succeeded)
            {
                var member = _memberService.GetByUsername(Username);
                if (member != null)
                {
                    member.SetValue("hasBankAccount", HasBankAccount);
                    _memberService.Save(member);
                }

                TempData["RegisterMessage"] = "✅ تم التسجيل بنجاح. يمكنك الآن تسجيل الدخول.";
                return Redirect("/login");
            }
            else
            {
                TempData["RegisterMessage"] = "❌ حدث خطأ أثناء التسجيل: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            var result = await _signInManager.PasswordSignInAsync(Username, Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                TempData["Message"] = "✅ تم تسجيل الدخول بنجاح!";
                return Redirect("https://localhost:44334");
            }
            else
            {
                TempData["Message"] = "❌ فشل تسجيل الدخول. تأكد من البيانات.";
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("https://localhost:44334");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var member = await _memberManager.FindByEmailAsync(email);
            if (member == null)
            {
                TempData["Message"] = "البريد الإلكتروني غير مسجل.";
                return RedirectToAction("ForgotPassword");
            }

            var token = await _memberManager.GeneratePasswordResetTokenAsync(member);
            //var resetLink = Url.Action("ResetPassword", "MemberAccount", new { userId = member.Id, token = token }, Request.Scheme);

            return RedirectToAction("ResetPassword", new { userId = member.Id, token = token});
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["Message"] = "كلمتا المرور غير متطابقتين.";
                return View(model);
            }

            var member = await _memberManager.FindByIdAsync(model.UserId);
            if (member == null)
            {
                TempData["Message"] = "المستخدم غير موجود.";
                return View(model);
            }

            var result = await _memberManager.ResetPasswordAsync(member, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["Message"] = "تمت إعادة تعيين كلمة المرور بنجاح. يمكنك الآن تسجيل الدخول.";
                return Redirect("/login");
            }

            TempData["Message"] = "حدث خطأ أثناء إعادة التعيين.";
            return View(model);
        }

    }

}
