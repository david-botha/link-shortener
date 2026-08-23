const form = document.getElementById("form");
const urlInput = document.getElementById("url");
const submit = document.getElementById("submit");
const hint = document.getElementById("hint");
const result = document.getElementById("result");
const short = document.getElementById("short");
const copy = document.getElementById("copy");

// If someone types "example.com" rather than "https://example.com"
function normalise(value) {
    const trimmed = value.trim();
    if (!trimmed) return "";
    return /^[a-z][a-z0-9+.-]*:\/\//i.test(trimmed) ? trimmed : "https://" + trimmed;
}

// Let the browser's own URL parser judge it rather than guessing with a regex
function isHttpUrl(value) {
    let parsed;

    try {
        parsed = new URL(value);
    } catch {
        return false;
    }

    return (parsed.protocol === "http:" || parsed.protocol === "https:") && parsed.hostname.includes(".");
}

function showHint(message) {
    hint.textContent = message;
    hint.hidden = false;
}

urlInput.addEventListener("input", () => (hint.hidden = true));

form.addEventListener("submit", async (e) => {
    e.preventDefault();

    const candidate = normalise(urlInput.value);

    if (!isHttpUrl(candidate)) {
        showHint(candidate ? "That doesn't look like a link — try example.com" : "Paste a link first.");
        urlInput.focus();
        return;
    }

    urlInput.value = candidate;
    hint.hidden = true;
    submit.disabled = true;
    submit.textContent = "Shortening…";

    try {
        const res = await fetch("/api/links", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ originalUrl: candidate })
        });

        if (!res.ok) throw new Error("Request failed: " + res.status);

        const data = await res.json();
        short.textContent = data.shortUrl;
        short.href = data.shortUrl;
        copy.hidden = false;
        result.className = "show";
    } catch {
        short.textContent = "Something went wrong. Try again.";
        short.removeAttribute("href");
        copy.hidden = true;
        result.className = "show error";
    } finally {
        submit.disabled = false;
        submit.textContent = "Shorten";
    }
});

copy.addEventListener("click", async () => {
    await navigator.clipboard.writeText(short.textContent);
    copy.textContent = "Copied";
    setTimeout(() => (copy.textContent = "Copy"), 1500);
});
