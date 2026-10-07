using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReachSystem.Enums;
using ReachSystem.Models;
using ReachSystem.Services;

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
        private readonly RelatorioAnimalService _relatorioAnimalService;
        private readonly RelatorioEventoService _relatorioEventoService;
        private readonly RelatorioParticipacaoService _relatorioParticipacaoService;

        public RelatorioController(AnimalService animalService, RelatorioAnimalService relatorioAnimalService, RelatorioEventoService relatorioEventoService, RelatorioParticipacaoService relatorioParticipacaoService)
        {
            _animalService = animalService;
            _relatorioAnimalService = relatorioAnimalService;
            _relatorioEventoService = relatorioEventoService;
            _relatorioParticipacaoService = relatorioParticipacaoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? pesquisar, string? especie, Sexo? sexo, Status? status, DateTime? dataInicio, DateTime? dataFim)
        {
            var filtro = new RelatorioAnimal
            {
                Pesquisar = pesquisar,
                Especie = especie,
                Sexo = sexo,
                Status = status,
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            var animais = await _relatorioAnimalService.FiltrarAsync(filtro);

            ViewBag.Especies = await _relatorioAnimalService.GetEspeciesAsync();

            ViewBag.Pesquisar = pesquisar;
            ViewBag.Especie = especie;
            ViewBag.Sexo = sexo;
            ViewBag.Status = status;
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

            return View(animais);
        }
        [HttpGet]
        public async Task<IActionResult> ExportarExcel(string? pesquisar, string? especie, Sexo? sexo, Status? status, DateTime? dataInicio, DateTime? dataFim)
        {
            var filtro = new RelatorioAnimal
            {
                Pesquisar = pesquisar,
                Especie = especie,
                Sexo = sexo,
                Status = status,
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            var animais = await _relatorioAnimalService.FiltrarAsync(filtro);

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
            var filtro = new RelatorioAnimal
            {
                Pesquisar = pesquisar,
                Especie = especie,
                Sexo = sexo,
                Status = status,
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            var animais = await _relatorioAnimalService.FiltrarAsync(filtro);

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
                                columns.RelativeColumn(2.0f);
                                columns.RelativeColumn(1.4f);
                                columns.RelativeColumn(1.5f);
                                columns.ConstantColumn(35);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1.4f);
                                columns.RelativeColumn(1.5f);
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

                            foreach (var animal in animais)
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
        [HttpGet]
        public async Task<IActionResult> Eventos(string? pesquisar, DateTime? dataInicio, DateTime? dataFim)
        {
            var filtro = new RelatorioEvento
            {
                Pesquisar = pesquisar,
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            var eventos = await _relatorioEventoService.FiltrarAsync(filtro);

            ViewBag.Pesquisar = pesquisar;
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

            return View(eventos);
        }
        [HttpGet]
        public async Task<IActionResult> ExportarExcelEventos(
    string? pesquisar,
    DateTime? dataInicio,
    DateTime? dataFim)
        {
            var filtro = new RelatorioEvento
            {
                Pesquisar = pesquisar,
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            var eventos = await _relatorioEventoService.FiltrarAsync(filtro);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Eventos");

            worksheet.Cell(1, 1).Value = "Relatório de Eventos";

            worksheet.Cell(3, 1).Value = "Nome";
            worksheet.Cell(3, 2).Value = "Data";
            worksheet.Cell(3, 3).Value = "Local";
            worksheet.Cell(3, 4).Value = "Descrição";

            var linha = 4;

            foreach (var evento in eventos)
            {
                worksheet.Cell(linha, 1).Value = evento.Nome;

                worksheet.Cell(linha, 2).Value = evento.Data;
                worksheet.Cell(linha, 2).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";

                worksheet.Cell(linha, 3).Value = evento.Local;
                worksheet.Cell(linha, 4).Value = evento.Descricao;

                linha++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            var arquivo = stream.ToArray();

            return File(
                arquivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Relatorio_Eventos.xlsx");
        }
        [HttpGet]
        public async Task<IActionResult> ExportarPdfEventos(
    string? pesquisar,
    DateTime? dataInicio,
    DateTime? dataFim)
        {
            var filtro = new RelatorioEvento
            {
                Pesquisar = pesquisar,
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            var eventos = await _relatorioEventoService.FiltrarAsync(filtro);

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.Header()
                        .Text("Relatório de Eventos")
                        .FontSize(20)
                        .Bold()
                        .FontColor("#173746");

                    page.Content()
                        .PaddingTop(20)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2.0f);
                                columns.RelativeColumn(1.4f);
                                columns.RelativeColumn(1.8f);
                                columns.RelativeColumn(3.0f);
                            });

                            table.Header(header =>
                            {
                                header.Cell()
                                    .Background("#2C5D7C")
                                    .Padding(6)
                                    .Text("Nome")
                                    .FontColor("#FFFFFF")
                                    .Bold();

                                header.Cell()
                                    .Background("#2C5D7C")
                                    .Padding(6)
                                    .AlignCenter()
                                    .Text("Data")
                                    .FontColor("#FFFFFF")
                                    .Bold();

                                header.Cell()
                                    .Background("#2C5D7C")
                                    .Padding(6)
                                    .Text("Local")
                                    .FontColor("#FFFFFF")
                                    .Bold();

                                header.Cell()
                                    .Background("#2C5D7C")
                                    .Padding(6)
                                    .Text("Descrição")
                                    .FontColor("#FFFFFF")
                                    .Bold();
                            });

                            foreach (var evento in eventos)
                            {
                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .Text(evento.Nome)
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .AlignCenter()
                                    .Text(evento.Data.ToString("dd/MM/yyyy HH:mm"))
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .Text(evento.Local)
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .Text(evento.Descricao)
                                    .FontSize(8);
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("ReachSystem • Relatório de Eventos");
                        });
                });
            });

            var pdf = documento.GeneratePdf();

            return File(
                pdf,
                "application/pdf",
                "Relatorio_Eventos.pdf");
        }
        [HttpGet]
        public async Task<IActionResult> Participacoes(string? pesquisar, string? evento, StatusParticipacao? status, DateTime? dataInicio, DateTime? dataFim)
        {
            var filtro = new RelatorioParticipacao
            {
                Pesquisar = pesquisar,
                Evento = evento,
                Status = status,
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            var participacoes =
                await _relatorioParticipacaoService.FiltrarAsync(filtro);

            ViewBag.Eventos =
                await _relatorioParticipacaoService.GetEventosAsync();

            ViewBag.Pesquisar = pesquisar;
            ViewBag.Evento = evento;
            ViewBag.Status = status;
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

            return View(participacoes);
        }
        [HttpGet]
        public async Task<IActionResult> ExportarExcelParticipacoes(
    string? pesquisar,
    string? evento,
    StatusParticipacao? status,
    DateTime? dataInicio,
    DateTime? dataFim)
        {
            var filtro = new RelatorioParticipacao
            {
                Pesquisar = pesquisar,
                Evento = evento,
                Status = status,
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            var participacoes =
                await _relatorioParticipacaoService.FiltrarAsync(filtro);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Participações");

            worksheet.Cell(1, 1).Value = "Relatório de Participações";

            worksheet.Cell(3, 1).Value = "Participante";
            worksheet.Cell(3, 2).Value = "E-mail";
            worksheet.Cell(3, 3).Value = "Evento";
            worksheet.Cell(3, 4).Value = "Data";
            worksheet.Cell(3, 5).Value = "Local";
            worksheet.Cell(3, 6).Value = "Status";

            var linha = 4;

            foreach (var participacao in participacoes)
            {
                worksheet.Cell(linha, 1).Value = participacao.Usuario.Nome;
                worksheet.Cell(linha, 2).Value = participacao.Usuario.Email;
                worksheet.Cell(linha, 3).Value = participacao.Evento.Nome;

                worksheet.Cell(linha, 4).Value = participacao.Evento.Data;
                worksheet.Cell(linha, 4).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";

                worksheet.Cell(linha, 5).Value = participacao.Evento.Local;
                worksheet.Cell(linha, 6).Value = participacao.Status.ToString();

                linha++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            var arquivo = stream.ToArray();

            return File(
                arquivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Relatorio_Participacoes.xlsx");
        }
        [HttpGet]
        public async Task<IActionResult> ExportarPdfParticipacoes(
    string? pesquisar,
    string? evento,
    StatusParticipacao? status,
    DateTime? dataInicio,
    DateTime? dataFim)
        {
            var filtro = new RelatorioParticipacao
            {
                Pesquisar = pesquisar,
                Evento = evento,
                Status = status,
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            var participacoes =
                await _relatorioParticipacaoService.FiltrarAsync(filtro);

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.Header()
                        .Text("Relatório de Participações")
                        .FontSize(20)
                        .Bold()
                        .FontColor("#173746");

                    page.Content()
                        .PaddingTop(20)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2.0f); // Participante
                                columns.RelativeColumn(2.5f); // E-mail
                                columns.RelativeColumn(2.0f); // Evento
                                columns.RelativeColumn(1.5f); // Data
                                columns.RelativeColumn(1.8f); // Local
                                columns.RelativeColumn(1.3f); // Status
                            });

                            table.Header(header =>
                            {
                                header.Cell()
                                    .Background("#2C5D7C")
                                    .Padding(6)
                                    .Text("Participante")
                                    .FontColor("#FFFFFF")
                                    .Bold();

                                header.Cell()
                                    .Background("#2C5D7C")
                                    .Padding(6)
                                    .Text("E-mail")
                                    .FontColor("#FFFFFF")
                                    .Bold();

                                header.Cell()
                                    .Background("#2C5D7C")
                                    .Padding(6)
                                    .Text("Evento")
                                    .FontColor("#FFFFFF")
                                    .Bold();

                                header.Cell()
                                    .Background("#2C5D7C")
                                    .Padding(6)
                                    .AlignCenter()
                                    .Text("Data")
                                    .FontColor("#FFFFFF")
                                    .Bold();

                                header.Cell()
                                    .Background("#2C5D7C")
                                    .Padding(6)
                                    .Text("Local")
                                    .FontColor("#FFFFFF")
                                    .Bold();

                                header.Cell()
                                    .Background("#2C5D7C")
                                    .Padding(6)
                                    .Text("Status")
                                    .FontColor("#FFFFFF")
                                    .Bold();
                            });

                            foreach (var participacao in participacoes)
                            {
                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .Text(participacao.Usuario.Nome)
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .Text(participacao.Usuario.Email ?? "")
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .Text(participacao.Evento.Nome)
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .AlignCenter()
                                    .Text(participacao.Evento.Data.ToString("dd/MM/yyyy HH:mm"))
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .Text(participacao.Evento.Local)
                                    .FontSize(8);

                                table.Cell()
                                    .Border(1)
                                    .BorderColor("#E6EDF5")
                                    .Padding(4)
                                    .Text(participacao.Status.ToString())
                                    .FontSize(8);
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("ReachSystem • Relatório de Participações");
                        });
                });
            });

            var pdf = documento.GeneratePdf();

            return File(
                pdf,
                "application/pdf",
                "Relatorio_Participacoes.pdf");
        }
    }
}