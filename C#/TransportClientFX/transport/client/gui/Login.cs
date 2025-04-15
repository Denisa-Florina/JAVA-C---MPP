
using Microsoft.VisualBasic.ApplicationServices;
using TransportServices.transport.services;
using User = TransportModel.transport.model.User;


namespace proiect.view
{
    public partial class Login : Form
    {
        private Angajat MainForm;
        private TransportModel.transport.model.User LoggedUser;
        private ITransportServices Server;

        public void SetServer(ITransportServices server)
        {
            this.Server = server;
        }
        
        public void SetMainForm(Angajat mainForm)
        {
            this.MainForm = mainForm;
        }
        public Login()
        {
            InitializeComponent();
            
        }

        private void Log_Click(object sender, EventArgs e)
        {
            string usernameInput = username.Text.Trim();
            string passwordInput = password.Text.Trim();

            if (string.IsNullOrEmpty(usernameInput) || string.IsNullOrEmpty(passwordInput))
            {
                MessageBox.Show("Both fields are required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            
            if (!Server.Login(usernameInput, passwordInput, MainForm))
            {
                MessageBox.Show("Invalid username or password!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            this.Hide();
            User user = new User(usernameInput, passwordInput);
            MainForm.SetUser(user);
            MainForm.Text = "Angajat: " + user.Username;
            MainForm.ShowDialog();
        }
    }
}
