const _globalOutsideClickRefs = {};

window.registerGlobalOutsideClickListener = (instanceId, dotNetRef, callbackMethod) => {
    _globalOutsideClickRefs[instanceId] = { dotNetRef, callbackMethod };

    if (!window._globalOutsideClickBound) {
        window._globalOutsideClickBound = true;
        document.addEventListener('click', (e) => {

            if (e.target.tagName === 'SELECT' || e.target.tagName === 'OPTION') return;

            const menuEl = e.target.closest('[data-context-menu-id]');
            const clickedId = menuEl?.dataset.contextMenuId ?? '';

            // Defer so Blazor's @onclick handlers run first
            setTimeout(() => {
                for (const ref of Object.values(_globalOutsideClickRefs)) {
                    ref.dotNetRef.invokeMethodAsync(ref.callbackMethod, clickedId);
                }
            }, 0);
        });
    }
};

window.unregisterGlobalOutsideClickListener = (instanceId) => {
    delete _globalOutsideClickRefs[instanceId];
};