using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
namespace weizhang
{
	public class Captcha
	{
		public static string Match(IEnumerable<MValidate> items, string picCode)
		{
			string c = string.Empty;
			MValidate item = items.FirstOrDefault((MValidate d) => d.Code == picCode);
			if (item != null)
			{
				c = item.C;
			}
			return c;
		}
		public static IEnumerable<MValidate> LoadSpecialData(string path)
		{
			IEnumerable<MValidate> items = null;
			if (File.Exists(path))
			{
				XDocument xdoc = XDocument.Load(path);
				items = 
					from c in xdoc.Descendants("item")
					select new MValidate
					{
						C = c.Attribute("char").Value,
						Code = c.Attribute("code").Value
					};
			}
			return items;
		}
		public static bool SaveSpecialData(string path, string c, string code)
		{
			if (string.IsNullOrEmpty(c))
			{
				return false;
			}
			if (File.Exists(path))
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(path);
				XmlNode root = xmlDoc.SelectSingleNode("items");
				XmlElement xe = xmlDoc.CreateElement("item");
				xe.SetAttribute("char", c);
				xe.SetAttribute("code", code);
				root.AppendChild(xe);
				xmlDoc.Save(path);
				return true;
			}
			return false;
		}
		public static IEnumerable<MSystemConfig> LoadSystemConfig(string path)
		{
			IEnumerable<MSystemConfig> items = null;
			if (File.Exists(path))
			{
				XDocument xdoc = XDocument.Load(path);
				items = 
					from c in xdoc.Descendants("system")
					select new MSystemConfig
					{
						Type = c.Attribute("type").Value,
						XinagShi = c.Attribute("xiangshi").Value,
						BackGroud = c.Attribute("background").Value,
						Hsb = c.Attribute("hsb").Value,
						RueiHua = c.Attribute("rueihua").Value,
						Za = c.Attribute("za").Value,
						XuanZhuan = c.Attribute("xuanzhuan").Value,
						Engine = c.Attribute("Engine").Value
					};
			}
			return items;
		}
		public static string DealCode(string picCode, int previeCount, int lastCount)
		{
			if (picCode.Length >= previeCount)
			{
				bool isC = false;
				for (int i = 0; i < previeCount; i++)
				{
					isC = (picCode[i] == '0');
				}
				if (isC)
				{
					picCode = picCode.Substring(previeCount);
				}
			}
			if (picCode.Length >= lastCount)
			{
				bool isB = false;
				for (int j = lastCount; j > 0; j--)
				{
					int b = j;
					int t = b + 1;
					isB = (picCode[t] == '0');
				}
				if (isB)
				{
					picCode = picCode.Remove(picCode.Length - lastCount);
				}
			}
			return picCode;
		}
		public static string ParseValidateCode(Bitmap validateImage, bool local)
		{
			Bitmap[] pics = null;
			List<string> picXml = null;
			string validateCode = string.Empty;
			if (validateImage != null)
			{
				CaptchaParser parser = new CaptchaParser(validateImage, local);
				validateCode = parser.DeOCR(out pics, local, out picXml);
				Bitmap arg_27_0 = parser.Img;
			}
			return validateCode;
		}
	}
}
