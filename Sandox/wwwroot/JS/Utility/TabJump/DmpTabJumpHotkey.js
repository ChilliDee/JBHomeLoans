window.dmpTabJumpRegisterHotkey = (dotNetRef) => {
    window.dmpTabJumpUnregisterHotkey();

    window._dmpTabJumpHandler = (e) => {
        if (e.altKey && !e.ctrlKey && !e.metaKey && !e.shiftKey && e.key.toLowerCase() === 's') {
            e.preventDefault();
            dotNetRef.invokeMethodAsync('OpenFromHotkey');
        }
    };

    document.addEventListener('keydown', window._dmpTabJumpHandler, true);
};

window.dmpTabJumpUnregisterHotkey = () => {
    if (window._dmpTabJumpHandler) {
        document.removeEventListener('keydown', window._dmpTabJumpHandler, true);
        delete window._dmpTabJumpHandler;
    }
};

window.dmpTabJumpFocusElementById = (id) => {
    const el = document.getElementById(id);
    if (el) el.focus();
};
