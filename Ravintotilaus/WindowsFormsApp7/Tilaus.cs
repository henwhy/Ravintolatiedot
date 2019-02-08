namespace WindowsFormsApp7
{
    public class TilausRivi
    {
        public string tilausnro;
        public string poytanro;
        public string tilattutuote;
        public string tilattumaara;
        public string tilausrivinhinta;
        public string tilauksentila;
        public string tiedostorivi;



        //Kun tätä konstruktoria kutsutaan luodaan olio rivin antamista tiedoista
        public static TilausRivi FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            TilausRivi uusiTilausRivi = new TilausRivi
            {
                tilausnro = values[0],
                poytanro = values[1],
                tilattutuote = values[2],
                tilattumaara = values[3],
                tilausrivinhinta = values[4],
                tilauksentila = values[5],
                tiedostorivi = values[6]
            };
            return uusiTilausRivi;
        }

        //Tämä metodi ylikirjoittaa valmiiksi olevan ToString "Metodin?".
        //Metodi palauttaa olion tiedot string muodossa
        public override string ToString()
        {
            return string.Join(";",
                tilausnro,
                poytanro,
                tilattutuote,
                tilattumaara,
                tilausrivinhinta,
                tilauksentila,
                tiedostorivi);
        }
    }

}