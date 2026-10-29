// draggable.js
window.initializeDraggable = function (componentId, initialX, initialY) {
    const draggableElement = document.getElementById(`draggable-${componentId}`);
    const header = document.getElementById(`draggable-header-${componentId}`);
    if (!draggableElement || !header) return;

    let startX = 0, startY = 0;
    let baseLeft = 0, baseTop = 0;
    let elWidth = 0, elHeight = 0;
    let dx = 0, dy = 0;
    let rafId = null;

    header.onmousedown = dragMouseDown;

    function dragMouseDown(e) {
        if (draggableElement.dataset.maximized === "true") return; // no dragging while maximized

        e = e || window.event;

        // Don't start a drag if the mousedown originated on a button (or anything inside
        // one) within the header — otherwise clicking a header button (e.g. close, or
        // "Accounts AI Summary") would also kick off a drag, since mousedown bubbles up
        // from the button to the header.
        if (e.target.closest('button')) {
            return;
        }

        e.preventDefault();

        const rect = draggableElement.getBoundingClientRect();
        draggableElement.style.left = rect.left + "px";
        draggableElement.style.top = rect.top + "px";
        draggableElement.style.transform = "none";

        baseLeft = rect.left;
        baseTop = rect.top;
        elWidth = draggableElement.offsetWidth;
        elHeight = draggableElement.offsetHeight;
        dx = 0;
        dy = 0;

        startX = e.clientX;
        startY = e.clientY;

        document.onmouseup = closeDragElement;
        document.onmousemove = elementDrag;

        draggableElement.style.zIndex = '9999';
        draggableElement.style.willChange = 'transform';
        document.body.style.userSelect = 'none';
    }

    function elementDrag(e) {
        e = e || window.event;
        e.preventDefault();

        dx = e.clientX - startX;
        dy = e.clientY - startY;

        if (rafId === null) {
            rafId = requestAnimationFrame(applyDrag);
        }
    }

    function applyDrag() {
        rafId = null;

        let newLeft = baseLeft + dx;
        let newTop = baseTop + dy;

        const maxX = window.innerWidth - elWidth;
        const maxY = window.innerHeight - elHeight;

        newLeft = Math.max(0, Math.min(newLeft, maxX));
        newTop = Math.max(0, Math.min(newTop, maxY));

        draggableElement.style.transform =
            `translate3d(${newLeft - baseLeft}px, ${newTop - baseTop}px, 0)`;
    }

    function closeDragElement() {
        document.onmouseup = null;
        document.onmousemove = null;
        document.body.style.userSelect = '';

        if (rafId !== null) {
            cancelAnimationFrame(rafId);
            rafId = null;
        }

        const rect = draggableElement.getBoundingClientRect();
        draggableElement.style.left = rect.left + "px";
        draggableElement.style.top = rect.top + "px";
        draggableElement.style.transform = "none";
        draggableElement.style.willChange = "auto";

        draggableElement.style.zIndex = '50';
    }
};

window.toggleMaximizeDraggable = function (componentId) {
    const el = document.getElementById(`draggable-${componentId}`);
    if (!el) return false;

    const currentlyMaximized = el.dataset.maximized === "true";

    if (!currentlyMaximized) {
        const rect = el.getBoundingClientRect();
        el.dataset.prevLeft = rect.left + "px";
        el.dataset.prevTop = rect.top + "px";
        el.dataset.prevWidth = rect.width + "px";
        el.dataset.prevHeight = rect.height + "px";
        el.dataset.prevMaxWidth = el.style.maxWidth || "";
        el.dataset.prevMinWidth = el.style.minWidth || "";

        el.style.left = "0px";
        el.style.top = "0px";
        el.style.width = "100vw";
        el.style.height = "100vh";
        el.style.maxWidth = "100vw";
        el.style.minWidth = "0";
        el.style.transform = "none";
        el.style.borderRadius = "0";

        el.dataset.maximized = "true";
        return true;
    } else {
        el.style.left = el.dataset.prevLeft || "";
        el.style.top = el.dataset.prevTop || "";
        el.style.width = el.dataset.prevWidth || "";
        el.style.height = el.dataset.prevHeight || "";
        el.style.maxWidth = el.dataset.prevMaxWidth || "";
        el.style.minWidth = el.dataset.prevMinWidth || "";
        el.style.transform = "none";
        el.style.borderRadius = "";

        el.dataset.maximized = "false";
        return false;
    }
};

window.destroyDraggable = function (componentId) {
    const header = document.getElementById(`draggable-header-${componentId}`);
    if (header) {
        header.onmousedown = null;
    }
    document.onmouseup = null;
    document.onmousemove = null;
    document.body.style.userSelect = '';
};