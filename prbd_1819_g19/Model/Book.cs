using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static prbd_1819_g19.Program;

namespace prbd_1819_g19
{
    public class Book : EntityBase<Model> 
    {
        [Required]
        public int BookId { get; set; }
        public string Isbn { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Editor { get; set; }
        public string PicturePath { get; set; }
        public int NumAvailableCopies { get; }

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<BookCopy> Copies { get; set; }

        protected Book(string isbn, string title, string author, string editor, int numCopies)
        {
            Isbn = isbn;
            Title = title;
            Author = author;
            Editor = editor;
            NumAvailableCopies = numCopies;
        }

        public void AddCategory(Category category)
        {
            if(Model.Categories.Find(category.Name) != null)
                if (!Categories.Contains(category))
                {
                    Categories.Add(category);
                }
        }

        public void AddCategories(Category[] tab)
        {
            foreach (Category category in tab)
                AddCategory(category);
        }

        public void RemoveCategory(Category category)
        {
            if(Categories.Contains(category))
                Categories.Remove(category);
        }

        public void AddCopies(int quantity, DateTime date)
        {
            for (int i = 0; i < quantity; ++i)
            {
                BookCopy copy = Model.BookCopies.Create();
                Copies.Add(copy);
                Model.BookCopies.Add(copy);
            }
        }
        
        public BookCopy GetAvailableCopy()
        {
            return (
                    from copy in Model.BookCopies
                    where copy.Book.BookId == BookId && 
                        ( from item in copy.RentalItems
                          where item.ReturnDate == null
                          select item 
                        ).Count() == 0
                    select copy
            ).FirstOrDefault();
                
        }

        public void DeleteCopy(BookCopy copy)
        {
            if (Model.BookCopies.Find(copy.BookCopyId) != null)
            {
                Model.BookCopies.Remove(copy);
                Copies.Remove(copy);
            }
        }

        public void Delete()
        {
            if (Model.Books.Find(BookId) != null)
                Model.Books.Remove(this);
        }
    }
}
