using Microsoft.Extensions.Configuration;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SIGEBI.Persistence.AdoNet
{
    public class PenalizacionRepository : IPenalizacionRepository
    {
        private readonly string _connectionString;

        public PenalizacionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SIGEBI");
        }

        public List<Penalizacion> GetPenalizacionesByUser(int usuarioId)
        {
            var penalizaciones = new List<Penalizacion>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(@"SELECT p.* FROM Penalizacion p
                                           INNER JOIN Prestamo pr ON p.PrestamoId = pr.Id
                                           WHERE pr.UsuarioId = @UsuarioId", conn);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        penalizaciones.Add(new Penalizacion
                        {
                            Id = (int)reader["Id"],
                            PrestamoId = (int)reader["PrestamoId"],
                            Monto = (decimal)reader["Monto"],
                            Motivo = reader["Motivo"].ToString(),
                            Pagado = (bool)reader["Pagado"],
                            FechaCreacion = (DateTime)reader["FechaCreacion"]
                        });
                    }
                }
            }
            return penalizaciones;
        }

        // Métodos AddPenalizacion, UpdatePenalizacion, DeletePenalizacion aquí, usando la tabla y campos correctos.
    }
}