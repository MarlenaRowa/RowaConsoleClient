using System.ComponentModel.DataAnnotations;

namespace Catalog.Dtos
{

 public record CreateItemDto

 {
  [Required]   
    public string Name {get; init;}


    [Required]
    public bool versionOnRobot  {get; init; }

 }

}