using System;
using System.IO;
using System.Linq;
using System.Text;

namespace WaveParser
{
    public class Loader
    {
        private string _WAVpath;
        private string _TXTpath;

        public Loader()
        {
            //SetPath("C:\\Users\\darkkat\\Downloads\\part.wav");
        }

        public short[] RawData { get; private set; }

        // http://stackoverflow.com/questions/1215326/open-source-c-sharp-code-to-present-wave-form
        public float[] NormalizedRawData //временно, сделаем по-людскее как-нить
        {
            get
            {
                float max = RawData.Max();
                float[] normalized = new float[RawData.Length];
                for (int i = 0; i < RawData.Length; i++)
                    normalized[i] = RawData[i]/max;

                return normalized;
            }
        }
        public Loader(string path)
        {
            SetPath(path);
        }

        public void SetPath(string path)
        {
            _WAVpath = path;
            _TXTpath = path + ".txt";
        }

        public void LoadFile()
        {
            BinaryReader br = new BinaryReader(File.OpenRead(_WAVpath), Encoding.ASCII);

            string comp;

            br.BaseStream.Seek(0L, SeekOrigin.Begin);

            comp = new string(br.ReadChars(4)).ToUpper();
            if (comp != "RIFF")
                throw new InvalidOperationException();

            br.ReadInt32();

            comp = new string(br.ReadChars(4)).ToUpper();
            if (comp != "WAVE")
                throw new InvalidOperationException();

            comp = new string(br.ReadChars(4)).ToUpper();
            if (comp != "FMT ")
                throw new InvalidOperationException();

            br.ReadInt32();

            short FormatTag = br.ReadInt16();
            short Channels = br.ReadInt16();
            int SamplesPerSec = br.ReadInt32();
            int AvgBytesPerSec = br.ReadInt32();
            short BlockAlign = br.ReadInt16();
            short BitsPerSample = br.ReadInt16();
            int BytesPerSample = BitsPerSample/8;

            if(Channels > 2)
                throw new NotSupportedException("Too much channels!");

            comp = new string(br.ReadChars(4)).ToUpper();
            while (comp != "DATA")
            {
                char temp = br.ReadChar();
                if(temp == 'd')
                    comp = (temp + new string(br.ReadChars(3))).ToUpper();
            }

            if (comp != "DATA")
                throw new InvalidOperationException();

            int num = br.ReadInt32();
            RawData = new short[num / 2];
            long SumCO = RawData.LongLength;
            byte[] buffer = new byte[BytesPerSample];

            if (Channels == 1)
            {
                for (long i = 0; i < SumCO; i++)
                {
                    for (int j = 0; j < BytesPerSample; j++)
                        buffer[j] = br.ReadByte();
                    RawData[i] = BitConverter.ToInt16(buffer, 0);
                }

            }
            else 
            {
                long limit = SumCO/2;
                for (long i = 0; i < limit; i++)
                {
                    //left
                    for (int j = 0; j < BytesPerSample; j++)
                        buffer[j] = br.ReadByte();
                    RawData[i] = BitConverter.ToInt16(buffer, 0);
                    //right
                    for (int j = 0; j < BytesPerSample; j++)
                        buffer[j] = br.ReadByte();
                    RawData[i+limit] = BitConverter.ToInt16(buffer, 0);
                }
            }
        }

        public void SaveAsText()
        {
            using (StreamWriter sw = new StreamWriter(File.Open(_TXTpath, FileMode.Create)))
                for (int i = 0; i < RawData.Length; i++)
                    sw.WriteLine(RawData[i]);
        }
    }
}


