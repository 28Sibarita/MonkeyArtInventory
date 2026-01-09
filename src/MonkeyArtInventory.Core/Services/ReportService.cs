using ClosedXML.Excel;
using MonkeyArtInventory.Core.DTOs;
using MonkeyArtInventory.Core.Interfaces;
using MonkeyArtInventory.Core.Models;
using MonkeyArtInventory.Core.Utilities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Globalization;

namespace MonkeyArtInventory.Core.Services;

public class ReportService
{
    private readonly IInventoryRepository _repository;

    public ReportService(IInventoryRepository repository)
    {
        _repository = repository;
    }

        public async Task<WeeklyReportDto> BuildWeeklyReportAsync(DateTime? now = null)
    {
        var clock = now ?? DateTime.Now;
        var periodEnd = clock.Date;
        var periodStart = periodEnd.AddDays(-6);
        var weeklyMovements = await _repository.ListMovementsAsync(periodStart, periodEnd.AddDays(1));
        var allMovements = await _repository.ListMovementsAsync();
        var products = await _repository.ListProductsAsync();

        var totalEntries = weeklyMovements
            .Where(m => m.Type == MovementType.Entrada || m.Type == MovementType.Ajuste)
            .Sum(m => m.Quantity);

        var totalExits = weeklyMovements
            .Where(m => m.Type == MovementType.Salida || m.Type == MovementType.Merma)
            .Sum(m => m.Quantity);

        // Total sales value (monetary): sum of unit price * quantity for Salida movements where unit price is provided
        var totalSalesValue = weeklyMovements
            .Where(m => m.Type == MovementType.Salida && m.UnitPrice.HasValue)
            .Sum(m => m.UnitPrice.GetValueOrDefault() * m.Quantity);

        // Total returns monetary value (merma)
        var totalReturnsValue = weeklyMovements
            .Where(m => m.Type == MovementType.Merma && m.UnitPrice.HasValue)
            .Sum(m => m.UnitPrice.GetValueOrDefault() * m.Quantity);

        // Total estimated inventory value: current stock * sale price for each product
        var inventoryValue = products.Select(p =>
        {
            var stock = allMovements.Where(m => m.ProductId == p.Id)
                .Sum(m2 => IsIncrease(m2.Type) ? m2.Quantity : -m2.Quantity);
            var price = p.SalePrice ?? 0m;
            return price * stock;
        }).Sum();

        var totalEstimatedSales = inventoryValue;

        var topProducts = weeklyMovements
            .Where(m => m.Type == MovementType.Salida)
            .GroupBy(m => m.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Quantity = g.Sum(m => m.Quantity)
            })
            .OrderByDescending(x => x.Quantity)
            .Take(5)
            .ToList();

        var topDtos = topProducts
            .Select(x =>
            {
                var productName = products.FirstOrDefault(p => p.Id == x.ProductId)?.Name ?? "Unknown";
                return new WeeklyReportTopItemDto(productName, x.Quantity);
            })
            .ToList();

        // Top returns (mermas)
        var topReturns = weeklyMovements
            .Where(m => m.Type == MovementType.Merma)
            .GroupBy(m => m.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Quantity = g.Sum(m => m.Quantity)
            })
            .OrderByDescending(x => x.Quantity)
            .Take(5)
            .ToList();

        var topReturnsDtos = topReturns
            .Select(x =>
            {
                var productName = products.FirstOrDefault(p => p.Id == x.ProductId)?.Name ?? "Unknown";
                return new WeeklyReportTopItemDto(productName, x.Quantity);
            })
            .ToList();

        // Client-related movements (consignments and client returns)
        var clientMovements = weeklyMovements
            .Where(m => !string.IsNullOrWhiteSpace(m.ClientName))
            .Select(m => new WeeklyReportClientItemDto(
                m.ClientName ?? "",
                products.FirstOrDefault(p => p.Id == m.ProductId)?.Name ?? "Unknown",
                m.Type.ToString(),
                m.Quantity,
                m.ClientPrice ?? m.UnitPrice,
                (m.ClientPrice ?? m.UnitPrice) * m.Quantity,
                m.ClientNote ?? m.Note))
            .OrderBy(cm => cm.ClientName)
            .ThenByDescending(cm => cm.TotalValue)
            .ToList();

        var stockByProduct = products
            .Select(p =>
            {
                var stock = allMovements.Where(m => m.ProductId == p.Id)
                    .Sum(m => IsIncrease(m.Type) ? m.Quantity : -m.Quantity);
                return new WeeklyReportStockItemDto(p.Name, p.Barcode, stock);
            })
            .OrderBy(p => p.ProductName)
            .ToList();

        return new WeeklyReportDto
        {
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            Totals = new WeeklyReportTotalsDto(totalEntries, totalExits, totalEstimatedSales, totalSalesValue, totalReturnsValue),
            TopProducts = topDtos,
            TopReturns = topReturnsDtos,
            StockByProduct = stockByProduct,
            ClientMovements = clientMovements
        };
    }

    public string ExportWeeklyReport(WeeklyReportDto report, string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);
        var week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(report.PeriodStart, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        var stamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var excelPath = Path.Combine(outputDirectory, $"informe_semana_{week}_{stamp}.xlsx");
        var pdfPath = Path.Combine(outputDirectory, $"informe_semana_{week}_{stamp}.pdf");

        // generate chart images into output directory
        var chartPaths = GenerateCharts(report, outputDirectory);

        ExportExcel(report, excelPath, chartPaths);
        ExportPdf(report, pdfPath, chartPaths);
        
        return pdfPath; // Return the path to the generated PDF
    }

    private static void ExportExcel(WeeklyReportDto report, string filePath, List<string>? chartPaths = null)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Resumen");

        sheet.Cell(1, 1).Value = "Informe Semanal";
        sheet.Cell(2, 1).Value = "Periodo";
        sheet.Cell(2, 2).Value = $"{report.PeriodStart:yyyy-MM-dd} to {report.PeriodEnd:yyyy-MM-dd}";

        sheet.Cell(4, 1).Value = "Entradas Totales";
        sheet.Cell(4, 2).Value = report.Totals.TotalEntries;
        sheet.Cell(5, 1).Value = "Salidas Totales";
        sheet.Cell(5, 2).Value = report.Totals.TotalExits;
        sheet.Cell(6, 1).Value = "Total Ventas Estimadas";
        sheet.Cell(6, 2).Value = report.Totals.TotalEstimatedSales;
        sheet.Cell(7, 1).Value = "Valor Total Ventas";
        sheet.Cell(7, 2).Value = report.Totals.TotalSalesValue;
        sheet.Cell(8, 1).Value = "Valor Total Devoluciones (Merma)";
        sheet.Cell(8, 2).Value = report.Totals.TotalReturnsValue;

        sheet.Cell(10, 1).Value = "Top Products (Exits)";
        sheet.Cell(11, 1).Value = "Product";
        sheet.Cell(11, 2).Value = "Quantity";
        var row = 12;
        foreach (var item in report.TopProducts)
        {
            sheet.Cell(row, 1).Value = item.ProductName;
            sheet.Cell(row, 2).Value = item.Quantity;
            row++;
        }

        // Top returns (mermas)
        row += 1;
        sheet.Cell(row, 1).Value = "Top Productos (Devoluciones - Merma)";
        row++;
        sheet.Cell(row, 1).Value = "Product";
        sheet.Cell(row, 2).Value = "Quantity";
        row++;
        foreach (var item in report.TopReturns)
        {
            sheet.Cell(row, 1).Value = item.ProductName;
            sheet.Cell(row, 2).Value = item.Quantity;
            row++;
        }

        row += 1;
        sheet.Cell(row, 1).Value = "Stock por Producto";
        row++;
        sheet.Cell(row, 1).Value = "Product";
        sheet.Cell(row, 2).Value = "Barcode";
        sheet.Cell(row, 3).Value = "Stock";
        row++;
        foreach (var item in report.StockByProduct)
        {
            sheet.Cell(row, 1).Value = item.ProductName;
            sheet.Cell(row, 2).Value = item.Barcode;
            sheet.Cell(row, 3).Value = item.Stock;
            row++;
        }

        // Client movements (consignación / devoluciones)
        if (report.ClientMovements != null && report.ClientMovements.Any())
        {
            row += 1;
            sheet.Cell(row, 1).Value = "Movimientos por Cliente";
            row++;
            sheet.Cell(row, 1).Value = "Cliente";
            sheet.Cell(row, 2).Value = "Producto";
            sheet.Cell(row, 3).Value = "Tipo";
            sheet.Cell(row, 4).Value = "Cantidad";
            sheet.Cell(row, 5).Value = "Precio unidad";
            sheet.Cell(row, 6).Value = "Valor total";
            row++;
            foreach (var cm in report.ClientMovements)
            {
                sheet.Cell(row, 1).Value = cm.ClientName;
                sheet.Cell(row, 2).Value = cm.ProductName;
                sheet.Cell(row, 3).Value = cm.MovementType;
                sheet.Cell(row, 4).Value = cm.Quantity;
                sheet.Cell(row, 5).Value = cm.UnitPrice;
                sheet.Cell(row, 6).Value = cm.TotalValue;
                row++;
            }
        }

        sheet.Columns().AdjustToContents();

        // insert charts if available
        if (chartPaths != null && chartPaths.Any())
        {
            var picRow = 20;
            foreach (var path in chartPaths)
            {
                try
                {
                    var pic = sheet.AddPicture(path).MoveTo(sheet.Cell(picRow, 1));
                    pic.Scale(0.5);
                    picRow += 18;
                }
                catch
                {
                    // ignore image insert errors
                }
            }
        }

        workbook.SaveAs(filePath);
    }

    private static void ExportPdf(WeeklyReportDto report, string filePath, List<string>? chartPaths = null)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Text("MonkeyArt Inventario - Informe Semanal").FontSize(18).SemiBold();

                page.Content().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Text($"Periodo: {report.PeriodStart:yyyy-MM-dd} a {report.PeriodEnd:yyyy-MM-dd}");

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Métrica");
                            header.Cell().Element(CellStyle).Text("Valor");
                        });

                            table.Cell().Element(CellStyle).Text("Entradas Totales");
                            table.Cell().Element(CellStyle).Text(report.Totals.TotalEntries.ToString());
                            table.Cell().Element(CellStyle).Text("Salidas Totales");
                            table.Cell().Element(CellStyle).Text(report.Totals.TotalExits.ToString());
                            table.Cell().Element(CellStyle).Text("Total Ventas Estimadas");
                            table.Cell().Element(CellStyle).Text(report.Totals.TotalEstimatedSales.ToString("F2"));
                            table.Cell().Element(CellStyle).Text("Valor Total Ventas");
                            table.Cell().Element(CellStyle).Text(report.Totals.TotalSalesValue.ToString("F2"));
                            table.Cell().Element(CellStyle).Text("Valor Total Devoluciones (Merma)");
                            table.Cell().Element(CellStyle).Text(report.Totals.TotalReturnsValue.ToString("F2"));
                    });

                    column.Item().Text("Productos principales (Salidas)").SemiBold();
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Producto");
                            header.Cell().Element(CellStyle).Text("Cantidad");
                        });

                        foreach (var item in report.TopProducts)
                        {
                            table.Cell().Element(CellStyle).Text(item.ProductName);
                            table.Cell().Element(CellStyle).Text(item.Quantity.ToString());
                        }
                    });

                    // Top returns (mermas)
                    column.Item().Text("Productos principales (Devoluciones - Merma)").SemiBold();
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Producto");
                            header.Cell().Element(CellStyle).Text("Cantidad");
                        });

                        foreach (var item in report.TopReturns)
                        {
                            table.Cell().Element(CellStyle).Text(item.ProductName);
                            table.Cell().Element(CellStyle).Text(item.Quantity.ToString());
                        }
                    });

                    column.Item().Text("Stock por Producto").SemiBold();
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Producto");
                            header.Cell().Element(CellStyle).Text("Código de barras");
                            header.Cell().Element(CellStyle).Text("Existencias");
                        });

                        foreach (var item in report.StockByProduct)
                        {
                            table.Cell().Element(CellStyle).Text(item.ProductName);
                            table.Cell().Element(CellStyle).Text(item.Barcode);
                            table.Cell().Element(CellStyle).Text(item.Stock.ToString());
                        }
                    });
                    // Client movements
                    if (report.ClientMovements != null && report.ClientMovements.Any())
                    {
                        column.Item().Text("Movimientos por Cliente").SemiBold();
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();
                                c.RelativeColumn();
                                c.RelativeColumn();
                                c.ConstantColumn(60);
                                c.ConstantColumn(80);
                                c.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Cliente");
                                header.Cell().Element(CellStyle).Text("Producto");
                                header.Cell().Element(CellStyle).Text("Tipo");
                                header.Cell().Element(CellStyle).Text("Cantidad");
                                header.Cell().Element(CellStyle).Text("Precio unidad");
                                header.Cell().Element(CellStyle).Text("Valor total");
                            });

                            foreach (var cm in report.ClientMovements)
                            {
                                table.Cell().Element(CellStyle).Text(cm.ClientName);
                                table.Cell().Element(CellStyle).Text(cm.ProductName);
                                table.Cell().Element(CellStyle).Text(cm.MovementType);
                                table.Cell().Element(CellStyle).Text(cm.Quantity.ToString());
                                table.Cell().Element(CellStyle).Text((cm.UnitPrice ?? 0m).ToString("F2"));
                                table.Cell().Element(CellStyle).Text((cm.TotalValue ?? 0m).ToString("F2"));
                            }
                        });
                    }

                    // Charts: embed generated PNG files into the PDF
                    if (chartPaths != null && chartPaths.Any())
                    {
                        column.Item().Text("Gráficos del informe").SemiBold();
                        foreach (var path in chartPaths)
                        {
                            try
                            {
                                if (File.Exists(path))
                                {
                                    var bytes = File.ReadAllBytes(path);
                                    column.Item().PaddingTop(8).Image(bytes, ImageScaling.FitWidth);
                                    column.Item().Text(" ");
                                }
                                else
                                {
                                    column.Item().Text($"(No se encontró: {Path.GetFileName(path)})");
                                }
                            }
                            catch
                            {
                                column.Item().Text($"(Error al incrustar: {Path.GetFileName(path)})");
                            }
                        }
                    }
                });
            });
        }).GeneratePdf(filePath);
    }

    private static List<string> GenerateCharts(WeeklyReportDto report, string outputDirectory)
    {
        var paths = new List<string>();

        try
        {
            Directory.CreateDirectory(outputDirectory);

            // Top products chart
            var top = report.TopProducts.Take(10).ToList();
            if (top.Any())
            {
                var topPath = Path.Combine(outputDirectory, "chart_top_products.png");
                DrawBarChart(top.Select(t => (Label: t.ProductName, Value: (double)t.Quantity)).ToList(), "Top Productos (Salidas)", topPath);
                paths.Add(topPath);
            }

            // Stock by product chart (take first 10)
            var stock = report.StockByProduct.Take(10).ToList();
            if (stock.Any())
            {
                var stockPath = Path.Combine(outputDirectory, "chart_stock_by_product.png");
                DrawBarChart(stock.Select(s => (Label: s.ProductName, Value: (double)s.Stock)).ToList(), "Stock por Producto", stockPath);
                paths.Add(stockPath);
            }

            // Top returns chart
            var returns = report.TopReturns.Take(10).ToList();
            if (returns.Any())
            {
                var returnsPath = Path.Combine(outputDirectory, "chart_top_returns.png");
                DrawBarChart(returns.Select(r => (Label: r.ProductName, Value: (double)r.Quantity)).ToList(), "Top Devoluciones (Merma)", returnsPath);
                paths.Add(returnsPath);
            }
        }
        catch
        {
            // ignore chart generation errors
        }

        return paths;
    }

    private static void DrawBarChart(List<(string Label, double Value)> items, string title, string outputPath)
    {
        const int width = 1200;
        const int height = 480;
        using var bmp = new System.Drawing.Bitmap(width, height);
        using var g = System.Drawing.Graphics.FromImage(bmp);
        g.Clear(System.Drawing.Color.White);

        var marginLeft = 100;
        var marginBottom = 110;
        var marginTop = 60;
        var chartWidth = width - marginLeft - 60;
        var chartHeight = height - marginTop - marginBottom;

        // title
        using (var fontTitle = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold))
        using (var brushTitle = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(30, 40, 70)))
        {
            g.DrawString(title, fontTitle, brushTitle, new System.Drawing.PointF(marginLeft, 8));
        }

        if (!items.Any())
        {
            bmp.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            return;
        }

        var maxVal = items.Max(i => i.Value);
        if (maxVal <= 0) maxVal = 1;

        var gap = 16;
        var barWidth = Math.Max(24, (chartWidth - gap * (items.Count + 1)) / items.Count);
        var x = marginLeft + gap;

        using var font = new System.Drawing.Font("Segoe UI", 10);
        using var brushText = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(40, 40, 40));
        using var penAxis = new System.Drawing.Pen(System.Drawing.Color.FromArgb(120, 120, 120), 1);

        // draw axes
        g.DrawLine(penAxis, marginLeft, marginTop, marginLeft, marginTop + chartHeight);
        g.DrawLine(penAxis, marginLeft, marginTop + chartHeight, marginLeft + chartWidth, marginTop + chartHeight);

        var palette = new[] { System.Drawing.Color.FromArgb(46, 125, 150), System.Drawing.Color.FromArgb(102, 187, 106), System.Drawing.Color.FromArgb(255, 167, 38), System.Drawing.Color.FromArgb(126, 87, 194), System.Drawing.Color.FromArgb(239, 83, 80) };

        for (int i = 0; i < items.Count; i++)
        {
            var val = items[i].Value;
            var barHeight = (float)(val / maxVal * (double)chartHeight);
            var rect = new System.Drawing.RectangleF(x, marginTop + chartHeight - barHeight, barWidth, barHeight);
            var color = palette[i % palette.Length];
            using var brushBar = new System.Drawing.SolidBrush(color);
            g.FillRectangle(brushBar, rect);
            g.DrawRectangle(System.Drawing.Pens.White, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);

            // label (wrap/trim)
            var label = items[i].Label.Length > 24 ? items[i].Label.Substring(0, 21) + "..." : items[i].Label;
            var labelSize = g.MeasureString(label, font);
            var labelX = x + (barWidth - labelSize.Width) / 2;
            g.DrawString(label, font, brushText, new System.Drawing.PointF(labelX, marginTop + chartHeight + 6));

            // value above bar
            var valStr = items[i].Value.ToString("0.##");
            var valSize = g.MeasureString(valStr, font);
            g.DrawString(valStr, font, brushText, new System.Drawing.PointF(x + (barWidth - valSize.Width) / 2, marginTop + chartHeight - barHeight - 18));

            x += barWidth + gap;
        }

        bmp.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
    }

    // Public wrapper to generate chart images for external callers (e.g., UI viewmodels)
    public List<string> GenerateChartImages(WeeklyReportDto report, string outputDirectory)
    {
        return GenerateCharts(report, outputDirectory);
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container.Border(1).BorderColor("#DDDDDD").Padding(4);
    }

    private static bool IsIncrease(MovementType type)
    {
        return type == MovementType.Entrada || type == MovementType.Ajuste;
    }
}
