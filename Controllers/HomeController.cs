﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnMyMind.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace OnMyMind.Controllers;

public class HomeController : Controller
{
    private MyContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger,MyContext context)
    {
        _context=context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        HttpContext.Session.Clear();
        return View();
    }


    [HttpGet("Register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("CreateAccount")]
    public IActionResult CreateAccount(User newUser)
    {
        if(ModelState.IsValid)
        {
            if(_context.Users.Any(a=>a.Email==newUser.Email))
            {
                ModelState.AddModelError("Email","Email already in use");
                return View("Register");
            }
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            _context.Add(newUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("user", newUser.UserId);
            return RedirectToAction ("EditProfile");
        }
        return View("Register");
    }

    [HttpPost("Login")]
    public IActionResult Login(LogUser logUser)
    {
        if(ModelState.IsValid)
        {
            User userInDb = _context.Users.FirstOrDefault(a => a.Email == logUser.LogEmail);
            if (userInDb == null)
            {
                ModelState.AddModelError("LogEmail", "Invalid Login");
                return View("Index");
            }
            PasswordHasher<LogUser> hasher = new PasswordHasher<LogUser>();
            var result = hasher.VerifyHashedPassword(logUser, userInDb.Password, logUser.LogPassword);
            if (result == 0)
            {
                ModelState.AddModelError("LogEmail", "Invalid Login");
                return View("Index");
            }
            else
            {
                HttpContext.Session.SetInt32("user", userInDb.UserId);
                return RedirectToAction("dashboard");
            }        }
        return View("Index");
    }

    [HttpGet("Dashboard")]
    public IActionResult Dashboard()
    {
        if(HttpContext.Session.GetInt32("user")==null)
        {
            return RedirectToAction("Index");
        }
            ViewBag.allUsers=_context.Users.Take(4);
            return View();
    }


    [HttpGet("Profile")]
    public IActionResult Profile()
    {
        if(HttpContext.Session.GetInt32("user")==null)
        {
            return RedirectToAction("Index");
        }
        ViewBag.User=_context.Users.Include(a=>a.CreatedPosts).FirstOrDefault(a=>a.UserId==HttpContext.Session.GetInt32("user"));
        return View();
    }

    [HttpGet("Profile/{userId}")]
    public IActionResult Profile2(int userId)
    {
        if(HttpContext.Session.GetInt32("user")==null)
        {
            return RedirectToAction("Index");
        }
        ViewBag.User=_context.Users.Include(a=>a.CreatedPosts).FirstOrDefault(a=>a.UserId==userId);
        return View("Profile");
    }

    [HttpGet("EditProfile")]
    public IActionResult EditProfile()
    {
        if(HttpContext.Session.GetInt32("user")==null)
        {
            return RedirectToAction("Index");
        }
        User loggedUser=_context.Users.FirstOrDefault(a=>a.UserId==HttpContext.Session.GetInt32("user"));
        return View(loggedUser);
    }

    [HttpPost("SaveEdit")]
    public IActionResult SaveEdit(User editUser)
    {
        User oldUser=_context.Users.FirstOrDefault(a=>a.UserId==HttpContext.Session.GetInt32("user"));
        oldUser.Bio=editUser.Bio;
        oldUser.Location=editUser.Location;
        oldUser.UpdatedAt=DateTime.Now;
        _context.SaveChanges();
        return RedirectToAction("Profile");
    }

    [HttpGet("NewPost")]
    public IActionResult NewPost()
    {
        if(HttpContext.Session.GetInt32("user")==null)
        {
            return RedirectToAction("Index");
        }
        ViewBag.loggedUser=_context.Users.FirstOrDefault(a=>a.UserId==HttpContext.Session.GetInt32("user"));
        return View();
    }


    [HttpPost("CreatePost")]
    public IActionResult CreatePost(Post newPost)
    {
        if(ModelState.IsValid)
        {
            _context.Add(newPost);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        return View("NewPost");
    }


    [HttpGet("Follow/{UserId}")]
    public IActionResult AddFollow(int UserId)
    {
        Connection newFollow=new Connection();
        newFollow.FollowerId=(int)HttpContext.Session.GetInt32("user");
        newFollow.UserFollowedId=UserId;
        _context.Add(newFollow);
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }

    [HttpGet("FollowingList/{UserId}")]
    public IActionResult FollowingList(int UserId)
    {
        ViewBag.User=_context.Users.Include(a=>a.Following).ThenInclude(a=>a.UserFollowed).FirstOrDefault(a=>a.UserId==UserId);
        return View("FollowingList");
    }































    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
