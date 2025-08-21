using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Indium.Common.DataTransferObjects;
using VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment;
using HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment;

namespace Indium.Common.Modules
{
    public class CreateFlatExcel
    {
        private class Rows
        {
            int RowIndex = 0;
            public int Get()
            {
                RowIndex++;
                return RowIndex - 1;
            }
        }

        private static void CreateCell(IRow Row, int Col, string Value, XSSFCellStyle Style, string dataType = "string")
        {
            ICell Cell = Row.CreateCell(Col);
            switch (dataType)
            {
                case "double":
                    double val_double;
                    Double.TryParse(Value, out val_double);
                    Cell.SetCellValue(val_double);
                    break;
                case "int":
                    int val_int;
                    int.TryParse(Value, out val_int);
                    Cell.SetCellValue(val_int);
                    break;
                case "date":
                    break;
                default:
                    Cell.SetCellValue(Value);
                    break;
            }
            Cell.CellStyle = Style;
        }
        public static async Task<NpoiMemoryStream> StreamExcelFile<T>(FlatExcelOptions<T> options, ILogger _logger)
        {
            NpoiMemoryStream stream = new NpoiMemoryStream();

            // Workbook
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet Sheet;

            // --- Fonts --- //
            //Default Font
            XSSFFont defultFont = (XSSFFont)workbook.CreateFont();
            defultFont.FontHeightInPoints = 11;
            defultFont.FontName = "Tahoma";

            //Header Font
            XSSFFont labelFont = (XSSFFont)workbook.CreateFont();
            labelFont.FontHeightInPoints = 12;
            labelFont.FontName = "Tahoma";
            labelFont.IsBold = (true);



            // --- Style --- //
            // default Style
            XSSFCellStyle defaultCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            defaultCellStyle.SetFont(defultFont);
            defaultCellStyle.BorderLeft = BorderStyle.None;
            defaultCellStyle.BorderTop = BorderStyle.None;
            defaultCellStyle.BorderRight = BorderStyle.None;
            defaultCellStyle.BorderBottom = BorderStyle.None;
            defaultCellStyle.VerticalAlignment = VerticalAlignment.Top;
            defaultCellStyle.Alignment = HorizontalAlignment.Right;

            // double Style
            XSSFCellStyle doubleCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            doubleCellStyle.SetFont(defultFont);
            doubleCellStyle.BorderLeft = BorderStyle.None;
            doubleCellStyle.BorderTop = BorderStyle.None;
            doubleCellStyle.BorderRight = BorderStyle.None;
            doubleCellStyle.BorderBottom = BorderStyle.None;
            doubleCellStyle.VerticalAlignment = VerticalAlignment.Top;
            doubleCellStyle.Alignment = HorizontalAlignment.Right;
            doubleCellStyle.SetDataFormat(workbook.CreateDataFormat().GetFormat("0.00"));

            // header style
            XSSFCellStyle labelCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            labelCellStyle.SetFont(labelFont);
            labelCellStyle.BorderLeft = BorderStyle.None;
            labelCellStyle.BorderTop = BorderStyle.None;
            labelCellStyle.BorderRight = BorderStyle.None;
            labelCellStyle.BorderBottom = BorderStyle.None;
            labelCellStyle.VerticalAlignment = VerticalAlignment.Top;
            labelCellStyle.Alignment = HorizontalAlignment.Center;

            if (options.Items.Count == 0) // No items, cannot retrieve object property info. Exit here, output blank workbook
            {
                Sheet = workbook.CreateSheet(options.SheetName ?? "Sheet1"); // Create a blank sheet, without this the file is corrupt
                if (!string.IsNullOrEmpty(options.SheetPassword))
                {
                    Sheet.ProtectSheet(options.SheetPassword);
                }
                stream = new NpoiMemoryStream();
                stream.AllowClose = false;
                workbook.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);
                stream.AllowClose = true;
                return stream;
            }

            //Create sheet
            Sheet = workbook.CreateSheet(options.SheetName ?? "Sheet1");
            if (!string.IsNullOrEmpty(options.SheetPassword))
            {
                Sheet.ProtectSheet(options.SheetPassword);
            }

            //Header Row
            Rows rn = new Rows();

            //Create Headers
            //Headers are Tuple<dbColumn, headerText, dataType>
            IRow HeaderRow = Sheet.CreateRow(rn.Get());
            List<string> columnSources = new List<string>();

            int currentHeaderCol = 0;
            foreach (Tuple<string, string, string> header in options.Headers)
            {
                CreateCell(HeaderRow, currentHeaderCol, header.Item2, labelCellStyle);
                columnSources.Add(header.Item1); // Add db column ref to columns
                currentHeaderCol++;
            }

            //Create Info Rows
            IRow InfoRow = Sheet.CreateRow(rn.Get());
            foreach (T item in options.Items)
            {
                Type itemType = item.GetType();
                PropertyInfo[] propertyInfos = itemType.GetProperties();

                int currentCol = 0;
                foreach (string colSource in columnSources)
                {
                    // foreach (PropertyInfo propertyInfo in propertyInfos)
                    // {
                    var value = item.GetType().GetProperty(colSource).GetValue(item, null);
                    // If the property name is not in the header options, skip it
                    //if (!columnSources.Contains(propertyInfo.Name))
                    //{
                    //    continue;
                    //}

                    //var getMethod = propertyInfo.GetGetMethod();
                    //if (getMethod != null) // null if either the property is non-public, or it has no get method
                    //{
                    //  var value = getMethod.Invoke(item, null);

                    XSSFCellStyle cellStyle;

                    if (options.Headers[currentCol].Item3 == "double")
                    {
                        cellStyle = doubleCellStyle;
                    }
                    else
                    {
                        cellStyle = defaultCellStyle;
                    }

                    CreateCell(InfoRow, currentCol, (value ?? "").ToString(), cellStyle, options.Headers[currentCol].Item3);
                    // }

                    currentCol++;
                    // }
                }

                InfoRow = Sheet.CreateRow(rn.Get()); // New row replace previous row, loop
            }

            int lastColumNum = Sheet.GetRow(0).LastCellNum;
            for (int i = 0; i <= lastColumNum; i++)
            {
                Sheet.AutoSizeColumn(i);
                GC.Collect();
            }

            stream = new NpoiMemoryStream();
            stream.AllowClose = false;
            workbook.Write(stream);
            stream.Seek(0, SeekOrigin.Begin);
            stream.AllowClose = true;
            return stream;
        }
    }
}