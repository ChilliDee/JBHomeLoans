window.conversationInfiniteScrollRegister = (elementId, dotNetRef, thresholdPx) => {
    window.conversationInfiniteScrollUnregister(elementId);

    const el = document.getElementById(elementId);
    if (!el) return;

    const handler = () => {
        if (el.scrollTop + el.clientHeight >= el.scrollHeight - (thresholdPx || 150)) {
            dotNetRef.invokeMethodAsync('OnScrolledNearBottom');
        }
    };

    window._conversationInfiniteScrollHandlers = window._conversationInfiniteScrollHandlers || {};
    window._conversationInfiniteScrollHandlers[elementId] = { el, handler };
    el.addEventListener('scroll', handler);
};

window.conversationInfiniteScrollUnregister = (elementId) => {
    const registry = window._conversationInfiniteScrollHandlers;
    const entry = registry && registry[elementId];
    if (entry) {
        entry.el.removeEventListener('scroll', entry.handler);
        delete registry[elementId];
    }
};

// A short page (or a narrow search result) can leave the container without enough content to
// overflow at all, in which case the 'scroll' event conversationInfiniteScrollRegister relies on
// never fires and infinite scroll stalls on page 1. Called from Blazor on every render while a
// container is mounted and HasMore is true, so a short page still pulls in the next one automatically.
window.conversationInfiniteScrollCheckFill = (elementId) => {
    const entry = window._conversationInfiniteScrollHandlers && window._conversationInfiniteScrollHandlers[elementId];
    if (entry) entry.handler();
};

// Keeps a conversation row visible after the list re-sorts (e.g. read/unread toggles
// moving it in the unread-first ordering) - no-ops if the row isn't currently rendered
// (wrong tab, filtered out by search, etc).
window.conversationScrollItemIntoView = (elementId) => {
    const el = document.getElementById(elementId);
    if (el) {
        el.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }
};
