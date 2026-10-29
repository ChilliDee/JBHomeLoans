window.multiSelect = (function () {
    let handler;
    return {
        registerOutsideClick: function (root, dotNetRef) {
            this.deregisterOutsideClick();
            handler = function (e) {
                if (root && !root.contains(e.target)) {
                    dotNetRef.invokeMethodAsync('CloseFromOutside');
                }
            };
            document.addEventListener('mousedown', handler, true);
        },
        deregisterOutsideClick: function () {
            if (handler) {
                document.removeEventListener('mousedown', handler, true);
                handler = null;
            }
        }
    };
})();