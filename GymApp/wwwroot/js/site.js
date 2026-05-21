const sidebar = document.getElementById("mainSidebar");
const sidebarToggle = document.getElementById("sidebarToggle");

if (sidebar && sidebarToggle) {
	sidebarToggle.addEventListener("click", () => {
		sidebar.classList.toggle("open");
	});
}

const searchInputs = document.querySelectorAll("[data-search-input]");

const buildSearchUrl = (baseUrl, params) => {
	const url = new URL(baseUrl, window.location.origin);
	Object.entries(params).forEach(([key, value]) => {
		if (value !== null && value !== undefined && value !== "") {
			url.searchParams.set(key, value);
		}
	});
	return url.toString();
};

searchInputs.forEach((input) => {
	const targetId = input.dataset.searchTarget;
	const target = targetId ? document.getElementById(targetId) : null;
	const searchUrl = input.dataset.searchUrl;
	const paramName = input.dataset.searchParam || "q";
	const filterName = input.dataset.filterName;
	const filterValue = input.dataset.filterValue;

	if (!target || !searchUrl) {
		return;
	}

	let debounceTimer;
	const runSearch = async () => {
		const params = {
			[paramName]: input.value.trim()
		};

		if (filterName && filterValue) {
			params[filterName] = filterValue;
		}

		try {
			const url = buildSearchUrl(searchUrl, params);
			const response = await fetch(url, {
				headers: {
					"X-Requested-With": "XMLHttpRequest"
				}
			});
			if (!response.ok) {
				return;
			}

			const html = await response.text();
			target.innerHTML = html;
		} catch (error) {
			console.error("Search failed", error);
		}
	};

	input.addEventListener("input", () => {
		window.clearTimeout(debounceTimer);
		debounceTimer = window.setTimeout(runSearch, 300);
	});
});

const autocompleteInputs = document.querySelectorAll("[data-autocomplete-input]");

const closeAllAutocomplete = () => {
	const menus = document.querySelectorAll("[data-autocomplete-menu]");
	menus.forEach((menu) => {
		menu.innerHTML = "";
		menu.classList.remove("open");
	});
};

const createAutocompleteItem = (item, onSelect) => {
	const button = document.createElement("button");
	button.type = "button";
	button.className = "autocomplete-item";
	button.textContent = item.label;
	if (item.meta) {
		const meta = document.createElement("span");
		meta.className = "autocomplete-meta";
		meta.textContent = item.meta;
		button.appendChild(meta);
	}
	button.addEventListener("click", () => onSelect(item));
	return button;
};

autocompleteInputs.forEach((input) => {
	const hiddenId = input.dataset.autocompleteTarget;
	const hidden = hiddenId ? document.getElementById(hiddenId) : null;
	const url = input.dataset.autocompleteUrl;
	const menu = input.closest(".autocomplete")?.querySelector("[data-autocomplete-menu]");

	if (!hidden || !url || !menu) {
		return;
	}

	let debounceTimer;
	const runAutocomplete = async () => {
		const query = input.value.trim();
		if (!query) {
			menu.innerHTML = "";
			menu.classList.remove("open");
			return;
		}

		try {
			const response = await fetch(buildSearchUrl(url, { q: query }), {
				headers: {
					"X-Requested-With": "XMLHttpRequest"
				}
			});
			if (!response.ok) {
				return;
			}

			const items = await response.json();
			menu.innerHTML = "";
			if (!items.length) {
				const empty = document.createElement("div");
				empty.className = "autocomplete-empty";
				empty.textContent = "Nema rezultata";
				menu.appendChild(empty);
				menu.classList.add("open");
				return;
			}

			items.forEach((item) => {
				menu.appendChild(createAutocompleteItem(item, (selected) => {
					hidden.value = selected.id;
					input.value = selected.label;
					menu.innerHTML = "";
					menu.classList.remove("open");
				}));
			});
			menu.classList.add("open");
		} catch (error) {
			console.error("Autocomplete failed", error);
		}
	};

	input.addEventListener("input", () => {
		hidden.value = "";
		window.clearTimeout(debounceTimer);
		debounceTimer = window.setTimeout(runAutocomplete, 250);
	});

	input.addEventListener("focus", () => {
		if (input.value.trim()) {
			runAutocomplete();
		}
	});

	input.addEventListener("blur", () => {
		if (window.jQuery && hidden.classList.contains("validate-hidden")) {
			window.jQuery(hidden).valid();
		}
	});
});

document.addEventListener("click", (event) => {
	if (!event.target.closest(".autocomplete")) {
		closeAllAutocomplete();
	}
});

if (window.jQuery && window.jQuery.validator) {
	window.jQuery.validator.setDefaults({
		ignore: ":hidden:not(.validate-hidden)",
		onfocusout(element) {
			this.element(element);
		}
	});
}

const prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

const animateCount = (element) => {
	const target = Number(element.dataset.count);
	if (Number.isNaN(target)) {
		return;
	}

	const duration = 700;
	const start = performance.now();
	const formatValue = (value) => Math.round(value).toString();

	const tick = (now) => {
		const progress = Math.min((now - start) / duration, 1);
		const value = target * (1 - Math.pow(1 - progress, 3));
		element.textContent = formatValue(value);
		if (progress < 1) {
			requestAnimationFrame(tick);
		}
	};

	requestAnimationFrame(tick);
};

const animateProgress = (element) => {
	const raw = Number(element.dataset.progress);
	if (Number.isNaN(raw)) {
		return;
	}

	const clamped = Math.max(0, Math.min(raw, 100));
	element.style.width = `${clamped}%`;
};

const animateOnView = (entries, observer) => {
	entries.forEach((entry) => {
		if (!entry.isIntersecting) {
			return;
		}

		entry.target.classList.add("in-view");
		const counters = entry.target.querySelectorAll("[data-count]");
		counters.forEach((counter) => {
			if (!counter.dataset.animated) {
				counter.dataset.animated = "true";
				if (prefersReducedMotion) {
					counter.textContent = counter.dataset.count;
				} else {
					animateCount(counter);
				}
			}
		});

		const progressBars = entry.target.querySelectorAll("[data-progress]");
		progressBars.forEach((bar) => {
			if (!bar.dataset.animated) {
				bar.dataset.animated = "true";
				if (!prefersReducedMotion) {
					animateProgress(bar);
				} else {
					bar.style.width = `${bar.dataset.progress}%`;
				}
			}
		});

		observer.unobserve(entry.target);
	});
};

const animatedSections = document.querySelectorAll("[data-animate]");
if (animatedSections.length) {
	if (prefersReducedMotion) {
		animatedSections.forEach((section) => section.classList.add("in-view"));
		const counters = document.querySelectorAll("[data-count]");
		counters.forEach((counter) => {
			counter.textContent = counter.dataset.count;
		});
		const progressBars = document.querySelectorAll("[data-progress]");
		progressBars.forEach((bar) => {
			bar.style.width = `${bar.dataset.progress}%`;
		});
	} else {
		const observer = new IntersectionObserver(animateOnView, {
			threshold: 0.2
		});
		animatedSections.forEach((section) => observer.observe(section));
	}
}
