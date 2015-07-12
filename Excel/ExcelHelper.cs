using System;
using System.Data;
using System.Data.OleDb;
using Microsoft.Office.Interop.Excel;
using System.Web;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Excel
{
    public class ExcelHelper
    {

        public System.Data.DataTable ExcelToDataTable(string fileName,HDR hdr)
        {
            System.Data.DataTable dt;
            string HeadFlag = hdr == 0 ? "NO" : "YES";
            string conStr = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR={1};IMEX=1'", fileName, HeadFlag);
            OleDbConnection myConn = new OleDbConnection(conStr);
            string strCom = " SELECT * FROM [Sheet1$]";
            myConn.Open();
            OleDbDataAdapter myCommand = new OleDbDataAdapter(strCom, myConn);
            dt = new System.Data.DataTable();
            myCommand.Fill(dt);
            myConn.Close();
            return dt;
        }

        [System.Flags]
        public enum HDR
        {
            NO=0,
            YES=1
        }


        public void DataTabletoExcel(System.Data.DataTable tmpDataTable, string strFileName)
        {

            if (tmpDataTable == null)
                return;

            int rowNum = tmpDataTable.Rows.Count;
            int columnNum = tmpDataTable.Columns.Count;
            int rowIndex = 1;
            int columnIndex = 0;

            Application xlApp = new Application();
            xlApp.DefaultFilePath = "";
            xlApp.DisplayAlerts = true;
            xlApp.SheetsInNewWorkbook = 1;
            Workbook xlBook = xlApp.Workbooks.Add(true);


            //将Cell格式设置成文本

            //将DataTable的列名导入Excel表第一行
            foreach (DataColumn dc in tmpDataTable.Columns)
            {
                columnIndex++;
                xlApp.Cells[rowIndex, columnIndex] = dc.ColumnName;
            }
            //将DataTable中的数据导入Excel中
            for (int i = 0; i < rowNum; i++)
            {

                rowIndex++;
                columnIndex = 0;
                for (int j = 0; j < columnNum; j++)
                {
                    columnIndex++;
                    if (tmpDataTable.Rows[i][j].ToString()[0]=='0')
                    {
                        ((Range)(xlApp.Cells[rowIndex, columnIndex])).NumberFormat = "@";
                    }
                    xlApp.Cells[rowIndex, columnIndex] = tmpDataTable.Rows[i][j].ToString();

                }
            }

            Range cells = xlBook.Worksheets[1].Cells;
            cells.HorizontalAlignment = XlHAlign.xlHAlignLeft;

            xlBook.SaveCopyAs(strFileName);
            xlBook.Close(false);

            /*
            if (xlApp != null)
            {
                xlApp.Workbooks.Close();
                xlApp.Quit();
                int generation = GC.GetGeneration(xlApp);
                Marshal.ReleaseComObject(xlApp);
                xlApp = null;
                GC.Collect(generation);
            }
            */

            GC.Collect();
            Process[] excelProc = Process.GetProcessesByName("EXCEL");
            foreach (Process p in excelProc)
            {
                p.Kill();
            }
            
        }

    }
}
