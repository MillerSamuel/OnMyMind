using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace OnMyMind.Models;

public class Comment
{
    [Key]
    public int CommentId{get;set;}

    public int UserId {get;set;}
    public User? UserCommenting{get;set;}

    public int PostId{get;set;}
    public Post? Post{get;set;}

    [Required]
    public string CommentText{get;set;}

    public DateTime CreatedAt {get;set;}=DateTime.Now;

}