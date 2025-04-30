using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using demofinish.Models;

namespace demofinish
{
    public partial class EditWindow : Window
    {
        private readonly MainWindow.AgentPresenter _selectedAgent;
        private readonly User1Context _context;
        
        public EditWindow()
        {
            InitializeComponent();
        }

        public EditWindow(MainWindow.AgentPresenter selectedAgent)
        {
            InitializeComponent();
            _selectedAgent = selectedAgent;
            _context = new User1Context();
            LoadAgentData();
        }

        private void LoadAgentData()
        {
            NameBox.Text = _selectedAgent.Title;
            PriorityBox.Text = _selectedAgent.Priority.ToString();
            BossNameBox.Text = _selectedAgent.Directorname;
            InnBox.Text = _selectedAgent.Inn;
            KppBox.Text = _selectedAgent.Kpp;
            PhoneBox.Text = _selectedAgent.Phone;
            EmailBox.Text = _selectedAgent.Email;
            AdressBox.Text = _selectedAgent.Address;

            var types = _context.Agenttypes.ToList();
            AgentTypeBox.ItemsSource = types;
            AgentTypeBox.SelectedItem = types.FirstOrDefault(t => t.Id == _selectedAgent.Agenttypeid);
        }

        private void BackButton(object? sender, RoutedEventArgs e) => Close();

        private void EditAgent_Button(object? sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                !int.TryParse(PriorityBox.Text, out int priority) ||
                string.IsNullOrWhiteSpace(BossNameBox.Text) ||
                InnBox.Text.Length != 10 || !InnBox.Text.All(char.IsDigit) ||
                KppBox.Text.Length != 9 || !KppBox.Text.All(char.IsDigit) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                !EmailBox.Text.Contains('@') || !EmailBox.Text.Contains('.') ||
                string.IsNullOrWhiteSpace(AdressBox.Text) ||
                AgentTypeBox.SelectedItem is not Agenttype selType)
            {
                ErrorTextBlock.Text = "Проверьте правильность заполнения всех полей";
                return;
            }

            _selectedAgent.Title = NameBox.Text.Trim();
            _selectedAgent.Priority = priority;
            _selectedAgent.Directorname = BossNameBox.Text.Trim();
            _selectedAgent.Inn = InnBox.Text.Trim();
            _selectedAgent.Kpp = KppBox.Text.Trim();
            _selectedAgent.Phone = PhoneBox.Text.Trim();
            _selectedAgent.Email = EmailBox.Text.Trim();
            _selectedAgent.Address = AdressBox.Text.Trim();
            _selectedAgent.Agenttypeid = selType.Id;

            var dbAgent = _context.Agents.FirstOrDefault(a => a.Id == _selectedAgent.Id);
            if (dbAgent != null)
            {
                dbAgent.Title = _selectedAgent.Title;
                dbAgent.Priority = _selectedAgent.Priority;
                dbAgent.Directorname = _selectedAgent.Directorname;
                dbAgent.Inn = _selectedAgent.Inn;
                dbAgent.Kpp = _selectedAgent.Kpp;
                dbAgent.Phone = _selectedAgent.Phone;
                dbAgent.Email = _selectedAgent.Email;
                dbAgent.Address = _selectedAgent.Address;
                dbAgent.Agenttypeid = _selectedAgent.Agenttypeid;

                _context.SaveChanges();
            }

            Close();
        }
    }
}