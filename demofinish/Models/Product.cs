using System;
using System.Collections.Generic;

namespace demofinish.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? Producttypeid { get; set; }

    public string Articlenumber { get; set; } = null!;

    public string? Description { get; set; }

    public string? Image { get; set; }

    public int? Productionpersoncount { get; set; }

    public decimal Mincostforagent { get; set; }

    public int? Productionworksshopnumber { get; set; }
}
