using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

//ÖNGÖRÜ

namespace İmalatMalzemeleri
{
    public partial class Form1 : Form
    {

        SqlConnection connectionString = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);

        private decimal toplamMaliyet = 0;
        private double totalSeconds = 0;

        private Dictionary<string, ComboBox> comboBoxControls;
        private Dictionary<string, ComboBox> sureComboBoxControls;
        private Dictionary<string, ComboBox> digerComboBoxControls;


        private int index = 0;

        public Form1()
        {
            InitializeComponent();

            comboBoxControls = new Dictionary<string, ComboBox>();
            sureComboBoxControls = new Dictionary<string, ComboBox>();
            digerComboBoxControls = new Dictionary<string, ComboBox>();

            ToplamMaliyet.Text = toplamMaliyet.ToString("0.00");
            ToplamSure.Text = totalSeconds.ToString("0 saat 0 dk 0 sn");

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Tab1
            foreach (var control in Adet1GB.Controls)
                if (control is TextBox textBox)
                    textBox.TextChanged += MalzemeFiyatGuncelle;             

            foreach (var control in Birim1GB.Controls)
                 if (control is ComboBox comboBox)
                    comboBox.SelectedIndexChanged += MalzemeFiyatGuncelle;
            //Tab2
            foreach (var control in Adet2GB.Controls)
                if (control is TextBox textBox)
                    textBox.TextChanged += MalzemeFiyatGuncelle;

            foreach (var control in Birim2GB.Controls)
                if (control is ComboBox comboBox)
                    comboBox.SelectedIndexChanged += MalzemeFiyatGuncelle;


            //Saat Dakika ve Saniye hesapla
            foreach (var control in ImalatSuresiGB.Controls)
            {
                if (control is TextBox textBox)
                    textBox.TextChanged += SureGuncelle;
                else if (control is ComboBox comboBox)
                    comboBox.SelectedIndexChanged += SureGuncelle;
            }

            //Diger seçeneklerini toplam malkyiete ekle:
            foreach (var control in DigerGB.Controls)
            {
                if (control is TextBox textBox)
                    textBox.TextChanged += MalzemeFiyatGuncelle;
                else if (control is ComboBox comboBox)
                    comboBox.SelectedIndexChanged += MalzemeFiyatGuncelle;
            }

            ComboboxControls();
            SureComboBoxControls();
            DigerComboBoxControls();

            //İş Emri Numaraları gelsin
            FillIsEmriNoComboBox();

            Diger2.Visible = false;
            Diger3.Visible = false;
            Diger4.Visible = false;
            Diger5.Visible = false;

            Diger2Miktar.Visible = false;
            Diger3Miktar.Visible = false;
            Diger4Miktar.Visible = false;
            Diger5Miktar.Visible = false;

            Diger2Birim.Visible = false;
            Diger3Birim.Visible = false;
            Diger4Birim.Visible = false;
            Diger5Birim.Visible = false;
        }

        #region GetBirimFiyat
        private decimal GetBirimFiyat(string malzemeSutunAdi)
        {
            //Başka tablodan birim fiyat çekme

            decimal fiyat = 0;
            try
            {
                connectionString.Open();

                // Sütun adı doğrudan kullanmak için
                string query = $"SELECT TOP 1 {malzemeSutunAdi} FROM MalzBirimFiyatTB ORDER BY KayitTarihi DESC";
                using (SqlCommand command = new SqlCommand(query, connectionString))
                {
                    object result = command.ExecuteScalar();
                    if (result != null && decimal.TryParse(result.ToString(), out fiyat))
                    {
                        return fiyat;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı sorgulama sırasında bir hata oluştu: " + ex.Message);
            }
            finally
            {
                connectionString.Close();
            }
            return fiyat;
        }
        #endregion

        #region FillIsEmriNoCombobox 
        //İş emri numarası combobox'da gelsin
        private void FillIsEmriNoComboBox()
        {
            try
            {
                connectionString.Open();

                //SQL
                string query = "SELECT İsEmriNo FROM IsTB";

                SqlCommand command = new SqlCommand(query, connectionString);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    IsEmriNo.Items.Add(reader["İsEmriNo"].ToString());
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler akınırken hata oluştu: " + ex.Message);
            }
            finally
            {
                connectionString.Close();
            }
        }
        #endregion

        #region Malzeme Kontrolleri

        #region ComboboxControls
        private void ComboboxControls()
        {
            comboBoxControls = new Dictionary<string, ComboBox>
            {
                {"İmalatCeligi", İmalatCeligiBirim },
                {"TakimCeligi", TakimCeligiBirim },
                {"Pirinc", PirincBirim },
                {"Stavax", StavaxBirim },
                {"Aluminyum", AluminyumBirim },
                {"PaslanmazCelik", PaslanmazCelikBirim },
                {"PoliamidKestamit", PoliamidKestamitBirim },
                {"Bakir", BakirBirim },
                {"Tungsten", TungstenBirim },
                {"ApmcoYerli", ApmcoYerliBirim },
                {"AmpcoIthal", AmpcoIthalBirim},
                {"Vulkolon", VulkolonBirim },
                {"Elastomer", ElastomerBirim },
                {"AluKosebent", AluKosebentBirim },
                {"AluProf45x45", AluProf45x45Birim },
                {"AluProf30x30", AluProf30x30Birim },
                {"AluProf25x20", AluProf25x20Birim },
                {"DemirProf50x50", DemirProf50x50Birim },
                {"DemirProf40x40", DemirProf40x40Birim },
                {"DemirProf30x30", DemirProf30x30Birim },
                {"DemirProf25x25", DemirProf25x25Birim },
                {"DemirKose50x50", DemirKose50x50Birim },
                {"DemirKose30x30", DemirKose30x30Birim },
                {"SilmeDemir3x30", SilmeDemir3x30Birim },
                {"SilmeDemir5x50", SilmeDemir5x50Birim },
                {"SilmeDemir10x20", SilmeDemir10x20Birim },
                {"SilmeDemir10x30", SilmeDemir10x30Birim },
                {"MDF8x1830x1830", MDF8x1830x1830Birim },
                {"MDF18x1830x1830", MDF18x1830x1830Birim },
                {"Polycarbonsolid4x1000x2000", Polycarbonsolid4x1000x2000Birim },
                {"DemirSacDelikli", DemirSacDelikliBirim },
                {"DemirSacDuz", DemirSacDuzBirim }
            };

        }
        #endregion

        #region ConvertBirimFiyat
        private decimal ConvertBirimFiyat(decimal fiyat, string birim)
        {
            switch (birim.ToLower())
            {
                case "mg":
                    return fiyat / 1000000;
                case "gr":
                    return fiyat / 1000;
                case "ton":
                    return fiyat * 1000;
                default:
                    return fiyat; // kg veya varsayılan
            }
        }
        #endregion

        #region GetComboBoxBirim
        private ComboBox GetComboBoxBirim(string textBoxName)
        {
            //Combobox isimleri malzemenin textbox ismi + Birim kelimesi eklenmiş hali
            string comboBoxName = textBoxName + "Birim";

            //return comboBoxControls.ContainsKey(comboBoxName) ? comboBoxControls[comboBoxName] : null;
            return this.Controls.Find(comboBoxName, true).FirstOrDefault() as ComboBox;
        }
        #endregion

        #region Birim + Miktar
        private string GetCombinedMalz(string key)
        {
            TextBox textBox = this.Controls.Find(key, true).FirstOrDefault() as TextBox;
            if (textBox == null || !comboBoxControls.ContainsKey(key))
            {
                return null;
            }

            ComboBox comboBox = comboBoxControls[key];
            if (comboBox != null && !string.IsNullOrWhiteSpace(textBox.Text) && comboBox.SelectedItem != null)
            {
                return $"{textBox.Text} {comboBox.SelectedItem.ToString()}";
            }
            return null;
        }
        #endregion

        #region Malzeme Fiyat Güncellemes:
        private void MalzemeFiyatGuncelle(object sender, EventArgs e)
        {
            try
            {
                toplamMaliyet = 0;

                var textControls = new List<TextBox>
                {
                    İmalatCeligi, TakimCeligi, Pirinc, Stavax, Aluminyum, PaslanmazCelik,
                    PoliamidKestamit, Bakir, Tungsten, ApmcoYerli, AmpcoIthal, Vulkolon,
                    Elastomer, AluKosebent, AluProf45x45, AluProf30x30, AluProf25x20,
                    DemirProf50x50, DemirProf40x40, DemirProf30x30, DemirProf25x25,
                    DemirKose50x50, DemirKose30x30, SilmeDemir3x30, SilmeDemir5x50,
                    SilmeDemir10x20, SilmeDemir10x30, MDF8x1830x1830, MDF18x1830x1830,
                    Polycarbonsolid4x1000x2000, DemirSacDelikli, DemirSacDuz
                };

                foreach (var textBox in textControls)
                {
                    if (!string.IsNullOrWhiteSpace(textBox.Text) && decimal.TryParse(textBox.Text, out decimal adet))
                    {
                        //Combobox'dan birim seçelim
                        string birim = GetComboBoxBirim(textBox.Name)?.SelectedItem?.ToString() ?? "kg";

                        //Diğer tablodan birim fiyatı çekelim:
                        decimal fiyat = GetBirimFiyat(textBox.Name);
                        //Seçilen birime göre fiyatı dönüştürelim:
                        decimal birimFiyat = ConvertBirimFiyat(fiyat, birim);

                        //Adet ile çarpıp o malzemenin maliyetini hesaplayalım:
                        decimal MalzemeninTotalFiyati = adet * birimFiyat;

                        //Alttaki toplam maliyet için toplamMaliyete ekleyelim
                        toplamMaliyet += MalzemeninTotalFiyati;
                    }
                }

                // "Diğer" malzemelerin maliyet hesaplaması
                for (int i = 1; i <= 5; i++)
                {
                    string digerTextBoxName = $"Diger{i}";
                    string digerMiktarTextBoxName = $"Diger{i}Miktar";
                    string digerBirimComboBoxName = $"Diger{i}Birim";

                    var digerTextBox = this.Controls.Find(digerTextBoxName, true).FirstOrDefault() as TextBox;
                    var digerMiktarTextBox = this.Controls.Find(digerMiktarTextBoxName, true).FirstOrDefault() as TextBox;
                    var digerComboBox = this.Controls.Find(digerBirimComboBoxName, true).FirstOrDefault() as ComboBox;

                    if (digerTextBox != null && digerMiktarTextBox != null && digerComboBox != null)
                    {
                        if (!string.IsNullOrWhiteSpace(digerMiktarTextBox.Text) && decimal.TryParse(digerMiktarTextBox.Text, out decimal adet))
                        {
                            string birim = digerComboBox.SelectedItem?.ToString() ?? "kg";
                            decimal fiyat = GetBirimFiyat(digerTextBoxName);
                            decimal birimFiyat = ConvertBirimFiyat(fiyat, birim);
                            decimal MalzemeninTotalFiyati = adet * birimFiyat;
                            toplamMaliyet += MalzemeninTotalFiyati;
                        }
                    }
                }

                ToplamMaliyet.Text = toplamMaliyet.ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hesaplama sırasında bir hata oluştu: " + ex.Message);
            }
        }
        #endregion

        #endregion

        #region Süre Kontrolleri

        #region SureComboboxControls
        private void SureComboBoxControls()
        {
            sureComboBoxControls = new Dictionary<string, ComboBox>
            {
                {"TasarimEB", TasarimEBBirim },
                {"Printer", PrinterBirim },
                {"Testere", TestereBirim },
                {"Freze", FrezeBirim },
                {"CNCFreze", CNCFrezeBirim },
                {"Torna", TornaBirim },
                {"CNCTorna", CNCTornaBirim },
                {"HizliDelme", HizliDelmeBirim },
                {"TelErozyon", TelErozyonBirim },
                {"DalmaErozyon", DalmaErozyonBirim },
                {"MaktapKlavuz", MaktapKlavuzBirim },
                {"Taslama", TaslamaBirim },
                {"Tesviye", TesviyeBirim },
                {"Montaj", MontajBirim },
                {"Kaynak", KaynakBirim },
                {"Bukme", BukmeBirim },
                {"DaireTestereBileme", DaireTestereBilemeBirim }
            };
        }

        #endregion

        #region ConvertSure
        private double ConvertSure(double timeValue, string unit)
        {
            double totalSeconds = 0;

            switch (unit.ToLower())
            {
                case "saat":
                    totalSeconds = timeValue * 3600;
                    break;
                case "dk":
                    totalSeconds = timeValue * 60;
                    break;
                case "sn":
                    totalSeconds = timeValue;
                    break;
                default:
                    MessageBox.Show($"Bilinmeyen birim: {unit}");
                    break;
            }
            return totalSeconds;
        }

        #endregion

        #region GetComboBoxBirimSure
        private ComboBox GetComboBoxBirimSure(string textBoxName)
        {
            string comboBoxName = textBoxName + "Birim";
            return this.Controls.Find(comboBoxName, true).FirstOrDefault() as ComboBox;
        }
        #endregion

        #region Sure + Miktar
        private string GetCombinedValue(string key)
        {
            TextBox textBox = this.Controls.Find(key, true).FirstOrDefault() as TextBox;
            if (textBox == null || !sureComboBoxControls.ContainsKey(key))
            {
                return null;
            }

            ComboBox comboBox = sureComboBoxControls[key];
            if (comboBox != null && !string.IsNullOrWhiteSpace(textBox.Text) && comboBox.SelectedItem != null)
            {
                return $"{textBox.Text} {comboBox.SelectedItem.ToString()}";
            }
            return null;
        }
        #endregion

        #region SureGuncelle
        private void SureGuncelle(object sender, EventArgs e)
        {
            try
            {
                double toplamSure = 0;

                var textControls2 = new List<TextBox>
                {
                    TasarimEB, Printer, Testere, Freze, CNCFreze, Torna, CNCTorna,
                    HizliDelme, TelErozyon, DalmaErozyon, MaktapKlavuz, Taslama, Tesviye, Montaj, Kaynak, Bukme, DaireTestereBileme
                };

                foreach (var textBox2 in textControls2)
                {
                    if (!string.IsNullOrWhiteSpace(textBox2.Text) && double.TryParse(textBox2.Text, out double sure))
                    {
                        string birim = GetComboBoxBirimSure(textBox2.Name)?.SelectedItem?.ToString() ?? "saat";
                        double sureConvert = ConvertSure(sure, birim);

                        toplamSure += sureConvert; // Toplam süreyi güncelle
                    }
                }

                // Toplam süreyi saat, dakika, saniye olarak hesapla
                int hours = (int)(toplamSure / 3600);
                int minutes = (int)((toplamSure % 3600) / 60);
                int seconds = (int)(toplamSure % 60);

                string totalTime = $"{hours} saat {minutes} dk {seconds} sn";

                // Sonucu `ToplamSure` adlı `TextBox`'a göster
                TextBox toplamSureTextBox = this.Controls.Find("ToplamSure", true).FirstOrDefault() as TextBox;
                if (toplamSureTextBox != null)
                {
                    toplamSureTextBox.Text = totalTime;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        #endregion

        #endregion

        #region Diğer Kontrolleri

        #region digerComboBoxControls
        private void DigerComboBoxControls()
        {
            digerComboBoxControls = new Dictionary<string, ComboBox>
            {
                {"Diger1",Diger1Birim },
                {"Diger2",Diger2Birim },
                {"Diger3",Diger3Birim },
                {"Diger4",Diger4Birim },
                {"Diger5",Diger5Birim }
            };
        }
        #endregion

        //ConvertDiger --> Burada ConvertBirimFiyat fonksiyonu kullanılır

        #region GetComboBoxBirimDiger
        private ComboBox GetComboBoxBirimDiger(string textBoxName)
        {
            //Combobox isimleri malzemenin textbox ismi + Birim kelimesi eklenmiş hali
            string comboBoxName = textBoxName + "Birim";

            //return comboBoxControls.ContainsKey(comboBoxName) ? comboBoxControls[comboBoxName] : null;
            return this.Controls.Find(comboBoxName, true).FirstOrDefault() as ComboBox;
        }
        #endregion

        #region Diger + Birimler
        private string GetCombinedDiger(string key)
        {

            TextBox miktarTextBox = this.Controls.Find(key + "Miktar", true).FirstOrDefault() as TextBox;
            ComboBox birimComboBox = this.Controls.Find(key + "Birim", true).FirstOrDefault() as ComboBox;

            if (miktarTextBox == null)
            {
                MessageBox.Show($"Miktar TextBox with key '{key}Miktar' not found.");
                return null;
            }

            if (birimComboBox == null)
            {
                MessageBox.Show($"ComboBox with key '{key}Birim' not found.");
                return null;
            }

            if (!string.IsNullOrWhiteSpace(miktarTextBox.Text) && birimComboBox.SelectedItem != null)
            {
                return $"{miktarTextBox.Text} {birimComboBox.SelectedItem.ToString()}";
            }
            return null;
        }
        #endregion

        #endregion

        #region Madde ekle
        private void ArtirBtn_Click(object sender, EventArgs e)
        {
            index++;

            switch (index)
            {
                case 1:
                    Diger2.Visible = true;
                    Diger2Miktar.Visible = true;
                    Diger2Birim.Visible = true;
                    break;
                case 2:
                    Diger3.Visible = true;
                    Diger3Miktar.Visible = true;
                    Diger3Birim.Visible = true;
                    break;
                case 3:
                    Diger4.Visible = true;
                    Diger4Miktar.Visible = true;
                    Diger4Birim.Visible = true;
                    break;
                case 4:
                    Diger5.Visible = true;
                    Diger5Miktar.Visible = true;
                    Diger5Birim.Visible = true;
                    break;
                default:
                    MessageBox.Show("En fazla 5 malzeme ekleyebilirsiniz.");
                    index = 5; // Index'i maksimum değerde tut
                    break;
            }
        }
        #endregion

        #region Madde Çıkar
        private void Eksilt_Click(object sender, EventArgs e)
        {

            if (index > 0)
            {
                switch (index)
                {
                    case 1:
                        Diger2.Visible = false;
                        Diger2Miktar.Visible = false;
                        Diger2Birim.Visible = false;
                        break;
                    case 2:
                        Diger3.Visible = false;
                        Diger3Miktar.Visible = false;
                        Diger3Birim.Visible = false;
                        break;
                    case 3:
                        Diger4.Visible = false;
                        Diger4Miktar.Visible = false;
                        Diger4Birim.Visible = false;
                        break;
                    case 4:
                        Diger5.Visible = false;
                        Diger5Miktar.Visible = false;
                        Diger5Birim.Visible = false;
                        break;
                }

                index--;
            }
            else
            {
                MessageBox.Show("Silinecek madde kalmadı.");
            }
        }
        #endregion

        #region SQL kaydet (TRY-CATCH)
        private void KaydetBtn_Click(object sender, System.EventArgs e)
        {
            try
            {
                connectionString.Open();

                string query = @"
                                INSERT INTO ImalatMalz_OngoruTB
                                (İmalatCeligi, TakimCeligi, Pirinc, Stavax, Aluminyum, PaslanmazCelik, PoliamidKestamit, Bakir, Tungsten, ApmcoYerli, AmpcoIthal, Vulkolon, Elastomer, AluKosebent, AluProf45x45, AluProf30x30, AluProf25x20, DemirProf50x50, DemirProf40x40, DemirProf30x30, DemirProf25x25, DemirKose50x50, DemirKose30x30, SilmeDemir3x30, SilmeDemir5x50, SilmeDemir10x20, SilmeDemir10x30, MDF8x1830x1830, MDF18x1830x1830, Polycarbonsolid4x1000x2000, DemirSacDelikli, DemirSacDuz, ImalatSorumlusuID, IsEmriNo, KayitTarihi, ToplamMaliyet, Diger1, Diger1_Miktar, Diger2, Diger2_Miktar, Diger3, Diger3_Miktar, Diger4, Diger4_Miktar, Diger5, Diger5_Miktar)
                                VALUES 
                                (@İmalatCeligi, @TakimCeligi, @Pirinc, @Stavax, @Aluminyum, @PaslanmazCelik, @PoliamidKestamit, @Bakir, @Tungsten, @ApmcoYerli, @AmpcoIthal, @Vulkolon, @Elastomer, @AluKosebent, @AluProf45x45, @AluProf30x30, @AluProf25x20, @DemirProf50x50, @DemirProf40x40, @DemirProf30x30, @DemirProf25x25, @DemirKose50x50, @DemirKose30x30, @SilmeDemir3x30, @SilmeDemir5x50, @SilmeDemir10x20, @SilmeDemir10x30, @MDF8x1830x1830, @MDF18x1830x1830, @Polycarbonsolid4x1000x2000, @DemirSacDelikli, @DemirSacDuz, @ImalatSorumlusuID, @IsEmriNo, GETDATE(), @ToplamMaliyet, @Diger1, @Diger1_Miktar, @Diger2, @Diger2_Miktar, @Diger3, @Diger3_Miktar, @Diger4, @Diger4_Miktar, @Diger5, @Diger5_Miktar)";

                SqlCommand command = new SqlCommand(query, connectionString);

                foreach (var item in comboBoxControls)
                {
                    string combinedValue = GetCombinedMalz(item.Key);
                    command.Parameters.AddWithValue("@" + item.Key, string.IsNullOrWhiteSpace(combinedValue) ? DBNull.Value : (object)combinedValue);
                }

                command.Parameters.AddWithValue("@ImalatSorumlusuID" , string.IsNullOrWhiteSpace(ImalatSorumlusuID.Text) ? DBNull.Value : (object)ImalatSorumlusuID.Text);
                command.Parameters.AddWithValue("@IsEmriNo", string.IsNullOrWhiteSpace(IsEmriNo.Text) ? DBNull.Value : (object)IsEmriNo.Text);
                command.Parameters.AddWithValue("@ToplamMaliyet", string.IsNullOrWhiteSpace(ToplamMaliyet.Text) ? DBNull.Value : (object)ToplamMaliyet.Text);

                // Diğer malzemeler için ekleme
                for (int i = 1; i <= 5; i++)
                {
                    string digerKey = $"Diger{i}";
                    string combinedDiger = GetCombinedDiger(digerKey);

                    // Miktar + Birim birleşimini veritabanının DigerX_Miktar sütununa kaydet
                    command.Parameters.AddWithValue($"@{digerKey}_Miktar", string.IsNullOrWhiteSpace(combinedDiger) ? (object)DBNull.Value : combinedDiger);

                    // DigerX TextBox değerini DigerX sütununa kaydet
                    var digerTextBox = this.Controls.Find(digerKey, true).FirstOrDefault() as TextBox;
                    command.Parameters.AddWithValue($"@{digerKey}", string.IsNullOrWhiteSpace(digerTextBox?.Text) ? (object)DBNull.Value : digerTextBox.Text);
                }

                command.ExecuteNonQuery();
                
                //İş Süresini Veritabanına kaydetmek için sorgu
                string query2 = @"INSERT INTO Ongoru_Sure
                                         (KayitTarihi, IsEmriNo, ImalatSorumlusuID, TasarimEB, Printer, Testere, Freze, CNCFreze, Torna, CNCTorna,
                                         HizliDelme, TelErozyon, DalmaErozyon, MaktapKlavuz, Taslama, Tesviye, Montaj, Kaynak, Bukme, DaireTestereBileme, ToplamSure)
                                
                                VALUES 
                                        (GETDATE(), @IsEmriNo, @ImalatSorumlusuID, @TasarimEB, @Printer, @Testere, @Freze, @CNCFreze, @Torna, @CNCTorna,
                                         @HizliDelme, @TelErozyon, @DalmaErozyon, @MaktapKlavuz, @Taslama, @Tesviye, @Montaj, @Kaynak, @Bukme, @DaireTestereBileme, @ToplamSure)";

                SqlCommand command2 = new SqlCommand(query2, connectionString);

                command2.Parameters.AddWithValue("@IsEmriNo", string.IsNullOrWhiteSpace(IsEmriNo.Text) ? DBNull.Value : (object)IsEmriNo.Text);
                command2.Parameters.AddWithValue("@ImalatSorumlusuID", string.IsNullOrWhiteSpace(ImalatSorumlusuID.Text) ? DBNull.Value : (object)ImalatSorumlusuID.Text);

                //Süre ve Miktarlar birleşip --> veritabanına
                foreach (var item in sureComboBoxControls)
                {
                    string combinedValue = GetCombinedValue(item.Key);
                    command2.Parameters.AddWithValue("@" + item.Key, string.IsNullOrWhiteSpace(combinedValue) ? DBNull.Value : (object)combinedValue);
                }

                command2.Parameters.AddWithValue("@ToplamSure", string.IsNullOrWhiteSpace(ToplamSure.Text) ? DBNull.Value : (object)ToplamSure.Text);

                command2.ExecuteNonQuery();

                MessageBox.Show("Tüm veriler kaydedildi!");

                connectionString.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }
        #endregion  -----

    }
}
