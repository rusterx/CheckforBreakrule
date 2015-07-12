using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Text;
namespace weizhang
{
	public static class Http
	{
		[DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);
		public static Stream GetStream(string target, string refere, ref string cookie, HttpRequestType method, Encoding encoding, int timeOut, bool keepAlive, params string[] parameters)
		{
			HttpWebResponse response = null;
			Stream responseStream = null;
			Stream returnStream = null;
			try
			{
				string ps = string.Empty;
				if (parameters != null && parameters.Length >= 1)
				{
					ps = string.Join("&", parameters);
				}
				byte[] bytes = encoding.GetBytes(ps);
				string urlPath = string.Empty;
				if (method == HttpRequestType.GET)
				{
					if (target.IndexOf("randCode.jsp?che=") < 0)
					{
						urlPath = string.Format("{0}?{1}", target, ps);
					}
					else
					{
						urlPath = target;
					}
				}
				else
				{
					urlPath = target;
				}
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPath);
				HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
				request.CachePolicy = policy;
				request.Timeout = timeOut * 1000;
				request.KeepAlive = keepAlive;
				request.Method = method.ToString().ToUpper();
				bool isPost = request.Method.ToUpper() == "POST";
				if (isPost)
				{
					request.ContentType = "application/x-www-form-urlencoded";
					request.ContentLength = (long)bytes.Length;
				}
				request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; SV1; .NET CLR 2.0.1124)";
				request.Referer = refere;
				request.Headers.Add("cookie", cookie);
				request.Headers.Add("Cache-Control", "no-cache");
				request.Accept = "*/*";
				request.Credentials = CredentialCache.DefaultCredentials;
				if (isPost)
				{
					Stream requestStream = request.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Close();
				}
				response = (HttpWebResponse)request.GetResponse();
				responseStream = response.GetResponseStream();
				byte[] buffer = Http.StreamToBytes(responseStream);
				returnStream = new MemoryStream(buffer);
				string outCookie = response.Headers.Get("Set-Cookie");
				if (outCookie != null)
				{
					if (outCookie.Contains("JSESSIONID_3UB2B"))
					{
						outCookie = outCookie.Replace(",JSESSIONID_3UB2B=", "; JSESSIONID_3UB2B=");
					}
					else
					{
						if (outCookie.Contains("JSESSIONID_HOB2B"))
						{
							outCookie = outCookie.Replace(",JSESSIONID_HOB2B=", "; JSESSIONID_HOB2B=");
						}
						else
						{
							if (outCookie.Contains("JSESSIONID_BKB2B"))
							{
								outCookie = outCookie.Replace(",JSESSIONID_BKB2B=", "; JSESSIONID_BKB2B=");
							}
							else
							{
								if (outCookie.Contains("JSESSIONID_MFB2B"))
								{
									outCookie = outCookie.Replace(",JSESSIONID_MFB2B=", "; JSESSIONID_MFB2B=");
								}
								else
								{
									outCookie = outCookie.Replace(",JSESSIONID=", "; JSESSIONID=");
								}
							}
						}
					}
					cookie = Http.SetCookies(cookie, outCookie);
				}
			}
			finally
			{
				if (response != null)
				{
					response.Close();
				}
				if (responseStream != null)
				{
					responseStream.Close();
				}
			}
			return returnStream;
		}
		public static Bitmap GetImage(string carrier, string target, string refere, ref string cookie, HttpRequestType method)
		{
			return Http.GetImage(carrier, target, refere, ref cookie, method, Encoding.GetEncoding("gb2312"), 60, true, new string[0]);
		}
		public static Bitmap GetImage(string carrier, string target, string refere, ref string cookie, HttpRequestType method, Encoding encoding, int timeOut, bool keepAlive, params string[] parameters)
		{
			return null;
		}
		public static string SetCookies(string cookies, string setCookies)
		{
			Dictionary<string, string> newCookies = new Dictionary<string, string>();
			if (cookies != null)
			{
				string[] tmpCookies = cookies.Split(";".ToCharArray());
				for (int i = 0; i < tmpCookies.Length; i++)
				{
					string[] cookie = tmpCookies[i].Split(new char[]
					{
						'='
					});
					if (cookie.Length == 2)
					{
						newCookies.Add(cookie[0].Trim(), cookie[1]);
					}
				}
			}
			if (setCookies != null)
			{
				string[] tmpCookies2 = setCookies.Split(";".ToCharArray());
				for (int j = 0; j < tmpCookies2.Length; j++)
				{
					string[] cookie2 = tmpCookies2[j].Split(new char[]
					{
						'='
					});
					if (cookie2.Length == 2)
					{
						if (!newCookies.ContainsKey(cookie2[0].Trim()))
						{
							newCookies.Add(cookie2[0].Trim(), cookie2[1]);
						}
						else
						{
							newCookies[cookie2[0].Trim()] = cookie2[1];
						}
					}
				}
			}
			string sznewCookies = string.Empty;
			Dictionary<string, string>.Enumerator it = newCookies.GetEnumerator();
			while (it.MoveNext())
			{
				string[] array = new string[6];
				array[0] = sznewCookies;
				array[1] = " ";
				string[] arg_11A_0 = array;
				int arg_11A_1 = 2;
				KeyValuePair<string, string> current = it.Current;
				arg_11A_0[arg_11A_1] = current.Key;
				array[3] = "=";
				string[] arg_137_0 = array;
				int arg_137_1 = 4;
				KeyValuePair<string, string> current2 = it.Current;
				arg_137_0[arg_137_1] = current2.Value;
				array[5] = ";";
				sznewCookies = string.Concat(array);
			}
			if (sznewCookies.Length != 0)
			{
				sznewCookies = sznewCookies.Substring(1, sznewCookies.Length - 1);
			}
			return sznewCookies;
		}
		private static byte[] StreamToBytes(Stream stream)
		{
			List<byte> bytes = new List<byte>();
			for (int temp = stream.ReadByte(); temp != -1; temp = stream.ReadByte())
			{
				bytes.Add((byte)temp);
			}
			return bytes.ToArray();
		}
	}
}
