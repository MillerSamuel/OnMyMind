using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace OnMyMind.Models;
public class Connection
{
    [Key]
    public int ConnectionId { get; set; }

    public int FollowerId { get; set; }
    public User? Follower { get; set; }

    public int UserFollowedId { get; set; }
    public User? UserFollowed { get; set; }

    public DateTime CreatedAt {get;set;}=DateTime.Now;

}