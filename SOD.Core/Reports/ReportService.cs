using FastReport.Export.PdfSimple;
using FastReport.Utils;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using SOD.Core.CustomUnits;
using SOD.Core.Infrastructure;
using SOD.Core.JsonConverters;
using SOD.Core.Props;
using SOD.Core.Seals;
using Spire.Pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Serialization.JsonNet;

namespace SOD.Core.Reports
{
    public class ReportService : IReportService
    {
        private const string rawReportTableName = "raw_reports";
        private string dbPath = Path.Combine(Directory.GetCurrentDirectory(), CoreConst.DatabaseFolder, "reports.db");
        private ILogger logger = LogManager.GetCurrentClassLogger();
        private Subject<Report> observeableReport = new Subject<Report>();
        private List<Stream> printedReport = null;
        private int pageNum = 0;
        PrintDocument pd = new PrintDocument();
        private PDFSimpleExport export = new PDFSimpleExport();
        public ReportService()
        {
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), CoreConst.DatabaseFolder)))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), CoreConst.DatabaseFolder));
            }

            BsonMapper.Global.Entity<Report>()
                .DbRef(x => x.RawReport, rawReportTableName)
                .Ignore(x => x.ReportData);

            BsonMapper.Global.RegisterType<Stream>(
                serialize: s =>
                 {
                     s.Seek(0, SeekOrigin.Begin);
                     using (var ms = new MemoryStream())
                     {
                         s.CopyTo(ms);
                         return ms.ToArray();
                     }
                 },
                deserialize: bson =>
                 {
                     var stream = new MemoryStream();
                     stream.Write(bson.AsBinary, 0, bson.AsBinary.Count());
                     stream.Seek(0, SeekOrigin.Begin);
                     return stream;
                 });

            var jsonSerializeSettings = new JsonSerializerSettings();
            jsonSerializeSettings.Converters.Add(new UnitsNetIQuantityJsonConverter());
            BsonMapper.Global.RegisterType<Property>
                (
                    serialize: (valveProperty) =>
                    {
                        valveProperty.SerializedValue = JsonConvert.SerializeObject(valveProperty.Value, jsonSerializeSettings);
                        return JsonConvert.SerializeObject(valveProperty, jsonSerializeSettings);

                    },
                    deserialize: (bson) =>
                    {
                        var property = JsonConvert.DeserializeObject<Property>(bson.AsString, jsonSerializeSettings);
                        switch (property.Type)
                        {
                            case PropertyType.Pressure:
                                property.Value = JsonConvert.DeserializeObject<Pressure>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.VolumeFlow:
                                property.Value = JsonConvert.DeserializeObject<VolumeFlow>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Volume:
                                property.Value = JsonConvert.DeserializeObject<Volume>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Area:
                                property.Value = JsonConvert.DeserializeObject<Area>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Lenght:
                                property.Value = JsonConvert.DeserializeObject<Length>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.PNTable:
                                property.Value = JsonConvert.DeserializeObject<DinPressureClass>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.AnsiClassTable:
                                property.Value = JsonConvert.DeserializeObject<AnsiPressureClass>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.String:
                                property.Value = JsonConvert.DeserializeObject<string>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Double:
                                property.Value = JsonConvert.DeserializeObject<double>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.Integer:
                                property.Value = JsonConvert.DeserializeObject<int>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.NPSMetric:
                                property.Value = JsonConvert.DeserializeObject<NominalPipeSizeMetric>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.NPSInch:
                                property.Value = JsonConvert.DeserializeObject<NominalPipeSizeInch>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.StringList:
                                property.Value = JsonConvert.DeserializeObject<List<object>>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.SealType:
                                property.Value = JsonConvert.DeserializeObject<SealType>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            case PropertyType.TestMedium:
                                property.Value = JsonConvert.DeserializeObject<TestMediumType>(property.SerializedValue, jsonSerializeSettings);
                                break;
                            default:
                                break;
                        }
                        return property;
                    }
                );

            
            var printerSettings = new PrinterSettings();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                printerSettings.PrinterName = printer;
                if (printerSettings.IsDefaultPrinter) break;
            }
            pd.PrinterSettings.PrinterName = printerSettings.PrinterName;
            pd.DefaultPageSettings.Margins = new Margins(25, 25, 25, 25);

            pd.PrintPage += (s, e) =>
            {
                printedReport[pageNum].Seek(0, SeekOrigin.Begin);
                e.Graphics.DrawImage(Image.FromStream(printedReport[pageNum]), e.MarginBounds);
                if (pageNum < printedReport.Count - 1)
                {
                    e.HasMorePages = true;
                    pageNum++;
                }
            };
        }
        public async Task<Report> CreateReportAsync(BaseReportData reportsData, string reportTemplate)
        {
            if (reportTemplate == null) throw new ArgumentNullException(reportTemplate);
            return await Task.Run(() =>
            {
                if (CurrentReport != null)
                {
                    CurrentReport?.RawReport.Report.Dispose();
                }

                var report = new Report();
                report.Number = GetReportNumber();
                report.RawReport.Report = new MemoryStream();
                report.Date = DateTime.Now;
                report.ReportData = reportsData;
                report.ReportTemplate = reportTemplate;
                try
                {
                    var freport = MakeFastReport(reportTemplate, reportsData);
                    freport.SetParameterValue("ReportNumber", report.Number);

                    freport.Save(reportTemplate);
                    freport.Prepare();
                    freport.SavePrepared(report.RawReport.Report);
                    
                }
                catch (Exception e)
                {
                    logger.Warn(e, "Ошибка генерации протокола");
                }

                report.IsSave = false;
                observeableReport.OnNext(report);
                CurrentReport = report;
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                //SaveReportTemplate();
                return report;
            });
        }

        public void Save(Report report)
        {
            
            if (report == null) throw new ArgumentNullException(nameof(report));

            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                report.IsSave = true;
                db.GetCollection<RawReport>(rawReportTableName).Insert(report.RawReport);
                db.GetCollection<Report>().Insert(report);
            }
        }

        public async Task RefreshAsync()
        {
            if (CurrentReport!=null)
            {
               await CreateReportAsync(CurrentReport.ReportData, CurrentReport.ReportTemplate);
            }
        }

        private FastReport.Report MakeFastReport(string templatePath, BaseReportData baseReportData)
        {
            var freport = new FastReport.Report();
            freport.Load(templatePath);
            foreach (var data in baseReportData.GetData())
            {
                if (data.Value is DataSet dataSet)
                {
                    freport.RegisterData(dataSet, data.Key, true);
                }
                else
                {
                    freport.RegisterData(new List<object> { data.Value }, data.Key, FastReport.Data.BOConverterFlags.AllowFields, 10);
                    freport.GetDataSource(data.Key).Enabled = true;
                }
            }
            return freport;
        }

        public Task<int> EditReportTemplate(string reportPath)
        {
            if (reportPath == null || !reportPath.Contains(".frx")) return Task.FromResult<int>(1);

            var tcs = new TaskCompletionSource<int>();

            //Подготавливаем открытие шаблона протокола
            var process = new Process
            {
                StartInfo = { FileName = reportPath, UseShellExecute = true },
                EnableRaisingEvents = true
            };

            //Дожидаемся закрытие шаблона
            process.Exited += (sender, args) =>
            {
                tcs.SetResult(process.ExitCode);
                process.Dispose();
            };

            try
            {
                process.Start();

                return tcs.Task;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                return Task.FromResult<int>(1);
            }
        }

        public IEnumerable<Report> GetReports()
        {
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                return db.GetCollection<Report>().FindAll().ToList();
            }
        }

        public Report GetReport(int id)
        {
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                return db.GetCollection<Report>().Include(r=>r.RawReport).FindById(id);
            }
        }

        public async  Task PrintAsync(Report report)
        {
            pageNum = 0;
            if (printedReport!=null)
            {
                foreach (var pr in printedReport)
                {
                    pr.Dispose();
                }
            }
            
            printedReport = await ReportToImages(report);
            pd.Print();
        }

        public async Task<List<Stream>> ReportToImages(Report report)
        {
            return await Task.Run(async () =>
            {
                var result = new List<Stream>();
                if (report.RawReport.Report == null) throw new ArgumentNullException("Raw report stream is null");
                report.RawReport.Report.Seek(0, SeekOrigin.Begin);
                using (var freport = new FastReport.Report())
                {
                    using (var ms = new MemoryStream())
                    {
                        await report.RawReport.Report.CopyToAsync(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        freport.LoadPrepared(ms);
                        using (var imageExport = new FastReport.Export.Image.ImageExport())
                        {

                            imageExport.ImageFormat = FastReport.Export.Image.ImageExportFormat.Jpeg;
                            imageExport.JpegQuality = 100;
                            imageExport.Resolution = 100;
                            imageExport.SeparateFiles = true;
                            imageExport.Export(freport, Path.Combine(Path.GetTempPath(), "report.jpg"));
                            for (int i = 0; i < imageExport.GeneratedFiles.Count; i++)
                            {
                                var fileStream = new MemoryStream();
                                using (var page = new FileStream(Path.Combine(Path.GetTempPath(), imageExport.GeneratedFiles[i]), FileMode.Open))
                                {
                                    page.CopyTo(fileStream);
                                    fileStream.Seek(0, SeekOrigin.Begin);
                                    result.Add(fileStream);
                                }
                            }
                        }
                    }
                    
                }
                return result;
            });
        }

        public void Remove(int id)
        {
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                db.GetCollection<Report>().Include(r => r.RawReport).Delete(id);
                db.GetCollection<RawReport>(rawReportTableName).Delete(id);
            }
        }

        public int GetReportNumber()
        {
            using (var db = new LiteDatabase(dbPath, BsonMapper.Global))
            {
                var reports = db.GetCollection<Report>().FindAll();
                if (reports.Count()>0)
                {
                    return reports.Select(r => r.Number).Max() + 1;
                }
                else
                {
                    return 1;
                }
            }
        }

        public IObservable<Unit> Export(Report report, string path)
        {
            if (!Directory.Exists(path)) throw new ArgumentException("Path не существует");
            if (report.RawReport == null) throw new ArgumentException("Протокол");
            return Observable.Create<Unit>(obs =>
            {
                return Task.Run(() =>
                {
                    try
                    {
                        using (var freport = new FastReport.Report())
                        {
                            using (var ms = new MemoryStream())
                            {
                                report.RawReport.Report.Seek(0, SeekOrigin.Begin);
                                report.RawReport.Report.CopyTo(ms);
                                ms.Seek(0, SeekOrigin.Begin);
                                freport.LoadPrepared(ms);
                                export.Export(freport, Path.Combine(path, $"Report {report.Number} {report.Date.ToString("dd\\_MM\\_yyyy")}.pdf"));
                                obs.OnCompleted();
                            }
                        }

                        // После экспорта в пдф - экспортируем в Excel
                        //PdfDocument pdf = new PdfDocument();
                        //pdf.LoadFromFile(Path.Combine(path, $"Report {report.Number} {report.Date.ToString("dd\\_MM\\_yyyy")}.pdf"));
                        //pdf.SaveToFile(Path.Combine(path, $"Report {report.Number} {report.Date.ToString("dd\\_MM\\_yyyy")}.xlsx"), FileFormat.XLSX);
                    }
                    catch (Exception e)
                    {

                        logger.Error(e, "Ошибка экспорта");
                        obs.OnError(e);
                    }
                });
                
            });
            
        }

        public void SaveReportTemplate()
        {
        }

        public IObservable<Report> Report => observeableReport;

        public Report CurrentReport { get; private set; }
    }
}
