using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ILoyInterview.Domain.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ILoyInterview.Domain.Reports
{
    public class ReportGenerator
    {
        private readonly ProjectTaskManager _manager;

        public ReportGenerator(ProjectTaskManager manager)
        {
            _manager = manager;
        }

        // generate excel document with in progress tasks list including project description for every task
        // better would be to clarify how exactly document should look like, but I don't want to spend to much time on this test project
        public async Task<MemoryStream> Generate(DateTime reportDate, CancellationToken token)
        {
            var tasks = await _manager.GetTasksWithProjectsAsync(reportDate, token);

            var memoryStream = new MemoryStream();

            using (var spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                var workbookpart = spreadsheetDocument.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                var sheets = spreadsheetDocument.WorkbookPart.Workbook.
                    AppendChild<Sheets>(new Sheets());

                var sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.
                    GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Report"
                };

                for (var i = 0; i < tasks.Count; i++)
                {
                    var row = new Row()
                    {
                        RowIndex = (uint)(i + 1),
                        Height = 20,
                        CustomHeight = true
                    };

                    var header1 = new Cell()
                    {
                        CellReference = "A" + (i + 1),
                        CellValue = new CellValue(tasks[i].Project.Name.ToString()),
                        DataType = CellValues.String
                    };
                    row.Append(header1);

                    var header2 = new Cell()
                    {
                        CellReference = "B" + (i + 1),
                        CellValue = new CellValue(tasks[i].Project.Code),
                        DataType = CellValues.String
                    };

                    row.Append(header2);

                    var header3 = new Cell()
                    {
                        CellReference = "C" + (i + 1),
                        CellValue = new CellValue(tasks[i].Project.StartDate.ToString()),
                        DataType = CellValues.String
                    };
                    row.Append(header3);

                    var header4 = new Cell()
                    {
                        CellReference = "D" + (i + 1),
                        CellValue = new CellValue(tasks[i].Project.FinishDate.ToString()),
                        DataType = CellValues.String
                    };

                    row.Append(header4);

                    var header5 = new Cell()
                    {
                        CellReference = "E" + (i + 1),
                        CellValue = new CellValue(tasks[i].Name),
                        DataType = CellValues.String
                    };
                    row.Append(header5);

                    var header6 = new Cell()
                    {
                        CellReference = "F" + (i + 1),
                        CellValue = new CellValue(tasks[i].Description),
                        DataType = CellValues.String
                    };

                    row.Append(header6);

                    var header7 = new Cell()
                    {
                        CellReference = "G" + (i + 1),
                        CellValue = new CellValue(tasks[i].StartDate.ToString()),
                        DataType = CellValues.String
                    };
                    row.Append(header7);

                    var header8 = new Cell()
                    {
                        CellReference = "H" + (i + 1),
                        CellValue = new CellValue(tasks[i].FinishDate.ToString()),
                        DataType = CellValues.String
                    };

                    row.Append(header8);

                    sheetData.Append(row);
                }

                var columns = worksheetPart.Worksheet.GetFirstChild<Columns>();
                var needToInsertColumns = false;
                
                if (columns == null)
                {
                    columns = new Columns();
                    needToInsertColumns = true;
                }

                columns.Append(new Column() { Min = 1, Max = 1, Width = 12, CustomWidth = true });
                columns.Append(new Column() { Min = 2, Max = 2, Width = 10, CustomWidth = true });
                columns.Append(new Column() { Min = 3, Max = 3, Width = 10, CustomWidth = true });

                if (needToInsertColumns)
                    worksheetPart.Worksheet.InsertAt(columns, 0);

                var cellFormats = new CellFormats() { Count = (UInt32Value)2U };

                var cellFormat = new CellFormat()
                {
                    NumberFormatId = (UInt32Value)0U,
                    FontId = (UInt32Value)0U,
                    FillId = (UInt32Value)0U,
                    BorderId = (UInt32Value)0U,
                    FormatId = (UInt32Value)0U,
                    ApplyAlignment = true,
                    Alignment = new Alignment
                    {
                        WrapText = true
                    }
                };

                var alignment = new Alignment() { WrapText = true };

                cellFormat.Append(alignment);

                cellFormats.Append(cellFormat);

                worksheetPart.Worksheet.InsertAt(cellFormats, 0);

                sheets.Append(sheet);

                workbookpart.Workbook.Save();

                spreadsheetDocument.Close();

                memoryStream.Seek(0, SeekOrigin.Begin);

                return memoryStream;
            }
        }
    }
}
