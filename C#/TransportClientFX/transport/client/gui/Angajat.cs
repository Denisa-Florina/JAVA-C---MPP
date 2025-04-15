using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TransportModel.transport.model;
using TransportServices.transport.services;


namespace proiect.view
{
    public partial class Angajat : Form, ITransportObserver
    {
        private ITransportServices server;
        private User LoggedUser;

        public Angajat(ITransportServices service)
        {
            InitializeComponent();
            this.server = service;
        }
        
        public void SetUser(User user)
        {
            this.LoggedUser = user;
            LoadFlights();
        }

        private void LoadFlights()
        {
            listView1.View = View.Details; 
            listView1.Columns.Clear();
            listView1.Columns.Add("Destination", 150); 
            listView1.Columns.Add("Departure Time", 150);
            listView1.Columns.Add("Available Seats", 120);
            
            List<Flight> allFlights = server.GetAllFlights().ToList();
            listView1.Items.Clear();
            foreach (var flight in allFlights)
            {

                var listViewItem = new ListViewItem(flight.Destination);
                listViewItem.SubItems.Add(flight.DepartureTime.ToString());
                listViewItem.SubItems.Add(flight.AvailableSeats.ToString());

       
                listView1.Items.Add(listViewItem);
            }

        }

        private void Search_Click(object sender, EventArgs e)
        {
            string destination = textBox1.Text;
            DateTime date = dateTimePicker1.Value; 

            if (string.IsNullOrEmpty(destination))
            {
                MessageBox.Show("Destination is required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<Flight> flights = server.SearchFlights(destination, date).ToList();
            listView1.Items.Clear();

            foreach (var flight in flights)
            {
                var listViewItem = new ListViewItem(flight.Destination);
                listViewItem.SubItems.Add(flight.DepartureTime.ToString());
                listViewItem.SubItems.Add(flight.AvailableSeats.ToString());

                listView1.Items.Add(listViewItem);
            }

            if (flights.Count == 0)
            {
                MessageBox.Show("No flights found!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void Buy_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a flight!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            string selectedDestination = listView1.SelectedItems[0].SubItems[0].Text;
            int selectedDepartureTime = int.Parse(listView1.SelectedItems[0].SubItems[2].Text);
    
            int numberOfSeats;
            if (!int.TryParse(seats.Text, out numberOfSeats))
            {
                MessageBox.Show("Select a valid number of seats!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> passengers = Passagers.Text.Split(',')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();

            if (passengers.Count == 0)
            {
                MessageBox.Show("Enter at least one passenger name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Apelează serviciul pentru a cumpăra biletele
            bool success = server.BuyTicket(selectedDestination, selectedDepartureTime, int.Parse(seats.Text), passengers);
    
            if (success)
            {
                MessageBox.Show("Ticket purchased successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadFlights();
            }
            else
            {
                MessageBox.Show("Not enough seats available!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ReservationAdded(object obj)
        {
            this.BeginInvoke((MethodInvoker) delegate
            {
                Console.WriteLine("NOTIFY");
                LoadFlights();
            });
        }
    }
}