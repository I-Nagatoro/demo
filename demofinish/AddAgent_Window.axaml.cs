using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using demofinish.Models;

namespace demofinish
{
    public partial class AddAgent_Window : Window
    {
        public AddAgent_Window()
        {
            InitializeComponent();
            LoadAgentTypes();
        }

        private void LoadAgentTypes()
        {
            using var context = new User1Context();
            var agentTypes = context.Agenttypes.ToList();
            TypeAgentCombobox.ItemsSource = agentTypes;
            if (agentTypes.Any())
                TypeAgentCombobox.SelectedIndex = 0;
        }

        private void Add_Agent(object? sender, RoutedEventArgs e)
        {
            // Сброс предыдущих ошибок
            ErrorTextBlock.Text = string.Empty;

            // Валидация полей
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                TypeAgentCombobox.SelectedItem is not Agenttype selType ||
                !int.TryParse(PriorityBox.Text, out int priority) ||
                string.IsNullOrWhiteSpace(AddressBox.Text) ||
                InnBox.Text.Length != 10 || !InnBox.Text.All(char.IsDigit) ||
                KppBox.Text.Length != 9 || !KppBox.Text.All(char.IsDigit) ||
                string.IsNullOrWhiteSpace(DirectorBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                !EmailBox.Text.Contains('@') || !EmailBox.Text.Contains('.'))
            {
                ErrorTextBlock.Text = "Проверьте правильность заполнения всех полей";
                return;
            }

            // Создание нового агента
            var newAgent = new Agent
            {
                Title = NameBox.Text.Trim(),
                Agenttypeid = selType.Id,
                Priority = priority,
                Address = AddressBox.Text.Trim(),
                Inn = InnBox.Text.Trim(),
                Kpp = KppBox.Text.Trim(),
                Directorname = DirectorBox.Text.Trim(),
                Phone = PhoneBox.Text.Trim(),
                Email = EmailBox.Text.Trim(),
                Logo = "picture.png"
            };

            // Сохранение в базу
            using var ctx = new User1Context();
            ctx.Agents.Add(newAgent);
            ctx.SaveChanges();

            // Закрыть окно
            Close();
        }

        private void GoBack_Button(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}