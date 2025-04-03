using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using demofinish.Models;
using Microsoft.EntityFrameworkCore;

namespace demofinish
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Agent> agents = new ObservableCollection<Agent>();
        public List<AgentPresenter> agentsList = new List<AgentPresenter>(); 
        private const int pageSize = 10;
        private int currentPage = 1;
        private int pageCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            LoadAgents();
            ComboboxAgentType();
            SearchFiels();
            TypeAgentCombobox.SelectionChanged += TypeAgentCombobox_SelectionChanged;
            NameComboBox.SelectionChanged += SortName_SelectedChanged;
            PriorityCombobox.SelectionChanged += PrioritySort_SelectedChanged;
            SaleCombobox.SelectionChanged += SaleSort_SelectedChanged;
            
            
        }

        private void SearchFiels()
        {
            using var context = new User1Context();
            
            var searchItems = context.Agents
                .Select(a => a.Title)
                .Union(context.Agents.Select(a => a.Phone))
                .Union(context.Agents.Select(a => a.Email))
                .Where(x => !string.IsNullOrEmpty(x)) 
                .ToList();
    
            SearchBox.ItemsSource = searchItems;
            SearchBox.SelectionChanged += SearchBox_SelectionChanged;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
            {
                currentPage = 1; 
                AgentListBox.ItemsSource = agents;
                ShowCurrentPage();
            }
        }
        
        private void SearchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchBox.SelectedItem == null)
            {
                AgentListBox.ItemsSource = agentsList;
                ShowCurrentPage();
                return;
            }

            string searchValue = SearchBox.SelectedItem.ToString();

            var filtered = agentsList
                .Where(a => a.Title == searchValue || 
                            a.Phone == searchValue || 
                            a.Email == searchValue)
                .ToList();

            AgentListBox.ItemsSource = filtered;
        }
        


        public class AgentPresenter : Agent
        {
            public string AgentTypeName { get; set; }
            public int DiscountPercent { get; set; }
            public decimal TotalSales { get; set; }

            Bitmap? LogoImage
            {
                get
                {
                    try
                    {
                        string absolutepass = Path.Combine(AppContext.BaseDirectory, Logo);
                        return new Bitmap(absolutepass);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }

        private void ComboboxAgentType()
        {
            using var context = new User1Context();

            var type = context.Agenttypes.Select(x => x.Title).ToList();
    
            type.Insert(0, "Все типы");
            
            
            TypeAgentCombobox.ItemsSource = type;
            TypeAgentCombobox.SelectedIndex = 0;
        }
        
        
        

        private void TypeAgentCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using var context = new User1Context();
    
            string selectedType = TypeAgentCombobox.SelectedItem.ToString();
            if (TypeAgentCombobox.SelectedItem == null) 
                return;

            if (TypeAgentCombobox.SelectedItem is string && (string)TypeAgentCombobox.SelectedItem == "Все типы")
            {
                LoadAgents();
            }
            else
            {
                var selectedTypeId = context.Agenttypes
                    .Where(at => at.Title == selectedType)
                    .Select(at => at.Id)
                    .FirstOrDefault();

                agentsList = agentsList
                    .Where(x => x.Agenttypeid == selectedTypeId)
                    .ToList();

                currentPage = 1;
                ApplyPagination();
            }
        }

        private void SortName_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (NameComboBox.SelectionBoxItem)
            {
                case null:
                    return;
                case "по возрастанию":
                    agentsList = agentsList.OrderBy(it => it.Title).ToList();
                    break;
                default:
                    agentsList = agentsList.OrderByDescending(it => it.Title).ToList();
                    break;
            }

            currentPage = 1;
            ApplyPagination();
        }

        private void PrioritySort_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {

            switch (PriorityCombobox.SelectionBoxItem)
            {
                case null:
                    return;
                case "по возрастанию":
                    agentsList = agentsList.OrderBy(it => it.Priority).ToList();
                    break;
                default:
                    agentsList = agentsList.OrderByDescending(it => it.Priority).ToList();
                    break;
            }
            
            currentPage = 1;
            ApplyPagination();
        }
        
        private void SaleSort_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {

            switch (SaleCombobox.SelectionBoxItem)
            {
                case null:
                    return;
                case "по возрастанию":
                    agentsList =  agentsList.OrderBy(it => it.DiscountPercent).ToList();
                    break;
                default:
                    agentsList = agentsList.OrderByDescending(it => it.DiscountPercent).ToList();
                    break;
            }
            
            currentPage = 1;
            ApplyPagination();
        }



    private void LoadAgents()
        {
            using var context = new User1Context();
            

            agentsList = context.Agents
                .Include(a => a.Agenttype)
                .Include(a => a.Productsales)
                .ThenInclude(ps => ps.Product)
                .AsEnumerable()
                .Select(agent => 
                {
                    decimal totalSales = agent.Productsales?.Sum(ps => 
                        ps.Productcount * (ps.Product?.Mincostforagent ?? 0)) ?? 0;
                
                    return new AgentPresenter
                    {
                        Id = agent.Id,
                        Title = agent.Title,
                        Agenttypeid = agent.Agenttypeid,
                        AgentTypeName = agent.Agenttype?.Title ?? "Неизвестный тип",
                        Address = agent.Address,
                        Inn = agent.Inn,
                        Kpp = agent.Kpp,
                        Directorname = agent.Directorname,
                        Phone = agent.Phone,
                        Email = agent.Email,
                        Logo = agent.Logo,
                        Priority = agent.Priority,
                        TotalSales = totalSales,
                        DiscountPercent = CalculateDiscount(totalSales) 
                    };
                })
                .ToList();
            
            
            
            foreach (var agent in agentsList)
            {
                agents.Add(agent);
            }
            
            AgentListBox.ItemsSource = agents;
            
            pageCount = (int)Math.Ceiling(agentsList.Count / (double) pageSize);
            ShowCurrentPage();
            ApplyPagination();
        }
    
        private int CalculateDiscount(decimal totalSales)
        {
            if (totalSales >= 500000) return 25;
            if (totalSales >= 150000) return 20;
            if (totalSales >= 50000) return 10;
            if (totalSales >= 10000) return 5;
            return 0;
        }    
    
        private void ApplyPagination()
        {
            pageCount = (int)Math.Ceiling(agentsList.Count / (double)pageSize);
            
            if (currentPage > pageCount && pageCount > 0)
            {
                currentPage = pageCount;
            }
            else if (currentPage < 1)
            {
                currentPage = 1;
            }
            
            var pagedAgents = agentsList
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            agents.Clear();
            foreach (var agent in pagedAgents)
            {
                agents.Add(agent);
            }
            
            AgentListBox.ItemsSource = agents;
        }
    

        private void ShowCurrentPage()
        {
            agents.Clear();
            var pageAgents = agentsList
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize);
    
            foreach (var e in pageAgents)
            {
                agents.Add(e);
            }
    
            pageCount = (int)Math.Ceiling(agentsList.Count / (double)pageSize);
            ApplyPagination();
        }

        private void NextPage()
        {
            if (currentPage < pageCount)
            {
                currentPage++;
                ShowCurrentPage();
            }
        }

        private void PrevPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
                ShowCurrentPage();
            }
        }
        
        
         
        
        
        

        private void PrevPage_Click(object sender, RoutedEventArgs e) => PrevPage();

        private void NextPage_Click(object sender, RoutedEventArgs e) => NextPage();

        private void AddNew_Agent(object? sender, RoutedEventArgs e)
        { 
            new AddAgent_Window().ShowDialog(this);
        }

        private void AgentListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (AgentListBox.SelectedItem is AgentPresenter selectedAgent)
            {
                var editWindow = new EditWindow(selectedAgent);
                editWindow.ShowDialog(this);
                
            }
        }
    }
}