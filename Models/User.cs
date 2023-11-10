using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace OnMyMind.Models;

public class User
{

    //Registration//
    [Key]
    public int UserId{get;set;}

    [Required(ErrorMessage ="Invalid First Name")]
    [MinLength(2)]
    public string FirstName {get;set;}

    [Required(ErrorMessage ="Invalid Last Name")]
    [MinLength(2)]
    public string LastName {get;set;}

    [Required(ErrorMessage ="Invalid Email")]
    [EmailAddress]
    public string Email {get;set;}

    [Required(ErrorMessage ="Invalid Password: Must be 8 characters")]
    [DataType(DataType.Password)]
    [MinLength(8)]
    public string Password {get;set;}

    [NotMapped]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string PassConfirm {get;set;}


//Profile Settings//
    public string? Bio {get;set;}

    public string? Location {get;set;}

    public byte[]? ProfilePic {get;set;}


//Relationships//

    [InverseProperty("Follower")]
    public List<Connection> Following {get;set;}=new List<Connection>();

    [InverseProperty("UserFollowed")]
    public List<Connection> Followers {get;set;}=new List<Connection>();

    public List<Post> CreatedPosts {get;set;} =new List<Post>();

    public List<Like> Likes {get;set;}=new List<Like>();

    public List<Comment> Comments{get;set;}=new List<Comment>();



    public DateTime CreatedAt {get;set;}=DateTime.Now;
    public DateTime UpdatedAt {get;set;}=DateTime.Now;
}