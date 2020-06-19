using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccount.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace BankAccount.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        private User _currentUser;

        public HomeController(MyContext context) {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            if(_isLogged()){
                return RedirectToAction("Account");
            }
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser user)
        {
            if(ModelState.IsValid && _login(user.LoginUserEmail, user.LoginUserPassword))
                return RedirectToAction("Account");
            else
                return View();
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            _logout();
            return RedirectToAction("Index");
        }


        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            User userInDB = _context.Users.FirstOrDefault(u=>u.Email==newUser.Email);

            if(ModelState.IsValid) {
                if(_context.Users.FirstOrDefault(u=>u.Email==newUser.Email) == null) {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    _context.Users.Add(newUser);
                    _context.SaveChanges();

                    _login(newUser.Email, newUser.Password);
                    return RedirectToAction("Account");

                } else {
                    ModelState.AddModelError("Email", "Email is already in use, try logging in");
                    return View("Index");
                }
            }
            else
                return View("Index");
        }

        [HttpGet("account")]
        public IActionResult Account()
        {
            if(_isLogged()) {

                List<Transaction> transactions = _context.Transactions
                    .Include(t => t.Owner)
                    .Where( t => t.UserID == _currentUser.UserID)
                    .OrderByDescending( t => t.CreatedAt)
                    .ToList();

                ViewBag.Transactions = transactions;
                ViewBag.User = _currentUser;
                var balance = transactions.Sum(t=>t.Amount);
                ViewBag.Balance = transactions.Sum(t=>t.Amount);

                return View();    
            }
            else {
                return RedirectToAction("Index");
            }
        }

        [HttpPost("save/transaction")]
        public IActionResult SaveTransaction(Transaction newTransaction)
        {
            if(_isLogged()){

                List<Transaction> transactions = _context.Transactions
                    .Include(t => t.Owner)
                    .Where( t => t.UserID == _currentUser.UserID)
                    .OrderByDescending( t => t.CreatedAt)
                    .ToList();

                ViewBag.Transactions = transactions;
                ViewBag.User = _currentUser;
                var balance = transactions.Sum(t=>t.Amount);
                ViewBag.Balance = transactions.Sum(t=>t.Amount);

                if(ModelState.IsValid) {
                    if(newTransaction.Amount!=0) {

                        if(balance+newTransaction.Amount>0){
                            newTransaction.UserID = _currentUser.UserID;
                            _context.Transactions.Add(newTransaction);
                            _context.SaveChanges();
                            return RedirectToAction("Account");
                        } else {
                            ModelState.AddModelError("Amount", "Not enough balance");
                            return View("Account");
                        }
                    } else {
                        ModelState.AddModelError("Amount", "Amount shoud be not equal 0");
                        return View("Account");
                    }
                }
                else
                    return View("Account");


            }
            else {
                return RedirectToAction("Index");
            }
        }

        private bool _isLogged() {
            if(HttpContext.Session.GetInt32("uid")!=null) {
                _currentUser = _context.Users.FirstOrDefault(u => u.UserID == HttpContext.Session.GetInt32("uid"));
                return true;
            }
            return false;
        }

        private bool _login(string email, string password) {

            // If inital ModelState is valid, query for a user with provided email
            var userInDb = _context.Users.FirstOrDefault(u => u.Email == email);
            // If no user exists with provided email
            if(userInDb == null)
            {
                return false;
            }
            
            // Initialize hasher object
            var hasher = new PasswordHasher<LoginUser>();

            LoginUser user = new LoginUser() {
                LoginUserEmail = email,
                LoginUserPassword = password
            };
            
            // verify provided password against hash stored in db
            var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.LoginUserPassword);
            
            // result can be compared to 0 for failure
            if(result == 0)
            {
                return false;
            }
            _currentUser = userInDb;
            HttpContext.Session.SetInt32("uid",userInDb.UserID);
            return true;   
        }

        private void _logout() {
            _currentUser = null; 
            HttpContext.Session.Remove("uid");
        } 

    }
}
