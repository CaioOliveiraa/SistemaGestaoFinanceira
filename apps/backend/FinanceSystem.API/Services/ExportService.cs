using ClosedXML.Excel;
using FinanceSystem.API.Repositories.Interfaces;

namespace FinanceSystem.API.Services
{
    public class ExportService
    {
        private readonly ITransactionRepository _repo;

        public ExportService(ITransactionRepository repo)
        {
            _repo = repo;
        }

        public async Task<byte[]> ExportToExcelAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            var transactions = await _repo.GetAllByUserIdAsync(userId);
            var filtered = transactions
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .OrderBy(t => t.Date)
                .ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Transações");

            worksheet.Cell(1,1).Value = "Data";
            worksheet.Cell(1,2).Value = "Descrição";
            worksheet.Cell(1,3).Value = "Categoria";
            worksheet.Cell(1,4).Value = "Tipo";
            worksheet.Cell(1,5).Value = "Valor";

            for (var i = 0; i < filtered.Count; i++)
            {
                var transaction = filtered[i];

                worksheet.Cell(i + 2, 1).Value = transaction.Date.ToString("dd/MM/yyyy");
                worksheet.Cell(i + 2, 2).Value = transaction.Description;
                worksheet.Cell(i + 2, 3).Value = transaction.Category.Name;
                worksheet.Cell(i + 2, 4).Value = transaction.Type.ToString();
                worksheet.Cell(i + 2, 5).Value = transaction.Amount;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        } 
    }
}