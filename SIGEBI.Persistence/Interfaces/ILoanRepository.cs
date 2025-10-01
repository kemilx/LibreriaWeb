using System.Collections.Generic;
using SIGEBI.Domain.Entities;

public interface ILoanRepository
{
    // Obtiene todos los préstamos de un usuario
    List<Loan> GetLoansByUser(int userId);

    // Obtiene un préstamo por su Id
    Loan GetLoanById(int loanId);

    // Crea un nuevo préstamo
    void AddLoan(Loan loan);

    // Actualiza un préstamo existente
    void UpdateLoan(Loan loan);

    // Elimina (lógica o física) un préstamo
    void DeleteLoan(int loanId);

}