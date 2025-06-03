﻿using System;
using System.Collections.Generic;

namespace Web.Models;
public partial class UserDTO
{
	public int Id { get; set; }

	public string UserName { get; set; } = null!;

	public string Password { get; set; } = null!;



	public string Email { get; set; } = null!;

	public string PhoneNumber { get; set; }
	public bool Gender { get; set; }
	public DateTime DoB { get; set; }
}
public partial class User
{
	public int Id { get; set; }

	public string UserName { get; set; } = null!;

	public string Password { get; set; } = null!;

	public string PasswordHash { get; set; } = null!;

	public string Email { get; set; } = null!;

	public bool? Active { get; set; }

	public virtual Admin? Admin { get; set; }

	public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

	public virtual Customer? Customer { get; set; }

	public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

