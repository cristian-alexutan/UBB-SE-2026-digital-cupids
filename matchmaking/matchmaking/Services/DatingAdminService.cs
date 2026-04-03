using matchmaking.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.Services
{
    internal class DatingAdminService
    {
        private DatingAdminRepository AdminRepo;

        public DatingAdminService(DatingAdminRepository repo)
        {
            AdminRepo = repo;
        }

        public bool IsAdmin (int userId)
        {
            return AdminRepo.FindById(userId) != null;
        }
    }
}
