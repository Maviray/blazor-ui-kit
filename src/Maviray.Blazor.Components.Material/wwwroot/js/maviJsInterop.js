const _globalOutsideClickRefs = {};

window.registerGlobalOutsideClickListener = (instanceId, dotNetRef, callbackMethod) => {
    _globalOutsideClickRefs[instanceId] = { dotNetRef, callbackMethod };

    if (!window._globalOutsideClickBound) {
        window._globalOutsideClickBound = true;
        document.addEventListener('click', (event) => {

            const target = event?.target;

            if (!(target instanceof HTMLElement)) {
                return;
            }

            if (target.tagName === 'SELECT' || target.tagName === 'OPTION') {
                return;
            }           

            const menuEl = event.target.closest('[data-context-menu-id]');
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