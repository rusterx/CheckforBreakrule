using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;

namespace Excel
{
    /// <summary>
    /// WXM 2012-04-13创建
    /// </summary>
    public class TableToExcel
    {
        #region 变量
        Microsoft.Office.Interop.Excel.Application xlApp = null;
        Microsoft.Office.Interop.Excel.Workbooks wbs = null;
        Microsoft.Office.Interop.Excel.Workbook wb = null;

        private int _rowindex = 0;   //全局行索引(使用时加n行)
        private int _saveindex = 0;  //保存全局行索引(数据导出后还原全局行索引)
        //文本
        private string _title = String.Empty;   //标题
        private string _headerdtext = String.Empty;   //页眉,即标题的下一行
        private string _footertext = String.Empty;   //页脚,即最后一行

        //正文(列表)是否显示边框
        private bool _isalldisplayborder = true;
        //正文(列表)边框类型
        private BorderWeightType _borderweight = BorderWeightType.xlThin;

        //保存路径
        private string _savepath = String.Empty;
        //字体
        private System.Drawing.Font _titlefont = new System.Drawing.Font("宋体", 15);
        private System.Drawing.Font _headerfont = new System.Drawing.Font("宋体", 11);
        private System.Drawing.Font _footerfont = new System.Drawing.Font("宋体", 11);
        private System.Drawing.Font _bodyheaderfont = new System.Drawing.Font("宋体", 11);
        private System.Drawing.Font _bodyfont = new System.Drawing.Font("宋体", 11);

        //脚文本Align
        private TextAlign _drawfootertextalign = TextAlign.xlHAlignRight;

        //要导出的表集合
        private List<System.Data.DataTable> _tables = new List<System.Data.DataTable>();

        //设置列宽(_isbodydisplayborder为false)
        private Dictionary<string, float> _columnswidth = new Dictionary<string, float>();

        //设置列的边框,
        private Dictionary<string, BorderWeightType> _columnsborder = new Dictionary<string, BorderWeightType>();

        //保存Table导入到那个Sheet表(打印时可以判断sheet是否有数据,没有数据则不打印)
        private Dictionary<string, System.Data.DataTable> SheetTable = new Dictionary<string, System.Data.DataTable>();

        private bool _iswraptext = true;  //单元格是否自动换行
        private bool _isbodylistheader = true;  //是否显示正文列表标题
        private bool _isautoconverttext = true;  //是否自动转换成文本格式
        private bool _isTitleAppendSheetName = false;  //标题后面是否追加SheetName

        //条码文本
        private string _BarCodeText = String.Empty;

        //设置打印时页面边距(程序中和Excel中的边距单元不一样(Excel设置0.5大概有5-10px,所以默认5px))
        private PaddingF _pageMargin = new PaddingF(5);
        private bool _isPrintFooter = true;   //是否打印页脚(只跟打印有关)
        private string _expandColumnName = String.Empty;
        #endregion

        #region 构造方法
        public TableToExcel(System.Data.DataTable table)
        {
            _tables = new List<System.Data.DataTable>() { table };
        }

        public TableToExcel(List<System.Data.DataTable> tables)
        {
            _tables = tables;
        }

        public TableToExcel(string savepath, System.Data.DataTable table)
        {
            _savepath = savepath;
            _tables = new List<System.Data.DataTable>() { table };
        }

        public TableToExcel(string savepath, List<System.Data.DataTable> tables)
        {
            _savepath = savepath;
            _tables = tables;
        }

        public TableToExcel(string title, string savepath, System.Data.DataTable table)
        {
            _savepath = savepath;
            Title = title;
            _tables = new List<System.Data.DataTable>() { table };
        }

        public TableToExcel(string title, string savepath, List<System.Data.DataTable> tables)
        {
            _savepath = savepath;
            Title = title;
            _tables = tables;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 行索引(表示从某行开始打印,如0表示从第一行开始)
        /// </summary>
        public virtual int RowIndex
        {
            get { return _rowindex; }
            set
            {
                _rowindex = _saveindex = value;
            }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                IsDrawTitle = !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// 头文本
        /// </summary>
        public virtual string HeaderText
        {
            get { return _headerdtext; }
            set
            {
                _headerdtext = value;
                IsDrawHeader = !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// 脚文本
        /// </summary>
        public virtual string FooterText
        {
            get { return _footertext; }
            set
            {
                _footertext = value;
                IsDrawFooter = !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// 保存地址
        /// </summary>
        public virtual string SavePath
        {
            get { return _savepath; }
            set
            {
                _savepath = value;
            }
        }

        /// <summary>
        /// 标题字体
        /// </summary>
        public virtual System.Drawing.Font TitleFont
        {
            get { return _titlefont; }
            set { _titlefont = value; }
        }

        /// <summary>
        /// Header字体
        /// </summary>
        public virtual System.Drawing.Font HeaderFont
        {
            get { return _headerfont; }
            set { _headerfont = value; }
        }

        /// <summary>
        /// 页脚字体
        /// </summary>
        public virtual System.Drawing.Font FooterFont
        {
            get { return _footerfont; }
            set { _footerfont = value; }
        }

        /// <summary>
        /// 正文标题字体
        /// </summary>
        public virtual System.Drawing.Font BodyHeaderFont
        {
            get { return _bodyheaderfont; }
            set { _bodyheaderfont = value; }
        }

        /// <summary>
        /// 正文字体
        /// </summary>
        public virtual System.Drawing.Font BodyFont
        {
            get { return _bodyfont; }
            set { _bodyfont = value; }
        }

        /// <summary>
        /// 导出表集合
        /// </summary>
        public virtual List<System.Data.DataTable> Tables
        {
            get { return _tables; }
            set { _tables = value; }
        }

        /// <summary>
        /// 列宽集合(A:A 表示第一列,A:B表示第一二列)
        /// </summary>
        public virtual Dictionary<string, float> ColumnsWidth
        {
            get { return _columnswidth; }
            set { _columnswidth = value; }
        }

        /// <summary>
        /// 那些列显示边框
        /// </summary>
        public virtual Dictionary<string, BorderWeightType> ColumnsBorder
        {
            get { return _columnsborder; }
            set { _columnsborder = value; }
        }

        public virtual bool IsDrawTitle { get; set; }

        public virtual bool IsDrawHeader { get; set; }

        public virtual bool IsDrawFooter { get; set; }

        public virtual TextAlign DrawFooterTextAlign
        {
            get { return _drawfootertextalign; }
            set { _drawfootertextalign = value; }
        }

        /// <summary>
        /// 是否显示数据列表标题
        /// </summary>
        public virtual bool IsBodyListHeader
        {
            get { return _isbodylistheader; }
            set { _isbodylistheader = value; }
        }

        /// <summary>
        /// 正文(列表)是否显示边框
        /// </summary>
        public virtual bool IsDispalyBorderAll
        {
            get { return _isalldisplayborder; }
            set { _isalldisplayborder = value; }
        }

        /// <summary>
        /// 是否自动转换成文本格式
        /// </summary>
        public virtual bool IsAutoConvertText
        {
            get { return _isautoconverttext; }
            set
            {
                _isautoconverttext = value;
            }
        }

        /// <summary>
        /// 标题后面是否追加SheetName
        /// </summary>
        public virtual bool IsTitleAppendSheetName
        {
            get { return _isTitleAppendSheetName; }
            set
            {
                _isTitleAppendSheetName = value;
            }
        }

        /// <summary>
        /// 正文(列表)边框类型
        /// </summary>
        public virtual BorderWeightType BorderWeight
        {
            get { return _borderweight; }
            set { _borderweight = value; }
        }

        /// <summary>
        /// 单元格是否自动适应宽度
        /// </summary>
        public virtual bool IsAutoFit { get; set; }

        /// <summary>
        /// 单元格是否自动换行
        /// </summary>
        public virtual bool IsWrapText
        {
            get { return _iswraptext; }
            set { _iswraptext = value; }
        }

        /// <summary>
        /// 条码文本(长度不能超过8位,超过8位请更新图片模版,否则打印出界)
        /// </summary>
        public string BarCodeText
        {
            get { return _BarCodeText; }
            set
            {
                _BarCodeText = value;
                if (!String.IsNullOrEmpty(value))
                {
                    IsDrawTitle = true;
                }
                else
                {
                    IsDrawTitle = !string.IsNullOrEmpty(_title);
                }
            }
        }

        /// <summary>
        /// 设置打印时页面边距(程序中和Excel中的边距单元不一样(Excel设置0.5大概有10px,所以默认10px))
        /// </summary>
        public PaddingF PageMargin
        {
            get { return _pageMargin; }
            set
            {
                _pageMargin = value;
            }
        }

        /// <summary>
        /// 是否打印页脚(只跟打印有关)
        /// </summary>
        public bool IsPrintFooter
        {
            get { return _isPrintFooter; }
            set
            {
                _isPrintFooter = value;
            }
        }

        /// <summary>
        /// 要扩大的列(打印时,如果设置了列宽自动适应,有可能总列宽缩小或放大,所以缩小或放大的宽度要算在ExpandColumnName列上)
        /// 格式 A:A  B:B  C:C
        /// </summary>
        public string ExpandColumnName
        {
            get { return _expandColumnName; }
            set { _expandColumnName = value; }
        }
        #endregion

        public void ExportExcel()
        {
            if (string.IsNullOrEmpty(_savepath))
            {
                throw new Exception("保存路径不能为空!");
            }

            try
            {
                Microsoft.Office.Interop.Excel.Workbook wb = GetExcelWorkbook();
                //保存工作表
                wb.SaveCopyAs(_savepath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// 打印Excel (打印设置列宽不能出现E:F)
        /// </summary>
        /// <param name="isLandscape">是否所有Sheet横向打印(true为横向打印)</param>
        public void PrintExcel(bool isLandscape)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Workbook wb = GetExcelWorkbook();
                if (wb == null)
                {
                    return;
                }

                foreach (var v in wb.Worksheets)
                {
                    try
                    {
                        Microsoft.Office.Interop.Excel.Worksheet wsheet = ((Microsoft.Office.Interop.Excel.Worksheet)v);
                        //无数据表无需打印
                        if (SheetTable[wsheet.Name].Rows.Count > 0)
                        {
                            if (isLandscape)
                            {
                                //设置横向打印,默认纵向
                                wsheet.PageSetup.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape;   //横向打印
                            }
                            //设置纸张类型
                            wsheet.PageSetup.PaperSize = Microsoft.Office.Interop.Excel.XlPaperSize.xlPaperA4;
                            //设置打印边距
                            wsheet.PageSetup.TopMargin = _pageMargin.Top;
                            wsheet.PageSetup.BottomMargin = _isPrintFooter ? 12.00 : _pageMargin.Bottom;
                            wsheet.PageSetup.LeftMargin = _pageMargin.Left;
                            wsheet.PageSetup.RightMargin = _pageMargin.Right;
                            //wsheet.PageSetup.CenterHorizontally = true;
                            //wsheet.PageSetup.CenterVertically = true;   //这2句是居中打印
                            //wsheet.PageSetup.CenterFooter = "&\"Arial\"&12 " + obj.pageid.ToString();//页脚字体和大小
                            wsheet.PageSetup.FooterMargin = 1;
                            if (_isPrintFooter)
                            {
                                //打印页脚
                                wsheet.PageSetup.LeftFooter = wsheet.Name;
                                wsheet.PageSetup.CenterFooter = @"第 &P 页/共 &N 页";
                                wsheet.PageSetup.RightFooter = "打印时间:" + DateTime.Now.ToString();
                            }
                            //列宽自适应 (中英文数字混排的时候,如果不设置自动适应列宽的话,有的时候打印会缺字)
                            wsheet.Columns.AutoFit();
                            if (!String.IsNullOrEmpty(_expandColumnName))  //如果有扩大的列
                            {
                                float swidth = 0;   //目前实际宽度
                                float ywidth = 0;   //应该显示的宽度
                                foreach (var vk in _columnswidth)
                                {
                                    object obj = ((Microsoft.Office.Interop.Excel.Range)wsheet.Columns[vk.Key, Type.Missing]).ColumnWidth;
                                    swidth += float.Parse(obj.ToString());
                                    ywidth += vk.Value;  //设置的列宽总和
                                }
                                //自适应宽度后,总宽度小于我打印时设置的总宽度,则把缩小的宽度添加到可以扩大的列上
                                //扩大某列的宽度
                                Microsoft.Office.Interop.Excel.Range colrange = (Microsoft.Office.Interop.Excel.Range)wsheet.Columns[_expandColumnName, Type.Missing];
                                float cwidth = float.Parse(colrange.ColumnWidth.ToString());  //列当前宽度
                                if (swidth < ywidth)
                                {
                                    colrange.ColumnWidth = (cwidth + (ywidth - swidth));
                                }
                                else
                                {
                                    colrange.ColumnWidth = (cwidth - (swidth - ywidth));
                                }
                            }
                            //自适应行
                            wsheet.Rows.AutoFit();
                            //打印
                            wsheet.PrintOutEx();
                        }
                    }
                    catch
                    { }
                }
                //打印整个Workbook(所有Sheet表都打印出来)
                //wb.PrintOutEx();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// 打印Excel 默认纵向打印
        /// </summary>
        public void PrintExcel()
        {
            PrintExcel(false);
        }

        /// <summary>
        /// 获取Workbook
        /// </summary>
        /// <param name="table"></param>
        /// <param name="filename"></param>
        public Microsoft.Office.Interop.Excel.Workbook GetExcelWorkbook()
        {
            if (_tables.Count == 0)
            {
                throw new Exception("Tables集合必须大于零!");
            }

            if (_rowindex < 0)
            {
                _rowindex = 0;
            }
            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlApp.Visible = false;
            xlApp.DisplayAlerts = false;
            wbs = xlApp.Workbooks;
            wb = wbs.Add(Missing.Value);   //添加一个工作簿
            //添加Sheet表,新建一个Excel文件时候,一般会默认有3个Sheet表,所以用[table.Count - wb.Sheets.Count]
            int tabcount = _tables.Count;
            int sheets = wb.Worksheets.Count;  //获取默认Sheet表个数,一般默认3个
            if (tabcount > sheets)
            {
                wb.Worksheets.Add(Missing.Value, Missing.Value, tabcount - sheets, Missing.Value);
            }
            //删除多余Sheet表
            //((Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[index]).Delete();
            //写入Excel
            WriteExcelSheet(wb);
            //保存工作表
            //xlApp.ActiveWorkbook.SaveCopyAs(filename);
            //wb.SaveCopyAs(_savepath);
            return wb;
        }

        private void Close()
        {
            #region 关闭Excel进程
            if (wb != null)
            {
                wb.Close(false, _savepath, Missing.Value);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
                wb = null;
            }
            if (wbs != null)
            {
                wbs.Close();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wbs);
                wbs = null;
            }
            if (xlApp != null)
            {
                xlApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                xlApp = null;
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            #endregion
        }

        /// <summary>
        /// 写入Excel的Sheet表
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="tables"></param>
        private void WriteExcelSheet(Microsoft.Office.Interop.Excel.Workbook wb)
        {
            List<string> SheetNames = GetSheetsName(wb);  //Sheet表名字集合,防止相同名称,出现错误!
            for (int i = 0; i < _tables.Count; i++)
            {
                System.Data.DataTable table = _tables[i];
                int rows = table.Rows.Count;
                int cols = table.Columns.Count;
                //获取Sheet表
                Microsoft.Office.Interop.Excel.Worksheet wsheet = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[i + 1];   //wb.Worksheets 所以从1开始
                //选中表
                wsheet.Select();
                //设置表名字
                wsheet.Name = GetCorrectSheetName(SheetNames, table.TableName, (i + 1)); //Name要特别注意

                //保存DataTable和Sheet表的绑定
                SheetTable.Add(wsheet.Name, table);

                //设置总标题
                DrawTitle(wsheet, cols);
                //设置HeaderText
                if (IsDrawHeader)
                {
                    DrawText(wsheet, _headerdtext, _headerfont, TextAlign.xlHAlignLeft, cols);// Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft, cols);
                }
                //数据列表标题
                //Microsoft.Office.Interop.Excel.Range r = ret.get_Range(ret.Cells[1, 1], ret.Cells[1, table.Columns.Count]);
                if (_isbodylistheader)
                {
                    _rowindex++;
                    Microsoft.Office.Interop.Excel.Range range = wsheet.Range[wsheet.Cells[_rowindex, 1], wsheet.Cells[_rowindex, cols]];
                    object[] header = new object[cols];
                    for (int j = 0; j < cols; j++)
                    {
                        header[j] = table.Columns[j].ToString();
                    }
                    range.Value2 = header;
                    SetFont(range, _bodyheaderfont);
                    if (_isalldisplayborder)
                    {
                        range.Borders.LineStyle = 1;  //边框线
                        range.Borders.Weight = (int)_borderweight;
                    }
                }

                if (rows > 0)
                {
                    _rowindex++;
                    Microsoft.Office.Interop.Excel.Range range = wsheet.get_Range("A" + _rowindex, Missing.Value);
                    object[,] objData = new Object[rows, cols];
                    for (int row = 0; row < rows; row++)
                    {
                        for (int col = 0; col < cols; col++)
                        {
                            string converttxt = String.Empty;
                            if (_isautoconverttext)
                            {
                                converttxt = "'";
                            }
                            objData[row, col] = converttxt + table.Rows[row][col].ToString();  //随后数据后面加单引号
                        }
                    }
                    range = range.get_Resize(rows, cols);
                    range.Value2 = objData;
                    SetFont(range, _bodyfont);
                    if (_isalldisplayborder)
                    {
                        range.Borders.LineStyle = 1;  //加线框
                        range.Borders.Weight = (int)_borderweight;
                    }
                    else
                    {
                        //((Microsoft.Office.Interop.Excel.Range)range.Columns["A:A", Type.Missing]).Borders.LineStyle = 1;
                        //((Microsoft.Office.Interop.Excel.Range)range.Columns["A:A", Type.Missing]).Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium;
                        //指定的列显示边框
                        foreach (var vk in _columnsborder)
                        {
                            Microsoft.Office.Interop.Excel.Borders borders = ((Microsoft.Office.Interop.Excel.Range)range.Columns[vk.Key, Type.Missing]).Borders;
                            borders.LineStyle = 1;
                            borders.Weight = (int)vk.Value;
                        }
                    }
                    if (IsAutoFit)
                    {
                        range.EntireColumn.AutoFit();  //自动适应列
                    }
                    //更新行索引
                    _rowindex += (rows - 1);
                }

                //设置FooterText
                if (IsDrawFooter)
                {
                    if (rows == 0)
                    {
                        _rowindex += 2; //与Body隔一行
                    }
                    DrawText(wsheet, _footertext, _footerfont, DrawFooterTextAlign, cols);
                }
                //所有列自动换行
                wsheet.Columns.WrapText = _iswraptext;
                //设置列宽
                if (!IsAutoFit)
                {
                    foreach (var vk in _columnswidth)
                    {
                        ((Microsoft.Office.Interop.Excel.Range)wsheet.Columns[vk.Key, System.Type.Missing]).ColumnWidth = vk.Value;
                    }
                }
                //保存名字
                SheetNames.Add(wsheet.Name);
                //还原
                _rowindex = _saveindex;
            }
            //选中第一个表
            ((Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1]).Select();
        }

        /// <summary>
        /// 写标题
        /// </summary>
        /// <param name="wsheet"></param>
        /// <param name="title"></param>
        /// <param name="colcount"></param>
        private void DrawTitle(Microsoft.Office.Interop.Excel.Worksheet wsheet, int colcount)
        {
            if (!IsDrawTitle)
            {
                return;
            }
            _rowindex++;  //加行数

            Microsoft.Office.Interop.Excel.Range cellrange = (Microsoft.Office.Interop.Excel.Range)wsheet.Cells[_rowindex, 1];
            //cellrange.RowHeight = 31;  //行高31
            //插入图片
            InsertImage(GetImagePath(), cellrange, wsheet);

            //取得整个报表的标题
            string titletxt = _title;
            if (_isTitleAppendSheetName)
            {
                titletxt += "(" + wsheet.Name + ")";
            }
            wsheet.Cells[_rowindex, 1] = titletxt;
            //设置整个报表的标题格式
            Microsoft.Office.Interop.Excel.Range range = wsheet.Range[wsheet.Cells[_rowindex, 1], wsheet.Cells[_rowindex, colcount]];
            //设置字体
            SetFont(range, _titlefont);
            //range.Borders.LineStyle = 1;  //边线框
            //下边框线
            Microsoft.Office.Interop.Excel.Border border = range.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom);
            //border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlSlantDashDot;// 选择先的类型
            border.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium;
            //字体剧中
            range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenterAcrossSelection;
            //合并单元格
            range.Merge(range.MergeCells);
            range.WrapText = true;
        }

        /// <summary>
        /// 写文本
        /// </summary>
        /// <param name="wsheet"></param>
        /// <param name="title"></param>
        /// <param name="colcount"></param>
        private void DrawText(Microsoft.Office.Interop.Excel.Worksheet wsheet, string txt, System.Drawing.Font font, TextAlign align, int colcount)
        {
            _rowindex++;
            //取得整个报表的标题
            wsheet.Cells[_rowindex, 1] = txt;
            //设置整个报表的标题格式
            Microsoft.Office.Interop.Excel.Range range = wsheet.Range[wsheet.Cells[_rowindex, 1], wsheet.Cells[_rowindex, colcount]];
            SetFont(range, font);
            //range.Borders.LineStyle = 1;  //边线
            range.HorizontalAlignment = (int)align; // Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
            //合并单元格
            range.Merge(range.MergeCells);
        }

        /// <summary>
        /// 插入图片方法
        /// </summary>
        /// <param name="imgpath"></param>
        /// <param name="range"></param>
        /// <param name="wsheet"></param>
        private void InsertImage(string imgpath, Microsoft.Office.Interop.Excel.Range range, Microsoft.Office.Interop.Excel.Worksheet wsheet)
        {
            if (System.String.IsNullOrEmpty(imgpath))
            {
                return;
            }
            try
            {
                //126
                range.Select();
                float PicLeft, PicTop;
                PicLeft = Convert.ToSingle(range.Left);
                PicTop = Convert.ToSingle(range.Top) + 1.5F;
                wsheet.Shapes.AddPicture(imgpath, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, PicLeft, PicTop, -1, -1);  //-1表示用源图尺寸
            }
            catch { }
        }

        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="range"></param>
        /// <param name="font"></param>
        private void SetFont(Microsoft.Office.Interop.Excel.Range range, System.Drawing.Font font)
        {
            range.Font.Bold = font.Bold;
            range.Font.Italic = font.Italic;
            range.Font.Name = font.Name;
            range.Font.Size = font.Size;
            range.Font.Underline = font.Underline;
        }

        /// <summary>
        /// 获取Workbook中所有Sheet表的名称
        /// </summary>
        /// <param name="wb"></param>
        /// <returns></returns>
        private List<string> GetSheetsName(Microsoft.Office.Interop.Excel.Workbook wb)
        {
            List<string> names = new List<string>();
            foreach (var sheet in wb.Worksheets)
            {
                string name = ((Microsoft.Office.Interop.Excel.Worksheet)sheet).Name;
                if (!String.IsNullOrEmpty(name))
                {
                    names.Add(name);
                }
            }
            return names;
        }

        /// <summary>
        /// 获取一个合法Sheet名称
        /// </summary>
        /// <returns></returns>
        private string GetCorrectSheetName(List<string> existnames, string tabname, int tabindex)
        {
            string name = String.Empty;
            //是否为空,是否大于31,是否有特殊字符
            //验证名字是否合法
            if (!VerifySheetName(tabname))
            {
                tabname = "Table" + tabindex;
            }

            bool isexist = existnames.Exists(n => n.Equals(tabname));  //是否存在
            if (isexist)
            {
                tabindex++;
                name = GetCorrectSheetName(existnames, "Table" + tabindex, tabindex);
            }
            else
            {
                name = tabname;
            }
            return name;
        }

        /// <summary>
        /// 判断是否合法
        /// </summary>
        private bool VerifySheetName(string sheetname)
        {
            if (string.IsNullOrEmpty(sheetname) || sheetname.Length > 31)
            {
                return false;
            }

            if (sheetname.Contains("\\") || sheetname.Contains("/"))
            {
                return false;
            }

            if (sheetname.Contains(":") || sheetname.Contains("："))
            {
                return false;
            }

            if (sheetname.Contains("?") || sheetname.Contains("？"))
            {
                return false;
            }

            if (sheetname.Contains("[") || sheetname.Contains("]"))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 保存图片到本地,并返回图片地址
        /// </summary>
        /// <returns></returns>
        public string GetImagePath()
        {
            if (String.IsNullOrEmpty(BarCodeText))
            {
                return String.Empty;
            }
            string imgpath = String.Empty;
            Bitmap bitmap = null;
            Graphics gs = null;
            try
            {
                bitmap = WMS.UI.Properties.Resources.BarCode_Template;
                gs = Graphics.FromImage(bitmap);   //获取模版图片的Graphics对象
                BarCode39 barcode = new BarCode39();
                barcode.BarcodeText = BarCodeText; //分拣单编号
                barcode.DrawBarBit(gs, new Rectangle(0, 0, bitmap.Width, bitmap.Height));  //条码画到模版图片上
                imgpath = Application.StartupPath + "\\BarCode.jpg";
                bitmap.Save(imgpath);   //保存图片
            }
            catch
            {
                imgpath = "";
            }
            finally
            {
                if (gs != null)
                {
                    gs.Dispose();
                    gs = null;
                }
                if (bitmap != null)
                {
                    bitmap.Dispose();
                    bitmap = null;
                }
            }
            return imgpath;
        }
    }

    public enum BorderWeightType
    {
        /// <summary>
        /// 中
        /// </summary>
        xlMedium = -4138,
        /// <summary>
        /// 极细
        /// </summary>
        xlHairline = 1,
        /// <summary>
        /// 细
        /// </summary>
        xlThin = 2,
        /// <summary>
        /// 粗
        /// </summary>
        xlThick = 4,
    }

    public enum BorderLineStyle
    {
        /// <summary>
        /// 无
        /// </summary>
        xlLineStyleNone = -4142,
        /// <summary>
        /// 双划线
        /// </summary>
        xlDouble = -4119,
        /// <summary>
        /// 点线
        /// </summary>
        xlDot = -4118,
        /// <summary>
        /// 划线
        /// </summary>
        xlDash = -4115,
        /// <summary>
        /// 连续线
        /// </summary>
        xlContinuous = 1,
        /// <summary>
        /// 点划线
        /// </summary>
        xlDashDot = 4,
        /// <summary>
        /// 双点的划线
        /// </summary>
        xlDashDotDot = 5,
        /// <summary>
        /// 倾斜点划线
        /// </summary>
        xlSlantDashDot = 13,
    }

    public enum TextAlign
    {
        xlHAlignRight = -4152,
        xlHAlignLeft = -4131,
        xlHAlignJustify = -4130,
        xlHAlignDistributed = -4117,
        xlHAlignCenter = -4108,
        xlHAlignGeneral = 1,
        xlHAlignFill = 5,
        xlHAlignCenterAcrossSelection = 7,
    }

    public struct PaddingF
    {
        double _Left;
        double _Right;
        double _Top;
        double _Bottom;

        public PaddingF(double all)
        {
            this._Left = all;
            this._Right = all;
            this._Top = all;
            this._Bottom = all;
        }

        public PaddingF(double left, double top, double right, double bottom)
        {
            this._Left = left;
            this._Right = right;
            this._Top = top;
            this._Bottom = bottom;
        }

        public void SetValue(double all)
        {
            SetValue(all, all, all, all);
        }

        public void SetValue(double top, double left, double right, double bottom)
        {
            this._Left = left;
            this._Right = right;
            this._Top = top;
            this._Bottom = bottom;
        }

        public double Left
        {
            get { return _Left; }
            set
            {
                _Left = value;
            }
        }

        public double Right
        {
            get { return _Right; }
            set
            {
                _Right = value;
            }
        }

        public double Top
        {
            get { return _Top; }
            set
            {
                _Top = value;
            }
        }

        public double Bottom
        {
            get { return _Bottom; }
            set
            {
                _Bottom = value;
            }
        }
    }

    #region BarCode 条形码
    public class BarCode39
    {
        public string BarcodeText = string.Empty;

        public int BarcodeHeight = 0;
        public int BarcodeWidth = 0;

        public Font footerFont = new Font("微软雅黑", 13.0f);   /*Bob 修改了字体大小*/

        private double wideToNarrowRatio = 3.0;

        public double WideToNarrowRatio
        {
            get { return wideToNarrowRatio; }
            set { wideToNarrowRatio = value; }
        }
        private int weight = 1;

        public int Weight
        {
            get { return weight; }
            set { weight = value; }
        }
        /// <summary>
        /// 39条码中能使用的字符
        /// </summary>
        private String alphabet39 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%*";

        #region
        private String[] coded39Char = 
        {
            /* 0 */ "001100100", 
            /* 1 */ "100010100", 
            /* 2 */ "010010100", 
            /* 3 */ "110000100",
            /* 4 */ "001010100", 
            /* 5 */ "101000100", 
            /* 6 */ "011000100", 
            /* 7 */ "000110100",
            /* 8 */ "100100100", 
            /* 9 */ "010100100", 
            /* A */ "100010010", 
            /* B */ "010010010",
            /* C */ "110000010", 
            /* D */ "001010010", 
            /* E */ "101000010", 
            /* F */ "011000010",
            /* G */ "000110010", 
            /* H */ "100100010", 
            /* I */ "010100010", 
            /* J */ "001100010",
            /* K */ "100010001", 
            /* L */ "010010001", 
            /* M */ "110000001", 
            /* N */ "001010001",
            /* O */ "101000001", 
            /* P */ "011000001", 
            /* Q */ "000110001", 
            /* R */ "100100001",
            /* S */ "010100001", 
            /* T */ "001100001", 
            /* U */ "100011000", 
            /* V */ "010011000",
            /* W */ "110001000", 
            /* X */ "001011000", 
            /* Y */ "101001000", 
            /* Z */ "011001000",
            /* - */ "000111000", 
            /* . */ "110000100", 
            /*' '*/ "011000100",
            /* $ */ "010101000",
            /* / */ "010100010", 
            /* + */ "010001010", 
            /* % */ "100101000", 
            /* * */ "001101000" 
        };
        #endregion

        public BarCode39()
        {
            BarcodeText = "1234";
        }
        /// <summary>
        /// 为了使得条形码居中先要计算条形码的Left和Top坐标
        /// </summary>
        /// <returns></returns>
        private int getX()
        {
            int currentLocation = 0;
            string strBarcode = "*" + BarcodeText.ToUpper() + "*";
            for (int i = 0; i < strBarcode.Length; i++)
            {
                string encodedString = coded39Char[alphabet39.IndexOf(strBarcode[i])];

                for (int j = 0; j < 5; j++)
                {

                    if (encodedString[j] == '0')
                    {
                        currentLocation += weight;
                    }
                    else
                    {
                        currentLocation += 3 * weight;
                    }
                    //画第6个     5   白条 
                    if ((j + 5) < 9)
                    {
                        if (encodedString[j + 5] == '0')
                        {
                            currentLocation += weight;
                        }
                        else
                        {
                            currentLocation += 3 * weight;
                        }
                    }
                }
                currentLocation += weight;
            }
            return currentLocation;
        }
        /// <summary>
        /// 显示条形码
        /// </summary>
        /// <param name="g">GDI+</param>
        /// <param name="rects">画图区域</param>
        protected void DrawBitmap(Graphics g, Rectangle rects)
        {
            if (BarcodeText == "") return;
            string strBarcode = "*" + BarcodeText.ToUpper() + "*";
            //string strBarcode =  BarcodeText.ToUpper() ;
            String encodedString = "";
            int currentLocation = 5;//rects.X + (rects.Width - getX()) / 2;
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            SolidBrush witeBrush = new SolidBrush(Color.White);
            int yTop = rects.Y;
            for (int i = 0; i < strBarcode.Length; i++)
            {
                encodedString = coded39Char[alphabet39.IndexOf(strBarcode[i])];

                for (int j = 0; j < 5; j++)
                {

                    if (encodedString[j] == '0')
                    {
                        Rectangle re1 = new Rectangle(currentLocation, yTop, weight, BarcodeHeight);
                        g.FillRectangle(blackBrush, re1);
                        currentLocation += weight;
                    }
                    else
                    {
                        Rectangle re1 = new Rectangle(currentLocation, yTop, 3 * weight, BarcodeHeight);
                        g.FillRectangle(blackBrush, re1);
                        currentLocation += 3 * weight;
                    }
                    //画第6个     5   白条 
                    if ((j + 5) < 9)
                    {
                        if (encodedString[j + 5] == '0')
                        {
                            Rectangle re1 = new Rectangle(currentLocation, yTop, weight, BarcodeHeight);
                            g.FillRectangle(witeBrush, re1);
                            currentLocation += weight;
                        }
                        else
                        {
                            Rectangle re1 = new Rectangle(currentLocation, yTop, 3 * weight, BarcodeHeight);
                            g.FillRectangle(witeBrush, re1);
                            currentLocation += 3 * weight;
                        }
                    }
                }
                Rectangle re2 = new Rectangle(currentLocation, yTop, weight, BarcodeHeight);
                g.FillRectangle(witeBrush, re2);
                currentLocation += weight;
            }


        }

        /// <summary>
        /// 显示条形码和文字
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rects"></param>
        public void DrawBarcode(Graphics g, Rectangle rects)
        {
            SizeF fsize = g.MeasureString(BarcodeText, this.footerFont);

            Rectangle b = rects;
            b.Y = rects.Y + rects.Height - (int)fsize.Height;
            b.X = rects.X + (rects.Width - (int)fsize.Width) / 2;
            b.Height = (int)fsize.Height;
            DrawText(BarcodeText, g, b);
            //BarCode
            Rectangle m = new Rectangle();
            m = rects;
            m.Height = rects.Height - b.Height;
            this.BarcodeHeight = m.Height;
            DrawBitmap(g, m);
        }

        /// <summary>
        /// 绘制条形码 无文字
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rects"></param>
        public void DrawBarBit(Graphics g, Rectangle rects)
        {
            SizeF fsize = g.MeasureString(BarcodeText, this.footerFont);

            Rectangle b = rects;
            b.Y = rects.Y + rects.Height - (int)fsize.Height;
            b.X = rects.X + (rects.Width - (int)fsize.Width) / 2;
            b.Height = (int)fsize.Height;
            //DrawText(BarcodeText, g, b);
            //BarCode
            Rectangle m = new Rectangle();
            m = rects;
            //m.Height = rects.Height - b.Height;
            this.BarcodeHeight = m.Height;
            DrawBitmap(g, m);
        }
        /// <summary>
        /// 文本显示
        /// </summary>
        /// <param name="text"></param>
        /// <param name="g"></param>
        /// <param name="rects"></param>
        protected void DrawText(string text, Graphics g, Rectangle rects)
        {
            g.DrawString(text, this.footerFont, Brushes.Black, rects);
        }

    }
    #endregion
}