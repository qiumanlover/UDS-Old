﻿function changeColor(id) {
    var arrayli = document.getElementById(id).getElementsByTagName('li');
    var bool = true; //奇数行为true
    var oldStyle; //保存原有样式
    for (var i = 0; i < arrayli.length; i++) {
        //各行变色
        if (bool === true) {
            arrayli[i].className = "change";
            bool = false;
        }
        else {
            arrayli[i].className = "";
            bool = true;
        }
        //划过变色
        arrayli[i].onmouseover = function () {
            oldStyle = this.className;
            this.className = "current"
        }
        arrayli[i].onmouseout = function () {
            this.className = oldStyle;
        }
    }
}
window.onload = function () {
    changeColor('list');
}

$("#fevent").click = function () {
    changeColor('list');
}
$("#record").click = function () {
    changeColor('list');
}
$("#apply").click = function () {
    changeColor('list');
}
$("#draft").click = function () {
    changeColor('list');
}