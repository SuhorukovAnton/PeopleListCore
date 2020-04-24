using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PeopleListCore.Models;

namespace PeopleListCore.Helpers
{
    public static class HelperWorkWithData
    {
        private static byte[] salt;
        static HelperWorkWithData()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string str = config["salt"];
            salt = Encoding.UTF8.GetBytes(str);
        }

        public static string GetHash(string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));
        }

        public static async Task<string> Save(this IFormFile img, int id, IWebHostEnvironment appEnvironment)
        {
            string path = id + Path.GetExtension(img.FileName);
            using (var fileStream = new FileStream(appEnvironment.WebRootPath + "/files/imgs/" + path, FileMode.Create))
            {
                await img.CopyToAsync(fileStream);
            }
            return path;
        }

        public static async Task<string> Save(this IFormFile file, IWebHostEnvironment appEnvironment)
        {
            string path = "peoples" + Path.GetExtension(file.FileName);
            using (var fileStream = new FileStream(appEnvironment.WebRootPath + "/files/download/" + path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return path;
        }

        public static string TransformDate(string date)
        {
            date = date.Split(' ')[0];
            var tmp = date.Split('.');
            if (tmp.Length >= 3)
            {
                return tmp[2] + "-" + tmp[1] + "-" + tmp[0];
            }
            else return null;
        }
        public static string FirstUpper(string str)
        {
            return str.Substring(0, 1).ToUpper() + (str.Length > 1 ? str.Substring(1) : "");
        }

    }
}
