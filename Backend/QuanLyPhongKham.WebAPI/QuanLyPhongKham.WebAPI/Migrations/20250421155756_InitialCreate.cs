using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyPhongKham.WebAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Khoas",
                columns: table => new
                {
                    KhoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaKhoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenKhoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Khoas", x => x.KhoaId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BenhNhans",
                columns: table => new
                {
                    BenhNhanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaBenhNhan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoaiGioiTinh = table.Column<int>(type: "int", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TienSuBenhLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenhNhans", x => x.BenhNhanId);
                    table.ForeignKey(
                        name: "FK_BenhNhans_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BacSis",
                columns: table => new
                {
                    BacSiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KhoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaBacSi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BangCap = table.Column<int>(type: "int", nullable: true),
                    SoNamKinhNghiem = table.Column<int>(type: "int", nullable: true),
                    GioLamViec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacSis", x => x.BacSiId);
                    table.ForeignKey(
                        name: "FK_BacSis_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BacSis_Khoas_KhoaId",
                        column: x => x.KhoaId,
                        principalTable: "Khoas",
                        principalColumn: "KhoaId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DichVus",
                columns: table => new
                {
                    DichVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KhoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaDichVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenDichVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTaDichVu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVus", x => x.DichVuId);
                    table.ForeignKey(
                        name: "FK_DichVus_Khoas_KhoaId",
                        column: x => x.KhoaId,
                        principalTable: "Khoas",
                        principalColumn: "KhoaId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LichKhams",
                columns: table => new
                {
                    LichKhamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BenhNhanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BacSiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayKham = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioKham = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiLichKham = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DichVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichKhams", x => x.LichKhamId);
                    table.ForeignKey(
                        name: "FK_LichKhams_BacSis_BacSiId",
                        column: x => x.BacSiId,
                        principalTable: "BacSis",
                        principalColumn: "BacSiId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichKhams_BenhNhans_BenhNhanId",
                        column: x => x.BenhNhanId,
                        principalTable: "BenhNhans",
                        principalColumn: "BenhNhanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BacSiDichVu",
                columns: table => new
                {
                    BacSiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DichVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacSiDichVu", x => new { x.BacSiId, x.DichVuId });
                    table.ForeignKey(
                        name: "FK_BacSiDichVu_BacSis_BacSiId",
                        column: x => x.BacSiId,
                        principalTable: "BacSis",
                        principalColumn: "BacSiId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BacSiDichVu_DichVus_DichVuId",
                        column: x => x.DichVuId,
                        principalTable: "DichVus",
                        principalColumn: "DichVuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhGiaDichVus",
                columns: table => new
                {
                    DanhGiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BenhNhanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BacSiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LichKhamId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DanhGia = table.Column<int>(type: "int", nullable: true),
                    PhanHoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGiaDichVus", x => x.DanhGiaId);
                    table.ForeignKey(
                        name: "FK_DanhGiaDichVus_BacSis_BacSiId",
                        column: x => x.BacSiId,
                        principalTable: "BacSis",
                        principalColumn: "BacSiId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanhGiaDichVus_BenhNhans_BenhNhanId",
                        column: x => x.BenhNhanId,
                        principalTable: "BenhNhans",
                        principalColumn: "BenhNhanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanhGiaDichVus_LichKhams_LichKhamId",
                        column: x => x.LichKhamId,
                        principalTable: "LichKhams",
                        principalColumn: "LichKhamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaKhams",
                columns: table => new
                {
                    KetQuaKhamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LichKhamId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChanDoan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChiDinhThuoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaKhams", x => x.KetQuaKhamId);
                    table.ForeignKey(
                        name: "FK_KetQuaKhams_LichKhams_LichKhamId",
                        column: x => x.LichKhamId,
                        principalTable: "LichKhams",
                        principalColumn: "LichKhamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BacSiDichVu_DichVuId",
                table: "BacSiDichVu",
                column: "DichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_BacSis_KhoaId",
                table: "BacSis",
                column: "KhoaId");

            migrationBuilder.CreateIndex(
                name: "IX_BacSis_UserId",
                table: "BacSis",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BenhNhans_UserId",
                table: "BenhNhans",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaDichVus_BacSiId",
                table: "DanhGiaDichVus",
                column: "BacSiId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaDichVus_BenhNhanId",
                table: "DanhGiaDichVus",
                column: "BenhNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaDichVus_LichKhamId",
                table: "DanhGiaDichVus",
                column: "LichKhamId",
                unique: true,
                filter: "[LichKhamId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DichVus_KhoaId",
                table: "DichVus",
                column: "KhoaId");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaKhams_LichKhamId",
                table: "KetQuaKhams",
                column: "LichKhamId",
                unique: true,
                filter: "[LichKhamId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LichKhams_BacSiId",
                table: "LichKhams",
                column: "BacSiId");

            migrationBuilder.CreateIndex(
                name: "IX_LichKhams_BenhNhanId",
                table: "LichKhams",
                column: "BenhNhanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BacSiDichVu");

            migrationBuilder.DropTable(
                name: "DanhGiaDichVus");

            migrationBuilder.DropTable(
                name: "KetQuaKhams");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "DichVus");

            migrationBuilder.DropTable(
                name: "LichKhams");

            migrationBuilder.DropTable(
                name: "BacSis");

            migrationBuilder.DropTable(
                name: "BenhNhans");

            migrationBuilder.DropTable(
                name: "Khoas");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
