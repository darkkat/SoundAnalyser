using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace WaveParser
{
    public class MWVM
    {
        private ICommand LoadFileInternalCommand;

        private MainWindow _window;
        private Loader _loader;
        private short[] _rawdata;

        public MWVM(MainWindow window)
        {
            _window = window;
            _loader = new Loader();

            LoadFileInternalCommand = new DelegateCommand(o =>
                {
                    if (_window.tbUri.Text == "")
            {
                        MessageBox.Show("Enter URI!");
                        return;
                    }
                    _loader.SetPath(_window.tbUri.Text);
                    _loader.LoadFile();
                    _loader.SaveAsText();
                    _rawdata = _loader.RawData;
                });
            DrawLine(0, 0, 100, 100, 1);
        }
        
    public PointCollection GraphicPoints
        {
            get
            {
                return
                    new PointCollection(new[]
                    {
                        new Point(0, 0),            
                        new Point(10, 30), 
                        new Point(20, 20), 
                        new Point(30, 40), 
                        new Point(40, 90),
                        new Point(50, 180),
                        new Point(60, 2),
                        new Point(70, 31),
                        new Point(80, 90),
                        new Point(90, 432),
                        new Point(100, 13),
                        new Point(110, 2),
                    });
            }

        public void DrawLine(int x1, int y1, int x2, int y2, int thickness)
        {
            Line line = new Line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            line.Stroke = Brushes.Black;
            line.StrokeThickness = thickness;
            line.Tag = string.Format("Line {0} {1} {2} {3}", x1, y1, x2, y2);
            _window.Canvas.Children.Add(line);
        }

        public ICommand LoadFile
        {
            get { return LoadFileInternalCommand; }
        }
    }
}
