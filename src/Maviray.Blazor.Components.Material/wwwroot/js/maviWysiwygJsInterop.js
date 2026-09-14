export function initialize(surface, dotNetRef) {
    if (!surface) return;
    surface._dotNetRef = dotNetRef;
}

export function setHtml(surface, html) {
    if (surface) surface.innerHTML = html ?? '';
}

export function getHtml(surface) {
    return surface ? surface.innerHTML : '';
}

export function dispose(surface) {
    if (surface) delete surface._dotNetRef;
}
