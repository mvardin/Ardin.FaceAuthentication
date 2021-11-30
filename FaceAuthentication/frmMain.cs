using FaceRecognitionDotNet;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Image = FaceRecognitionDotNet.Image;

namespace FaceAuthentication
{
    public partial class frmMain : Form
    {
        public System.Drawing.Image CurrentRawPicture { get; set; }
        public frmMain()
        {
            InitializeComponent();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
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
                pbMain.Invoke((MethodInvoker)delegate
                {
                    pbMain.Image = img;
                    lblFace.Text = "Detected";
                });
            }
            catch (Exception ex)
            {
                log(ex.Message);
            }
        }
        public static object _cropLock { get; set; }
        public Bitmap cropAtRect(Bitmap src, Rectangle cropRect)
        {
            lock (_cropLock)
            {
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
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
            }
            return null;
        }
        private void btnAddFace_Click(object sender, EventArgs e)
        {
            string filename = Application.StartupPath + "\\Faces\\" + txtName.Text + ".jpg";
            if (!File.Exists(filename))
            {
                CurrentRawPicture.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            else
            {
                MessageBox.Show("Face is exist");
            }
        }
        private void Authenticate(FaceRecognition faceRecognition)
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

                lblAuthenticated.Invoke((MethodInvoker)delegate
                {
                    if (isAuthenticated)
                    {
                        lblAuthenticated.Text = "welcome " + Path.GetFileName(user);
                    }
                    else
                    {
                        lblAuthenticated.Text = "Athentication faild";
                    }
                });
            }
            catch (Exception ex)
            {
                log(ex.Message);
            }
        }
        private void log(string message)
        {
            listBox1.Invoke((MethodInvoker)delegate
            {
                listBox1.Items.Insert(0, message);
            });
        }
        private void btnTestFaceLocation_Click(object sender, EventArgs e)
        {
            string directory = Path.GetFullPath("Models");
            FaceRecognition _faceRecognition = FaceRecognition.Create(directory);
            string pathB = @"C:\temp\aa.png";
            Image imageB = FaceRecognition.LoadImageFile(pathB);
            IEnumerable<Location> locations = _faceRecognition.FaceLocations(imageB);

            var img = imageB.ToBitmap();
            foreach (var faceLocation in locations)
            {
                System.Drawing.Rectangle rectFToFill = new System.Drawing.Rectangle(faceLocation.Left, faceLocation.Top, (faceLocation.Right - faceLocation.Left), (faceLocation.Bottom - faceLocation.Top));
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img))
                {
                    System.Drawing.Color customColor = System.Drawing.Color.FromArgb(100, System.Drawing.Color.Red);
                    System.Drawing.SolidBrush shadowBrush = new System.Drawing.SolidBrush(customColor);
                    System.Drawing.Pen pen = new Pen(customColor, 10);
                    g.DrawRectangle(pen, rectFToFill);
                }
            }
            pbMain.Image = img;
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                btnStart.Invoke((MethodInvoker)delegate
                {
                    btnStart.Enabled = false;
                    btnStart.Text = "Working ...";
                });
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
                            pbMain.Invoke((MethodInvoker)delegate
                            {
                                pbMain.Image = img;
                            });

                            if (faceLocations.Any())
                            {
                                foreach (Location faceLocation in faceLocations)
                                {
                                    ShowInForm(faceLocation, img);
                                    Thread.Sleep(100);
                                    Authenticate(_faceRecognition);
                                    Thread.Sleep(100);
                                }
                            }
                            else
                            {
                                lblFace.Invoke((MethodInvoker)delegate
                                {
                                    lblFace.Text = "Not Detected";
                                });
                            }

                            // Clear the frames array to start the next batch
                            foreach (Image frame in frames)
                            {
                                frame.Dispose();
                            }

                            frames.Clear();

                            lblTimeElapsed.Invoke((MethodInvoker)delegate
                            {
                                lblTimeElapsed.Text = DateTime.Now.Subtract(dtStart).TotalSeconds.ToString("#.##") + " Seconds";
                            });
                        }
                        catch (Exception ex)
                        {
                            log(ex.Message);
                        }
                    }
                }
            });

        }
    }
}