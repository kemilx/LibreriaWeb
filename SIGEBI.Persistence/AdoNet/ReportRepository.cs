using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SIGEBI.Domain.Entities;
using SIGEBI.Persistence.Interfaces;

namespace SIGEBI.Persistence.AdoNet
{
    public class ReportRepository : IReportRepository
    {
        private readonly string _connectionString;

        public ReportRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SIGEBI");
        }

        public List<Reporte> GetReportesByUser(int usuarioId)
        {
            var reportes = new List<Reporte>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT * FROM Reporte WHERE GeneradoPor = @UsuarioId", conn);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reportes.Add(new Reporte
                        {
                            Id = (int)reader["Id"],
                            TipoReporte = reader["TipoReporte"].ToString(),
                            Titulo = reader["Titulo"].ToString(),
                            GeneradoPor = (int)reader["GeneradoPor"],
                            FechaGenerado = (DateTime)reader["FechaGenerado"],
                            RutaArchivo = reader["RutaArchivo"].ToString()
                        });
                    }
                }
            }
            return reportes;
        }

    }
}