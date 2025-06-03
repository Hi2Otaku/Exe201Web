using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Web.Models;
using Microsoft.AspNetCore.Identity;

public class LoginController : Controller
{
	private readonly WebContext _context;

	public LoginController(WebContext context)
	{
		_context = context;
	}
	[HttpGet]
	public IActionResult Index()
	{
		return View();
	}

	[HttpGet]
	public IActionResult Register()
	{
		return View();
	}
	[HttpPost]
	public IActionResult Index(string identifier, string password)
	{
		if (string.IsNullOrEmpty(identifier) || string.IsNullOrEmpty(password))
		{
			ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
			return View();
		}

		string passwordHash = GetSha256Hash(password);

		var user = _context.Users
			.Include(u => u.Customer)
			.FirstOrDefault(u =>
				(u.Email == identifier ||
				 (u.Customer != null && u.Customer.PhoneNumber == identifier)) &&
				u.PasswordHash == passwordHash &&
				u.Active == true);

		if (user == null)
		{
			ViewBag.Error = "Email/SĐT hoặc mật khẩu không đúng.";
			return View();
		}

		HttpContext.Session.SetString("UserId", user.Id.ToString());
		HttpContext.Session.SetString("UserName", user.UserName);

		return RedirectToAction("Index", "Home");
	}

	// Bắt đầu đăng nhập với Google
	public IActionResult GoogleLogin()
	{
		var properties = new AuthenticationProperties
		{
			RedirectUri = Url.Action("GoogleResponse")
		};
		return Challenge(properties, GoogleDefaults.AuthenticationScheme);
	}

	public async Task<IActionResult> GoogleResponse()
	{
		var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

		if (!result.Succeeded)
		{
			return RedirectToAction("Index", "Home");
		}

		var claims = result.Principal?.Identities?.FirstOrDefault()?.Claims;
		string email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
		string name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

		if (string.IsNullOrEmpty(email))
		{
			return RedirectToAction("Index", "Home");
		}

		// Tìm user trong hệ thống
		var existingUser = await _context.Users
			.Include(u => u.Customer)
			.FirstOrDefaultAsync(u => u.Email == email);

		if (existingUser == null)
		{
			// Tạo User mới
			var newUser = new User
			{
				UserName = name ?? email.Split('@')[0],
				Password = "123",
				Email = email,
				Active = true
			};

			// Hash mật khẩu giả (do cần trường PasswordHash)
			var passwordHasher = new PasswordHasher<User>();
			newUser.PasswordHash = passwordHasher.HashPassword(newUser, "DefaultPasswordForGoogleLogin");

			_context.Users.Add(newUser);
			await _context.SaveChangesAsync(); // Phải lưu trước để có Id

			// Tạo Customer gắn với User
			var newCustomer = new Customer
			{
				Id = newUser.Id,                  // Liên kết 1-1 với User
				FullName = newUser.UserName,
				Gender = null,
				PhoneNumber = null,
				DateofBirth = null,
				Address = null
			};

			_context.Customers.Add(newCustomer);
			await _context.SaveChangesAsync(); // Lưu Customer

			existingUser = newUser; // Gán lại để dùng dưới
		}

		// Lưu thông tin đăng nhập vào session
		HttpContext.Session.SetString("UserId", existingUser.Id.ToString());
		HttpContext.Session.SetString("UserName", existingUser.UserName);

		return RedirectToAction("Index", "Home");
	}



	[HttpPost]
	public IActionResult Register(UserDTO model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		if (_context.Users.Any(u => u.Email == model.Email || u.UserName == model.PhoneNumber))
		{
			ModelState.AddModelError("", "Email hoặc số điện thoại đã tồn tại.");
			return View(model);
		}

		string passwordHash = GetSha256Hash(model.Password);

		var user = new User
		{
			UserName = model.UserName,
			Password = model.Password,
			PasswordHash = passwordHash,
			Email = model.Email,
			Active = true
		};

		_context.Users.Add(user);
		_context.SaveChanges();

		var customer = new Customer
		{
			Id = user.Id,
			FullName = model.UserName,
			Gender = model.Gender,
			PhoneNumber = model.PhoneNumber,
			DateofBirth = model.DoB,
			Address = null
		};

		_context.Customers.Add(customer);
		_context.SaveChanges();

		return RedirectToAction("Index", "Login");
	}
	private string GetSha256Hash(string input)
	{
		using var sha256 = SHA256.Create();
		var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
		return BitConverter.ToString(bytes).Replace("-", "").ToLower();
	}
}
