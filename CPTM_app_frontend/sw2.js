const CACHE_NAME = 'app-shell-v5';
const OFFLINE_URL = '/menu_cards%20copy.html';
const ASSETS = [
    '/',    
    '/index%20copy.html',
    '/Forms%20copy.html',
    '/menu_cards%20copy.html',
    '/perfil.html',
    '/manifest.json',    
    '/sw2.js',
    'https://unpkg.com/leaflet@1.9.4/dist/leaflet.css',
    'https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'
];

// Install: populate cache
self.addEventListener('install', event => {
    self.skipWaiting();
    event.waitUntil((async () => {
        const cache = await caches.open(CACHE_NAME);
        const toCache = ASSETS.concat([OFFLINE_URL]);
        for (const asset of toCache) {
            try {
                // use Request to allow same-origin checks; avoid failing whole install
                await cache.add(new Request(asset, { cache: 'no-cache' }));
            } catch (err) {
                // warn but don't fail the install for a single resource
                console.warn('sw install: failed to cache', asset, err);
            }
        }
    })());
});

// Activate: cleanup old caches
self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(keys => Promise.all(
            keys.filter(k => k !== CACHE_NAME).map(k => caches.delete(k))
        ))
    );
    self.clients.claim();
});

// Fetch: serve from cache, network fallback, offline fallback
self.addEventListener('fetch', event => {
    const req = event.request;
    // Don't handle non-GET (POST / form submissions) here
    if (req.method !== 'GET') return;

    event.respondWith(
        caches.match(req, { ignoreSearch: true }).then(cached => {
            if (cached) return cached;
            return fetch(req).then(networkResp => {
                // cache cross-origin only if needed; guard opaque responses
                if (networkResp && networkResp.status === 200 && networkResp.type === 'basic') {
                    const clone = networkResp.clone();
                    caches.open(CACHE_NAME).then(cache => cache.put(req, clone));
                }
                return networkResp;
            }).catch(() => caches.match(OFFLINE_URL));
        })
    );
});

// Background Sync: enviar formulários pendentes do IndexedDB
function promisifyRequest(req) {
    return new Promise((res, rej) => { req.onsuccess = () => res(req.result); req.onerror = () => rej(req.error); });
}
function waitForTransactionComplete(tx) {
    return new Promise((res, rej) => { tx.oncomplete = () => res(); tx.onerror = () => rej(tx.error); tx.onabort = () => rej(tx.error); });
}



// (copie aqui sua função enviarFormulariosPendentes já existente)
function openIndexedDB() {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open("FormDB", 1);
        request.onupgradeneeded = event => {
            const db = event.target.result;
            if (!db.objectStoreNames.contains("formularios")) {
                db.createObjectStore("formularios", { autoIncrement: true });
            }
        };
        request.onsuccess = event => resolve(event.target.result);
        request.onerror = event => reject(event.target.error);
    });
}

async function enviarFormulariosPendentes() {
    const db = await openIndexedDB();
    const readTx = db.transaction('formularios', 'readonly');
    const store = readTx.objectStore('formularios');
    const [todos, keys] = await Promise.all([promisifyRequest(store.getAll()), promisifyRequest(store.getAllKeys())]);
    // readTx fecha aqui
    for (let i = 0; i < todos.length; i++) {
        const item = todos[i], key = keys[i];
        const fd = new FormData(); for (const [k, v] of Object.entries(item)) if (!k.startsWith('_')) fd.append(k, v);
        const headers = item._token ? { Authorization: `Bearer ${item._token}` } : {};
        try {
            console.log('SW: tentando enviar key=', key);
            const resp = await fetch('https://localhost:7134/api/enviarForm', { method: 'POST', headers, body: fd });
            if (resp.ok) {
            const delTx = db.transaction('formularios', 'readwrite');
            const delStore = delTx.objectStore('formularios');
            await promisifyRequest(delStore.delete(key));
            await waitForTransactionComplete(delTx);
            console.log('Item enviado e removido, key=', key);
        } else console.error('Envio retornou status não OK', resp.status);
    } catch (e) {
        console.error('Erro de rede ao enviar item, mantendo no DB para retry:', e);
    }}
}

self.addEventListener('sync', event => {
    if (event.tag === 'sync-form') {
        event.waitUntil(enviarFormulariosPendentes());
    }
});

self.addEventListener('message', e => { if (e.data === 'try-sync') e.waitUntil(enviarFormulariosPendentes()); });

