using Catalog.Entities;

namespace Catalog.Repositiories

{


    public class InMemItemsRepository : IItemsRepository

    {

        private readonly List<Item> items = new()

{

    new Item { Id = Guid.NewGuid(), Name = "blasius1", versionOnRobot = true },

    new Item { Id = Guid.NewGuid(), Name = "blasius2", versionOnRobot = true },

    new Item { Id = Guid.NewGuid(), Name = "blasius5", versionOnRobot = true }

};

        public IEnumerable<Item> GetItems()
        {
            return items;

        }

        public Item GetItem(Guid id)

        {
            return items.Where(item => item.Id == id).SingleOrDefault();

        }

        public void CreateItem(Item item)
        {
            items.Add(item);
        }

        public void UpdateItem(Item item)
        {
           var index = items.FindIndex(existingItem => existingItem.Id == item.Id);
           items[index] = item;
        }

        public void DeleteItem(Guid id)
        {
           var index = items.FindIndex(existingItem => existingItem.Id == id);
           items.RemoveAt(index);
        }
    }



}