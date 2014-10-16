using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveParser
{
    public class Modifier
    {
        private short[] _data;

        private List<short[]> _frames; 

        public Modifier()
        {}

        public Modifier(short[] data)
        {
            _data = (short[])data.Clone();
            _frames = new List<short[]>();
        }

        public void SetData(short[] data)
        {
            _data = (short[])data.Clone();
        }

        public void DivideToFrames(int number)
        {
            var dataSize = _data.Length;
            var frameSize = dataSize/number;
            var temp = new short[frameSize];

            for (int i = 0; i < number; i++)
            {
                temp[i%frameSize] = _data[i];
                if (i%frameSize == frameSize - 1)
                {
                    _frames.Add(temp);
                    temp = new short[frameSize];
                }
            }
        }

        static int HzToMel(int hz)
        {
            return (int) (1127.01048*Math.Log(1 + hz/700));
        }

        static int MelToHz(int mel)
        {
            return 700*(int) (Math.Pow(Math.E, mel/1127.01048) - 1);
        }
    }
}
