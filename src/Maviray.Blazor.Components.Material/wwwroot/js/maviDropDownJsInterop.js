window.maviDropdownInit = (dotNetRef, containerElement) => {
    if (!containerElement) return;

    const handleClickOutside = (event) => {
        if (!containerElement.contains(event.target)) {
            dotNetRef.invokeMethodAsync('CloseFromJs');
        }
    };

    // Add click listener to document
    document.addEventListener('click', handleClickOutside, true);

    // Store cleanup function
    containerElement._maviDropdownCleanup = () => {
        document.removeEventListener('click', handleClickOutside, true);
    };
};

// Cleanup on dispose
window.maviDropdownDispose = (containerElement) => {
    if (containerElement && containerElement._maviDropdownCleanup) {
        containerElement._maviDropdownCleanup();
        delete containerElement._maviDropdownCleanup;
    }
};