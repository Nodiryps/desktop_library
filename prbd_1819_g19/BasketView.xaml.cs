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

        private bool boolConfirm;
        public bool BoolConfirm
        {
            get => boolConfirm;
            set => SetProperty<bool>(ref boolConfirm, value);
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
            ConfirmDisabled();
            App.Register<Book>(this, AppMessages.MSG_ADD_BOOK_TO_BASKET, book =>
            { 
                Items = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
                ConfirmDisabled();
            });
        }

        private void ClearBasket()
        {
            if (Items != null && Items.Count() > 0)
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
            ConfirmDisabled();
            App.NotifyColleagues(AppMessages.MSG_NBCOPIES_CHANGED);

        }

        private void UsersComboBox()
        {
            UserFilter = new RelayCommand(() => {
                Console.WriteLine(SelectedUser);
                if (SelectedUser.Basket == null)
                    SelectedUser.CreateBasket();
                    Items = new ObservableCollection<RentalItem>(SelectedUser.Basket.Items);
                ConfirmDisabled();
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
            if(Items != null && Items.Count() > 0)
            {
                SelectedUser.Basket.Confirm();
                BoolConfirm = false;
            }
                
            Items = new ObservableCollection<RentalItem>();
            App.NotifyColleagues(AppMessages.MSG_CONFIRM_BASKET);
        }

        public void ConfirmDisabled()
        {
            if (Items != null && Items.Count() > 0)
                BoolConfirm = true;
            else
                BoolConfirm = false;
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
