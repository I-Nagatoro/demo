using System;
using System.Collections.Generic;

namespace demofinish.Models;

public partial class ProductType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public double? DefectedPercent { get; set; }
}
