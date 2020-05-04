using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace VisualHEX.Models
{
    public class ZonaHEX
    {
        internal Records Records { get; set; }
        internal uint AddressHI { get; set; }
        internal uint AddressLO { get; set; }
        
        public ulong Start
        {
            get
            {
                return (ulong)((AddressHI << 16) + AddressLO);
            }
        }

        public ulong End
        {
            get
            {
                return (ulong)(Start + Len - 1);
            }
        }
        public int RecordCount
        {
            get
            {
                return Records.Count;
            }
        }

        public uint Len { get; set; }
        public string StartHEX { get { return String.Format("{00000000:X}", Start); } }
        public string EndHEX { get { return String.Format("{00000000:X}", End); } }
        public string LenHEX { get { return String.Format("{0000:X}", Len); } }

        public ZonaHEX() { }

        public override string ToString()
        {
            return $"From {StartHEX} To {EndHEX},  {LenHEX} bytes, {Records.Count} record";
        }
    }

    public class ZoneHEX : List<ZonaHEX>
    {

        public ulong Start { get { return Min.Start; } }
        public ulong End { get { return Max.End; } }
        public ZoneHEX(){}

        public ZoneHEX( string nomeFile ) {

            Records tutti = new Records(nomeFile);
            Records parziali = new Records();

            int idx = -1;
            uint HI = 0;
            uint LO = 0;
            uint calculatedNext = 0;
            bool eIlPrimoData = true;

            do
            {
                idx++;
                RecordHEX r = tutti[idx];
                parziali.Add(r);

                if (r.Tipo == RecordType.ExtLinearAddr)
                {
                    // Dal primo record ricavo la parte alta dell'indirizzo a 32bit
                    HI = (uint)((r.Dati[0] << 8) + (r.Dati[1]));
                    eIlPrimoData = true;
                }
                else if (r.Tipo == RecordType.Data)
                {
                    if (eIlPrimoData)
                    {
                        // OK mi ricavo il LO 
                        LO = r.Address;

                        // Il prossimo Address è la somma dell'offset iniziale
                        // + la lunghezza 
                        calculatedNext = LO;
                        eIlPrimoData = false;
                    }

                    // Se il prossimo address è diverso dal previsto
                    if (tutti[idx + 1].Address != calculatedNext + r.Count)
                    {
                        uint len = calculatedNext - LO + r.Count;
                        Add(new ZonaHEX { AddressHI = HI, AddressLO = LO, Len = len, Records=parziali });
                        parziali = new Records();
                        eIlPrimoData = true;
                    }
                    else
                    {
                        calculatedNext += r.Count;
                    }

                }

            } while (idx < tutti.Count - 1);

        }

        public ZonaHEX Max
        {
            get
            {
                var max = this.Max(s => s.Len);
                var t = this.Where(s => s.Len == max);
                return t.First();
            }
        }
        public ZonaHEX Min
        {
            get
            {
                var min = this.Min(s => s.Len);
                var t = this.Where(s => s.Len == min);
                return t.First();
            }
        }
    }
}
