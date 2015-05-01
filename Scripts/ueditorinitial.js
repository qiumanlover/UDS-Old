var content = $("input#AttachContent").val();
var editorOption = {
    initialContent: content,
    initialFrameHeight: 300,
    initialFrameWidth: 880,
    autoHeightEnabled: false,
};
var editor = new baidu.editor.ui.Editor(editorOption);
editor.render('editor');
$('#save').click(function () {
    $('#AttachContent').val(editor.getContent());
});