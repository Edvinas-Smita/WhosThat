using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xamarin.Forms;

using Emgu.CV;

namespace BigBrother.ViewModels
{
    class RecognitionViewModel : INotifyPropertyChanged
    {
	    public event PropertyChangedEventHandler PropertyChanged = delegate { };

	    private ImageSource imgSource;

	    public ImageSource ImgSource
	    {
		    get { return imgSource; }
		    set
		    {
			    imgSource = value;
			    PropertyChanged(this, new PropertyChangedEventArgs("ImgSource"));
		    }
	    }

	    public void SetSourceFromMat(Mat mat)
		{
			if (mat == null || mat.DataPointer == IntPtr.Zero || mat.Cols == 0 || mat.Rows == 0)
			{
				Debug.WriteLine("Retreived a bad mat");
				return;
			}
			try
			{
				ImgSource = ImageSource.FromStream(() => new MemoryStream(mat.GetData()));
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
    }
}
