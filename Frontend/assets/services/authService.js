$(document).ready(function () {
    // $('#loginForm').on('submit', function (e) {
    //     e.preventDefault(); // Ngăn chặn form tự động submit
    //     // Lấy thông tin đăng nhập từ form
    //     const username = $("#username").val();
    //     const password = $("#password").val();

    //     // Gửi request đăng nhập
    //     axiosJWT
    //         .post("/api/Auth/login", {
    //             username: username,
    //             password: password,
    //         })
    //         .then(function (response) {
    //             // Lưu accessToken và refreshToken vào localStorage
    //             const { accessToken, refreshToken } = response.data;

    //             localStorage.setItem("accessToken", accessToken);
    //             localStorage.setItem("refreshToken", refreshToken);
    //             localStorage.setItem("userName", username);
    //             getUserId(username);
    //             // localStorage.setItem("expiration", expiration);
    //             console.log(accessToken);
    //             console.log(refreshToken);
    //             console.log("Đăng nhập thành công, token đã được lưu");
    //             // Tiến hành chuyển hướng hoặc các hành động khác sau khi đăng nhập thành công
    //             let token = accessToken;
    //             var userRole;
    //             if (token) {
    //                 try {
    //                     let decodedToken = jwt_decode(token);
    //                     userRole = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    //                     localStorage.setItem('userName', username);
    //                     console.log(decodedToken); // In ra để kiểm tra thông tin token
    //                 } catch (error) {
    //                     console.error('Token không hợp lệ:', error);
    //                 }
    //             } else {
    //                 console.error('Token rỗng hoặc không hợp lệ.');
    //             }

    //             // Kiểm tra role và chuyển hướng
    //             if (userRole === 'Admin') {
    //                 window.location.href = '/Admin/MainAdmin.html'; // Chuyển hướng admin
    //                 console.log(username);
    //                 $("#displayUser").text(username);
    //             }
    //             else if (userRole === 'Patient') {
    //                 window.location.href = '/User/index.html'; // Chuyển hướng doctor
    //             }
    //             else if (userRole === 'Doctor') {
    //                 window.location.href = '/Doctor/MainDoctor.html';// Chuyển hướng patient
    //             }
    //         })
    //         .catch(function (error) {
    //             // showErrorPopup();
    //             // console.error("Lỗi khi đăng nhập:", error);
    //             if (error.response) {
    //                 if (error.response.status === 400) {
    //                     // Xử lý lỗi 400
    //                     console.error("Lỗi 400: Yêu cầu không hợp lệ", error.response.data);
    //                     showErrorPopup();
    //                 }
    //             } else {
    //                 console.error("Lỗi khi tạo yêu cầu:", error.message);
    //                 alert("Đã xảy ra lỗi không xác định. Vui lòng thử lại.");
    //             }
    //         });
    // });
    $('#loginForm').on('submit', async function (e) {
        e.preventDefault(); // Ngăn chặn form tự động submit
    
        // Lấy thông tin đăng nhập từ form
        const username = $("#username").val();
        const password = $("#password").val();
    
        try {
            // Gửi request đăng nhập
            const response = await axiosJWT.post("/api/Auth/login", {
                username: username,
                password: password,
            });
    
            // Lưu accessToken và refreshToken vào localStorage
            const { accessToken, refreshToken } = response.data;
            localStorage.setItem("accessToken", accessToken);
            localStorage.setItem("refreshToken", refreshToken);
            localStorage.setItem("userName", username);
    
            console.log(accessToken);
            console.log(refreshToken);
            console.log("Đăng nhập thành công, token đã được lưu");
    
            // Lấy User ID từ API sau khi đăng nhập thành công
            const userId = await getUserId(username);  // Dùng await để đợi hàm getUserId
            
    
            let token = accessToken;
            let userRole;
    
            if (token) {
                try {
                    let decodedToken = jwt_decode(token);
                    userRole = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
                    localStorage.setItem('userName', username);
                    console.log(decodedToken); // In ra để kiểm tra thông tin token
                } catch (error) {
                    console.error('Token không hợp lệ:', error);
                }
            } else {
                console.error('Token rỗng hoặc không hợp lệ.');
            }
    
            // Kiểm tra role và chuyển hướng
            if (userRole === 'Admin') {
                window.location.href = '/Admin/MainAdmin.html'; // Chuyển hướng admin
                console.log(username);
                $("#displayUser").text(username);
            } else if (userRole === 'Patient') {
                window.location.href = '/User/index.html'; // Chuyển hướng patient
            } else if (userRole === 'Doctor') {
                window.location.href = '/Doctor/MainDoctor.html'; // Chuyển hướng doctor
            }
        } catch (error) {
            // Xử lý lỗi
            if (error.response) {
                if (error.response.status === 400) {
                    // Xử lý lỗi 400
                    console.error("Lỗi 400: Yêu cầu không hợp lệ", error.response.data);
                    showErrorPopup();
                }
            } else {
                console.error("Lỗi khi tạo yêu cầu:", error.message);
                alert("Đã xảy ra lỗi không xác định. Vui lòng thử lại.");
            }
        }
    });
    
    
    $('#registerForm').on('submit', function (e) {
        e.preventDefault();

        let email = $('#email').val();
        let username = $('#username').val();
        let password = $('#password').val();
        if(!kiemTraMatKhau(password)){

        }

        if (!username || !email || !password) {
            alert("Hãy điền đầy đủ thông tin!");
            return;
        }
        // Gửi request đăng ký
        axiosNoJWT
            .post("/api/Auth/register", {
                username: username,
                email: email,
                password: password
            })
            .then(function (response) {
                if(response.status === 200){
                    console.log('Đăng ký thành công:', response);
                    showSuccessPopup();
                    setTimeout(() => {
                        window.location.href = "login.html";
                    }, 3000);
                    
                }
                else{
                    console.log('Đăng ký thất bại:', response.message);
                }
            })
            .catch(function (error) {
                showErrorPopup();
                console.error("Lỗi khi đăng ký:", error);
            });
    });

    // function getUserId(username) {
    //     axiosNoJWT
    //         .get(`/api/Auth/${username}`)  // Truyền trực tiếp username vào URL
    //         .then(function (response) {
    //             localStorage.setItem("userId", response.data);
    //             console(response.data);
    //         })
    //         .catch(function (error) {
    //             showErrorPopup();
    //             console.error("Lỗi khi gọi API:", error);
    //         });
    // }
    async function getUserId(username) {
        // Giả sử có một API gọi đến backend để lấy ID người dùng
        try {
            const response = await axiosJWT.get(`/api/Auth/${username}`);
            localStorage.setItem("userId", response.data);
            return response.data.userId;  // Trả về userId sau khi nhận được từ API
        } catch (error) {
            console.error("Lỗi khi lấy UserId:", error);
            throw error;  // Ném lỗi để catch ở ngoài
        }
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
});