using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using demofinish.Models;
using Avalonia.Controls.ApplicationLifetimes;
using System;

namespace demofinish;

public partial class EditWindow : Window
{
    private readonly MainWindow.AgentPresenter _selectedAgent;
    private readonly User1Context _context;
    
    public EditWindow()
    {
        InitializeComponent();
    }

    public EditWindow(MainWindow.AgentPresenter selectedAgent) : this()
    {
        _selectedAgent = selectedAgent;
        _context = new User1Context();
        
        InitializeComponent();
        LoadAgentData();
    }

    private void LoadAgentData()
    {
        
        NameBox.Text = _selectedAgent.Title;
        AdressBox.Text = _selectedAgent.Address;
        PhoneBox.Text = _selectedAgent.Phone;
        EmailBox.Text = _selectedAgent.Email;
        InnBox.Text = _selectedAgent.Inn;
        KppBox.Text = _selectedAgent.Kpp;
        BossNameBox.Text = _selectedAgent.Directorname;
        PriorityBox.Text = _selectedAgent.Priority.ToString();

        
        var agentTypes = _context.Agenttypes.ToList();
        AgentTypeBox.ItemsSource = agentTypes;
        
        
        var currentType = agentTypes.FirstOrDefault(at => at.Id == _selectedAgent.Agenttypeid);
        AgentTypeBox.SelectedItem = currentType;
    }

    private void BackButton(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void EditAgent_Button(object? sender, RoutedEventArgs e)
    {
        

            _selectedAgent.Title = NameBox.Text;
            _selectedAgent.Address = AdressBox.Text;
            _selectedAgent.Phone = PhoneBox.Text;
            _selectedAgent.Email = EmailBox.Text;
            _selectedAgent.Inn = InnBox.Text;
            _selectedAgent.Kpp = KppBox.Text;
            _selectedAgent.Directorname = BossNameBox.Text;

            if (int.TryParse(PriorityBox.Text, out int priority))
            {
                _selectedAgent.Priority = priority;
            }



            if (AgentTypeBox.SelectedItem is Agenttype selectedType)
            {
                _selectedAgent.Agenttypeid = selectedType.Id;
            }


            var dbAgent = _context.Agents.FirstOrDefault(a => a.Id == _selectedAgent.Id);
            if (dbAgent != null)
            {
                dbAgent.Title = _selectedAgent.Title;
                dbAgent.Address = _selectedAgent.Address;
                dbAgent.Phone = _selectedAgent.Phone;
                dbAgent.Email = _selectedAgent.Email;
                dbAgent.Inn = _selectedAgent.Inn;
                dbAgent.Kpp = _selectedAgent.Kpp;
                dbAgent.Directorname = _selectedAgent.Directorname;
                dbAgent.Priority = _selectedAgent.Priority;
                dbAgent.Agenttypeid = _selectedAgent.Agenttypeid;

                await _context.SaveChangesAsync();


                Close();
            }
    }
    
}