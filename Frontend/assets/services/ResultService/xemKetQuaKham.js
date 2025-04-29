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
  doc.setFontSize(12);
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
  doc.line(115, 29, 173, 29);

  doc.setFontSize(14);
  doc.text("PHIẾU KẾT QUẢ KHÁM BỆNH", 70, 43);

  // --- Thông tin bệnh nhân ---
  let y = 56;
  doc.setFontSize(12);
  doc.setFont("NotoSerif-Light", "normal");
  doc.text("THÔNG TIN BỆNH NHÂN", 40, y);
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
    135,
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
  // Hiển thị tiêu đề
  y += 10;
  y = drawSection(
    doc,
    "1. Tiền sử bệnh lý:",
    result.benhNhan?.tienSuBenhLy,
    30,
    y
  );

  y += 10;
  y = drawSection(doc, "2. Chẩn đoán:", result.chanDoan, 30, y);
  y += 10;
  doc.setFont("NotoSerif-SemiBold-Bold", "bold");
  doc.text("3. Chỉ định:", 30, y);

  // Tách chiDinhThuoc và ghiChu thành mảng
  const chiDinhThuoc = result.chiDinhThuoc
    ? result.chiDinhThuoc.split(",")
    : [];
  const ghiChu = result.ghiChu ? result.ghiChu.split(";") : [];

  // Dữ liệu bảng
  const tableData = chiDinhThuoc.map((thuoc, index) => [
    index + 1,
    thuoc.trim(),
    ghiChu[index] ? ghiChu[index].trim() : "Không có ghi chú",
  ]);
  // Tạo bảng cách tiêu đề 10px
  doc.autoTable({
    startY: y + 5, // cách tiêu đề đúng 10px
    head: [["STT", "Chỉ định thuốc", "Ghi chú"]],
    body: tableData,
    theme: "grid",
    styles: {
      font: "NotoSerif-Light", // font nhẹ cho nội dung bảng
      fontStyle: "normal",
      fontSize: 12,
      cellPadding: 2, // padding nhỏ để mỗi dòng khoảng 10px
      minCellHeight: 10, // dòng cao đúng 10px
      lineWidth: 0.005, // đường viền mỏng
      lineColor: [200, 200, 200],
    },
    headStyles: {
      fillColor: [255, 255, 255],
      textColor: [0, 0, 0],
      font: "NotoSerif-SemiBold-Bold",
      fontStyle: "bold",
      lineWidth: 0.005, // đường viền mỏng
      lineColor: [200, 200, 200],
      halign: "center",
    },
  });

  // --- Bác sĩ ký tên ---
  doc.setFont("NotoSerif-SemiBold-Bold", "bold");

  // Lấy vị trí Y kết thúc của bảng
  let finalY = doc.lastAutoTable.finalY + 10; // +20 để có khoảng cách thoáng

  // Kiểm tra xem phần ký tên có vượt quá chiều cao của trang không
  if (finalY + 40 > doc.internal.pageSize.height) {
    doc.addPage(); // Thêm trang mới nếu phần ký tên sẽ vượt quá trang hiện tại
    finalY = 30; // Cập nhật lại vị trí Y trên trang mới
  }

  doc.text("Bác sĩ khám bệnh", 140, finalY);
  doc.setFont("NotoSerif-Light-Italic", "italic");
  doc.setFontSize(8);
  doc.text("(Ký và ghi rõ họ tên)", 148, finalY + 7);
  doc.setFont("NotoSerif-Light", "normal");
  doc.setFontSize(12);
  doc.text(`${safeText(result.bacSi?.hoTen)}`, 144, finalY + 35);

  // --- Hiển thị PDF ---
  window.open(doc.output("bloburl"), "_blank");
}

//Hàm hiển thị phần nội dung khám
function drawSection(doc, title, content, x, yStart) {
  const safeText = (text) => (text ? text : "Không có");
  // Hiển thị tiêu đề
  doc.setFont("NotoSerif-SemiBold-Bold", "bold");
  doc.text(title, x, yStart);

  // Hiển thị nội dung, cách tiêu đề 10px
  doc.setFont("NotoSerif-Light", "normal");
  const lines = doc.splitTextToSize(safeText(content), 160);
  const lineHeight = 10;

  for (let i = 0; i < lines.length; i++) {
    doc.text(lines[i], x, yStart + 10 + i * lineHeight);
  }

  // Trả về y tiếp theo để dùng cho mục sau
  return yStart + lines.length * lineHeight;
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
