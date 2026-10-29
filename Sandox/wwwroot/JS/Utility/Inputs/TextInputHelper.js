window.textInputHelper = {
    focusAndSelect: function (element) {
        if (!element) return;
        element.focus();
        element.select();
    }
};
