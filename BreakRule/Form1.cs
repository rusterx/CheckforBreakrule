using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using weizhang;

namespace BreakRule
{


    public partial class Form1 : Form
	{
    
        /*
		[CompilerGenerated]
		private static class <ErrorCrd_ExportToExcel>o__SiteContainer0
		{
			public static CallSite<Func<CallSite, object, Worksheet>> <>p__Site1;
			public static CallSite<Func<CallSite, object, Range>> <>p__Site2;
		}
    
        */
		//private IContainer components;
		//private Label labTips;

		public Form1()
		{
			InitializeComponent();
		}

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Click += new EventHandler(btnImport_Click);
        }

		private void btnImport_Click(object sender, EventArgs e)
		{
			//this.labTips.Visible = false;
			List<MVehicle> vehicleLs = new List<MVehicle>();
			//OpenFileDialog fd = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				//this.labTips.Text = "正在查询...";
				//this.labTips.Visible = true;
                string fileName = openFileDialog1.FileName;
				DataSet ds = this.ExcelToDS(fileName);
				for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
				{
					vehicleLs.Add(new MVehicle
					{
						CardNo = ds.Tables[0].Rows[i][0].ToString(),
						VinNo = ds.Tables[0].Rows[i][1].ToString()
					});
				}
                MessageBox.Show("go");
				ValidCodeBll codeBll = new ValidCodeBll();
				DateTime arg_E9_0 = DateTime.Now;
                
				foreach (MVehicle temp in vehicleLs)
				{
					if (!string.IsNullOrEmpty(temp.CardNo) && !string.IsNullOrEmpty(temp.VinNo))
					{
                      
						temp.CheckResult = codeBll.GetCheckResult(temp.CardNo, temp.VinNo);

					}
				}
 
                System.Data.DataTable dt = new System.Data.DataTable();
				dt.Columns.Add("车牌号", Type.GetType("System.String"));
				dt.Columns.Add("车架号", Type.GetType("System.String"));
				DataColumn dc = new DataColumn("结果", Type.GetType("System.String"));
				dt.Columns.Add(dc);
				foreach (MVehicle temp2 in vehicleLs)
				{
					DataRow dr = dt.NewRow();
					dr["车牌号"] = temp2.CardNo;
					dr["车架号"] = temp2.VinNo;
					dr["结果"] = temp2.CheckResult;
					dt.Rows.Add(dr);
				}
				//Form1.ErrorCrd_ExportToExcel(dt, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "查询结果" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx"));
				//this.labTips.Text = "查询完成，已导出";
			}
		}

        /*
		public static void ErrorCrd_ExportToExcel(System.Data.DataTable dt, string pathName)
		{
			if (dt == null)
			{
				return;
			}
			Microsoft.Office.Interop.Excel.Application xlApp = (Microsoft.Office.Interop.Excel.Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("00024500-0000-0000-C000-000000000046")));
			if (xlApp == null)
			{
				return;
			}
			Workbooks workbooks = xlApp.Workbooks;
			Workbook workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);
			if (Form1.<ErrorCrd_ExportToExcel>o__SiteContainer0.<>p__Site1 == null)
			{
				Form1.<ErrorCrd_ExportToExcel>o__SiteContainer0.<>p__Site1 = CallSite<Func<CallSite, object, Worksheet>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(Worksheet), typeof(Form1)));
			}
			Worksheet worksheet = Form1.<ErrorCrd_ExportToExcel>o__SiteContainer0.<>p__Site1.Target(Form1.<ErrorCrd_ExportToExcel>o__SiteContainer0.<>p__Site1, workbook.Worksheets[1]);
			Range range = null;
			long totalCount = (long)dt.Rows.Count;
			long rowRead = 0L;
			for (int r = 0; r < dt.DefaultView.Count; r++)
			{
				for (int i = 0; i < dt.Columns.Count; i++)
				{
					worksheet.Cells[r + 1, i + 1] = dt.DefaultView[r][i];
					if (Form1.<ErrorCrd_ExportToExcel>o__SiteContainer0.<>p__Site2 == null)
					{
						Form1.<ErrorCrd_ExportToExcel>o__SiteContainer0.<>p__Site2 = CallSite<Func<CallSite, object, Range>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(Range), typeof(Form1)));
					}
					range = Form1.<ErrorCrd_ExportToExcel>o__SiteContainer0.<>p__Site2.Target(Form1.<ErrorCrd_ExportToExcel>o__SiteContainer0.<>p__Site2, worksheet.Cells[r + 1, i + 1]);
					range.Font.Size = 9;
					range.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, null);
					range.EntireColumn.AutoFit();
				}
				rowRead += 1L;
				float arg_1A3_0 = (float)(100L * rowRead) / (float)totalCount;
				System.Windows.Forms.Application.DoEvents();
			}
			range.Borders[XlBordersIndex.xlInsideHorizontal].Weight = XlBorderWeight.xlThin;
			if (dt.Columns.Count > 1)
			{
				range.Borders[XlBordersIndex.xlInsideVertical].Weight = XlBorderWeight.xlThin;
			}
			try
			{
				workbook.Saved = true;
				workbook.SaveCopyAs(pathName);
			}
			catch (Exception)
			{
				return;
			}
			workbooks.Close();
			if (xlApp != null)
			{
				xlApp.Workbooks.Close();
				xlApp.Quit();
				int generation = GC.GetGeneration(xlApp);
				Marshal.ReleaseComObject(xlApp);
				xlApp = null;
				GC.Collect(generation);
			}
			GC.Collect();
			Process[] excelProc = Process.GetProcessesByName("EXCEL");
			DateTime startTime = default(DateTime);
			int killId = 0;
			for (int j = 0; j < excelProc.Length; j++)
			{
				if (startTime < excelProc[j].StartTime)
				{
					startTime = excelProc[j].StartTime;
					killId = j;
				}
			}
			if (!excelProc[killId].HasExited)
			{
				excelProc[killId].Kill();
			}
		}
         * 
         */

        /*
		private void fileOutOleDb(System.Data.DataTable tempdt, string filePath)
		{
			int rowNum = tempdt.Rows.Count;
			int columnNum = tempdt.Columns.Count;
			int rowIndex = 1;
			int columnIndex = 0;
			Microsoft.Office.Interop.Excel.Application xlApp = (Microsoft.Office.Interop.Excel.Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("00024500-0000-0000-C000-000000000046")));
			xlApp.DefaultFilePath = "";
			xlApp.DisplayAlerts = true;
			xlApp.SheetsInNewWorkbook = 1;
			Workbook xlBook = xlApp.Workbooks.Add(true);
			foreach (DataColumn dc in tempdt.Columns)
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
					xlApp.Cells[rowIndex, columnIndex] = tempdt.Rows[i][j].ToString();
				}
			}
			xlBook.SaveCopyAs(filePath);
		}
		private System.Data.DataTable ReadCsv(string path)
		{
			return null;
		}
         */
		public DataSet ExcelToDS(string Path)
		{
			string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Path + ";Extended Properties='Excel 8.0;HDR=NO;IMEX = 1;'";
			OleDbConnection conn = new OleDbConnection(strConn);
			conn.Open();
			string strExcel = "select * from [sheet1$]";
			OleDbDataAdapter myCommand = new OleDbDataAdapter(strExcel, strConn);
			DataSet ds = new DataSet();
			myCommand.Fill(ds, "table1");
			return ds;
		}


 

	}
}
