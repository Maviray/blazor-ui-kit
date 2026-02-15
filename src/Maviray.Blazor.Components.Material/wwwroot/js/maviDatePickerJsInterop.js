export function initializeDatePicker(container, dotNetRef) {
    if (!container || !dotNetRef) return;

    // Close calendar when clicking outside
    const clickHandler = (e) => {
        if (!container.contains(e.target)) {
            dotNetRef.invokeMethodAsync('CloseCalendar');
        }
    };

    document.addEventListener('click', clickHandler);

    // Store handler for cleanup
    container._clickHandler = clickHandler;
    container._dotNetRef = dotNetRef;
}

export function clearInputValue(container) {
    if (!container) return;
    const input = container.querySelector('input[type="text"]');
    if (input) {
        input.value = '';
    }
}

export function disposeDatePicker(container) {
    if (container && container._clickHandler) {
        document.removeEventListener('click', container._clickHandler);
        delete container._clickHandler;
        delete container._dotNetRef;
    }
}