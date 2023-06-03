window.BajarScroll = function () {
   // $('#chat-history').scrollTop($('#chat-history').prop('scrollHeight'));    
    $('#chat-history').animate({ scrollTop: $('#chat-history').prop('scrollHeight') }, 1000);
}

window.active = function (id) {

    $('button').removeClass('activeButtonChat');
    $('li').removeClass('active');
    $('#NewPersonaLi').remove();
    var string = '*[id*=' + id + ']';
    $(string).parent().addClass('active');
    $(string).removeClass('rz-button rz-primary');
    $(string).addClass('activeButtonChat');
   // $('#chat-history').animate({ scrollTop: $('#chat-history').prop('scrollHeight') }, 1000);
   
   
}

window.AddPerson = function (name) {
    $('#NewPersonaLi').remove();
    $('li').removeClass('active');
    $('button').removeClass('activeButtonChat');
    var string = '<li id=NewPersonaLi class="clearfix active"><button style="color:black" type="button" class="rz-button rz-button-md rz-variant-filled rz-primary rz-shade-default BotonSimple activeButtonChat" id="NewPersona" ><span class= "rz-button-box"><div class="about"><div class="name">' + name +'</div></div></span></button ></li>';
    var $cabecera = $('.chat-list');
    $cabecera.prepend($(string));

}

window.RemoveNewChat = function () {
    $('#NewPersonaLi').remove();
}

window.FillPage = function () {
        var height = $(window).height();
        var HeaderHeight = $('.headerDiv').height();
        var FooterHeight = $('.footer').height();
    var pxValue = emToPx(2);
    var value = height - HeaderHeight - FooterHeight - pxValue;
    $('#Idchat').height(value);

    //var chatHeaderHeight = $('.chat-header').height();
    //var chatMessageHeight = $('.chat-message').height();
    //var value2 = value - chatHeaderHeight - chatMessageHeight
    //$('#Idchat-history').height(value2);

}

function emToPx(emValue) {
    var fontSize = parseFloat(getComputedStyle(document.documentElement).fontSize);
    var pxValue = emValue * fontSize;
    return pxValue;
}

