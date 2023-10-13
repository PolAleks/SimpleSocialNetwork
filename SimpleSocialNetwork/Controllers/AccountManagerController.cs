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
using SimpleSocialNetwork.DAL.Repository;
using System.Collections.Generic;
using SimpleSocialNetwork.DAL.UoW;

namespace SimpleSocialNetwork.Controllers
{
    public class AccountManagerController : Controller
    {
        private IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public AccountManagerController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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

            var result = await _userManager.GetUserAsync(user);

            var model = new UserViewModel(result);

            model.Friends = GetAllFriend(model.User);

            return View("User", model);
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
        public IActionResult Edit()
        {
            // Получаем удостоверения ClaimsPrincipal текущего пользователя
            ClaimsPrincipal claimsCurrentUser = User;

            // Вытягиваем из БД пользователя 
            var user = _userManager.GetUserAsync(claimsCurrentUser);

            var model = _mapper.Map<UserEditViewModel>(user.Result);

            return View("Edit", model);

        }

        /// <summary>
        /// Метод для вывода результата поиска пользователей
        /// </summary>
        [Route("UserList")]
        [HttpPost]
        public async Task<IActionResult> UserList(string search)
        {
            SearchViewModel model = await CreateSearch(search);

            return View("UserList", model);
        }

        private async Task<SearchViewModel> CreateSearch(string search)
        {
            // Считываем ClaimsPrincipal ассоциированного с этим методом 
            var currentUser = User;

            // На осноавании ClaimPrincipal получаем пользователя
            User user = await _userManager.GetUserAsync(currentUser);

            // Получаем список пользователей на основании запроса из поисковой строки
            List<User> listUserSearch = _userManager.Users.AsEnumerable().Where(u => u.GetFullName().Contains(search, StringComparison.CurrentCultureIgnoreCase)).ToList();

            // Получаем список друзей текущего пользователя
            var withFriend = GetAllFriend(user);

            // Инициализируем список пользователей с друзьями
            var data = new List<UserWithFriendExt>();

            // Для каждого пользователя из списка запроса поисковой строки
            listUserSearch.ForEach(x =>
            {
                // Получаем модель представление UserWithFriendExt - на базе User
                UserWithFriendExt t = _mapper.Map<UserWithFriendExt>(x);
                // Ставим флаг друг текущему пользователю или нет
                t.IsFriendWithCurrent = withFriend.Where(y => y.Id == x.Id || x.Id == user.Id).Count() != 0;

                data.Add(t);
            });

            var model = new SearchViewModel()
            {
                UserList = data
            };

            return model;
        }

        private List<User> GetAllFriend(User user)
        {
            //var user = User;

            //var result = await _userManager.GetUserAsync(user);

            // Получаем репозиторий с друзьями
            FriendsRepository repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(user);
        }

    }
}
