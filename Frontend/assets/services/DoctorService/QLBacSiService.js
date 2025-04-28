var dsBS;
var bsID = "";
var dsKhoa;
$(document).ready(function () {
  getData();
  $("#refresh-data").click(function () {
    getData(); // Gọi hàm getData
  });
  const departmentSelectAdd = document.querySelector(
    "#modalThemBacSi #add-khoa"
  );
  getAllDepartment(departmentSelectAdd);
  const departmentSelectEdit = document.querySelector(
    "#modalSuaBacSi #edit-khoa"
  );
  getAllDepartment(departmentSelectEdit);

  // Gắn sự kiện cho nút hiển thị modal sửa
  $(document).on("click", ".m-edit", function () {
    // const benhNhanId = $(this).data("id"); // Lấy id từ thuộc tính data-id
    const bacSiId = $(this).closest("tr").attr("bs-id");
    bsID = bacSiId;
    const bacSi = dsBS.find((bs) => bs.bacSiId === bacSiId); // Tìm bệnh nhân trong danh sách

    if (bacSi) {
      fillEditModal(bacSi); // Hiển thị thông tin lên modal
    } else {
      console.error("Không tìm thấy thông tin bác sĩ!");
    }
  });

  // Gắn sự kiện cho nút hiển thị modal Thêm
  $("#btnOpenModalAdd").click(function () {
    let maBSNext = getMaxBacSiCode(dsBS);
    $("#add-mabs").val(maBSNext);
  });
  // Gắn sự kiện cho nút Thêm
  $("#btnAdd").click(function () {
    const bangCapValue = parseInt($("#add-bangCap").val());
    const khoaIdValue = $("#add-khoa").val(); // Lấy ID khoa từ combobox
    const newDoctor = {
      maBacSi: $("#add-mabs").val(),
      hoTen: $("#add-tenbs").val(),
      email: $("#add-email").val() || null,
      soDienThoai: $("#add-sdt").val() || null,
      bangCap: bangCapValue,
      diaChi: $("#add-diaChi").val() || null,
      soNamKinhNghiem: $("#add-namKinhNghiem").val() || null,
      gioLamViec: $("#add-gioLamViec").val() || null,
      khoaId: khoaIdValue || null,
    };
    console.log("Dữ liệu thêm:", newDoctor);

    // Gửi yêu cầu add tới API
    axiosJWT
      .post(`/api/Doctors`, newDoctor)
      .then(function (response) {
        console.log("Thêm thành công:", response.data);
        getData(); // Tải lại dữ liệu sau khi cập nhật
      })
      .catch(function (error) {
        showErrorPopup();
        console.error("Lỗi khi thêm:", error);
      });
  });

  // Gắn sự kiện cho nút Sửa
  $("#btnEdit").click(function () {
    const bangCapValue = parseInt($("#edit-bangCap").val());
    const khoaIdValue = $("#edit-khoa").val(); // Lấy ID khoa từ combobox
    const updatedDoctor = {
      bacSiId: bsID,
      maBacSi: $("#edit-mabs").val(),
      hoTen: $("#edit-tenbs").val(),
      email: $("#edit-email").val(),
      soDienThoai: $("#edit-sdt").val() || null,
      bangCap: bangCapValue,
      diaChi: $("#edit-diaChi").val() || null,
      soNamKinhNghiem: $("#edit-namKinhNghiem").val() || null,
      gioLamViec: $("#edit-gioLamViec").val() || null,
      khoaId: khoaIdValue || null,
    };

    console.log("Dữ liệu cập nhật:", updatedDoctor);

    // Gửi yêu cầu cập nhật tới API
    axiosJWT
      .put(`/api/Doctors/${updatedDoctor.bacSiId}`, updatedDoctor)
      .then(function (response) {
        console.log("Cập nhật thành công:", response.data);
        getData(); // Tải lại dữ liệu sau khi cập nhật
        showSuccessPopup();
      })
      .catch(function (error) {
        showErrorPopup();
        console.error("Lỗi khi cập nhật:", error);
      });
  });

  //Mở modal xác nhận xóa
  $(document).on("click", ".m-delete", function () {
    const bacSiId = $(this).closest("tr").attr("bs-id");
    bsID = bacSiId;
    console.log(bsID);
    const bacSi = dsBS.find((bs) => bs.bacSiId === bacSiId); // Tìm bệnh nhân trong danh sách
  });

  $("#btnDelete").click(function () {
    axiosJWT
      .delete(`/api/Doctors/${bsID}`)
      .then(function (response) {
        console.log("Xóa thành công:", response.data);
        getData(); // Tải lại dữ liệu sau khi cập nhật
      })
      .catch(function (error) {
        showErrorPopup();
        console.error("Lỗi khi xóa:", error);
      });
  });
  // Sự kiện khi nhập vào ô tìm kiếm
  $(".m-input-search").on("keyup", function () {
    var value = $(this).val().toLowerCase();
    $("#tblBacSi tbody tr").filter(function () {
      $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
    });
  });
  //Xử lý sự kiện khi nhấn nút Export
  $(".m-toolbar-export").click(function () {
    exportToExcel();
  });
  $(".m-toolbar-import").on("click", function () {
    $("#fileInput").click();
  });
  // Sự kiện thêm dữ liệu từ excel
  $("#fileInput").on("change", function () {
    var file = $("#fileInput")[0].files[0];
    if (!file) {
      showErrorPopup("Vui lòng chọn file.");
      return;
    }

    var formData = new FormData();
    formData.append("file", file); // Thêm file vào formData

    // Sử dụng Axios để gửi file lên server
    axiosJWT
      .post("/api/Doctors/import-bacsi", formData, {
        headers: {
          "Content-Type": "multipart/form-data", // Đảm bảo gửi với type này
        },
      })
      .then(function (response) {
        // Nếu upload thành công
        showSuccessPopup("Thêm thành công! " + response.data.message);
      })
      .catch(function (error) {
        // Nếu có lỗi trong quá trình upload
        showErrorPopup("Có lỗi xảy ra! Vui lòng thử lại sau!");
        console.error("Error:", error);
      });
  });
});

// Xử lý khi nhấn xuất file
async function exportToExcel() {
  try {
    // Lấy tên bệnh nhân và bác sĩ đồng thời cho tất cả các mục trong dsLK
    const formattedData = await Promise.all(
      dsBS.map(async (item) => {
        return {
          "Định danh": item.bacSiId,
          "Mã BS": item.maBacSi,
          "Bác sĩ": item.hoTen,
          Email: item.email,
          "Số điện thoại": item.soDienThoai,
          "Kinh nghiệm": item.soNamKinhNghiem + " năm",
          "Bằng cấp": item.tenBangCap,
          "Giờ làm việc": item.gioLamViec,
          "ĐỊa chỉ": item.diaChi,
        };
      })
    );

    // Tạo workbook và worksheet
    const worksheet = XLSX.utils.json_to_sheet(formattedData);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "BacSi");

    // Xuất file Excel
    XLSX.writeFile(workbook, "BacSi.xlsx");
  } catch (error) {
    console.error("Lỗi khi xuất dữ liệu:", error);
    // Hiển thị thông báo lỗi nếu cần
    showPopup("error", "Không thể xuất dữ liệu!");
  }
}

function getData() {
  // var userId = localStorage.getItem("userId");
  // $('#hotenHeader').text(localStorage.getItem(loggedInUsername));
  axiosJWT
    .get(`/api/Doctors`)
    .then(function (response) {
      dsBS = response.data;
      // console.log(dsBS);
      $(".preloader").addClass("d-block");
      display(dsBS);

      $(".preloader").removeClass("d-block");
    })
    .catch(function (error) {
      console.error("Lỗi không tìm được:", error);
    });
}
function display(data) {
  const tableBody = document.querySelector("#tblBacSi tbody");
  tableBody.innerHTML = ""; // Xóa nội dung cũ nếu có

  // Tạo một Map để ánh xạ từ khoaId sang tenKhoa
  const khoaMap = new Map();
  if (dsKhoa != null) {
    dsKhoa.forEach((item) => {
      khoaMap.set(item.khoaId, item.tenKhoa);
    });
  }
  // Lặp qua danh sách bác sĩ và xây dựng các hàng bảng
  data.forEach((item, index) => {
    const tenKhoa = khoaMap.get(item.khoaId) || "Chưa có khoa"; // Lấy tên khoa từ Map
    const row = `
      <tr bs-id="${item.bacSiId}">
        <td class="text-center">${index + 1}</td>
        <td class="m-data-left">${item.maBacSi}</td>
        <td class="m-data-left">${item.hoTen}</td>
        <td class="m-data-left">${item.tenBangCap}</td>
        <td class="m-data-left">${tenKhoa}</td>
        <td class="m-data-left">${item.soDienThoai}</td>
        <td class="m-data-left">${item.email || "Chưa có email"}</td>
        <td>
          <div class="m-table-tool">
            <div class="m-edit m-tool-icon" data-bs-toggle="modal" data-bs-target="#modalSuaBacSi" data-id="${
              item.bacSiId
            }">
              <i class="fas fa-edit text-primary"></i>
            </div>
            <div class="m-delete m-tool-icon" data-bs-toggle="modal" data-bs-target="#confirm-delete">
              <i class="fas fa-trash-alt text-danger"></i>
            </div>
          </div>
        </td>
      </tr>
    `;
    tableBody.innerHTML += row; // Thêm hàng vào bảng
  });
}

// Lấy toàn bộ khoa
function getAllDepartment(selectElement) {
  axiosJWT
    .get(`/api/v1/Departments`)
    .then(function (response) {
      dsKhoa = response.data;
      dsKhoa.forEach((item) => {
        const option = document.createElement("option");
        option.value = item.khoaId;
        option.textContent = item.tenKhoa;
        selectElement.appendChild(option);
      });
    })
    .catch(function (error) {
      console.error("Lỗi không tìm được:", error);
    });
}

// Hàm điền thông tin vào modal
function fillEditModal(bacSi) {
  // Gán dữ liệu vào các trường input của modal
  $("#edit-mabs").val(bacSi.maBacSi); // Mã bệnh nhân
  $("#edit-tenbs").val(bacSi.hoTen); // Họ tên
  $("#edit-sdt").val(bacSi.soDienThoai); // Số điện thoại
  $("#edit-email").val(bacSi.email || ""); // Email
  // Gán bằng cấp
  const bangCapValue = bacSi.bangCap;
  $("#edit-bangCap").val(bangCapValue);
  $("#edit-namKinhNghiem").val(bacSi.soNamKinhNghiem); // Số năm kinh nghiệm
  // Gán địa chỉ
  $("#edit-diaChi").val(bacSi.diaChi || "");
  $("#edit-gioLamViec").val(bacSi.gioLamViec); // Giờ làm việc
  // Gán bằng cấp
  const khoaIdValue = bacSi.khoaId;
  $("#edit-khoa").val(khoaIdValue);
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

function showErrorPopup() {
  const errorPopup = document.getElementById("error-popup");
  errorPopup.style.visibility = "visible";

  // Ẩn popup sau 3 giây
  setTimeout(() => {
    hideErrorPopup();
  }, 3000);
}
function hideErrorPopup() {
  const errorPopup = document.getElementById("error-popup");
  errorPopup.style.visibility = "hidden";
}

// Hàm lấy mã bác sĩ lớn nhất
function getMaxBacSiCode(dsBS) {
  let maxCode = 0;
  dsBS.forEach((item) => {
    const code = parseInt(item.maBacSi.replace("BS", "")); // Loại bỏ 'BS' và chuyển thành số
    if (code > maxCode) {
      maxCode = code;
    }
  });
  const nextCode = maxCode + 1;
  return "BS" + nextCode.toString().padStart(3, "0");
}
function showSuccessPopup(message = "") {
  // Hiển thị popup
  const popup = document.getElementById("success-popup");
  popup.style.visibility = "visible"; // Hoặc có thể dùng popup.classList.add('visible');
  if (message) {
    $(".m-popup-text-success span").text(message);
  }

  // Tự động ẩn popup sau 3 giây (3000ms)
  setTimeout(() => {
    closePopup();
  }, 3000);
}

function closePopup() {
  const popup = document.getElementById("success-popup");
  popup.style.visibility = "hidden"; // Ẩn popup
}
function kiemTraTen() {
  const ten = document.getElementById("add-tenbs");
  const specialCharRegex = /[^a-zA-Z0-9\s\u00C0-\u1EF9]/; // Tìm bất kỳ ký tự nào không phải là chữ cái, chữ số hoặc khoảng trắng

  if (specialCharRegex.test(ten.value)) {
    ten.style.border = "2px solid red"; // Thêm viền đỏ khi có ký tự đặc biệt
    ten.setCustomValidity("Tên không được chứa ký tự đặc biệt!");
  } else {
    ten.style.border = ""; // Xóa viền nếu không có ký tự đặc biệt
    ten.setCustomValidity("");
  }
}
function kiemTraEmail() {
  const emailInput = document.getElementById("add-email");
  // Biểu thức chính quy kiểm tra định dạng email hợp lệ
  const emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;

  // Kiểm tra nếu email trống
  if (emailInput.value.trim() === "") {
    emailInput.style.border = "2px solid red"; // Viền đỏ khi trống
    emailInput.setCustomValidity("Email không được để trống");
  }
  // Kiểm tra email hợp lệ
  else if (!emailRegex.test(emailInput.value)) {
    emailInput.style.border = "2px solid red"; // Viền đỏ nếu email không hợp lệ
    emailInput.setCustomValidity("Email không hợp lệ");
  } else {
    emailInput.style.border = ""; // Xóa viền đỏ nếu email hợp lệ
    emailInput.setCustomValidity(""); // Không có lỗi
  }
}
