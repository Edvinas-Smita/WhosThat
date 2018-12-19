using Emgu.CV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Face;
using System.Drawing;
using Emgu.CV.Structure;

namespace Backend.Logic.Recognition.Util
{
	class EmguSingleton
	{
		private static EmguSingleton instance = new EmguSingleton();
		public static EmguSingleton Instance {
			get
			{
				if (!instance.IsInitialised)
				{
					Initialize();
				}
				return instance;
			}
		}

		public CascadeClassifier FaceDetector { get; set; }
		public CascadeClassifier EyeDetector { get; set; }

		public LBPHFaceRecognizer Recognizer { get; } = new LBPHFaceRecognizer();
		public bool RecognizerIsTrained = false;

		public bool IsInitialised = false;
		private const string DefaultFace = @"C:\TOP_BB\lbpcascade_frontalface_improved.xml", DefaultEye = @"C:\TOP_BB\haarcascade_eye.xml", DefaultData = @"C:\TOP_BB\data.xml";

		public bool SetUp(string faceHaarCascadePath, string eyeHaarCascadePath, string recognizerDataPath)
		{
			if (File.Exists(faceHaarCascadePath) && File.Exists(eyeHaarCascadePath))
			{
				FaceDetector = new CascadeClassifier(faceHaarCascadePath);
				EyeDetector = new CascadeClassifier(eyeHaarCascadePath);
				if (File.Exists(recognizerDataPath))
				{
					Recognizer.Read(recognizerDataPath);
					RecognizerIsTrained = true;
				}

				IsInitialised = true;
				return true;
			}

			return false;
		}

		public static void Initialize(string faceCascadePath = DefaultFace, string eyeCascadePath = DefaultEye, string recognizerDataPath = DefaultData)
		{
			instance.SetUp(faceCascadePath, eyeCascadePath, recognizerDataPath);
		}

		public static void SaveData(string path = DefaultData)
		{
			instance.Recognizer.Write(path);
		}

		public static Rectangle[] DetectFacesFromBitmap(Bitmap bitmap)
		{
			var grayscale = new Image<Gray, byte>(bitmap);
			return Instance.FaceDetector.DetectMultiScale(grayscale, 1.3, 5);
		}
		public static Rectangle[] DetectFacesFromGrayscale(Image<Gray, byte> grayscale)
		{
			return Instance.FaceDetector.DetectMultiScale(grayscale, 1.3, 5);
		}
	}
}
