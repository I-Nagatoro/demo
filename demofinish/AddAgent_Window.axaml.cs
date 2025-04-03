using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using demofinish.Models;

namespace demofinish;

public partial class AddAgent_Window : Window
{
    public AddAgent_Window()
    {
        InitializeComponent();
        LoadAgentTypes();
    }

    private void LoadAgentTypes()
    {
        try
        {
            using var context = new User1Context();
            var agentTypes = context.Agenttypes.ToList();
            TypeAgentCombobox.ItemsSource = agentTypes;
            
            if (agentTypes.Any())
                TypeAgentCombobox.SelectedIndex = 0;
        }
        catch
        {
            this.Close();
        }
    }

    private void Add_Agent(object? sender, RoutedEventArgs e)
    {
        try
        {
            using var context = new User1Context();
            
            
            if (TypeAgentCombobox.SelectedItem is not Agenttype selectedType)
                return;

            
            int.TryParse(PrioritryBox.Text, out int priority);

            var newAgent = new Agent()
            {
                Title = NameBox.Text,
                Inn = InnBox.Text,
                Agenttypeid = selectedType.Id,
                Kpp = KppBox.Text,
                Email = EmailBox.Text,
                Phone = PhoneBox.Text,
                Address = AdressBox.Text,
                Priority = priority,
                Directorname = DirectorBox.Text,
                Logo = "picture.png"
            };
            
            context.Agents.Add(newAgent);
            context.SaveChanges();

            
            ClearFields();
            this.Close();
        }
        catch
        {
            this.Close();
        }
    }

    private void ClearFields()
    {
        NameBox.Text = "";
        InnBox.Text = "";
        KppBox.Text = "";
        EmailBox.Text = "";
        PhoneBox.Text = "";
        AdressBox.Text = "";
        PrioritryBox.Text = "";
        DirectorBox.Text = "";
    }

    private void GoBack_Button(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}