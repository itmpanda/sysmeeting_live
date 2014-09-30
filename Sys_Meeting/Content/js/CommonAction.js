function openDailog(odiv, title, fn) {

    $(odiv).show().dialog({
        modal: true,
        width: 500,
        height: 400,
        title: title,
        //                    href: '/meet/new/',
        cache: false,
        collapsible: false,
        minimizable: false,
        maximized: false,
        resizable: true,
        toolbar: [
            {
                text: '保存',
                iconCls: 'icon-ok',
                handler: function() {
                    fn('save');
                }
            }, '-', {
                text: '關閉',
                iconCls: 'icon-no',
                handler: function() {
                    fn('close');
                }
            }
        ]
    });
}

//返回值為表格的選擇行，數組類型
//function OpenSearchDailog(o, div, d, url, t, fn) {
function OpenSearchDailog(odiv,title,url,fn) {
    //                curOpenSearchName = '#' + o;
    $(odiv).show().dialog({
        modal: true,
        width: 500,
        height: 400,
        title: title, //'會議內容',
        href: url, // '/account/search/',
        cache: false,
        collapsible: false,
        minimizable: false,
        maximized: false,
        resizable: true,
        toolbar: [
            {
                text: '選擇',
                iconCls: 'icon-ok',
                handler: function() {
                    fn('select');
                }
            }, '-', {
                text: '關閉',
                iconCls: 'icon-no',
                handler: function() {
                    //                    CloseDialog(div);
                    fn('close');
                }
            }
        ]
    });
}


//下拉選擇框
function InitSearchCombox() {
    $('#cc').combo({
        required: true,
        editable: false
    });
    $('#sp').appendTo($('#cc').combo('panel'));
    $('#sp input').click(function() {
        var v = $(this).val();
        var s = $(this).next('span').text();
        $('#cc').combo('setValue', v).combo('setText', s).combo('hidePanel');
    });
}

function PushFieldValue(t, f) {
    var f1 = '', f2 = '';
    if (t == 'list') {
        f1 = 'ListSysId';
        f2 = 'ListName';
    }
    else if (t == 'account') {
        f1 = 'UserId';
        f2 = 'FulName';
    }
    else if (t == 'meet') {
        f1 = 'sysid';
        f2 = 'name';
    }
    if (f == 'id') {
        return f1;
    }
    else if (f == 'name') {
        return f2;
    }
}

function CloseDialog(o) {
    $(o).dialog('close');
}


function Delete(o, u) {
    var row = $(o).datagrid('getSelected');
    if (row) {
        $.messager.confirm('提示', '確定要刪除此記錄', function (data) {
            if (data) {
                var index = $(o).datagrid('getRowIndex', row);
                GetRequest(u + row.sysid, '', function (ret) {
                    if (ret.result > '0') {
                        $.messager.alert('提示', '刪除成功！', 'info', function () {
                            $(o).datagrid('deleteRow', index);//刪除選擇的行
                        });
                    } else {
                        $.messager.alert('提示', ret.errmsg, 'info');
                    }
                });
            }
        });
    } else {
        $.messager.alert('提示', '請選擇要刪除的記錄', 'info');
    }
}



//-->Panels操作
function removeAllPanel(o) {
    $(o + ' .panel .panel-title').each(function() {
        $(o).accordion('remove', $(this).html());
    });
}

//*******************************************
//Panels操作--<