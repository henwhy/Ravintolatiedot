using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp7
{
    public class KirjautumisTiedot
    {
        public string kayttajatunnus;
        public string salasana;
        public string rooli;


        //Kun tätä konstruktoria kutsutaan luodaan olio rivin antamista tiedoista
        public static KirjautumisTiedot FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            KirjautumisTiedot uusiTunnus = new KirjautumisTiedot
            {
                kayttajatunnus = values[0],
                salasana = values[1],
                rooli = values[2]
            };
            return uusiTunnus;
        }


        //Tämä metodi ylikirjoittaa valmiiksi olevan ToString "Metodin?".
        //Metodi palauttaa olion tiedot string muodossa
        public override string ToString()
        {
            return string.Join(";",
                kayttajatunnus,
                salasana,
                rooli);
        }
    }
}
