using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PRBD_Framework;

namespace prbd_1819_g19
{
    /// <summary>
    /// Logique d'interaction pour BasketView.xaml
    /// </summary>
    public partial class BasketView : UserControlBase
    {
        public BasketView()
        {
            InitializeComponent();
            DataContext = this;
            SelectedUser = App.CurrentUser;
            Users = new ObservableCollection<User>(App.Model.Users);
            Delete = new RelayCommand(DeleteRental, () => { return true; });
            Clear = new RelayCommand(ClearBasket);
            ConfirmBasket();
            //App.Register(this, AppMessages.MSG_BASKET_CHANGED, () => {
            //    Items = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
            //});
            UsersComboBox();
            AddBookToBasket();

            //ConfirmBtn();
        }

        private void ClearBasket()
        {
            SelectedUser.ClearBasket();
            Items = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
        }

        public ICommand UserFilter { get; set; }
        public ICommand Confirm { get; set; }
        public ICommand Clear { get; set; }
        public ICommand Delete { get; set; }

        public User selectedUser;
        public User SelectedUser
        {

            get => selectedUser;
            set => SetProperty<User>(ref selectedUser, value);
        }

        public string IsAdmin
        {
            get { return HiddenShow(App.IsAdmin()); }
        }

        private String HiddenShow(bool valeur)
        {
            if (!valeur)
                return "hidden";
            else
                return "show";
        }

        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get => users;
            set => SetProperty<ObservableCollection<User>>(ref users, value, () => { });
        }

        public User GetUserCurrent()
        {
            return App.CurrentUser;
        }

        public RentalItem selectedItem;
        public RentalItem SelectedItem
        {
            get => selectedItem;
            set => SetProperty<RentalItem>(ref selectedItem, value);
        }

        private ObservableCollection<RentalItem> items;
        public ObservableCollection<RentalItem> Items
        {
            get => items;
            set => SetProperty<ObservableCollection<RentalItem>>(ref items, value, () => { });
        }

        private void UsersComboBox()
        {
            UserFilter = new RelayCommand(() => {
                if (SelectedUser.Basket == null)
                {
                    SelectedUser.CreateBasket();
                }
                Items = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
            });
        }

        private void ConfirmBasket()
        {
            Confirm = new RelayCommand(ConfirmRental, () => { return true; });

            App.NotifyColleagues(AppMessages.MSG_CONFIRM_BASKET, items);
            App.NotifyColleagues(AppMessages.MSG_BASKET_CHANGED, items);
        }

        private void AddBookToBasket()
        {
            App.Register<Book>(this, AppMessages.MSG_ADD_BOOK_TO_BASKET, book =>
            {
                User currUser = App.CurrentUser;
                currUser.AddToBasket(book);
                SelectByUser();
            });
        }

        private void ConfirmRental()
        {
            SelectedUser.Basket.Confirm();
            if (AddBasket()) 
                Items = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
        }

        public void SelectByUser()
        {
            var res = new ObservableCollection<RentalItem>();
            if (SelectedUser == null)
                selectedUser = App.CurrentUser;
            var v = (from r in App.Model.RentalItems
                     where SelectedUser.UserId == r.Rental.User.UserId
                     select r);
            if (v != null)
            {
                foreach (var b in v)
                {
                    res.Add(b);
                }
                Items = res;
            }
            App.Model.SaveChanges();
        }

        private void DeleteRental()
        {
            var v = (from r in App.Model.RentalItems
                     where selectedItem.RentalItemId == r.RentalItemId
                     select r).FirstOrDefault();
            if (v != null)
                App.Model.RentalItems.Remove(v);
            else
                AddError("DeleteCat", Properties.Resources.Error_Required);
            App.Model.SaveChanges();
            Items = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
            App.NotifyColleagues(AppMessages.MSG_NBCOPIES_CHANGED);
            Reset();
        }

        private void Reset()
        {
            selectedItem = null;
            Items = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
        }

        public bool AddBasket() {
            if (selectedUser.Basket == null) {
                selectedUser.CreateBasket();
                return true;
            }
            return false;
        }
    }
}
