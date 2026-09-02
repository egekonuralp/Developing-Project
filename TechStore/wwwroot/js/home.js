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
