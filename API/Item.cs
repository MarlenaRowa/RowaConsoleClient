<<<<<<< HEAD
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
      

=======
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
      

>>>>>>> 7666a386791b1a9474f76be6fe1d04ec25ac94de
}