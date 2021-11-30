using FaceRecognitionDotNet;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Image = FaceRecognitionDotNet.Image;

namespace Ardin.FaceAuthentication
{
    public partial class frmMain : Form
    {
        #region Props
        public Bitmap CurrentRawPicture { get; private set; }
        public bool IsSystemRun { get; set; }
        public List<string> AuthenticatedUserList { get; set; }
        public List<string> AllUserList { get; set; }
        #endregion

        #region Ctor and loader
        public frmMain()
        {
            InitializeComponent();
            IsSystemRun = false;
            AuthenticatedUserList = new List<string>();
            AllUserList = new List<string>();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            StartMonitoring();
        }
        #endregion

        #region Main methods
        public void StartMonitoring()
        {
            Task.Run(() =>
            {
                log("سیستم در حال راه اندازی");
                string directory = Path.GetFullPath("Models");
                if (!Directory.Exists(directory))
                {
                    log($"بررسی کنید ببینید آیا مدل در  '{directory}' وجود دارد یا خیر!");
                }

                FaceRecognition _faceRecognition = FaceRecognition.Create(directory);

                using VideoCapture capture = new(0);
                while (capture.IsOpened())
                {
                    try
                    {
                        DateTime dtStart = DateTime.Now;
                        Image currentFrame;

                        #region Grab a single frame of video
                        using (Mat frame = new())
                        {
                            bool ret = capture.Read(frame);

                            if (!IsSystemRun)
                            {
                                log("سیستم راه اندازی شد");
                            }

                            IsSystemRun = true;

                            // Bail out when the video file ends
                            if (!ret || !frame.IsContinuous())
                            {
                                break;
                            }

                            using Mat tmp = frame.CvtColor(ColorConversionCodes.BGR2RGB);
                            byte[] array = new byte[tmp.Width * tmp.Height * tmp.ElemSize()];
                            Marshal.Copy(tmp.Data, array, 0, array.Length);

                            Image image = FaceRecognition.LoadImage(array, tmp.Rows, tmp.Cols, tmp.Width * tmp.ElemSize(), Mode.Rgb);
                            currentFrame = image;
                        }
                        #endregion

                        //TODO Ardin
                        //currentFrame = FaceRecognition.LoadImageFile(@"C:\temp\Ardin.FaceAuthentication\bin\Debug\net5.0-windows\Faces\sample.jpg");

                        Location[] faceLocations = _faceRecognition.FaceLocations(currentFrame, 0).ToArray();

                        CurrentRawPicture = currentFrame.ToBitmap();
                        pbLive.Invoke((MethodInvoker)delegate
                        {
                            pbLive.Image = currentFrame.ToBitmap();
                        });

                        if (faceLocations.Any())
                        {
                            IEnumerable<Image> croppedImageList = FaceRecognition.CropFaces(currentFrame, faceLocations);
                            LogAll(croppedImageList);
                            foreach (Location faceLocation in faceLocations)
                            {
                                var pbImage = pbLive.Image as Bitmap;
                                Authenticate(_faceRecognition, faceLocation, currentFrame);
                                ShowInForm(faceLocation, pbImage);
                            }
                        }
                        else
                        {
                            //lblFace.Invoke((MethodInvoker)delegate
                            //{
                            //    lblFace.Text = "Not Detected";
                            //});
                        }

                        currentFrame.Dispose();

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
            });
        }
        private void Authenticate(FaceRecognition faceRecognition, Location faceLocation, Image CurrentImage)
        {
            try
            {
                List<KeyValuePair<Image, string>> dataset = new();
                string path = Application.StartupPath + "\\Faces\\";
                DirectoryInfo di = new(path);
                foreach (FileInfo item in di.GetFiles())
                {
                    dataset.Add(new KeyValuePair<Image, string>(FaceRecognition.LoadImageFile(item.FullName), item.Name));
                }

                bool isAuthenticated = false;
                string user = string.Empty;
                foreach (KeyValuePair<Image, string> personImage in dataset)
                {
                    IEnumerable<Location> locationsA = faceRecognition.FaceLocations(personImage.Key, 0);
                    IEnumerable<Location> locationsB = faceRecognition.FaceLocations(CurrentImage, 0);

                    if (locationsA.Any() && locationsB.Any())
                    {
                        IEnumerable<FaceEncoding> encodingA = faceRecognition.FaceEncodings(personImage.Key, locationsA);
                        IEnumerable<FaceEncoding> encodingB = faceRecognition.FaceEncodings(CurrentImage, locationsB);

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
                            user = personImage.Value;
                            break;
                        }
                    }
                }
                if (isAuthenticated)
                {
                    string username = Path.GetFileNameWithoutExtension(user);
                    if (!AuthenticatedUserList.Contains(username))
                    {
                        log("کاربر " + username + " شناسایی شد");

                        var selectedFace = FaceRecognition.CropFaces(CurrentImage, new List<Location>() { faceLocation });
                        LogAuthenticated(selectedFace.FirstOrDefault() , username);
                        AuthenticatedUserList.Add(username);
                    }
                }
                CurrentImage.Dispose();
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
        #endregion

        #region Second level methods
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
        private void LogAll(IEnumerable<Image> faces)
        {
            flpAll.Invoke((MethodInvoker)delegate
            {
                flpAll.Controls.Clear();
            });
            foreach (var face in faces)
            {
                PictureBox pb = new()
                {
                    Width = 50,
                    Height = 50,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Image = face.ToBitmap()
                };
                flpAll.Invoke((MethodInvoker)delegate
                {
                    flpAll.Controls.Add(pb);
                });
            }
        }
        private void LogAuthenticated(Image face, string name)
        {
            PictureBox pb = new()
            {
                Width = 50,
                Height = 50,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Image = face.ToBitmap(),
            };
            pb.Paint += new PaintEventHandler((sender, e) =>
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                e.Graphics.DrawString(name, Font, Brushes.LightGreen, 0, 30);
            });

            flpAuthenticated.Invoke((MethodInvoker)delegate
            {
                flpAuthenticated.Controls.Add(pb);
            });
        }
        #endregion

        #region Tools methods
        private void log(string message)
        {
            lbLog.Invoke((MethodInvoker)delegate
            {
                lbLog.Items.Insert(0, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + message);
            });
        }
        #endregion

        #region move window
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private void frmMain_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion

        #region Buttons
        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion
    }
}
