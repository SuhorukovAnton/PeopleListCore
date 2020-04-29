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
    public class HelperWorkWithData
    {
        private byte[] salt;
        private IWebHostEnvironment appEnvironment;
        public HelperWorkWithData(IConfiguration config, IWebHostEnvironment appEnvironment)
        {
            salt = Encoding.UTF8.GetBytes(config["salt"]);
            this.appEnvironment = appEnvironment;
        }

        public string GetHash(string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));
        }

        public async Task<string> Save(IFormFile img, int id)
        {
            string path = id + Path.GetExtension(img.FileName);
            using (var fileStream = new FileStream(appEnvironment.WebRootPath + "/files/imgs/" + path, FileMode.Create))
            {
                await img.CopyToAsync(fileStream);
            }
            return path;
        }

        public async Task<string> Save(IFormFile file)
        {
            string path = "peoples" + Path.GetExtension(file.FileName);
            using (var fileStream = new FileStream(appEnvironment.WebRootPath + "/files/download/" + path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return path;
        }

        public  string TransformDate(string date)
        {
            date = date.Split(' ')[0];
            var tmp = date.Split('.');
            if (tmp.Length >= 3)
            {
                return tmp[2] + "-" + tmp[1] + "-" + tmp[0];
            }
            else return null;
        }
        public  string FirstUpper(string str)
        {
            return str.Substring(0, 1).ToUpper() + (str.Length > 1 ? str.Substring(1) : "");
        }

    }
}
