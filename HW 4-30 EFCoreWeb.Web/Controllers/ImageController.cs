using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HW_4_30_EFCoreWeb.Web.Models;
using HW_4_30_EFCoreWeb.Data;
using System.Text.Json;

namespace HW_4_30_EFCoreWeb.Web.Controllers;

public class ImageController : Controller
{
    private readonly string _connectionString;
    private IWebHostEnvironment _webHostEnvironment;

    public ImageController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        _connectionString = configuration.GetConnectionString("ConStr");
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        var repo = new ImageRepository(_connectionString);
        return View(new IndexViewModel()
        {
            Images = repo.GetAllImages()
        });
    }

    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Upload(IFormFile file, string title)
    {
        var fileName = $"{Guid.NewGuid()}-{file.FileName}";
        var fullImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

        using FileStream fs = new FileStream(fullImagePath, FileMode.Create);
        file.CopyTo(fs);
        var repo = new ImageRepository(_connectionString);
        var img = new Image { ImagePath = fileName, Title = title, DateUploaded = DateTime.Now };

        repo.AddImage(img);

        return Redirect("/");
    }

    public IActionResult ViewImage(int id)
    {
        var repo = new ImageRepository(_connectionString);
        var vm = new ViewImageViewModel();
        vm.Image = repo.GetById(id);
        vm.MyLiked = HttpContext.Session.Get<List<int>>("myLiked");
        return View(vm);
    }

    [HttpPost]
    public void UpdateLikes(int imageId)
    {
        var myLiked = HttpContext.Session.Get<List<int>>("myLiked");
        if (myLiked == null)
        {
            myLiked = new List<int>();
        }

        if (!myLiked.Contains(imageId))
        {
            myLiked.Add(imageId);
            HttpContext.Session.Set<List<int>>("myLiked", myLiked);
            var repo = new ImageRepository(_connectionString);
            repo.IncreaseLikes(imageId);
        }
    }

    public int GetLikes(int imageId)
    {
        var repo = new ImageRepository(_connectionString);
        return repo.GetLikes(imageId);
    }
}

public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T Get<T>(this ISession session, string key)
    {
        string value = session.GetString(key);

        return value == null ? default(T) :
            JsonSerializer.Deserialize<T>(value);
    }
}
