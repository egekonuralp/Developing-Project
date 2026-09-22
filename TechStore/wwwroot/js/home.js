document.addEventListener("DOMContentLoaded", () => {
    const countdown = document.querySelector("[data-countdown]");

    if (!countdown) {
        return;
    }

    const endDate = new Date();
    endDate.setDate(endDate.getDate() + 7);

    const updateCountdown = () => {
        const remaining = Math.max(0, endDate.getTime() - Date.now());
        const totalSeconds = Math.floor(remaining / 1000);
        const values = {
            days: Math.floor(totalSeconds / 86400),
            hours: Math.floor((totalSeconds % 86400) / 3600),
            minutes: Math.floor((totalSeconds % 3600) / 60),
            seconds: totalSeconds % 60
        };

        Object.entries(values).forEach(([key, value]) => {
            const element = countdown.querySelector(`[data-${key}]`);
            if (element) {
                element.textContent = String(value).padStart(2, "0");
            }
        });
    };

    updateCountdown();
    window.setInterval(updateCountdown, 1000);
});

document.addEventListener("DOMContentLoaded", () => {
    const carousel = document.querySelector("[data-hero-carousel]");

    if (!carousel) {
        return;
    }

    const track = carousel.querySelector(".hero-carousel-track");
    const slides = Array.from(carousel.querySelectorAll(".hero-carousel-slide"));
    const dots = Array.from(carousel.querySelectorAll("[data-carousel-dot]"));
    const prevBtn = carousel.querySelector("[data-carousel-prev]");
    const nextBtn = carousel.querySelector("[data-carousel-next]");

    if (!track || slides.length <= 1) {
        return;
    }

    let current = Math.max(0, slides.findIndex((slide) => slide.classList.contains("active")));

    const goTo = (index) => {
        const total = slides.length;
        current = ((index % total) + total) % total;

        track.style.transform = `translateX(-${current * (100 / total)}%)`;

        slides.forEach((slide, i) => {
            slide.classList.toggle("active", i === current);
        });

        dots.forEach((dot, i) => {
            dot.classList.toggle("active", i === current);
        });
    };

    prevBtn?.addEventListener("click", () => goTo(current - 1));
    nextBtn?.addEventListener("click", () => goTo(current + 1));

    dots.forEach((dot, i) => {
        dot.addEventListener("click", () => goTo(i));
    });

    goTo(current);
});