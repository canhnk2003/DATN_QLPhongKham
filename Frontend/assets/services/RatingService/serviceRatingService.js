var dsDG;
var dgId = "";
let dsDGByDoctor = "";
$(document).ready(async function () {
  //Lấy tất cả dữ liệu
  getData();

  //Lấy tất cả dữ liệu đánh giá trung bình của bác sĩ
  await getDataRatingDoctor();

  //Mở modal xác nhận xóa
  $(document).on("click", ".m-delete", function () {
    dgId = $(this).data("id");
    console.log("ID được chọn:", dgId);
  });

  //Xử lý sự kiện khi nhấn nút xóa
  $("#btnDelete").click(async function () {
    deleteRating();
  });

  //Xử lý sự kiện khi nhấn nút Refresh
  $(".m-toolbar-refresh").click(async function () {
    getData();
  });

  // Tìm kiếm bảng đánh giá chi tiết
  initTableSearch(".m-input-search", "#tblRating");

  // Tìm kiếm bảng đánh giá trung bình
  initTableSearch("#searchInput", "#tblRatingAverage");

  // const tooltipTriggerList = $('[data-bs-toggle="tooltip"]').tooltip();
  // tooltipTriggerList.forEach((el) => new bootstrap.Tooltip(el));
  var tooltipTriggerList = [].slice.call(
    document.querySelectorAll('[data-bs-toggle="tooltip"]')
  );
  var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl);
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

    const formattedDataStatis = await Promise.all(
      dsDGByDoctor.map(async (item) => {
        return {
          "Định danh": item.bacSiId,
          "Mã BS": item.maBacSi,
          "Bác sĩ": item.tenBacSi,
          "Bằng cấp": item.bangCap,
          "Kinh nghiệm": item.soNamKinhNghiem + " năm",
          "Khoa": item.tenKhoa,
          "Tổng lượt đánh giá": item.soLuotDanhGia,
          "Đánh giá trung bình": item.soSaoTrungBinh,
          "Xếp hạng": item.thuHang + "/" + item.tongSoBacSi,
        };
      })
    );

    // Tạo workbook và worksheet
    const worksheet = XLSX.utils.json_to_sheet(formattedData);
    const worksheetSta = XLSX.utils.json_to_sheet(formattedDataStatis);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "DanhGia");
    XLSX.utils.book_append_sheet(workbook, worksheetSta, "ThongKeDanhGia");

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
async function getDataRatingDoctor() {
  try {
    const response = await axiosJWT.get(`/api/v1/ServiceRatings/average`);
    dsDGByDoctor = response.data;
    // console.log(dsDGByDoctor);
    await displayDGByDoctor(dsDGByDoctor); // Hiển thị dữ liệu lên bảng
  } catch (error) {
    console.error("Lỗi không tìm được:", error);
  }
}

//Hiển thị dữ liệu lên bảng đánh giá trung bình của bác sĩ
async function displayDGByDoctor(data) {
  let tbody = $("#tblRatingAverage tbody");
  tbody.empty(); // Xóa dữ liệu cũ trước khi thêm mới

  data.forEach((item, index) => {
    const star = generateStarRating(item.soSaoTrungBinh);

    const row = `
            <tr>
                <td class="align-middle">${index + 1}</td>
                <td class="align-middle">${item.maBacSi}</td>
                <td class="align-middle">${item.tenBacSi}</td>
                <td class="align-middle">${item.bangCap}</td>
                <td class="align-middle">${item.soNamKinhNghiem}</td>
                <td class="align-middle">${item.tenKhoa}</td>
                <td class="align-middle">${item.soLuotDanhGia}</td>
                <td class="align-middle">${star}</td>
                <td class="align-middle">${item.thuHang} / ${
      item.tongSoBacSi
    }</td>
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

//Hàm xóa đánh giá theo ID
function deleteRating() {
  // Hiển thị trạng thái đang xử lý
  $("#dialog-confirm-delete #btnDelete")
    .prop("disabled", true)
    .text("Đang xóa...");
  axiosJWT
    .delete(`/api/v1/ServiceRatings/${dgId}`)
    .then(function (response) {
      console.log("Xóa đánh giá thành công:", response.data);
      showPopup("success", "Thành công! Đánh giá đã được xóa.");
      $("#dialog-confirm-delete #btnDelete")
        .prop("disabled", false)
        .text("Xóa");
      $("#dialog-confirm-delete").modal("hide");
      getData(); // Tải lại dữ liệu sau khi cập nhật
    })
    .catch(function (error) {
      showPopup("error", "Lỗi! Không thể xóa đánh giá.");
      $("#dialog-confirm-delete #btnDelete")
        .prop("disabled", false)
        .text("Xóa");
      $("#dialog-confirm-delete").modal("hide");
      console.error("Lỗi khi xóa đánh giá: ", error);
    });
}

//Hàm hiển thị popup thông báo
function showPopup(type, message) {
  const popupItem = $(`.m-popup-item.m-popup-${type}`);

  // Xóa nội dung trước đó
  popupItem.find(".m-popup-text").empty();

  // Cập nhật nội dung thông báo
  const [title, detail] = message.split("! ");
  popupItem
    .find(".m-popup-text")
    .append(`<span>${title}! </span>`)
    .append(detail);

  // Hiển thị popup block
  popupItem.addClass("show");

  // Ẩn popup sau 3 giây
  setTimeout(() => {
    popupItem.removeClass("show");
  }, 3000);

  // Đảm bảo tắt popup nếu người dùng đóng
  popupItem.find(".m-popup-close").on("click", function () {
    // Đóng popup
    popupItem.removeClass("show");
  });
}

// Lấy toàn bộ lịch khám
async function getData() {
  try {
    const response = await axiosJWT.get(`/api/v1/ServiceRatings`);
    dsDG = response.data;
    // console.log(dsDG);
    display(dsDG); // Hiển thị dữ liệu lên bảng
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
          <td style="text-align: center">${item.tenBacSi || ""}</td>
          <td style="text-align: center">${item.tenDichVu || ""}</td>
          <td style="text-align: center"> ${star}</td>
          <td style="text-align: center">${item.phanHoi || ""}</td>
          <td>
            <div class="m-table-tool">
              <div class="m-delete m-tool-icon"
                   data-id="${item.danhGiaId}"
                   data-bs-toggle="modal"
                   data-bs-target="#dialog-confirm-delete">
                <i class="fas fa-trash-alt text-danger"></i>
              </div>
            </div>
          </td>
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
