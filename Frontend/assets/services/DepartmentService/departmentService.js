var dsK;
let khoaId = "";
$(document).ready(function () {
  //Lấy dữ liệu
  getData();

  //Khi nhấn vào 1 hàng thì điền dữ liệu vào input
  $("#tblDepartment tbody").on("click", "tr", function () {
    // Lấy giá trị attribute, ví dụ "data-id"
    khoaId = $(this).attr("k-id");
    // Lấy nội dung của cột "Tên khoa" (cột thứ 3, index 2)
    var tenKhoa = $(this).find("td:eq(2)").text().trim();

    // Gán giá trị vào ô input
    $("#tenKhoa").val(tenKhoa);
  });

  //Xử lý sự kiện thêm mới
  $("#add").click(function () {
    addDepartment();
  });

  //Xử lý sự kiện sửa
  $("#update").click(function () {
    updateDepartment();
  });
  //Xứ lý sự kiện xóa
  $("#btnDelete").click(function () {
    deleteDepartment();
  });

  //Xử lý sự kiện khi nhấn nút Refresh
  $("#refresh-data").click(function () {
    getData();
  });

  // Sự kiện khi nhập vào ô tìm kiếm
  $(".m-input-search").on("keyup", function () {
    var value = $(this).val().toLowerCase();
    $("#tblDepartment tbody tr").filter(function () {
      $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
    });
  });
});

//Hàm lấy dữ liệu điền vào table
function getData() {
  axiosJWT
    .get(`/api/v1/Departments`)
    .then(function (response) {
      dsK = response.data;
      //   console.log(dsK);
      $(".preloader").removeClass("d-none");
      $(".preloader").addClass("d-block");
      display(dsK);
      $(".preloader").removeClass("d-block");
      $(".preloader").addClass("d-none");
    })
    .catch(function (error) {
      console.error("Lỗi không tìm được:", error);
    });
}
//Hiển thị dữ liệu lên bảng
function display(data) {
  const tableBody = document.querySelector("#tblDepartment tbody");
  tableBody.innerHTML = ""; // Xóa nội dung cũ nếu có

  data.forEach((item, index) => {
    // Tạo hàng mới
    const row = document.createElement("tr");
    row.setAttribute("k-id", item.khoaId);

    row.innerHTML = `
        <td>${index + 1}</td>
        <td>${item.maKhoa}</td>
        <td>${item.tenKhoa}</td>
      `;
    tableBody.appendChild(row); // Thêm hàng vào bảng
  });
}

//Hàm thêm mới 1 khoa
function addDepartment() {
  const tenKhoa = $("#tenKhoa").val();
  if (!tenKhoa) {
    showPopup("error", "Lỗi! Tên khoa không được để trống!");
  } else {
    const department = {
      khoaId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      maKhoa: "string",
      tenKhoa: tenKhoa,
    };
    axiosJWT
      .post(`/api/v1/Departments`, department)
      .then(function (response) {
        console.log("Thêm mới thành công:", response.data);
        // Hiển thị trạng thái thành công
        showPopup("success", "Thành công! Phòng ban đã được thêm.");
        getData(); // Tải lại dữ liệu sau khi cập nhật
      })
      .catch(function (error) {
        console.error("Lỗi khi thêm mới: ", error);
        showPopup("error", "Lỗi! Không thể thêm mới phòng ban.");
      });
  }
}
//Hàm thêm mới 1 khoa
function updateDepartment() {
  const tenKhoa = $("#tenKhoa").val();
  console.log(tenKhoa);
  if (!tenKhoa) {
    showPopup("error", "Lỗi! Tên khoa không được để trống!");
  } else {
    const department = {
      khoaId: khoaId,
      maKhoa: "string",
      tenKhoa: tenKhoa,
    };
    axiosJWT
      .put(`/api/v1/Departments/${khoaId}`, department)
      .then(function (response) {
        console.log("Cập nhật thành công:", response.data);
        // Hiển thị trạng thái thành công
        showPopup("success", "Thành công! Phòng ban đã được cập nhật.");
        getData(); // Tải lại dữ liệu sau khi cập nhật
      })
      .catch(function (error) {
        console.error("Lỗi khi cập nhật: ", error);
        showPopup("error", "Lỗi! Không thể cập nhật phòng ban.");
      });
  }
}
//Hàm thêm mới 1 khoa
function deleteDepartment() {
  axiosJWT
    .delete(`/api/v1/Departments/${khoaId}`)
    .then(function (response) {
      console.log("Xóa thành công:", response.data);
      // Hiển thị trạng thái thành công
      showPopup("success", "Thành công! Phòng ban đã được xóa.");
      getData(); // Tải lại dữ liệu sau khi cập nhật
    })
    .catch(function (error) {
      console.error("Lỗi khi cập nhật: ", error);
      showPopup("error", "Lỗi! Không thể xóa phòng ban.");
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
