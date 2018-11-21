using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;

namespace WhosThat.Recognition
{
    class RecognizerEngine
    {
        private FaceRecognizer _faceRecognizer;
        //private DataStoreAccess _dataStoreAccess;
        private String _recognizerFilePath;

        public RecognizerEngine(String databasePath, String recognizerFilePath)
        {
            _recognizerFilePath = recognizerFilePath;
            //_dataStoreAccess = new DataStoreAccess(databasePath);
            _faceRecognizer = new EigenFaceRecognizer(80, double.PositiveInfinity);
        }

        /*public bool TrainRecognizer()
        {
            var allFaces = Storage.Faces;
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
            }
            return true;

        }*/

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
