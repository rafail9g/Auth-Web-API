# Sistem Manajemen Pertanian — REST API

## a) Deskripsi Project

REST API untuk mengelola data pertanian meliputi lahan, tanaman, dan hasil panen. 
Sistem ini memungkinkan pengguna melakukan operasi CRUD pada tiga entitas utama yang saling berelasi: 
data lahan pertanian, tanaman yang ditanam di lahan tersebut, serta catatan hasil panen dari setiap tanaman.
Domain yang dipilih: **Pertanian**



## b) Teknologi yang Digunakan

| Komponen | Teknologi |
|----------|-----------|
Bahasa C# 
Framework ASP.NET Core 8.0 
Database PostgreSQL
Library koneksi DB Npgsql 8.0.3
Dokumentasi API Swagger (Swashbuckle) 
IDE Visual Studio 2022 



## c) Langkah Instalasi dan Cara Menjalankan

### Prasyarat
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [pgAdmin](https://www.pgadmin.org/) (opsional, untuk kelola database secara visual)

### Langkah-langkah
1. **Clone repository ini**
   git clone <url-repository-ini>
   cd "Tugas PAA TM" 

2. **Sesuaikan connection string**

   Buka file `appsettings.json`, ubah bagian berikut sesuai konfigurasi PostgreSQL kamu:
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=pertanian_db;Username=postgres;Password=your_password;"
   }

3. **Import database** (lihat bagian d di bawah)

4. **Jalankan project**
   dotnet run
   Atau tekan **F5** di Visual Studio.

6. Buka Swagger UI di browser:
   https://localhost:7102/swagger
   


d) Cara Import Database

1. Buka **pgAdmin**, login ke server PostgreSQL kamu
2. Klik kanan pada **Databases** → **Create** → **Database**, beri nama `pertanian_db`
3. Klik kanan pada database `pertanian_db` → **Query Tool**
4. Klik ikon **Open File**, pilih file `database.sql` dari folder project ini
5. Klik tombol **Execute / Run (F5)**
6. Pastikan tidak ada error — semua tabel, index, trigger, dan sample data akan terbuat otomatis

Atau via terminal `psql`:
psql -U postgres -d pertanian_db -f database.sql


## e) Daftar Endpoint

### Lahan

| Method | URL | Keterangan |
|--------|-----|------------|
| GET | `/api/lahan` | Ambil semua data lahan |
| GET | `/api/lahan/{id}` | Ambil data lahan berdasarkan ID |
| POST | `/api/lahan` | Tambah lahan baru |
| PUT | `/api/lahan/{id}` | Update data lahan |
| DELETE | `/api/lahan/{id}` | Hapus data lahan |

### Tanaman

| Method | URL | Keterangan |
|--------|-----|------------|
| GET | `/api/tanaman` | Ambil semua data tanaman |
| GET | `/api/tanaman?lahanId={id}` | Filter tanaman berdasarkan lahan |
| GET | `/api/tanaman/{id}` | Ambil data tanaman berdasarkan ID |
| POST | `/api/tanaman` | Tambah tanaman baru |
| PUT | `/api/tanaman/{id}` | Update data tanaman |
| DELETE | `/api/tanaman/{id}` | Hapus data tanaman |

### Panen

| Method | URL | Keterangan |
|--------|-----|------------|
| GET | `/api/panen` | Ambil semua catatan panen |
| GET | `/api/panen?tanamanId={id}` | Filter panen berdasarkan tanaman |
| GET | `/api/panen/{id}` | Ambil catatan panen berdasarkan ID |
| POST | `/api/panen` | Tambah catatan panen baru |
| PUT | `/api/panen/{id}` | Update catatan panen |
| DELETE | `/api/panen/{id}` | Hapus catatan panen |

### Contoh Format Response
Sukses (200 OK):
{
  "success": true,
  "message": "Ditemukan 6 lahan",
  "data": [ ... ]
}

Error (404 Not Found):
{
  "success": false,
  "message": "Lahan dengan id 99 tidak ditemukan",
  "data": null
}


## f) Link Video Presentasi
[Klik di sini untuk menonton video presentasi](https://youtu.be/LINK_VIDEO_KAMU)


## Struktur Project
Tugas PAA TM/
Controllers/
- LahanController.cs
- TanamanController.cs
- PanenController.cs
Models/
- Models.cs
- ApiResponse.cs
Repositories/
- LahanRepository.cs
- TanamanRepository.cs
- PanenRepository.cs
appsettings.json
Program.cs
Tugas PAA TM.csproj
database.sql
README.md
