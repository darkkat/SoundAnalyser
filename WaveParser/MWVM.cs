using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using WaveParser.Annotations;

namespace WaveParser
{
    public class MWVM : INotifyPropertyChanged
    {
        private ICommand LoadFileInternalCommand;

        private MainWindow _window;
        private Loader _loader;
        private Modifier _modifier;
        private short[] _rawdata;

        public MWVM(MainWindow window)
        {
            _window = window;
            _loader = new Loader();
            _modifier = new Modifier();

            LoadFileInternalCommand = new DelegateCommand(o =>
                {
                    var opener = new OpenFileDialog();
                    opener.Filter = @"Wave Sound (*.wav,*.wave)|*.wav;*.wave|All files(*.*)|*.*";
                    opener.FileOk += (sender, args) =>
                        {
                            _loader.SetPath(opener.FileName);
                            _loader.LoadFile();
                            _loader.SaveAsText();
                            _rawdata = _loader.RawData;

                            _modifier.SetPath(opener.FileName);
                            _modifier.SetData(_rawdata);
                            _modifier.DivideToFrames();
                            _modifier.CalculateEnergies();
                            _modifier.CalculateZeroCrosses();
                            _modifier.SaveAsText("energies,crosses");

                            OnPropertyChanged("GraphicPoints");
                        };
                    opener.ShowDialog();
                });
        }

        public PointCollection GraphicPoints
        {
            get
            {
                 var points = new PointCollection();
                if (_rawdata == null) return points;
                for (int i = 0; i < _rawdata.Length/10; i+=10)
                    points.Add(new Point(i, _rawdata[i]/300));
                return points;
            }
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
