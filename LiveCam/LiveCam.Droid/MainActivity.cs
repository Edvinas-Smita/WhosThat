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
                CreateCameraSource(CameraFacing.Back);
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
                                            .SetFacing(CameraFacing.Back)
                    .SetRequestedFps(60.0f)
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
                CreateCameraSource(CameraFacing.Back);
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
            }
            catch (System.Exception e)
            {
                Console.WriteLine("==================================================");
                Console.WriteLine(e);
                
            }
            
        }

        public override void OnUpdate(Detector.Detections detections, Java.Lang.Object item)
        {
            var face = item as Face;
            mOverlay.Add(mFaceGraphic);
            mFaceGraphic.UpdateFace(face);
            
            
        }

        public override void OnMissing(Detector.Detections detections)
        {
            mOverlay.Remove(mFaceGraphic);

        }

        public override void OnDone()
        {
            mOverlay.Remove(mFaceGraphic);

        }

        public void OnPictureTaken(byte[] data)
        {
            Task.Run(async () =>
            {
                try
                {
                    isProcessing = true;

                    Console.WriteLine("face detected: ");

                    _img.SetImageBitmap(BitmapFactory.DecodeByteArray(data, 0, data.Length));

                    var imageAnalyzer = new ImageAnalyzer(data);
                    await LiveCamHelper.ProcessCameraCapture(imageAnalyzer);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("======================================");
                    Console.WriteLine(ex);
                }
                finally
                {
                    isProcessing = false;


                }

            });
        }
    }

}


