using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnMyMind.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DbConnection;
namespace OnMyMind.Controllers;


public class HomeController : Controller
{
    private MyContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _context = context;
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
        if (ModelState.IsValid)
        {
            if (_context.Users.Any(a => a.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "Email already in use");
                return View("Register");
            }
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            _context.Add(newUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("user", newUser.UserId);
            return RedirectToAction("EditProfile");
        }
        return View("Register");
    }

    [HttpPost("Login")]
    public IActionResult Login(LogUser logUser)
    {
        if (ModelState.IsValid)
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
            }
        }
        return View("Index");
    }

    [HttpGet("Dashboard")]
    public IActionResult Dashboard()
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("Index");
        }
        ViewBag.allUsers = _context.Users.Include(a => a.Followers).ThenInclude(a => a.Follower).ToList();
        ViewBag.LoggedUser = _context.Users.Include(a => a.Following)
        .ThenInclude(a => a.UserFollowed)
        .ThenInclude(followed => followed.CreatedPosts)
        .ThenInclude(post => post.Likes)
        .Include(a => a.Likes).ThenInclude(a => a.Post)
        .FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("user"));
        // ViewBag.AllPosts=_context.Posts.Include(post=>Likes).ToList();
        return View();
    }


    // [HttpGet("Profile")]
    // public IActionResult Profile()
    // {
    //     if(HttpContext.Session.GetInt32("user")==null)
    //     {
    //         return RedirectToAction("Index");
    //     }
    //     ViewBag.User=_context.Users.Include(a=>a.CreatedPosts).FirstOrDefault(a=>a.UserId==HttpContext.Session.GetInt32("user"));
    //     return View();
    // }

    [HttpGet("Profile/{userId}")]
    public IActionResult Profile2(int userId)
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("Index");
        }
        ViewBag.User = _context.Users.Include(a => a.CreatedPosts).ThenInclude(post => post.Likes).Include(a => a.Followers).ThenInclude(a => a.Follower).Include(a => a.Following).ThenInclude(a => a.UserFollowed).FirstOrDefault(a => a.UserId == userId);
        ViewBag.LoggedUser = _context.Users.Include(a => a.Likes).ThenInclude(a => a.Post).Include(a => a.Following).ThenInclude(a => a.UserFollowed).FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("user"));
        return View("Profile");
    }

    [HttpGet("EditProfile")]
    public IActionResult EditProfile()
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("Index");
        }
        User LoggedUser = _context.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("user"));
        ViewBag.LoggedUser = _context.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("user"));
        return View(LoggedUser);
    }

    [HttpPost("SaveEdit")]
    public IActionResult SaveEdit(User editUser)
    {
        User oldUser = _context.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("user"));
        oldUser.Bio = editUser.Bio;
        oldUser.Location = editUser.Location;
        oldUser.UpdatedAt = DateTime.Now;
        oldUser.ProfilePic = editUser.ProfilePic;
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }

    [HttpGet("NewPost")]
    public IActionResult NewPost()
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("Index");
        }
        User LoggedUser = _context.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("user"));
        ViewBag.LoggedUser = _context.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("user"));
        return View();
    }


    [HttpPost("CreatePost")]
    public IActionResult CreatePost(Post newPost)
    {
        if (ModelState.IsValid)
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
        Connection newFollow = new Connection();
        newFollow.FollowerId = (int)HttpContext.Session.GetInt32("user");
        newFollow.UserFollowedId = UserId;
        _context.Add(newFollow);
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }


    [HttpGet("Unfollow/{UserId}")]
    public IActionResult RemoveFollow(int UserId)
    {
        Connection connection = _context.Connections.First(a => a.UserFollowedId == UserId && a.FollowerId == (HttpContext.Session.GetInt32("user")));
        _context.Remove(connection);
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }



    [HttpGet("FollowingList/{UserId}")]
    public IActionResult FollowingList(int UserId)
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("Index");
        }
        ViewBag.loggedUser = _context.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("user"));
        ViewBag.User = _context.Users.Include(a => a.Following).ThenInclude(a => a.UserFollowed).FirstOrDefault(a => a.UserId == UserId);
        return View("FollowingList");
    }

    [HttpGet("FollowersList/{UserId}")]
    public IActionResult FollowersList(int UserId)
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("Index");
        }
        ViewBag.loggedUser = _context.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("user"));
        ViewBag.User = _context.Users.Include(a => a.Followers).ThenInclude(a => a.Follower).FirstOrDefault(a => a.UserId == UserId);
        return View("FollowersList");
    }


    [HttpGet("Like/{PostId}")]
    public IActionResult AddLike(int PostId)
    {
        Like newLike = new Like();
        newLike.PostId = PostId;
        newLike.UserId = (int)HttpContext.Session.GetInt32("user");
        _context.Add(newLike);
        _context.SaveChanges();
        return RedirectToAction("dashboard");
    }

    [HttpGet("Unlike/{Postid}")]
    public IActionResult Unlike(int PostId)
    {
        Like like = _context.Likes.First(a => a.PostId == PostId && a.UserId == (HttpContext.Session.GetInt32("user")));
        _context.Remove(like);
        _context.SaveChanges();
        return RedirectToAction("dashboard");
    }

    // [HttpGet("Like2/{PostId}")]
    // public IActionResult AddLike2(int PostId)
    // {
    //     Like newLike=new Like();
    //     newLike.PostId=PostId;
    //     newLike.UserId=(int)HttpContext.Session.GetInt32("user");
    //     _context.Add(newLike);
    //     _context.SaveChanges();
    //     return RedirectToAction ("ViewPost");
    // }



    [HttpGet("LikesList/{UserId}")]
    public IActionResult LikesList(int UserId)
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("Index");
        }
        ViewBag.LoggedUser = _context.Users.Include(user => user.Likes).ThenInclude(likes => likes.Post).FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("user"));
        ViewBag.User = _context.Users.Include(a => a.Likes).ThenInclude(a => a.Post).ThenInclude(a => a.User).Include(user => user.Likes).ThenInclude(likes => likes.Post.Likes).FirstOrDefault(a => a.UserId == UserId);
        return View("LikesList");
    }

    [HttpPost("PostComment")]
    public IActionResult PostComment(Comment comment)
    {
        ViewBag.allUsers = _context.Users.Include(a => a.Followers).ThenInclude(a => a.Follower).ToList();
        ViewBag.LoggedUser = _context.Users.Include(a => a.Following)
        .ThenInclude(a => a.UserFollowed)
        .ThenInclude(followed => followed.CreatedPosts)
        .ThenInclude(post => post.Likes)
        .Include(a => a.Likes).ThenInclude(a => a.Post)
        .FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("user")); if (ModelState.IsValid)
        {
            _context.Add(comment);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        return View("Dashboard");
    }


    [HttpGet("ViewPost/{PostId}")]
    public IActionResult ViewPost(int PostId)
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("Index");
        }
        ViewBag.LoggedUser = _context.Users.Include(a => a.Following)
                .ThenInclude(a => a.UserFollowed)
                .ThenInclude(followed => followed.CreatedPosts)
                .ThenInclude(post => post.Likes)
                .Include(a => a.Likes).ThenInclude(a => a.Post)
                .FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("user"));
        ViewBag.Post = _context.Posts.Include(post => post.Comments).ThenInclude(comment => comment.UserCommenting).FirstOrDefault(a => a.PostId == PostId);
        return View();
    }


    [HttpPost("Search")]
    public IActionResult SearchResults(string Search)
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("Index");
        }
        ViewBag.LoggedUser = _context.Users.Include(user => user.Likes).ThenInclude(like => like.Post).FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("user"));
        List<Dictionary<string, object>> SearchedUsers = DbConnector.Query($"SELECT * FROM USERS WHERE FirstName Like '%{Search}%'or LastName Like '%{Search}%'");
        ViewBag.NewResults = SearchedUsers;
        return View("SearchResults");
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
