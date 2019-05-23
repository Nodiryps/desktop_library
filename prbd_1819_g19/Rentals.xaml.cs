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
using prbd_1819_g19;
using PRBD_Framework;

namespace prbd_1819_g19
{
    /// <summary>
    /// Logique d'interaction pour Rentals.xaml
    /// </summary>
    public partial class Rentals : UserControlBase
    {
        public Rentals()
        {
            InitializeComponent();
            DataContext = this;
            EnableTable = true;
            if (App.IsAdmin())
            {
                enableTable = true;
            }
            Rentalz = new ObservableCollection<Rental>();
            Items = new ObservableCollection<RentalItem>();
            AddRentals();
            App.Register<RentalItem>(this, AppMessages.MSG_CONFIRM_BASKET, rental => 
            {
                Items = new ObservableCollection<RentalItem>(App.Model.RentalItems);
                FillRentalz();
            });
            SetRental = new RelayCommand<Rental>(rental => {
                Items = new ObservableCollection<RentalItem>(SelectedRental.Items);
            });

            Return();
            Delete();
            //AddBook();

        }

        public ICommand SetRental { get; set; }
        public ICommand ReturnBtn { get; set; }
        public ICommand DeleteBtn { get; set; }


        private ObservableCollection<Rental> rentalz;
        public ObservableCollection<Rental> Rentalz
        {
            get => rentalz;
            set => SetProperty<ObservableCollection<Rental>>(ref rentalz, value, () => { });
        }

        private ObservableCollection<RentalItem> items;
        public ObservableCollection<RentalItem> Items
        {
            get => items;
            set => SetProperty<ObservableCollection<RentalItem>>(ref items, value, () => { });
        }

        public Rental selectedRental;
        public Rental SelectedRental
        {
            get => selectedRental;
            set => SetProperty<Rental>(ref selectedRental, value);
        }

        private bool enableTable = false;
        public bool EnableTable
        {
            get => enableTable;
            set => RaisePropertyChanged(nameof(enableTable));
        }

        private void Return()
        {
            ReturnBtn = new RelayCommand<RentalItem>(ri => {
                ri.DoReturn();
                RefreshView();
            });
        }

        private void Delete()
        {
            DeleteBtn = new RelayCommand<RentalItem>(ri => {
                SelectedRental.RemoveItem(ri);
                RefreshView();
            });
        }

        private void RefreshView()
        {
            Items = new ObservableCollection<RentalItem>(SelectedRental.Items);
            Rentalz = new ObservableCollection<Rental>(App.Model.Rentals);
        }

       private void FillRentalz()
        {

            foreach(RentalItem ri in Items)
            {
                if (ri.Rental.RentalDate != null)
                    Rentalz.Add(ri.Rental);

            }

        }

        public void AddRentals()
        {
            foreach (var r in App.Model.Rentals)
            {
                if (r.RentalDate != null)
                    Rentalz.Add(r);
                
            }
        }







    }
}
