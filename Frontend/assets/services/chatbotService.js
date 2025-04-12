$(document).ready(function () {
  $("#open-chat").click(function () {
    $("#chatbox").toggle();
  });

  $("#close-chat").click(function () {
    $("#chatbox").hide();
  });

  $("#send-btn").click(function () {
    sendMessage();
  });

  $("#chat-input").keypress(function (event) {
    if (event.which === 13) {
      sendMessage();
    }
  });

  function sendMessage() {
    var userMessage = $("#chat-input").val().trim();
    if (userMessage === "") return;

    $("#chat-body").append(
      '<div class="chat-message user-message">' + userMessage + "</div>"
    );
    $("#chat-input").val("");

    $("#chat-body").append(
      '<div class="chat-message bot-message">Đang suy nghĩ...</div>'
    );
    $("#chat-body").scrollTop($("#chat-body")[0].scrollHeight);
    // Gọi API bằng Axios
    axiosJWT
      .post("/api/v1/Chats/chat", { message: userMessage })
      .then(function (response) {
        // Lấy câu trả lời từ API (giả sử câu trả lời là trong response.data.choices[0].message.content)
        let botReply = response.data; // Cập nhật dữ liệu với cấu trúc đúng
        // Cập nhật tin nhắn bot
        $(".bot-message:last").html(botReply);
        $("#chat-body").scrollTop($("#chat-body")[0].scrollHeight);
      })
      .catch(function (error) {
        // Xử lý lỗi nếu có
        console.error("Lỗi khi gọi API:", error);
        $(".bot-message:last").html("Xin lỗi, chatbot đang gặp lỗi.");
      });
  }
});
