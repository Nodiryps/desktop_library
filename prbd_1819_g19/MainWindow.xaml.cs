using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using System.Security.Principal;
using System.Collections.ObjectModel;

namespace prbd_1819_g19
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase
    {
        public ICommand SandBox { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            WindowBase();

            //BookDetail();
            CloseTab();
            AddBookToBasket();
            NewBook();
            //NewCategory();
            DisplayBook();
            ISBNChanged();

            ////App.Register(this, AppMessages.MSG_NEW_MEMBER, () => {
            //    // crée une nouvelle instance pour un nouveau membre
            //    var member = App.Model.Users.Create();
            //    App.Model.Users.Add(member);
            //    // crée dynamiquement un nouvel onglet avec le titre "<new member>"
            //  //  AddTab(member, true);
            //});

            //App.Register<User>(this, AppMessages.MSG_DISPLAY_MEMBER, member => {
            //    if (member != null)
            //    {
            //    //    var tab = (from TabItem t in tabControl.Items where (string)t.Header == member.UserName select t).FirstOrDefault();
            //    //    if (tab == null)
            //    //       // AddTab(member, false);
            //    //    else
            //    //        Dispatcher.InvokeAsync(() => tab.Focus());
            //    //}
            //}};

            //App.Register<string>(this, AppMessages.MSG_PSEUDO_CHANGED, (s) => {
            //    (tabControl.SelectedItem as TabItem).Header = s;
            //});


        }

        private void ISBNChanged()
        {
            App.Register<string>(this, AppMessages.MSG_ISBN_CHANGED, (s) => {
                (tabControl.SelectedItem as TabItem).Header = s;
            });
        }

        private void DisplayBook()
        {
            App.Register<Book>(this, AppMessages.MSG_DISPLAY_BOOK, book =>
            {
                if (book != null)
                {
                    AddTabBook(book, false);
                    var tab = new TabItem()
                    {
                        Header = book.Title,
                        Content = new BookDetailView(book, false)
                    };
                    tabControl.Items.Add(tab);
                    Dispatcher.InvokeAsync(() => tab.Focus());
                    CloseAnglet(tab);
                }
            });
        }

        private void NewBook()
        {
            App.Register(this, AppMessages.MSG_NEW_BOOK, () =>
            {
                var book = App.Model.Books.Create();
                App.Model.Books.Add(book);
                AddTabBook(book, true);
                //var book = App.Model.Books.Create();
                var tab = new TabItem()
                {
                    Header = "<new book>",
                    Content = new BookDetailView(book, true)
                };
                tabControl.Items.Add(tab);
                Dispatcher.InvokeAsync(() => tab.Focus());

                CloseAnglet(tab);
                CloseTab();


            });
        }

        private void AddBookToBasket()
        {
            App.Register<Book>(this, AppMessages.MSG_ADD_BOOK_TO_BASKET, book =>
            {
                User currUser = App.CurrentUser;
                //if (currUser.Basket != null)
                    currUser.AddToBasket(book);
                //else
                //    currUser.CreateBasket();
            });
        }

        private void DeleteCategory()
        {
            App.Register<Category>(this, AppMessages.MSG_CAT_DEL, catToDel =>
            {
                App.Model.Categories.Remove(catToDel);
                AddTabCat(new ObservableCollection<Category>(App.Model.Categories), false);
            });
        }

        private void AddTabCat(ObservableCollection<Category> list, bool isNew)
        {
            var ctl = new CategoriesView();
            var tab = new TabItem()
            {
                Header = "<categories>",
                Content = ctl
            };
            CloseAnglet(tab);
            //ajoute cet onglet à la liste des onglets existant du TabControl
            //tabControl.Items.Add(tab);
            //exécute la méthode Focus() de l'onglet pour lui donner le focus (càd l'activer)
            Dispatcher.InvokeAsync(() => tab.Focus());
        }

        private void AddTabBook(Book book, bool isNew)
        {
            var ctl = new BookDetailView(book, isNew);
            var tab = new TabItem()
            {
                Header = isNew ? "<new book>" : book.Isbn,
                Content = ctl
            };
            CloseAnglet(tab);
            //ajoute cet onglet à la liste des onglets existant du TabControl
            //tabControl.Items.Add(tab);
            //exécute la méthode Focus() de l'onglet pour lui donner le focus (càd l'activer)
            Dispatcher.InvokeAsync(() => tab.Focus());
        }

        private void CloseTab()
        {
            App.Register<UserControlBase>(this, AppMessages.MSG_CLOSE_TAB, ctl => {
                var tab = (from TabItem t in tabControl.Items where t.Content == ctl select t).SingleOrDefault();
                ctl.Dispose();
                tabControl.Items.Remove(tab);
            });
        }

        public void CloseAnglet(TabItem tab)
        {
            tab.MouseDown += (o, e) => {
                if (e.ChangedButton == MouseButton.Middle &&
                    e.ButtonState == MouseButtonState.Pressed)
                {
                    tabControl.Items.Remove(o);
                    (tab.Content as UserControlBase).Dispose();
                }
            };
            tab.PreviewKeyDown += (o, e) => {
                if (e.Key == Key.W && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    tabControl.Items.Remove(o);
                    (tab.Content as UserControlBase).Dispose();
                }
            };
        }

        private void WindowBase()
        {
            SandBox = new RelayCommand<string>((name) =>
            {
                WindowBase frm = (WindowBase)Assembly.GetExecutingAssembly().CreateInstance("prbd_1819_g19." + name);
                if (frm != null)
                    frm.ShowDialog();
            });
        }
    }
}
