using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using System.ComponentModel;


namespace course
{
    class ExcelGenerator
    {
        private string chartFilePath = @"C:\Users\Пользователь\Desktop\Chart.xlsx";
        public ExcelGenerator()
        {
        }
        public void GenerateChart(BindingList<part> partList)
        {
            var excelApp = new Excel.Application();
            var workbook = excelApp.Workbooks.Add();
            var worksheet = workbook.Worksheets[1];

            var Groups = partList.GroupBy(c => c.Price); // заполнение данных для диаграммы
            int row = 1;
            foreach (var group in Groups)
            {
                worksheet.Cells[row, 1] = group.Key;
                worksheet.Cells[row, 2] = group.Count() * 1000;
                row++;
            }

            var charts = worksheet.ChartObjects() as Excel.ChartObjects; // создание диаграммы
            var chartObject = charts.Add(60, 10, 300, 300);
            var chart = chartObject.Chart;

            var range = worksheet.Range["A1:B" + (row - 1)]; // выбор диапазона данных для диаграммы
            chart.SetSourceData(range);
            chart.ChartType = Excel.XlChartType.xlColumnClustered; // установка типа диаграммы
            chart.HasLegend = false;
            chart.HasTitle = true;
            chart.ChartTitle.Text = "График цен и количества";



            workbook.SaveAs(chartFilePath);
            workbook.Close();
            excelApp.Quit();
        }

        public void AddChartToReport()
        {
            var wordApp = new Word.Application();
            var document = wordApp.Documents.Open(@"C:\Users\Пользователь\source\repos\course\course\Graphics.doc");

            var range = document.Content;
            range.InsertAfter("\n");

            var oleObjects = document.InlineShapes.AddOLEObject(
                ClassType: "Excel.Chart",
                FileName: chartFilePath,
                LinkToFile: false,
                DisplayAsIcon: false,
                IconFileName: "",
                Range: document.Range(document.Content.End - 1, document.Content.End - 1)
            );

            document.Save();
            document.Close();
            wordApp.Quit();
        }


    }
}
