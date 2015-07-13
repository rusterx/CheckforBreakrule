using System;
using System.Data;
using System.Data.OleDb;
using ExcelApp=Microsoft.Office.Interop.Excel;
using System.Web;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Excel
{
    public class ExcelHelper
    {


        [System.Flags]
        public enum HDR
        {
            NO=0,
            YES=1
        }


        /// <summary>
        /// 将Excel转化成表格
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="hdr"></param>
        /// <returns></returns>
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



        /// <summary>
        /// 将表格转换成Excel表格
        /// </summary>
        /// <param name="tmpDataTable"></param>
        /// <param name="strFileName"></param>
        /// <param name="rows"></param>
        public void DataTabletoExcel(System.Data.DataTable tmpDataTable, string strFileName, int rows)
        {

            if (tmpDataTable == null)
                return;

            //暂时没明白
            if (rows == 0)
            {
                MessageBox.Show("你当前的任务数量为0，程序无法为您保存结果！");
                KillExcel();
                return;
            }

            int rowNum = tmpDataTable.Rows.Count;
            int columnNum = tmpDataTable.Columns.Count;
            int rowIndex = 1;
            int columnIndex = 0;

            ExcelApp.Application xlApp = new ExcelApp.Application();
            xlApp.DefaultFilePath = "";
            xlApp.DisplayAlerts = true;
            xlApp.SheetsInNewWorkbook = 1;
            ExcelApp.Workbook xlBook = xlApp.Workbooks.Add(true);

            foreach (DataColumn dc in tmpDataTable.Columns)
            {
                columnIndex++;
                xlApp.Cells[rowIndex, columnIndex] = dc.ColumnName;
            }

            for (int i = 0; i < rowNum; i++)
            {
                
                rowIndex++;
                columnIndex = 0;
                for (int j = 0; j < columnNum; j++)
                {
                    columnIndex++;
                    if (tmpDataTable.Rows[i][j].ToString()[0]=='0')
                    {
                        ((ExcelApp.Range)(xlApp.Cells[rowIndex, columnIndex])).NumberFormat = "@";
                    }
                    xlApp.Cells[rowIndex, columnIndex] = tmpDataTable.Rows[i][j].ToString();

                }
            }

            ExcelApp.Range cells = xlBook.Worksheets[1].Cells;
            cells.HorizontalAlignment = ExcelApp.XlHAlign.xlHAlignLeft;
            xlBook.SaveCopyAs(strFileName);
            xlBook.Close(false);

            KillExcel();
            
        }

        /// <summary>
        /// 结束Excel进程
        /// </summary>
        public void KillExcel()
        {
            GC.Collect();
            Process[] excelProc = Process.GetProcessesByName("EXCEL");
            foreach (Process p in excelProc)
            {
                p.Kill();
            }
        }
    }
}


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
