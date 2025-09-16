using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITSystem.Data;

namespace ITSystem.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task AddUserAsync(User user);

        Task<List<User>> GetAllUsersAsync();
    }
}
