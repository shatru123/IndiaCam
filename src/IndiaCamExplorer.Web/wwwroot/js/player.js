// IndiaCam Explorer - Video & Snapshot Player Engine

(function () {
  'use strict';

  window.initIndiaCamPlayer = function (config) {
    const {
      cameraId,
      streamUrl,
      streamType, // 'Hls', 'OfficialEmbed', 'Snapshot', 'Direct'
      containerId = 'player-viewport',
      isOffline = false
    } = config;

    const container = document.getElementById(containerId);
    if (!container) return;

    if (isOffline) {
      renderOfflineBanner(container, "This camera is currently marked offline by automated health checks.");
      return;
    }

    if (streamType === 'Hls') {
      initHlsPlayer(container, streamUrl);
    } else if (streamType === 'OfficialEmbed') {
      initEmbedPlayer(container, streamUrl);
    } else if (streamType === 'Snapshot') {
      initSnapshotPlayer(container, streamUrl);
    } else {
      initDirectVideoPlayer(container, streamUrl);
    }

    initPlayerControls(container);
    initReportStreamModal(cameraId);
  };

  // 1. HLS Player via Hls.js
  function initHlsPlayer(container, streamUrl) {
    container.innerHTML = `
      <video id="live-video-element" class="player-video" controls autoplay playsinline muted></video>
    `;
    const video = container.querySelector('#live-video-element');

    if (window.Hls && window.Hls.isSupported()) {
      const hls = new window.Hls({
        enableWorker: true,
        lowLatencyMode: true,
        backBufferLength: 60
      });
      hls.loadSource(streamUrl);
      hls.attachMedia(video);
      hls.on(window.Hls.Events.MANIFEST_PARSED, () => {
        video.play().catch(e => console.log('Autoplay deferred:', e));
      });
      hls.on(window.Hls.Events.ERROR, (event, data) => {
        if (data.fatal) {
          console.warn('Fatal HLS error, attempting recovery:', data);
          switch (data.type) {
            case window.Hls.ErrorTypes.NETWORK_ERROR:
              hls.startLoad();
              break;
            case window.Hls.ErrorTypes.MEDIA_ERROR:
              hls.recoverMediaError();
              break;
            default:
              hls.destroy();
              renderOfflineBanner(container, "Live stream stream playback failed. The upstream broadcaster may be offline.");
              break;
          }
        }
      });
    } else if (video.canPlayType('application/vnd.apple.mpegurl')) {
      // Native Safari HLS
      video.src = streamUrl;
      video.addEventListener('loadedmetadata', () => {
        video.play().catch(e => console.log('Autoplay deferred:', e));
      });
      video.addEventListener('error', () => {
        renderOfflineBanner(container, "Native stream playback error. Broadcaster feed might be temporarily unreachable.");
      });
    } else {
      renderOfflineBanner(container, "Your browser does not support HLS stream playback.");
    }
  }

  // 2. Official Embed Player (YouTube / Official Iframe)
  function initEmbedPlayer(container, streamUrl) {
    let embedSrc = streamUrl;
    // Ensure autoplay & origin flags where beneficial
    if (embedSrc.includes('youtube.com/embed') && !embedSrc.includes('autoplay=')) {
      const separator = embedSrc.includes('?') ? '&' : '?';
      embedSrc += `${separator}autoplay=1&mute=1&playsinline=1&rel=0`;
    }

    container.innerHTML = `
      <iframe class="player-iframe" 
              src="${embedSrc}" 
              title="Live Stream Broadcast"
              allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" 
              allowfullscreen>
      </iframe>
    `;
  }

  // 3. Snapshot Player (Refreshing image with progress countdown)
  function initSnapshotPlayer(container, streamUrl) {
    let intervalSeconds = 15;
    let secondsLeft = intervalSeconds;
    let timer = null;

    container.innerHTML = `
      <div class="position-relative w-100 h-100">
        <img id="snapshot-img" class="player-snapshot" src="${getCacheBustedUrl(streamUrl)}" alt="Live Snapshot Feed" />
        <div id="snapshot-progress" class="snapshot-progress-bar" style="width: 100%;"></div>
        <div class="position-absolute bottom-0 start-0 m-3 d-flex align-items-center gap-2" style="z-index: 5;">
          <span class="badge bg-dark bg-opacity-75 text-cyan border border-cyan-subtle">
            <i class="bi bi-clock-history me-1"></i> Refreshing in <span id="snapshot-countdown">${secondsLeft}</span>s
          </span>
          <button id="btn-force-refresh" class="btn btn-sm btn-surface" title="Refresh Now">
            <i class="bi bi-arrow-repeat"></i>
          </button>
        </div>
      </div>
    `;

    const img = container.querySelector('#snapshot-img');
    const countdownEl = container.querySelector('#snapshot-countdown');
    const progressEl = container.querySelector('#snapshot-progress');
    const refreshBtn = container.querySelector('#btn-force-refresh');

    function refreshImage() {
      img.src = getCacheBustedUrl(streamUrl);
      secondsLeft = intervalSeconds;
      if (progressEl) progressEl.style.width = '100%';
      if (countdownEl) countdownEl.textContent = secondsLeft;
    }

    refreshBtn.addEventListener('click', refreshImage);

    timer = setInterval(() => {
      secondsLeft--;
      if (secondsLeft <= 0) {
        refreshImage();
      } else {
        if (countdownEl) countdownEl.textContent = secondsLeft;
        if (progressEl) {
          const pct = (secondsLeft / intervalSeconds) * 100;
          progressEl.style.width = `${pct}%`;
        }
      }
    }, 1000);
  }

  function getCacheBustedUrl(url) {
    const separator = url.includes('?') ? '&' : '?';
    return `${url}${separator}_t=${Date.now()}`;
  }

  // 4. Direct HTML5 Video
  function initDirectVideoPlayer(container, streamUrl) {
    container.innerHTML = `
      <video class="player-video" controls autoplay playsinline muted src="${streamUrl}"></video>
    `;
  }

  // 5. Offline Diagnostic Banner
  function renderOfflineBanner(container, message) {
    container.innerHTML = `
      <div class="player-overlay-offline">
        <div class="text-danger mb-3" style="font-size: 3rem;">
          <i class="bi bi-broadcast-pin"></i>
        </div>
        <h4 class="fw-bold mb-2">Stream Currently Unreachable</h4>
        <p class="text-secondary max-w-500 mb-4">${message}</p>
        <div class="d-flex gap-2">
          <button class="btn btn-outline-cyan btn-sm" onclick="location.reload()">
            <i class="bi bi-arrow-clockwise me-1"></i> Retry Stream
          </button>
          <button class="btn btn-surface btn-sm" data-bs-toggle="modal" data-bs-target="#reportStreamModal">
            <i class="bi bi-flag me-1"></i> Report Broken Feed
          </button>
        </div>
      </div>
    `;
  }

  // 6. Theater Mode & Fullscreen Controls
  function initPlayerControls(container) {
    const theaterBtn = document.getElementById('btn-theater-mode');
    const fullscreenBtn = document.getElementById('btn-fullscreen');

    if (theaterBtn) {
      theaterBtn.addEventListener('click', () => {
        const layoutWrapper = document.getElementById('player-layout-wrapper');
        if (layoutWrapper) {
          layoutWrapper.classList.toggle('container-fluid');
          layoutWrapper.classList.toggle('container');
          theaterBtn.classList.toggle('active');
          window.showToast('Toggled theater mode', 'info');
        }
      });
    }

    if (fullscreenBtn) {
      fullscreenBtn.addEventListener('click', () => {
        if (!document.fullscreenElement) {
          container.requestFullscreen().catch(err => {
            console.warn('Error requesting fullscreen:', err);
          });
        } else {
          document.exitFullscreen();
        }
      });
    }
  }

  // 7. Broken Stream Report Modal
  function initReportStreamModal(cameraId) {
    const form = document.getElementById('report-stream-form');
    if (!form) return;

    form.addEventListener('submit', async (e) => {
      e.preventDefault();
      const submitBtn = form.querySelector('button[type="submit"]');
      submitBtn.disabled = true;
      submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span> Submitting...';

      const reason = form.querySelector('[name="Reason"]').value;
      const notes = form.querySelector('[name="Notes"]').value;

      try {
        const res = await fetch(`/api/cameras/${cameraId}/report`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({
            cameraId: Number(cameraId),
            reason,
            notes
          })
        });

        if (res.ok) {
          window.showToast('Thank you! Your report was submitted to administrators.', 'success');
          const modalEl = document.getElementById('reportStreamModal');
          if (modalEl && window.bootstrap) {
            const modal = window.bootstrap.Modal.getInstance(modalEl);
            if (modal) modal.hide();
          }
          form.reset();
        } else {
          window.showToast('Failed to submit report. Please try again.', 'error');
        }
      } catch (err) {
        window.showToast('Network error submitting report.', 'error');
      } finally {
        submitBtn.disabled = false;
        submitBtn.innerHTML = '<i class="bi bi-send me-1"></i> Submit Report';
      }
    });
  }

})();
