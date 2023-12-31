﻿using AutoMapper;
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
        private readonly IMapper _mapper;
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

            model.Friends = await GetAllFriend(model.User);

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
        public async Task<IActionResult> Edit()
        {
            // Получаем набор утверждений ClaimsPrincipal текущего пользователя
            ClaimsPrincipal claimsCurrentUser = User;

            // Ищем пользователя по набору утверждений Claims в БД 
            var user = await _userManager.GetUserAsync(claimsCurrentUser);

            return View("Edit", _mapper.Map<UserEditViewModel>(user));
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

        /// <summary>
        /// Метод для поиска пользователей по поисковому запросу
        /// </summary>
        /// <returns>Возвращает SearchViewModel</returns>
        private async Task<SearchViewModel> CreateSearch(string search)
        {
            // Считываем утверждения ClaimsPrincipal для пользователя ассоциированного с этим методом 
            var currentUser = User;

            // На осноавании ClaimPrincipal получаем пользователя
            User user = await _userManager.GetUserAsync(currentUser);

            List<User> listUserSearch = _userManager.Users.ToList();

            if (!string.IsNullOrEmpty(search))
            {
                // Получаем список пользователей на основании запроса из поисковой строки
                listUserSearch = _userManager.Users.AsEnumerable().Where(u => u.GetFullName().Contains(search, StringComparison.CurrentCultureIgnoreCase)).ToList();
            } 
            // Получаем список друзей текущего пользователя
            var withFriend = await GetAllFriend();

            // Инициализируем предстваление для отображения друзей
            var data = new List<UserWithFriendExt>();

            // Для каждого пользователя из списка запроса поисковой строки
            listUserSearch.ForEach(searchUser =>
            {
                // Получаем модель представление UserWithFriendExt - на базе User
                UserWithFriendExt userWithFriend = _mapper.Map<UserWithFriendExt>(searchUser);
                // Ставим флаг друг текущему пользователю или нет
                userWithFriend.IsFriendWithCurrent = withFriend.Where(y => y.Id == searchUser.Id || searchUser.Id == user.Id).Any();

                data.Add(userWithFriend);
            });

            var model = new SearchViewModel()
            {
                UserList = data
            };

            return model;
        }

        /// <summary>
        /// Метод для получения списка друзей
        /// </summary>
        private async Task<List<User>> GetAllFriend(User user)
        {
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(user);
        }

        private async Task<List<User>> GetAllFriend()
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(result);
        }

        /// <summary>
        /// Метод добавления друзей
        /// </summary>
        /// <param name="id">Идентификатор друга</param>
        [HttpPost]
        [Route("AddFriend")]
        public async Task<IActionResult> AddFriend(string id)
        {
            var currentClaimsUser = User;

            var currentUser = await _userManager.GetUserAsync(currentClaimsUser);

            var friend = await _userManager.FindByIdAsync(id);

            FriendsRepository repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            await repository.AddFriend(currentUser, friend);

            return RedirectToAction("MyPage", "AccountManager");
        }

        /// <summary>
        /// Метод удаления из друзей
        /// </summary>
        [HttpPost]
        [Route("DeleteFriend")]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            var currentClaimsUser = User;

            var currentUser = await _userManager.GetUserAsync(currentClaimsUser);

            var friend = await _userManager.FindByIdAsync(id);

            FriendsRepository repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            await repository.DeleteFriend(currentUser, friend);

            return RedirectToAction("MyPage", "AccountManager");
        }

        /// <summary>
        /// Метод отображения чата
        /// </summary>
        [Route("Chat")]
        [HttpPost]
        public async Task<IActionResult> Chat(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var mess = repository.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };
            return View("Chat", model);
        }

        private async Task<ChatViewModel> GenerateChat(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var mess = repository.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };

            return model;
        }

        [Route("Chat")]
        [HttpGet]
        public async Task<IActionResult> Chat()
        {

            var id = Request.Query["id"];

            var model = await GenerateChat(id);
            return View("Chat", model);
        }


        /// <summary>
        /// Метод отправки сообщения
        /// </summary>
        [Route("NewMessage")]
        [HttpPost]
        public async Task<IActionResult> NewMessage(string id, ChatViewModel chat)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var item = new Message()
            {
                Sender = result,
                Recipient = friend,
                Text = chat.NewMessage.Text,
            };
            await repository.Create(item);

            var mess = repository.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };
            return View("Chat", model);
        }
    }
}
