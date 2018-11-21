using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections;
using System.Drawing;

namespace WhosThat.Recognition.Util
{
	class Statics
	{
		//@param every face image should already contain only one face
		public static void TrainSinglePersonFaces(IList faceImages, int id)
		{
			var grayScaleFaces = new Image<Gray, byte>[faceImages.Count];
			var arrayFromId = new int[faceImages.Count];
			int failsOrSkips = 0;
			for (int i = 0; i < faceImages.Count; i++)
			{
				Bitmap image = (Bitmap) faceImages[i];
				var grayScaleFull = new Image<Gray, byte>(image);
				var faceRects = EmguSingleton.Instance.FaceDetector.DetectMultiScale(grayScaleFull, 1.3, 5);
				if (faceRects.Length > 0)
				{
					grayScaleFaces[i - failsOrSkips] = grayScaleFull.Copy(faceRects[0]);
					arrayFromId[i - failsOrSkips] = id;
				}
				else
				{
					++failsOrSkips;
					Array.Resize(ref grayScaleFaces, grayScaleFaces.Length - 1);
					Array.Resize(ref arrayFromId, arrayFromId.Length - 1);
				}
				grayScaleFull.Dispose();
			}

			if (grayScaleFaces.Length > 0)
			{
				EmguSingleton.Instance.Recognizer.Update(grayScaleFaces, arrayFromId);
			}
			foreach (var face in grayScaleFaces)
			{
				face.Dispose();
			}
		}
	}
}
