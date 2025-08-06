using Application.Interfaces;
using Application.Models;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _repository;

        public VentaService(IVentaRepository repository)
        {
            _repository = repository;
        }

        public List<Venta> GetAllByClient(int clientId)
        {
            return _repository.GetAllByClient(clientId);
        }

        public Venta? GetById(int id)
        {
            return _repository.GetById(id);
        }

        public int AddVenta(VentaDto dto)
        {
            var venta = new Venta()
            {
                ClientId = dto.ClientId,
            };
            return _repository.Add(venta).Id;
        }

        public void DeleteVenta(int id)
        {
            var ventaToDelete = _repository.Get(id);
            if (ventaToDelete != null)
            {
                _repository.Delete(ventaToDelete);
            }
        }

        public void UpdateVenta(int id, VentaDto dto)
        {
            var ventaToUpdate = _repository.Get(id);
            if (ventaToUpdate != null)
            {
                ventaToUpdate.ClientId = dto.ClientId;
                _repository.Update(ventaToUpdate);
            }
        }
    }
}