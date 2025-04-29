let results = []; // Biến lưu trữ toàn bộ danh sách kết quả
var bsId;
var tenBN;
var prescriptions = [];
var notes = [];

$(document).ready(async function () {
  await getDoctorId();

  loadResults(); // Tải danh sách kết quả khi trang được load

  // Sự kiện chỉnh sửa kết quả khám
  $(document).on("click", ".m-edit", async function () {
    const resultId = $(this).data("resultId");
    const result = results.find((s) => s.ketQuaKhamId === resultId);
    if (!result) {
      showErrorPopup("Không tìm thấy kết quả để chỉnh sửa.");
      return;
    }
    const lkId = result.lichKhamId;
    // 👉 Gọi API để lấy bệnh nhân ID từ lịch khám
    const bnId = await getLichKhamById(lkId);
    // 👉 Gọi API để lấy tiền sử bệnh lý từ bệnh nhân ID
    const tienSu = await getBenhNhanById(bnId);

    // Đổ dữ liệu vào modal chỉnh sửa
    const chanDoan = $("#dialog-edit #diagnose");
    const chiDinhThuoc = $("#dialog-edit #prescription");
    const ghiChu = $("#dialog-edit #note");
    const tienSuBenhLy = $("#dialog-edit #medicalHistory");

    tienSuBenhLy.val(tienSu); // Đổ tiền sử bệnh
    chanDoan.val(result.chanDoan);
    chiDinhThuoc.val("");
    ghiChu.val("");
    const prescriptionStr = result.chiDinhThuoc;
    const noteStr = result.ghiChu;

    //Tách prescriptionStr, noteStr và gắn vào mảng
    prescriptions = prescriptionStr.split(",").map((x) => x.trim());
    notes = noteStr.split(";").map((x) => x.trim());
    //Điền dữ liệu vào table từ mảng
    $("#resultTable tbody").empty();

    // Hiển thị từng dòng lên table
    if (prescriptions.length === notes.length) {
      for (let i = 0; i < prescriptions.length; i++) {
        const newRow = `
              <tr>
                <td><input type="checkbox" class="row-check" /></td>
                <td>${i + 1}</td>
                <td>${prescriptions[i]}</td>
                <td>${notes[i] || ""}</td>
              </tr>
            `;
        $("#resultTable tbody").append(newRow);
      }
    }

    // Xử lý sự kiện sửa
    $("#btnEdit")
      .off("click")
      .on("click", function () {
        // console.log("Đang xử lý sửa...");
        const chanDoanStr = chanDoan.val();
        // Cập nhật lại mảng từ bảng để đảm bảo dữ liệu mới nhất
        prescriptions = [];
        notes = [];

        $("#resultTable tbody tr").each(function () {
          const pres = $(this).find("td:eq(2)").text().trim();
          const note = $(this).find("td:eq(3)").text().trim();
          prescriptions.push(pres);
          notes.push(note);
        });
        // Gộp lại thành chuỗi mới
        const chiDinhThuocStr = prescriptions.join(", ");
        const ghiChuStr = notes.join("; ");

        // Kiểm tra các trường dữ liệu
        if (!chanDoanStr) {
          showErrorPopup(
            "Sửa không thành công: Chẩn đoán không được để trống!"
          );
          return;
        }
        const updatedResult = {
          ketQuaKhamId: resultId,
          lichKhamId: lkId,
          chanDoan: chanDoanStr,
          chiDinhThuoc: chiDinhThuocStr,
          ghiChu: ghiChuStr,
        };
        // console.log(updatedResult);

        axiosJWT
          .put(`/api/Results/${resultId}`, updatedResult)
          .then((response) => {
            console.log("Dữ liệu đã được cập nhật: ", response.data); // Lấy dữ liệu từ response
            // 👉 Gọi cập nhật bệnh nhân
            updatePatient(bnId);
            loadResults(); // Tải lại danh sách
            $("#dialog-edit").modal("hide"); // Đóng modal
          })
          .catch((error) => {
            console.error("Lỗi khi chỉnh sửa kết quả:", error);
            // showErrorPopup("Sửa không thành công: Đã xảy ra lỗi từ server!");
          });
      });
  });

  //Tạo sự kiện khi nhấn nút thêm, xóa chỉ định thuốc và ghi chú
  //Sự kiện thêm
  $("#btnAddRow").on("click", function () {
    const prescription = $("#prescription").val().trim();
    const note = $("#note").val().trim();

    if (prescription || note) {
      prescriptions.push(prescription);
      notes.push(note);

      const index = $("#resultTable tbody tr").length + 1; // Đếm số dòng hiện tại

      const newRow = `
        <tr>
          <td><input type="checkbox" class="row-check" /></td>
          <td>${index}</td>
          <td>${prescription}</td>
          <td>${note}</td>
        </tr>
      `;
      $("#resultTable tbody").append(newRow);

      // Clear input
      $("#prescription").val("");
      $("#note").val("");
    }
  });
  //Sự kiện xóa
  $("#btnDeleteRow").on("click", function () {
    // Duyệt ngược để tránh lỗi index khi splice
    $("#resultTable tbody tr").each(function (index) {
      if ($(this).find(".row-check").is(":checked")) {
        $(this).remove();
      }
    });

    // Cập nhật lại mảng prescriptions và notes
    prescriptions = [];
    notes = [];
    $("#resultTable tbody tr").each(function () {
      const pres = $(this).find("td:eq(2)").text();
      const note = $(this).find("td:eq(3)").text();
      prescriptions.push(pres);
      notes.push(note);
    });

    // Cập nhật lại STT
    $("#resultTable tbody tr").each(function (i) {
      $(this)
        .find("td:eq(1)")
        .text(i + 1); // Cột STT
    });
  });
  //Tạo sự kiện khi nhấn CheckAll
  $("#checkAll").on("change", function () {
    $(".row-check").prop("checked", this.checked);
  });
  //Tạo sự kiện khi nhấn 1 hàng thì dữ liệu điền vào ô input
  $(document).on("click", "#resultTable tbody tr", function () {
    const prescription = $(this).find("td:eq(2)").text();
    const note = $(this).find("td:eq(3)").text();

    $("#prescription").val(prescription);
    $("#note").val(note);
  });
});

// ✅ Hàm async để lấy bệnh nhân ID từ lịch khám
async function getLichKhamById(lichKhamId) {
  try {
    const res = await axiosJWT.get(`/api/v1/Appointments/${lichKhamId}`);
    return res.data.benhNhanId;
  } catch (err) {
    console.error("Lỗi khi lấy thông tin lịch khám:", err);
    return null;
  }
}

// ✅ Hàm async để lấy tiền sử bệnh lý từ bệnh nhân
async function getBenhNhanById(benhNhanId) {
  try {
    const res = await axiosJWT.get(`/api/Patients/${benhNhanId}`);
    return res.data.tienSuBenhLy;
  } catch (err) {
    console.error("Lỗi khi lấy thông tin bệnh nhân:", err);
    return "";
  }
}

// ✅ Cập nhật thông tin bệnh nhân
function updatePatient(bnId) {
  const patient = {
    benhNhanId: bnId,
    maBenhNhan: "", // Nếu cần giữ nguyên thì backend giữ lại
    hoTen: "", // Tương tự
    tienSuBenhLy: $("#dialog-edit #medicalHistory").val(),
  };
  axiosJWT
    .put(`/api/Patients/${bnId}`, patient)
    .then((res1) => {
      console.log("Dữ liệu bệnh nhân đã được cập nhật: ", res1.data);
    })
    .catch((err1) => {
      console.error("Lỗi khi cập nhật thông tin bệnh nhân:", err1);
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

async function getTenBenhNhan(lichKhamId) {
  try {
    // Gọi API lấy tên bệnh nhân từ lichKhamId
    const response = await axiosJWT.get(
      `/api/Results/tenbenhnhan/${lichKhamId}`
    );
    return response.data; // Giả sử API trả về trực tiếp tên bệnh nhân
  } catch (error) {
    console.error("Lỗi khi lấy tên bệnh nhân:", error);
    return "Không có tên bệnh nhân"; // Trả về giá trị mặc định nếu có lỗi
  }
}

// Hàm tải danh sách dịch vụ
function loadResults() {
  axiosJWT
    .get(`/api/Results/doctor/${bsId}`)
    .then((response) => {
      results = response.data;
      // console.log(results);
      $(".preloader").removeClass("d-none");
      $(".preloader").addClass("d-block");
      displayResults(results); // Hiển thị danh sách kết quả
      $(".preloader").removeClass("d-block");
      $(".preloader").addClass("d-none");
    })
    .catch((error) => {
      console.error("Lỗi khi tải danh sách kết quả:", error);
    });
}

// Hàm hiển thị danh sách kết quả
async function displayResults(results) {
  const resultTableBody = $("#tblData"); // Xác định phần tbody của bảng
  resultTableBody.empty(); // Xóa nội dung cũ trước khi thêm mới

  if (results.length === 0) {
    resultTableBody.append(
      '<tr><td colspan="9">Không có kết quả khám nào.</td></tr>'
    ); // Hiển thị thông báo nếu không có dữ liệu
    return;
  }

  // Lặp qua danh sách kết quả và tạo từng dòng
  // Lặp qua danh sách kết quả và tạo từng dòng
  for (const [index, result] of results.entries()) {
    let tenBenhNhan = "Đang tải..."; // Giá trị mặc định trong khi đợi API trả về

    try {
      tenBenhNhan = await getTenBenhNhan(result.lichKhamId); // Gọi API lấy tên bệnh nhân
    } catch (error) {
      console.error("Lỗi khi lấy tên bệnh nhân:", error);
    }

    const resultRow = `
            <tr>
                <td style="display: none">${result.ketQuaKhamId}</td>
                <td>${index + 1}</td>
                <td>${tenBenhNhan}</td> <!-- Thêm cột tên bệnh nhân -->
                <td>${formatDate(result.ngayTao)}</td>
                <td>${result.chanDoan || "Không có chẩn đoán"}</td>
                <td>${result.chiDinhThuoc || "Không có chỉ định thuốc"}</td>
                <td>
                    <div class="m-table-tool">
                        <div class="m-edit m-tool-icon" data-result-id="${
                          result.ketQuaKhamId
                        }" data-bs-toggle="modal" data-bs-target="#dialog-edit">
                            <i class="fas fa-edit text-primary"></i>
                        </div>
                    </div>
                </td>
            </tr>
        `;
    resultTableBody.append(resultRow); // Thêm dòng vào bảng
  }
}

// Hàm formatDate (giả định rằng bạn có một hàm này để định dạng ngày tháng)
function formatDate(date) {
  if (!date) return "";
  const d = new Date(date);
  return d.toLocaleDateString("vi-VN"); // Định dạng ngày theo kiểu Việt Nam (dd/mm/yyyy)
}

// Hàm định dạng ngày (nếu ngày không null)
function formatDate(dateString) {
  if (!dateString) return "Không có dữ liệu";
  const date = new Date(dateString);
  return date.toLocaleDateString("vi-VN"); // Định dạng theo ngày Việt Nam
}
