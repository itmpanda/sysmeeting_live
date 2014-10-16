$(document).delegate('.tab-meetlist tr', 'mouseover', function () {
    $(this).find('td').addClass('tr-over');
});

$(document).delegate('.tab-meetlist tr', 'mouseout', function () {
    $(this).find('td').removeClass('tr-over');
});