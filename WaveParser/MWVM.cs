using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Input;
using Microsoft.Win32;
using WaveParser.Annotations;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using Point = System.Windows.Point;
using System.Windows.Media.Imaging;

namespace WaveParser
{
    public class MWVM : INotifyPropertyChanged
    {
        private readonly ICommand LoadFileInternalCommand;

        private MainWindow _window;
        private Loader _loader;
        private short[] _rawdata;
        private float[] _normalized;
        private Bitmap _graphic;
        private BitmapImage _graphicImage;

        public MWVM(MainWindow window)
        {
            _window = window;
            _loader = new Loader();

            LoadFileInternalCommand = new DelegateCommand(o =>
            {
                var opener = new OpenFileDialog();
                opener.Filter = @"Wave Sound (*.wav,*.wave)|*.wav;*.wave|All files(*.*)|*.*";
                opener.FileOk += (sender, args) =>
                {
                    _loader.SetPath(opener.FileName);
                    _loader.LoadFile();
                    _rawdata = _loader.RawData;
                    _normalized = _loader.NormalizedRawData;
                    Graphic = DrawNormalizedAudio(_normalized, Color.Green, 640, 480);
                    GraphicImage = BitmapToBitmapImage(Graphic);
                    OnPropertyChanged("GraphicPoints");
                };
                opener.ShowDialog();
            });
        }

        public Bitmap Graphic
        {
            get { return _graphic; }
            set
            {
                _graphic = value;
                OnPropertyChanged("Graphic");
            }
        }
        public BitmapImage GraphicImage
        {
            get { return _graphicImage; }
            set
            {
                _graphicImage = value;
                OnPropertyChanged("GraphicImage");
            }
        }

        public PointCollection GraphicPoints
        {
            get
            {
                var points = new PointCollection();
                if (_rawdata == null) return points;
                for (int i = 0; i < _rawdata.Length/10; i+=10)
                    points.Add(new Point(i, _rawdata[i]));
                return points;
            }
        }
        public ICommand LoadFile
        {
            get { return LoadFileInternalCommand; }
        }
        
        private Bitmap DrawNormalizedAudio(float[] data, Color color, int width, int height)
        {
            var bmp = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                Pen pen = new Pen(color);
                int size = data.Length;
                for (int iPixel = 0; iPixel < width; iPixel++)
                {
                    // determine start and end points within WAV
                    int start = (int)((float)iPixel * ((float)size / (float)width));
                    int end = (int)((float)(iPixel + 1) * ((float)size / (float)width));
                    float min = float.MaxValue;
                    float max = float.MinValue;
                    for (int i = start; i < end; i++)
                    {
                        float val = data[i];
                        min = val < min ? val : min;
                        max = val > max ? val : max;
                    }
                    int yMax = height - (int)((max + 1) * .5 * height);
                    int yMin = height - (int)((min + 1) * .5 * height);
                    g.DrawLine(pen, iPixel, yMax, iPixel, yMin);
                }
            }
            return bmp;
        }
        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage;
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
