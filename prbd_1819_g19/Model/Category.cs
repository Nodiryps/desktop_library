using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static prbd_1819_g19.Program;

namespace prbd_1819_g19
{
    public class Category : EntityBase<Model>
    {
        protected Category(string name)
        {
            Name = name;
        }

        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Book> Books { get; set; }

        public bool HasBook(Book book)
        {
            return Model.Categories.Find(book.BookId) != null;
        }

        public void AddBook(Book book)
        {
            if(Model.Books.Find(book.BookId) != null)
                Books.Add(book);
        }

        public void RemoveBook(Book book)
        {
            if (Books.Contains(book))
                Books.Remove(book);
        }

        public void Delete()
        {
            if(Model.Categories.Find(this) != null)
                Model.Categories.Remove(this);
        }
    }
    
}