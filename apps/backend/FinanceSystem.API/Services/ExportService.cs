using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using FinanceSystem.API.Repositories.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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

            worksheet.Cell(1, 1).Value = "Data";
            worksheet.Cell(1, 2).Value = "Descrição";
            worksheet.Cell(1, 3).Value = "Categoria";
            worksheet.Cell(1, 4).Value = "Tipo";
            worksheet.Cell(1, 5).Value = "Valor";

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

        public async Task<byte[]> ExportToPdfAsync(Guid userId, DateTime start, DateTime end)
        {
            var transactions = await _repo.GetAllByUserIdAsync(userId);
            var filtered = transactions
                .Where(t => t.Date >= start && t.Date <= end)
                .OrderBy(t => t.Date)
                .ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header()
                        .Text($"Relatório de Transações ({start:dd/MM/yyyy} a {end:dd/MM/yyyy})")
                        .SemiBold().FontSize(18).FontColor(QuestPDF.Helpers.Colors.Blue.Medium);

                    page.Content()
                        .Table(table =>
                        {
                            // Definir colunas
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // Cabeçalho
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Data");
                                header.Cell().Element(CellStyle).Text("Descrição");
                                header.Cell().Element(CellStyle).Text("Categoria");
                                header.Cell().Element(CellStyle).Text("Tipo");
                                header.Cell().Element(CellStyle).Text("Valor");
                            });

                            // Corpo
                            foreach (var t in filtered)
                            {
                                table.Cell().Element(CellStyle).Text(t.Date.ToString("dd/MM/yyyy"));
                                table.Cell().Element(CellStyle).Text(t.Description);
                                table.Cell().Element(CellStyle).Text(t.Category.Name);
                                table.Cell().Element(CellStyle).Text(t.Type.ToString());
                                table.Cell().Element(CellStyle).Text(t.Amount.ToString("C"));
                            }

                            // Estilo das células
                            static IContainer CellStyle(IContainer container)
                            {
                                return container.PaddingVertical(5).BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2);
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Relatório gerado em ");
                            x.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}").SemiBold();
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}