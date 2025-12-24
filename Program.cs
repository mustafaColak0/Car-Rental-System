
    using System;
    using System.Collections.Generic; //List ve Dictionary için gerekli
    using System.IO; //Dosya işlemleri için gerekli
    using System.Linq; //Koleksiyon işlemleri için gerekli


namespace ARAC_KİRALAMA_SİSTEMİ
{

    // --- VERİ MODELLERİ (CLASSLAR) ---
    class Rezervasyon
    {
       // = "" diyerek varsayılan değer atıyoruz ki hata almayalım.
        public string? MusteriAdi { get; set; } = "";
        public string? AracPlaka { get; set; } = ""; 
        public DateTime Baslangic;
        public DateTime Bitis;
        public double ToplamUcret;
    }

    // Araç özelliklerini tutan sınıf.
    class Arac
    {
        public string marka;
        public string model;
        public int yil;
        public string renk;
        public double gunlukUcret;
        public string plaka { get; set; }

        // Constructor (Yapıcı Metot): Yeni bir 'Arac' oluşturulduğunda özellikleri kolayca girmemizi sağlar.
        public Arac(string marka, string model, string plaka, int yil, string renk, double gunlukUcret)
        {
            this.marka = marka;
            this.model = model;
            this.plaka = plaka;
            this.yil = yil;
            this.renk = renk;
            this.gunlukUcret = gunlukUcret;
        }
    }

    class Program
    {
        // Verilerin kaydedileceği dosyanın adı
        static string dosyaYolu = "rezervasyonlar.txt";

        // Sistemdeki sabit araç listesi (Veritabanı gibi davranır)
        static List<Arac> araclar = new List<Arac>()
    {
        new Arac ("Toyota", "Corolla","APT7641", 2020, "Beyaz", 1500),
        new Arac("Honda", "Civic", "34SRT21",2019, "Siyah", 3500),
        new Arac("Ford", "Focus","34STU07" ,2021, "Kirmizi", 2500),
        new Arac("Range" , "Rover","34ABC10", 2022, "Mavi", 6000),
        new Arac ( "BMW" , "320i","01ADN01",2022,"Mavi" , 5500 ),
        new Arac ( "Audi" , "A6","34IZM05",2022,"Siyah" , 4000 ),
        new Arac ( "Mercedes" , "AMG", "46RSP14",2022,"Gri" , 5000 ),
    };
        // Yapılan rezervasyonları bellekte (RAM) tutan liste
        static List<Rezervasyon> rezervasyonlar = new List<Rezervasyon>();

        static void Main(string[] args)
        {
            // Program başlarken, önceki kayıtları dosyadan belleğe yüklüyoruz.
            VerileriYukle();

            // Sonsuz döngü: Kullanıcı 0 ı tuşlayana kadar menü ekranında kalır.
            while (true)
            {
                Console.Clear();
                Console.WriteLine("====Araç Kiralama Sistemi===");
                Console.WriteLine("1.Müsait araçları listele");
                Console.WriteLine("2.Yeni Rezervasyon yap");
                Console.WriteLine("3.Rezervasyon iptal et");
                Console.WriteLine("(Rapor):4.Toplam Gelir");
                Console.WriteLine("(Rapor):5.Müşteri Rezervasyonları");
                Console.WriteLine("(Rapor):6.En Çok kiralanan araç");
                Console.WriteLine("7.Tüm Rezervasyonları Sıfırla");
                Console.WriteLine("(0. Cikis)");
                Console.Write("Seciminizi yapin: ");

                // Kullanıcıdan aldığımız seçim boş olursa string kabul ediyoruz (?? operatörü).
                string secim = Console.ReadLine() ?? "";

                // Seçime göre ilgili metoda yönlendiriyoruz.
                switch (secim)
                {
                    case "1":
                        MusaitAraclariListeleEkrani();
                        break;
                    case "2":
                        RezervasyonYapEkrani();
                        break;
                    case "3":
                        RezervasyonİptalEtEkrani();
                        break;
                    case "4":

                        Console.WriteLine($"\n Toplam Gelir: {ToplamGelir()} TL");
                        Console.ReadKey();
                        break;
                    case "5":
                        MusteriRezervasyonlariEkrani();
                        break;
                    case "6":
                        Console.WriteLine($"\n En Popüler Araç: {EnCokKiralananArac()}");
                        Bekle();
                        break;

                    case "7":
                        TumRezervasyonlariSifirla();
                         break;
                    case "0":
                        Environment.Exit(0); // Programı tamamen kapatır.
                        break;
                    default:
                        Console.WriteLine("Gecersiz secim. Tekrar deneyin.");
                        
                        Bekle(); break;
                }
            }
        }
        // Kullanıcı bir tuşa basana kadar programı bekleten metot
        static void Bekle()
        {
            Console.WriteLine("\nDevam etmek için bir tuşa basın...");
            Console.ReadKey();
        }

        // --- DOSYA YÖNETİMİ VE SIFIRLAMA ---

        // Tüm verileri hem listeden hem de dosyadan siler
        static void TumRezervasyonlariSifirla()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkRed; // Dikkat çekmek için kırmızı arka plan
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n DİKKAT: TÜM VERİLERİ SİLMEK ÜZERESİNİZ! ");
            Console.ResetColor();

            Console.WriteLine("Kayıtlı tüm rezervasyonlar kalıcı olarak silinecek.");
            Console.Write("Onaylıyor musunuz? (Evet için 'E' yazın): ");

            string onay = Console.ReadLine() ?? "";

            if (onay.ToUpper() == "E")
            {
               
                rezervasyonlar.Clear(); // bellekteki listeyi temizler


                if (File.Exists(dosyaYolu))
                {
                    File.Delete(dosyaYolu); 
                }

                Console.WriteLine("\n Başarılı: Tüm rezervasyonlar ve dosya sıfırlandı.");
            }
            else
            {
                Console.WriteLine("\n İşlem iptal edildi. Veriler silinmedi.");
            }
            Bekle();
        }


        // Rezervasyon listesini dosyaya kaydeder
        static void VerileriKaydet()
        {
            try
            {

                // StreamWriter ile dosyayı yazma işleminde kullanıyoruz.
                // 'using' bloğu, işlem bitince dosyayı otomatik kapatır.
                using (StreamWriter sw = new StreamWriter(dosyaYolu)) 
                {
                    foreach (var rezervasyon in rezervasyonlar)
                    {
                        // Verileri aralarına '|' koyarak tek bir string haline getirir
                        string kayit = $"{rezervasyon.AracPlaka}|{rezervasyon.MusteriAdi}|{rezervasyon.Baslangic}|{rezervasyon.Bitis}|{rezervasyon.ToplamUcret}";
                        sw.WriteLine(kayit); // Dosyaya yaz
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kayıt hatası: {ex.Message}");
            }
        }
        // Program açıldığında dosyadaki verileri okuyup listeye geri yükler
        static void VerileriYukle()
        {

            try

            {
                if (!File.Exists(dosyaYolu)) return; // Eğer dosya henüz oluşmamışsa işlem yapma (ilk çalıştırılışta hata vermemesi için)
                string[] satirlar = File.ReadAllLines(dosyaYolu); // Dosyadaki tüm satırları oku

                foreach (string satir in satirlar)
                {

                    {
                        string[] secenek = satir.Split('|'); // '|' işaretinden bölerek parçalara ayır

                        if (secenek.Length == 5) // Eğer satırda beklenen 5 veri parçası varsa nesneye çevir
                        {
                            Rezervasyon eskiRezervasyon = new Rezervasyon
                            {
                                AracPlaka = secenek[0],
                                MusteriAdi = secenek[1],
                                Baslangic = DateTime.Parse(secenek[2]), // String'i DateTime'a çevir
                                Bitis = DateTime.Parse(secenek[3]),
                                ToplamUcret = double.Parse(secenek[4]) // String'i double'a çevir
                            };
                            rezervasyonlar.Add(eskiRezervasyon);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Yükleme hatası: {ex.Message}");
                Bekle();
            }
        }

        // --- MANTIK VE KONTROL METOTLARI ---


        // Belirtilen tarihlerde aracın müsait olup olmadığını kontrol eder
        static bool AracMusaitMi(string aracPlaka, DateTime baslangic, DateTime bitis)
        {
            for (int i = 0; i < rezervasyonlar.Count; i++)
            {
                Rezervasyon rezervasyon = rezervasyonlar[i];

                if (rezervasyon.AracPlaka == aracPlaka)
                {

                    // Tarihlerin çakışmaması için başlangıç ve bitiş tarihlerini kontrol eder

                    if (baslangic < rezervasyon.Bitis && bitis > rezervasyon.Baslangic)
                    {
                        return false; // Çakışma var, araç müsait değil
                    }
                }
            }
            return true; // Hiçbir çakışma yok, araç müsait
        }


        // Plakası verilen aracın günlük ücretini bulur
        static double AracGunlukUcretGetir(string aracPlaka)
        {
            foreach (var arac in araclar)
            {
                // Hem böylece "34 ABC 123" ile "34ABC123" aynı kabul edilir.
                // ToUpper() ile büyük/küçük harf duyarlılığını kaldırıyoruz.
                if (arac.plaka.Replace(" ", "").ToUpper() == aracPlaka.Replace(" ", "").ToUpper())
                        return arac.gunlukUcret;
            }
            return 0; // Araç bulunamazsa 0 döner
        }

        // Gün sayısı * Günlük Ücret işlemini yapar
        static double RezervasyonUcretHesapla(string aracPlaka, DateTime baslangic, DateTime bitis)
        {
            double gunlukUcret = AracGunlukUcretGetir(aracPlaka);
            // İki tarih arasındaki farkı alıp gün sayısını (.Days) buluyoruz

            int kiralamaGunu = (bitis - baslangic).Days;

            if (kiralamaGunu <= 0) return 0; // Tarihler hatalıysa ücreti 0 olarak yazar
            return gunlukUcret * kiralamaGunu;
        }


        // Rezervasyon oluşturma ve kaydetme işlemi
        static void RezervasyonYap(string musteri, string plaka, DateTime baslangic, DateTime bitis)
        {
            if (AracMusaitMi(plaka, baslangic, bitis))
            {
                double toplamUcret = RezervasyonUcretHesapla(plaka, baslangic, bitis);

                Rezervasyon yeniRezervasyon = new Rezervasyon();
                yeniRezervasyon.MusteriAdi = musteri;
                yeniRezervasyon.AracPlaka = plaka;
                yeniRezervasyon.Baslangic = baslangic;
                yeniRezervasyon.Bitis = bitis;
                yeniRezervasyon.ToplamUcret = toplamUcret;


                rezervasyonlar.Add(yeniRezervasyon); // yeni rezervasyonu listeye ekle
                VerileriKaydet(); // Dosyaya kaydet

                Console.WriteLine($"\n Rezervasyon başarılı! Toplam Ücret: {toplamUcret} TL");
            }
            else
            {
                Console.WriteLine("\n Seçilen araç bu tarihler arasında müsait değildir.");
            }
        }

        static void RezervasyonIptalEt(string aracPlaka, string musteriAdi)
        {
            Rezervasyon? rezervasyonToRemove = null;

            // Silinecek rezervasyonu listede arıyoruz

            foreach (var rezervasyon in rezervasyonlar)
            {
                if (rezervasyon.MusteriAdi == musteriAdi && rezervasyon.AracPlaka == aracPlaka)
                {
                    rezervasyonToRemove = rezervasyon; // silinecek rezervasyonu bulduk
                    break;
                }
            }
            if (rezervasyonToRemove != null) // Eğer rezervasyon bulunduysa sil
            {
                rezervasyonlar.Remove(rezervasyonToRemove); // rezervasyonu listeden sil
                VerileriKaydet(); // Dosyayı güncelle (silineni dosyadan da kaldırır)

                Console.WriteLine("\n Rezervasyon iptal edildi.");
            }
            else
            {
                Console.WriteLine("\n Rezervasyon bulunamadı.");
            }
        }

        // --- RAPORLAMA METOTLARI ---
        static double ToplamGelir()
        {
            double toplam = 0;
            // Listedeki tüm rezervasyonların ücretlerini topluyoruz
            foreach (var rezervasyon in rezervasyonlar)
            {
                toplam += rezervasyon.ToplamUcret;
            }
            return toplam;
        }

        static List<string> MusteriRezervasyonlari(string musteriAdi)
        {
            List<string> liste = new List<string>();

            // Sadece ismi eşleşen rezervasyonları seçiyoruz
            foreach (var rezervasyon in rezervasyonlar)
            {
                if (rezervasyon.MusteriAdi == musteriAdi)
                {
                    liste.Add($"- Araç Plakası: {rezervasyon.AracPlaka}, Başlangıç: {rezervasyon.Baslangic.ToShortDateString()}, Bitiş: {rezervasyon.Bitis.ToShortDateString()}, Toplam Ücret: {rezervasyon.ToplamUcret} TL");
                }
            }
            return liste;
        }

        static string EnCokKiralananArac()
        {
            // Dictionary kullanarak hangi plakanın kaç kere kiralandığını sayıyoruz.
            // Key (Anahtar) = Plaka, Value (Değer) = Sayı
            Dictionary<string, int> aracKiralamaSayilari = new Dictionary<string, int>();

            foreach (var rezervasyon in rezervasyonlar)
            {

                if (string.IsNullOrEmpty(rezervasyon.AracPlaka)) continue;

                if (aracKiralamaSayilari.ContainsKey(rezervasyon.AracPlaka))
                {
                    aracKiralamaSayilari[rezervasyon.AracPlaka]++; // Varsa sayıyı artır
                }
                else
                {
                    aracKiralamaSayilari[rezervasyon.AracPlaka] = 1; // Yoksa ilk kez ekle
                }

            }
            // En yüksek sayıyı bulma algoritması

            string? enCokKiralananArac = null;
            int maxKiralama = 0;

            foreach (var arac in aracKiralamaSayilari)
            {
                if (arac.Value > maxKiralama)
                {
                    maxKiralama = arac.Value;
                    enCokKiralananArac = arac.Key;
                }
            }
            return enCokKiralananArac ?? "Henüz araç kiralanmamış.";
        }


        // --- EKRAN / ARAYÜZ METOTLARI ---
        static void MusaitAraclariListeleEkrani()
        {
            Console.WriteLine("\n ŞU AN MÜSAİT OLAN ARAÇLAR (Anlık Durum):");
            Console.WriteLine("------------------------------------------------");

            bool aracVar = false;
            DateTime suan = DateTime.Now;

            // 'suan'ı hem başlangıç hem bitiş olarak gönderip anlık kontrol yapıyoruz
            foreach (var arac in araclar)
            {

                bool musait = AracMusaitMi(arac.plaka, suan, suan);

                if (musait)
                {
                    Console.WriteLine($"-  {arac.marka} {arac.model} [{arac.plaka}] ({arac.yil})  {arac.renk}  {arac.gunlukUcret} TL");
                    aracVar = true;
                }
            }

            if (!aracVar)
            {
                Console.WriteLine(" Şu anda garajda boş araç yok, hepsi kirada.");
            }
            Console.WriteLine("\n Ana Menüye dönmek için bir tuşa basın...");

            Console.ReadKey();
        }

        static void RezervasyonYapEkrani()
        {
            try
            {
                Console.Write("\nMüşteri Adı: ");
                string musteriAdi = Console.ReadLine() ?? "";

                Console.Write("Araç Plakası: ");
                // Kullanıcı " 34 ab c 12 " girse bile bunu "34ABC12" yapar.
                string aracPlaka = (Console.ReadLine() ?? "").Replace(" ", "").ToUpper();
                
                if (AracGunlukUcretGetir(aracPlaka) == 0)
                {
                    Console.WriteLine(" HATA: Bu plakaya sahip bir araç bulunamadı.");
                    return;  // Metodu sonlandır
                }

                Console.Write("Kiralama Başlangıç Tarihi (yyyy-MM-dd): ");
                DateTime baslangic = DateTime.Parse(Console.ReadLine() ?? "");

                Console.Write("Kiralama Bitiş Tarihi (yyyy-MM-dd): ");
                DateTime bitis = DateTime.Parse(Console.ReadLine() ?? "");

                RezervasyonYap(musteriAdi, aracPlaka, baslangic, bitis);
            }
            catch (Exception)
            {
                // kullanıcı tarih formatını yanlış girdiğinde hata vermemesi için genel bir hata uyarısı verdik
                Console.WriteLine("Hatalı tarih girdin! Lütfen formatı düzgün yaz (Örn: 2023-12-01).");
            }
            Console.ReadKey();
        }

        static void RezervasyonİptalEtEkrani()
        {
            Console.Write("\n Müşteri Adı: ");
            string musteriAdi = Console.ReadLine() ?? "";

            Console.Write("İptal edilecek aracın plakası: ");
            string aracPlaka = Console.ReadLine() ?? "";

            RezervasyonIptalEt(aracPlaka, musteriAdi);
            Console.ReadKey();
        }

        static void MusteriRezervasyonlariEkrani()
        {

            Console.Write("\n Müşteri Adı: ");
            string musteriAdi = Console.ReadLine() ?? "";

            var liste = MusteriRezervasyonlari(musteriAdi);

            Console.WriteLine($"\n--- {musteriAdi} İçin Kayıtlar ---");

            if (liste.Count == 0) Console.WriteLine("Kayıt bulunamadı.");

            foreach (var item in liste)
            {
                Console.WriteLine(item);
            }

            Console.ReadKey();
        }
    }
}








