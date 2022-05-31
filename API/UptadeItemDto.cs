using System.ComponentModel.DataAnnotations;

namespace Catalog.Dtos
{

 public record UpdateItemDto

 {
  [Required]   
    public string Name {get; init;}

    [Required]
    public bool versionOnRobot  {get; init; }

 }

}