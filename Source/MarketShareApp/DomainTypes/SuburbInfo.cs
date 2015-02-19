using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class SuburbInfo
{
    public int SuburbId { get; set; }
    public string SuburbName { get; set; }
    public bool CanEdit { get; set; }
    public bool Fated { get; set; }
    public int? FatedCount { get; set; }
    public int? UnfatedCount { get; set; }
    public int SeeffCurrentListingCount { get; set; }
}