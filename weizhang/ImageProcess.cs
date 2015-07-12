using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
namespace weizhang
{
	public class ImageProcess
	{
		public static byte[] ImageGdi(Bitmap bmp)
		{
			Bitmap xbmp = new Bitmap(bmp);
			MemoryStream ms = new MemoryStream();
			xbmp.Save(ms, ImageFormat.Jpeg);
			ms.Flush();
			double new_width = 0.0;
			double new_height = 0.0;
			Image m_src_image = Image.FromStream(ms);
			if (m_src_image.Width >= m_src_image.Height)
			{
				new_width = 40.0;
				new_height = new_width * (double)m_src_image.Height / (double)m_src_image.Width;
			}
			else
			{
				if (m_src_image.Height >= m_src_image.Width)
				{
					new_height = 27.0;
					new_width = new_height * (double)m_src_image.Width / (double)m_src_image.Height;
				}
			}
			Bitmap bbmp = new Bitmap((int)new_width, (int)new_height, m_src_image.PixelFormat);
			Graphics m_graphics = Graphics.FromImage(bbmp);
			m_graphics.SmoothingMode = SmoothingMode.HighQuality;
			m_graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			m_graphics.DrawImage(m_src_image, 0, 0, bbmp.Width, bbmp.Height);
			ms = new MemoryStream();
			bbmp.Save(ms, ImageFormat.Jpeg);
			byte[] buffer = ms.GetBuffer();
			ms.Close();
			return buffer;
		}
		public static Bitmap Bytetobmp(byte[] buffer)
		{
			MemoryStream ms = new MemoryStream();
			ms.Write(buffer, 0, buffer.Length);
			Bitmap bmp = new Bitmap(ms);
			ms.Close();
			return bmp;
		}
	}
}
