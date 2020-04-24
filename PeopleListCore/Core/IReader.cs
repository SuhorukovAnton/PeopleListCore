using Microsoft.AspNetCore.Http;
using PeopleListCore.Models;
using System.Threading.Tasks;
using System.Web;

namespace PeopleList.Core
{
    interface IReader
    {
        Task AddPeople(string path);
        void Create(string path);
    }
}
