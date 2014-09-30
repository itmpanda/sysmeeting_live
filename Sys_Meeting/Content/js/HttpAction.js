function GetRequest(url,jsondata,fn) {
    $.ajax({
        url: url, //'/meetmaintenance/action',
        dataType: 'json',
        data: JSON.stringify(jsondata),
        contentType: 'application/json',
        type: 'get',
        success: function (data, status) {
//            alert(status);
            if (status == 'success') {
                fn(data);
            }
        },
        error: function () {
            data.result = '0';
            data.errmsg = '網絡錯誤';
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
        },
        error: function () {
            var data = [];
            data.result = '0';
            data.errmsg = '網絡錯誤';
            fn(JSON.stringify(data));
        }
    });

}