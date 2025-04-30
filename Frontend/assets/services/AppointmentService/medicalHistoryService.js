var userId = localStorage.getItem("userId");
var lkId = "";
var kq;
let medicalHistory = "";
let selectedRating = 0;
let appointment = "";
let ratingId = "";
let statusDelete = "";
$(document).ready(async function () {
  await getAvata();
  // Lắng nghe sự kiện click trên các nút .optionButton
  $(document).on("click", ".optionButton", function () {
    // Tìm phần tử cha gần nhất có lớp .custom-card
    const parentCard = $(this).closest(".custom-card");

    // Lấy giá trị lkId từ thuộc tính của phần tử cha
    lkId = parentCard.attr("lkId");
    // console.log(lkId);
    statusDelete = "deleteAppointment";
  });

  //Xử lý khi nhấn nút đồng ý hủy
  $("#btnCancel").click(function () {
    //Lấy lý do
    const reason = $("#modal-confirm-cancel #reason").val();
    // Kiểm tra nếu lý do trống
    if (!reason.trim()) {
      // Thêm class m-input-error vào ô input lý do
      $("#modal-confirm-cancel #reason").addClass("input-error");
      // Thêm thông báo title cho người dùng
      $("#modal-confirm-cancel #reason").attr(
        "title",
        "Lý do từ chối không được để trống!"
      );
      // Focus vào ô input
      $("#modal-confirm-cancel #reason").focus();
    } else {
      // Xóa thông báo lỗi nếu lý do không trống
      $("#modal-confirm-cancel #reason").removeClass("m-input-error");
      $("#modal-confirm-cancel #reason").removeAttr("title");
      // Gọi API Hủy lịch khám
      cancelAppointment(reason);
    }
  });
  //Xử lý khi nhấn nút đồng ý xóa
  $("#btnDelete").click(function () {
    //Gọi API Hủy lịch khám
    if (statusDelete === "deleteAppointment") {
      deleteAppointment();
    }
    if (statusDelete === "deleteRating") {
      deleteRating();
    }
  });
  //Xử lý khi nhấn nút đồng ý hoàn thành
  $("#btnComplete").click(function () {
    //Gọi API Hủy lịch khám
    completeAppointment();
  });

  //Xử lý khi nhấn option xem
  $(document).on("click", "#optionViewResult", function () {
    getResultAppointment();
  });
  //Xử lý khi nhấn option hoàn thành
  $(document).on("click", "#optionViewStatus", function () {
    const status = $(this).data("status");
    console.log(status);
    getViewStatus(status.trim());
  });
  //Xử lý khi nhấn option đánh giá
  $(document).on("click", "#optionRating", function () {
    getAppointmentById();
    const str = $(this).text().trim();
    // console.log(str);
    const ratingData = $(this).data("obj");
    const rating =
      ratingData !== null
        ? JSON.parse(decodeURIComponent(atob(ratingData)))
        : null;
    // console.log(rating);
    ratingId = rating ? rating.danhGiaId : null; //Lấy ra id đánh giá
    //Nếu là "Xem đánh giá" thì gọi chức năng sửa đánh giá
    if (str === "Xem đánh giá") {
      //Hiển thị đánh giá
      $("#modal-rating-appointment .modal-title").text("Xem đánh giá");
      selectedRating = rating.danhGia;
      $("#reviewContent").val(rating.phanHoi);
      highlightStars(selectedRating);
      //Ẩn nút gửi đánh giá
      $("#submitReview").addClass("d-none");
      //Hiện nút sửa đánh giá
      $("#editReview").removeClass("d-none");
      //Ẩn nút xóa đánh giá
      $("#deleteReview").removeClass("d-none");
    } else {
      //Nếu là "Đánh giá dịch vụ" thì gọi chức năng đánh giá dịch vụ
      $("#modal-rating-appointment .modal-title").text("Đánh giá dịch vụ");
      selectedRating = 0;
      $("#reviewContent").val("");
      highlightStars(selectedRating);
      //Ẩn nút sửa đánh giá
      $("#editReview").addClass("d-none");
      //Hiện nút gửi đánh giá
      $("#submitReview").removeClass("d-none");
      //Ẩn nút xóa đánh giá
      $("#deleteReview").addClass("d-none");
    }
  });

  //Hiệu ứng + sự kiện khi nhấn vào ngối sao
  $(".rating-stars i")
    .on("mouseenter", function () {
      const index = $(this).data("index");
      highlightStars(index);
    })
    .on("mouseleave", function () {
      highlightStars(selectedRating);
    })
    .on("click", function () {
      selectedRating = $(this).data("index");
      highlightStars(selectedRating);
    });

  //Xử lý khi nhấn nút gửi đánh giá
  $("#submitReview").click(function () {
    createRating();
  });
  //Xử lý khi nhấn nút sửa đánh giá
  $("#editReview").click(function () {
    getRating();
  });
  //Xử lý khi nhấn nút Xóa đánh giá
  $("#deleteReview").click(function () {
    statusDelete = "deleteRating";
    //Đổi nội dung modal
    $("#modal-confirm-delete .modal-body").text(
      "Bạn có chắc chắn muốn xóa đánh giá này không?"
    );
  });
});

//Xử lý xóa đánh giá
// function deleteRating() {
//   // Hiển thị trạng thái đang xử lý
//   $("#modal-rating-appointment #deleteReview")
//     .prop("disabled", true)
//     .text("Đang xóa đánh giá...");
//   //Gọi API xóa đánh giá
//   axiosJWT
//     .delete(`/api/v1/ServiceRatings/${ratingId}`)
//     .then(function (response) {
//       // console.log("Xóa đánh giá thành công:", response.data);
//       getAvata();
//       showPopup("success", "Thành công! Đánh giá đã được xóa.");
//       $("#modal-rating-appointment").modal("hide");
//       $("#modal-rating-appointment #deleteReview")
//         .prop("disabled", false)
//         .text("Xóa");
//       // Reset lại nội dung đánh giá
//       $("#reviewContent").val("");
//       selectedRating = 0; // Đặt lại giá trị đánh giá đã chọn
//       highlightStars(0); // Đặt lại sao về trạng thái ban đầu
//     })
//     .catch(function (error) {
//       getAvata();
//       showPopup("error", "Lỗi! Không thể xóa đánh giá.");
//       // console.error("Lỗi khi xóa đánh giá: ", error);
//       $("#modal-rating-appointment #deleteReview")
//         .prop("disabled", false)
//         .text("Xóa");
//       $("#modal-rating-appointment").modal("hide");
//       // Reset lại nội dung đánh giá
//       $("#reviewContent").val("");
//       selectedRating = 0; // Đặt lại giá trị đánh giá đã chọn
//       highlightStars(0); // Đặt lại sao về trạng thái ban đầu
//     });
// }
function deleteRating() {
  handleApiRequest({
    method: "delete",
    url: `/api/v1/ServiceRatings/${ratingId}`,
    btnSelector: "#modal-rating-appointment #btnDelete",
    btnLoadingText: "Đang xóa đánh giá...",
    btnDefaultText: "Xóa",
    successMessage: "Thành công! Đánh giá đã được xóa.",
    errorMessage: "Lỗi! Không thể xóa đánh giá.",
    modalSelector: "#modal-rating-appointment",
  });
}

//Xử lý khi nhấn nút sửa đánh giá
function getRating() {
  //Lấy nội dung từ modal và gắn vào đối tượng
  const ratingData = {
    danhGiaId: ratingId,
    // benhNhanId: appointment.benhNhanId,
    // bacSiId: appointment.bacSiId,
    // lichKhamId: lkId,
    danhGia: selectedRating || 0,
    phanHoi: $("#reviewContent").val().trim() || "",
    // ngayTao: new Date().toISOString(),
    // ngayCapNhat: new Date().toISOString(),
  };
  // console.log(ratingData);
  //Nếu là sửa đánh giá thì gọi hàm sửa đánh giá
  sendEditRating(ratingData);
}

//Hàm sửa đánh giá dịch vụ
function sendEditRating(ratingData) {
  handleApiRequest({
    method: "put",
    url: `/api/v1/ServiceRatings/${ratingId}`,
    data: ratingData,
    btnSelector: "#modal-rating-appointment #editReview",
    btnLoadingText: "Đang sửa đánh giá...",
    btnDefaultText: "Sửa",
    successMessage: "Thành công! Đánh giá đã được sửa.",
    errorMessage: "Lỗi! Không thể sửa đánh giá.",
    modalSelector: "#modal-rating-appointment",
  });
}

//Xử lý khi nhấn nút gửi đánh giá
function createRating() {
  //Lấy nội dung từ modal và gắn vào đối tượng
  const ratingData = {
    danhGiaId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    benhNhanId: appointment.benhNhanId,
    bacSiId: appointment.bacSiId,
    lichKhamId: lkId,
    danhGia: selectedRating || 0,
    phanHoi: $("#reviewContent").val().trim() || "",
    ngayTao: new Date().toISOString(),
    ngayCapNhat: new Date().toISOString(),
  };
  // console.log(ratingData);
  sendRating(ratingData);
}

//Hàm đánh giá dịch vụ
function sendRating(ratingData) {
  handleApiRequest({
    method: "post",
    url: "/api/v1/ServiceRatings",
    data: ratingData,
    btnSelector: "#modal-rating-appointment #submitReview",
    btnLoadingText: "Đang gửi đánh giá...",
    btnDefaultText: "Gửi",
    successMessage: "Thành công! Đánh giá đã được gửi.",
    errorMessage: "Lỗi! Không thể gửi đánh giá.",
    modalSelector: "#modal-rating-appointment",
  });
}

//Hàm lấy thông tin lịch khám theo id
function getAppointmentById() {
  //Gọi API lấy thông tin lịch khám
  axiosJWT
    .get(`/api/v1/Appointments/${lkId}`)
    .then(function (response) {
      appointment = response.data;
      // console.log("Lịch khám:", appointment);
    })
    .catch(function (error) {
      console.error("Lỗi khi lấy lịch khám: ", error);
    });
}

function highlightStars(rating) {
  $(".rating-stars i").each(function () {
    const index = $(this).data("index");
    if (index <= rating) {
      $(this).addClass("hovered");
    } else {
      $(this).removeClass("hovered");
    }
  });
}

//Hoàn thành lịch khám
function completeAppointment() {
  handleApiRequest({
    method: "put",
    url: `/api/v1/Appointments/appointment/${lkId}`,
    btnSelector: "#modal-confirm-complete #btnComplete",
    btnLoadingText: "Đang hoàn thành...",
    btnDefaultText: "Có",
    successMessage: "Thành công! Lịch khám đã được hoàn thành.",
    errorMessage: "Lỗi! Không thể hoàn thành lịch khám.",
    modalSelector: "#modal-confirm-complete",
    resetUI: false,
  });
}

//Viết hàm dùng chung
function handleApiRequest({
  method = "get", // get, post, put, delete
  url,
  data = null,
  btnSelector = null, // nút đang xử lý
  btnLoadingText = "Đang xử lý...",
  btnDefaultText = "Xác nhận",
  successMessage = "Thành công!",
  errorMessage = "Đã xảy ra lỗi!",
  onSuccess = () => {},
  onError = () => {},
  modalSelector = null,
  resetUI = true,
}) {
  if (btnSelector) {
    $(btnSelector).prop("disabled", true).text(btnLoadingText);
  }

  axiosJWT[method](url, data)
    .then((response) => {
      getAvata();
      showPopup("success", successMessage);
      if (modalSelector) {
        $(modalSelector).modal("hide");
      }
      if (btnSelector) {
        $(btnSelector).prop("disabled", false).text(btnDefaultText);
      }
      if (resetUI) {
        $("#reviewContent").val("");
        selectedRating = 0;
        highlightStars(0);
      }
      onSuccess(response.data);
    })
    .catch((error) => {
      getAvata();
      showPopup("error", errorMessage);
      if (btnSelector) {
        $(btnSelector).prop("disabled", false).text(btnDefaultText);
      }
      if (modalSelector) {
        $(modalSelector).modal("hide");
      }
      if (resetUI) {
        $("#reviewContent").val("");
        selectedRating = 0;
        highlightStars(0);
      }
      onError(error);
    });
}

//Lấy kết quả khám theo LichKhamId
async function getResultAppointment() {
  try {
    const response = await axiosJWT.get(`/api/Results/ketqua/${lkId}`);
    kq = response.data;
    fillViewModal();
  } catch (error) {
    console.error("Lỗi không tìm được kết quả khám: ", error);
  }
}

//Điền thông tin vào modal xem
function fillViewModal() {
  if (kq === null) {
    showPopup("error", "Lỗi! Không có kết quả khám!");
  } else {
    $("#medicalHistory").val(medicalHistory);
    $("#diagnose").val(kq.chanDoan);
    const prescriptionStr = kq.chiDinhThuoc;
    const noteStr = kq.ghiChu;

    //Tách prescriptionStr, noteStr và gắn vào mảng
    const prescriptions = prescriptionStr.split(",").map((x) => x.trim());
    const notes = noteStr.split(";").map((x) => x.trim());
    //Điền dữ liệu vào table từ mảng
    $("#resultTable tbody").empty();

    // Hiển thị từng dòng lên table
    if (prescriptions.length === notes.length) {
      for (let i = 0; i < prescriptions.length; i++) {
        const newRow = `
              <tr>
                <td class="text-center">${i + 1}</td>
                <td>${prescriptions[i]}</td>
                <td>${notes[i] || ""}</td>
              </tr>
            `;
        $("#resultTable tbody").append(newRow);
      }
    }
  }
}

//Xử lý khi nhấn đồng ý xóa lịch khám
function deleteAppointment() {
  handleApiRequest({
    method: "delete",
    url: `/api/v1/Appointments/${lkId}`,
    btnSelector: "#modal-confirm-cancel #btnDelete",
    btnLoadingText: "Đang xóa...",
    btnDefaultText: "Xóa",
    successMessage: "Thành công! Lịch khám đã được xóa.",
    errorMessage: "Lỗi! Không thể xóa lịch khám.",
    modalSelector: "#modal-confirm-cancel",
    resetUI: false,
  });
}

//Xử lý khi nhấn đồng ý hủy lịch khám
function cancelAppointment(reason) {
  // Hiển thị trạng thái đang xử lý
  $("#modal-confirm-cancel #btnCancel")
    .prop("disabled", true)
    .text("Đang xử lý...");
  axiosJWT
    .put(
      `/api/v1/Appointments/cancel/${lkId}`,
      JSON.stringify("Bệnh nhân: " + reason),
      {
        headers: {
          "Content-Type": "application/json", // Đảm bảo header là application/json
        },
      }
    )
    .then(function (response) {
      console.log("Hủy lịch khám thành công:", response.data);
      $("#modal-confirm-cancel").modal("hide");
      getAvata();
      showPopup("success", "Thành công! Lịch khám đã được hủy.");
      $("#modal-confirm-cancel #btnCancel").prop("disabled", false).text("Có");
    })
    .catch(function (error) {
      $("#modal-confirm-cancel").modal("hide");
      getAvata();
      showPopup("error", "Lỗi! Không thể hủy lịch khám.");
      $("#modal-confirm-cancel #btnCancel").prop("disabled", false).text("Có");
      console.error("Lỗi khi hủy lịch khám: ", error);
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

// Lấy avatar và lịch khám của bệnh nhân
async function getAvata() {
  try {
    const { data: bn } = await axiosJWT.get(
      `/api/Patients/getbyuserid/${userId}`
    );

    if (bn.hinhAnh) {
      $("#avatar").attr("src", `http://localhost:37649${bn.hinhAnh}`);
      $("#uploadedImage").attr("src", "http://localhost:37649" + bn.hinhAnh);
    }
    if (bn.hoTen) {
      $("#hotenHeader").text(bn.hoTen);
    } else {
      $("#hotenHeader").text("User name");
    }
    medicalHistory = bn.tienSuBenhLy;
    getAllAppointmentByBenhNhanId(bn.benhNhanId);
  } catch (error) {
    console.error("Lỗi không tìm được:", error);
  }
}

//Lấy danh sách lịch khám
function getAllAppointmentByBenhNhanId(benhNhanId) {
  // console.log(benhNhanId);
  axiosJWT
    .get(`/api/v1/Appointments/patient/${benhNhanId}`)
    .then(function (response) {
      const dsLK = response.data;
      display1(dsLK);
    })
    .catch(function (error) {
      console.error("Lỗi không tìm được:", error);
    });
}
// Hàm để chuyển trạng thái thành lớp CSS
function getBadgeClass(trangThai) {
  switch (trangThai) {
    case "Đã đặt":
      return "bg-primary";
    case "Đang xử lý":
      return "bg-warning";
    case "Đã hủy":
      return "bg-danger";
    case "Hoàn thành":
      return "complete";
    default:
      return "bg-success";
  }
}
// Hàm để chuyển trạng thái thành icon
function getIcon(trangThai) {
  switch (trangThai) {
    case "Đã đặt":
      return '<i class="fa-solid fa-check me-1"></i>';
    case "Đang xử lý":
      return '<i class="fas fa-spinner fa-spin me-1"></i>';
    case "Đã hủy":
      return '<i class="fa-solid fa-xmark me-1"></i>';
    case "Hoàn thành":
      return '<i class="fa-regular fa-calendar-check me-1"></i>';
    default:
      return '<i class="fa-solid fa-circle-check me-1"></i>';
  }
}

//Lấy tên bác sĩ theo id
async function getNameById(id) {
  try {
    const response = await axiosJWT.get(`/api/Doctors/${id}`);
    const object = response.data;
    return object.hoTen; // Trả về họ tên
  } catch (error) {
    console.error("Lỗi không tìm được bác sĩ: ", error);
    return null; // Trả về null nếu có lỗi
  }
}

//Gọi API lấy ra Đánh giá theo LichKhamId
async function getRatings() {
  try {
    const response = await axiosJWT.get(`/api/v1/ServiceRatings`);
    const object = response.data;
    return object;
  } catch (error) {
    console.error("Lỗi không tìm được đánh giá: ", error);
    return null; // Trả về null nếu có lỗi
  }
}

//Hiển thị lịch khám theo benhNhanId
async function display1(dsLK) {
  // console.log(dsLK);
  // Hiển thị dữ liệu
  const container = $("#container_lk .row");
  container.empty(); // Xóa nội dung cũ

  // Tạo danh sách các Promise để lấy tên bác sĩ
  const doctorNamesPromises = dsLK.map((item) => getNameById(item.bacSiId));
  const doctorNames = await Promise.all(doctorNamesPromises); // Chờ tất cả Promise hoàn thành

  const response = await axiosJWT.get(`/api/Services`);
  const dsDichVu = response.data;
  // console.log(dsDichVu);

  //Lấy danh sách đánh giá
  const ratings = await getRatings();
  // console.log(ratings);

  // Kiểm tra dsDichVu có tồn tại và là một mảng không
  if (!Array.isArray(dsDichVu)) {
    console.error("dsDichVu không phải là mảng hợp lệ.");
  }
  dsLK.forEach((lichKham, index) => {
    // Lấy tên bệnh nhân từ mảng đã xử lý
    const doctorName = doctorNames[index];
    // // Định dạng hiển thị dd/MM/yyyy
    const dateString = lichKham.ngayKham;
    const date = new Date(dateString);
    const formattedDate = date.toLocaleDateString("en-GB"); // 'en-GB' chuẩn Anh (ngày/tháng/năm)
    // Xử lý trạng thái lịch khám
    const status = lichKham.trangThaiLichKham;

    //Có thể hoàn thành
    const completeDisabled = status === "Hoàn thành" ? "" : "disabled";

    //Xem kết quả khám
    let readDisabled = status === "Hoàn thành" ? "" : "disabled";
    readDisabled = status === "Đã hoàn thành" ? "" : "disabled";

    //Có thể hủy
    const editDisabled =
      status === "Đang xử lý" || status === "Đã đặt" ? "" : "disabled";

    //Có thể đánh giá
    const ratingDisabled =
      status === "Hoàn thành" || status === "Đã hoàn thành" ? "" : "disabled";

    //Hiển thị dịch vụ hoặc lý do hủy
    const readReason = status === "Đã hủy" ? "" : "d-none";
    const readService = status !== "Đã hủy" ? "" : "d-none";

    //Hiển thị dịch vụ
    // Tìm dịch vụ tương ứng với lichKham.dichVuId trong dsDichVu
    const service = dsDichVu.find(
      (dichVu) => dichVu.dichVuId === lichKham.dichVuId
    );

    // Tìm đánh giá tương ứng với lichKham.lichKhamId trong ratings
    const rating = ratings.find(
      (danhGia) => danhGia.lichKhamId === lichKham.lichKhamId
    );
    //Nếu có đánh giá thì hiển thị thì hiển thị là "Xem đánh giá"
    const ratingText = rating ? "Xem đánh giá" : "Đánh giá dịch vụ";
    //Lấy ra đánh giá id
    const ratingStr = rating
      ? btoa(encodeURIComponent(JSON.stringify(rating)))
      : "null";

    const serviceName = service ? service.tenDichVu : "Chưa xác định";
    const col = `
          <div class="col-md-4 mb-3">
              <div class="card custom-card h-100" lkId="${lichKham.lichKhamId}">
                  <div class="d-flex justify-content-between align-items-start">
                      <span class="badge rounded-pill ${getBadgeClass(status)}">
                         ${getIcon(status)} ${status}
                      </span>
                      <span class="dropdown">
                          <button class="optionButton btn btn-link dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                              <i class="fas fa-ellipsis-v"></i>
                          </button>
                          <ul class="dropdown-menu">
                              <li>
                                  <div class="dropdown-item ${completeDisabled}" data-bs-target="#modal-confirm-complete" data-bs-toggle="modal">
                                      <i class="fas fa-check me-2" style="color: rgb(28, 212, 37)"></i> Hoàn thành
                                  </div>
                              </li>
                              <li>
                                  <div id="optionViewResult" class="dropdown-item ${readDisabled}" data-bs-target="#modal-view-result" data-bs-toggle="modal">
                                      <i class="fas fa-eye me-2" style="color: rgb(28, 212, 212)"></i> Xem kết quả khám
                                  </div>
                              </li>
                              <li>
                                  <div id="optionRating" data-obj='${ratingStr}' class="dropdown-item ${ratingDisabled}" data-bs-toggle="modal" data-bs-target="#modal-rating-appointment">
                                      <i class="fa-solid fa-star me-2" style="color: rgb(255, 245, 65)"></i> ${ratingText}
                                  </div>
                              </li>
                              <li>
                                  <div id="optionViewStatus" class="dropdown-item" data-bs-toggle="modal" data-status='${status}' data-bs-target="#progressModal">
                                      <i class="fa-regular fa-calendar-check me-2" style="color: #6f42c1"></i> Xem trạng thái lịch khám
                                  </div>
                              </li>
                              <li>
                                  <div class="dropdown-item ${editDisabled}" data-bs-target="#modal-confirm-cancel" data-bs-toggle="modal">
                                      <i class="fa-solid fa-xmark me-2" style="color: rgb(255, 123, 0)"></i> Hủy
                                  </div>
                              </li>
                              <li>
                                  <div class=" dropdown-item" data-bs-target="#modal-confirm-delete" data-bs-toggle="modal">
                                      <i class="fas fa-trash-alt me-2" style="color: rgb(245, 76, 76)"></i> Xóa
                                  </div>
                              </li>
                          </ul>
                      </span>
                  </div>
                  <div class="mt-2">
                      <h5 class="mb-1">Bác sĩ: ${doctorName}</h5>
                      <p class="mb-1">Ngày khám: ${formattedDate}</p>
                      <p class="mb-1">Ca khám: ${lichKham.gioKham}</p>
                      <p class="mb-1 ${readReason}">Lý do hủy: ${
      lichKham.lyDo
    }</p>
                      <p class="mb-0 ${readService}">Dịch vụ: ${serviceName}</p>
                  </div>
              </div>
          </div>
      `;

    container.append(col);
  });
}

//Hàm xử lý khi nhấn nút "Xem trạng thái lịch khám"
function getViewStatus(status) {
  //xóa class .current ở các div có class .point
  $(".point").removeClass("current fs-4");

  //Thêm class .current vào các div có class .point theo trạng thái
  if (status === "Đang xử lý") {
    $(".processing").addClass("current fs-4");
  } else if (status === "Đã đặt") {
    $(".ordered").addClass("current fs-4");
  } else if (status === "Đã hủy") {
    $(".cancelled").addClass("current fs-4");
  } else if (status === "Hoàn thành") {
    $(".completed").addClass("current fs-4");
  } else {
    $(".finished").addClass("current fs-4");
  }
}
