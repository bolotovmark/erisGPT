$(document).ready(function () {





   

    function SendMessage() {
        if ($('.input-text').val() == '') {
            return;
        }


        var messages = [];

        $('.message-user, .message-bot').each(function () {
            if ($(this).hasClass('.message-user')) {
                messages.push({ role: 'user', content: $(this).text()});
            }
            else {
                messages.push({ role: 'assistant', content: $(this).text() });

            }
        });


        $('#sendMessage').html('<div class="spinner-border" role="status"><span class= "sr-only"> Loading...</span ></div >');


        

      
       

        $('.text').append('<div class="container-user"><div class="message-user">' + $('.input-text').val() + '</div><div ></div></div>');
        $('.text').append('<div class="container-bot"><div></div><div class="message-bot">...</div></div>');

        var text = $('.input-text').val();
        $('.input-text').val('');


        $('.input-text').attr('readonly', true);

        //$('#params input[Name="prompt"]').val(text);

        document.getElementById('text').scrollTo(0, $('#text').prop('scrollHeight'));

        startTimer();
        
        $.ajax({
            url: 'Home/SendMessage',
            data: {
                prompt: text,
                messages: JSON.stringify(messages)
            },
            type: 'POST',
            dataType: 'JSON',
            success: function (data) {


                $('#sendMessage').html('Отправить');
                $('.input-text').attr('readonly', false);

               

                stopTimer();

                if (data == '') {
                    $('.text .message-bot').last().html("<span style='color:red;'>Ошибка выполнения запроса к серверу!</span>");
                    document.getElementById('text').scrollTo(0, $('#text').prop('scrollHeight'));
                    return;
                }

                var d = JSON.parse(data);
               
                $('.text .message-bot').last().html(d.response);


                document.getElementById('text').scrollTo(0, $('#text').prop('scrollHeight'));

            }
        });
    }


});







var milisec = 0;
var sec = 0;
var min = 0;
var timer;

function startTimer() {
    timer = setInterval(function () {
        
        milisec++;
        if (milisec == 100) {
            milisec = 0;
            sec++;
            $("#sec").text(sec);
        }
        if (sec == 60) {
            sec = 0;
            min++;
            $("#min").text(min);
        }

        $('.status').text("Время выполнения: " + ("0" + min).slice(-2) + ":" + ("0" + sec).slice(-2) + ":" + ("0" + milisec).slice(-2));

    }, 10);
}


function stopTimer() {
    clearInterval(timer);
    $("#milisec, #sec, #min ").text("0");
    milisec = 0;
    sec = 0;
    min = 0;
    $('.status').text("");
}
