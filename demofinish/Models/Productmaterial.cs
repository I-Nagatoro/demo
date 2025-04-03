using System;
using System.Collections.Generic;

namespace demofinish.Models;

public partial class Productmaterial
{
    public int Productid { get; set; }

    public int Materialid { get; set; }

    public decimal? Count { get; set; }

    public virtual Material Material { get; set; } = null!;
}
