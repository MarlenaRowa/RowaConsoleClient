namespace Catalog.Entities

{

public record Item
{
    public Guid Id { get; init; }

    public string Name {get; init;}

    public bool versionOnRobot {get; init; }


}
      

}