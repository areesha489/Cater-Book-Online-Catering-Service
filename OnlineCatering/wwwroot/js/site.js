(function () {
    'use strict';

    const navbar = document.querySelector('.navbar-main');
    if (navbar) {
        window.addEventListener('scroll', () => {
            navbar.classList.toggle('scrolled', window.scrollY > 50);
        });
    }

    // Counter animation
    function animateCounters(root) {
        (root || document).querySelectorAll('[data-count]').forEach(el => {
            if (el.dataset.animated) return;
            el.dataset.animated = '1';
            const target = parseInt(el.dataset.count, 10);
            const duration = 1800;
            const start = performance.now();
            function update(now) {
                const progress = Math.min((now - start) / duration, 1);
                const eased = 1 - Math.pow(1 - progress, 3);
                el.textContent = Math.floor(eased * target) + (el.dataset.suffix || '');
                if (progress < 1) requestAnimationFrame(update);
            }
            requestAnimationFrame(update);
        });
    }

    // Scroll reveal
    const revealEls = document.querySelectorAll('.reveal, .reveal-up');
    if (revealEls.length > 0) {
        const observer = new IntersectionObserver(entries => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('visible');
                    if (entry.target.querySelector('[data-count]')) animateCounters(entry.target);
                }
            });
        }, { threshold: 0.12 });
        revealEls.forEach(el => observer.observe(el));
    }

    // Parallax floating elements
    const parallaxEls = document.querySelectorAll('[data-parallax]');
    if (parallaxEls.length) {
        window.addEventListener('scroll', () => {
            const scrollY = window.scrollY;
            parallaxEls.forEach(el => {
                const speed = parseFloat(el.dataset.parallax) || 0.1;
                el.style.transform = `translateY(${scrollY * speed * -1}px)`;
            });
        }, { passive: true });
    }

    // Menu slider
    const menuSlider = document.getElementById('menuSlider');
    const menuPrev = document.getElementById('menuPrev');
    const menuNext = document.getElementById('menuNext');
    let menuOffset = 0;

    function slideMenu(dir) {
        if (!menuSlider) return;
        const item = menuSlider.querySelector('.menu-slide-item');
        const step = item ? item.offsetWidth + 20 : 220;
        const max = menuSlider.scrollWidth - menuSlider.parentElement.clientWidth;
        menuOffset = Math.max(0, Math.min(max, menuOffset + dir * step));
        menuSlider.style.transform = `translateX(-${menuOffset}px)`;
    }

    menuPrev?.addEventListener('click', () => slideMenu(-1));
    menuNext?.addEventListener('click', () => slideMenu(1));

    // Menu category filter
    document.querySelectorAll('.menu-tab').forEach(tab => {
        tab.addEventListener('click', () => {
            document.querySelectorAll('.menu-tab').forEach(t => t.classList.remove('active'));
            tab.classList.add('active');
            const filter = tab.dataset.filter;
            document.querySelectorAll('.menu-slide-item').forEach(item => {
                const show = filter === 'All' || item.dataset.category === filter;
                item.style.display = show ? '' : 'none';
            });
            menuOffset = 0;
            if (menuSlider) menuSlider.style.transform = 'translateX(0)';
        });
    });

    // Testimonial slider scroll
    const testSlider = document.getElementById('testimonialSlider');
    document.getElementById('testPrev')?.addEventListener('click', () => {
        testSlider?.scrollBy({ left: -360, behavior: 'smooth' });
    });
    document.getElementById('testNext')?.addEventListener('click', () => {
        testSlider?.scrollBy({ left: 360, behavior: 'smooth' });
    });

    // Chef thumbs
    const chefThumbs = document.querySelectorAll('.chef-thumb');
    const chefMainImg = document.getElementById('chefMainImg');
    const chefName = document.getElementById('chefName');
    const chefDesc = document.getElementById('chefDesc');
    chefThumbs.forEach(thumb => {
        thumb.addEventListener('click', () => {
            chefThumbs.forEach(t => t.classList.remove('active'));
            thumb.classList.add('active');
            if (chefMainImg) chefMainImg.src = thumb.dataset.img;
            if (chefName) chefName.textContent = thumb.dataset.name;
            if (chefDesc && thumb.dataset.desc) chefDesc.textContent = thumb.dataset.desc;
        });
    });

    // Back to top
    const backBtn = document.querySelector('.back-to-top');
    if (backBtn) {
        window.addEventListener('scroll', () => {
            backBtn.classList.toggle('show', window.scrollY > 400);
        });
        backBtn.addEventListener('click', () => {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }

    // Active nav link
    const path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.nav-link[data-nav]').forEach(link => {
        if (path.includes(link.dataset.nav)) link.classList.add('active');
    });

    // Auto-animate stats on load if visible
    animateCounters(document.querySelector('.stats-glass')?.closest('section'));
})();
