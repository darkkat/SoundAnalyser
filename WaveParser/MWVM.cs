using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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
