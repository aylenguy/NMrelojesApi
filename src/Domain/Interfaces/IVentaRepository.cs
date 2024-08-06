using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IVentaRepository : IRepositoryBase<Venta>
    {
        List<Venta> GetAllByClient(int clientId);
        Venta? GetById(int id);
    }
}
