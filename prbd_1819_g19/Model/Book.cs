using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static prbd_1819_g19.Program;

namespace prbd_1819_g19
{
    public class Book : EntityBase<Model> 
    {
        public Book(string isbn, string title, string author, string editor, int numCopies)
        {
            Isbn = isbn;
            Title = title;
            Author = author;
            Editor = editor;
            if (numCopies >= 1)
                NumAvailableCopies = numCopies;
            else
                throw new Exception("numCopies < 1 !");
        }

        public int BookId { get; set; }
        public string Isbn { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Editor { get; set; }
        public string PicturePath { get; set; }
        public int NumAvailableCopies { get; }

        public void AddCategory(Category category)
        {
            if (Model.Categories.Find(category.Name) == null)
                Model.Categories.Add(category);
        }

        public void AddCategories(Category[] tab)
        {
            foreach(Category category in tab)
                if (Model.Categories.Find(category.Name) == null)
                    Model.Categories.Add(category);
        }


        public void RemoveCategory(Category category)
        {
            if(Model.Categories.Find(category.Name) != null)
                Model.Categories.Remove(category);
        }

        public void AddCopies(int quantity, DateTime date)
        {
            if(Model.BookCopies.Find(BookId) == null)
            {
                for (int i = 0; i < quantity; ++i)
                {
                    BookCopy toAdd = Model.BookCopies.Create();
                    Model.BookCopies.Add(toAdd);
                }
            }
        }

        public List<BookCopy> GetBookCopies()
        {
            List<BookCopy> list = new List<BookCopy>();
            Model.BookCopies.Find(BookId);

            return list;
        }

        public BookCopy GetAvailableCopy()
        {
            BookCopy availableCopy = null;
            if (NumAvailableCopies > 0)
                availableCopy.BookCopyId = BookId;
            return availableCopy;
        }

        public void DeleteCopy(BookCopy copy)
        {
            if (Model.BookCopies.Find(copy.BookCopyId) != null)
                Model.BookCopies.Remove(copy);
        }

        public void Delete()
        {
            if (Model.Books.Find(BookId) != null)
                Model.Books.Remove(this);
        }
    }
}
