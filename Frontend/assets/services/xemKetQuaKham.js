let results = []; // Biến lưu trữ toàn bộ danh sách kết quả
var bnId;
var bn;
$(document).ready(async function () {
  await getBNId();
  // console.error('bnId đã lấy được là ', bnId);

  loadResults(); // Tải danh sách kết quả khi trang được load

  $(document).on("click", ".btnXemKQKham", function () {
    const ketQuaKhamId = $(this).data("id");
    // console.log(ketQuaKhamId);
    generateMedicalExaminationForm(ketQuaKhamId);
  });
});

// Hàm tạo phiếu khám
function generateMedicalExaminationForm(ketQuaKhamId) {
  const result = results.find((item) => item.ketQuaKhamId === ketQuaKhamId);
  if (!result) {
    alert("Không tìm thấy kết quả khám!");
    return;
  }

  const safeText = (text) => (text ? text : "Không có");

  const { jsPDF } = window.jspdf;
  const doc = new jsPDF();

  //Đăng ký font
  doc.addFileToVFS("NotoSerif-Light-normal.ttf", NotoSerifLight);
  doc.addFont("NotoSerif-Light-normal.ttf", "NotoSerif-Light", "normal");
  doc.addFileToVFS("NotoSerif-Light-italic.ttf", NotoSerifLightItalic);
  doc.addFont("NotoSerif-Light-italic.ttf", "NotoSerif-Light-Italic", "italic");
  doc.addFileToVFS("NotoSerif-SemiBold-bold.ttf", NotoSerifSemiBold);
  doc.addFont("NotoSerif-SemiBold-bold.ttf", "NotoSerif-SemiBold-Bold", "bold");

  // --- Tiêu đề ---
  doc.setFontSize(13);
  doc.setFont("NotoSerif-SemiBold-Bold", "bold");
  doc.text("Phòng khám Đa khoa Quang Việt", 20, 20);
  doc.text("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM", 100, 20);

  doc.setFont("NotoSerif-Light", "normal");
  doc.text("Phòng số: ...", 40, 27);
  // Kẻ dòng bên dưới
  doc.setLineWidth(0.05);
  doc.setDrawColor(0); // màu đen
  doc.line(40, 29, 65, 29);

  doc.setFont("NotoSerif-SemiBold-Bold", "bold");
  doc.text("Độc lập - Tự do - Hạnh phúc", 115, 27);
  // Kẻ dòng bên dưới
  doc.setLineWidth(0.05);
  doc.setDrawColor(0); // màu đen
  doc.line(115, 29, 178, 29);

  doc.setFontSize(15);
  doc.text("PHIẾU KẾT QUẢ KHÁM BỆNH", 70, 45);

  // --- Thông tin bệnh nhân ---
  let y = 60;
  doc.setFontSize(14);
  doc.setFont("NotoSerif-Light", "normal");
  doc.text("THÔNG TIN BỆNH NHÂN", 40, y);
  doc.setFontSize(13);
  // Mã bệnh nhân
  y += 10;
  printBoldNormal(
    doc,
    "Mã bệnh nhân:",
    safeText(result.benhNhan?.maBenhNhan),
    30,
    y
  );
  y += 10;
  printBoldNormal(doc, "Họ tên:", safeText(result.benhNhan?.hoTen), 30, y);
  printBoldNormal(
    doc,
    "Ngày sinh:",
    formatDate(result.benhNhan?.ngaySinh),
    100,
    y
  );
  printBoldNormal(
    doc,
    "Giới tính:",
    safeText(result.benhNhan?.gioiTinh),
    160,
    y
  );
  y += 10;
  printBoldNormal(doc, "Địa chỉ:", safeText(result.benhNhan?.diaChi), 30, y);

  y += 10;
  printBoldNormal(doc, "Email:", safeText(result.benhNhan?.email), 30, y);

  printBoldNormal(
    doc,
    "Số điện thoại:",
    safeText(result.benhNhan?.soDienThoai),
    120,
    y
  );

  // --- Nội dung khám ---
  y += 15;
  doc.text("NỘI DUNG KHÁM", 40, y);
  y += 10;
  printBoldNormal(doc, "Ngày khám:", formatDate(result.ngayKham), 30, y);
  printBoldNormal(doc, "Giờ khám:", safeText(result.gioKham), 100, y);
  y += 10;
  printBoldNormal(
    doc,
    "Dịch vụ khám:",
    safeText(result.dichVu?.tenDichVu),
    30,
    y
  );
  y += 10;
  y = drawMultilineSection(
    "1. Tiền sử bệnh lý:",
    safeText(result.benhNhan?.tienSuBenhLy),
    30,
    y,
    doc
  );
  y = drawMultilineSection(
    "2. Chẩn đoán:",
    safeText(result.chanDoan),
    30,
    y,
    doc
  );
  y = drawMultilineSection(
    "3. Chỉ định:",
    safeText(result.chiDinhThuoc),
    30,
    y,
    doc
  );

  // --- Chỉ định thuốc (nếu có thể dùng sau) ---
  // Bỏ autoTable nếu chưa cần

  // --- Bác sĩ ký tên ---
  doc.setFont("NotoSerif-SemiBold-Bold", "bold");
  let finalY = y + 10;
  doc.text("Bác sĩ khám bệnh", 140, finalY);
  doc.setFont("NotoSerif-Light-Italic", "italic");
  doc.setFontSize(8);
  doc.text("(Ký và ghi rõ họ tên)", 148, finalY + 7);
  doc.setFont("NotoSerif-Light", "normal");
  doc.setFontSize(14);
  doc.text(`${safeText(result.bacSi?.hoTen)}`, 143, finalY + 35);

  // --- Hiển thị PDF ---
  window.open(doc.output("bloburl"), "_blank");
}

//Hàm hiển thị phần nội dung khám
function drawMultilineSection(title, content, x, yStart, doc) {
  doc.setFont("NotoSerif-SemiBold-Bold", "bold");
  doc.text(title, x, yStart);

  doc.setFont("NotoSerif-Light", "normal");
  const lines = doc.splitTextToSize(content, 160);
  doc.text(lines, x, yStart + 8); // nội dung cách tiêu đề 8px

  // Trả về y tiếp theo để dùng cho mục sau
  return yStart + lines.length * 8 + 10; // 7 là chiều cao 1 dòng, 10 là khoảng cách giữa các mục
}

//Hàm hiển thị tiêu đề + nội dung
function printBoldNormal(doc, title, content, x, y) {
  // Font đậm cho tiêu đề
  doc.setFont("NotoSerif-SemiBold-Bold", "bold");
  doc.text(title, x, y);

  // Tính độ rộng tiêu đề
  const titleWidth = doc.getTextWidth(title);

  // Font thường cho nội dung
  doc.setFont("NotoSerif-Light", "normal");
  doc.text(content, x + titleWidth + 2, y); // Cách tiêu đề 2px
}

async function getBNId() {
  try {
    let userId = localStorage.getItem("userId");
    const response = await axiosJWT.get(`/api/Patients/getbyuserid/${userId}`);
    bnId = response.data.benhNhanId; // Lấy giá trị ID bác sĩ
    // console.error('bnId đã lấy được là ', bnId);
  } catch (error) {
    console.error("Lỗi khi gọi API:", error);
  }
}

function loadResults() {
  axiosJWT
    .get(`api/Results/ketquakham/${bnId}`)
    .then((response) => {
      results = response.data;
      // console.log('ket qua : ', results);
      displayResults(results); // Hiển thị danh sách kết quả
    })
    .catch((error) => {
      console.error("Lỗi khi tải danh sách kết quả:", error);
    });
}

function getData() {
  var userId = localStorage.getItem("userId");
  console.log(userId);
  // $('#hotenHeader').text(localStorage.getItem(loggedInUsername));
  axiosJWT
    .get(`/api/Patients/getbyuserid/${userId}`)
    .then(function (response) {
      bn = response.data;
      display();
      getAvata();
    })
    .catch(function (error) {
      showErrorPopup();
      console.error("Lỗi không tìm được:", error);
    });
}

function display() {
  console.log(bn);
  $("#hotenHeader").text(bn.hoTen);
  // document.getElementById('uploadedImage').src = "http://localhost:37649" + response.data.fileUrl;
  if (bn.hinhAnh != null) {
    $("#uploadedImage").attr("src", "http://localhost:37649" + bn.hinhAnh);
  }
}

// Hàm hiển thị danh sách kết quả
function displayResults(results) {
  const resultTableBody = $("#resultList"); // Xác định phần tbody của bảng
  resultTableBody.empty(); // Xóa nội dung cũ trước khi thêm mới

  if (results.length === 0) {
    resultTableBody.append(
      '<tr><td colspan="9">Không có kết quả khám nào.</td></tr>'
    ); // Hiển thị thông báo nếu không có dữ liệu
    return;
  }

  console.log(results);
  // Lặp qua danh sách kết quả và tạo từng dòng
  results.forEach((result, index) => {
    const resultRow = `
            <tr>
                <td>${formatDate(result.ngayKham)}</td>           
                <td>${
                  result.dichVu ? result.dichVu.tenDichVu : "Không có dịch vụ"
                }</td>
                <td>${result.chanDoan || "Không có chẩn đoán"}</td>
                <td>${result.chiDinhThuoc || "Không có chỉ định thuốc"}</td>
                <td>${result.ghiChu || "Không có ghi chú"}</td>
                <td><button class="btnXemKQKham btn btn-outline-primary" data-id="${
                  result.ketQuaKhamId
                }">Xem thêm</button></td>
            </tr>
        `;
    resultTableBody.append(resultRow); // Thêm dòng vào bảng
    // console.log("abc", result)
  });
}
// Hàm định dạng ngày (nếu ngày không null)
function formatDate(dateString) {
  if (!dateString) return "Không có";
  const date = new Date(dateString);
  return date.toLocaleDateString("vi-VN"); // Định dạng theo ngày Việt Nam
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
