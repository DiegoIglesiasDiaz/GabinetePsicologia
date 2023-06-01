window.BajarScroll = function () {
    //$('#chat-history').scrollTop($('#chat-history').prop('scrollHeight'));    
    $('#chat-history').animate({ scrollTop: $('#chat-history').prop('scrollHeight') }, 1000);
}

