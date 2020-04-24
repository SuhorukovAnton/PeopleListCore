using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;
using PeopleListCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PeopleListCore.Helpers
{
    public static class HelperConnect
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static DbContextOptions<PeopleContext> options;

        static HelperConnect()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string connectionString = config.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<PeopleContext>();

            options = optionsBuilder
                    .UseMySql(connectionString)
                    .Options;
        }
        public async static Task AddPeople(FormAdd formAdd)
        {
            using (var db = new PeopleContext(options))
            {
                var people = new People() { Name = formAdd.Name, Surname = formAdd.Surname, Email = formAdd.Email, Password = formAdd.Password, Birthday = formAdd.Birthday, Role = Roles.User };
                try
                {
                    db.People.Add(people);
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    logger.Info("Wrong person format: " + e.Message);
                }
            }
        }
        public async static Task AddPeople(People people)
        {
            using (var db = new PeopleContext(options))
            {
                try
                {
                    if (people.Password == null)
                    {
                        people.Password = HelperWorkWithData.GetHash("123");
                    }
                    db.People.Add(people);
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    logger.Info("Wrong person format: " + e.Message);
                }
            }
        }
        public async static Task RemovePeople(int id)
        {
            using (var db = new PeopleContext(options))
            {
                var people = db.People.FirstOrDefault(p => p.id == id);
                if (people != null)
                {
                    db.People.Remove(people);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async static Task<People> FindUser(string email, string password)
        {
            using (var db = new PeopleContext(options))
            {
                var passwordHash = HelperWorkWithData.GetHash(password);

                return await db.People.FirstOrDefaultAsync(p => p.Password == passwordHash && p.Email == email);
            }
        }

        public async static Task<bool> FindEmail(string email)
        {
            using (var db = new PeopleContext(options))
            {
                var people = await db.People.FirstOrDefaultAsync(p => p.Email == email);
                return people != null;
            }
        }

        public static List<People> GetPeoples()
        {
            using (var db = new PeopleContext(options))
            {
                return new List<People>(db.People);
            }
        }
        public async static Task<People> GetPeople(int id)
        {
            using (var db = new PeopleContext(options))
            {
                var people = await db.People.FirstOrDefaultAsync(p => p.id == id);
                if (people != null)
                {
                    return people;
                }
                return null;
            }
        }
        public async static Task<People> GetPeople(string email)
        {
            using (var db = new PeopleContext(options))
            {
                var people = await db.People.FirstOrDefaultAsync(p => p.Email == email);
                if (people != null)
                {
                    return people;
                }
                return null;
            }
        }

        public async static Task EditPeople(FormEdit formEdit)
        {
            using (var db = new PeopleContext(options))
            {
                var people = await db.People.FirstOrDefaultAsync(p => p.id == formEdit.Id);
                if (people != null)
                {
                    people.Name = formEdit.Name;
                    people.Surname = formEdit.Surname;
                    people.Email = formEdit.Email;
                    people.Birthday = formEdit.Birthday;

                    db.Entry(people).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            }
        }

        public async static Task AddImg(int id, string file)
        {
            using (var db = new PeopleContext(options))
            {
                var people = await db.People.FirstOrDefaultAsync(p => p.id == id);
                if (people != null)
                {
                    people.Img = file;
                    db.Entry(people).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
