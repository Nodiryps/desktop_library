using static prbd_1819_g19.Program;

namespace prbd_1819_g19
{
    public class Category : EntityBase<Model>
    {
        public Category(string name)
        {
            Name = name;
        }

        public int CategoryId { get; set; }
        public string Name { get; set; }

        public bool HasBook(Book book)
        {
            return book != null;
        }

        public void AddBook(Book book)
        {

        }

        public void RemoveBook(Book book)
        {

        }

        public void Delete()
        {

        }
    }
    
}