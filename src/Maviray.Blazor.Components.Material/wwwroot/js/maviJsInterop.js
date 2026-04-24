const _globalOutsideClickRefs = {};

window.registerGlobalOutsideClickListener = (instanceId, dotNetRef, callbackMethod) => {
    _globalOutsideClickRefs[instanceId] = { dotNetRef, callbackMethod };

    if (!window._globalOutsideClickBound) {
        window._globalOutsideClickBound = true;
        document.addEventListener('mousedown', (e) => {
            const menuEl = e.target.closest('[data-context-menu-id]');
            const clickedId = menuEl?.dataset.contextMenuId ?? '';

            for (const ref of Object.values(_globalOutsideClickRefs)) {
                ref.dotNetRef.invokeMethodAsync(ref.callbackMethod, clickedId);
            }
        });
    }
};

window.unregisterGlobalOutsideClickListener = (instanceId) => {
    delete _globalOutsideClickRefs[instanceId];
};