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
        public async Task<IActionResult> ExportarExcel(
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

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Animais");

            // TÍTULO
            var titulo = worksheet.Range("A1:H1");
            titulo.Merge();
            titulo.Value = "ReachSystem | Relatório de Animais";

            titulo.Style.Font.Bold = true;
            titulo.Style.Font.FontSize = 16;
            titulo.Style.Font.FontColor = XLColor.White;
            titulo.Style.Fill.BackgroundColor = XLColor.FromHtml("#173746");
            titulo.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            titulo.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            worksheet.Row(1).Height = 35;

            // DATA DE GERAÇÃO
            var dataGeracao = worksheet.Range("A2:H2");
            dataGeracao.Merge();
            dataGeracao.Value = $"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}";

            dataGeracao.Style.Font.FontSize = 10;
            dataGeracao.Style.Font.Italic = true;
            dataGeracao.Style.Font.FontColor = XLColor.FromHtml("#5B7180");
            dataGeracao.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            // CABEÇALHO
            worksheet.Cell(3, 1).Value = "Nome";
            worksheet.Cell(3, 2).Value = "Espécie";
            worksheet.Cell(3, 3).Value = "Raça";
            worksheet.Cell(3, 4).Value = "Idade";
            worksheet.Cell(3, 5).Value = "Porte";
            worksheet.Cell(3, 6).Value = "Sexo";
            worksheet.Cell(3, 7).Value = "Status";
            worksheet.Cell(3, 8).Value = "Data de Entrada";

            var cabecalho = worksheet.Range("A3:H3");

            cabecalho.Style.Font.Bold = true;
            cabecalho.Style.Font.FontColor = XLColor.White;
            cabecalho.Style.Fill.BackgroundColor = XLColor.FromHtml("#2C5D7C");
            cabecalho.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cabecalho.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            worksheet.Row(3).Height = 32;

            // DADOS
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

                // LINHAS ALTERNADAS
                if (linha % 2 == 0)
                {
                    worksheet.Range(linha, 1, linha, 8)
                        .Style.Fill.BackgroundColor = XLColor.FromHtml("#F4F7FB");
                }

                linha++;
            }

            // FORMATAÇÃO DOS DADOS
            if (linha > 4)
            {
                var dados = worksheet.Range(4, 1, linha - 1, 8);

                dados.Style.Font.FontColor = XLColor.FromHtml("#173746");
                dados.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                dados.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                dados.Style.Border.BottomBorderColor = XLColor.FromHtml("#E6EDF5");

                // Centralizar idade e data
                worksheet.Range(4, 4, linha - 1, 4)
                    .Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Range(4, 8, linha - 1, 8)
                    .Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // FILTRO AUTOMÁTICO
            worksheet.Range(3, 1, Math.Max(3, linha - 1), 8).SetAutoFilter();

            // CONGELAR TÍTULO E CABEÇALHO
            worksheet.SheetView.FreezeRows(3);

            // AJUSTAR LARGURA DAS COLUNAS
            worksheet.Columns(1, 8).AdjustToContents();

            // LIMITAR E DEFINIR LARGURAS
            worksheet.Column(1).Width = 30; // Nome
            worksheet.Column(2).Width = 25; // Espécie
            worksheet.Column(3).Width = 25; // Raça
            worksheet.Column(4).Width = 13; // Idade
            worksheet.Column(5).Width = 16; // Porte
            worksheet.Column(6).Width = 16; // Sexo
            worksheet.Column(7).Width = 20; // Status
            worksheet.Column(8).Width = 23; // Data de Entrada

            // ALINHAMENTO VERTICAL
            worksheet.Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // GERAR ARQUIVO
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

            // TÍTULO
            var titulo = worksheet.Range("A1:D1");
            titulo.Merge();
            titulo.Value = "ReachSystem | Relatório de Eventos";

            titulo.Style.Font.Bold = true;
            titulo.Style.Font.FontSize = 16;
            titulo.Style.Font.FontColor = XLColor.White;
            titulo.Style.Fill.BackgroundColor = XLColor.FromHtml("#173746");
            titulo.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            titulo.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            worksheet.Row(1).Height = 35;

            // DATA DE GERAÇÃO
            var dataGeracao = worksheet.Range("A2:D2");
            dataGeracao.Merge();
            dataGeracao.Value = $"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}";

            dataGeracao.Style.Font.FontSize = 10;
            dataGeracao.Style.Font.Italic = true;
            dataGeracao.Style.Font.FontColor = XLColor.FromHtml("#5B7180");
            dataGeracao.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            // CABEÇALHO
            worksheet.Cell(3, 1).Value = "Nome";
            worksheet.Cell(3, 2).Value = "Data";
            worksheet.Cell(3, 3).Value = "Local";
            worksheet.Cell(3, 4).Value = "Descrição";

            var cabecalho = worksheet.Range("A3:D3");

            cabecalho.Style.Font.Bold = true;
            cabecalho.Style.Font.FontColor = XLColor.White;
            cabecalho.Style.Fill.BackgroundColor = XLColor.FromHtml("#2C5D7C");
            cabecalho.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cabecalho.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            worksheet.Row(3).Height = 32;

            // DADOS
            var linha = 4;

            foreach (var evento in eventos)
            {
                worksheet.Cell(linha, 1).Value = evento.Nome;

                worksheet.Cell(linha, 2).Value = evento.Data;
                worksheet.Cell(linha, 2).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";

                worksheet.Cell(linha, 3).Value = evento.Local;
                worksheet.Cell(linha, 4).Value = evento.Descricao;

                // LINHAS ALTERNADAS
                if (linha % 2 == 0)
                {
                    worksheet.Range(linha, 1, linha, 4)
                        .Style.Fill.BackgroundColor = XLColor.FromHtml("#F4F7FB");
                }

                linha++;
            }

            // FORMATAÇÃO DOS DADOS
            if (linha > 4)
            {
                var dados = worksheet.Range(4, 1, linha - 1, 4);

                dados.Style.Font.FontColor = XLColor.FromHtml("#173746");
                dados.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                dados.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                dados.Style.Border.BottomBorderColor = XLColor.FromHtml("#E6EDF5");

                worksheet.Range(4, 2, linha - 1, 2)
                    .Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // FILTRO AUTOMÁTICO
            worksheet.Range(3, 1, Math.Max(3, linha - 1), 4).SetAutoFilter();

            // CONGELAR TÍTULO E CABEÇALHO
            worksheet.SheetView.FreezeRows(3);

            // AJUSTAR LARGURA DAS COLUNAS
            worksheet.Columns(1, 4).AdjustToContents();

            // DEFINIR LARGURAS
            worksheet.Column(1).Width = 30; // Nome
            worksheet.Column(2).Width = 23; // Data
            worksheet.Column(3).Width = 30; // Local
            worksheet.Column(4).Width = 45; // Descrição

            worksheet.Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // GERAR ARQUIVO
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

            // TÍTULO
            var titulo = worksheet.Range("A1:F1");
            titulo.Merge();
            titulo.Value = "ReachSystem | Relatório de Participações";

            titulo.Style.Font.Bold = true;
            titulo.Style.Font.FontSize = 16;
            titulo.Style.Font.FontColor = XLColor.White;
            titulo.Style.Fill.BackgroundColor = XLColor.FromHtml("#173746");
            titulo.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            titulo.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            worksheet.Row(1).Height = 35;

            // DATA DE GERAÇÃO
            var dataGeracao = worksheet.Range("A2:F2");
            dataGeracao.Merge();
            dataGeracao.Value = $"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}";

            dataGeracao.Style.Font.FontSize = 10;
            dataGeracao.Style.Font.Italic = true;
            dataGeracao.Style.Font.FontColor = XLColor.FromHtml("#5B7180");
            dataGeracao.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            // CABEÇALHO
            worksheet.Cell(3, 1).Value = "Participante";
            worksheet.Cell(3, 2).Value = "E-mail";
            worksheet.Cell(3, 3).Value = "Evento";
            worksheet.Cell(3, 4).Value = "Data";
            worksheet.Cell(3, 5).Value = "Local";
            worksheet.Cell(3, 6).Value = "Status";

            var cabecalho = worksheet.Range("A3:F3");

            cabecalho.Style.Font.Bold = true;
            cabecalho.Style.Font.FontColor = XLColor.White;
            cabecalho.Style.Fill.BackgroundColor = XLColor.FromHtml("#2C5D7C");
            cabecalho.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cabecalho.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            worksheet.Row(3).Height = 32;

            // DADOS
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

                // LINHAS ALTERNADAS
                if (linha % 2 == 0)
                {
                    worksheet.Range(linha, 1, linha, 6)
                        .Style.Fill.BackgroundColor = XLColor.FromHtml("#F4F7FB");
                }

                linha++;
            }

            // FORMATAÇÃO DOS DADOS
            if (linha > 4)
            {
                var dados = worksheet.Range(4, 1, linha - 1, 6);

                dados.Style.Font.FontColor = XLColor.FromHtml("#173746");
                dados.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                dados.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                dados.Style.Border.BottomBorderColor = XLColor.FromHtml("#E6EDF5");

                worksheet.Range(4, 4, linha - 1, 4)
                    .Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // FILTRO AUTOMÁTICO
            worksheet.Range(3, 1, Math.Max(3, linha - 1), 6).SetAutoFilter();

            // CONGELAR TÍTULO E CABEÇALHO
            worksheet.SheetView.FreezeRows(3);

            // AJUSTAR LARGURA DAS COLUNAS
            worksheet.Columns(1, 6).AdjustToContents();

            // DEFINIR LARGURAS
            worksheet.Column(1).Width = 28; // Participante
            worksheet.Column(2).Width = 35; // E-mail
            worksheet.Column(3).Width = 30; // Evento
            worksheet.Column(4).Width = 23; // Data
            worksheet.Column(5).Width = 30; // Local
            worksheet.Column(6).Width = 20; // Status

            worksheet.Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // GERAR ARQUIVO
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