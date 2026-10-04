using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReachSystem.Enums;
using ReachSystem.Services;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ReachSystem.Controllers
{
    [Authorize]
    public class RelatorioController : Controller
    {
        static RelatorioController()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        private readonly AnimalService _animalService;

        public RelatorioController(AnimalService animalService)
        {
            _animalService = animalService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? pesquisar,
            string? especie,
            Sexo? sexo,
            Status? status,
            DateTime? dataInicio,
            DateTime? dataFim)
        {
            var todosAnimais = await _animalService.GetAllAnimalsAsync();

            // Opções dos filtros
            ViewBag.Especies = todosAnimais
                .Select(a => a.Especie)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct()
                .OrderBy(e => e)
                .ToList();

            ViewBag.Pesquisar = pesquisar;
            ViewBag.Especie = especie;
            ViewBag.Sexo = sexo;
            ViewBag.Status = status;
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

            // Filtro por nome ou raça
            if (!string.IsNullOrWhiteSpace(pesquisar))
            {
                todosAnimais = todosAnimais.Where(a =>
                    a.Nome.Contains(pesquisar, StringComparison.OrdinalIgnoreCase) ||
                    a.Raca.Contains(pesquisar, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por espécie
            if (!string.IsNullOrWhiteSpace(especie))
            {
                todosAnimais = todosAnimais.Where(a =>
                    a.Especie.Equals(especie, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por sexo
            if (sexo.HasValue)
            {
                todosAnimais = todosAnimais.Where(a =>
                    a.SexoAnimal == sexo.Value);
            }

            // Filtro por status
            if (status.HasValue)
            {
                todosAnimais = todosAnimais.Where(a =>
                    a.StatusAnimal == status.Value);
            }

            // Filtro por data inicial
            if (dataInicio.HasValue)
            {
                todosAnimais = todosAnimais.Where(a =>
                    a.DataDeEntrada.Date >= dataInicio.Value.Date);
            }

            // Filtro por data final
            if (dataFim.HasValue)
            {
                todosAnimais = todosAnimais.Where(a =>
                    a.DataDeEntrada.Date <= dataFim.Value.Date);
            }

            return View(todosAnimais.OrderBy(a => a.Nome));
        }
        [HttpGet]
        public async Task<IActionResult> ExportarExcel(
    string? pesquisar,
    string? especie,
    Sexo? sexo,
    Status? status,
    DateTime? dataInicio,
    DateTime? dataFim)
        {
            var animais = await _animalService.GetAllAnimalsAsync();

            if (!string.IsNullOrWhiteSpace(pesquisar))
            {
                animais = animais.Where(a =>
                    a.Nome.Contains(pesquisar, StringComparison.OrdinalIgnoreCase) ||
                    a.Raca.Contains(pesquisar, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(especie))
            {
                animais = animais.Where(a =>
                    a.Especie.Equals(especie, StringComparison.OrdinalIgnoreCase));
            }

            if (sexo.HasValue)
            {
                animais = animais.Where(a => a.SexoAnimal == sexo.Value);
            }

            if (status.HasValue)
            {
                animais = animais.Where(a => a.StatusAnimal == status.Value);
            }

            if (dataInicio.HasValue)
            {
                animais = animais.Where(a =>
                    a.DataDeEntrada.Date >= dataInicio.Value.Date);
            }

            if (dataFim.HasValue)
            {
                animais = animais.Where(a =>
                    a.DataDeEntrada.Date <= dataFim.Value.Date);
            }

            animais = animais.OrderBy(a => a.Nome);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Animais");

            worksheet.Cell(1, 1).Value = "Relatório de Animais";

            worksheet.Cell(3, 1).Value = "Nome";
            worksheet.Cell(3, 2).Value = "Espécie";
            worksheet.Cell(3, 3).Value = "Raça";
            worksheet.Cell(3, 4).Value = "Idade";
            worksheet.Cell(3, 5).Value = "Porte";
            worksheet.Cell(3, 6).Value = "Sexo";
            worksheet.Cell(3, 7).Value = "Status";
            worksheet.Cell(3, 8).Value = "Data de Entrada";

            var linha = 4;

            foreach (var animal in animais)
            {
                worksheet.Cell(linha, 1).Value = animal.Nome;
                worksheet.Cell(linha, 2).Value = animal.Especie;
                worksheet.Cell(linha, 3).Value = animal.Raca;
                worksheet.Cell(linha, 4).Value = animal.Idade;
                worksheet.Cell(linha, 5).Value = animal.Porte;
                worksheet.Cell(linha, 6).Value = animal.SexoAnimal.ToString();
                worksheet.Cell(linha, 7).Value = animal.StatusAnimal.ToString();
                worksheet.Cell(linha, 8).Value = animal.DataDeEntrada;
                worksheet.Cell(linha, 8).Style.DateFormat.Format = "dd/MM/yyyy";

                linha++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            var arquivo = stream.ToArray();

            return File(
                arquivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Relatorio_Animais.xlsx");
        }
        [HttpGet]
        public async Task<IActionResult> ExportarPdf(
    string? pesquisar,
    string? especie,
    Sexo? sexo,
    Status? status,
    DateTime? dataInicio,
    DateTime? dataFim)
        {
            var animais = await _animalService.GetAllAnimalsAsync();

            if (!string.IsNullOrWhiteSpace(pesquisar))
            {
                animais = animais.Where(a =>
                    a.Nome.Contains(pesquisar, StringComparison.OrdinalIgnoreCase) ||
                    a.Raca.Contains(pesquisar, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(especie))
            {
                animais = animais.Where(a =>
                    a.Especie.Equals(especie, StringComparison.OrdinalIgnoreCase));
            }

            if (sexo.HasValue)
            {
                animais = animais.Where(a =>
                    a.SexoAnimal == sexo.Value);
            }

            if (status.HasValue)
            {
                animais = animais.Where(a =>
                    a.StatusAnimal == status.Value);
            }

            if (dataInicio.HasValue)
            {
                animais = animais.Where(a =>
                    a.DataDeEntrada.Date >= dataInicio.Value.Date);
            }

            if (dataFim.HasValue)
            {
                animais = animais.Where(a =>
                    a.DataDeEntrada.Date <= dataFim.Value.Date);
            }

            var listaAnimais = animais
                .OrderBy(a => a.Nome)
                .ToList();

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.Header()
                        .Text("Relatório de Animais")
                        .FontSize(20)
                        .Bold()
                        .FontColor("#173746");

                    page.Content()
                        .PaddingTop(20)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2.0f);  // Nome
                                columns.RelativeColumn(1.4f);  // Espécie
                                columns.RelativeColumn(1.5f);  // Raça
                                columns.ConstantColumn(35);    // Idade
                                columns.RelativeColumn(1.1f);  // Porte
                                columns.RelativeColumn(1.1f);  // Sexo
                                columns.RelativeColumn(1.4f);  // Status
                                columns.RelativeColumn(1.5f);  // Entrada
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#2C5D7C").Padding(6)
                                    .Text("Nome").FontColor("#FFFFFF").Bold();

                                header.Cell().Background("#2C5D7C").Padding(6)
                                    .Text("Espécie").FontColor("#FFFFFF").Bold();

                                header.Cell().Background("#2C5D7C").Padding(6)
                                    .Text("Raça").FontColor("#FFFFFF").Bold();

                                header.Cell().Background("#2C5D7C").Padding(6)
                                    .AlignCenter()
                                    .Text("Idade").FontColor("#FFFFFF").Bold();

                                header.Cell().Background("#2C5D7C").Padding(6)
                                    .Text("Porte").FontColor("#FFFFFF").Bold();

                                header.Cell().Background("#2C5D7C").Padding(6)
                                    .Text("Sexo").FontColor("#FFFFFF").Bold();

                                header.Cell().Background("#2C5D7C").Padding(6)
                                    .Text("Status").FontColor("#FFFFFF").Bold();

                                header.Cell().Background("#2C5D7C").Padding(6)
                                    .AlignCenter()
                                    .Text("Entrada").FontColor("#FFFFFF").Bold();
                            });

                            foreach (var animal in listaAnimais)
                            {
                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .Text(animal.Nome)
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .Text(animal.Especie)
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .Text(animal.Raca)
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .AlignCenter()
                                    .Text(animal.Idade.ToString())
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .Text(animal.Porte)
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .Text(animal.SexoAnimal.ToString())
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .Text(animal.StatusAnimal.ToString())
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .AlignCenter()
                                    .Text(animal.DataDeEntrada.ToString("dd/MM/yyyy"))
                                    .FontSize(8);
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("ReachSystem • Relatório de Animais");
                        });
                });
            });

            var pdf = documento.GeneratePdf();

            return File(
                pdf,
                "application/pdf",
                "Relatorio_Animais.pdf");
        }
    }
}