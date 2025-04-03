using System;
using System.Collections.Generic;

namespace demofinish.Models;

public partial class Productcosthistory
{
    public int Id { get; set; }

    public int Productid { get; set; }

    public DateTime Changedate { get; set; }

    public decimal Costvalue { get; set; }
}
