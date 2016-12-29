using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubanMovieExport
{
    public class Export2Excel
    {
        public void Export()
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];

            worksheet.Cells[1, 1] = "标题";
            worksheet.Cells[1, 2] = "副标题";
            worksheet.Cells[1, 3] = "URL";
            worksheet.Cells[1, 4] = "封面图片";
            worksheet.Cells[1, 5] = "影片介绍";
            worksheet.Cells[1, 6] = "标签";
            worksheet.Cells[1, 7] = "评论";
            worksheet.Cells[1, 8] = "创建时间";

        }

        /// <summary> 
        /// 合并单元格，并赋值，对指定WorkSheet操作 
        /// </summary> 
        /// <param name="sheetIndex">WorkSheet索引</param> 
        /// <param name="beginRowIndex">开始行索引</param> 
        /// <param name="beginColumnIndex">开始列索引</param> 
        /// <param name="endRowIndex">结束行索引</param> 
        /// <param name="endColumnIndex">结束列索引</param> 
        /// <param name="text">合并后Range的值</param> 
        private void MergeCells(Microsoft.Office.Interop.Excel.Worksheet workSheet, int beginRowIndex, int beginColumnIndex, int endRowIndex, int endColumnIndex, string text)
        {
            Microsoft.Office.Interop.Excel.Range range = workSheet.get_Range(workSheet.Cells[beginRowIndex, beginColumnIndex], workSheet.Cells[endRowIndex, endColumnIndex]);
            range.ClearContents(); //先把Range内容清除，合并才不会出错 
            range.MergeCells = true;
            range.Value2 = text;
            range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
        }
    }
}
