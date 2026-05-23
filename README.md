# 🎮 Mobile GTA Demo Roadmap & Project

Bu depo, **iPhone 15 Pro Max** sensörleri (LiDAR + Kamera) kullanılarak taranan gerçek dünya bölgelerini bir araya getirerek Unity motorunda geliştirilecek olan **GTA 3 tarzı üçüncü şahıs mobil demo oyununun** kaynak kodlarını ve yol haritasını barındırır.

---

## 🚀 Proje Hedefleri

- **Gerçekçi Harita:** iPhone 15 Pro Max LiDAR sensörü ve fotogrametri (Polycam/Scaniverse) aracılığıyla taranmış 3D sokak ve bina modelleri.
- **GTA 3 Tarzı Mekanikler:** Üçüncü şahıs (TPS) kamera bakış açısı, araçlara binebilme ve sürebilme, basit envanter sistemi ve bölge satın alma mekanikleri.
- **Mobil Odaklı Geliştirme:** Unity URP (Universal Render Pipeline) kullanılarak düşük donanımlı mobil cihazlarda bile akıcı çalışabilen 30 FPS hedefli optimizasyon.
- **Sıfır Bütçe:** Tamamen ücretsiz ve açık kaynaklı araçlar (Blender, Mixamo, Polycam free vb.) kullanılarak geliştirme.

---

## 🗺️ Proje Yol Haritası (Geliştirme Aşamaları)

### 📈 Faz 1: Proje Kurulumu & Yapı (Hafta 1-2)
- Unity URP projesinin oluşturulması ve mobil girdi (Input System) paketlerinin ayarlanması.
- Temel kod yapısı ve `GameManager`, `EventSystem`, `SaveSystem` gibi çekirdek sınıfların iskeletlerinin yazılması.

### 📡 Faz 2: LiDAR & Fotogrametri ile Harita Üretimi (Hafta 3-6)
- iPhone 15 Pro Max ile dış mekan ve binaların taranması.
- Blender yardımıyla taranan modellerin temizlenmesi, polygon sayısının azaltılması (Decimate modifier), UV Unwrap yapılması ve LOD (Level of Detail) seviyelerinin oluşturulması.

### 🎮 Faz 3: Temel Oyun Mekanikleri (Hafta 7-14)
- **Karakter ve Kamera:** Mixamo animasyonlu yaya hareketi ve orbital TPS kamera.
- **Araç Sistemi:** Unity WheelCollider tabanlı arcade tarzı araç sürüş fiziği ve araca binme/inme etkileşimleri.
- **Envanter ve Bölge Sistemi:** ScriptableObject tabanlı envanter slotları ve harita bölgelerini para karşılığı satın alma tetikleyicileri.

### 🎨 Faz 4: UI/HUD & Touch Kontroller (Hafta 15-16)
- Mobil cihazlar için ekrana yerleştirilen sanal Joystick ve durumsal butonlar (Bin, İn, Zıpla, Silah Seç).
- Sağlık, zırh, para ve mini-map (radar) içeren klasik GTA 3 HUD tasarımı.

### 🚦 Faz 5: Yaya & Trafik Yapay Zekası (Hafta 17-19)
- Waypoint tabanlı basit yaya yürüme yapay zekası ve spline takip eden araç trafiği.

### 🔊 Faz 6: Ses, Efektler & Derin Optimizasyon (Hafta 20-22)
- Motor sesleri, adım sesleri ve ortam ambiyansları.
- Mobil cihazlar için draw-call azaltma, dynamic batching ve Occlusion Culling optimizasyonları.

### 🏁 Faz 7: Entegrasyon & Demo Build (Hafta 23-26)
- Tüm modüllerin birleştirilmesi, JSON tabanlı Save/Load sisteminin testi ve cihaz üzerinde (iOS/Android) ilk oynanabilir demo build alımı.

---

## 🛠️ Kullanılan Teknolojiler

- **Oyun Motoru:** Unity (URP)
- **3D Modelleme & Optimizasyon:** Blender / MeshLab
- **Tarama Araçları:** Polycam / Scaniverse (iPhone 15 Pro Max)
- **Karakter & Animasyon:** Adobe Mixamo
- **Programlama Dili:** C#

---

## 📁 Klasör Yapısı

Proje dosyaları oluşturuldukça `Assets/_Project/` altında aşağıdaki düzende organize edilecektir:
- `Scripts/`: Kod dosyaları (Core, Player, Vehicle, Inventory, Zone vb.)
- `Prefabs/`: Hazır oyun nesneleri
- `ScannedMeshes/`: iPhone ile taranmış 3D harita verileri
- `Materials/` & `Textures/`: Görsel kaplamalar
- `Scenes/`: Menü ve oyun sahneleri
