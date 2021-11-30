using FaceRecognitionDotNet;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Image = FaceRecognitionDotNet.Image;

namespace FaceAuthentication
{
    public partial class frmAuthentication : Form
    {
        public Bitmap CurrentRawPicture { get; private set; }
        public frmAuthentication()
        {
            InitializeComponent();
        }
        private void frmAuthentication_Load(object sender, EventArgs e)
        {
            StartMonitoring();
        }
        public void StartMonitoring()
        {
            Task.Run(() =>
            {
                log("سیستم در حال راه اندازی");
                string directory = Path.GetFullPath("Models");
                if (!Directory.Exists(directory))
                {
                    log($"Please check whether model directory '{directory}' exists");
                }

                FaceRecognition _faceRecognition = FaceRecognition.Create(directory);

                List<Image> frames = new List<Image>();
                int frameCount = 0;

                using (VideoCapture capture = new VideoCapture(0))
                {
                    while (capture.IsOpened())
                    {
                        try
                        {
                            DateTime dtStart = DateTime.Now;

                            // Grab a single frame of video
                            using (Mat frame = new Mat())
                            {
                                bool ret = capture.Read(frame);

                                // Bail out when the video file ends
                                if (!ret || !frame.IsContinuous())
                                {
                                    break;
                                }

                                // Convert the image from BGR color (which OpenCV uses) to RGB color (which face_recognition uses)
                                using (Mat tmp = frame.CvtColor(ColorConversionCodes.BGR2RGB))
                                {
                                    byte[] array = new byte[tmp.Width * tmp.Height * tmp.ElemSize()];
                                    Marshal.Copy(tmp.Data, array, 0, array.Length);

                                    Image image = FaceRecognition.LoadImage(array, tmp.Rows, tmp.Cols, tmp.Width * tmp.ElemSize(), Mode.Rgb);

                                    // Save each frame of the video to a list
                                    frameCount += 1;
                                    frames.Add(image);
                                }
                            }

                            Location[] batchOfFaceLocations = _faceRecognition.FaceLocations(frames.FirstOrDefault(), 0).ToArray();

                            Location[] faceLocations = batchOfFaceLocations;
                            int numberOfFacesInFrame = batchOfFaceLocations.Length;

                            Bitmap img = frames.FirstOrDefault().ToBitmap();
                            CurrentRawPicture = img;
                            string pathB = Application.StartupPath + "\\Faces\\Current.jpg";
                            if (File.Exists(pathB))
                            {
                                File.Delete(pathB);
                            }
                            img.Save(pathB, System.Drawing.Imaging.ImageFormat.Jpeg);
                            pbLive.Invoke((MethodInvoker)delegate
                            {
                                pbLive.Image = img;
                            });

                            if (faceLocations.Any())
                            {
                                foreach (Location faceLocation in faceLocations)
                                {
                                    ShowInForm(faceLocation, img);
                                    Authenticate(_faceRecognition, faceLocation, img);
                                    var cropped = cropAtRect(img, faceLocation);
                                    LogOnHistory(cropped);
                                }
                            }
                            else
                            {
                                //lblFace.Invoke((MethodInvoker)delegate
                                //{
                                //    lblFace.Text = "Not Detected";
                                //});
                            }

                            // Clear the frames array to start the next batch
                            foreach (Image frame in frames)
                            {
                                frame.Dispose();
                            }

                            frames.Clear();

                            //lblTimeElapsed.Invoke((MethodInvoker)delegate
                            //{
                            //    lblTimeElapsed.Text = DateTime.Now.Subtract(dtStart).TotalSeconds.ToString("#.##") + " Seconds";
                            //});
                        }
                        catch (Exception ex)
                        {
                            log(ex.Message);
                        }
                    }
                }
            });
        }
        private void LogOnCurrent(Bitmap img)
        {
            PictureBox pb = new PictureBox
            {
                Width = 50,
                Height = 50,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Image = img
            };
            flpLive.Invoke((MethodInvoker)delegate
            {
                flpLive.Controls.Add(pb);
            });
        }
        private void LogOnHistory(Bitmap img)
        {
            PictureBox pb = new PictureBox
            {
                Width = 50,
                Height = 50,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Image = img
            };
            flpHistory.Invoke((MethodInvoker)delegate
            {
                flpHistory.Controls.Add(pb);
            });
        }
        public Bitmap cropAtRect(Bitmap src, Location faceLocation)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    System.Drawing.Rectangle cropRect = new System.Drawing.Rectangle(faceLocation.Left, faceLocation.Top, (faceLocation.Right - faceLocation.Left), (faceLocation.Bottom - faceLocation.Top));
                    Bitmap target = new Bitmap(cropRect.Width + 50, cropRect.Height + 50);
                    using (Graphics g = Graphics.FromImage(target))
                    {
                        g.DrawImage(src, new Rectangle(0, 0, target.Width + 50, target.Height + 50),
                                         cropRect,
                                         GraphicsUnit.Pixel);
                    }
                    return target;
                }
                catch (Exception)
                {
                }
            }
            return null;
        }
        private void Authenticate(FaceRecognition faceRecognition, Location faceLocation, Bitmap img)
        {
            try
            {
                List<KeyValuePair<Image, string>> list = new List<KeyValuePair<Image, string>>();
                string path = Application.StartupPath + "\\Faces\\";
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo item in di.GetFiles())
                {
                    if (!item.Name.EndsWith("Current.jpg"))
                    {
                        list.Add(new KeyValuePair<Image, string>(FaceRecognition.LoadImageFile(item.FullName), item.Name));
                    }
                }

                string pathB = Application.StartupPath + "\\Faces\\Current.jpg";
                Image imageB = FaceRecognition.LoadImageFile(pathB);

                bool isAuthenticated = false;
                string user = string.Empty;
                foreach (KeyValuePair<Image, string> imageA in list)
                {
                    IEnumerable<Location> locationsA = faceRecognition.FaceLocations(imageA.Key);
                    IEnumerable<Location> locationsB = faceRecognition.FaceLocations(imageB);

                    if (locationsA.Any() && locationsB.Any())
                    {
                        IEnumerable<FaceEncoding> encodingA = faceRecognition.FaceEncodings(imageA.Key, locationsA);
                        IEnumerable<FaceEncoding> encodingB = faceRecognition.FaceEncodings(imageB, locationsB);

                        const double tolerance = 0.6d;
                        isAuthenticated = FaceRecognition.CompareFace(encodingA.First(), encodingB.First(), tolerance);

                        foreach (FaceEncoding item in encodingA)
                        {
                            item.Dispose();
                        }
                        foreach (FaceEncoding item in encodingB)
                        {
                            item.Dispose();
                        }
                        if (isAuthenticated)
                        {
                            user = imageA.Value;
                            break;
                        }
                    }
                }
                imageB.Dispose();
                if (isAuthenticated)
                {
                    log("کاربر " + Path.GetFileName(user) + " شناسایی شد");
                    var cropped = cropAtRect(img, faceLocation);
                    LogOnCurrent(cropped);
                }
            }
            catch (Exception ex)
            {
                log(ex.Message);
            }
        }
        private void ShowInForm(Location faceLocation, Bitmap img)
        {
            try
            {
                System.Drawing.Rectangle rectFToFill = new System.Drawing.Rectangle(faceLocation.Left, faceLocation.Top, (faceLocation.Right - faceLocation.Left), (faceLocation.Bottom - faceLocation.Top));
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img))
                {
                    System.Drawing.Color customColor = System.Drawing.Color.FromArgb(100, System.Drawing.Color.Red);
                    System.Drawing.SolidBrush shadowBrush = new System.Drawing.SolidBrush(customColor);
                    System.Drawing.Pen pen = new Pen(customColor, 10);
                    g.DrawRectangle(pen, rectFToFill);
                }
                pbLive.Invoke((MethodInvoker)delegate
                {
                    pbLive.Image = img;
                    //lblFace.Text = "Detected";
                });

            }
            catch (Exception ex)
            {
                log(ex.Message);
            }
        }
        private void log(string message)
        {
            lbLog.Invoke((MethodInvoker)delegate
            {
                lbLog.Items.Insert(0, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + message);
            });
        }
    }
}
