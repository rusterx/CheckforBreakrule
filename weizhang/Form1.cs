using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Excel;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace weizhang
{
    public partial class Form1 : Form
    {
        //查询所用的两个网址，必须从domain.txt中读取
        private string mainUrl = "";
        private string imageUrl = "";

        //运行状态
        private string status = "";

        //车辆信息存储列表
        private List<MVehicle> vehicleLs;
        private BackgroundWorker bwUI = new BackgroundWorker();
        private BackgroundWorker bwTime = new BackgroundWorker();

        //储存时间，更新程序用时
        private int milliTime = 0;
        private int cosumeTime;
        private int cosumeSecond;
        private int cosumeMinute;
        private int cosumeHour;

        //两个公有类
        private ExcelHelper eh;
        private ValidCodeBll codeBll;

        //是否停止更新UI
        private bool statusStop = true;
        private bool timeStop = true;
        private bool isError = true;

        //代理网络内容获取
        public delegate void CallBackDelegate();


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //BackgroundWorker只要没有使用RunAysac就不会运行，运行结束之后通过该方法
            //又可以重新唤醒
            bwUI.DoWork += new DoWorkEventHandler(UpdateStatus);
            bwTime.DoWork += new DoWorkEventHandler(UpdateTime);
        }



        /// <summary>
        /// 导入Excel提取信息，然后获取信息，并存在Excel中。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, EventArgs e)
        {
            
            if (vehicleLs!=null)
            {
                vehicleLs.Clear();
            }

            isError = false;

            //更新域名
            string domainPath = Path.Combine(Application.StartupPath, "domain.txt");
            FileStream fs = new FileStream(@domainPath, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            mainUrl = sr.ReadLine().TrimEnd('\n').TrimEnd('\r');
            imageUrl = sr.ReadLine().TrimEnd('\n').TrimEnd('\r');
            sr.Close();
            fs.Close();

            //初始化Mvehicle变量，然后进行元素添加
            vehicleLs = new List<MVehicle>();
            //初始化ExcelHelper类
            eh = new ExcelHelper();  
       	

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "";
            ofd.Filter = "Excel文档(*.xlsx,*.xls)|*.xlsx;*.xls";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                DataTable dtCar = eh.ExcelToDataTable(ofd.FileName, ExcelHelper.HDR.NO);
                for (int i = 0; i < dtCar.Rows.Count; i++)
                {
                    if (RemoveSpecialChars(dtCar.Rows[i][0].ToString()) == ""||
                        RemoveSpecialChars(dtCar.Rows[i][1].ToString()) == "")
                    {
                        continue;
                    }
                    vehicleLs.Add(new MVehicle
                    {
                        CardNo = RemoveSpecialChars(dtCar.Rows[i][0].ToString()),
                        VinNo = RemoveSpecialChars(dtCar.Rows[i][1].ToString())
                    });
                }

                //对于Thread，运行完成之后自动处于stopped状态，难以再次使用，那么
                //只有再次创建新的线程，而且很难安全的结束线程
                new Thread(new ThreadStart(FetchResult)).Start();
          
                //委托UpdateStatus去更新主界面         
                bwUI.RunWorkerAsync();
                statusStop = false;

                //更新时间            
                bwTime.RunWorkerAsync();
                timeStop = false;

                btnImport.Enabled = false;

            }
        }

        /// <summary>
        /// remove some special char in string
        /// </summary>
        /// <param name="rawString"></param>
        /// <returns></returns>
        public string RemoveSpecialChars(string rawString)
        {
            return rawString.Replace("\n","").Replace("\r","").Replace(" ","");
        }


        /// <summary>
        /// BackgroundWorker线程通过DoWorkEventHandler委托去委托UpdateStatus去更新UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateStatus(object sender, DoWorkEventArgs e)
        {
            while (!statusStop)
            {
                //必须用this.Invoke这个函数
                this.Invoke((MethodInvoker)delegate
                {
                    toolStripStatusLabel2.Text = status;
                });
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTime(object sender, DoWorkEventArgs e)
        {
            string formatString = "{0:00}时{1:00}分{2:00}秒";
            cosumeSecond = 0;
            cosumeMinute = 0;
            cosumeHour = 0;
            milliTime = System.Environment.TickCount;
             
            while (!timeStop)
            {
                cosumeTime = System.Environment.TickCount - milliTime;
                cosumeHour = cosumeTime / (1000 * 3600);
                cosumeMinute = cosumeTime / (1000 * 60) - cosumeHour * 60;
                cosumeSecond = cosumeTime / 1000 - cosumeMinute * 60 - cosumeHour * 3600;
                this.Invoke((MethodInvoker)delegate
                {
                    label1.Text = String.Format(formatString, cosumeHour, cosumeMinute, cosumeSecond);
                });
            }
        }


        /// <summary>
        /// 获取结果
        /// </summary>
        private void FetchResult()
        {
            codeBll = new ValidCodeBll();
            codeBll.mainUrl = mainUrl;
            codeBll.imageUrl = imageUrl;

            int i = 0;
            int inditator = vehicleLs.Count;
            string tempResult = "";
            
            foreach (MVehicle temp in vehicleLs)
            {
                i += 1;
                //访问网络是会阻塞UI线程的
                tempResult = codeBll.GetCheckResult(temp.CardNo, temp.VinNo);
                if (tempResult == "SaveAndExit")
                {
                    label1.Text = "正在保存结果，程序即将退出...";
                    isError = true;
                    CallBack();
                    Application.Exit();
                }
                if (tempResult == "Retry")
                {
                    label1.Text = "正在重试重新连接,已为您保存最后的工作...";
                    isError = true;
                    CallBack();
                }
                while (tempResult == "Retry")
                {
                    tempResult = codeBll.GetCheckResult(temp.CardNo, temp.VinNo);
                }
                temp.CheckResult = tempResult;
                string formatString = "正在获取车牌号为{0}的违章信息({1}/{2})...";
                status = String.Format(formatString, temp.CardNo,i,inditator);
                
            }

            //停止后台线程更新界面
            statusStop = true;
            timeStop = true;

            toolStripStatusLabel2.Text = "查询结束";

            CallBackDelegate cbd = new CallBackDelegate(CallBack);
            this.Invoke(cbd);
        }

        /// <summary>
        /// 由于子线程会阻塞SaveFileDialog，所以利用委托，当子线完成任务是，委托他去主线程把
        /// CallBack这个函数执行
        /// </summary>
        public void CallBack()
        {

            System.Data.DataTable dt = new System.Data.DataTable();
            //dt.Columns.Add("车牌号", Type.GetType("System.String"));
            //dt.Columns.Add("车架号", Type.GetType("System.String"));
            //DataColumn dc = new DataColumn("结果", Type.GetType("System.String"));
            //dt.Columns.Add(dc);

            string fileName = "";
            string currentTim = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

            int index = 0;
            foreach (MVehicle temp2 in vehicleLs)
            {
                if (index == 0)
                {
                    //添加表头
                    if (isError)
                    {
                        currentTim += "_异常退出";
                    }
                    fileName += temp2.CardNo + "查询结果" + currentTim + ".xlsx";
                    dt.Columns.Add(temp2.CardNo, Type.GetType("System.String"));
                    dt.Columns.Add(temp2.CheckResult, Type.GetType("System.String"));
                }
                else
                {                
                    DataRow dr = dt.NewRow();
                    dr[0] = temp2.CardNo;
                    dr[1] = temp2.CheckResult;
                    dt.Rows.Add(dr);
                }
                index++;

            }

            //是否存在“查询结果”文件夹，不存在则创建，否则不动
            string dir = Path.Combine(Application.StartupPath, "查询结果");
            string fullpath = Path.Combine(dir, fileName);
            if (!Directory.Exists(dir))
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                di.Create();
            }

            //存储结果，提示完成
            eh.DataTabletoExcel(dt, fullpath);

            if (!isError)
            {
                MessageBox.Show("查询结束!");
            }
            else
            {
                MessageBox.Show("程序异常退出，已经保存当前工作");
            }
            btnImport.Enabled = true;
        }
    }
}
