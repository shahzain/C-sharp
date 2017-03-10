using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using OpenCvSharp;

namespace LaneDetection
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            // CvCapture cap = CvCapture.FromFile("video.avi");
            CvCapture cap = CvCapture.FromFile("road_3.avi");

            CvWindow w = new CvWindow("Lane Detection");
            CvWindow canny = new CvWindow("Lane Detection_2");
            CvWindow hough = new CvWindow("Lane Detection");
         //   CvWindow smoothing = new CvWindow("Lane Detection_3");


            IplImage src, gray, dstCanny, halfFrame, smallImg;
            CvMemStorage storage = new CvMemStorage();
            CvSeq lines;
            CvHaarClassifierCascade cascade = CvHaarClassifierCascade.FromFile("haarcascade_cars3.xml");

            const double Scale = 2.0;
            const double ScaleFactor = 1.05;
            const int MinNeighbors = 3;
            double min_range = 70;
            double max_range = 120;


            CvSeq<CvAvgComp> cars;

            while (CvWindow.WaitKey(10) < 0)
            {
                src = cap.QueryFrame();
                halfFrame = new IplImage(new CvSize(src.Size.Width / 2, src.Size.Height / 2), BitDepth.U8, 3);
                Cv.PyrDown(src, halfFrame, CvFilter.Gaussian5x5);

                
                gray = new IplImage(src.Size, BitDepth.U8, 1);
               
                dstCanny = new IplImage(src.Size, BitDepth.U8, 1);
                /*

               smallImg = new IplImage(new CvSize(Cv.Round(src.Width / Scale), Cv.Round(src.Height / Scale)), BitDepth.U8, 1);
               using (IplImage grey = new IplImage(src.Size, BitDepth.U8, 1))
               {
                   Cv.CvtColor(src, grey, ColorConversion.BgrToGray);
                   Cv.Resize(grey, smallImg, Interpolation.Linear);
                   Cv.EqualizeHist(smallImg, smallImg);
               }

               cars = Cv.HaarDetectObjects(smallImg, cascade, storage, ScaleFactor, MinNeighbors, HaarDetectionType.DoCannyPruning, new CvSize(30, 30));

               for (int i = 0; i < cars.Total; i++)
               {
                   CvRect r = cars[i].Value.Rect;
                   CvPoint center = new CvPoint
                   {
                       X = Cv.Round((r.X + r.Width * 0.5) * Scale),
                       Y = Cv.Round((r.Y + r.Height * 0.5) * Scale)
                   };
                   int radius = Cv.Round((r.Width + r.Height) * 0.25 * Scale);
                   src.Circle(center, radius, CvColor.Blue, 2, LineType.AntiAlias, 0);
               } */

                // Crop off top half of image since we're only interested in the lower portion of the video
                int halfWidth = src.Width / 2;
                int halfHeight = src.Height / 2;
                int startX = halfWidth - (halfWidth / 2);
                src.SetROI(new CvRect(0, halfHeight - 0, src.Width - 1, src.Height - 1));

                gray.SetROI(src.GetROI());
                dstCanny.SetROI(src.GetROI());

                src.CvtColor(gray, ColorConversion.BgrToGray);
                Cv.Smooth(gray, gray, SmoothType.Gaussian, 5, 5);
                Cv.Canny(gray, dstCanny, 50, 200, ApertureSize.Size3);

                storage.Clear();
                lines = dstCanny.HoughLines2(storage, HoughLinesMethod.Probabilistic, 1, Math.PI / 180, 50, 50, 100);

                for (int i = 0; i < lines.Total; i++)
                {
                    CvLineSegmentPoint elem = lines.GetSeqElem<CvLineSegmentPoint>(i).Value;

                    int dx = elem.P2.X - elem.P1.X;
                    int dy = elem.P2.Y - elem.P1.Y;
                    double angle = Math.Atan2(dy, dx) * 180 / Math.PI;

                 //   if (Math.Abs(angle) <= 10)
                   //     continue;

                    if (elem.P1.Y > elem.P2.Y + 50 || elem.P1.Y < elem.P2.Y - 50)
                    {
                        src.Line(elem.P1, elem.P2, CvColor.Green, 9, LineType.Link8, 0);
                    }
                }

                src.ResetROI();
                

                storage.Clear();
                w.Image = src;
               // canny.Image = dstCanny;
               // smoothing.Image = gray;
            //    w.Image = dstCanny;
              //  w.Image = dstCanny;
            }
        }
    }
}
