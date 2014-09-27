function GetRequest(url,jsondata,fn) {
    $.ajax({
        url: url, //'/meetmaintenance/action',
        dataType: 'json',
        data: JSON.stringify(jsondata),
        contentType: 'application/json',
        type: 'post',
        success: function (data) {
            fn(data);
        }
    });

}

function PostRequest(url, jsondata, fn) {
    $.ajax({
        url: url, //'/meetmaintenance/action',
        dataType: 'json',
        data: JSON.stringify(jsondata),
        contentType: 'application/json',
        type: 'post',
        success: function (data) {
            fn(data);
        }
    });

}