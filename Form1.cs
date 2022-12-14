using Emgu.CV;
using Emgu.CV.Structure;
using System.Numerics;
using System.Text.RegularExpressions;
using Windows.Media.Effects;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;


namespace CameraRecz
{
    public partial class Form1 : Form
    {
        Emgu.CV.VideoCapture capture = null;
        Emgu.CV.VideoCapture vCapture = null;

        double TotalFrame;
        double Fps;
        int FrameNo;
        

        Mat tempMat = new Mat();
        Mat tempMat1 = new Mat();
        Emgu.CV.Image<Bgr, Byte> imageFrame ;

        private bool captureInProgres = false;

        

        public Form1()
        {
            InitializeComponent();

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        private void startToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if(vCapture!= null)
            {
                vCapture.Dispose();
                vCapture= null;
            }

            if (capture == null )
            {
                capture = new Emgu.CV.VideoCapture(0);
                
            }

            capture.ImageGrabbed += Capture_ImageGrabbed;
            

            capture.Start();
            
        }

        private void VCapture_ImageGrabbed(object? sender, EventArgs e)
        {
           throw new NotImplementedException(); 
        }

        


        private void Capture_ImageGrabbed(object? sender, EventArgs e)
        {
            try
            {   
                
                Mat tempMat = new Mat();
                if (sender != null)
                {
                    ((VideoCapture)sender).Retrieve(tempMat);

                    using (var ms = new MemoryStream(tempMat.ToImage<Bgr, byte>().ToJpegData(95)))
                    {
                        
                        pictureBox1.Image = Image.FromStream(ms);
                        Thread.Sleep((int)((VideoCapture)sender).Get(Emgu.CV.CvEnum.CapProp.Fps));
                    }
                }

                

                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                capture.Stop();
                capture = null;
            
            }
            finally 
            {
                capture = null;
            }
        }

        private void stopToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            try
            {
                if (capture != null)
                {
                    capture.Stop();
                    capture.Dispose();
                }

                //capture = null;
            }
            catch (Exception ex100)
            {
                MessageBox.Show(ex100.Message);
            }
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if(capture != null)
                {
                    capture.Pause();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //capture.Stop();
            }
               
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                    
                    capture.Stop();                    
                    capture.Dispose();                    
                    capture = null;
                    //Application.Exit();
                    
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void startToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Capture_ImageGrabbed1(object? sender, EventArgs e)
        {
            CvInvoke.DestroyAllWindows();
            
            vCapture.Dispose();
            capture.Dispose();
            vCapture = null;
            capture = null;
        }

        private void stopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (vCapture!= null)
            {                
                vCapture.Stop();
                vCapture.Dispose();
                CvInvoke.DestroyAllWindows();
            }
            
                       
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            vCapture = new VideoCapture(0);
            tempMat1= new Mat();    
            tempMat1 = vCapture.QueryFrame();

            using (var ms = new MemoryStream(tempMat1.ToImage<Bgr, byte>().ToJpegData(95)))
            {

                pictureBox1.Image = Image.FromStream(ms);
                Thread.Sleep((int)(vCapture.Get(Emgu.CV.CvEnum.CapProp.Fps)));
            }

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            vCapture = new Emgu.CV.VideoCapture(0);
        }

        //�Ұ�webcam
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //if webcam �S�Ұ�
            if(vCapture ==null)
            {
                try
                {
                    //���}�w�]��webcam
                    vCapture = new VideoCapture(0);
                }
                catch (NullReferenceException ex) 
                {
                    MessageBox.Show(ex.Message);                
                }

            }
            //webcam�Ұ�
            if(vCapture != null)
            {
                //frame �Ұ�
                if(captureInProgres)
                {
                    //Stop the capture
                    captureInProgres = false;
                    toolStripMenuItem1.Text = "�}�l...";
                    Application.Idle -= ProcessFrame;

                }
                else
                {
                    //frame����
                    //start the capture
                    captureInProgres= true;
                    toolStripMenuItem1.Text = "����...";
                    Application.Idle += ProcessFrame;
                }
            }
        }

        //���s�v��
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                vCapture = new VideoCapture();
                if (vCapture == null)
                {
                    MessageBox.Show("Can't find a camera...", "error");

                }
                //Mat mat2 = new Mat();
                
                Mat mat2 = vCapture.QueryFrame();

                Emgu.CV.Image<Bgr, byte> temp;
                
                temp = mat2.ToImage<Bgr, byte>();
                              

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                saveFileDialog.Filter = "Video Files(*.mp4)|*.mp4";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("�}�l���v�A��ESC�������v");

                }

                int fourcc = VideoWriter.Fourcc('H', '2', '6', '4');


                VideoWriter vw = new VideoWriter(
                    saveFileDialog.FileName, fourcc, 30, new Size(640, 480), true);
                
                //Int64 indexFrame = 0;

                Emgu.CV.CvInvoke.NamedWindow("camera0", Emgu.CV.CvEnum.WindowFlags.Normal);


                while (temp != null)
                {
                    CvInvoke.Imshow("camera0", temp);
                    
                    int ch = CvInvoke.WaitKey(20);

                    if (vCapture.IsOpened)
                    {
                        mat2 = vCapture.QueryFrame();
                        temp = mat2.ToImage<Bgr, byte>();
                        vw.Write(temp);

                        if (ch == 27)
                        {
                            break;
                        }

                    }
                        
                }


                vw.Dispose();
                vCapture.Stop();
                vCapture.Dispose();
                temp = null;
                
                CvInvoke.DestroyWindow("camera0");


                //���v���ݱN�v������M�|�X���C

                captureInProgres = false;
                toolStripMenuItem1.Text = "�}�l...";
                Application.Idle -= ProcessFrame;

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                vCapture.Dispose();
                CvInvoke.DestroyAllWindows();
                captureInProgres = false;
                
                capture = null;
                vCapture= null;
                
            }
            







        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            
        }

        private void pauseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            vCapture.Pause();
        }
    }
}