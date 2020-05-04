using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace VisualHEX.Models
{
    public class RecordHEX
    {
        internal string _riga { get; set; }
        public RecordHEX() { }
        public RecordHEX(string riga)
            : this()
        {
            _riga = riga;
        }

        public override string ToString()
        {
            return $"{Count}-{Tipo}-{String.Format("{0:X}", CheckSum)}";
        }
        public bool VerifyStart()
        {
            return _riga[0] == ':';
        }

        public RecordType Tipo
        {
            get
            {
                string strTipo = _riga.Substring(7, 2);
                RecordType t;
                Enum.TryParse(strTipo, true, out t);
                return t;
            }
        }

        // Indexer per accedere ai vari byte della riga, in modo 
        // da poter generare il checksum
        public byte this[int idx]
        {
            get
            {
                // I due caratteri vanno interpretati come esadecimali durante il parse...
                string strVal = _riga.Substring(idx*2 + 1, 2);
                Byte.TryParse(strVal, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b);
                return b;
            }
        }

        public byte Count
        {
            get { return this[0]; }
        }

        public byte CheckSum
        {
            get {
                byte retVal = 0;
                for (int i = 0; i < Count + 4; i++)
                    retVal += this[i];

                return (byte)(~retVal+1);
            }
        }

        public byte[] Dati
        {
            get
            {
                byte[] retVal = new byte[Count];
                for (int idx = 0; idx < Count; idx++)
                    retVal[idx] = this[idx + 4];
                return retVal;
            }
        }
        public string DatiHEX
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int idx = Count-1; idx >= 0; idx--)
                    sb.Append( $"{string.Format("{0:X2}",  this[idx + 4])}, ");
                return sb.ToString();
            }
        }
        public uint Address
        {
            get
            {
                return (uint)((this[1] << 8) + (this[2]));
            }
        }
        public string AddressHEX
        {
            get
            {
                return $"{string.Format("{0:X4}", Address)}";
            }
        }
    }
    public class Records : List<RecordHEX>
    {
        public Records() { }
        public Records(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            while (!sr.EndOfStream)
            {
                Add(new RecordHEX(sr.ReadLine()));
            }
        }
        public override string ToString()
        {
            return Count.ToString();
        }

    }


}
