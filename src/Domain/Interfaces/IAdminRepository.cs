using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Domain determina la arquitectura fundamental de la aplicación.
// Por eso los contratos de acceso a datos (intrefaces) son parte del Domain, no de la Infraestructura.

namespace Domain.Interfaces
{
    public interface IAdminRepository : IRepositoryBase<Admin>
    {
        Admin? Get(string name);
    }
}
