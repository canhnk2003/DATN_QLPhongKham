var dsBN;
var bsId = "";
$(document).ready(async function () {
  await getDoctorId();
  getData();
  // Sự kiện khi nhập vào ô tìm kiếm
  $(".m-input-search").on("keyup", function () {
    var value = $(this).val().toLowerCase();
    $("#tblBenhNhan tbody tr").filter(function () {
      $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
    });
  });

  //Xử lý sự kiện khi nhấn nút Export
  $(".m-toolbar-export").click(function () {
    exportToExcel();
  });

  //Xử lý sự kiện khi nhấn nút Refresh
  $(".m-toolbar-refresh").click(function () {
    getData();
  });
});

// Xử lý khi nhấn xuất file
async function exportToExcel() {
  try {
    // Lấy tên bệnh nhân và bác sĩ đồng thời cho tất cả các mục trong dsLK
    const formattedData = await Promise.all(
      dsBN.map(async (item) => {
        return {
          "Định danh": item.benhNhanId,
          "Mã bệnh nhân": item.maBenhNhan,
          "Họ tên": item.hoTen,
          "Email": item.email,
          "Ngày sinh": new Date(item.ngaySinh).toLocaleDateString("en-GB"),
          "Giới tính": item.gioiTinh,
          "Địa chỉ": item.diaChi,
          "Số điện thoại": item.soDienThoai,
        };
      })
    );

    // Tạo workbook và worksheet
    const worksheet = XLSX.utils.json_to_sheet(formattedData);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "BenhNhan");

    // Xuất file Excel
    XLSX.writeFile(workbook, "BenhNhan.xlsx");
  } catch (error) {
    console.error("Lỗi khi xuất dữ liệu:", error);
    // Hiển thị thông báo lỗi nếu cần
    showPopup("error", "Không thể xuất dữ liệu!");
  }
}

function getData() {
  axiosJWT
    .get(`/api/Patients/getbydoctorid/${bsId}`)
    .then(function (response) {
      dsBN = response.data;
      $(".preloader").removeClass("d-none");
      $(".preloader").addClass("d-block");
      display(dsBN);
      $(".preloader").removeClass("d-block");
      $(".preloader").addClass("d-none");
    })
    .catch(function (error) {
      console.error("Lỗi không tìm được:", error);
    });
}

function display(data) {
  const tableBody = document.querySelector("#tblBenhNhan tbody");
  tableBody.innerHTML = ""; // Xóa nội dung cũ nếu có

  data.forEach((item, index) => {
    const row = `
      <tr bn-id="${item.benhNhanId}">
        <td class="m-data-left">${index + 1}</td>
        <td class="m-data-left">${item.maBenhNhan}</td>
        <td class="m-data-left">${item.hoTen}</td>
        <td class="m-data-left">${item.gioiTinh || "Không xác định"}</td>
        <td class="m-data-left">${formatDate(item.ngaySinh)}</td>
        <td class="m-data-left">${item.email || "Chưa có email"}</td>
        <td class="m-data-left">${item.diaChi || "Chưa có địa chỉ"}</td>
    `;
    tableBody.innerHTML += row; // Thêm hàng vào bảng
  });
}

function formatDate(dateString) {
  if (!dateString) return "Không xác định";
  const date = new Date(dateString);
  return date.toLocaleDateString("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });
}

async function getDoctorId() {
  try {
    let userId = localStorage.getItem("userId");
    const response = await axiosJWT.get(`/api/Doctors/getbyuserid/${userId}`);
    bsId = response.data.bacSiId; // Lấy giá trị ID bác sĩ
  } catch (error) {
    console.error("Lỗi khi gọi API:", error);
  }
}
