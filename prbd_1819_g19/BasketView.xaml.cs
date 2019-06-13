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
        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get => users;
            set => SetProperty<ObservableCollection<User>>(ref users, value, () => { });
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

        public User selectedUser;
        public User SelectedUser
        {

            get => selectedUser;
            set
            {
                SetProperty<User>(ref selectedUser, value);
                if (SelectedUser != null)
                    App.SelectedUser = SelectedUser;
            }

        }

        public string IsAdmin
        {
            get { return HiddenShow(App.IsAdmin()); }
        }

        private string HiddenShow(bool valeur)
        {
            if (!valeur)
                return "hidden";
            else
                return "show";
        }

        public ICommand UserFilter { get; set; }
        public ICommand Confirm { get; set; }
        public ICommand Clear { get; set; }
        public ICommand Delete { get; set; }

        /////////////////////////////////////////////////////////CONSTRUCT/////////////////////////////////////////////////////////
        public BasketView()
        {
            InitializeComponent();
            DataContext = this;

            SelectedUser = App.CurrentUser;
            if (SelectedUser.Basket != null)
            {
                Items = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
            }
            Users = new ObservableCollection<User>(App.Model.Users);
            Delete = new RelayCommand(DeleteRental);
            Clear = new RelayCommand(ClearBasket);
            ConfirmBasket();
            UsersComboBox();
            AddBookToBasket();
            App.Register<Book>(this, AppMessages.MSG_ADD_BOOK_TO_BASKET, book =>
            {
                Items = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
            });
        }

        private void ClearBasket()
        {
            foreach (var v in SelectedUser.Basket.Items.ToList())
            {
                if (v != null)
                    App.Model.RentalItems.Remove(v);
                else
                    AddError("DeleteCat", Properties.Resources.Error_Required);
                App.Model.SaveChanges();
            }
            SelectedUser.ClearBasket();
            Items = new ObservableCollection<RentalItem>();
            App.NotifyColleagues(AppMessages.MSG_NBCOPIES_CHANGED);

        }

        private void UsersComboBox()
        {
            UserFilter = new RelayCommand(() => {
                Console.WriteLine(SelectedUser);
                if (SelectedUser.Basket == null)
                    SelectedUser.CreateBasket();
                Items = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
            });
        }

        private void ConfirmBasket()
        {
            Confirm = new RelayCommand(ConfirmRental);
        }

        private void AddBookToBasket()
        {

        }

        private void ConfirmRental()
        {
            Console.WriteLine(SelectedUser.Basket);
            SelectedUser.Basket.Confirm();
            if (SelectedUser.Basket == null)
            {
                SelectedUser.CreateBasket();
                Items = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
            }

            App.NotifyColleagues(AppMessages.MSG_CONFIRM_BASKET);
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
            //Items = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
            App.NotifyColleagues(AppMessages.MSG_NBCOPIES_CHANGED);
            Reset();
        }

        private void Reset()
        {
            selectedItem = null;
            Items = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
        }
    }
}
