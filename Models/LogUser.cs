using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace OnMyMind.Models;

public class LogUser 
{ 
    [Required(ErrorMessage ="Invalid Email")]
    [EmailAddress] 
    public string LogEmail{get;set;}

    [Required(ErrorMessage ="Invalid Password")]
    [DataType(DataType.Password)]
    public string LogPassword{get;set;}
} 