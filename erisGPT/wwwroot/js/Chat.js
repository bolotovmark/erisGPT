
let output_text = "";                // Суммируемый ответ модели для обработки разметки MarkDown
let listImgWithMetaData = [];       // Для картинок на страничке
let listImgReplaceMetaData = [];    // Для отправки в GPT модель

// Инициализация парсера разметки
converter = new showdown.Converter();

// Установка конфигурации
converter.setOption('tables', true);
converter.setOption('strikethrough', true);
converter.setOption('tablesHeaderId', true);

//  Форма загрузка вложений
$('#formFileMultiple').on('change', function (event) {

    $('.bottom_content_panel').html("");
    listImgWithMetaData = [];
    listImgReplaceMetaData = [];
    
    const files = event.target.files;
    for (let i = 0; i < files.length; i++) {
        const file = files[i];
        const reader = new FileReader();
        
        reader.onload = function (e) {
            const base64StringWithMetaData = e.target.result;
            const base64StringReplaceMetaData = base64StringWithMetaData.replace(/^data:.*;base64,/, '');

            listImgWithMetaData.push(base64StringWithMetaData);
            listImgReplaceMetaData.push(base64StringReplaceMetaData);

            $('.bottom_content_panel').append(getImgElementHTML(base64StringWithMetaData))
        };
        
        reader.readAsDataURL(file);
    }
});


//  Cформировать html вложения
function getImgElementHTML(base64StringWithMetaData) {
    return '<img class="content" src="' + base64StringWithMetaData +'" alt="img_content"/>';
}


// Вызов формы для загрузки вложений
$('#sendContent').on('click', function () {
    $('#formFileMultiple').click();
});


//  Последовательная генерация ответа
hubConnection.on("Update", (data, isDone) => {
    
    if (isDone) {
        stopTimer();

        $('#sendMessage').html('Отправить');
        $('.input-text').attr('readonly', false);
        
        let html = converter.makeHtml(output_text);
        $('.text .message-bot').last().html(html);

        output_text = "";
        
    }
    else {
        if ($('.text .message-bot').last().html() === '...')
            $('.text .message-bot').last().html('');
        
        output_text = output_text + data;
        $('.text .message-bot').last().html($('.text .message-bot').last().html() + data);
        
        
        let html = converter.makeHtml(output_text);
        $('.text .message-bot').last().html(html);

        document.getElementById('text').scrollTo(0, $('#text').prop('scrollHeight'));
    }
    //if (data == '') {
    //    $('.text .message-bot').last().html("<span style='color:red;'>Ошибка выполнения запроса к серверу!</span>");
    //    document.getElementById('text').scrollTo(0, $('#text').prop('scrollHeight'));
    //    return;
    //}
});


function getUserChatElementHTML() {
    let messageUserElementHTML;
    let messageUserText = $('.input-text').val();
    let messageImg = '';

    for (let i = 0; i < listImgWithMetaData.length; i++) {
        const base64StringWithMetaData = listImgWithMetaData[i];
        let imgHTML = getImgElementHTML(base64StringWithMetaData);
        messageImg += imgHTML
    }
    messageUserElementHTML = '<div class="container-user">' +
                                '<div></div>' +
                                '<div class="message-user">' + messageUserText + '<br>' + messageImg +'</div>' +
                             '</div>'
    
    return messageUserElementHTML
}


function getBotChatElementHTML() {
    return '<div class="container-bot">' +
                '<div class="message-bot">...</div>' +
                '<div></div>' +
           '</div>'
}

function SendMsg() {

    if (($('.input-text').val() === '') ){
        return;
    }
    
    var messages = [];
    $('.message-user, .message-bot').each(function () {
        if ($(this).hasClass('message-user')) {
            messages.push({ role: 'user', content: $(this).text(), images: listImgReplaceMetaData});
        }
        else {
            messages.push({ role: 'assistant', content: $(this).text() });

        }
    });
    $('#sendMessage').html('<div class="spinner-border" role="status"><span class= "sr-only"> Loading...</span ></div >');

    
    $('.text').append(getUserChatElementHTML);
    $('.text').append(getBotChatElementHTML);

    
    var text = $('.input-text').val();
    $('.input-text').val('');
    $('.input-text').attr('readonly', true);

    document.getElementById('text').scrollTo(0, $('#text').prop('scrollHeight'));

    startTimer();
    $('.bottom_content_panel').html("");

    console.log(text)
    // console.log(listImgReplaceMetaData)
    var responseImage = listImgReplaceMetaData;
    listImgWithMetaData = [];
    listImgReplaceMetaData = [];
    
    if (responseImage.length === 0){
        hubConnection.invoke("Send",
            text,
            $('#user-uid').val());
    }
    else {
        hubConnection.invoke("SendImg",
            text,
            JSON.stringify(responseImage),
            $('#user-uid').val());
    }
    
}


$('#model-changer .btn').on('click', function () {
    $('#model-changer .btn').removeClass('btn-secondary');
    $('#model-changer .btn').addClass('btn-outline-secondary');
    $(this).removeClass('btn-outline-secondary');
    $(this).addClass('btn-secondary');
});


$('#sendMessage').on('click', function () {
    SendMsg();
});


$(document).on('keypress', function (e) {
    if (e.which === 13) {
        SendMsg();
    }
});


hubConnection.on("EditOn", (data) => {
});
