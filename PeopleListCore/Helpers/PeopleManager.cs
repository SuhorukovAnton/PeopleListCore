using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;
using PeopleListCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeopleListCore.Helpers
{
	public class PeopleManager
	{
        private Logger logger;
        private PeopleContext db;
        private HelperWorkWithData helperWork;
        public PeopleManager(PeopleContext db, HelperWorkWithData helperWork)
        {
            this.helperWork = helperWork;
            logger = LogManager.GetCurrentClassLogger();
            this.db = db;
        }

        public async  Task AddPeople(FormAdd formAdd)
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
        public async  Task AddPeople(People people)
        {
            try
            {
                if (people.Password == null)
                {
                    people.Password = helperWork.GetHash("123");
                }
                db.People.Add(people);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.Info("Wrong person format: " + e.Message);
            }

        }
        public async  Task RemovePeople(int id)
        {
            var people = db.People.FirstOrDefault(p => p.id == id);
            if (people != null)
            {
                db.People.Remove(people);
                await db.SaveChangesAsync();
            }
        }

        public async  Task<People> FindUser(string email, string password)
        {

            var passwordHash = helperWork.GetHash(password);

            return await db.People.FirstOrDefaultAsync(p => p.Password == passwordHash && p.Email == email);

        }

        public async  Task<bool> FindEmail(string email)
        {
            var people = await db.People.FirstOrDefaultAsync(p => p.Email == email);
            return people != null;
        }

        public  List<People> GetPeoples()
        {
            return new List<People>(db.People);
        }
        public async  Task<People> GetPeople(int id)
        {
            var people = await db.People.FirstOrDefaultAsync(p => p.id == id);
            if (people != null)
            {
                return people;
            }
            return null;
        }
        public async  Task<People> GetPeople(string email)
        {
            var people = await db.People.FirstOrDefaultAsync(p => p.Email == email);
            if (people != null)
            {
                return people;
            }
            return null;
        }

        public async  Task EditPeople(FormEdit formEdit)
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

        public async  Task AddImg(int id, string file)
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
