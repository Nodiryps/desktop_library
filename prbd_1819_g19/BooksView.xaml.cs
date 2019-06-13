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

        public string HideBtnNewBook { get => HiddenShow(); }

        public ICommand ClearFilter { get; set; }
        public ICommand NewBooks { get; set; }
        public ICommand DisplayBookDetails { get; set; }
        public ICommand CategoryFilter { get; set; }
        public ICommand AddToBasket { get; set; }
        public ICommand LinkCat { get; set; }

        public string Text { get; set; }


        //////////////////////////////////////CONSTRUCT//////////////////////////////////////
        public BooksView()
        {
            InitializeComponent();
            DataContext = this;
            Filter = "";
            Books = new ObservableCollection<Book>(App.Model.Books.OrderBy(b => b.Title));
            FillCat();

            ClearFilter = new RelayCommand(() => 
            {
                Filter = "";
                Category All = App.Model.Categories.Create();
                All.Name = "All";
                Categories = new ObservableCollection<Category>(App.Model.Categories);
                Categories.Insert(0, All);
                SelectedCat = All;
                ComboBox.SelectedItem = All;
            });

            if (App.IsAdmin())
            {
                NewBooks = new RelayCommand(() => { App.NotifyColleagues(AppMessages.MSG_NEW_BOOK); });
            }

            DisplayBookDetails = new RelayCommand<Book>(book => { App.NotifyColleagues(AppMessages.MSG_DISPLAY_BOOK, book); });
            CategoryFilter = new RelayCommand<Category>(cat => { ApplyFilterAction(); });
            LinkCat = new RelayCommand<Category>(cat => { App.NotifyColleagues(AppMessages.MSG_LINK_CAT, cat); });
            AddToBasket = new RelayCommand<Book>(book => 
            {
                App.SelectedUser.AddToBasket(book);
                Console.WriteLine("AddTOBasketBooksView :" + App.CurrentUser);
                App.NotifyColleagues(AppMessages.MSG_ADD_BOOK_TO_BASKET, book);
                Books = new ObservableCollection<Book>(App.Model.Books.OrderBy(b => b.Title));
            });            

            App.Register<Book>(this, AppMessages.MSG_BOOK_CHANGED, book => { Books = new ObservableCollection<Book>(App.Model.Books.OrderBy(b => b.Title)); });
            App.Register<ICollection<Category>>(this, AppMessages.MSG_CAT_CHANGED, list => { FillCat(); ApplyFilterAction(); });
            App.Register(this, AppMessages.MSG_NBCOPIES_CHANGED, () => { Books = new ObservableCollection<Book>(App.Model.Books.OrderBy(b => b.Title)); });
        }

        private void ApplyFilterAction()
        {
            Console.WriteLine(SelectedCat);
            if (SelectedCat != null)
            {
                if (StringOk(filter) && SelectedCat.Name == "All")
                {
                    Books = new ObservableCollection<Book>(FilterQuery().OrderBy(b=>b.Title));
                }

                else if (StringOk(filter) && SelectedCat.Name != "All")
                {
                    Books = new ObservableCollection<Book>(FilterQueryCat().OrderBy(b => b.Title));
                }
                else if (!StringOk(filter) )
                {
                    if(SelectedCat.Name != "All")
                    {
                        Books.Clear();
                        foreach (var c in App.Model.Books.OrderBy(b => b.Title))
                        {
                            Console.WriteLine(SelectedCat);
                            if (c.Categories.Contains(SelectedCat))
                            {
                                Books.Add(c);
                            }
                        }
                    }
                    else if(SelectedCat.Name == "All")
                        Books = new ObservableCollection<Book>(App.Model.Books.OrderBy(b => b.Title));
                }
            }
        }

        private List<Book> FilterQuery()
        {
            return (from b in App.Model.Books
                        where b.Isbn.Contains(filter) || b.Title.Contains(filter)
                            || b.Author.Contains(filter) || b.Editor.Contains(filter) orderby b.Title
                        select b).ToList();
        }

        private List<Book> FilterQueryOnlyCat()
        {
            var list = new List<Book>();
            
            foreach (var c in App.Model.Books.OrderBy(b => b.Title)) {
                if (c.Categories.Contains(SelectedCat)) {
                    list.Add(c);
                }
            }
            return list;
        }

        private List<Book> FilterQueryCat()
        {
            var list = new List<Book>();
            var query= (from b in App.Model.Books
                    where b.Isbn.Contains(filter) || b.Title.Contains(filter)
                        || b.Author.Contains(filter) || b.Editor.Contains(filter) orderby b.Title
                    select b).ToList();

            foreach (var b in query) {
                if (b.Categories.Contains(SelectedCat)) {
                    list.Add(b);
                }
            }
            return list;
        }

        public void FillCat() {
            Categories = new ObservableCollection<Category>();
            Category All = App.Model.Categories.Create();
            All.CategoryId = -1;
            All.Name = "All";

            Categories = new ObservableCollection<Category>(App.Model.Categories);
            Categories.Insert(0, All);
            SelectedCat = Categories.ElementAt(0);
        }

        private string HiddenShow()
        {
            if (App.IsAdmin())
                return "show";
            else
                return "hidden";
        }

        private bool StringOk(string s)
        {
            return !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s);
        }
    }
}
