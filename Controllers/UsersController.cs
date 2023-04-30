using CoolEvents.Data;
using CoolEvents.Models;
using CoolEvents.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static CoolEvents.Areas.Identity.Pages.Account.RegisterModel;

namespace CoolEvents.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IUserStore<AppUser> _userStore;

        public UsersController(ApplicationDbContext db, UserManager<AppUser> userManager, IUserStore<AppUser> userStore)
        {
            _userManager = userManager;
            _db = db;
            _userStore = userStore;
        }

        public async Task<IActionResult> Index()
        {
            List<AppUser> users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            AppUser userToEdit = await _userManager.FindByIdAsync(id);
            EditUserViewModel userData = new EditUserViewModel();
            userData.FirstName = userToEdit.FirstName;
            userData.LastName = userToEdit.LastName;
            userData.UserName = userToEdit.UserName;
            userData.UserId = userToEdit.Id;

            return View(userData);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel userNewData)
        {
            var user = await _userManager.FindByIdAsync(userNewData.UserId);

            try
            {
                if (user.FirstName != userNewData.FirstName)
                {
                    user.FirstName = userNewData.FirstName;
                }

                if (user.LastName != userNewData.LastName)
                {
                    user.LastName = userNewData.LastName;
                }

                if (user.UserName != userNewData.UserName)
                {
                    user.UserName = userNewData.UserName;
                }

                if (!string.IsNullOrEmpty(userNewData.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, userNewData.Password);
                }
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(userNewData);
            }


        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(InputModel input)
        {
            var user = new AppUser { UserName = input.Username, FirstName = input.FirstName, LastName = input.LastName };

            await _userStore.SetUserNameAsync(user, input.Username, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, input.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View(input);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            AppUser userToDelete = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(userToDelete);
            return RedirectToAction("Index");
        }
    }
}
