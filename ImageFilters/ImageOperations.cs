using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace IMAGE_FILTERS
{
    public class ImageOperations
    {
        public static byte[,] OpenImage(string ImagePath)
        {
            // gets Image path and create the bitmap file of the chosen image with the px header and body of the image 
            /*
            Bitmap original_bm = new Bitmap(ImagePath);
            */
            Bitmap original_bm = (Bitmap)Image.FromFile(ImagePath);

            // get height and Width of image
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            // Contains image (Bitmap) while working on it.
            byte[,] Buffer = new byte[Height, Width];


            // unsfe keyword used to use the pointer variable (whose value is the address of another variable)
            unsafe
            {
                Rectangle rec = new Rectangle(0, 0, Width, Height);

                BitmapData bmd = original_bm.LockBits(rec, ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                // check if the pixel format of the image is (24bit/32bit/8bit...) ,

                // if true (width*3) to convert 24bit to 3 bytes. {bit=0.125 byte}
                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;

                    nWidth = Width * 3;
                }

                // if true (width*3) to convert 32 bit to 4 bytes.

                //********
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }

                // if true (width*3) to convert 8 bit to 1 bytes.
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }

                // The stride is the width of a single row of pixels (a scan line).
                // offset values work when you enlarge an image. When you resize an image, the graphics method examines the pixels in the result image.

                //***************
                int nOffset = bmd.Stride - nWidth;


                // declare pointer P of type byte , whose value is the address of the fisrt pixel in the image
                // Get the address of the first line.
                byte* p = (byte*)bmd.Scan0;

                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        // if the bitmap data of the image is of format 8 bits (1 bit= 1 byte):
                        if (Format8)
                        {
                            Buffer[y, x] = p[0];
                            p++;
                        }

                        // else if the bitmap data of the image is of format 24/32 bits:
                        else
                        {
                            // *************************
                            Buffer[y, x] = (byte)((int)(p[0] + p[1] + p[2]) / 3);

                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }


                // LockBits method to lock an existing bitmap in system memory so that it can be changed programmatically
                original_bm.UnlockBits(bmd);
            }

            // return the buffer containing the image bitmapdata
            return Buffer;
        }


        // define a multi-dimensional array (ImageMatrix) containign te image  


        // returns number of elements in the row direction of 2d array (ImageMatrix)
        public static int GetHeight(byte[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }


        // returns number of elements in the column direction oof 2d array (ImageMatrix)
        public static int GetWidth(byte[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }


        // create display image function that takes the 2d array of the image + the picturebox
        public static void DisplayImage(byte[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);


            // create new bitmap data object contains: Data of Pixels {Wdith, Height, PixelFormat} 
            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);


            // unsfe keyword used to use the pointer variable (whose value is the address of another variable)
            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;

                for (int i = 0; i < Height; i++)

                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[0] = p[1] = p[2] = ImageMatrix[i, j];
                        p += 3;
                    }

                    p += nOffset;
                }

                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }


        /*
        public static byte[] K_Select_Th(byte[] Array, int ArrayLength, byte Max, byte Min)
        {
            byte[] New_Arr = new byte[ArrayLength];

            int pos1 =0, pos2=0 ;
            int min = 0;
            int max = 0;
            
            for(int i=0; i < ArrayLength; i++)
            {
                if(Array[i] < min)
                {
                    min = Array[i];
                }

                else if(Array[i] > max)
                {
                    max = Array[i];
                }

                if(i == Array[ArrayLength - 1])
                {
                    for(int k = 0; k < ArrayLength; k++)
                    {
                        if (max == Array[k])
                        {
                            pos1 = Array[k];
                        }

                        if (min == Array[k])
                        {
                            pos2 = Array[k];
                        }
                    }


                    return Array;

                }
            }

            for (int i = pos1 - 1; i < ArrayLength; i++)
            {
                Array[i] = Array[i + 1];
            }
            for (int i = pos2 - 1; i < ArrayLength; i++)
            {
                Array[i] = Array[i + 1];
            }

            int sum = 0;

            for (int L = 0; L < ArrayLength; L++)
            {
                sum = sum + Array[L];

            }

            int avg = sum / ArrayLength;

        }

        */

        // QUICK_SORT 
        public static int PARTITION(byte[] Array, int ArrayLength)

        {

            byte SmallByte = Array[ArrayLength];
            byte Temp;
            // p=0 the begining of the array
            int i = 0;
            for (int j = 0; j < ArrayLength; j++)
            {
                if (Array[j] <= SmallByte)
                {
                    Temp = Array[j];
                    Array[j] = Array[i];
                    Array[i++] = Temp;
                }
            }
            Temp = Array[i];
            Array[i] = Array[ArrayLength];
            Array[ArrayLength] = Temp;
            return i;
        }


        public static byte[] QUICK_SORT(byte[] Array,int p, int ArrayLength)
        {
            // p = 0 as the begining of the index
            if (p < ArrayLength)
            {
                int q = PARTITION(Array, ArrayLength);
                QUICK_SORT(Array, 0, q - 1);
                QUICK_SORT(Array, q + 1, ArrayLength);
            }
            return Array;
        }

        // COUNTING_SORT
        public static byte[] COUNTING_SORT(byte[] Array, int ArrayLength, byte Max, byte Min)
        {
            byte[] count = new byte[Max - Min + 1];
            int z = 0;

            for (int i = 0; i < count.Length; i++) { count[i] = 0; }

            for (int i = 0; i < ArrayLength; i++) { count[Array[i] - Min]++; }

            for (int i = Min; i <= Max; i++)
            {
                while (count[i - Min]-- > 0)
                {
                    Array[z] = (byte)i;
                    z++;
                }
            }
            return Array;
        }


        public static byte Alpha_Filter(byte[,] ImageMatrix, int x, int y, int Wmax, int Sort)
        {
            byte[] Array;
            int[] Dx, Dy;

            if (Wmax % 2 != 0)
            {
                Array = new byte[Wmax * Wmax];
                Dx = new int[Wmax * Wmax];
                Dy = new int[Wmax * Wmax];
            }

            else
            {
                Array = new byte[(Wmax + 1) * (Wmax + 1)];
                Dx = new int[(Wmax + 1) * (Wmax + 1)];
                Dy = new int[(Wmax + 1) * (Wmax + 1)];
            }


            int Index = 0;


            for (int _y = -(Wmax / 2); _y <= (Wmax / 2); _y++)
            {
                for (int _x = -(Wmax / 2); _x <= (Wmax / 2); _x++)
                {
                    Dx[Index] = _x;
                    Dy[Index] = _y;
                    Index++;
                }
            }


            byte Max, Min, Z;
            int ArrayLength, Sum, NewY, NewX, Avg;
            Sum = 0;
            Max = 0;
            Min = 255;
            ArrayLength = 0;
            Z = ImageMatrix[y, x];
            for (int i = 0; i < Wmax * Wmax; i++)
            {
                NewY = y + Dy[i];
                NewX = x + Dx[i];
                if (NewX >= 0 && NewX < GetWidth(ImageMatrix) && NewY >= 0 && NewY < GetHeight(ImageMatrix))
                {
                    Array[ArrayLength] = ImageMatrix[NewY, NewX];
                    if (Array[ArrayLength] > Max)
                        Max = Array[ArrayLength];
                    if (Array[ArrayLength] < Min)
                        Min = Array[ArrayLength];
                    Sum += Array[ArrayLength];
                    ArrayLength++;
                }
            }

            if (Sort == 1) Array = QUICK_SORT(Array,0, ArrayLength - 1);
            else if (Sort == 2) Array = COUNTING_SORT(Array, ArrayLength, Max, Min);
            else if (Sort == 3) Array = COUNTING_SORT(Array, ArrayLength, Max, Min);

            Sum -= Max;
            Sum -= Min;
            ArrayLength -= 2;
            Avg = Sum / ArrayLength;
            return (byte)Avg;
        }
        


        public static byte Adaptive(byte[,] ImageMatrix, int x, int y, int W, int Wmax, int Sort)
        {

            byte[] Array = new byte[W * W];
            int[] Dx = new int[W * W];
            int[] Dy = new int[W * W];
            int Index = 0;
            for (int _y = -(W / 2); _y <= (W / 2); _y++)
            {
                for (int _x = -(W / 2); _x <= (W / 2); _x++)
                {
                    Dx[Index] = _x;
                    Dy[Index] = _y;
                    Index++;
                }
            }
            byte Max, Min, Med, Z;
            int A1, A2, B1, B2, ArrayLength, NewY, NewX;
            Max = 0;
            Min = 255;
            ArrayLength = 0;
            Z = ImageMatrix[y, x];
            for (int i = 0; i < W * W; i++)
            {
                NewY = y + Dy[i];
                NewX = x + Dx[i];
                if (NewX >= 0 && NewX < GetWidth(ImageMatrix) && NewY >= 0 && NewY < GetHeight(ImageMatrix))
                {
                    Array[ArrayLength] = ImageMatrix[NewY, NewX];
                    if (Array[ArrayLength] > Max)
                        Max = Array[ArrayLength];
                    if (Array[ArrayLength] < Min)
                        Min = Array[ArrayLength];
                    ArrayLength++;
                }
            }

            if (Sort == 1) Array = QUICK_SORT(Array,0, ArrayLength - 1);
            else if (Sort == 2) Array = COUNTING_SORT(Array, ArrayLength, Max, Min);
            else if (Sort == 3) Array = COUNTING_SORT(Array, ArrayLength, Max, Min);


            Min = Array[0];
            Med = Array[ArrayLength / 2];
            A1 = Med - Min;
            A2 = Max - Med;
            if (A1 > 0 && A2 > 0)
            {
                B1 = Z - Min;
                B2 = Max - Z;
                if (B1 > 0 && B2 > 0)
                    return Z;
                else
                {
                    if (W + 2 < Wmax)
                        return Adaptive(ImageMatrix, x, y, W + 2, Wmax, Sort);
                    else
                        return Med;
                }
            }
            else
            {
                return Med;
            }
        }


        public static byte[,] ImageFilter(byte[,] ImageMatrix, int Max_Size, int Sort, int filter)
        {
            byte[,] ImageMatrix2 = ImageMatrix;
            for (int y = 0; y < GetHeight(ImageMatrix); y++)
            {
                for (int x = 0; x < GetWidth(ImageMatrix); x++)
                {
                    if (filter == 1)
                        ImageMatrix2[y, x] = Alpha_Filter(ImageMatrix, x, y, Max_Size, Sort);
                    else
                        ImageMatrix2[y, x] = Adaptive(ImageMatrix, x, y, 3, Max_Size, Sort);
                }
            }
            return ImageMatrix2;
        }
    }
}
