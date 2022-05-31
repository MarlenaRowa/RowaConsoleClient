namespace Catalog.Entities

{

public class Item : ICloneable
{
    public Guid Id { get; set; }

    public string Name {get; set;}

    public bool versionOnRobot {get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
      

}