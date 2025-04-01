var dsBN;
var bnID = "";
$(document).ready(function () {
    getData();

    $("#refresh-data").click(function(){
        getData();
      });
    // Gắn sự kiện cho nút hiển thị modal sửa
    $(document).on("click", ".m-edit", function () {
        // const benhNhanId = $(this).data("id"); // Lấy id từ thuộc tính data-id
        const benhNhanId = $(this).closest("tr").attr("bn-id");
        bnID = benhNhanId;
        const benhNhan = dsBN.find((bn) => bn.benhNhanId === benhNhanId); // Tìm bệnh nhân trong danh sách

        if (benhNhan) {
            fillEditModal(benhNhan); // Hiển thị thông tin lên modal
            console.log($("#gender").val());
        } else {
            console.error("Không tìm thấy thông tin bệnh nhân!");
        }
    });

    // Gắn sự kiện cho nút hiển thị modal Thêm
    $("#btnOpenModalAdd").click(function () {
        let maBNNext = getMaxBenhNhanCode(dsBN);
        $("#mabn-add").val(maBNNext);
    })
    // Gắn sự kiện cho nút Thêm
    $("#btnAdd").click(function () {
        const genderValue = parseInt($("#gender-add").val());
        const newPatient = {
            maBenhNhan:  $("#mabn-add").val(),
            hoTen: $("#hoten-add").val(),
            email: $("#email-add").val() || null,
            soDienThoai: $("#sdt-add").val() || null,
            loaiGioiTinh: genderValue,
            ngaySinh: $("#ngaysinh-add").val() ? $("#ngaysinh-add").val() + "T00:00:00" : null,
            diaChi: $("#diachi-add").val() || null,
            tienSuBenhLy: $("#tiensubenhly-add").val() || null,
        };
        console.log("Dữ liệu thêm:", newPatient);

        // Gửi yêu cầu add tới API
        axiosJWT
            .post(`/api/Patients`, newPatient)
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
        const genderValue = parseInt($("#gender-edit").val());
        // let ngaySinhValue = null;
        // if($("#ngaysinh").val() != null){
        //     ngaySinhValue = $("#ngaysinh").val() + "T00:00:00";
        // }
        const updatedPatient = {
            benhNhanId: bnID,
            maBenhNhan: $("#mabn").val(),
            hoTen: $("#hoten").val(),
            email: $("#email").val(),
            soDienThoai: $("#sdt").val() || null,
            loaiGioiTinh: genderValue,
            ngaySinh: $("#ngaysinh").val() ? $("#ngaysinh").val() + "T00:00:00" : null,
            diaChi: $("#diachi").val() || null,
            tienSuBenhLy: $("#tiensubenhly").val() || null,
        };

        console.log("Dữ liệu cập nhật:", updatedPatient);

        // Gửi yêu cầu cập nhật tới API
        axiosJWT
            .put(`/api/Patients/${updatedPatient.benhNhanId}`, updatedPatient)
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
        const benhNhanId = $(this).closest("tr").attr("bn-id");
        bnID = benhNhanId;
        console.log(bnID);
        const benhNhan = dsBN.find((bn) => bn.benhNhanId === benhNhanId); // Tìm bệnh nhân trong danh sách
    });
    // $("#confirm-delete").click(function(){
    //     const benhNhanId = $(this).closest("tr").attr("bn-id");
    //     bnID = benhNhanId;
    //     console.log(bnID);
    //     const benhNhan = dsBN.find((bn) => bn.benhNhanId === benhNhanId); // Tìm bệnh nhân trong danh sách

    // });

    $("#btnDelete").click(function () {
        axiosJWT
            .delete(`/api/Patients/${bnID}`)
            .then(function (response) {
                console.log("Xóa thành công:", response.data);
                getData(); // Tải lại dữ liệu sau khi cập nhật
            })
            .catch(function (error) {
                showErrorPopup();
                console.error("Lỗi khi xóa:", error);
            });
    });
});

function getData() {
    // var userId = localStorage.getItem("userId");
    // $('#hotenHeader').text(localStorage.getItem(loggedInUsername));
    axiosJWT
        .get(`/api/Patients`)
        .then(function (response) {
            dsBN = response.data;
            console.log(dsBN);
            display(dsBN);
        })
        .catch(function (error) {
            console.error("Lỗi không tìm được:", error);
        });
}

function display(data) {
    const tableBody = document.querySelector('#tblBenhNhan tbody');
    tableBody.innerHTML = ''; // Xóa nội dung cũ nếu có

    data.forEach((item, index) => {
        const row = `
      <tr bn-id="${item.benhNhanId}">
        <td class="chk">
          <input type="checkbox" />
        </td>
        <td class="m-data-left">${index + 1}</td>
        <td class="m-data-left">${item.maBenhNhan}</td>
        <td class="m-data-left">${item.hoTen}</td>
        <td class="m-data-left">${item.gioiTinh || "Không xác định"}</td>
        <td class="m-data-left">${formatDate(item.ngaySinh)}</td>
        <td class="m-data-left">${item.email || "Chưa có email"}</td>
        <td class="m-data-left">${item.diaChi || "Chưa có địa chỉ"}</td>
        <td>
                  <div class="m-table-tool">
                    <div class="m-edit m-tool-icon" data-bs-toggle="modal" data-bs-target="#edit-patient" data-id="${item.benhNhanId}">
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

// Hàm điền thông tin vào modal
function fillEditModal(benhNhan) {
    // Gán dữ liệu vào các trường input của modal
    $("#mabn").val(benhNhan.maBenhNhan); // Mã bệnh nhân
    $("#hoten").val(benhNhan.hoTen); // Họ tên
    $("#email").val(benhNhan.email || ""); // Email
    $("#sdt").val(benhNhan.soDienThoai); // Số điện thoại
    // Gán giới tính
    const genderValue = benhNhan.gioiTinh === "Nam" ? 0 : benhNhan.gioiTinh === "Nữ" ? 1 : 2;
    $("#gender-edit").val(genderValue);

    // // Gán ngày sinh
    // const formattedDate = benhNhan.ngaySinh
    //     ? new Date(benhNhan.ngaySinh).toISOString().split("T")[0]
    //     : "";
    // $("#ngaysinh").val(formattedDate);
    const formattedDate = benhNhan.ngaySinh
        ? new Date(benhNhan.ngaySinh).toLocaleDateString('en-CA') // Định dạng YYYY-MM-DD theo múi giờ cục bộ
        : "";
    $("#ngaysinh").val(formattedDate);

    // Gán địa chỉ
    $("#diachi").val(benhNhan.diaChi || "");

    // Gán tiền sử bệnh lý
    $("#tiensubenhly").val(benhNhan.tienSuBenhLy || "");
}

function formatDate(dateString) {
    if (!dateString) return "Không xác định";
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
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

// Hàm lấy mã bệnh nhân lớn nhất
function getMaxBenhNhanCode(dsBN) {
    let maxCode = 0;
    dsBN.forEach(item => {
        const code = parseInt(item.maBenhNhan.replace('BN', '')); // Loại bỏ 'BN' và chuyển thành số
        if (code > maxCode) {
            maxCode = code;
        }
    });
    const nextCode = maxCode + 1;
    return 'BN' + nextCode.toString().padStart(3, '0');
}

function showSuccessPopup() {
    // Hiển thị popup
    const popup = document.getElementById("success-popup");
    popup.style.visibility = "visible";  // Hoặc có thể dùng popup.classList.add('visible');

    // Tự động ẩn popup sau 3 giây (3000ms)
    setTimeout(() => {
        closePopup();
    }, 3000);
}

function closePopup() {
    const popup = document.getElementById("success-popup");
    popup.style.visibility = "hidden";  // Ẩn popup
}
