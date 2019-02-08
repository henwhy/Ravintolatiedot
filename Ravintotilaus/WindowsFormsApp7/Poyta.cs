namespace WindowsFormsApp7
{
    public class Poyta
    {
        public string poytanro;
        public string tuolimaara;
        public string varattu;

        //Kun tätä konstruktoria kutsutaan luodaan olio rivin antamista tiedoista
        public static Poyta FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            Poyta uusiPoyta = new Poyta();
            uusiPoyta.poytanro = values[0];
            uusiPoyta.tuolimaara = values[1];
            uusiPoyta.varattu = values[2];
            return uusiPoyta;
        }
        
        //Tämä metodi ylikirjoittaa valmiiksi olevan ToString "Metodin?".
        //Metodi palauttaa olion tiedot string muodossa
        public override string ToString()
        {
            return string.Join(";",
                poytanro,
                tuolimaara,
                varattu);
        }
    }
}