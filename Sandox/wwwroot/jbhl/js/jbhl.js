// Scrolls whatever scrolls the Jbhl page (the layout's main element or the window) back to the top.
export function scrollToTop(id) {
    let parent = document.getElementById(id)?.parentElement;
    while (parent) {
        const overflow = getComputedStyle(parent).overflowY;
        if ((overflow === 'auto' || overflow === 'scroll') && parent.scrollHeight > parent.clientHeight) {
            parent.scrollTop = 0;
        }
        parent = parent.parentElement;
    }
    window.scrollTo(0, 0);
}
