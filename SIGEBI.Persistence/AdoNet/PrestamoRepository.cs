using Microsoft.Extensions.Configuration;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SIGEBI.Persistence.AdoNet
{
    public class PrestamoRepository : IPrestamoRepository
    {
        private readonly string _connectionString;

        public PrestamoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SIGEBI");
        }

        public List<Prestamo> GetPrestamosByUser(int usuarioId)
        {
            var prestamos = new List<Prestamo>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT * FROM Prestamo WHERE UsuarioId = @UsuarioId", conn);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        prestamos.Add(new Prestamo
                        {
                            Id = (int)reader["Id"],
                            LibroId = (int)reader["LibroId"],
                            UsuarioId = (int)reader["UsuarioId"],
                            FechaPrestamo = (DateTime)reader["FechaPrestamo"],
                            FechaVencimiento = (DateTime)reader["FechaVencimiento"],
                            FechaDevolucion = reader["FechaDevolucion"] == DBNull.Value ? null : (DateTime?)reader["FechaDevolucion"],
                            Devuelto = (bool)reader["Devuelto"]
                        });
                    }
                }
            }
            return prestamos;
        }

       
    }
}