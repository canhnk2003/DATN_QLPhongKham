var dsDG;
var dgId = "";
var dgByDoctor = "";
const userId = localStorage.getItem("userId");
$(document).ready(async function () {
  const bacSiId = await getDoctorByUserId(); // Lấy thông tin bác sĩ theo userId
  //   console.log("Bác sĩ ID:", bacSiId);
  //Lấy tất cả dữ liệu
  await getData(bacSiId);

  //Lấy tất cả dữ liệu đánh giá trung bình của bác sĩ
  await getDataRatingDoctor(bacSiId);

  // Tìm kiếm bảng đánh giá chi tiết
  initTableSearch(".m-input-search", "#tblRating");

  var tooltipTriggerList = [].slice.call(
    document.querySelectorAll('[data-bs-toggle="tooltip"]')
  );
  var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl);
  });

  //Xử lý sự kiện khi nhấn nút Refresh
  $(".m-toolbar-refresh").click(function () {
    getData(bacSiId);
  });

  //Xử lý sự kiện khi nhấn nút Export
  $(".m-toolbar-export").click(function () {
    exportToExcel();
  });
});

// Xử lý khi nhấn xuất file
async function exportToExcel() {
  try {
    // Lấy tên bệnh nhân và bác sĩ đồng thời cho tất cả các mục trong dsLK
    const formattedData = await Promise.all(
      dsDG.map(async (item) => {
        return {
          "Mã đánh giá": item.danhGiaId,
          "Mã lịch khám": item.lichKhamId,
          "Bác sĩ": item.tenBacSi,
          "Bệnh nhân": item.tenBenhNhan,
          "Đánh giá": item.tenBenhNhan,
          "Phản hồi": item.phanHoi,
          "Dịch vụ": item.tenDichVu,
        };
      })
    );

    // Tạo workbook và worksheet
    const worksheet = XLSX.utils.json_to_sheet(formattedData);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "DanhGia");

    // Xuất file Excel
    XLSX.writeFile(workbook, "DanhGia.xlsx");
  } catch (error) {
    console.error("Lỗi khi xuất dữ liệu:", error);
    // Hiển thị thông báo lỗi nếu cần
    showPopup("error", "Không thể xuất dữ liệu!");
  }
}

//Hàm tìm kiếm khi nhập nội dung vào ô tìm kiếm
function initTableSearch(inputSelector, tableSelector) {
  $(inputSelector).on("keyup", function () {
    var value = $(this).val().toLowerCase();
    $(tableSelector + " tbody tr").filter(function () {
      $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
    });
  });
}

//Hàm lấy dữ liệu đánh giá trung bình của bác sĩ
async function getDataRatingDoctor(bacSiId) {
  try {
    const response = await axiosJWT.get(
      `/api/v1/ServiceRatings/average/${bacSiId}`
    );
    dgByDoctor = response.data;
    // console.log(dgByDoctor);
    await displayDGByDoctor(dgByDoctor); // Hiển thị dữ liệu lên bảng
  } catch (error) {
    console.error("Lỗi không tìm được:", error);
  }
}

//Hiển thị dữ liệu lên bảng đánh giá trung bình của bác sĩ
async function displayDGByDoctor(data) {
  // Gán dữ liệu số lượt và trung bình
  $("#sumRating").text(data.soLuotDanhGia);
  $("#averageRating").text(data.soSaoTrungBinh);
  const star = generateStarRating(data.soSaoTrungBinh);
  $(".m-rating-star").html(star); // Hiển thị sao đánh giá trung bình
  $(".rating-text").text(data.thuHang + " / " + data.tongSoBacSi); // Hiển thị thứ hạng
}

// Hàm tạo ra đánh giá sao với gradient
function generateStarRating(rating) {
  let stars = "";
  for (let i = 1; i <= 5; i++) {
    let fillPercent = 0;

    if (rating >= i) {
      fillPercent = 100;
    } else if (rating + 1 > i) {
      fillPercent = Math.round((rating - (i - 1)) * 100);
    }

    stars += `<i class="fa fa-star" style="
          background: linear-gradient(90deg,rgb(255, 245, 65) ${fillPercent}%, #ccc ${fillPercent}%);
          -webkit-background-clip: text;
          -webkit-text-fill-color: transparent;"></i>
      `;
  }

  // Bọc sao trong 1 thẻ có tooltip
  return `<span data-bs-toggle="tooltip" title="${rating.toFixed(
    1
  )} / 5 sao">${stars}</span>`;
}

// Lấy toàn bộ lịch khám
async function getData(bacSiId) {
  try {
    const response = await axiosJWT.get(
      `/api/v1/ServiceRatings/doctor/${bacSiId}`
    );
    dsDG = response.data;
    $(".preloader").removeClass("d-none");
    $(".preloader").addClass("d-block");
    display(dsDG); // Hiển thị dữ liệu lên bảng
    $(".preloader").removeClass("d-block");
    $(".preloader").addClass("d-none");
  } catch (error) {
    console.error("Lỗi không tìm được:", error);
  }
}
// Lấy ra bác sĩ theo userId
async function getDoctorByUserId() {
  try {
    const response = await axiosJWT.get(`/api/Doctors/getbyuserid/${userId}`);
    const doctor = response.data;
    return doctor.bacSiId; // Trả về bacSiId
  } catch (error) {
    console.error("Lỗi không tìm được:", error);
  }
}

//Hiển thị dữ liệu lên bảng
async function display(data) {
  let tbody = $("#tblRating tbody");
  tbody.empty(); // Xóa dữ liệu cũ trước khi thêm mới

  data.forEach((item, index) => {
    const star = generateStarRating(item.danhGia);

    const row = `
        <tr>
          <td style="text-align: center">${index + 1}</td>
          <td style="text-align: center">${item.tenBenhNhan || ""}</td>
          <td style="text-align: center">${item.tenDichVu || ""}</td>
          <td style="text-align: center"> ${star}</td>
          <td style="text-align: center">${item.phanHoi || ""}</td>
          
        </tr>
      `;
    tbody.append(row);
  });
  // Khởi tạo tooltip sau khi các phần tử được thêm vào
  const tooltipElements = $('[data-bs-toggle="tooltip"]');
  tooltipElements.each(function () {
    new bootstrap.Tooltip(this); // Khởi tạo tooltip cho từng phần tử
  });
}
