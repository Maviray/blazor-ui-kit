window.coverDropdownOutsideClickHandlers = {};

window.registerOutsideClickCallback = (elementId, dotNetRef, callbackMethod) => {
    try {

        // If already registered → remove old handler first
        const existingHandler = window.coverDropdownOutsideClickHandlers[elementId];
        if (existingHandler) {
            document.removeEventListener("click", existingHandler);
            delete window.coverDropdownOutsideClickHandlers[elementId];
        }

        const handler = (event) => {
            const element = document.getElementById(elementId);           

            if (element && !element.contains(event.target)) {
                dotNetRef.invokeMethodAsync(callbackMethod, elementId);
            }
        };
        document.addEventListener("click", handler);
        window.coverDropdownOutsideClickHandlers[elementId] = handler;
    }
    catch (error) {
        console.log(error);
    }
};

window.unregisterOutsideClickCallback = (elementId) => {
    try {
        const handler = window.coverDropdownOutsideClickHandlers[elementId];
        if (handler) {
            document.removeEventListener("click", handler);
            delete window.coverDropdownOutsideClickHandlers[elementId];
        }
    }
    catch (error) {
        console.log(error);
    }
};