using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
namespace weizhang
{
	public class UnCodebase
	{
		public Bitmap bmpobj;
		public Bitmap Bmpobj
		{
			get
			{
				return this.bmpobj;
			}
			set
			{
				this.bmpobj = value;
			}
		}
		public UnCodebase(Bitmap pic)
		{
			if (pic != null)
			{
				this.bmpobj = new Bitmap(pic);
			}
		}
		public void ClearNoise(int grayValue, int maxNearPoints)
		{
			for (int i = 0; i < this.bmpobj.Width; i++)
			{
				for (int j = 0; j < this.bmpobj.Height; j++)
				{
					if ((int)this.bmpobj.GetPixel(i, j).R >= grayValue)
					{
						int nearDots = 0;
						if (i == 0 || i == this.bmpobj.Width - 1 || j == 0 || j == this.bmpobj.Height - 1)
						{
							this.bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));
						}
						else
						{
							if ((int)this.bmpobj.GetPixel(i - 1, j - 1).R < grayValue)
							{
								nearDots++;
							}
							if ((int)this.bmpobj.GetPixel(i, j - 1).R < grayValue)
							{
								nearDots++;
							}
							if ((int)this.bmpobj.GetPixel(i + 1, j - 1).R < grayValue)
							{
								nearDots++;
							}
							if ((int)this.bmpobj.GetPixel(i - 1, j).R < grayValue)
							{
								nearDots++;
							}
							if ((int)this.bmpobj.GetPixel(i + 1, j).R < grayValue)
							{
								nearDots++;
							}
							if ((int)this.bmpobj.GetPixel(i - 1, j + 1).R < grayValue)
							{
								nearDots++;
							}
							if ((int)this.bmpobj.GetPixel(i, j + 1).R < grayValue)
							{
								nearDots++;
							}
							if ((int)this.bmpobj.GetPixel(i + 1, j + 1).R < grayValue)
							{
								nearDots++;
							}
						}
						if (nearDots < maxNearPoints)
						{
							this.bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));
						}
					}
					else
					{
						this.bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));
					}
				}
			}
		}
		public void RemoveBlack()
		{
			List<MLOcation> list = new List<MLOcation>();
			for (int i = 0; i < this.bmpobj.Width; i++)
			{
				for (int j = 0; j < this.bmpobj.Height; j++)
				{
					Color piexl = this.bmpobj.GetPixel(i, j);
					if (i <= 5 || i >= this.bmpobj.Width - 5 || j <= 5 || j >= this.bmpobj.Height - 5)
					{
						this.bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));
					}
					else
					{
						if (!this.IsRed(piexl))
						{
							this.bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));
							int nearDots = 0;
							if (this.IsRed(this.bmpobj.GetPixel(i + 1, j)))
							{
								nearDots++;
							}
							if (this.IsRed(this.bmpobj.GetPixel(i + 2, j)))
							{
								nearDots++;
							}
							if (this.IsRed(this.bmpobj.GetPixel(i, j + 1)))
							{
								nearDots++;
							}
							if (this.IsRed(this.bmpobj.GetPixel(i, j + 2)))
							{
								nearDots++;
							}
							if (this.IsRed(this.bmpobj.GetPixel(i + 1, j + 1)))
							{
								nearDots++;
							}
							if (nearDots > 2)
							{
								list.Add(new MLOcation
								{
									X = i,
									Y = j
								});
							}
						}
					}
				}
			}
			foreach (MLOcation temp in list)
			{
				this.bmpobj.SetPixel(temp.X, temp.Y, Color.FromArgb(255, 0, 0));
			}
		}
		private bool IsRed(Color cur)
		{
			Color redPiex = Color.FromArgb(166, 161, 165);
			return cur.R >= redPiex.R && cur.G <= redPiex.G && cur.B <= redPiex.B;
		}
		public void ViolentSet()
		{
			for (int i = 0; i < this.bmpobj.Width; i++)
			{
				for (int j = 0; j < this.bmpobj.Height; j++)
				{
					Color piexl = this.bmpobj.GetPixel(i, j);
					if (i <= 10 || i >= this.bmpobj.Width - 10 || j <= 10 || j >= this.bmpobj.Height - 10)
					{
						this.bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));
					}
					else
					{
						if (!this.IsRed(piexl))
						{
							int nearDots = 0;
							Color neightbor = this.bmpobj.GetPixel(i - 1, j - 1);
							if (this.IsRed(neightbor))
							{
								nearDots++;
							}
							neightbor = this.bmpobj.GetPixel(i, j - 1);
							if (this.IsRed(neightbor))
							{
								nearDots++;
							}
							neightbor = this.bmpobj.GetPixel(i + 1, j - 1);
							if (this.IsRed(neightbor))
							{
								nearDots++;
							}
							neightbor = this.bmpobj.GetPixel(i - 1, j);
							if (this.IsRed(neightbor))
							{
								nearDots++;
							}
							neightbor = this.bmpobj.GetPixel(i + 1, j);
							if (this.IsRed(neightbor))
							{
								nearDots++;
							}
							neightbor = this.bmpobj.GetPixel(i - 1, j + 1);
							if (this.IsRed(neightbor))
							{
								nearDots++;
							}
							neightbor = this.bmpobj.GetPixel(i, j + 2);
							if (this.IsRed(neightbor))
							{
								nearDots++;
							}
							neightbor = this.bmpobj.GetPixel(i + 1, j + 1);
							if (this.IsRed(neightbor))
							{
								nearDots++;
							}
							if (nearDots > 4)
							{
								if (this.IsBlack(piexl))
								{
									this.bmpobj.SetPixel(i, j, Color.FromArgb(255, 0, 0));
								}
							}
							else
							{
								this.bmpobj.SetPixel(i, j, Color.FromArgb(255, 255, 255));
							}
						}
					}
				}
			}
		}
		public void GrayByPixels()
		{
			for (int i = 0; i < this.bmpobj.Height; i++)
			{
				for (int j = 0; j < this.bmpobj.Width; j++)
				{
					int tmpValue = this.GetGrayNumColor(this.bmpobj.GetPixel(j, i));
					this.bmpobj.SetPixel(j, i, Color.FromArgb(tmpValue, tmpValue, tmpValue));
				}
			}
		}
		public void ClearPicBorder(int borderWidth)
		{
			for (int i = 0; i < this.bmpobj.Height; i++)
			{
				for (int j = 0; j < this.bmpobj.Width; j++)
				{
					if (i < borderWidth || j < borderWidth || j > this.bmpobj.Width - 1 - borderWidth || i > this.bmpobj.Height - 1 - borderWidth)
					{
						this.bmpobj.SetPixel(j, i, Color.FromArgb(255, 255, 255));
					}
				}
			}
		}
		public void GrayByLine()
		{
			Rectangle rec = new Rectangle(0, 0, this.bmpobj.Width, this.bmpobj.Height);
			BitmapData bmpData = this.bmpobj.LockBits(rec, ImageLockMode.ReadWrite, this.bmpobj.PixelFormat);
			IntPtr scan0 = bmpData.Scan0;
			int len = this.bmpobj.Width * this.bmpobj.Height;
			int[] pixels = new int[len];
			Marshal.Copy(scan0, pixels, 0, len);
			for (int i = 0; i < len; i++)
			{
				int grayValue = this.GetGrayNumColor(Color.FromArgb(pixels[i]));
				pixels[i] = (int)((byte)Color.FromArgb(grayValue, grayValue, grayValue).ToArgb());
			}
			this.bmpobj.UnlockBits(bmpData);
		}
		public void GetPicValidByValue(int grayValue, int charsCount)
		{
			int arg_0B_0 = this.bmpobj.Width;
			int posy = this.bmpobj.Height;
			int posy2 = 0;
			for (int i = 0; i < this.bmpobj.Height; i++)
			{
				for (int j = 0; j < this.bmpobj.Width; j++)
				{
					Color cur = this.bmpobj.GetPixel(j, i);
					if (cur.R == 0 && cur.G == 0 && cur.B == 0)
					{
						if (posy > i)
						{
							posy = i;
						}
						if (posy2 < i)
						{
							posy2 = i;
						}
					}
				}
			}
			Rectangle cloneRect = new Rectangle(0, posy, this.bmpobj.Width, posy2 - posy + 1);
			this.bmpobj = this.bmpobj.Clone(cloneRect, this.bmpobj.PixelFormat);
		}
		public void GetPicValidByValueFirst(int charsCount)
		{
			int posx = this.bmpobj.Width;
			int posy = this.bmpobj.Height;
			int posx2 = 0;
			int posy2 = 0;
			for (int i = 20; i < this.bmpobj.Height - 20; i++)
			{
				for (int j = 5; j < this.bmpobj.Width - 5; j++)
				{
					Color cur = this.bmpobj.GetPixel(j, i);
					if (cur.R == 0 && cur.G == 0 && cur.B == 0)
					{
						int neighnorCount = 0;
						this.bmpobj.GetPixel(j, i + 1);
						if (cur.R == 0 && cur.G == 0 && cur.B == 0)
						{
							neighnorCount++;
						}
						this.bmpobj.GetPixel(j, i + 2);
						if (cur.R == 0 && cur.G == 0 && cur.B == 0)
						{
							neighnorCount++;
						}
						this.bmpobj.GetPixel(j + 1, i);
						if (cur.R == 0 && cur.G == 0 && cur.B == 0)
						{
							neighnorCount++;
						}
						this.bmpobj.GetPixel(j + 2, i);
						if (cur.R == 0 && cur.G == 0 && cur.B == 0)
						{
							neighnorCount++;
						}
						if (neighnorCount > 2)
						{
							if (posx > j)
							{
								posx = j;
							}
							if (posy > i)
							{
								posy = i;
							}
							if (posx2 < j)
							{
								posx2 = j;
							}
							if (posy2 < i)
							{
								posy2 = i;
							}
						}
					}
				}
			}
			int span = charsCount - (posx2 - posx + 1) % charsCount;
			if (span < charsCount)
			{
				int leftSpan = span / 2;
				if (posx > leftSpan)
				{
					posx -= leftSpan;
				}
				if (posx2 + span - leftSpan < this.bmpobj.Width)
				{
					posx2 = posx2 + span - leftSpan;
				}
			}
			Rectangle cloneRect = new Rectangle(posx, posy, posx2 - posx + 1, posy2 - posy + 1);
			this.bmpobj = this.bmpobj.Clone(cloneRect, this.bmpobj.PixelFormat);
		}
		private bool IsBlack(Color cur)
		{
			Color redPiex = Color.FromArgb(10, 10, 10);
			return cur.R <= redPiex.R && cur.G <= redPiex.G && cur.B <= redPiex.B;
		}
		public Bitmap[] GetSplitPics(int rowNum, int colNum, int standWidth, int standHeight)
		{
			int singW = this.bmpobj.Width / rowNum;
			int range = singW / 7 * 3;
			int singH = this.bmpobj.Height / colNum;
			int minCount = this.bmpobj.Height;
			int secondX = 0;
			int minHeight = this.bmpobj.Height / 10;
			int maxHeight = this.bmpobj.Height / 10 * 9;
			Bitmap[] picArray = new Bitmap[rowNum * colNum];
			for (int picCount = 0; picCount < rowNum; picCount++)
			{
				int firstX = secondX;
				secondX = secondX + singW - range;
				for (int i = secondX; i < singW * (picCount + 1) + range; i++)
				{
					if (picCount == rowNum - 1)
					{
						secondX = this.bmpobj.Width;
						break;
					}
					int curCount = 0;
					if (i < this.bmpobj.Width)
					{
						for (int j = minHeight; j < maxHeight; j++)
						{
							if (this.IsBlack(this.bmpobj.GetPixel(i, j)))
							{
								curCount++;
							}
						}
					}
					if (curCount < minCount)
					{
						minCount = curCount;
						secondX = i;
					}
				}
				minCount = this.bmpobj.Height;
				Rectangle cloneRect = new Rectangle(firstX, 0, secondX - firstX, singH);
				picArray[picCount] = this.bmpobj.Clone(cloneRect, this.bmpobj.PixelFormat);
			}
			return picArray;
		}
		public string GetSingleBmpCode(Bitmap singlepic, int grayValue)
		{
			if (singlepic == null)
			{
				return string.Empty;
			}
			StringBuilder code = new StringBuilder();
			for (int y = 0; y < singlepic.Height; y++)
			{
				for (int x = 0; x < singlepic.Width; x++)
				{
					if ((int)singlepic.GetPixel(x, y).R < grayValue)
					{
						code.Append("1");
					}
					else
					{
						code.Append("0");
					}
				}
			}
			return code.ToString();
		}
		public void BitmapToBalackAndWrite(double hsb)
		{
			int w = this.bmpobj.Width;
			int h = this.bmpobj.Height;
			Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
			BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
			for (int y = 0; y < h; y++)
			{
				byte[] scan = new byte[(w + 7) / 8];
				for (int x = 0; x < w; x++)
				{
					if ((double)this.bmpobj.GetPixel(x, y).GetBrightness() >= hsb)
					{
						byte[] expr_7B_cp_0 = scan;
						int expr_7B_cp_1 = x / 8;
						expr_7B_cp_0[expr_7B_cp_1] |= (byte)(128 >> x % 8);
					}
				}
				Marshal.Copy(scan, 0, (IntPtr)((int)data.Scan0 + data.Stride * y), scan.Length);
			}
			bmp.UnlockBits(data);
			this.bmpobj = bmp;
		}
		public void ErosionPic(int grayValue, int mode, bool[,] structure)
		{
			int lwidth = this.bmpobj.Width;
			int lheight = this.bmpobj.Height;
			Bitmap newBmp = new Bitmap(lwidth, lheight);
			if (mode == 0)
			{
				for (int i = 0; i < lheight; i++)
				{
					for (int j = 1; j < lwidth - 1; j++)
					{
						newBmp.SetPixel(j, i, Color.Black);
						if ((int)this.bmpobj.GetPixel(j - 1, i).R > grayValue || (int)this.bmpobj.GetPixel(j, i).R > grayValue || (int)this.bmpobj.GetPixel(j + 1, i).R > grayValue)
						{
							newBmp.SetPixel(j, i, Color.White);
						}
					}
				}
			}
			else
			{
				if (mode == 1)
				{
					for (int i = 1; i < lheight - 1; i++)
					{
						for (int j = 0; j < lwidth; j++)
						{
							newBmp.SetPixel(j, i, Color.Black);
							if ((int)this.bmpobj.GetPixel(j, i - 1).R > grayValue || (int)this.bmpobj.GetPixel(j, i).R > grayValue || (int)this.bmpobj.GetPixel(j, i + 1).R > grayValue)
							{
								newBmp.SetPixel(j, i, Color.White);
							}
						}
					}
				}
				else
				{
					if (structure.Length != 9)
					{
						return;
					}
					for (int i = 1; i < lheight - 1; i++)
					{
						for (int j = 1; j < lwidth - 1; j++)
						{
							newBmp.SetPixel(j, i, Color.Black);
							for (int k = 0; k < 3; k++)
							{
								for (int l = 0; l < 3; l++)
								{
									if (structure[k, l] && (int)this.bmpobj.GetPixel(j + k - 1, i + l - 1).R > grayValue)
									{
										newBmp.SetPixel(j, i, Color.White);
										break;
									}
								}
							}
						}
					}
				}
			}
			this.bmpobj = newBmp;
		}
		private int GetGrayNumColor(Color posClr)
		{
			return (int)posClr.R * 19595 + (int)posClr.G * 38469 + (int)posClr.B * 7472 >> 16;
		}
		public void BitmapToAverage(int grayMin, int grayMax)
		{
			int w = this.bmpobj.Width;
			int h = this.bmpobj.Height;
			for (int y = 1; y < h - 1; y++)
			{
				for (int x = 1; x < w - 1; x++)
				{
					Color c = this.bmpobj.GetPixel(x, y);
					if (this.GetGrayNumColor(c) < grayMax && this.GetGrayNumColor(c) > grayMin)
					{
						byte r = Convert.ToByte((int)((this.bmpobj.GetPixel(x - 1, y - 1).R + this.bmpobj.GetPixel(x - 1, y).R + this.bmpobj.GetPixel(x - 1, y + 1).R + this.bmpobj.GetPixel(x, y - 1).R + this.bmpobj.GetPixel(x, y + 1).R + this.bmpobj.GetPixel(x + 1, y - 1).R + this.bmpobj.GetPixel(x + 1, y).R + this.bmpobj.GetPixel(x + 1, y + 1).R) / 8));
						byte g = Convert.ToByte((int)((this.bmpobj.GetPixel(x - 1, y - 1).G + this.bmpobj.GetPixel(x - 1, y).G + this.bmpobj.GetPixel(x - 1, y + 1).G + this.bmpobj.GetPixel(x, y - 1).G + this.bmpobj.GetPixel(x, y + 1).G + this.bmpobj.GetPixel(x + 1, y - 1).G + this.bmpobj.GetPixel(x + 1, y).G + this.bmpobj.GetPixel(x + 1, y + 1).G) / 8));
						byte b = Convert.ToByte((int)((this.bmpobj.GetPixel(x - 1, y - 1).B + this.bmpobj.GetPixel(x - 1, y).B + this.bmpobj.GetPixel(x - 1, y + 1).B + this.bmpobj.GetPixel(x, y - 1).B + this.bmpobj.GetPixel(x, y + 1).B + this.bmpobj.GetPixel(x + 1, y - 1).B + this.bmpobj.GetPixel(x + 1, y).B + this.bmpobj.GetPixel(x + 1, y + 1).G) / 8));
						this.bmpobj.SetPixel(x, y, Color.FromArgb((int)r, (int)g, (int)b));
					}
				}
			}
		}
	}
}
