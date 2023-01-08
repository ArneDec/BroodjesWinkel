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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Prb.Ee.Broodjes.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //array voor het opslaan van de prijzen
        decimal[] sandwichPrice = new decimal[] { 0.5m, 0.6m, 0.7m };
        decimal[] toppingPrice = new decimal[] { 1.0m, 0.7m, 2.0m, 1.0m };

        decimal breadPlusToppingPrice = 0.0m;
        decimal revenuePerSandwich = 0.0m;

        decimal sandwichTotalPrice = 0.0m;
        decimal toppingTotalPrice = 0.0m;

        //list voor het opslaan van de broodjes prijzen brood + beleg
        List<decimal> sumBreadAndTopping = new List<decimal>();

        decimal revenue = 0.0m;
        int sandwichesSold = 0;

        public MainWindow()
        {
            InitializeComponent();
            FillCombobox();
            tbkError.Text = string.Empty;
            btnPay.IsEnabled = false;
            btnRemoveOrder.IsEnabled = false;
            txtCustomerName.Focus();

        }

        //Combobox opvullen met de enums
        void FillCombobox()
        {
            cmbSandwiches.Items.Clear();
            cmbSandwiches.ItemsSource = Enum.GetValues(typeof(BreadType));
            cmbSandwiches.SelectedIndex = 0;
        }

        //Methode om te controleren of er een textinput is bij naam, en berekent de geselecteerde items met de correcte prijs
        void OrderSummaryAndCalculatePrice()
        {
            //checkt als naam is ingevuld
            if (txtCustomerName.Text == string.Empty)
            {
                tbkError.Text = "Gelieve een klantnaam in te vullen, aub.";
                btnPay.IsEnabled = false;
                return;
            }


            lstOrderSummary.Items.Clear();
            TblTotaal.Text = string.Empty;

            string customerName = txtCustomerName.Text;
            string sandwichType = cmbSandwiches.SelectedItem.ToString();
            string toppings = "";

            //switch case om het geselecteerde broodje te kiezen en prijs te bepalen
            switch (cmbSandwiches.SelectedIndex)
            {
                case 0:
                    sandwichTotalPrice = sandwichPrice[0];
                    break;
                case 1:
                    sandwichTotalPrice = sandwichPrice[1];
                    break;
                case 2:
                    sandwichTotalPrice = sandwichPrice[2];
                    break;
            }

            //if statement om de prijs van de gekozen toppings samen te tellen
            if (chkTopping1.IsChecked == true)
            {
                toppings += "Hesp\n";
                toppingTotalPrice += toppingPrice[0];
            }
            if (chkTopping2.IsChecked == true)
            {
                toppings += "Kaas\n";
                toppingTotalPrice += toppingPrice[1];
            }
            if (chkTopping3.IsChecked == true)
            {
                toppings += "Groentjes\n";
                toppingTotalPrice += toppingPrice[2];
            }
            else if (chkTopping1.IsChecked == false && chkTopping2.IsChecked == false && chkTopping3.IsChecked == false)
            {
                toppings += "Geen Beleg\n";
                toppingTotalPrice = toppingPrice[3];
            }

            //berekening en de prijs toevoegen in een list<>
            breadPlusToppingPrice = sandwichTotalPrice + toppingTotalPrice;
            sumBreadAndTopping.Add(breadPlusToppingPrice);

            //listbox vullen alvorens de bestelling word betaald
            lstOrderSummary.Items.Add($"Klantnaam: {customerName} \nBroodje: {sandwichType}\nBeleg: \n{toppings}");
            TblTotaal.Text = breadPlusToppingPrice.ToString();

            //user friendly maken (minder kans op verkeerde button kliks
            btnOrder.IsEnabled = false;
            btnPay.IsEnabled = true;
            tbkError.Text = string.Empty;
        }

    

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderSummaryAndCalculatePrice();
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            string customerName = txtCustomerName.Text;

            //gaat de listbox van de betaling doorvoeren naar de listbox van de bestellingen + update de hoeveelheid verkochte broodjes
            lstOrderSummary.Items.Clear();
            TblTotaal.Text = "€ 0";
            LstBestellingen.Items.Add($" {customerName} - €{breadPlusToppingPrice}");

            sandwichesSold++;

            //de button remove mag maar beschikbaar zijn na het toevoegen van de eerste beselling (user friendly)
            if (LstBestellingen.Items.Count > 1)
            {
                btnRemoveOrder.IsEnabled = true;
            }
            else
            {
                btnRemoveOrder.IsEnabled = false;
            }

            //toevoegen Methodes die de statistieken en alles reset na drukken op de knop 'betaal'
            MakeStatistics();
            ResetAfterPay();
        }

        //Methode om Alles bij te houden en te berekening in het statistiek deeltje
        private void MakeStatistics()
        {

            revenue = sumBreadAndTopping.Sum();
            revenuePerSandwich = Math.Round(revenue, 2) / sandwichesSold;
            tblRevenue.Text = revenue.ToString();
            tblAmountSold.Text = sandwichesSold.ToString();
            tblRevenueAvarage.Text = Math.Round(revenuePerSandwich, 2).ToString();
        }

        //maakt het wat user friendly + reset de toppingprice naar 0
        void ResetAfterPay()
        {
            txtCustomerName.Text = string.Empty;
            chkTopping1.IsChecked = false;
            chkTopping2.IsChecked = false;
            chkTopping3.IsChecked = false;
            cmbSandwiches.SelectedIndex = 0;

            txtCustomerName.Focus();

            btnPay.IsEnabled = false;
            btnOrder.IsEnabled = true;

            toppingTotalPrice = 0;
        }

        private void BtnEndDay_Click(object sender, RoutedEventArgs e)
        {
            //een knop om de 'dag af te sluiten', reset het volledige programma en zet terug op begin waarden
            LstBestellingen.Items.Clear();
            sumBreadAndTopping.Clear();
            sandwichesSold = 0;
            tblRevenue.Text = "€ 0";
            tblAmountSold.Text = "0";
            tblRevenueAvarage.Text = "€ 0";
        }

        private void btnRemoveOrder_Click(object sender, RoutedEventArgs e)
        {
            //verwijderd een specifieke bestelling in de lijst bestellingen
            int index = LstBestellingen.SelectedIndex;

            if (LstBestellingen.Items.Count > 1)
            {
                try
                {
                    sumBreadAndTopping.RemoveAt(index);
                    LstBestellingen.Items.RemoveAt(index);
                    sandwichesSold--;
                    MakeStatistics();
                }
                catch (DivideByZeroException ex)
                {
                    MessageBox.Show("Kan laatste bestelling niet verwijderen", "Fout!", MessageBoxButton.OK, MessageBoxImage.Error) ;
                }
            }
            else
            {
                MessageBox.Show("voeg een nieuwe bestelling toe alvorens de laatste bestelling te verwijderen", "Fout bij verwijderen!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
        }



    }
}
