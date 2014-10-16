function Edit(o) {
    var row = $(o).datagrid('getSelected');
    if (row) {
        $.ajax({
            url: '/meetmaintenance/getmeet/' + row.sysid,
            type: 'get',
            success: function (data) {
                var amasters = [], ajoins = [], aunjoins = [], ashares = [];
                var amasters_f = [], ajoins_f = [], aunjoins_f = [], ashares_f = [];
                var t = [], d = [], d2 = [],d3=[],d4=[],d5=[];
                var ipos = 0;

                t = data.rows;

                for (var i = 0; i < t.length; i++) {
                    var t2 = t[i];
                    d = t2.listmasters;//主席
                    d2 = t2.listjoins;//出席
                    d3 = t2.listunjoins;//缺席
                    d4 = t2.listsharelists;//共享
                    d5 = t2.listitems;//事項列表

                    //主席
                    for (ipos = 0; ipos < d.length; ipos++) {
                        var masterdetail = d[ipos];

                        amasters.push(masterdetail.FulName);
                        amasters_f.push(masterdetail.UserId);
                    }

                    //出席
                    for (ipos = 0; ipos < d2.length; ipos++) {
                        var joindetail = d2[ipos];
                        ajoins.push(joindetail.FulName);
                        ajoins_f.push(joindetail.UserId);
                    }

                    //未出席
                    for (ipos = 0; ipos < d3.length; ipos++) {
                        var unjoindetail = d3[ipos];
                        aunjoins.push(unjoindetail.FulName);
                        aunjoins_f.push(unjoindetail.UserId);
                    }

                    //共享
                    for (ipos = 0; ipos < d4.length; ipos++) {
                        var sharedetail = d4[ipos];
                        ashares.push(sharedetail.FulName);
                        ashares_f.push(sharedetail.UserId);
                    }

                    //事項列表
                    for (ipos = 0; ipos < d5.length; ipos++) {
                        var listdetail = d5[ipos];
                        addPanels('#meetlistpanels', listdetail.title, listdetail.content, listdetail.listsysid);
                    }
                    
                    $('#txtSysId').val(t2.sysid);
                    $('#txtId').val(t2.id);
                    $('#txtDte').datebox('setValue', t2.date); // $('#txtDte').val(t2.id);
                    $('#txtTime').val(t2.time);
                    $('#txtAddr').val(t2.addr);
                    $('#txtName').val(t2.name);

                    $('#txtMaster').val(amasters.join(','));
                    $('#txtMaster_f').val(amasters_f.join(','));

                    $('#txtJoins').val(ajoins.join(','));
                    $('#txtJoins_f').val(ajoins_f.join(','));

                    $('#txtUnJoins').val(aunjoins.join(','));
                    $('#txtUnJoins_f').val(aunjoins_f.join(','));

                    $('#txtShareList').val(ashares.join(','));
                    $('#txtShareList_f').val(ashares_f.join(','));


                }
            }
        });
    } else {
        $.messager.alert('提示', '請選擇要修改的記錄', 'info');
    }

}

//新增面板
function addPanels(o, title, content, id) {
    var idx = 1;
    $(o).accordion('add', {
        title: title,
        content: '<div style="padding:1px;overflow:auto;" id="listdetail"><textarea cols="100" rows="4" data-listsysid="' + id + '">' + content + '</textarea></div>'
        //content: '<div style="padding:1px;overflow:auto;" id="listdetail"><div data-listsysid="' + id + '">' + content + '</div></div>'
    });
    idx++;
};

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

function removeAllPanel(o) {
    $('#meetlistpanels .panel .panel-title').each(function () {
        $(o).accordion('remove', $(this).html());
    });
}