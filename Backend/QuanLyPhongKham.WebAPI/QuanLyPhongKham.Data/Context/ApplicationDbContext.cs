using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public DbSet<Khoa> Khoas { get; set; }
        public DbSet<DichVu> DichVus { get; set; }
        public DbSet<BenhNhan> BenhNhans { get; set; }
        public DbSet<BacSi> BacSis { get; set; }
        public DbSet<LichKham> LichKhams { get; set; }
        public DbSet<DanhGiaDichVu> DanhGiaDichVus { get; set; }
        public DbSet<KetQuaKham> KetQuaKhams { get; set; }
        public ApplicationDbContext Context { get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BenhNhan>()
                .HasOne(b => b.User) // Liên kết đến ApplicationUser
                .WithOne() // Một BenhNhan chỉ có một ApplicationUser
                .HasForeignKey<BenhNhan>(b => b.UserId) // Sử dụng UserId làm khóa ngoại
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Khoa>()
                .HasKey(b => b.KhoaId);
            modelBuilder.Entity<Khoa>()
                .HasMany(b => b.DichVus)
                .WithOne(k => k.Khoa)
                .HasForeignKey(k => k.KhoaId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Khoa>()
                .HasMany(b => b.BacSis)
                .WithOne(k => k.Khoa)
                .HasForeignKey(k => k.KhoaId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DanhGiaDichVu>()
                .HasKey(d => d.DanhGiaId);
            //modelBuilder.Entity<DanhGiaDichVu>()
            //    .HasOne<BenhNhan>()
            //    .WithOne()
            //    .HasForeignKey<BenhNhan>(b => b.BenhNhanId)
            //    .OnDelete(DeleteBehavior.Restrict);

            // Mối quan hệ giữa BenhNhan và LichKham
            modelBuilder.Entity<BenhNhan>()
                .HasMany(b => b.LichKhams)
                .WithOne(l => l.BenhNhan)
                .HasForeignKey(l => l.BenhNhanId)
                .OnDelete(DeleteBehavior.Cascade); // Đổi thành Restrict

 

            // Mối quan hệ giữa BacSi và LichKham
            modelBuilder.Entity<BacSi>()
                .HasMany(b => b.LichKhams)
                .WithOne(l => l.BacSi)
                .HasForeignKey(l => l.BacSiId)
                .OnDelete(DeleteBehavior.Cascade);

            // Mối quan hệ giữa LichKham và KetQuaKham
            modelBuilder.Entity<LichKham>()
                .HasOne(l => l.KetQuaKham)
                .WithOne(kk => kk.LichKham)
                .HasForeignKey<KetQuaKham>(kk => kk.LichKhamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DichVu>()
                .Property(d => d.DonGia)
                .HasColumnType("decimal(18,2)");

            // Mối quan hệ giữa BenhNhan và DanhGiaDichVu
            modelBuilder.Entity<BenhNhan>()
                .HasMany(b => b.DanhGiaDichVus)
                .WithOne(d => d.BenhNhan)
                .HasForeignKey(d => d.BenhNhanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Mối quan hệ giữa BacSi và DanhGiaDichVu
            modelBuilder.Entity<BacSi>()
                .HasMany(b => b.DanhGiaDichVus)
                .WithOne(d => d.BacSi)
                .HasForeignKey(d => d.BacSiId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình cho mối quan hệ nhiều-nhiều giữa BacSi và DichVu thông qua BacSiDichVu
            modelBuilder.Entity<BacSiDichVu>()
                .HasKey(bsdv => new { bsdv.BacSiId, bsdv.DichVuId }); // Khóa chính là sự kết hợp giữa BacSiId và DichVuId

            modelBuilder.Entity<BacSiDichVu>()
                .HasOne(bsdv => bsdv.BacSi)
                .WithMany(b => b.BacSiDichVus) // Một BacSi có nhiều BacSiDichVu
                .HasForeignKey(bsdv => bsdv.BacSiId); // Khóa ngoại đến BacSiId

            modelBuilder.Entity<BacSiDichVu>()
                .HasOne(bsdv => bsdv.DichVu)
                .WithMany(d => d.BacSiDichVus) // Một DichVu có nhiều BacSiDichVu
                .HasForeignKey(bsdv => bsdv.DichVuId); // Khóa ngoại đến DichVuId
        }
       }
}
