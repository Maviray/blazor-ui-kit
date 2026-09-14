// Per-surface saved selection range, keyed via a WeakMap.
const _savedRanges = new WeakMap();

function saveSelection(surface) {
    const sel = window.getSelection();
    if (sel && sel.rangeCount > 0) {
        const range = sel.getRangeAt(0);
        if (surface.contains(range.commonAncestorContainer)) {
            _savedRanges.set(surface, range.cloneRange());
        }
    }
}

function restoreSelection(surface) {
    const range = _savedRanges.get(surface);
    const sel = window.getSelection();
    surface.focus();
    if (range && sel) {
        sel.removeAllRanges();
        sel.addRange(range);
    }
}

export function initialize(surface, dotNetRef) {
    if (!surface) return;
    surface._dotNetRef = dotNetRef;

    surface._onBlur = () => {
        saveSelection(surface);
        dotNetRef?.invokeMethodAsync('OnEditorBlurAsync', surface.innerHTML);
    };
    surface._onKeyUp = () => saveSelection(surface);
    surface._onMouseUp = () => saveSelection(surface);

    surface.addEventListener('blur', surface._onBlur);
    surface.addEventListener('keyup', surface._onKeyUp);
    surface.addEventListener('mouseup', surface._onMouseUp);
}

export function exec(surface, command, value) {
    restoreSelection(surface);
    document.execCommand(command, false, value ?? null);
    saveSelection(surface);
    return surface.innerHTML;
}

export function formatBlock(surface, tag) {
    return exec(surface, 'formatBlock', '<' + tag + '>');
}

export function setFontName(surface, name) {
    return exec(surface, 'fontName', name);
}

export function setForeColor(surface, color) {
    return exec(surface, 'foreColor', color);
}

export function setBackColor(surface, color) {
    // styleWithCSS makes hiliteColor produce inline style spans reliably.
    document.execCommand('styleWithCSS', false, true);
    const html = exec(surface, 'hiliteColor', color);
    document.execCommand('styleWithCSS', false, false);
    return html;
}

export function insertLink(surface, url, text) {
    restoreSelection(surface);
    const sel = window.getSelection();
    if (sel && !sel.isCollapsed) {
        document.execCommand('createLink', false, url);
        // Force target=_blank on the just-created anchor(s).
        const anchor = sel.anchorNode?.parentElement?.closest('a');
        if (anchor) { anchor.target = '_blank'; anchor.rel = 'noopener'; }
    } else {
        const safeText = (text && text.length) ? text : url;
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.target = '_blank';
        anchor.rel = 'noopener';
        anchor.textContent = safeText;
        const range = _savedRanges.get(surface);
        if (range) { range.insertNode(anchor); }
        else { surface.appendChild(anchor); }
    }
    saveSelection(surface);
    return surface.innerHTML;
}

export function insertImage(surface, dataUrl) {
    return exec(surface, 'insertImage', dataUrl);
}

export function insertTable(surface, rows, cols) {
    let html = '<table style="border-collapse:collapse;width:100%">';
    for (let r = 0; r < rows; r++) {
        html += '<tr>';
        for (let c = 0; c < cols; c++) {
            html += '<td style="border:1px solid #ccc;padding:6px;min-width:32px">&#8203;</td>';
        }
        html += '</tr>';
    }
    html += '</table><p>&#8203;</p>';
    return exec(surface, 'insertHTML', html);
}

export function getHtml(surface) {
    return surface ? surface.innerHTML : '';
}

export function getText(surface) {
    return surface ? surface.innerText : '';
}

export function setHtml(surface, html) {
    if (surface) surface.innerHTML = html ?? '';
}

export function focus(surface) {
    if (surface) surface.focus();
}

export function toggleFullscreen(on) {
    document.body.style.overflow = on ? 'hidden' : '';
}

export function dispose(surface) {
    if (!surface) return;
    surface.removeEventListener('blur', surface._onBlur);
    surface.removeEventListener('keyup', surface._onKeyUp);
    surface.removeEventListener('mouseup', surface._onMouseUp);
    _savedRanges.delete(surface);
    delete surface._dotNetRef;
}
