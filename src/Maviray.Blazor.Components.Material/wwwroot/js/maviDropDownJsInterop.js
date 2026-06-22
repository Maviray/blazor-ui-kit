export function initializeDropdown(dotNetRef, containerElement) {
    if (!containerElement) return;

    const handleClickOutside = (event) => {

        const target = event?.target;

        if (!(target instanceof HTMLElement)) {
            return;
        }

        if (target.tagName === 'SELECT' || target.tagName === 'OPTION') {
            return;
        }

        if (!containerElement.contains(target)) {
            dotNetRef.invokeMethodAsync('CloseFromJs');
        }
    };

    // Add click listener to document
    document.addEventListener('click', handleClickOutside, true);

    // Store cleanup function
    containerElement._maviDropdownCleanup = () => {
        document.removeEventListener('click', handleClickOutside, true);
    };
}

export function disposeDropdown(containerElement) {
    if (containerElement && containerElement._maviDropdownCleanup) {
        containerElement._maviDropdownCleanup();
        delete containerElement._maviDropdownCleanup;
    }
}