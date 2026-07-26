// Part10 - ES module interop
// 纪律：JS 是 escape hatch；每个句柄都有 owner 与 Dispose。

export function getElementHeight(selector) {
    const el = document.querySelector(selector);
    return el ? el.offsetHeight : 0;
}

export function focusTrap(el) {
    if (!el) return () => {};
    const handler = (e) => {
        if (e.key !== 'Tab') return;
        const focusables = el.querySelectorAll('button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])');
        if (focusables.length === 0) return;
        const first = focusables[0];
        const last = focusables[focusables.length - 1];
        if (e.shiftKey && document.activeElement === first) {
            e.preventDefault();
            last.focus();
        } else if (!e.shiftKey && document.activeElement === last) {
            e.preventDefault();
            first.focus();
        }
    };
    el.addEventListener('keydown', handler);
    // 返回 cleanup 函数给 C# 侧 Dispose 时调用
    return () => el.removeEventListener('keydown', handler);
}

export function observeViewport(dotnet, el, componentName) {
    if (!el) return;
    const obs = new IntersectionObserver((entries) => {
        for (const entry of entries) {
            if (entry.isIntersecting) {
                dotnet.invokeMethodAsync('OnVisible', componentName);
            }
        }
    });
    obs.observe(el);
    // 把 observer 挂到元素上，便于 C# 侧清理时再调 disconnect
    el._viewportObserver = obs;
}

export function disconnectObserver(el) {
    if (el && el._viewportObserver) {
        el._viewportObserver.disconnect();
        el._viewportObserver = null;
    }
}
