
window.onload = function () {
    $('#loading-mask').fadeOut();
}

//作用：
//当点击li栏目菜单时替换颜色及当>5个栏目项时可折叠栏目和查看
var LanMu = $(".lanmu-list");
var lanMuSun = LanMu.children('li');
if ((lanMuSun.size()) > 8) {
    LanMu.children("li:gt(5)").hide();
    $(".listmore").show();
};

$(".listmore").bind("click", function () {
    if (!$(".listmore").hasClass('ListMoreOn')) {
        $(".listmore").addClass('ListMoreOn');
        LanMu.children("li:gt(5)").slideDown();
        $(".listmore").html("折叠栏目 ↑");
    } else {
        $(".listmore").removeClass('ListMoreOn');
        LanMu.children("li:gt(5)").slideUp();
        $(".listmore").html("查看更多 ↓");

    }
});

$(".lanmu-list li a").click(function () {
    $(".lanmu-list li a").removeAttr("class");
    $(this).toggleClass("current");
//    var catgroyid = $(this).attr("catgroyid");
    //var liText=$(this).text();
    //alert(catgroyid);
//    $.ajax(
//        {
//            url: "../../generalHandler/loginproc.ashx",
//            type: "GET",
//            data: "id=" + catgroyid,
//            success: function (data, status) {
//                alert(data);
//                //$("#home").html(data);
//            }
//        });
})