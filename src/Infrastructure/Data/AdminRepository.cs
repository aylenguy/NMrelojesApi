using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class AdminRepository : RepositoryBase<Admin>, IAdminRepository
    {
        private readonly ApplicationContext _context;
        public AdminRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public Admin? Get(string name)
        {
            return _context.Admins.FirstOrDefault(x => x.Name == name);
        }

        public Admin? GetByEmail(string email)
        {
            return _context.Admins.FirstOrDefault(a => a.Email == email);
        }

    }
}
