using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO;
using BigBrother.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Internals;

using Emgu.CV;
using Emgu.CV.CvEnum;

namespace BigBrother.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RecognitionPage : ContentPage
	{
		private VideoCapture capture = null;
		private RecognitionViewModel viewModel = new RecognitionViewModel();
		private bool debug = false;
		public RecognitionPage ()
		{
			InitializeComponent ();
			
			BindingContext = viewModel;
			
			
			try
			{
				capture = new VideoCapture(CaptureType.Any);
				capture.
				/*if (File.Exists(@"DeltaHeavy_WhiteFlag.mp4"))   //Video capture from file (only works for UWPx86)
				{
					capture = new VideoCapture(@"DeltaHeavy_WhiteFlag.mp4");
				} else
				{
					Debug.WriteLine(Directory.GetCurrentDirectory() + @"\DeltaHeavy_WhiteFlag.mp4 - File not found!");
				}*/

				if (capture != null)
				{
					Debug.WriteLine("======================================"+capture.IsOpened);
					Timer timer = new Timer(1000 / 0.25);
					timer.Elapsed += DoStuff;
					timer.AutoReset = true;
					timer.Start();

					/*capture.ImageGrabbed += OnImageGrabbed;
					capture.Start();*/
				}
				else
				{
					Debug.WriteLine("Something went wrong :(");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private void DoStuff(object sender, EventArgs args)
		{
		    Mat mat = new Mat();
		    capture.Retrieve(mat);
			viewModel.SetSourceFromMat(mat);    //updating image from mat
			if (mat != null)
			{
				Debug.WriteLine("empty: {}", mat.IsEmpty);
			}
			else
			{
				Debug.WriteLine("mat is null");
			}
			//mat.Dispose();
		}

		private void OnImageGrabbed(object sender, EventArgs args)
		{
			var mat = new Mat();
			capture.Read(mat);
			viewModel.SetSourceFromMat(mat);    //updating image from mat
		}

		private void DebugCaptureFrame(object sender, EventArgs e)
		{
		    Console.WriteLine("Debug capture frame");
			if (debug)
			{
				var mat = new Mat();
				capture.Read(mat);
				viewModel.SetSourceFromMat(mat);    //updating image from mat
				Debug.WriteLine(mat);
				//mat.Dispose();
			}
			else
			{
				capture.Grab();
			}

			debug = !debug;
		}
	}
}