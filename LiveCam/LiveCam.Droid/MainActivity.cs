using Android.App;
using Android.Widget;
using Android.OS;
using Android.Gms.Vision;
using Android.Support.V4.App;
using Android.Support.V7.App;

using Android.Util;
using Android;
using Android.Support.Design.Widget;
using Android.Content;
using Android.Gms.Vision.Faces;
using Java.Lang;
using System;
using Android.Runtime;
using static Android.Gms.Vision.MultiProcessor;
using Android.Content.PM;
using Android.Gms.Common;
using LiveCam.Shared;
using System.Threading.Tasks;
using Android.Graphics;
using ServiceHelpers;
using Exception = Java.Lang.Exception;
using Android.Graphics;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
//using Java.IO;
using System.Drawing;

namespace LiveCam.Droid
{
    [Activity(Label = "LiveCam.Droid", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.NoActionBar", ScreenOrientation = ScreenOrientation.FullSensor)]
    public class MainActivity : AppCompatActivity, IFactory
    {
        private static readonly string TAG = "FaceTracker";

        private CameraSource _mCameraSource = null;

        private CameraSourcePreview _mPreview;
        private GraphicOverlay _mGraphicOverlay;
        private ImageButton _imgBtn;

        public static string GreetingsText{ get; set; }

        private static readonly int RC_HANDLE_GMS = 9001;
        // permission request codes need to be < 256
        private static readonly int RC_HANDLE_CAMERA_PERM = 2;

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _mPreview = FindViewById<CameraSourcePreview>(Resource.Id.preview);
            _mGraphicOverlay = FindViewById<GraphicOverlay>(Resource.Id.faceOverlay);
            _imgBtn = FindViewById<ImageButton>(Resource.Id.imageButton1);
            //greetingsText = FindViewById<TextView>(Resource.Id.greetingsTextView);


            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
            {
                CreateCameraSource(CameraFacing.Front);
                //LiveCamHelper.Init();
                //LiveCamHelper.GreetingsCallback = (s) => { RunOnUiThread(()=> GreetingsText = s ); };
                //await LiveCamHelper.RegisterFaces();
            }
            else { RequestCameraPermission(); }


        }

        protected override void OnResume()
        {
            base.OnResume();
            StartCameraSource();
        }

        protected override void OnPause()
        {
            base.OnPause();
            _mPreview.Stop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_mCameraSource != null)
            {
                _mCameraSource.Release();
            }
        }

        private void RequestCameraPermission()
        {
            Log.Warn(TAG, "Camera permission is not granted. Requesting permission");

            var permissions = new string[] { Manifest.Permission.Camera };

            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this,
                    Manifest.Permission.Camera))
            {
                ActivityCompat.RequestPermissions(this, permissions, RC_HANDLE_CAMERA_PERM);
                return;
            }

            Snackbar.Make(_mGraphicOverlay, Resource.String.permission_camera_rationale,
                    Snackbar.LengthIndefinite)
                    .SetAction(Resource.String.ok, (o) => { ActivityCompat.RequestPermissions(this, permissions, RC_HANDLE_CAMERA_PERM); })
                    .Show();
        }

        /**
         * Creates and starts the camera.  Note that this uses a higher resolution in comparison
         * to other detection examples to enable the barcode detector to detect small barcodes
         * at long distances.
         */
        private void CreateCameraSource(CameraFacing direction)
        {

            var context = Application.Context;
            FaceDetector detector = new FaceDetector.Builder(context)
                    .SetClassificationType(ClassificationType.All)
                    .Build();

            detector.SetProcessor(
                    new MultiProcessor.Builder(this)
                            .Build());

            if (!detector.IsOperational)
            {
                // Note: The first time that an app using face API is installed on a device, GMS will
                // download a native library to the device in order to do detection.  Usually this
                // completes before the app is run for the first time.  But if that download has not yet
                // completed, then the above call will not detect any faces.
                //
                // isOperational() can be used to check if the required native library is currently
                // available.  The detector will automatically become operational once the library
                // download completes on device.
                Log.Warn(TAG, "Face detector dependencies are not yet available.");
            }

            _mCameraSource = new CameraSource.Builder(context, detector)
                    .SetRequestedPreviewSize(640, 480)
                                            .SetFacing(CameraFacing.Front)
                    .SetRequestedFps(30.0f)
                    .Build();

            
        }

        /**
         * Starts or restarts the camera source, if it exists.  If the camera source doesn't exist yet
         * (e.g., because onResume was called before the camera source was created), this will be called
         * again when the camera source is created.
         */
        private void StartCameraSource()
        {

            // check that the device has play services available.
            int code = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(
                    this.ApplicationContext);
            if (code != ConnectionResult.Success)
            {
                Dialog dlg =
                        GoogleApiAvailability.Instance.GetErrorDialog(this, code, RC_HANDLE_GMS);
                dlg.Show();
            }

            if (_mCameraSource != null)
            {
                try
                {
                    _mPreview.Start(_mCameraSource, _mGraphicOverlay);
                }
                catch (System.Exception e)
                {
                    Log.Error(TAG, "Unable to start camera source.", e);
                    _mCameraSource.Release();
                    _mCameraSource = null;
                }
            }
        }
        public Tracker Create(Java.Lang.Object item)
        {
            return new GraphicFaceTracker(_mGraphicOverlay, _imgBtn, _mCameraSource);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode != RC_HANDLE_CAMERA_PERM)
            {
                Log.Debug(TAG, "Got unexpected permission result: " + requestCode);
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                return;
            }

            if (grantResults.Length != 0 && grantResults[0] == Permission.Granted)
            {
                Log.Debug(TAG, "Camera permission granted - initialize the camera source");
                // we have permission, so create the camerasource
                CreateCameraSource(CameraFacing.Front);
                return;
            }

            Log.Error(TAG, "Permission not granted: results len = " + grantResults.Length +
                    " Result code = " + (grantResults.Length > 0 ? grantResults[0].ToString() : "(empty)"));


            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetTitle("LiveCam")
                    .SetMessage(Resource.String.no_camera_permission)
                    .SetPositiveButton(Resource.String.ok, (o, e) => Finish())
                    .Show();

        }
    }


    class GraphicFaceTracker : Tracker, CameraSource.IPictureCallback
    {
        private GraphicOverlay mOverlay;
        private FaceGraphic mFaceGraphic;
        private CameraSource mCameraSource = null;
        private bool isProcessing = false;
        private ImageButton _img;
        private Face _face;

        public GraphicFaceTracker(GraphicOverlay overlay, ImageButton img, CameraSource cameraSource =null)
        {
            mOverlay = overlay;
            mFaceGraphic = new FaceGraphic(overlay);
            mCameraSource = cameraSource;
            _img = img;
        }

        public override void OnNewItem(int id, Java.Lang.Object item)
        {
            mFaceGraphic.SetId(id);
            try
            {
                if (mCameraSource != null && !isProcessing)
                    mCameraSource.TakePicture(null, this);

                _face = item as Face;
                Console.WriteLine($"position x: {_face.Position.X} position y: {_face.Position.Y} width: {_face.Width} height: {_face.Height}");
            }
            catch (System.Exception e)
            {
                Console.WriteLine("==================================================");
                Console.WriteLine(e);
                
                
            }
            
        }

        public override void OnUpdate(Detector.Detections detections, Java.Lang.Object item)
        {
            _face = item as Face;
            
            mOverlay.Add(mFaceGraphic);
            mFaceGraphic.UpdateFace(_face);
            
            
        }

        public override void OnMissing(Detector.Detections detections)
        {
            mOverlay.Remove(mFaceGraphic);

        }

        public override void OnDone()
        {
            mOverlay.Remove(mFaceGraphic);

        }

        private async Task<string> PostRecognition(Bitmap bitmap)
        {
            //convert to base64
            var client = new HttpClient();    //Iskelt kad ne ant kiekvieno siuntimo kurtu
            client.BaseAddress = new Uri("http://88.119.27.98:55555");

            var stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
            var content = new MultipartContent("application/octet-stream");
            content.Add(new StreamContent(stream));

            var response = await client.PostAsync("/api/recognize", content);

            Console.WriteLine("Response from /api/recognize is " + response.StatusCode);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            if (stream.Equals(null)) Console.WriteLine("The stream is null");
            else Console.WriteLine("the stream is not null");
            stream.Dispose();
            
            return response.StatusCode.ToString();









            //String encodedImage = Base64.EncodeToString()


            //var client = new HttpClient();
            //var content = new StringContent(
            //    JsonConvert.SerializeObject(new { username = "myusername", usage = "recognise" }));
            //var result = await client.PostAsync("localhost:8080", content).ConfigureAwait(false);
            //return result.ToString();
        }

        public void OnPictureTaken(byte[] data)
        {
            Task.Run(async () =>
            {
                try
                {
                    isProcessing = true;

                    Console.WriteLine("face detected: ");
                    var bitmap = Bitmap.CreateScaledBitmap(BitmapFactory.DecodeByteArray(data, 0, data.Length),240,320,false);
                    
                    if (_face != null)
                    {
                        //240width
                        //320height
                        //bitmap.Height = 320;
                        //bitmap.Width = 240;

                        Console.WriteLine($"position x: {_face.Position.X} position y: {_face. Position.Y} width: {_face.Width} height: {_face.Height} bitmapWidth: {bitmap.Width} bitmapHeight: {bitmap.Height}");

                        //var bitmapScalled = Bitmap.CreateScaledBitmap(bitmap, 128, 128, true);
                        //bitmap = Bitmap.CreateBitmap(bitmap, (int)_face.Position.X, (int)_face.Position.Y, (int)_face.Width, (int)_face.Height);


#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        Task.Factory.StartNew(() =>
                        {
                            System.Threading.Thread.Sleep(200);
                            bitmap = Bitmap.CreateBitmap(bitmap, (int)_face.Position.X/2, (int)_face.Position.Y/2, (int)_face.Width/2, (int)_face.Height/2);
                            bitmap = Bitmap.CreateScaledBitmap(bitmap, 240, 320, false);
                             _img.SetImageBitmap(bitmap);


                            Task<string> task = PostRecognition(bitmap);
                            
                            task.Wait();
                            var x = task.Result;
                            Console.WriteLine(x+" --- Post recognition response");



                        });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed


                    }
                    

                    var imageAnalyzer = new ImageAnalyzer(data);
                    await LiveCamHelper.ProcessCameraCapture(imageAnalyzer);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("======================================");
                    Console.WriteLine(ex);
                    throw;
                }
                finally
                {
                    isProcessing = false;


                }

            });
        }
    }

}


