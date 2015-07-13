using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;


namespace weizhang
{
	public class ValidCodeBll
	{
        // 网址设置
        public string mainUrl = "http://219.153.5.18:2333/cxxt/jdccxjg.html";
        public string imageUrl = "http://219.153.5.18:2333/cxxt/captcha-image.html";
        
        /// <summary>
        /// 检查网络连通性选择这个函数
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="vinNo"></param>
        /// <returns></returns>
        public string GetStatus(string cardNo, string vinNo)
        {
            string result = this.GetResponse(cardNo, vinNo);
            if (result == "Error")
            {
                return "Retry";
            }
            else
            {
                return "OK";
            }
        }


        /// <summary>
        /// 正常请求，选择这个函数
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="vinNo"></param>
        /// <returns></returns>
		public string GetCheckResult(string cardNo, string vinNo)
		{
			string result = this.GetResponse(cardNo, vinNo);
            if(result == "Error")
            {
                DialogResult opt = MessageBox.Show("保存工作退出（取消），或者等待(重试)",
                    "网络错误",MessageBoxButtons.RetryCancel);
                if (opt == DialogResult.Cancel)
                {
                    return "SaveAndExit";
                }
                if (opt == DialogResult.Retry)
                {
                    return "Retry";
                }
            }
			while(result=="")
            {
                result = this.GetResponse(cardNo, vinNo);
            }
            string key = result;
			switch (key = result)
			{
			case "1":
			case "2":
			case "3":
			case "4":
			case "5":
			case "6":
			case "7":
				result = "Y";
				return result;
			case "8":
				result = "由于短时间访问过多，此IP已被禁止访问3小时！";
				return result;
			case "9":
				result = "N";
				return result;
			}
			result = "Y";
			return result;
		}

        /// <summary>
        /// 封装返回请求
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="vinNo"></param>
        /// <returns></returns>
		private string GetResponse(string cardNo, string vinNo)
		{
			string cookie = string.Empty;
			string result;
			do
			{
                Bitmap img = ValidCodeBll.GetImage(imageUrl, false, out cookie);
                if (img == null)
                {
                    return "Error";
                }
				string code = Captcha.ParseValidateCode(img, false);
				string postData = string.Concat(new string[]
				{
					"hpzl=",
					HttpUtility.UrlEncode("02"),
					"&hphm=",
					HttpUtility.UrlEncode("渝" + cardNo),
					"&vin=",
					HttpUtility.UrlEncode(vinNo),
					"&yzm=",
					HttpUtility.UrlEncode(code)
				});
				result = this.PostWebRequest(postData, Encoding.UTF8, cookie);
                
			}
			while (!(result != "1"));
			return result;
		}


        /// <summary>
        /// 主要的请求函数
        /// </summary>
        /// <param name="paramData"></param>
        /// <param name="dataEncode"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
		private string PostWebRequest(string paramData, Encoding dataEncode, string cookie)
		{
            string postUrl = mainUrl;
            string ret = string.Empty;
			try
			{

                byte[] byteArray = dataEncode.GetBytes(paramData);
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                CookieContainer co = new CookieContainer();
                co.SetCookies(new Uri(postUrl), cookie);
                webReq.CookieContainer = co;
                webReq.ContentLength = (long)byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
			}
			catch
			{
                return "Error";
			}
			return ret;
		}


        /// <summary>
        /// 获取验证码图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="local"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
		public static Bitmap GetImage(string url, bool local, out string cookie)
		{
            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                //获得cookie
                cookie = response.Headers.Get("Set-Cookie");
                Stream st = response.GetResponseStream();
                Bitmap bitmap = (Bitmap)Image.FromStream(st);
                st.Dispose();
                request.Abort();
                response.Close();
                return bitmap;
            }
            catch
            {
                cookie = null;
                return null;
            }
		}
	}
}
