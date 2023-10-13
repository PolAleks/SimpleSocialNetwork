using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleSocialNetwork.DAL.Entity;
using System.Threading.Tasks;
using SimpleSocialNetwork.BLL.ViewModels.Account;
using SimpleSocialNetwork.BLL.Extentions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace SimpleSocialNetwork.Controllers
{
    public class AccountManagerController : Controller
    {
        private IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountManagerController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Home/Login");
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        /// <summary>
        /// Метод аутентификации
        /// </summary>
        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("MyPage", "AccountManager");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Выход из аккаунта
        /// </summary>
        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Страница с информацией о пользователи
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("PersonalPage")]
        public async Task<IActionResult> MyPage()
        {
            var user = User;

            var result = _userManager.GetUserAsync(user);

            return View("User", new UserViewModel(result.Result));
        }

        /// <summary>
        /// Обновление информации о пользователе
        /// </summary>
        [Authorize]
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                user.Convert(model);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("MyPage", "AccountManager");
                }
                else
                {
                    return RedirectToAction("Edit", "AccountManager");
                }
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
                return View("Edit", model);
            }
        }

        /// <summary>
        /// Метод для редактирования данных пользователя
        /// </summary>
        [Route("Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            // Получаем удостоверения ClaimsPrincipal текущего пользователя
            ClaimsPrincipal claimsCurrentUser = User;

            // Вытягиваем из БД пользователя 
            Task<User> user = _userManager.GetUserAsync(claimsCurrentUser);

            return View("Edit", _mapper.Map<UserEditViewModel>(user.Result));
        }

        /// <summary>
        /// Метод для вывода результата поиска пользователей
        /// </summary>
        [Route("UserList")]
        [HttpPost]
        public IActionResult UserList(string search)
        {
            var model = new SearchViewModel
            {
                UserList = _userManager.Users.AsEnumerable().Where(u => u.GetFullName().Contains(search, StringComparison.CurrentCultureIgnoreCase)).ToList()
            };
            return View("UserList", model);
        }

    }
}
