using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW_4_30_EFCoreWeb.Data
{
    public class ImageRepository
    {
        private readonly string _connectionString;

        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Image> GetAllImages()
        {
            using var ctx = new ImageDataContext(_connectionString);
            return ctx.Images.OrderByDescending(image => image.DateUploaded).ToList();
        }

        public Image GetById(int id)
        {
            using var ctx = new ImageDataContext(_connectionString);
            return ctx.Images.FirstOrDefault(p => p.Id == id);
        }

        public void AddImage(Image image)
        {
            using var ctx = new ImageDataContext(_connectionString);
            ctx.Images.Add(image);
            ctx.SaveChanges();
        }

        public void IncreaseLikes(int id)
        {
            using var ctx = new ImageDataContext(_connectionString);
            ctx.Database.ExecuteSqlInterpolated($"UPDATE Images SET Likes = Likes+1 WHERE Id = {id}");
        }

        public int GetLikes(int id)
        {
            using var ctx = new ImageDataContext(_connectionString);
            return ctx.Images.FirstOrDefault(p => p.Id == id).Likes;
        }

    }
}
