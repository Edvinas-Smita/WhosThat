using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using WhosThat.Recognition.Util;
using WhosThat.Recognition;

namespace WhosThat.Recognition
{
    class RecognizerEngine
    {
        public EigenFaceRecognizer _faceRecognizer;
        //private DataStoreAccess _dataStoreAccess;
        private String _recognizerFilePath;

        public RecognizerEngine(String databasePath, String recognizerFilePath)
        {
            _recognizerFilePath = recognizerFilePath;
            //_dataStoreAccess = new DataStoreAccess(databasePath);
            _faceRecognizer = new EigenFaceRecognizer(80, double.PositiveInfinity);
        }

        public bool TrainRecognizer(int widthToProccess = 128, int heightToProccess = 128)
        {
	        var allPeople = Storage.People;
			if (allPeople != null)
			{
				List<Bitmap> fullImageList = new List<Bitmap>();
				List<int> fullIdList = new List<int>();
				foreach (var person in allPeople)
				{
					fullImageList.AddRange(person.Images);
					foreach (var notUsed in person.Images)
					{
						fullIdList.Add(person.Id);
					}
				}
				var grayScaleFaces = new Image<Gray, byte>[fullImageList.Count];
				for (int i = 0; i < fullImageList.Count; i++)
				{
					Bitmap image = fullImageList[i];

					var grayScaleFull = new Image<Gray, byte>(image);
					var faceRects = EmguSingleton.DetectFacesFromGrayscale(grayScaleFull);
					if (faceRects.Length > 0)
					{
						grayScaleFaces[i] = grayScaleFull.Copy(faceRects[0]).Resize(widthToProccess, heightToProccess, Inter.Cubic);
					} else
					{
						grayScaleFaces[i] = grayScaleFull.Clone().Resize(widthToProccess, heightToProccess, Inter.Cubic);
					}
					grayScaleFull.Dispose();
				}
				_faceRecognizer.Train(grayScaleFaces, fullIdList.ToArray());
				_faceRecognizer.Write(_recognizerFilePath);
				foreach (var grayScaleFace in grayScaleFaces)
				{
					grayScaleFace.Dispose();
				}
			}
            /*var allFaces = Storage.Faces;
            if (allFaces != null)
            {
                var faceImages = new Image<Gray, byte>[allFaces.Count()];
                var faceLabels = new int[allFaces.Count()];
                for (int i = 0; i < allFaces.Count(); i++)
                {
                    Stream stream = new MemoryStream();
                    stream.Write(allFaces[i].Image, 0, allFaces[i].Image.Length);
                    var faceImage = new Image<Gray, byte>(new Bitmap(stream));
                    faceImages[i] = faceImage.Resize(100, 100, Inter.Cubic);
                    faceLabels[i] = allFaces[i].UserId;
                }
                _faceRecognizer.Train(faceImages, faceLabels);
                _faceRecognizer.Write(_recognizerFilePath);
            }*/
            return true;
        }

        public void LoadRecognizerData()
        {
            _faceRecognizer.Read(_recognizerFilePath);
        }

        public int RecognizeUser(Image<Gray, byte> userImage)
        {
              Stream stream = new MemoryStream();
              stream.Write(userImage.Bytes, 0, userImage.Bytes.Length);
              var faceImage = new Image<Gray, byte>(new Bitmap(stream));
            _faceRecognizer.Read(_recognizerFilePath);

            var result = _faceRecognizer.Predict(userImage.Resize(100, 100, Inter.Cubic));
            return result.Label;
        }
    }
}
