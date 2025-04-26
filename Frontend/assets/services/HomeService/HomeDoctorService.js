let userId = localStorage.getItem("userId");
let doctorId = "";
$(document).ready(async function () {
  //Lấy bác sĩ Id từ userId
  await getDoctorByUserId();
//   console.log("userId:", doctorId);

  //Lấy thông tin phòng khám
  await getClinicInfo();

  //Hiển thị năm vào select
  populateYearSelect($("#yearSelect"));

  //Lấy thông tin thống kê lịch khám theo năm
  await getDataStatisticsByYear();

  //Lấy thông tin thống kê lịch khám theo trạng thái
  await getDataStatisticsByStatus();

  //Lấy thông tin đánh giá bác sĩ
  await getDataStatisticsRatingDoctor();

  //Lấy thông tin lịch khám theo ngày
  await getDataStatisticsByDate();
});

//Hàm lấy thông tin phòng khám
async function getClinicInfo() {
  try {
    // Gọi API để lấy thông tin phòng khám
    const response = await axiosJWT.get(`/api/v1/Homes/${doctorId}`);
    const clinicInfo = response.data;
    // console.log(clinicInfo);
    const sum = clinicInfo.sumAppointment;
    const suc = clinicInfo.sumAppointmentSuccess;
    const can = clinicInfo.sumAppointmentCancel;
    // Cập nhật thông tin lên giao diện
    $("#sumAppointment").text(sum);
    $("#sumAppointmentSuc").text(suc);
    $("#sumAppointmentSucPer").text(calculatePercentage(suc, sum) + "%");
    $("#sumAppointmentCan").text(can);
    $("#sumAppointmentCanPer").text(calculatePercentage(can, sum) + "%");
    $("#sumPatient").text(clinicInfo.sumPatient);
  } catch (error) {
    // Log lỗi nếu có lỗi kết nối
    console.error("Lỗi kết nối:", error);
  }
}

// Hàm tính tỷ lệ phần trăm
function calculatePercentage(part, total) {
  if (total === 0) {
    return 0; // Tránh chia cho 0
  }
  const percentage = (part / total) * 100;
  return Math.round(percentage * 100) / 100; // Làm tròn đến 2 chữ số thập phân
}

// Hàm hiển thị thông tin select option theo năm
async function populateYearSelect(selectElement) {
  // Lấy năm hiện tại
  const currentYear = new Date().getFullYear();

  // Tạo các năm cần thiết
  const years = [currentYear, currentYear - 1, currentYear - 2];

  // Xóa các option cũ (nếu có)
  selectElement.empty();

  // Duyệt qua mảng các năm và thêm chúng vào select
  years.forEach((year) => {
    const option = $("<option></option>")
      .val(year) // Set giá trị option
      .text(year); // Hiển thị năm
    selectElement.append(option); // Thêm option vào select
  });

  // Chọn mặc định là năm hiện tại
  selectElement.val(currentYear);
}

// Hàm lấy thông tin thống kê lịch khám theo năm
async function getDataStatisticsByYear() {
  try {
    // Gọi API để lấy thông tin thống kê lịch khám theo năm
    const response = await axiosJWT.get(
      `/api/v1/Homes/statisticByYear/${doctorId}`
    );
    const statisticsByYear = response.data;
    // console.log(statisticsByYear);
    displayStatistics(statisticsByYear); // Hiển thị dữ liệu lên bảng
  } catch (error) {
    // Log lỗi nếu có lỗi kết nối
    console.error("Lỗi không tìm được:", error);
  }
}

//Ham hiển thị thông tin thống kê lịch khám theo năm
function displayStatistics(data) {
  // Dữ liệu mẫu cho các năm
  const dataByYear = {};
  // Chuyển dữ liệu API thành một đối tượng {year: [data]}
  data.forEach((item) => {
    dataByYear[item.nam] = item.soLuongTheoThang;
  });
  const yearSelect = $("#yearSelect").val();

  // Khởi tạo biểu đồ
  const ctx = $("#chartStatistics")[0].getContext("2d");
  const chart = new Chart(ctx, {
    type: "line",
    data: {
      labels: [
        "Tháng 1",
        "Tháng 2",
        "Tháng 3",
        "Tháng 4",
        "Tháng 5",
        "Tháng 6",
        "Tháng 7",
        "Tháng 8",
        "Tháng 9",
        "Tháng 10",
        "Tháng 11",
        "Tháng 12",
      ],
      datasets: [
        {
          label: "Lịch khám",
          data: dataByYear[yearSelect] || [], // Dữ liệu mặc định là năm 2025
          borderColor: "rgba(75, 192, 192, 1)",
          backgroundColor: "rgba(75, 192, 192, 0.2)",
          borderWidth: 2,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: "top",
        },
      },
    },
  });

  // Lắng nghe sự kiện thay đổi năm
  $("#yearSelect").on("change", function () {
    const selectedYear = $(this).val(); // Lấy giá trị năm được chọn
    chart.data.datasets[0].data = dataByYear[selectedYear] || []; // Cập nhật dữ liệu
    chart.update(); // Cập nhật biểu đồ
  });
}

// Hàm lấy thông tin thống kê lịch khám theo trạng thái
async function getDataStatisticsByStatus() {
  try {
    // Gọi API để lấy thông tin thống kê lịch khám theo trạng thái
    const response = await axiosJWT.get(
      `/api/v1/Homes/statisticByStatus/${doctorId}`
    );
    const statisticsByStatus = response.data;
    displayStatisticsByStatus(statisticsByStatus); // Hiển thị dữ liệu lên bảng
  } catch (error) {
    // Log lỗi nếu có lỗi kết nối
    console.error("Lỗi không tìm được:", error);
  }
}

// Hàm hiển thị thông tin thống kê lịch khám theo trạng thái
function displayStatisticsByStatus(data) {
  // Dữ liệu mẫu
  const statuses = [
    "Đang xử lý",
    "Đã đặt",
    "Hoàn thành",
    "Đã hoàn thành",
    "Đã hủy",
  ];
  // Khởi tạo số lượng ban đầu là 0 cho tất cả trạng thái
  const counts = statuses.map((status) => {
    // Tìm trạng thái trong dữ liệu API
    const match = data.find((item) => item.trangThai === status);
    return match ? match.soLuong : 0;
  });

  // Khởi tạo biểu đồ
  const ctx = $("#chartAppointmentStatus")[0].getContext("2d");
  const chart = new Chart(ctx, {
    type: "bar", // Biểu đồ thanh ngang
    data: {
      labels: statuses,
      datasets: [
        {
          label: "Số lượng lịch khám",
          data: counts,
          backgroundColor: [
            "rgba(255, 159, 64, 0.6)", // Đang xử lý
            "rgba(54, 162, 235, 0.6)", // Đã đặt
            "rgba(75, 192, 192, 0.6)", // Hoàn thành
            "rgba(153, 102, 255, 0.6)", // Đã hoàn thành
            "rgba(255, 99, 132, 0.6)", // Đã hủy
          ],
          borderColor: [
            "rgba(255, 159, 64, 1)",
            "rgba(54, 162, 235, 1)",
            "rgba(75, 192, 192, 1)",
            "rgba(153, 102, 255, 1)",
            "rgba(255, 99, 132, 1)",
          ],
          borderWidth: 1,
        },
      ],
    },
    options: {
      indexAxis: "y",
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: "top",
        },
      },
      scales: {
        x: {
          beginAtZero: true,
        },
      },
    },
  });
}

// Hàm lấy thông tin thống kê lịch khám theo ngày
async function getDataStatisticsByDate() {
  try {
    // Gọi API để lấy thông tin thống kê lịch khám theo ngày
    const response = await axiosJWT.get(
      `/api/v1/Homes/appointment/${doctorId}`
    );
    const statisticsByDate = response.data;
    // console.log(statisticsByDate);
    displayStatisticsByDate(statisticsByDate); // Hiển thị dữ liệu lên bảng
  } catch (error) {
    // Log lỗi nếu có lỗi kết nối
    console.error("Lỗi không tìm được:", error);
  }
}

// Hàm hiển thị thông tin thống kê lịch khám theo ngày
function displayStatisticsByDate(data) {
  let tbody = $("#appointmentTable tbody");
  tbody.empty(); // Xóa dữ liệu cũ trước khi thêm mới

  data.forEach((item, index) => {
    // // Định dạng hiển thị dd/MM/yyyy
    const dateString = item.ngayKham;
    const date = new Date(dateString);
    const formattedDate = date.toLocaleDateString("en-GB"); // 'en-GB' chuẩn Anh (ngày/tháng/năm)
    // Xử lý màu badge
    const status = item.trangThaiLichKham;
    let bgColor = "";
    if (status === "Đã hủy") {
      bgColor = "bg-danger";
    } else if (status === "Đang xử lý") {
      bgColor = "bg-warning";
    } else if (status === "Đã đặt") {
      bgColor = "bg-primary";
    } else {
      bgColor = "bg-success";
    }
    const row = `
          <tr>
              <td class="align-middle">${index + 1}</td>
              <td class="align-middle">${item.maBenhNhan}</td>
              <td class="align-middle">${item.tenBenhNhan}</td>
              <td class="align-middle">${formattedDate}</td>
              <td class="align-middle">${item.gioKham}</td>
              <td class="align-middle">${item.tenDichVu}</td>
              <td class="align-middle"><span class="badge rounded-pill ${bgColor}">${
      item.trangThaiLichKham
    }</span></td>
          </tr>`;
    tbody.append(row);
  });
}

// Hàm lấy thông tin bác sĩ theo userId
async function getDoctorByUserId() {
  try {
    // Gọi API để lấy thông tin bác sĩ theo userId
    const response = await axiosJWT.get(`/api/Doctors/getbyuserid/${userId}`);
    const doctorInfo = response.data;
    doctorId = doctorInfo.bacSiId;
  } catch (error) {
    // Log lỗi nếu có lỗi kết nối
    console.error("Lỗi không tìm được:", error);
  }
}

// Hàm lấy thông tin đánh giá bác sĩ
async function getDataStatisticsRatingDoctor() {
  try {
    // Gọi API để lấy thông tin đánh giá bác sĩ
    const response = await axiosJWT.get(
      `/api/v1/Homes/statisticRating/${doctorId}`
    );
    const ratingData = response.data;
    // console.log(ratingData);
    displayRatingStatistics(ratingData); // Hiển thị dữ liệu lên bảng
  } catch (error) {
    // Log lỗi nếu có lỗi kết nối
    console.error("Lỗi không tìm được:", error);
  }
}

// Hàm hiển thị thông tin đánh giá bác sĩ
function displayRatingStatistics(data) {
  // Dữ liệu mẫu
  const ratings = data.map((item) => item.danhGia); // Lấy danh sách đánh giá từ dữ liệu API
  const percentages = data.map((item) => item.phanTram); // Tỷ lệ phần trăm

  // Khởi tạo biểu đồ
  const ctx = $("#rating")[0].getContext("2d");
  const chart = new Chart(ctx, {
    type: "doughnut", // Loại biểu đồ: hình bánh rỗng
    data: {
      labels: ratings, // Tên các dịch vụ
      datasets: [
        {
          data: percentages, // Tỷ lệ phần trăm
          backgroundColor: [
            "rgba(255, 99, 132, 0.6)", // Màu sắc cho từng phần
            "rgba(54, 162, 235, 0.6)",
            "rgba(255, 206, 86, 0.6)",
            "rgba(75, 192, 192, 0.6)",
            "rgba(153, 102, 255, 0.6)",
          ],
          borderColor: [
            "rgba(255, 99, 132, 1)",
            "rgba(54, 162, 235, 1)",
            "rgba(255, 206, 86, 1)",
            "rgba(75, 192, 192, 1)",
            "rgba(153, 102, 255, 1)",
          ],
          borderWidth: 1,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: "top", // Vị trí của chú thích
        },
        tooltip: {
          callbacks: {
            label: function (tooltipItem) {
              const label = ratings[tooltipItem.dataIndex];
              const value = percentages[tooltipItem.dataIndex];
              return `${label}: ${value}%`;
            },
          },
        },
      },
    },
  });
}
