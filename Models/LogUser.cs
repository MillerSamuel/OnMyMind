using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace OnMyMind.Models;

public class LogUser 
{ 
    [Required]
    [EmailAddress] 
    public string LogEmail{get;set;}

    [Required]
    [DataType(DataType.Password)]
    public string LogPassword{get;set;}
} 