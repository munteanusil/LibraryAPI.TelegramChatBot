using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Persistance
{
    public class UserRepository : IUserRepository
    {
        private readonly LibraryContext _context; 
        public UserRepository(LibraryContext context)
        {
            _context = context;
        }
        public async Task CreateUser(User user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user,ct);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmail(string email, CancellationToken ct = default)
        {
            await _context.Users.FirstOrDefaultAsync(e => e.Email == email,ct);
        }
    }
}
