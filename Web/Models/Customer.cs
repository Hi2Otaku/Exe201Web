using System;
using System.Collections.Generic;

namespace Web.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public bool? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? DateofBirth { get; set; }

    public virtual User IdNavigation { get; set; } = null!;
}
