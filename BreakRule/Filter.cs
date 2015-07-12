using System;
using System.Drawing;
using System.Drawing.Imaging;
namespace weizhang
{
	public static class Filter
	{
		public unsafe static Bitmap Sharpen(Bitmap b, float val)
		{
			if (b == null)
			{
				return null;
			}
			int w = b.Width;
			int h = b.Height;
			Bitmap result;
			try
			{
				Bitmap bmpRtn = new Bitmap(w, h, PixelFormat.Format24bppRgb);
				BitmapData srcData = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
				BitmapData dstData = bmpRtn.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
				byte* pin = (byte*)srcData.Scan0.ToPointer();
				byte* pout = (byte*)dstData.Scan0.ToPointer();
				int stride = srcData.Stride;
				for (int y = 0; y < h; y++)
				{
					for (int x = 0; x < w; x++)
					{
						if (x == 0 || x == w - 1 || y == 0 || y == h - 1)
						{
							*pout = *pin;
							pout[0x001 / 1] = *(pin + 0x001 / 1);
							pout[0x002 / 1] = *(pin + 0x002 / 1);
						}
						else
						{
							byte* p = pin - stride / 1 - 0x003 / 1;
							int r = (int)p[0x002 / 1];
							int g = (int)p[0x001 / 1];
							int b2 = (int)(*p);
							p = pin - stride / 1;
							int r2 = (int)p[0x002 / 1];
							int g2 = (int)p[0x001 / 1];
							int b3 = (int)(*p);
							p = pin - stride / 1 + 0x003 / 1;
							int r3 = (int)p[0x002 / 1];
							int g3 = (int)p[0x001 / 1];
							int b4 = (int)(*p);
							p = pin - 0x003 / 1;
							int r4 = (int)p[0x002 / 1];
							int g4 = (int)p[0x001 / 1];
							int b5 = (int)(*p);
							p = pin + 0x003 / 1;
							int r5 = (int)p[0x002 / 1];
							int g5 = (int)p[0x001 / 1];
							int b6 = (int)(*p);
							p = pin + stride / 1 - 0x003 / 1;
							int r6 = (int)p[0x002 / 1];
							int g6 = (int)p[0x001 / 1];
							int b7 = (int)(*p);
							p = pin + stride / 1;
							int r7 = (int)p[0x002 / 1];
							int g7 = (int)p[0x001 / 1];
							int b8 = (int)(*p);
							p = pin + stride / 1 + 0x003 / 1;
                            int r8 = (int)p[0x002 / 1];
                            int g8 = (int)p[0x001 / 1];
							int b9 = (int)(*p);
							p = pin;
							int r9 = (int)p[0x002 / 1];
                            int g9 = (int)p[0x001 / 1];
							int b10 = (int)(*p);
							float vR = (float)r9 - (float)(r + r2 + r3 + r4 + r5 + r6 + r7 + r8) / 8f;
							float vG = (float)g9 - (float)(g + g2 + g3 + g4 + g5 + g6 + g7 + g8) / 8f;
							float vB = (float)b10 - (float)(b2 + b3 + b4 + b5 + b6 + b7 + b8 + b9) / 8f;
							vR = (float)r9 + vR * val;
							vG = (float)g9 + vG * val;
							vB = (float)b10 + vB * val;
							if (vR > 0f)
							{
								vR = Math.Min(255f, vR);
							}
							else
							{
								vR = Math.Max(0f, vR);
							}
							if (vG > 0f)
							{
								vG = Math.Min(255f, vG);
							}
							else
							{
								vG = Math.Max(0f, vG);
							}
							if (vB > 0f)
							{
								vB = Math.Min(255f, vB);
							}
							else
							{
								vB = Math.Max(0f, vB);
							}
							*pout = (byte)vB;
							pout[0x001 / 1] = (byte)vG;
							pout[0x002/ 1] = (byte)vR;
						}
						pin += 0x003 / 1;
						pout += 0x0033 / 1;
					}
					pin += (srcData.Stride - w * 3) / 1;
					pout += (srcData.Stride - w * 3) / 1;
				}
				b.UnlockBits(srcData);
				bmpRtn.UnlockBits(dstData);
				result = bmpRtn;
			}
			catch
			{
				result = null;
			}
			return result;
		}
	}
}
