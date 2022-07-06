using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace OnMyMind.Models;

public class Post
{
    [Key]
    public int PostId{get;set;}

    
    public string? PostText{get;set;}

    public string? MediaLocation{get;set;}

    public int UserId {get;set;}
    public User? User{get;set;}

    public List<Like> Likes=new List<Like>();

    public List<Comment> Comments=new List<Comment>();


    public DateTime CreatedAt {get;set;}=DateTime.Now;


}