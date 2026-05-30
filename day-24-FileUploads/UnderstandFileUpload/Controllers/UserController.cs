using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs; 
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
        _context.Database.EnsureCreated();
    }

    [HttpPost("single")]
    public async Task<IActionResult> AddSingleUser([FromBody] User user)
    {
        if (user == null) return BadRequest("Invalid user data.");

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User added successfully!" });
    }

  [HttpPost("upload-users")]
public async Task<IActionResult> BulkUploadUsers(IFormFile file)
{
    if (file == null || file.Length == 0) 
        return BadRequest("Please upload a valid Excel file.");

    var importedUsers = new List<User>();

    using (var stream = new MemoryStream())
    {
        await file.CopyToAsync(stream);
        stream.Position = 0; 

        var rows = stream.Query(useHeaderRow: true); 

        foreach (IDictionary<string, object> row in rows)
        {
            var name = row.ContainsKey("Name") ? row["Name"]?.ToString() : null;
            var email = row.ContainsKey("Email") ? row["Email"]?.ToString() : null;
            
            string? ph = null;
            if (row.ContainsKey("Ph"))
            {
                ph = row["Ph"]?.ToString();
            }
            else if (row.ContainsKey("Phone"))
            {
                ph = row["Phone"]?.ToString();
            }

            int age = 0;
            if (row.ContainsKey("Age") && row["Age"] != null)
            {
                int.TryParse(row["Age"].ToString(), out age);
            }

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(ph))
            {
                importedUsers.Add(new User
                {
                    Name = name,
                    Ph = ph,
                    Email = email==null?string.Empty:email,
                    Age = age
                });
            }
        }
    }

    if (importedUsers.Count > 0)
    {
        _context.Users.AddRange(importedUsers);
        await _context.SaveChangesAsync();
        return Ok(new { message = $"{importedUsers.Count} users imported successfully from Excel!" });
    }

    return BadRequest("No valid user data found. Ensure your headers include Name and either Ph or Phone.");
}

    [HttpGet("download")]
    public async Task<IActionResult> DownloadAllAsExcel()
    {
        var users = await _context.Users.ToListAsync();

        var memoryStream = new MemoryStream();
        
        memoryStream.SaveAs(users);
        memoryStream.Position = 0;

        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        string fileName = "Users_Export.xlsx";

        return File(memoryStream, contentType, fileName);
    }
}