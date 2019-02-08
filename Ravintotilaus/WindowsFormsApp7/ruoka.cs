using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp7
{

    public class Ruoka
    {
        public string tuote;
        public string hinta;
        public string valmiiksitarjottavissa;

        //Kun tätä konstruktoria kutsutaan luodaan olio rivin antamista tiedoista
        public static Ruoka FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            Ruoka uusiRuoka = new Ruoka
            {
                tuote = values[0],
                hinta = values[1],
                valmiiksitarjottavissa = values[2]
            };
            return uusiRuoka;
        }

    }
}
