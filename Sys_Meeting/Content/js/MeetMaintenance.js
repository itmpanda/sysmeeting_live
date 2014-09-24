/*<script type="text/javascript">*/
var mode = '';
var panelIdx = 0;

function removePanel() {
    var o = '#meetlistpanels';
    var pp = $(o).accordion('getSelected');
    if (pp) {
        var index = $(o).accordion('getPanelIndex', pp);
//                    alert(index);
        $(o).accordion('remove', index);
    }
}

//            function selectPanel() {
//                var s = $('#txtMtListId').val();
//                alert(s);
//                var b=$('#meetlistpanels').accordion('select',s);
//                alert(b);
//            }

//創建會議
$('#btnCreate').click(function() {
    mode = 'create';

    ClearValue('#add-list-proc input');

    $("#txtId").removeAttr("disabled");
    $('#txtAction').val(mode);

//                if (CheckIsSave()) {
    //var o = new ContainerModel();
    var d='#add-list-proc';
    var u = '/meetmaintenance/action';
    openDailog(d,u, HandlerDone);
//                }
});

//修改
$('#btnEdit').click(function () {
    $("#txtId").attr("disabled", "disabled");
});

//刪除
$('#btnDelete').click(function () {

});

function CheckIsSave() {
    if ($("#txtId").val() == "") {
        $.messager.alert('提示', '請輸入會議編號!', 'info');
        return false;
    } else if ($("#txtName").val() == "") {
        $.messager.alert('提示', '請輸入會議名稱!', 'info');
        return false;
    }
    else if ($('#txtAction').val() == '') {
        $.messager.alert('提示', '非法操作', 'info');
        return false;
    } else {
        return true;
    }
    //return true;
}

function ContainerModel() {
    var self = this;
    self.id = $('#txtId').val();
    self.name = $('#txtName').val();
    self.action = $('#txtAction').val();
    self.sysid = $('#txtSysId').val();
//                console.log(self.id);
//                console.log(self.name);
//                console.log(self.action);
//                console.log(self.sysid);
}

function ClearValue(o) {
    $(o).each(function () {
        $(this).val('');
    });
}

function HandlerDone() {
    ClearValue('#add-list-proc input');
    $('#dg').datagrid('reload');
    $('#add-list-proc').dialog('close');
    alert('HandlerDone');
}

function openDailog(d,u,fn) {
//                var o = new ListModel();

    $(d).show().dialog({
        modal: true,
        width: 500,
        height: 400,
        title: '會議內容',
        //                    href: '/meet/new/',
        cache: false,
        collapsible: false,
        minimizable: false,
        maximized: true,
        resizable: true,
        toolbar: [
            {
                text: '保存',
                iconCls: 'icon-ok',
                handler: function() {
                    if (CheckIsSave()) {
                        var o = new ContainerModel();
                        SaveList(o, u, fn);
                    }
                }
            }, '-', {
                text: '關閉',
                iconCls: 'icon-no',
                handler: function() {
                    fn();
//                                $('#txtId').val('');
//                                $('#txtName').val('');
//                                //                                $.messager.alert('info', mode);
//                                $('#add-list-proc').dialog('close');
                }
            }
        ]
    });
}

/*
@*-->
o:ContainerModel
u:URL
d:Div
dg:DataGrid重載數據使用
fn:保存成功后回調函數
*@*/
//保存會議
function SaveList(o,u,fn) {
//                if (CheckIsSave()) {
//                    var o = new ContainerModel();
    $.ajax({
        url: u,//'/meetmaintenance/action',
        dataType: 'json',
        data: JSON.stringify(o),
        contentType: 'application/json',
        type: 'post',
        success: function (data) {
            //alert(data.result);
            if (data.result <= '0') {
                $.messager.alert('提示', '保存失敗,消息:' + data.errmsg, 'info');
            } else {
                $.messager.alert('提示', '保存成功!', 'info', function () {
                    fn();
//                                    $('#dg').datagrid('reload');
//                                    $(dg).datagrid('reload');
//                                    $(d).dialog('close');
//                                    $('#txtId').val('');
//                                    $('#txtName').val('');
                    //                                $.messager.alert('info', mode);
//                                    $('#add-list-proc').dialog('close');
                });
                //                                $('#dg').datagrid('reload');

            }
        }
    });
//                };
};

//新增面板
//            var panelIdx = 3;
function addPanels(){
//            $('#btnAddPanels').click(function () {
//                $('#btnAddPanels').accordion('add', {
//                    title: 'addpanel:' + panelIdx,
//                    content:'<div style="panding:10px">content'+panelIdx+'</div>'
//                });
//                panelIdx += 1;
    //                alert('addpanels');
    $('#meetlistpanels').accordion('add', {
        title: '事項' + panelIdx,
        content: '<div style="padding:10px"><input type="text" id="listTitle'+panelIdx+'"/>Content' + panelIdx + '</div>'
    });
    panelIdx++;
};

function ListExists() {
    var guid = $('#txtMtListId').val();
    $.ajax({
        url: '/MeetMaintenance/GetGUIDExists',
        data: 'guid=' + $('#txtMtListId').val(),
        type:'get',
        success: function(data) {
            alert(data);
        }
    });
}

function  GetGuid() {
    $.ajax({
        url: '/MeetMaintenance/GetGUID',
        type:'get',
        success:function(data) {
            alert(data);
        }
    });
}


$(function () {
    initWndData();
});

function initWndData() {
    //下拉選擇框
    $('#cc').combo({
        required: true,
        editable: false
    });
    $('#sp').appendTo($('#cc').combo('panel'));
    $('#sp input').click(function () {
        var v = $(this).val();
        var s = $(this).next('span').text();
        $('#cc').combo('setValue', v).combo('setText', s).combo('hidePanel');
    });

    //初始化表格數據
    $('#dg').datagrid({
        url: '/meetmaintenance/getmeet',
        method: 'get',
        singleSelect: true,
        fit: true,
        pagination: true,
        rownumbers: true,
        pageSize: 10,
        pageList: [5, 10, 15, 20],
        remoteSort: false,
        columns: [
            [
                { field: 'sys_id', title: 'sysid', width: 1, hidden: true },
                { field: 'mt_id', title: '會議編號', width: '20%' },
                { field: 'mt_time', title: '會議名稱', width: '15%' },
                { field: 'mt_dte', title: '會議日期', width: '30%' },
                { field: 'mt_time', title: '會議時間', width: '15%' },
                { field: 'addr', title: '會議地點', width: '20%' }
            ]
        ]
    });
};

        