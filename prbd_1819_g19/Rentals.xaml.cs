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
            Rentalz = new ObservableCollection<Rental>();
            Items = new ObservableCollection<RentalItem>();
            addRentals();
            //addBook();


        }

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


        public void addBook()
        {
            foreach (var b in SelectedRental.Items)
            {
                Items.Add(b);

            }
        }

        public void addRentals()
        {
            foreach (var r in App.Model.Rentals)
            {
                Rentalz.Add(r);
                
            }
        }







    }
}
