using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WaveParser
{
    public class Modifier
    {
        private const int FrameSize = 512;
        private short[] _data;
        private long _dataLength;
        private short _minValue;
        private short _maxValue;
        private int _attitude;
        private int _noiseEdge;
        private string _TXTpath;

        private List<short[]> _frames;
        private int[] _frameEnergies;
        private short[] _nullCrosses;

        public Modifier()
        {
            _frames = new List<short[]>();
        }

        public Modifier(short[] data)
        {
            SetData(data);

            _frames = new List<short[]>();
        }

        public void SetPath(string fileName)
        {
            _TXTpath = fileName + "-modified.txt";
        }

        public void SetData(short[] data)
        {
            _data = (short[])data.Clone();
            _dataLength = _data.LongLength;
        }

        //public void Normalise()
        //{
        //    for (long i = 0; i < _dataLength; i++)
        //    {
        //        if (_minValue > _data[i])
        //            _minValue = _data[i];
        //        if (_maxValue < _data[i])
        //            _maxValue = _data[i];
        //    }

        //    _attitude = _maxValue - _minValue;
            
        //    //
        //}

        public void DivideToFrames()
        {
            var dataSize = _data.LongLength;
            var temp = new short[FrameSize];

            for (long i = 0; i < dataSize; i++)
            {
                temp[i%FrameSize] = _data[i];
                if (i%FrameSize == FrameSize - 1)
                {
                    _frames.Add(temp);
                    temp = new short[FrameSize];
                }
            }

            _frameEnergies = new int[_frames.Count];
            _nullCrosses = new short[_frames.Count];
        }

        public void CalculateEnergies()
        {
            if (_frameEnergies == null) return;

            int tempEnergy = 0;

            for (int i = 0; i < _frames.Count; i++)
            {
                short[] frame = _frames[i];
                for (int j = 0; j < FrameSize; j++)
                    tempEnergy += frame[j];

                tempEnergy /= FrameSize;
                _frameEnergies[i] =  (tempEnergy > 0) ? tempEnergy : -tempEnergy;

                tempEnergy = 0;
            }

            _noiseEdge = (_frameEnergies[0] > _frameEnergies[1]) ? _frameEnergies[0]/2 : _frameEnergies[1]/2;
        }

        public void CalculateZeroCrosses()
        {
            if (_frameEnergies == null) return;

            short tempCrosses = 0;

            for (int i = 0; i < _frames.Count; i++)
            {
                short[] frame = _frames[i];
                for (int j = 1; j < FrameSize; j++)
                    if (LessThanNull(frame[j - 1]) != LessThanNull(frame[j]) && Math.Abs(frame[j-1]-frame[j]) > _noiseEdge)
                        tempCrosses++;

                _nullCrosses[i] = tempCrosses;

                tempCrosses = 0;
            }

        }

        public void SaveAsText(string parameterString)
        {
            using (StreamWriter sw = new StreamWriter(_TXTpath))
            {
                string[] parameters = parameterString.Split(',');
                foreach (string param in parameters)
                {
                    switch (param.ToLower())
                    {
                        case "energies":
                        case "energy":
                            sw.WriteLine("Energies:");
                            foreach (var energy in _frameEnergies)
                                sw.WriteLine(energy);
                            break;
                        case "crosses":
                            sw.WriteLine("Null-crosses:");
                            foreach (var cross in _nullCrosses)
                                sw.WriteLine(cross);
                            break;
                    }
                }
            }
        }

        static bool LessThanNull(int value)
        {
            return value < 0;
        }

        static int HzToMel(int hz)
        {
            return (int) (1127*Math.Log(1 + hz/700));
        }

        static int MelToHz(int mel)
        {
            return 700*(int) (Math.Pow(Math.E, mel/1127) - 1);
        }

    }
}
