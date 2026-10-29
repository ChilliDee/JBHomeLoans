// verticalSplitter.js
window.verticalSplitter = {
    init(container, handle, dotnetRef, minPct, maxPct, splitterPx) {
        const onPointerDown = (e) => {
            if (e.button !== 0) return;
            e.preventDefault();
            handle.setPointerCapture(e.pointerId);
            document.body.style.userSelect = 'none';
            const move = (ev) => {
                const rect = container.getBoundingClientRect();
                const raw = ((ev.clientY - rect.top) / rect.height) * 100;
                const pct = Math.max(minPct, Math.min(maxPct, raw));
                container.style.gridTemplateRows = `${pct}fr ${splitterPx}px ${100 - pct}fr`;
                container._pct = pct;
            };
            const end = () => {
                handle.removeEventListener('pointermove', move);
                handle.removeEventListener('pointerup', end);
                handle.removeEventListener('pointercancel', end);
                document.body.style.userSelect = '';
                dotnetRef.invokeMethodAsync('OnSplitCommit', container._pct ?? 50);
            };
            handle.addEventListener('pointermove', move);
            handle.addEventListener('pointerup', end);
            handle.addEventListener('pointercancel', end);   // mouse leaves window, alt-tab, etc.
        };
        handle.addEventListener('pointerdown', onPointerDown);
        handle._vsDispose = () => handle.removeEventListener('pointerdown', onPointerDown);
    },
    dispose(handle) {
        handle?._vsDispose?.();
    }
};