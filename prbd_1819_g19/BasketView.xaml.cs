﻿using System;
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
            Items = new ObservableCollection<RentalItem>();
            Users = new ObservableCollection<User>(App.Model.Users);
            AddBook();
            Delete = new RelayCommand(DeleteRental, () =>{ return true; });
            Clear = new RelayCommand(ClearRental, () => { return true; });
            Confirm = new RelayCommand(ConfirmRental, () => { return true; });
            UserFilter = new RelayCommand<RentalItem>(r => { SelectByUser(); });
            AddBookToBasket();

            //ConfirmBtn();
        }

        public ICommand UserFilter { get; set; }

        public User selectedUser;
        public User SelectedUser
        {

            get => selectedUser;
            set => SetProperty<User>(ref selectedUser, value);
        }


        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get => users;
            set => SetProperty<ObservableCollection<User>>(ref users, value, () => {});
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
        
        private void AddBook() {
                    
            //Si l'utilisateur courant n'est pas Admin
                    if (App.CurrentUser.Role != 0)
                    {
                    var res = new ObservableCollection<RentalItem>();
                    var v = (from r in App.Model.RentalItems
                                 where App.CurrentUser.UserId == r.Rental.User.UserId
                                 select r);
                        if (v != null)
                        {
                            foreach (var c in v)
                            {
                                res.Add(c);
                            }
                            Items = res;
                        }
                    }
            //Si l'utilisateur courant est un Admin
            else
            {
                    foreach (var b in App.Model.RentalItems)
                    {
                    Items.Add(b);
                    }
                }
            App.Model.SaveChanges();
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

        public void  SelectByUser()
        {
            //Fonctionnalité uniquement pour l'admin

            //if(App.CurrentUser.Role == 0)
            //{
                var res = new ObservableCollection<RentalItem>();

                if (SelectedUser != null)
                {
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

            //}
        }
       

        

        public ICommand Confirm { get; set; }
        public ICommand Clear { get; set; }
        public ICommand Delete { get; set; }

        private void ConfirmBtn()
        {
            Confirm = new RelayCommand(ConfirmRental);
        }

        private void ConfirmRental()
        {
           App.CurrentUser.Basket.Confirm();
        }

        //

        //private void ClearBtn()
        //{
        //    Clear = new RelayCommand(ClearRental);
        //}

        private void ClearRental()
        {
            if(App.CurrentUser.Role != 0)
            Items.Clear();
            App.CurrentUser.ClearBasket();
        }

        //


        //private void DeleteBtn()
        //{
        //     Console.Write("DSFDSJFSDLKF");

        //    Delete = new RelayCommand(DeleteRental, () =>
        //    {
               
        //        //return IsNullOrEmpty(SelectedCategory.Name);
        //        return true;

        //    });
        //}

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
            
            Reset();

        }

        private void Reset()
        {
         
            selectedItem = null;
            Items = new ObservableCollection<RentalItem>(App.Model.RentalItems);
        }
    }
}
