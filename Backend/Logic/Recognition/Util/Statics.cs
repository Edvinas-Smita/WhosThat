using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections;
using System.Drawing;
using Emgu.CV.CvEnum;

namespace WhosThat.Recognition.Util
{
	static class Statics
	{
		//@param every face image should already contain only one face
		public static void TrainSinglePersonFaces(IList faceImages, int id)
		{
			var grayScaleFaces = new Image<Gray, byte>[faceImages.Count];
			var arrayFromId = new int[faceImages.Count];
			int failsOrSkips = 0;
			for (int i = 0; i < faceImages.Count; i++)
			{
				var grayScaleFull = (Image<Gray, byte>) faceImages[i];	//240x320 grayscale image
				var faceRects = EmguSingleton.Instance.FaceDetector.DetectMultiScale(grayScaleFull, 1.3, 5);
				if (faceRects.Length > 0)
				{
					grayScaleFaces[i - failsOrSkips] = grayScaleFull.Copy(faceRects[0]).Resize(240, 320, Inter.Cubic);
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

			EmguSingleton.Instance.RecognizerIsTrained = true;
		}

		public static int RecognizeUser(Image<Gray, byte> userFaceImage)
		{
			return EmguSingleton.Instance.Recognizer.Predict(userFaceImage).Label;
		}

		public static Image<Gray, byte> ByteArrayToImage(byte[] array, int width, int height)
		{
			var image = new Image<Gray,byte>(new Size(width, height));
			image.Bytes = array;
			return image;
		}

		public static T[] SubArray<T>(this T[] original, int index, int length)
		{
			T[] subArray = new T[length];
			Array.Copy(original, index, subArray, 0, length);
			return subArray;
		}
	}
}
