using System.IO;
using System;
using Backend.Models;
using WhosThat.Recognition;
using WhosThat.Recognition.Util;
using Emgu.CV.Face;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV.CvEnum;

namespace Backend
{
    public class Recognizer
    {
        private EigenFaceRecognizer FaceRecognition { get; set; }
        private CascadeClassifier FaceDetection { get; set; }
        private CascadeClassifier EyeDetection { get; set; }
        private Mat Frame { get; set; }
        private List<Image<Gray, byte>> Faces { get; set; }
        private List<int> IDs { get; set; }
        private int ProcessedImageWidth { get; set; } = 128;
        private int ProcessedImageHeight { get; set; } = 128;
        private int TimerCounter { get; set; } = 0;
        private int TimeLimit { get; set; } = 20;
        private int ScanCounter { get; set; } = 0;
        private Timer Timer { get; set; }
        //private byte flags = (byte)Flags.EYE_SQUARE | (byte)Flags.FACE_SQUARE | (byte)Flags.IS_CAMERA_TAB;
        private const int _threshold = 3750;
        private int idToRemember;
        List<Person> listOfPeople = new List<Person>();
        List<Person> currentPeople = new List<Person>();
        private static string FaceHaarCascadePath = @"Recognition\HaarClassifiers\haarcascade_frontalface_default.xml";
        private static string EyeHaarCascadePath = @"Recognition\HaarClassifiers\haarcascade_eye.xml";
        private static string DetectorDataPath = @"TEST.xml";
        private string YMLPath { get; set; } = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Recognition\trainingData.yml";
        private RecognizerEngine engine;

        public Recognizer() // may change to private later (supposed to be a singleton)
        {
            Console.WriteLine("current path: " + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            EmguSingleton.Instance.SetUp
            (
                FaceHaarCascadePath,
                EyeHaarCascadePath,
                DetectorDataPath
            );
            engine = new RecognizerEngine("", YMLPath);
            if (File.Exists(YMLPath) && new FileInfo(YMLPath).Length > 0)
            {
                FaceRecognition.Read(YMLPath);
            }
            FaceDetection = new CascadeClassifier(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) +
                                                  @"\Recognition\HaarClassifiers\haarcascade_frontalface_default.xml");

            EyeDetection = new CascadeClassifier(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) +
                                                 @"\Recognition\HaarClassifiers\haarcascade_eye.xml");
            Frame = new Mat();
            Faces = new List<Image<Gray, byte>>();
            IDs = new List<int>();
        }

        private Person RecognisePerson(Mat Frame)
        {
            var imageFrame = Frame.ToImage<Bgr, byte>();
            if (imageFrame != null)
            {
                var grayFrame = imageFrame.Convert<Gray, byte>();
                var faces = FaceDetection.DetectMultiScale(grayFrame, 1.3, 5);
                //var eyes = EyeDetection.DetectMultiScale(grayFrame, 1.3, 5);

                if (faces.Count() != 0)
                {
                    foreach (var face in faces)
                    {
                        var processedImage = grayFrame.Copy(face).Resize(ProcessedImageWidth, ProcessedImageHeight, Emgu.CV.CvEnum.Inter.Cubic);
                        try
                        {
                            var result = FaceRecognition.Predict(processedImage);
                            var recognisedPerson = CheckRecognizeResults(result, _threshold);
                            /*
                            var personNameIfFound = recognisedPerson == null
                                ? "Spooky ghost no. " + result.Label.ToString()
                                : recognisedPerson.Name;
                            */
                            return recognisedPerson;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Mission failed: " + ex.Message + " Data: " + ex.Data);
                        }
                    }
                }
            }
            return null;
        }

        private void Timer_Tick(Mat Frame)
        {
            var imageFrame = Frame.ToImage<Gray, byte>();
            if (TimerCounter < TimeLimit)
            {
                TimerCounter++;
                Storage.People.Last().Images.Add(Frame.ToImage<Bgr, byte>().ToBitmap());	//bit wonky but whatever
                /*if (imageFrame != null)
                {
                    var faces = FaceDetection.DetectMultiScale(imageFrame, 1.3, 5);
                    
                    if (faces.Count() > 0)
                    {
                        var processedImage = imageFrame.Copy(faces[0]).Resize(ProcessedImageWidth, ProcessedImageHeight,
                            Emgu.CV.CvEnum.Inter.Cubic);
                        Faces.Add(processedImage);
                        IDs.Add(int.Parse(txtNewFaceName.Text));
                        Console.WriteLine("ID: "+IDs[IDs.Count-1]);

                        ScanCounter++;
                        Console.WriteLine($"{ScanCounter} Successful Scans Taken...");
                       
                    }
                }*/
            }
            else
            {
                engine.TrainRecognizer();
                EndTraining(true);
                /*if (Faces.Count > 0)
	            {
	                System.Diagnostics.Debug.WriteLine("ADDED FACE IMAGES FOR TRAINING: " + Faces.ToArray().Length + '\n');
	                System.Diagnostics.Debug.WriteLine("IDs array length " + IDs.ToArray().Length);
	                FaceRecognition.Train(Faces.ToArray(), IDs.ToArray());
	            }
	            EndTraining(Faces.Count > 0);*/
            }
        }

        private void EndTraining(bool facesDetected)
        {
            //FaceRecognition.Write(YMLPath);
            //Timer.Stop();
            TimerCounter = 0;
            Console.Write($"Training Complete! {Environment.NewLine}");
            if (!facesDetected)
            {
                Console.WriteLine("ERROR: No faces detected during training.");
                return;
            }
        }

        public int RecognizeUser(Image<Bgr, Byte> userImage)
        {
            FaceRecognition.Read(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            var result = FaceRecognition.Predict(userImage.Resize(100, 100, Inter.Cubic));
            return result.Label;
        }

        private void recognizeButton_Click(object sender, EventArgs e)
        {
            //Webcam.Retrieve(Frame);
            var imageFrame = Frame.ToImage<Gray, byte>();

            if (imageFrame != null)
            {
                var faces = FaceDetection.DetectMultiScale(imageFrame, 1.3, 5);
                Console.WriteLine($"Faces detected: {faces.Count()}");
                if (faces.Count() != 0)
                {
                    var processedImage = imageFrame.Copy(faces[0]).Resize(ProcessedImageWidth, ProcessedImageHeight, Emgu.CV.CvEnum.Inter.Cubic);
                    try
                    {
                        var result = FaceRecognition.Predict(processedImage);
                        Console.WriteLine(CheckRecognizeResults(result, _threshold));
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine("No faces trained, can't recognize");
                    }
                }
                else
                {
                    //Console.WriteLine("No faces found");
                }

            }
        }

        private Person CheckRecognizeResults(FaceRecognizer.PredictionResult result, int threshold)
        {
            // @param threshold should usually be in [0, 5000]
            string EigenLabel;
            float EigenDistance = -1;
            if (result.Label == -1)
            {
                EigenLabel = "Unknown";
                EigenDistance = 0;
                return null;
            }
            else
            {
                EigenLabel = result.Label.ToString();
                EigenDistance = (float)result.Distance;
                //EigenLabel = EigenDistance > threshold ? "Unknown" : result.Label.ToString();
                if (EigenDistance < threshold)
                {
                    return Storage.FindPersonByID(result.Label);
                }

            }

            return null;
            //return EigenLabel;// + '\n' + "Distance: " + EigenDistance.ToString();

        }

    }
}