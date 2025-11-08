using DataBase;
using Flashcard.Modules.Users.Application.Interfaces.Repositories;
using Flashcard.Modules.Users.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCard.Modules.Users.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Context _context;
        public UserRepository(Context context)
        {
            _context = context;
        }

        public async Task  AddUserAsync(User user)
        {
            await _context.Set<User>().AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
