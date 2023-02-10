function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}

function swalNotification(title, text, icon, buttons, dangerMode) {
    return swal({
        title: title,
        text: text,
        icon: icon,
        buttons: buttons,
        dangerMode: dangerMode,
    });
}