using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace prbd_1819_g19
{
    /// <summary>
    /// Logique d'interaction pour BooksView.xaml
    /// </summary>
    public partial class BooksView : UserControlBase
    {

        public BooksView()
        {
            InitializeComponent();
            //if (DesignerProperties.GetIsInDesignMode(this))
            //    return;

            DataContext = this;

            Books = new ObservableCollection<Book>(App.Model.Books);
            Categories = new ObservableCollection<Category>(App.Model.Categories);

            ClearFilter = new RelayCommand(() => { Filter = ""; });
            NewBooks = new RelayCommand(() => { App.NotifyColleagues(AppMessages.MSG_NEW_BOOK); });
            DisplayBookDetails = new RelayCommand<Book>(book => { App.NotifyColleagues(AppMessages.MSG_DISPLAY_BOOK, book); });
            CategoryFilter = new RelayCommand<Category>(cat=> { ApplyComboBoxFilter(); });
            AddToBasket = new RelayCommand<Book>(book => { App.NotifyColleagues(AppMessages.MSG_ADD_BOOK_TO_BASKET,book); });

            App.Register<Book>(this, AppMessages.MSG_ADD_BOOK_TO_BASKET, book => { Books = new ObservableCollection<Book>(App.Model.Books); });
            App.Register<Book>(this, AppMessages.MSG_BOOK_CHANGED, book => { Books = new ObservableCollection<Book>(App.Model.Books); });
            App.Register(this, AppMessages.MSG_CAT_CHANGED, () => { Categories = new ObservableCollection<Category>(App.Model.Categories); });
        }



        public Category SelectedCat { get; set; }

        private ObservableCollection<Book> books;
        public ObservableCollection<Book> Books
        {
            get => books;
            set => SetProperty<ObservableCollection<Book>>(ref books, value);
        }

        private ObservableCollection<Category> categories;
        public ObservableCollection<Category> Categories
        {
            get => categories;
            set => SetProperty<ObservableCollection<Category>>(ref categories, value);
        }

        private string filter;
        public string Filter
        {
            get => filter;
            set => SetProperty<string>(ref filter, value, ApplyFilterAction);
        }

        public ICommand ClearFilter { get; set; }
        public ICommand NewBooks { get; set; }
        public ICommand DisplayBookDetails { get; set; }
        public ICommand CategoryFilter { get; set; }
        public ICommand AddToBasket { get; set; }

        

        private void ApplyFilterAction()
        {
            IEnumerable<Book> filtered = App.Model.Books;
            if (!string.IsNullOrEmpty(Filter))
            {
                filtered = from b in App.Model.Books
                           where b.Isbn.Contains(filter) || b.Title.Contains(filter)
                               || b.Author.Contains(filter) || b.Editor.Contains(filter)
                           select b;
                Books = new ObservableCollection<Book>(filtered);
                foreach (var t in filtered)
                {
                    Console.Write(t.Title);
                }
            }
            else
            {
                Books = new ObservableCollection<Book>(App.Model.Books);
            }
        }

        private void ApplyComboBoxFilter()
        {
                Books = new ObservableCollection<Book>(SelectedCat.Books);
        }

        private void RefreshBook()
        {
            App.Register(this, AppMessages.MSG_NEW_BOOK, () =>
            {
                Books = new ObservableCollection<Book>(App.Model.Books);

            });
        }

        private void RefreshCategories()
        {
            App.Register(this, AppMessages.MSG_CAT_CHANGED, () =>
             {
                 Categories = new ObservableCollection<Category>(App.Model.Categories);
             });
        }
    }
}
