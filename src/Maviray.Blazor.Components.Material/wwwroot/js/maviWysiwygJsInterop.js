// Per-surface saved selection range, keyed via a WeakMap.
const _savedRanges = new WeakMap();
let _fullscreenOwner = null;

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
    if (!surface || !dotNetRef) return;
    if (surface._onBlur) { dispose(surface); }
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

// NOTE for callers: toolbar controls that call exec()/format*()/insert*()/set*Color()
// MUST call preventDefault() on their mousedown, so clicking a button does not move
// focus out of the editable surface and collapse the selection before the command
// runs. In Blazor, add @onmousedown:preventDefault to each toolbar button.
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

export function setFontSize(surface, size) {
    // execCommand('fontSize') only accepts the legacy 1-7 scale, so mark the
    // selection with size 7 (styleWithCSS off guarantees a <font size="7">
    // element), then rewrite those to the exact CSS size the caller asked for.
    restoreSelection(surface);
    document.execCommand('styleWithCSS', false, false);
    document.execCommand('fontSize', false, '7');
    surface.querySelectorAll('font[size="7"]').forEach((el) => {
        el.removeAttribute('size');
        el.style.fontSize = size;
    });
    saveSelection(surface);
    return surface.innerHTML;
}

export function setForeColor(surface, color) {
    return exec(surface, 'foreColor', color);
}

export function setBackColor(surface, color) {
    // styleWithCSS makes hiliteColor produce inline style spans reliably.
    document.execCommand('styleWithCSS', false, true);
    try {
        return exec(surface, 'hiliteColor', color);
    } finally {
        document.execCommand('styleWithCSS', false, false);
    }
}

export function insertLink(surface, url, text) {
    restoreSelection(surface);
    const sel = window.getSelection();
    if (sel && !sel.isCollapsed) {
        const before = new Set(surface.querySelectorAll('a'));
        document.execCommand('createLink', false, url);
        surface.querySelectorAll('a').forEach(a => {
            if (!before.has(a)) { a.target = '_blank'; a.rel = 'noopener'; }
        });
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
    rows = Math.max(1, Math.min(50, rows | 0));
    cols = Math.max(1, Math.min(50, cols | 0));
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
    if (surface) {
        surface.innerHTML = html ?? '';
        _savedRanges.delete(surface);
    }
}

export function focus(surface) {
    if (surface) surface.focus();
}

export function toggleFullscreen(surface, on) {
    if (on) {
        _fullscreenOwner = surface;
        document.body.style.overflow = 'hidden';
    } else if (_fullscreenOwner === surface) {
        _fullscreenOwner = null;
        document.body.style.overflow = '';
    }
}

export function dispose(surface) {
    if (!surface) return;
    surface.removeEventListener('blur', surface._onBlur);
    surface.removeEventListener('keyup', surface._onKeyUp);
    surface.removeEventListener('mouseup', surface._onMouseUp);
    delete surface._onBlur;
    delete surface._onKeyUp;
    delete surface._onMouseUp;
    _savedRanges.delete(surface);
    delete surface._dotNetRef;
    if (_fullscreenOwner === surface) {
        _fullscreenOwner = null;
        document.body.style.overflow = '';
    }
}
