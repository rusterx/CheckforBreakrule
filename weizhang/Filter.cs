using System;
using System.Drawing;
using System.Drawing.Imaging;
namespace weizhang
{
	public static class Filter
	{
		public static Bitmap Sharpen( Bitmap b, float val )
        {
	         if ( b == null )
	         {
		         return(null);
	         }

	         int w	= b.Width;
	         int h	= b.Height;

	         try
	         {
		         Bitmap bmpRtn = new Bitmap( w, h, PixelFormat.Format24bppRgb );

		         BitmapData srcData	= b.LockBits( new Rectangle( 0, 0, w, h ), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );
		         BitmapData dstData	= bmpRtn.LockBits( new Rectangle( 0, 0, w, h ), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );

		        unsafe
		        {
			        byte * pIn	= (byte *) srcData.Scan0.ToPointer();
			        byte * pOut	= (byte *) dstData.Scan0.ToPointer();
			        int stride	= srcData.Stride;
			        byte * p;

			        for ( int y = 0; y < h; y++ )
			        {
				        for ( int x = 0; x < w; x++ )
				        {
					         // Check the value of around 9 o 'clock. The point is located on the edge of unchanged.
					        if (x ==   0   || x == w -  1   || y ==   0   || y == h -   1)
					        {
					        // Do not do
					        pOut [0] = pIn [0];
					        pOut [1] = pIn [1];
					        pOut [2] = pIn [2];
					        }
					        else
					        {
					        int r1, r2, r3, r4, r5, r6, r7, r8, r0;
					        int g1, g2, g3, g4, g5, g6, g7, g8, g0;
					        int b1, b2, b3, b4, b5, b6, b7, b8, b0;

					        float vR, vG, vB;

					        // Top Left
					        p = pIn - stride -   3;
					        r1 = p [2];
					        g1 = p [1];
					        b1 = p [0];

					        // On
					        p = pIn - stride;
					        r2 = p[2];
					        g2 = p[1];
					        b2 = p[0];

					        // Top Right
					        p = pIn - stride +   3;
					        r3 = p [2];
					        g3 = p [1];
					        b3 = p [0];

					        // Left
					        p = pIn -  3;
					        r4 = p [2];
					        g4 = p [1];
					        b4 = p [0];

					        // Right
					        p = pIn +   3;
					        r5 = p [2];
					        g5 = p [1];
					        b5 = p [0];

					        // Lower right
					        p = pIn + stride -   3;
					        r6 = p[2];
					        g6 = p[1];
					        b6 = p[0];

					        // Next
					        p = pIn + stride;
					        r7 = p [2];
					        g7 = p [1];
					        b7 = p [0];

					        // Lower right
					        p = pIn + stride +   3;
					        r8 = p [2];
					        g8 = p [1];
					        b8 = p [0];

					        //
					        p = pIn;
					        r0 = p [2];
					        g0 = p [1];
					        b0 = p [0];

					        vR = (float) r0 - (float) (r1 + r2 + r3 + r4 + r5 + r6 + r7 + r8) /   8;
					        vG = (float) g0 - (float) (g1 + g2 + g3 + g4 + g5 + g6 + g7 + g8) /   8;
					        vB = (float) b0 - (float) (b1 + b2 + b3 + b4 + b5 + b6 + b7 + b8) /   8;

					        vR = r0 + vR * val;
					        vG = g0 + vG * val;
					        vB = b0 + vB * val;

					        if (vR>   0)
					        {
					        vR = Math.Min(255, vR);
					        }
					        else
					        {
					        vR = Math.Max (0, vR);
					        }

					        if (vG>   0)
					        {
					        vG = Math.Min(255, vG);
					        }
					        else
					        {
					        vG = Math.Max(0,vG);
					        }

					        if (vB>   0)
					        {
					        vB = Math.Min(255, vB);
					        }
					        else
					        {
					        vB = Math.Max(0, vB);
					        }

					        pOut [0] = (byte)vB;
					        pOut [1] = (byte)vG;
					        pOut [2] = (byte)vR;

					        }

					        pIn +=  3;
					        pOut += 3;
				        } 

				        pIn += srcData.Stride - w *   3;
				        pOut += srcData.Stride - w *   3;
			        }  
		        }

		        b.UnlockBits (srcData);
		        bmpRtn.UnlockBits (dstData);

		        return bmpRtn;
	        }
	        catch
	        {
		        return   null;
	        }
        }   

	}
}
