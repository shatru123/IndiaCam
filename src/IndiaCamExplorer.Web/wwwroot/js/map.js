// IndiaCam Explorer - Interactive Leaflet Map Engine

(function () {
  'use strict';

  let mapInstance = null;
  let tileLayer = null;
  let markerGroup = null;
  let allMarkersData = [];

  const TILES = {
    dark: 'https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png',
    light: 'https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png'
  };

  const ATTRIBUTION = '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>';

  window.initIndiaMap = async function (containerId = 'india-map-container') {
    const el = document.getElementById(containerId);
    if (!el || !window.L) return;

    const currentTheme = document.documentElement.getAttribute('data-theme') || 'dark';
    const tileUrl = currentTheme === 'light' ? TILES.light : TILES.dark;

    mapInstance = L.map(containerId, {
      center: [22.3511, 78.6677], // India center
      zoom: 5,
      minZoom: 4,
      maxZoom: 18,
      scrollWheelZoom: true
    });

    tileLayer = L.tileLayer(tileUrl, {
      attribution: ATTRIBUTION,
      subdomains: 'abcd',
      maxZoom: 19
    }).addTo(mapInstance);

    markerGroup = L.layerGroup().addTo(mapInstance);

    // Listen for theme switch to swap map tiles
    window.addEventListener('indiacam:themechanged', (e) => {
      const theme = e.detail?.theme || 'dark';
      if (tileLayer && mapInstance) {
        mapInstance.removeLayer(tileLayer);
        tileLayer = L.tileLayer(theme === 'light' ? TILES.light : TILES.dark, {
          attribution: ATTRIBUTION,
          subdomains: 'abcd',
          maxZoom: 19
        }).addTo(mapInstance);
      }
    });

    // Fetch markers from API
    try {
      const res = await fetch('/api/map/markers');
      if (res.ok) {
        allMarkersData = await res.json();
        renderMarkers(allMarkersData);
      }
    } catch (err) {
      console.error('Error fetching map markers:', err);
    }

    initFilterToolbar();
  };

  function createCustomIcon(marker) {
    const isOnline = marker.healthStatus === 1; // HealthStatus.Online
    const dotClass = isOnline ? 'bg-success' : 'bg-danger';

    return L.divIcon({
      className: 'custom-map-pin',
      html: `
        <div style="position: relative; width: 32px; height: 32px; background: #070b14; border: 2px solid #00f0ff; border-radius: 50%; display: flex; align-items: center; justify-content: center; box-shadow: 0 0 10px rgba(0, 240, 255, 0.4);">
          <i class="bi bi-camera-video-fill" style="color: #00f0ff; font-size: 14px;"></i>
          <span style="position: absolute; top: -2px; right: -2px; width: 8px; height: 8px; border-radius: 50%;" class="${dotClass}"></span>
        </div>
      `,
      iconSize: [32, 32],
      iconAnchor: [16, 16],
      popupAnchor: [0, -18]
    });
  }

  function renderMarkers(items) {
    if (!markerGroup) return;
    markerGroup.clearLayers();

    items.forEach(item => {
      if (!item.latitude || !item.longitude) return;

      const isOnline = item.healthStatus === 1;
      const statusBadge = isOnline
        ? '<span class="badge bg-success-subtle text-success border border-success-subtle">ONLINE</span>'
        : '<span class="badge bg-danger-subtle text-danger border border-danger-subtle">OFFLINE</span>';

      const thumb = item.thumbnailUrl || 'https://images.unsplash.com/photo-1524492412937-b28074a5d7da?w=400&q=80';

      const popupContent = `
        <div class="map-popup-card">
          <img src="${thumb}" class="map-popup-thumb" alt="${item.title}" />
          <div class="d-flex justify-content-between align-items-center mb-1">
            <span class="text-cyan fw-bold" style="font-size: 0.72rem; text-transform: uppercase;">${item.categoryName || 'Camera'}</span>
            ${statusBadge}
          </div>
          <h6 class="fw-bold mb-1 text-truncate" style="color: var(--text-primary);">${item.title}</h6>
          <p class="text-secondary small mb-2"><i class="bi bi-geo-alt me-1"></i>${item.city}, ${item.state}</p>
          <a href="/cameras/${item.slug}" class="btn btn-cyan btn-sm w-100 py-1" style="font-size: 0.8rem;">
            <i class="bi bi-play-circle me-1"></i> Watch Stream
          </a>
        </div>
      `;

      const m = L.marker([item.latitude, item.longitude], {
        icon: createCustomIcon(item)
      }).bindPopup(popupContent);

      markerGroup.addLayer(m);
    });

    const countEl = document.getElementById('map-visible-count');
    if (countEl) countEl.textContent = items.length;
  }

  function initFilterToolbar() {
    const categorySelect = document.getElementById('map-filter-category');
    const stateSelect = document.getElementById('map-filter-state');
    const statusSelect = document.getElementById('map-filter-status');
    const resetBtn = document.getElementById('map-btn-reset');

    function applyFilters() {
      const cat = categorySelect?.value || '';
      const state = stateSelect?.value || '';
      const status = statusSelect?.value || '';

      const filtered = allMarkersData.filter(m => {
        if (cat && m.categoryName !== cat) return false;
        if (state && m.state !== state) return false;
        if (status === 'online' && m.healthStatus !== 1) return false;
        if (status === 'offline' && m.healthStatus === 1) return false;
        return true;
      });

      renderMarkers(filtered);

      if (filtered.length > 0 && mapInstance) {
        const bounds = L.latLngBounds(filtered.map(f => [f.latitude, f.longitude]));
        mapInstance.fitBounds(bounds, { padding: [50, 50], maxZoom: 10 });
      }
    }

    categorySelect?.addEventListener('change', applyFilters);
    stateSelect?.addEventListener('change', applyFilters);
    statusSelect?.addEventListener('change', applyFilters);

    resetBtn?.addEventListener('click', () => {
      if (categorySelect) categorySelect.value = '';
      if (stateSelect) stateSelect.value = '';
      if (statusSelect) statusSelect.value = '';
      renderMarkers(allMarkersData);
      mapInstance?.setView([22.3511, 78.6677], 5);
    });
  }

})();
