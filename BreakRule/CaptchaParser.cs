using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
namespace weizhang
{
	public class CaptchaParser
	{
		private int grayVale = 200;
		private float score = 80f;
		private float rueihua = 0.2f;
		private int zacount = 2;
		private double black = 0.5;
		private int standWidth = 35;
		private int standHeight = 35;
		private int configType = 2;
		public Bitmap Img
		{
			get;
			private set;
		}
		public CaptchaParser(Bitmap img, bool local)
		{
			this.Img = img;
			this.PreDeal(local);
		}
		private CaptchaParser()
		{
		}
		public string DeOCR(out Bitmap[] pics, bool isLocal, out List<string> picXml)
		{
			pics = null;
			string validateCode = string.Empty;
			UnCodebase uncode = new UnCodebase(this.Img);
			uncode.GrayByPixels();
			pics = uncode.GetSplitPics(5, 1, this.standWidth, this.standHeight);
			picXml = new List<string>();
			if (pics != null && pics.Length >= 5)
			{
				string arg_4E_0 = string.Empty;
				string arg_54_0 = string.Empty;
				string arg_5A_0 = string.Empty;
				string arg_60_0 = string.Empty;
				string arg_66_0 = string.Empty;
				string picCode = string.Empty;
				string picCode2 = string.Empty;
				string picCode3 = string.Empty;
				string picCode4 = string.Empty;
				string picCode5 = string.Empty;
				for (int i = 0; i < 5; i++)
				{
					UnCodebase uncodeAgain = new UnCodebase(pics[i]);
					uncodeAgain.GetPicValidByValue(190, 1);
					pics[i] = uncodeAgain.bmpobj;
				}
				for (int j = 0; j < 5; j++)
				{
					pics[j] = new Bitmap(pics[j], this.standWidth, this.standHeight);
				}
				int black = 10;
				picCode = uncode.GetSingleBmpCode(pics[0], black);
				picCode2 = uncode.GetSingleBmpCode(pics[1], black);
				picCode3 = uncode.GetSingleBmpCode(pics[2], black);
				picCode4 = uncode.GetSingleBmpCode(pics[3], black);
				picCode5 = uncode.GetSingleBmpCode(pics[4], black);
				if (isLocal)
				{
					string picStr = "";
					string picStr2 = "";
					string picStr3 = "";
					string picStr4 = "";
					string picStr5 = "";
					Captcha.SaveSpecialData("ValidCode.xml", picStr, picCode);
					Captcha.SaveSpecialData("ValidCode.xml", picStr2, picCode2);
					Captcha.SaveSpecialData("ValidCode.xml", picStr3, picCode3);
					Captcha.SaveSpecialData("ValidCode.xml", picStr4, picCode4);
					Captcha.SaveSpecialData("ValidCode.xml", picStr5, picCode5);
				}
				string[] arr = new string[]
				{
					picCode,
					picCode2,
					picCode3,
					picCode4,
					picCode5
				};
				picXml.Add(picCode);
				picXml.Add(picCode2);
				picXml.Add(picCode3);
				picXml.Add(picCode4);
				picXml.Add(picCode5);
				validateCode = this.GetPicnums(arr);
			}
			return validateCode;
		}
		public string GetPicnums(string[] arr)
		{
			string strCode = string.Empty;
			for (int i = 0; i < 5; i++)
			{
				string code = arr[i];
				string specialDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ValidCode.xml");
				IEnumerable<MValidate> specialData = Captcha.LoadSpecialData(specialDataPath);
				MValidate[] arrayList = specialData.ToArray<MValidate>();
				int smallPoint = 0;
				int smallIndex = 0;
				for (int arrayIndex = 0; arrayIndex < arrayList.Length; arrayIndex++)
				{
					int point = 0;
					for (int comparison = 0; comparison < code.Length; comparison++)
					{
						char temp = arr[i][comparison];
						if (arrayList[arrayIndex].Code.Length <= comparison)
						{
							point += code.Length - arrayList[arrayIndex].Code.Length;
							break;
						}
						char temp2 = arrayList[arrayIndex].Code[comparison];
						if (temp != temp2)
						{
							point++;
						}
					}
					if (point < 6)
					{
						smallIndex = arrayIndex;
						break;
					}
					if (arrayIndex == 0)
					{
						smallPoint = point;
						smallIndex = 0;
					}
					else
					{
						if (point < smallPoint)
						{
							smallPoint = point;
							smallIndex = arrayIndex;
						}
					}
				}
				strCode += arrayList[smallIndex].C;
			}
			return strCode;
		}
		private void PreDeal(bool local)
		{
			UnCodebase uncode = new UnCodebase(this.Img);
			if (!local)
			{
				uncode.RemoveBlack();
				uncode.GrayByPixels();
				uncode.BitmapToAverage(100, 200);
				uncode.BitmapToBalackAndWrite(this.black);
				uncode.Bmpobj = Filter.Sharpen(uncode.Bmpobj, this.rueihua);
				uncode.GetPicValidByValueFirst(5);
			}
			this.Img = uncode.Bmpobj;
		}
		private Bitmap DealBeforSplit(Bitmap img, int grayValue)
		{
			UnCodebase uncode = new UnCodebase(img);
			uncode.ClearNoise(grayValue, this.zacount);
			uncode.ErosionPic(128, 0, null);
			Bitmap arg_27_0 = uncode.Bmpobj;
			uncode.ClearPicBorder(1);
			return uncode.Bmpobj;
		}
	}
}
