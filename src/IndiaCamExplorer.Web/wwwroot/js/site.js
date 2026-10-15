// IndiaCam Explorer - Core Client Scripts

(function () {
  'use strict';

  // 1. Theme Management
  const STORAGE_KEY_THEME = 'indiacam_theme';
  const themeToggleBtn = document.getElementById('theme-toggle-btn');
  const themeIcon = document.getElementById('theme-toggle-icon');

  function initTheme() {
    const savedTheme = localStorage.getItem(STORAGE_KEY_THEME) || 'dark';
    applyTheme(savedTheme);
  }

  function applyTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem(STORAGE_KEY_THEME, theme);
    if (themeIcon) {
      themeIcon.className = theme === 'dark' ? 'bi bi-moon-stars-fill' : 'bi bi-sun-fill';
    }
    // Dispatch custom event for Leaflet map tile adjustment
    window.dispatchEvent(new CustomEvent('indiacam:themechanged', { detail: { theme } }));
  }

  if (themeToggleBtn) {
    themeToggleBtn.addEventListener('click', () => {
      const current = document.documentElement.getAttribute('data-theme') || 'dark';
      const next = current === 'dark' ? 'light' : 'dark';
      applyTheme(next);
      showToast(`Switched to ${next} mode`, 'info');
    });
  }

  // 2. Favorites Management
  const STORAGE_KEY_FAVORITES = 'indiacam_favorites';

  window.getFavorites = function () {
    try {
      return JSON.parse(localStorage.getItem(STORAGE_KEY_FAVORITES)) || [];
    } catch {
      return [];
    }
  };

  window.isFavorite = function (id) {
    const favs = window.getFavorites();
    return favs.includes(Number(id));
  };

  window.toggleFavorite = function (id, title) {
    const numId = Number(id);
    let favs = window.getFavorites();
    const index = favs.indexOf(numId);
    let added = false;

    if (index > -1) {
      favs.splice(index, 1);
    } else {
      favs.push(numId);
      added = true;
    }

    localStorage.setItem(STORAGE_KEY_FAVORITES, JSON.stringify(favs));
    updateFavoritesBadge();
    updateFavoriteButtons(numId, added);

    if (added) {
      showToast(`Added "${title || 'Camera'}" to favorites`, 'success');
    } else {
      showToast(`Removed "${title || 'Camera'}" from favorites`, 'info');
    }

    // Notify any active page listener (e.g. Favorites page)
    window.dispatchEvent(new CustomEvent('indiacam:favoriteschanged', { detail: { favorites: favs } }));
  };

  function updateFavoritesBadge() {
    const badge = document.getElementById('nav-favorites-count');
    if (badge) {
      const count = window.getFavorites().length;
      badge.textContent = count;
      badge.style.display = count > 0 ? 'inline-flex' : 'none';
    }
  }

  function updateFavoriteButtons(id, isFav) {
    document.querySelectorAll(`.btn-favorite-heart[data-camera-id="${id}"]`).forEach(btn => {
      if (isFav) {
        btn.classList.add('active');
        btn.innerHTML = '<i class="bi bi-heart-fill"></i>';
      } else {
        btn.classList.remove('active');
        btn.innerHTML = '<i class="bi bi-heart"></i>';
      }
    });
  }

  function initFavoriteButtons() {
    const favs = window.getFavorites();
    document.querySelectorAll('.btn-favorite-heart').forEach(btn => {
      const id = Number(btn.getAttribute('data-camera-id'));
      const isFav = favs.includes(id);
      if (isFav) {
        btn.classList.add('active');
        btn.innerHTML = '<i class="bi bi-heart-fill"></i>';
      } else {
        btn.classList.remove('active');
        btn.innerHTML = '<i class="bi bi-heart"></i>';
      }
    });
  }

  // 3. Toast Notifications
  window.showToast = function (message, type = 'info') {
    let container = document.getElementById('app-toast-container');
    if (!container) {
      container = document.createElement('div');
      container.id = 'app-toast-container';
      container.className = 'toast-container-fixed';
      document.body.appendChild(container);
    }

    const toast = document.createElement('div');
    toast.className = 'toast-item';

    let icon = 'bi-info-circle-fill text-info';
    if (type === 'success') icon = 'bi-check-circle-fill text-success';
    if (type === 'warning') icon = 'bi-exclamation-triangle-fill text-warning';
    if (type === 'error') icon = 'bi-x-circle-fill text-danger';

    toast.innerHTML = `
      <i class="bi ${icon} fs-5"></i>
      <div class="flex-grow-1 fs-6">${message}</div>
      <button type="button" class="btn-close btn-close-white ms-auto" aria-label="Close"></button>
    `;

    toast.querySelector('.btn-close').addEventListener('click', () => toast.remove());
    container.appendChild(toast);

    setTimeout(() => {
      if (toast.parentNode) {
        toast.style.transition = 'opacity 0.3s ease';
        toast.style.opacity = '0';
        setTimeout(() => toast.remove(), 300);
      }
    }, 4000);
  };

  // 4. Clipboard helper
  window.copyToClipboard = function (text, successMsg = 'Copied to clipboard!') {
    navigator.clipboard.writeText(text).then(() => {
      showToast(successMsg, 'success');
    }).catch(() => {
      showToast('Could not copy to clipboard', 'warning');
    });
  };

  // 5. DOM Ready init
  document.addEventListener('DOMContentLoaded', () => {
    initTheme();
    updateFavoritesBadge();
    initFavoriteButtons();

    // Event delegation for favorite buttons
    document.body.addEventListener('click', (e) => {
      const favBtn = e.target.closest('.btn-favorite-heart');
      if (favBtn) {
        e.preventDefault();
        e.stopPropagation();
        const id = favBtn.getAttribute('data-camera-id');
        const title = favBtn.getAttribute('data-camera-title');
        window.toggleFavorite(id, title);
      }
    });
  });

})();
