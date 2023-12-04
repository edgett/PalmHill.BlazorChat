function textAreaAdjust(element) {
    element.style.height = "1px";
    var targetHeight = element.scrollHeight;

    if (targetHeight > 500) {
        targetHeight = 500;
    }

    element.style.height = (targetHeight) + "px";
}