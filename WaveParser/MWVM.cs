using System.Windows;
using System.Windows.Media;

namespace WaveParser
{
    public class MWVM
    {
        private readonly MainWindow _window;

        public MWVM(MainWindow window)
        {
            _window = window;
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
        }
    }
}
