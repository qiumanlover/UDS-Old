function showtime() {
    var today = new Date()
    var year = today.getFullYear()
    var month = today.getMonth()
    var day = today.getDate()
    var h = today.getHours()
    var m = today.getMinutes()
    var s = today.getSeconds()
    m = checkTime(m)
    s = checkTime(s)
    $('#timeshow').text((year + "-" + month + "-" + day + " " + h + ":" + m + ":" + s).toString())
    setTimeout('showtime()', 500)
}
function checkTime(i) {
    if (i < 10)
    { i = "0" + i }
    return i
}

var sh;
function IsLoad() {
    var uid = document.getElementById("uid").innerHTML;
    var guid = document.getElementById("guid").innerHTML;
    ajaxPost("/Login/CheckLoad", "uid=" + uid + "&guid=" + guid, function onsuccess(text) {
        if (text.indexOf("Error") >= 0) {
            clearInterval(sh);
            //document.write(text);
            alert(('当前账号在其他地方登录, 请重新登录!'));
            self.location.href = '/Login/Index';
        }
    }, function onfail(text) {
        clearInterval(sh);
    })
}
sh = setInterval(IsLoad, 3000);

$(document).ready(function () {
    showtime();
    $("a#fevent").click();
});
