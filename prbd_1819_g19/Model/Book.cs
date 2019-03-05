using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1819_g19
{
    public class Book : EntityBase<Model> 
    {
        private string title;
        private int numCopies;

        public Book(string isbn, string title, string author, string editor, int numCopies)
        {
            Isbn = isbn;
            this.title = title;
            Author = author;
            Editor = editor;
            this.numCopies = numCopies;
        }

        public int BookId { get; set; }
        public string Isbn { get; set; }
        public string Author { get; set; }
        public string Editor { get; set; }
        public string PicturePath { get; set; }
        public int NumAvailableCopies { get; }

        public void AddCategory(Category category)
        {
            
        }


        public void RemoveCategory(Category category)
        {
            
        }

        public void AddCopies(int quantity, DateTime date)
        {

        }

        public BookCopy GetAvailableCopy()
        {

        }

        public void DeleteCopy(BookCopy copy)
        {

        }

        public void Delete()
        {

        }
    }
}
